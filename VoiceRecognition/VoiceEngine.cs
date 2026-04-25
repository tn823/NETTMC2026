using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using NAudio.Wave;
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
        private const double VoiceLevelThreshold = 0.018;
        private const int MinimumVoiceMs = 250;
        private const int SilenceAfterVoiceMs = 650;

        private readonly double _threshold;
        private readonly List<string> _commandList = new List<string>();
        private readonly List<VoiceCommandDefinition> _commandDefinitions = new List<VoiceCommandDefinition>();

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
        private int _maxRecordingMs = 8000;

        private SpeechSynthesizer _tts;
        private bool _isDisposed;

        public VoiceEngine(double threshold = 0.80)
        {
            _threshold = threshold;
            InitializeTTS();
        }

        public bool IsRecording => _isRecording;

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
                Log($"Loi khoi tao Whisper: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
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
                Log($"Loi mic: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
            }
        }

        public async Task StartSmartPushToTalkAsync(int maxRecordingMs = 5000)
        {
            StartPushToTalk(maxRecordingMs);

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
                Log($"Loi doc file: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
            }
        }

        private void OnAudioData(object sender, WaveInEventArgs e)
        {
            _waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);

            DateTime now = DateTime.UtcNow;
            double level = CalculateRms(e.Buffer, e.BytesRecorded);
            bool hasVoice = level >= VoiceLevelThreshold;

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

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            _waveWriter?.Flush();
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

                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    Log("Khong nhan duoc gi.");
                    SpeakAsync("Khong nghe thay gi.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                Log($"Nghe duoc: \"{recognizedText}\" (Prob: {avgProb:P0})");

                VoiceCommandMatch parsedCommand = null;
                if (_commandDefinitions.Count > 0)
                {
                    parsedCommand = VoiceCommandParser.Parse(recognizedText, _commandDefinitions, _threshold);
                }

                var (matchedCommand, score) = FindBestMatch(recognizedText);
                if (parsedCommand?.IsSuccess == true && parsedCommand.ConfidenceScore >= score)
                {
                    matchedCommand = parsedCommand.ToDisplayText();
                    score = parsedCommand.ConfidenceScore;
                }

                var result = new VoiceMatchResult
                {
                    RecognizedText = recognizedText,
                    ParsedCommand = parsedCommand,
                    MatchedCommand = (matchedCommand != null && score >= _threshold) ? matchedCommand : null,
                    ConfidenceScore = score
                };

                if (result.IsSuccess)
                {
                    Log($"Khop lenh: \"{result.MatchedCommand}\" (Score: {score:P0})");
                    SpeakAsync($"Da nhan lenh {result.MatchedCommand}");
                }
                else
                {
                    string info = matchedCommand != null
                        ? $"Gan nhat: \"{matchedCommand}\" ({score:P0}) - duoi nguong {_threshold:P0}"
                        : "Khong tim thay lenh phu hop";
                    Log(info);
                    SpeakAsync("Khong nhan ra lenh.");
                }

                CommandRecognized?.Invoke(this, result);
            }
            catch (Exception ex)
            {
                Log($"Loi nhan dang: {ex.Message}");
                ChangeState(VoiceEngineState.Error);
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
            StateChanged?.Invoke(this, newState);
        }

        private void Log(string message)
        {
            LogMessage?.Invoke(this, message);
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
        }
    }
}
