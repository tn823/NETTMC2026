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
        public IReadOnlyList<VoiceCommandMatch> ParsedCommands { get; set; }
        public double ProcessingTimeSec { get; set; }
        public string CleanedText { get; set; }
        public string NormalizedText { get; set; }
        public string AudioDiagnostics { get; set; }
        public string ParseTrace { get; set; }
        public bool IsSuccess => ParsedCommand?.IsSuccess == true || ParsedCommands?.Any(c => c?.IsSuccess == true) == true || MatchedCommand != null;
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
        private const int DefaultMinimumVoiceMs = 120;
        private const int DefaultSilenceAfterVoiceMs = 800;

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
        private double _recordingMaxRms;
        private double _recordingMaxPeak;
        private double _recordingRmsSum;
        private int _recordingChunks;
        private int _speechChunks;
        private long _recordedBytes;

        private SpeechSynthesizer _tts;
        private bool _isDisposed;
        private string _lastErrorMessage;

        public VoiceEngine(double threshold = 0.78)
        {
            _threshold = threshold;
            InitializeTTS();
            InitializeVad();
        }

        /// <summary>
        /// Khi bật Auto-Loop, engine áp dụng bộ lọc ketat hơn:
        /// chỉ fire CommandRecognized khi kết quả chứa Action nghiệp vụ
        /// (pass/fail/re-pass/re-fail/clear) để tránh nhiễu môi trường.
        /// </summary>
        public bool IsAutoLoopMode { get; set; } = false;
        public bool EmitBatchCommandResult { get; set; } = false;
        public double InputGain { get; set; } = 3.0;
        public double RmsVoiceThreshold { get; set; } = 0.0025;
        public double VadVoiceFrameRatio { get; set; } = 0.15;
        public int MinimumVoiceDurationMs { get; set; } = DefaultMinimumVoiceMs;
        public int SilenceAfterVoiceDurationMs { get; set; } = DefaultSilenceAfterVoiceMs;

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
                foreach (var cmd in commands.Where(c => c != null))
                {
                    cmd.BuildNormalizedCache();   // Tính sẵn normalized phrases 1 lần
                    _commandDefinitions.Add(cmd);
                }
            }

            SetCommandList(_commandDefinitions.SelectMany(c => c.AllPhrases()).Distinct(StringComparer.OrdinalIgnoreCase));
            if (_modelReady && !_isRecording)
            {
                RebuildWhisperProcessor();
            }
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
                RebuildWhisperProcessor();

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
                _recordingMaxRms = 0;
                _recordingMaxPeak = 0;
                _recordingRmsSum = 0;
                _recordingChunks = 0;
                _speechChunks = 0;
                _recordedBytes = 0;

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

                Log($"Bat dau ghi am. max={_maxRecordingMs}ms gain={InputGain:F1} rms={RmsVoiceThreshold:F4} vadRatio={VadVoiceFrameRatio:F2}");
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
            byte[] audio = PrepareAudioBuffer(e.Buffer, e.BytesRecorded);
            _waveWriter?.Write(audio, 0, audio.Length);

            DateTime now = DateTime.UtcNow;
            bool hasVoice = HasVoiceVad(audio, audio.Length);
            UpdateRecordingStats(audio, audio.Length, hasVoice);

            if (hasVoice)
            {
                if (!_speechDetected)
                {
                    _firstVoiceAt = now;
                }

                _speechDetected = (now - _firstVoiceAt).TotalMilliseconds >= MinimumVoiceDurationMs;
                _lastVoiceAt = now;
            }

            bool maxReached = (now - _recordingStartedAt).TotalMilliseconds >= _maxRecordingMs;
            bool silenceReached = _speechDetected &&
                (now - _lastVoiceAt).TotalMilliseconds >= SilenceAfterVoiceDurationMs;

            if (maxReached || silenceReached)
            {
                _isRecording = false;
                _waveIn?.StopRecording();
            }
        }

        // Phân tích giọng nói bằng WebRTC VAD (chia chunk 20ms)
        // Nếu VAD chưa sẵn sàng, fallback về RMS cơ bản
        private byte[] PrepareAudioBuffer(byte[] buffer, int bytesRecorded)
        {
            int length = Math.Max(0, bytesRecorded);
            var audio = new byte[length];
            if (length == 0)
            {
                return audio;
            }

            double gain = InputGain;
            if (gain < 1.0) gain = 1.0;
            if (gain > 8.0) gain = 8.0;

            if (gain <= 1.01)
            {
                Buffer.BlockCopy(buffer, 0, audio, 0, length);
                return audio;
            }

            for (int i = 0; i < length - 1; i += 2)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                int boosted = (int)Math.Round(sample * gain);
                if (boosted > short.MaxValue) boosted = short.MaxValue;
                if (boosted < short.MinValue) boosted = short.MinValue;

                byte[] bytes = BitConverter.GetBytes((short)boosted);
                audio[i] = bytes[0];
                audio[i + 1] = bytes[1];
            }

            return audio;
        }

        private void UpdateRecordingStats(byte[] buffer, int bytesRecorded, bool hasVoice)
        {
            double rms = CalculateRms(buffer, bytesRecorded);
            double peak = CalculatePeak(buffer, bytesRecorded);

            _recordingChunks++;
            _recordedBytes += bytesRecorded;
            _recordingRmsSum += rms;
            if (rms > _recordingMaxRms) _recordingMaxRms = rms;
            if (peak > _recordingMaxPeak) _recordingMaxPeak = peak;
            if (hasVoice) _speechChunks++;
        }

        private bool HasVoiceVad(byte[] buffer, int bytesRecorded)
        {
            if (_vad == null || bytesRecorded < VadFrameBytes)
            {
                // Giảm threshold RMS từ 0.018 xuống 0.005 để bắt giọng nói nhỏ tốt hơn
                return CalculateRms(buffer, bytesRecorded) >= RmsVoiceThreshold;
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

            // Có giọng nói nếu >=25% frame bị phân loại là speech (giảm từ 40% để dễ nhận diện giọng nhỏ)
            bool vadDetected = totalFrames > 0 && (double)voiceFrames / totalFrames >= VadVoiceFrameRatio;
            bool rmsDetected = CalculateRms(buffer, bytesRecorded) >= RmsVoiceThreshold;
            return vadDetected || rmsDetected;
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
            var sw = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                if (_audioBuffer == null || _audioBuffer.Length < 1000)
                {
                    sw.Stop();
                    Log($"Audio qua ngan. - {sw.Elapsed.TotalSeconds:F2}s");
                    Log("==============================================");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                _audioBuffer.Position = 0;
                if (_whisperProcessor == null)
                {
                    sw.Stop();
                    Log("Whisper processor chua san sang.");
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                string audioDiagnostics = BuildAudioDiagnostics();
                Log($"[Audio] {audioDiagnostics}");

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
                string normalizedText = VoiceTextNormalizer.Normalize(cleanedText);

                if (string.IsNullOrWhiteSpace(cleanedText))
                {
                    sw.Stop();
                    Log($"Chi nghe duoc tu ket thuc: raw=\"{recognizedText}\" - {sw.Elapsed.TotalSeconds:F2}s");
                    Log("=============================================="); 
                    CommandRecognized?.Invoke(this, new VoiceMatchResult
                    {
                        RecognizedText = recognizedText,
                        CleanedText = cleanedText,
                        NormalizedText = normalizedText,
                        MatchedCommand = "[Bỏ qua] Chỉ từ kết thúc",
                        ConfidenceScore = avgProb,
                        ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                        AudioDiagnostics = audioDiagnostics,
                        ParseTrace = "Only end keyword after StripEndKeyword"
                    });

                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                // Tầng 1: Probability filter đã tắt — Whisper.net trả về Probability = 0 luôn
                // (model không tính token probability theo mặc định, không thể dùng làm filter)
                // Log để tham khảo nhưng không block:
                if (LooksLikeNoSpeechTranscript(cleanedText))
                {
                    sw.Stop();
                    Log($"[Whisper noise] Chi nghe silence/noise, Whisper tra: \"{recognizedText}\" - {sw.Elapsed.TotalSeconds:F2}s");
                    Log("==============================================");
                    CommandRecognized?.Invoke(this, new VoiceMatchResult
                    {
                        RecognizedText = recognizedText,
                        CleanedText = cleanedText,
                        NormalizedText = normalizedText,
                        MatchedCommand = "[Bỏ qua] Whisper noise",
                        ConfidenceScore = avgProb,
                        ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                        AudioDiagnostics = audioDiagnostics,
                        ParseTrace = "Whisper returned only dash/punctuation placeholders"
                    });
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                Log($"Nghe duoc: \"{recognizedText}\" -> xu ly: \"{cleanedText}\" | norm=\"{normalizedText}\" (Prob: {avgProb:P0})");

                // ── TẦNG 2: Kiểm tra từ khóa nghiệp vụ ──────────────────────────
                // Auto-loop phải chứa ít nhất 1 từ liên quan đến nghiệp vụ (số, từ hành động VN)
                if (IsAutoLoopMode && !ContainsBusinessKeyword(cleanedText))
                {
                    sw.Stop();
                    Log($"[Bỏ qua] Không chứa từ khoá nghiệp vụ: \"{cleanedText}\" - {sw.Elapsed.TotalSeconds:F2}s");
                    Log("==============================================");
                    CommandRecognized?.Invoke(this, new VoiceMatchResult
                    {
                        RecognizedText = recognizedText,
                        CleanedText = cleanedText,
                        NormalizedText = normalizedText,
                        MatchedCommand = "[Bỏ qua] Không nghiệp vụ",
                        ConfidenceScore = avgProb,
                        ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                        AudioDiagnostics = audioDiagnostics,
                        ParseTrace = "No business keyword"
                    });
                    ChangeState(VoiceEngineState.Ready);
                    return;
                }

                if (_commandDefinitions.Count > 0)
                {
                    var allMatches = VoiceCommandParser.ParseAll(cleanedText, _commandDefinitions, _threshold);

                    if (allMatches.Count > 0)
                    {
                        // ── TẦNG 3: Auto-loop chỉ fire khi có Action nghiệp vụ ────
                        // Lý do: "Cô chí nha" → match Part:C + Error:12 nhưng không có Action
                        //        → không đủ để ghi nhận lỗi → bỏ qua, chờ câu hoàn chỉnh
                        bool hasAction = allMatches.Any(m =>
                            m.Kind == VoiceCommandKind.Action ||
                            (m.Kind == VoiceCommandKind.Composite && m.ActionType != null));

                        if (IsAutoLoopMode && !hasAction)
                        {
                            sw.Stop();
                            Log($"[Bỏ qua] Auto-loop: nhận ra Part/Error nhưng thiếu Action — chờ câu hoàn chỉnh. - {sw.Elapsed.TotalSeconds:F2}s");
                            Log("==============================================");
                            CommandRecognized?.Invoke(this, new VoiceMatchResult
                            {
                                RecognizedText = recognizedText,
                                CleanedText = cleanedText,
                                NormalizedText = normalizedText,
                                MatchedCommand = "[Bỏ qua] Thiếu Action",
                                ConfidenceScore = avgProb,
                                ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                                AudioDiagnostics = audioDiagnostics,
                                ParseTrace = string.Join("; ", allMatches.Select(m => m.ToDisplayText())),
                                ParsedCommands = allMatches
                            });
                            ChangeState(VoiceEngineState.Ready);
                            return;
                        }

                        if (EmitBatchCommandResult)
                        {
                            sw.Stop();
                            var batchCommand = BuildBatchCommand(cleanedText, allMatches);
                            Log($"[Voice OK] {batchCommand.ToDisplayText()} ({batchCommand.ConfidenceScore:P0}) - {sw.Elapsed.TotalSeconds:F2}s");
                            Log("==============================================");
                            CommandRecognized?.Invoke(this, new VoiceMatchResult
                            {
                                RecognizedText = recognizedText,
                                CleanedText = cleanedText,
                                NormalizedText = normalizedText,
                                ParsedCommand = batchCommand,
                                ParsedCommands = allMatches,
                                MatchedCommand = batchCommand.ToDisplayText(),
                                ConfidenceScore = batchCommand.ConfidenceScore,
                                ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                                AudioDiagnostics = audioDiagnostics,
                                ParseTrace = string.Join("; ", allMatches.Select(m => m.ToDisplayText()))
                            });
                            SpeakAsync($"Đã nhận {allMatches.Count} lệnh");
                            return;
                        }

                        foreach (var parsedCommand in allMatches)
                        {
                            sw.Stop();
                            Log($"[Voice OK] {parsedCommand.ToDisplayText()} ({parsedCommand.ConfidenceScore:P0}) - {sw.Elapsed.TotalSeconds:F2}s");
                            Log("==============================================");
                            var result = new VoiceMatchResult
                            {
                                RecognizedText  = recognizedText,
                                CleanedText     = cleanedText,
                                NormalizedText  = normalizedText,
                                ParsedCommand   = parsedCommand,
                                MatchedCommand  = parsedCommand.ToDisplayText(),
                                ConfidenceScore = parsedCommand.ConfidenceScore,
                                ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                                AudioDiagnostics = audioDiagnostics,
                                ParseTrace = parsedCommand.MatchedCommand
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
                    CleanedText     = cleanedText,
                    NormalizedText  = normalizedText,
                    ParsedCommand   = null,
                    MatchedCommand  = (matchedCommand != null && score >= _threshold) ? matchedCommand : null,
                    ConfidenceScore = score,
                    ProcessingTimeSec = sw.Elapsed.TotalSeconds,
                    AudioDiagnostics = audioDiagnostics,
                    ParseTrace = $"best={matchedCommand ?? ""}; score={score:P0}"
                };

                sw.Stop();
                if (fallbackResult.IsSuccess)
                    Log($"[Voice OK] \"{fallbackResult.MatchedCommand}\" ({score:P0}) - {sw.Elapsed.TotalSeconds:F2}s");
                else
                    Log($"[Voice NG] Không nhận ra: \"{cleanedText}\" (gần nhất: \"{matchedCommand}\" {score:P0}) - {sw.Elapsed.TotalSeconds:F2}s");
                Log("==============================================");

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

        private void RebuildWhisperProcessor()
        {
            if (_whisperFactory == null)
            {
                return;
            }

            try { _whisperProcessor?.Dispose(); } catch { }

            _whisperProcessor = _whisperFactory.CreateBuilder()
                .WithLanguage("vi")
                .WithThreads(Environment.ProcessorCount)
                .WithSingleSegment()
                .WithNoContext()
                .WithPrompt(BuildPrompt())
                .Build();

            Log($"Whisper prompt cap nhat: {_commandList.Count} phrases.");
        }

        private string BuildPrompt()
        {
            if (_commandList.Count == 0)
            {
                return "đạt, lỗi, xóa, mười bảy, mười tám, hai mươi mốt";
            }

            // Lấy tối đa 80 từ khóa.
            // Vì _commandList đã được nạp theo thứ tự ưu tiên (17, 18, 21 đứng đầu),
            // prompt tự nhiên sẽ ưu tiên đúng những từ khóa quan trọng nhất.
            var seed = new[]
            {
                //"A mười bảy không đạt",
                //"A mười tám không đạt",
                //"B mười tám không đạt",
                //"C hai mươi mốt không đạt",
                "A",
                "B",
                "C",
                "D",
                "ok",
                "bốn mươi lăm",
                "bốn mươi năm",
                "bốn năm",
                "Pass",
                "Fail",
                "Repass",
                "Refail",
                "bốn tám",


                "đạt",
                "không đạt",
                "đạt lại",
                "lỗi lại",
                "xóa",
                "part A",
                "part B",
                "part C",
                "mười bảy",
                "mười tám",
                "hai mươi mốt",
                "bảy chín",
                "tám hai"
            };

            return string.Join(", ", seed
                .Concat(_commandList)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(120));
        }

        private static VoiceCommandMatch BuildBatchCommand(string input, IReadOnlyList<VoiceCommandMatch> matches)
        {
            var useful = matches?.Where(m => m != null && m.IsSuccess).ToList() ?? new List<VoiceCommandMatch>();
            var part = useful.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.PartCode))?.PartCode;
            var errors = useful
                .Where(m => !string.IsNullOrWhiteSpace(m.ErrorCode))
                .Select(m => m.ErrorCode)
                .Distinct()
                .ToList();
            var action = useful.FirstOrDefault(m => !string.IsNullOrWhiteSpace(m.ActionType))?.ActionType;
            double confidence = useful.Count == 0 ? 0 : useful.Min(m => m.ConfidenceScore);

            return new VoiceCommandMatch
            {
                RecognizedText = input,
                Kind = VoiceCommandKind.Composite,
                PartCode = part,
                ErrorCode = errors.Count == 0 ? null : string.Join(",", errors),
                ActionType = action,
                MatchedCommand = string.Join("; ", useful.Select(m => m.MatchedCommand).Where(x => !string.IsNullOrWhiteSpace(x))),
                ConfidenceScore = confidence
            };
        }

        /// <summary>
        /// Tầng 2 filter: kiểm tra text có chứa từ khoá nghiệp vụ hợp lệ không.
        /// Nguyên tắc: phải có SỐ hoặc PART PREFIX rõ ràng, hoặc ACTION từ ghép.
        /// "lỗi" đơn thuần KHÔNG đủ — phải kèm số hoặc part.
        /// Câu quá dài (>8 từ) không có số/part prefix cũng bị reject.
        /// </summary>
        private static bool ContainsBusinessKeyword(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;

            string lower = text.ToLowerInvariant();
            int wordCount = lower.Split(new[] { ' ', ',', '.', '!', '?', ';' },
                StringSplitOptions.RemoveEmptyEntries).Length;

            // Chứa chữ số → có thể là mã lỗi (số + gì đó là OK)
            if (lower.Any(char.IsDigit)) return true;

            // Từ số tiếng Việt (phát âm rõ, không lẫn với hội thoại thông thường)
            string[] numberWords = {
                "mười", "mươi", "mùi", "một", "hai", "bốn", "năm",
                "sáu", "bảy", "tám", "chín", "mốt", "lăm", "mướng"
            };
            foreach (var w in numberWords)
                if (lower.Contains(w)) return true;

            // Chữ Part đứng đầu hoặc đứng đơn (A, bê, xê, sê...)
            string[] partPrefixes = { "bê ", "xê ", "sê ", "bê,", "xê,", "sê,", "bê\t", "xê\t", "sê\t" };
            foreach (var p in partPrefixes)
                if (lower.StartsWith(p) || lower.Contains(" " + p.Trim())) return true;
            // Chữ đơn A/B/C/D/E/F đứng một mình đầu câu
            if (System.Text.RegularExpressions.Regex.IsMatch(lower, @"^[a-f][\s,\.]")
                || lower == "a" || lower == "b" || lower == "c"
                || lower == "d" || lower == "e" || lower == "f") return true;

            // Action rõ ràng (phrase đầy đủ, không phải từ đơn)
            string[] actionPhrases = {
                "không đạt", "không đặt", "hông đạt",
                "đặt lại", "đạt lại", "lỗi lại"
            };
            foreach (var w in actionPhrases)
                if (lower.Contains(w)) return true;

            // "đạt" đơn hoặc "pass" đơn OK (câu ngắn ≤3 từ)
            if (wordCount <= 3 && (lower.Contains("đạt") || lower.Contains("pass"))) return true;

            // Câu dài mà không có gì đặc trưng → reject
            return false;
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

        private static double CalculatePeak(byte[] buffer, int bytesRecorded)
        {
            if (bytesRecorded <= 0)
            {
                return 0;
            }

            double peak = 0;
            for (int i = 0; i < bytesRecorded - 1; i += 2)
            {
                short sample = BitConverter.ToInt16(buffer, i);
                double normalized = Math.Abs(sample / 32768.0);
                if (normalized > peak) peak = normalized;
            }

            return peak;
        }

        private string BuildAudioDiagnostics()
        {
            double avgRms = _recordingChunks == 0 ? 0 : _recordingRmsSum / _recordingChunks;
            double seconds = _recordedBytes <= 0 ? 0 : _recordedBytes / (double)(SampleRate * 2);
            return $"len={seconds:F1}s speech={_speechDetected} chunks={_speechChunks}/{_recordingChunks} avgRms={avgRms:F4} maxRms={_recordingMaxRms:F4} peak={_recordingMaxPeak:F2} gain={InputGain:F1}";
        }

        private static bool LooksLikeNoSpeechTranscript(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return true;
            }

            string compact = new string(text.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            if (compact.Length == 0)
            {
                return true;
            }

            return compact.All(ch =>
                ch == '-' ||
                ch == '_' ||
                ch == '.' ||
                ch == ',' ||
                ch == '[' ||
                ch == ']' ||
                ch == '(' ||
                ch == ')' ||
                ch == '…');
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
                    // HighQuality: nhạy hơn, ít lọc âm hơn, phù hợp cho văn phòng yên tĩnh
                    OperatingMode = OperatingMode.HighQuality
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
