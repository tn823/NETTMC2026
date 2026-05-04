using NAudio.Wave;
using System.Drawing;
using System.Speech.Synthesis;
using Whisper.net;
using Whisper.net.Ggml;

namespace VoiceWhisperApp2
{
    // ════════════════════════════════════════════════════════════════════════
    //  EventArgs
    // ════════════════════════════════════════════════════════════════════════

    public class RecognitionResultEventArgs : EventArgs
    {
        public string RawText { get; init; } = "";
        public string? MatchedCommand { get; init; }
        public double MatchScore { get; init; }
        public double Confidence { get; init; }
        public bool IsMatched { get; init; }
    }

    public class ModelStatusEventArgs : EventArgs
    {
        public string Message { get; init; } = "";
        public ModelStatusLevel Level { get; init; }
    }

    public enum ModelStatusLevel { Info, Warning, Success, Error }

    /// <summary>
    /// Dùng để bắn thông báo text + màu ra UI.
    /// Form chỉ cần: svc.MessageLogged += (_, e) => ShowMessage(e.Message, e.Color);
    /// </summary>
    public class MessageLoggedEventArgs : EventArgs
    {
        public string Message { get; init; } = "";
        public Color Color { get; init; }
    }

    // ════════════════════════════════════════════════════════════════════════
    //  VoiceRecognitionService
    // ════════════════════════════════════════════════════════════════════════

    public sealed class VoiceRecognitionService : IAsyncDisposable, IDisposable
    {
        private const string MODEL_PATH = "whisper-model.bin";
        private const GgmlType MODEL_TYPE = GgmlType.Base;
        private const int SAMPLE_RATE = 16000;

        public double Threshold { get; set; } = 0.80;

        private readonly List<string> _commands;
        private WhisperFactory? _factory;
        private WhisperProcessor? _processor;
        private WaveInEvent? _waveIn;
        private WaveFileWriter? _waveWriter;
        private MemoryStream? _audioBuffer;
        private SpeechSynthesizer? _tts;
        private bool _modelReady;
        private bool _recording;
        private bool _disposed;

        // ── Events ──────────────────────────────────────────────────────────

        /// <summary>Bắn sau mỗi lần nhận dạng xong.</summary>
        public event EventHandler<RecognitionResultEventArgs>? RecognitionCompleted;

        /// <summary>Trạng thái tải/load model.</summary>
        public event EventHandler<ModelStatusEventArgs>? ModelStatusChanged;

        /// <summary>
        /// Thông báo text + màu để hiển thị ra UI.
        /// Kết nối thẳng vào ShowMessage của Form:
        ///   svc.MessageLogged += (_, e) => ShowMessage(e.Message, e.Color);
        ///
        /// Các thông báo được bắn theo thứ tự:
        ///   1. "🔊 Nghe được: ..."  — text Whisper nghe thấy     (White)
        ///   2. "✔ Lệnh: ..."        — khi khớp lệnh              (LimeGreen)
        ///      HOẶC "⚠ Gần nhất: ..." — khi không đủ ngưỡng      (OrangeRed)
        /// </summary>
        public event EventHandler<MessageLoggedEventArgs>? MessageLogged;

        public bool IsModelReady => _modelReady;
        public bool IsRecording => _recording;

        // ── Constructor ─────────────────────────────────────────────────────

        public VoiceRecognitionService(IEnumerable<string> commands, bool enableTts = true)
        {
            _commands = new List<string>(commands);
            if (enableTts) InitializeTts();
        }

        // ════════════════════════════════════════════════════════════════════
        //  KHỞI TẠO MODEL
        // ════════════════════════════════════════════════════════════════════

        public async Task InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                RaiseStatus("⏳ Kiểm tra model…", ModelStatusLevel.Info);

                if (!File.Exists(MODEL_PATH))
                {
                    RaiseStatus("⬇ Đang tải Whisper model (~466 MB)…", ModelStatusLevel.Warning);
                    using var http = new System.Net.Http.HttpClient();
                    var dl = new WhisperGgmlDownloader(http);
                    await using var src = await dl.GetGgmlModelAsync(MODEL_TYPE);
                    await using var dst = File.OpenWrite(MODEL_PATH);
                    await src.CopyToAsync(dst, ct);
                    RaiseStatus("✔ Tải model xong!", ModelStatusLevel.Success);
                }

                RaiseStatus("⚙ Đang load model…", ModelStatusLevel.Info);

                _factory = WhisperFactory.FromPath(MODEL_PATH);
                _processor = _factory.CreateBuilder()
                    .WithLanguage("vi")
                    .WithThreads(Environment.ProcessorCount)
                    .WithSingleSegment()
                    .WithNoContext()
                    .WithPrompt(string.Join(", ", _commands))
                    .Build();

                _modelReady = true;
                RaiseStatus("✔ Whisper sẵn sàng | vi-VN | Offline", ModelStatusLevel.Success);
            }
            catch (Exception ex)
            {
                RaiseStatus($"✘ Lỗi: {ex.Message}", ModelStatusLevel.Error);
                throw;
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  GHI ÂM
        // ════════════════════════════════════════════════════════════════════

        public void StartRecording()
        {
            if (!_modelReady)
                throw new InvalidOperationException("Model chưa sẵn sàng. Gọi InitializeAsync() trước.");
            if (_recording) return;

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
            _recording = true;

            FireMessage("🎙 Đang nghe…", Color.DodgerBlue);
        }

        private void OnAudioData(object? sender, WaveInEventArgs e)
            => _waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);

