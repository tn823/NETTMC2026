using System;
using System.Collections.Generic;
using System.IO;
using System.Speech.Synthesis;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Whisper.net;
using Whisper.net.Ggml;

namespace NETTMC.VoiceRecognition
{
    // ════════════════════════════════════════════════════════════════════════
    //  MODELS & EVENTS
    // ════════════════════════════════════════════════════════════════════════
    public class VoiceMatchResult
    {
        public string RecognizedText { get; set; }
        public string MatchedCommand { get; set; }
        public double ConfidenceScore { get; set; }
        public bool IsSuccess => MatchedCommand != null;
    }

    public enum VoiceEngineState
    {
        Initializing,
        Ready,
        Recording,
        Processing,
        Error
    }

    public class VoiceEngine : IDisposable
    {
        // ── Events ────────────────────────────────────────────────────────────────
        public event EventHandler<VoiceMatchResult> CommandRecognized;
        public event EventHandler<VoiceEngineState> StateChanged;
        public event EventHandler<string> LogMessage;

        // ── Config ───────────────────────────────────────────────────────────────
        private readonly double _threshold;
        private const int SAMPLE_RATE = 16000;
        private List<string> _commandList = new List<string>();

        // ── Core Objects ─────────────────────────────────────────────────────────
        private VoiceEngineState _currentState = VoiceEngineState.Initializing;
        private bool _isDisposed;

        public VoiceEngine(double threshold = 0.80)
        {
            _threshold = threshold;
            InitializeTTS();
        }

        public void SetCommandList(IEnumerable<string> commands)
        {
            _commandList.Clear();
            _commandList.AddRange(commands);
        }

        private void ChangeState(VoiceEngineState newState)
        {
            _currentState = newState;
            StateChanged?.Invoke(this, newState);
        }

        private void Log(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        // ════════════════════════════════════════════════════════════════════════
        #region Whisper AI
        // ════════════════════════════════════════════════════════════════════════
        private WhisperFactory _whisperFactory;
        private WhisperProcessor _whisperProcessor;
        private bool _modelReady;

        public async Task InitializeAsync(string modelPath = "whisper-model.bin", GgmlType ggmlType = GgmlType.Base)
        {
            try
            {
                ChangeState(VoiceEngineState.Initializing);
                Log("Kiểm tra model Whisper...");

                // Download model if not present
                if (!File.Exists(modelPath))
                {
                    Log($"Đang tải model {ggmlType}... Vui lòng chờ.");
                    using var httpClient = new System.Net.Http.HttpClient();
                    var downloader = new WhisperGgmlDownloader(httpClient);
                    await using var modelStream = await downloader.GetGgmlModelAsync(ggmlType);
                    await using var fileStream = File.OpenWrite(modelPath);
                    await modelStream.CopyToAsync(fileStream);
                    Log("✔  Tải model xong!");
                }

                // Load model
                Log("Đang khởi tạo Whisper...");
                _whisperFactory = WhisperFactory.FromPath(modelPath);
                
                // Extract prompt from command list if available
                string prompt = _commandList.Count > 0 ? string.Join(", ", _commandList) : "Lỗi, xác nhận, hủy bỏ";

                _whisperProcessor = _whisperFactory.CreateBuilder()
                    .WithLanguage("vi")
                    .WithThreads(Environment.ProcessorCount)
                    .WithSingleSegment()
                    .WithNoContext()
                    .WithPrompt(prompt)
                    .Build();

                _modelReady = true;
                Log("✔  Model sẵn sàng!");
                ChangeState(VoiceEngineState.Ready);
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi khởi tạo Whisper: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
                throw;
            }
        }
        #endregion

        // ════════════════════════════════════════════════════════════════════════
        #region Audio Capture
        // ════════════════════════════════════════════════════════════════════════
        private WaveInEvent _waveIn;
        private MemoryStream _audioBuffer;
        private WaveFileWriter _waveWriter;
        private bool _isRecording;

        public void StartPushToTalk()
        {
            if (!_modelReady || _isRecording) return;

            try
            {
                _audioBuffer = new MemoryStream();
                _waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SAMPLE_RATE, 16, 1),
                    BufferMilliseconds = 100
                };

                _waveWriter = new WaveFileWriter(_audioBuffer, _waveIn.WaveFormat);
                _waveIn.DataAvailable += OnAudioData;
                _waveIn.RecordingStopped += OnRecordingStopped;
                
                _waveIn.StartRecording();
                _isRecording = true;

                Log("🎙  Bắt đầu ghi âm...");
                ChangeState(VoiceEngineState.Recording);
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi mic: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
            }
        }

