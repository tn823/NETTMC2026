using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using NAudio.Wave;
using WebRtcVadSharp;
using Whisper.net;
using Whisper.net.Ggml;

#nullable disable

namespace NETTMC.VoiceRecognition
{
    public class VoiceMatchResult
    {
        public string RecognizedText { get; set; }
        public string MatchedCommand { get; set; }
        public double ConfidenceScore { get; set; }
        public VoiceCommandMatch ParsedCommand { get; set; }
        public bool IsSuccess => ParsedCommand?.IsSuccess == true || MatchedCommand != null;
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
        public event EventHandler<VoiceMatchResult> CommandRecognized;
        public event EventHandler<VoiceEngineState> StateChanged;
        public event EventHandler<string> LogMessage;

        private const int SampleRate = 16000;
        // 20ms frame = 16000Hz * 0.02s * 2 bytes (16-bit) = 640 bytes
        private const int VadFrameBytes = 640;
        private const int MinimumVoiceMs = 250;
        private const int SilenceAfterVoiceMs = 400;

        // Từ khoá kết thúc câu — được strip trước khi match.
        // Không dùng "ok"/"ô kê" vì có xung đột với action "pass".
        // Người dùng nói: "A hở keo xong" → strip "xong" → xử lý "A hở keo"
        private static readonly string[] EndKeywords = { "xong", "hết", "done" };

        private readonly double _threshold;
        private readonly List<string> _commandList = new List<string>();
        private readonly List<VoiceCommandDefinition> _commandDefinitions = new List<VoiceCommandDefinition>();

        private WebRtcVad _vad;
        private WhisperFactory _whisperFactory;
        private WhisperProcessor _whisperProcessor;
        private bool _modelReady;

        private WaveInEvent _waveIn;
        private MemoryStream _audioBuffer;
        private WaveFileWriter _waveWriter;
        private bool _isRecording;
        private bool _hasPendingAudio;
        private bool _speechDetected;
        private DateTime _recordingStartedAt;
        private DateTime _firstVoiceAt;
        private DateTime _lastVoiceAt;
        private int _maxRecordingMs = 5000;

        private SpeechSynthesizer _tts;
        private bool _isDisposed;
        private string _lastErrorMessage;

        public VoiceEngine(double threshold = 0.80)
        {
            _threshold = threshold;
            InitializeTTS();
            InitializeVad();
        }

        public bool IsRecording => _isRecording;
        public string LastErrorMessage => _lastErrorMessage;

        public void SetCommandList(IEnumerable<string> commands)
        {
            _commandList.Clear();
            if (commands != null)
            {
                _commandList.AddRange(commands.Where(c => !string.IsNullOrWhiteSpace(c)));
            }
        }

        public void SetCommandDefinitions(IEnumerable<VoiceCommandDefinition> commands)
        {
            _commandDefinitions.Clear();
            if (commands != null)
            {
                _commandDefinitions.AddRange(commands.Where(c => c != null));
            }

            SetCommandList(_commandDefinitions.SelectMany(c => c.AllPhrases()).Distinct(StringComparer.OrdinalIgnoreCase));
        }

        public async Task InitializeAsync(string modelPath = "whisper-model.bin", GgmlType ggmlType = GgmlType.Base)
        {
            try
            {
                ChangeState(VoiceEngineState.Initializing);
                Log("Kiem tra model Whisper...");

                if (!File.Exists(modelPath))
                {
                    Log($"Dang tai model {ggmlType}. Vui long cho.");
                    using var httpClient = new System.Net.Http.HttpClient();
                    var downloader = new WhisperGgmlDownloader(httpClient);
                    await using var modelStream = await downloader.GetGgmlModelAsync(ggmlType);
                    await using var fileStream = File.OpenWrite(modelPath);
                    await modelStream.CopyToAsync(fileStream);
                    Log("Tai model xong.");
                }

                Log("Dang khoi tao Whisper...");
                _whisperFactory = WhisperFactory.FromPath(modelPath);

                _whisperProcessor = _whisperFactory.CreateBuilder()
                    .WithLanguage("vi")
                    .WithThreads(Environment.ProcessorCount)
                    .WithSingleSegment()
                    .WithNoContext()
                    .WithPrompt(BuildPrompt())
                    .Build();

                _modelReady = true;
                Log("Model san sang.");
                ChangeState(VoiceEngineState.Ready);
            }
            catch (Exception ex)
            {
                SetError("Loi khoi tao Whisper", ex);
                throw;
            }
        }