        public async Task StopAndRecognizeAsync()
        {
            if (!_recording) return;
            _recording = false;
            _waveIn?.StopRecording();
            _waveWriter?.Flush();
            FireMessage("⚙ Đang xử lý…", Color.Orange);
            await RecognizeAsync();
        }

        private void OnRecordingStopped(object? sender, StoppedEventArgs e)
        {
            _waveWriter?.Flush();
            _ = RecognizeAsync();
        }

        // ════════════════════════════════════════════════════════════════════
        //  NHẬN DẠNG
        // ════════════════════════════════════════════════════════════════════

        private async Task RecognizeAsync()
        {
            if (_processor is null || _audioBuffer is null || _audioBuffer.Length < 1000)
            {
                FireMessage("Audio quá ngắn hoặc không có tiếng.", Color.OrangeRed);
                FireResult(new RecognitionResultEventArgs { RawText = "", Confidence = 0 });
                return;
            }

            try
            {
                _audioBuffer.Position = 0;

                string raw = "";
                double totalP = 0;
                int segCount = 0;

                await foreach (var seg in _processor.ProcessAsync(_audioBuffer))
                {
                    raw += seg.Text + " ";
                    totalP += seg.Probability;
                    segCount++;
                }

                raw = raw.Trim();
                double confidence = segCount > 0 ? totalP / segCount : 0;

                // Trường hợp không nghe thấy gì
                if (string.IsNullOrWhiteSpace(raw))
                {
                    FireMessage("Không nghe thấy gì. Vui lòng thử lại.", Color.OrangeRed);
                    FireResult(new RecognitionResultEventArgs { RawText = "", Confidence = confidence });
                    SpeakAsync("Không nghe thấy gì. Vui lòng thử lại.");
                    return;
                }

                // Xuất text Whisper nghe được — đây là dòng ShowMessage đầu tiên
                FireMessage($"🔊 Nghe được: \"{raw}\"", Color.White);

                // Matching
                var (matched, score) = FindBestMatch(raw);
                bool isMatch = matched != null && score >= Threshold;

                FireResult(new RecognitionResultEventArgs
                {
                    RawText = raw,
                    Confidence = confidence,
                    IsMatched = true // luôn true, để form tự xử lý
                });
            }
            catch (Exception ex)
            {
                FireMessage($"✘ Lỗi nhận dạng: {ex.Message}", Color.OrangeRed);
                RaiseStatus($"✘ Lỗi nhận dạng: {ex.Message}", ModelStatusLevel.Error);
            }
        }

        // ════════════════════════════════════════════════════════════════════
        //  TEST
        // ════════════════════════════════════════════════════════════════════

        public RecognitionResultEventArgs TestInput(string text)
        {
            FireMessage($"🔤 Test: \"{text}\"", Color.Silver);

            var (matched, score) = FindBestMatch(text);
            bool isMatch = matched != null && score >= Threshold;

            if (isMatch)
                FireMessage($"✔ Lệnh: {matched}  ({score:P0})", Color.LimeGreen);
            else
            {
                string hint = matched != null
                    ? $"Gần nhất: \"{matched}\" ({score:P0}) — dưới ngưỡng"
                    : "Không khớp lệnh nào";
                FireMessage($"⚠ {hint}", Color.OrangeRed);
            }

            var result = new RecognitionResultEventArgs
            {
                RawText = text,
                MatchedCommand = matched,
                MatchScore = score,
                Confidence = 1.0,
                IsMatched = isMatch
            };
            FireResult(result);
            return result;
        }

        // ════════════════════════════════════════════════════════════════════
        //  MATCHING (Jaro-Winkler)
        // ════════════════════════════════════════════════════════════════════

        private (string? best, double score) FindBestMatch(string input)
        {
            string normIn = Normalize(input);
            double best = 0;
            string? match = null;

            foreach (var cmd in _commands)
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
                { "loi ",   "lỗi " },
                { "ho keo", "hở keo" },
                { " do",    " dơ" }
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

        // ════════════════════════════════════════════════════════════════════
        //  TTS
        // ════════════════════════════════════════════════════════════════════

        private void InitializeTts()
        {
            _tts = new SpeechSynthesizer { Rate = 0, Volume = 100 };
            foreach (var v in _tts.GetInstalledVoices())
                if (v.VoiceInfo.Culture.Name.StartsWith("vi"))
                { _tts.SelectVoice(v.VoiceInfo.Name); break; }
        }

        private void SpeakAsync(string text)
            => Task.Run(() => { try { _tts?.Speak(text); } catch { } });

        // ════════════════════════════════════════════════════════════════════
        //  HELPER EVENTS
        // ════════════════════════════════════════════════════════════════════

        private void FireResult(RecognitionResultEventArgs e)
            => RecognitionCompleted?.Invoke(this, e);

        private void FireMessage(string message, Color color)
            => MessageLogged?.Invoke(this, new MessageLoggedEventArgs { Message = message, Color = color });

        private void RaiseStatus(string msg, ModelStatusLevel level)
            => ModelStatusChanged?.Invoke(this, new ModelStatusEventArgs { Message = msg, Level = level });

        // ════════════════════════════════════════════════════════════════════
        //  DISPOSE
        // ════════════════════════════════════════════════════════════════════

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _recording = false;
            try { _waveIn?.StopRecording(); _waveIn?.Dispose(); } catch { }
            try { _waveWriter?.Dispose(); _audioBuffer?.Dispose(); } catch { }
            try { _processor?.Dispose(); _factory?.Dispose(); } catch { }
            try { _tts?.Dispose(); } catch { }
        }

        public async ValueTask DisposeAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }
    }
}