        private void OnAudioData(object sender, WaveInEventArgs e)
        {
            _waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);
        }

        public async Task StopAndRecognizeAsync()
        {
            if (!_isRecording) return;
            
            _isRecording = false;
            _waveIn?.StopRecording();
            
            Log("Đang xử lý audio...");
            ChangeState(VoiceEngineState.Processing);

            // Wait a bit to ensure RecordingStopped event has fired and flush is done.
            await Task.Delay(100); 
            await RecognizeAudioAsync();
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _waveWriter?.Flush();
            // We handle RecognizeAudioAsync in StopAndRecognizeAsync now to make it an awaitable process from outside.
        }

        /// <summary>
        /// Test nhận dạng từ file WAV sẵn có (không cần mic).
        /// File WAV phải là 16kHz, 16-bit, Mono — dùng Audacity hoặc FFmpeg để convert.
        /// </summary>
        public async Task RecognizeFromFileAsync(string wavFilePath)
        {
            if (!_modelReady)
            {
                Log("⚠  Model chưa sẵn sàng. Gọi InitializeAsync() trước.");
                return;
            }
            if (!File.Exists(wavFilePath))
            {
                Log($"✘  Không tìm thấy file: {wavFilePath}");
                return;
            }

            try
            {
                Log($"📂  Đang đọc file: {Path.GetFileName(wavFilePath)}");
                ChangeState(VoiceEngineState.Processing);

                using var fileStream = File.OpenRead(wavFilePath);
                _audioBuffer = new MemoryStream();
                await fileStream.CopyToAsync(_audioBuffer);
                _audioBuffer.Position = 0;

                await RecognizeAudioAsync();
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi đọc file: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
            }
        }

        private async Task RecognizeAudioAsync()
        {
            try
            {
                if (_audioBuffer == null || _audioBuffer.Length < 1000)
                {
                    Log("⚠  Audio quá ngắn.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                _audioBuffer.Position = 0;

                string recognizedText = "";
                double avgProb = 0;
                int segCount = 0;

                await foreach (var segment in _whisperProcessor.ProcessAsync(_audioBuffer))
                {
                    recognizedText += segment.Text + " ";
                    avgProb += segment.Probability;
                    segCount++;
                }

                recognizedText = recognizedText.Trim();
                if (segCount > 0) avgProb /= segCount;

                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    Log("⚠  Không nhận được gì.");
                    SpeakAsync("Không nghe thấy gì.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                Log($"🔊  Nghe được: \"{recognizedText}\" (Prob: {avgProb:P0})");

                var (matchedCommand, score) = FindBestMatch(recognizedText);

                var result = new VoiceMatchResult
                {
                    RecognizedText = recognizedText,
                    MatchedCommand = (matchedCommand != null && score >= _threshold) ? matchedCommand : null,
                    ConfidenceScore = score
                };

                if (result.IsSuccess)
                {
                    Log($"✔  Khớp lệnh: \"{result.MatchedCommand}\" (Score: {score:P0})");
                    SpeakAsync($"Đã nhận lệnh {result.MatchedCommand}");
                }
                else
                {
                    string info = matchedCommand != null
                        ? $"Gần nhất: \"{matchedCommand}\" ({score:P0}) — dưới ngưỡng {_threshold:P0}"
                        : "Không tìm thấy lệnh phù hợp";
                    Log($"⚠  {info}");
                    SpeakAsync("Không nhận ra lệnh.");
                }

                CommandRecognized?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi nhận dạng: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
            }
            finally
            {
                ChangeState(VoiceEngineState.Ready);
            }
        }
        #endregion

        // ════════════════════════════════════════════════════════════════════════
        #region Fuzzy Matching
        // ════════════════════════════════════════════════════════════════════════
        private (string best, double score) FindBestMatch(string input)
        {
            string normIn = Normalize(input);
            double best = 0;
            string match = null;

            foreach (var cmd in _commandList)
            {
                string normCmd = Normalize(cmd);
                double jw = JaroWinkler(normIn, normCmd);
                double sub = (normIn.Contains(normCmd) || normCmd.Contains(normIn)) ? 0.12 : 0;
                double s = Math.Min(jw + sub, 1.0);
                if (s > best) { best = s; match = cmd; }
            }
            return (match, best);
        }

        private static string Normalize(string s)
        {
            s = s.ToLowerInvariant().Trim();
            var map = new Dictionary<string, string>
            {
                { "loi ",  "lỗi " }, { "lỗi 1", "lỗi 1" },
                { "ho keo","hở keo" }, { " do",   " dơ" }
            };
            foreach (var kv in map) s = s.Replace(kv.Key, kv.Value);
            return s;
        }

        private static double JaroWinkler(string s1, string s2)
        {
            if (s1 == s2) return 1.0;
            if (s1.Length == 0 || s2.Length == 0) return 0.0;
            int dist = Math.Max(s1.Length, s2.Length) / 2 - 1;
            if (dist < 0) dist = 0;
            bool[] m1 = new bool[s1.Length], m2 = new bool[s2.Length];
            int matches = 0, trans = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                int lo = Math.Max(0, i - dist), hi = Math.Min(i + dist + 1, s2.Length);
                for (int j = lo; j < hi; j++)
                {
                    if (m2[j] || s1[i] != s2[j]) continue;
                    m1[i] = m2[j] = true; matches++; break;
                }
            }
            if (matches == 0) return 0.0;
            int k = 0;
            for (int i = 0; i < s1.Length; i++)
            {
                if (!m1[i]) continue;
                while (!m2[k]) k++;
                if (s1[i] != s2[k]) trans++;
                k++;
            }
            double jaro = (matches / (double)s1.Length +
                           matches / (double)s2.Length +
                           (matches - trans / 2.0) / matches) / 3.0;
            int pre = 0;
            for (int i = 0; i < Math.Min(Math.Min(s1.Length, s2.Length), 4); i++)
            { if (s1[i] == s2[i]) pre++; else break; }
            return jaro + pre * 0.1 * (1 - jaro);
        }
        #endregion

        // ════════════════════════════════════════════════════════════════════════
        #region TTS / Feedback
        // ════════════════════════════════════════════════════════════════════════
        private SpeechSynthesizer _tts;

        private void InitializeTTS()
        {
            try
            {
                _tts = new SpeechSynthesizer();
                _tts.Rate = 0;
                _tts.Volume = 100;
                foreach (var v in _tts.GetInstalledVoices())
                {
                    if (v.VoiceInfo.Culture.Name.StartsWith("vi"))
                    {
                        _tts.SelectVoice(v.VoiceInfo.Name);
                        break;
                    }
                }
            }
            catch
            {
                // TTS is optional, ignore if failed
            }
        }

        private void SpeakAsync(string text)
        {
            if (_tts == null) return;
            Task.Run(() => { try { _tts.Speak(text); } catch { } });
        }
        #endregion

        // ════════════════════════════════════════════════════════════════════════
        #region Cleanup
        // ════════════════════════════════════════════════════════════════════════
        public void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;

            _isRecording = false;
            try { _waveIn?.StopRecording(); _waveIn?.Dispose(); } catch { }
            try { _waveWriter?.Dispose(); _audioBuffer?.Dispose(); } catch { }
            try { _whisperProcessor?.Dispose(); _whisperFactory?.Dispose(); } catch { }
            try { _tts?.Dispose(); } catch { }
        }
        #endregion
    }
}