        public void StartPushToTalk(int maxRecordingMs = 8000)
        {
            if (!_modelReady || _isRecording)
            {
                return;
            }

            try
            {
                _maxRecordingMs = Math.Max(1000, maxRecordingMs);
                _recordingStartedAt = DateTime.UtcNow;
                _firstVoiceAt = DateTime.MinValue;
                _lastVoiceAt = DateTime.MinValue;
                _speechDetected = false;
                _hasPendingAudio = true;

                _audioBuffer = new MemoryStream();
                _waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SampleRate, 16, 1),
                    BufferMilliseconds = 100
                };

                _waveWriter = new WaveFileWriter(_audioBuffer, _waveIn.WaveFormat);
                _waveIn.DataAvailable += OnAudioData;
                _waveIn.RecordingStopped += OnRecordingStopped;

                _isRecording = true;
                _waveIn.StartRecording();

                Log("Bat dau ghi am.");
                ChangeState(VoiceEngineState.Recording);
            }
            catch (Exception ex)
            {
                _isRecording = false;
                _hasPendingAudio = false;
                CleanupRecordingResources();
                SetError("Loi mic", ex);
            }
        }

        public async Task StartSmartPushToTalkAsync(int maxRecordingMs = 5000)
        {
            StartPushToTalk(maxRecordingMs);

            if (!_isRecording)
            {
                return;
            }

            while (_isRecording)
            {
                await Task.Delay(50);
            }

            await StopAndRecognizeAsync();
        }

        public async Task StopAndRecognizeAsync()
        {
            if (_isRecording)
            {
                _isRecording = false;
                _waveIn?.StopRecording();
            }

            if (!_hasPendingAudio)
            {
                return;
            }

            _hasPendingAudio = false;
            Log("Dang xu ly audio...");
            ChangeState(VoiceEngineState.Processing);

            await Task.Delay(100);
            FinalizeWaveWriter();
            await RecognizeAudioAsync();
        }

        public async Task RecognizeFromFileAsync(string wavFilePath)
        {
            if (!_modelReady)
            {
                Log("Model chua san sang. Goi InitializeAsync() truoc.");
                return;
            }

            if (!File.Exists(wavFilePath))
            {
                Log($"Khong tim thay file: {wavFilePath}");
                return;
            }

            try
            {
                Log($"Dang doc file: {Path.GetFileName(wavFilePath)}");
                ChangeState(VoiceEngineState.Processing);

                await using var fileStream = File.OpenRead(wavFilePath);
                _audioBuffer = new MemoryStream();
                await fileStream.CopyToAsync(_audioBuffer);
                _audioBuffer.Position = 0;

                await RecognizeAudioAsync();
            }
            catch (Exception ex)
            {
                SetError("Loi doc file", ex);
            }
        }

        private void OnAudioData(object sender, WaveInEventArgs e)
        {
            _waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);

            DateTime now = DateTime.UtcNow;
            bool hasVoice = HasVoiceVad(e.Buffer, e.BytesRecorded);

            if (hasVoice)
            {
                if (!_speechDetected)
                {
                    _firstVoiceAt = now;
                }

                _speechDetected = (now - _firstVoiceAt).TotalMilliseconds >= MinimumVoiceMs;
                _lastVoiceAt = now;
            }

            bool maxReached = (now - _recordingStartedAt).TotalMilliseconds >= _maxRecordingMs;
            bool silenceReached = _speechDetected &&
                (now - _lastVoiceAt).TotalMilliseconds >= SilenceAfterVoiceMs;

            if (maxReached || silenceReached)
            {
                _isRecording = false;
                _waveIn?.StopRecording();
            }
        }

        // Phân tích giọng nói bằng WebRTC VAD (chia chunk 20ms)
        // Nếu VAD chưa sẵn sàng, fallback về RMS cơ bản
        private bool HasVoiceVad(byte[] buffer, int bytesRecorded)
        {
            if (_vad == null || bytesRecorded < VadFrameBytes)
            {
                return CalculateRms(buffer, bytesRecorded) >= 0.018;
            }

            int offset = 0;
            int voiceFrames = 0;
            int totalFrames = 0;

            while (offset + VadFrameBytes <= bytesRecorded)
            {
                byte[] frame = new byte[VadFrameBytes];
                Buffer.BlockCopy(buffer, offset, frame, 0, VadFrameBytes);

                try
                {
                    if (_vad.HasSpeech(frame, WebRtcVadSharp.SampleRate.Is16kHz, FrameLength.Is20ms))
                    {
                        voiceFrames++;
                    }
                }
                catch
                {
                    // Nếu VAD lỗi một frame, bỏ qua
                }

                totalFrames++;
                offset += VadFrameBytes;
            }

            // Có giọng nói nếu >=30% frame bị phân loại là speech
            return totalFrames > 0 && (double)voiceFrames / totalFrames >= 0.30;
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            FinalizeWaveWriter();
        }

        private void FinalizeWaveWriter()
        {
            if (_waveWriter == null)
            {
                return;
            }

            try
            {
                _waveWriter.Flush();
                _waveWriter.Dispose();
                _waveWriter = null;

                if (_audioBuffer != null)
                {
                    byte[] wavBytes = _audioBuffer.ToArray();
                    _audioBuffer.Dispose();
                    _audioBuffer = new MemoryStream(wavBytes);
                    _audioBuffer.Position = 0;
                }
            }
            catch (Exception ex)
            {
                SetError("Loi dong goi audio", ex);
            }
        }

        private async Task RecognizeAudioAsync()
        {
            try
            {
                if (_audioBuffer == null || _audioBuffer.Length < 1000)
                {
                    Log("Audio qua ngan.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                _audioBuffer.Position = 0;

                string recognizedText = string.Empty;
                double avgProb = 0;
                int segCount = 0;

                await foreach (var segment in _whisperProcessor.ProcessAsync(_audioBuffer))
                {
                    recognizedText += segment.Text + " ";
                    avgProb += segment.Probability;
                    segCount++;
                }

                recognizedText = recognizedText.Trim();
                if (segCount > 0)
                {
                    avgProb /= segCount;
                }

                // Strip từ khoá kết thúc câu ("ok", "ô kê"...) trước khi parse
                string cleanedText = StripEndKeyword(recognizedText);

                if (string.IsNullOrWhiteSpace(cleanedText))
                {
                    Log("Chỉ nghe được từ kết thúc, bỏ qua.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                Log($"Nghe được: \"{recognizedText}\" → xử lý: \"{cleanedText}\" (Prob: {avgProb:P0})");

                if (_commandDefinitions.Count > 0)
                {
                    // ParseAll: nhận NHIỀU lệnh trong một câu (A 11 21 82 → part A + errors 11,21,82)
                    var allMatches = VoiceCommandParser.ParseAll(cleanedText, _commandDefinitions, _threshold);

                    if (allMatches.Count > 0)
                    {
                        foreach (var parsedCommand in allMatches)
                        {
                            Log($"[Voice OK] {parsedCommand.ToDisplayText()} ({parsedCommand.ConfidenceScore:P0})");
                            var result = new VoiceMatchResult
                            {
                                RecognizedText  = recognizedText,
                                ParsedCommand   = parsedCommand,
                                MatchedCommand  = parsedCommand.ToDisplayText(),
                                ConfidenceScore = parsedCommand.ConfidenceScore
                            };
                            CommandRecognized?.Invoke(this, result);
                        }

                        SpeakAsync($"Đã nhận {allMatches.Count} lệnh");
                        return;
                    }
                }

                // Fallback: dùng simple keyword list (không có _commandDefinitions)
                var (matchedCommand, score) = FindBestMatch(cleanedText);
                var fallbackResult = new VoiceMatchResult
                {
                    RecognizedText  = recognizedText,
                    ParsedCommand   = null,
                    MatchedCommand  = (matchedCommand != null && score >= _threshold) ? matchedCommand : null,
                    ConfidenceScore = score
                };

                if (fallbackResult.IsSuccess)
                    Log($"[Voice OK] \"{fallbackResult.MatchedCommand}\" ({score:P0})");
                else
                    Log($"[Voice NG] Không nhận ra: \"{cleanedText}\" (gần nhất: \"{matchedCommand}\" {score:P0})");

                CommandRecognized?.Invoke(this, fallbackResult);

            }
            catch (Exception ex)
            {
                SetError("Loi nhan dang", ex);
            }
            finally
            {
                ChangeState(VoiceEngineState.Ready);
            }
        }

        private string BuildPrompt()
        {
            if (_commandList.Count == 0)
            {
                return "dat, loi, xoa, huy, A mot, B hai, C ba";
            }

            return string.Join(", ", _commandList.Take(80));
        }

        private (string best, double score) FindBestMatch(string input)
        {
            string normIn = VoiceTextNormalizer.Normalize(input);
            double best = 0;
            string match = null;

            foreach (var cmd in _commandList)
            {
                string normCmd = VoiceTextNormalizer.Normalize(cmd);
                double jw = VoiceTextNormalizer.JaroWinkler(normIn, normCmd);
                double sub = (normIn.Contains(normCmd) || normCmd.Contains(normIn)) ? 0.12 : 0;
                double score = Math.Min(jw + sub, 1.0);
                if (score > best)
                {
                    best = score;
                    match = cmd;
                }
            }

            return (match, best);
        }

        private static double CalculateRms(byte[] buffer, int bytesRecorded)
        {
            if (bytesRecorded <= 0)
            {
                return 0;
            }

            double sumSquares = 0;
            int samples = bytesRecorded / 2;
            for (int i = 0; i < bytesRecorded - 1; i += 2)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                double normalized = sample / 32768.0;
                sumSquares += normalized * normalized;
            }

            return Math.Sqrt(sumSquares / Math.Max(1, samples));
        }

        private void InitializeTTS()
        {
            try
            {
                _tts = new SpeechSynthesizer
                {
                    Rate = 0,
                    Volume = 100
                };

                foreach (var voice in _tts.GetInstalledVoices())
                {
                    if (voice.VoiceInfo.Culture.Name.StartsWith("vi"))
                    {
                        _tts.SelectVoice(voice.VoiceInfo.Name);
                        break;
                    }
                }
            }
            catch
            {
                _tts = null;
            }
        }

        private void InitializeVad()
        {
            try
            {
                _vad = new WebRtcVad
                {
                    // VeryAggressive: ít false positive nhất trong môi trường ồn
                    OperatingMode = OperatingMode.VeryAggressive
                };
            }
            catch
            {
                _vad = null;
                Log("WebRTC VAD khởi tạo thất bại, fallback về RMS.");
            }
        }

        private static string StripEndKeyword(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            string normalized = text.Trim();
            foreach (string kw in EndKeywords)
            {
                if (normalized.EndsWith(kw, StringComparison.OrdinalIgnoreCase))
                {
                    return normalized.Substring(0, normalized.Length - kw.Length).Trim();
                }
            }

            return normalized;
        }

        private void SpeakAsync(string text)
        {
            if (_tts == null)
            {
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    _tts.Speak(text);
                }
                catch
                {
                }
            });
        }

        private void ChangeState(VoiceEngineState newState)
        {
            if (newState != VoiceEngineState.Error)
            {
                _lastErrorMessage = null;
            }

            StateChanged?.Invoke(this, newState);
        }

        private void Log(string message)
        {
            LogMessage?.Invoke(this, message);
        }

        private void SetError(string context, Exception ex)
        {
            _lastErrorMessage = $"{context}: {ex.GetType().Name}: {ex.Message}";
            Log($"[Voice Error] ExceptionType: {ex.GetType().FullName}");
            Log($"[Voice Error] {context}: {ex.Message}");
            ChangeState(VoiceEngineState.Error);
        }

        private void CleanupRecordingResources()
        {
            try { _waveIn?.StopRecording(); } catch { }
            try { _waveIn?.Dispose(); } catch { }
            try { _waveWriter?.Dispose(); } catch { }
            try { _audioBuffer?.Dispose(); } catch { }

            _waveIn = null;
            _waveWriter = null;
            _audioBuffer = null;
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            _isDisposed = true;
            _isRecording = false;

            try { _waveIn?.StopRecording(); _waveIn?.Dispose(); } catch { }
            try { _waveWriter?.Dispose(); _audioBuffer?.Dispose(); } catch { }
            try { _whisperProcessor?.Dispose(); _whisperFactory?.Dispose(); } catch { }
            try { _tts?.Dispose(); } catch { }
            try { _vad?.Dispose(); } catch { }
        }
    }
}
