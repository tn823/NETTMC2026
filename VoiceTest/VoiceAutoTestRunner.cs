using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;
using NETTMC.VoiceRecognition;

#nullable disable

namespace VoiceTest
{
    public class VoiceTestCaseResult
    {
        public int    Stt            { get; set; }
        public string InputText      { get; set; }
        public string ExpectedPart   { get; set; }
        public string ExpectedError  { get; set; }
        public string ExpectedAction { get; set; }

        public string RecognizedText { get; set; }
        public string ActualPart     { get; set; }
        public string ActualError    { get; set; }
        public string ActualAction   { get; set; }
        public double Confidence     { get; set; }
        public double ProcessingTimeSec { get; set; }

        public bool PartOk   => string.IsNullOrEmpty(ExpectedPart)   || ExpectedPart   == ActualPart;
        public bool ErrorOk  => string.IsNullOrEmpty(ExpectedError)  || ExpectedError  == ActualError;
        public bool ActionOk => string.IsNullOrEmpty(ExpectedAction) || ExpectedAction == ActualAction;
        public bool IsPass   => PartOk && ErrorOk && ActionOk;

        public string Note { get; set; }
    }

    public class VoiceTestCase
    {
        public int    Stt            { get; set; }
        public string InputText      { get; set; }
        public string ExpectedPart   { get; set; }
        public string ExpectedError  { get; set; }
        public string ExpectedAction { get; set; }
        public string Note           { get; set; }
    }

    /// <summary>
    /// Chạy toàn bộ test case tự động: Google TTS → WAV → VoiceEngine → So sánh.
    /// Test cases CHỈ dùng số REASON_ID — người dùng nhìn số trên nút UI và đọc số đó.
    /// Xem: d:\NETTMC_\docs\docs_new\voice_core_context.txt
    /// </summary>
    public class VoiceAutoTestRunner
    {
        private readonly VoiceEngine _engine;
        private readonly Action<string> _log;
        private readonly string _tempDir;

        private static readonly HttpClient _httpClient = new HttpClient();

        // ── 20 test case chuẩn ──────────────────────────────────────────────
        // QUY TẮC: CHỈ dùng số REASON_ID, KHÔNG dùng tên lỗi (hở keo, lem keo...)
        // Người dùng nhìn số trên nút → đọc số đó → VoiceEngine parse số
        // Ref: d:\NETTMC_\docs\docs_new\voice_core_context.txt
        public static readonly IReadOnlyList<VoiceTestCase> DefaultTestCases =
            new List<VoiceTestCase>
            {
                // ── NGUYÊN TẮC ─────────────────────────────────────────────────────────────
                // • InputText viết đúng như người Việt NÓI → Google TTS phát âm tự nhiên → Whisper nhận tốt
                // • Dùng âm tiết kép (bê bê, xê xê, đê đê...) thay ký tự đơn B/C/D vì Whisper nhận ký tự đơn rất kém
                // • Part A dùng "a a" hoặc "lỗi a" vì âm "a" đơn dễ bị Whisper nuốt
                // • KHÔNG test action (không đạt / đạt) — chỉ kiểm tra Part + Error ID
                // • 16 REASON_ID cố định của A14: 1,2,3,4,5,6,8,9,12,17,18,21,22,23,79,82
                // • Thứ tự: 17, 18, 21 ĐẦU TIÊN (lỗi thường gặp nhất)
                // ─────────────────────────────────────────────────────────────────────────────

                // ── Ưu tiên cao: 3 lỗi thường gặp nhất ──────────────────────────────────
                new VoiceTestCase { Stt=1,  InputText="a a mười bảy",          ExpectedPart="A", ExpectedError="17", Note="[PRIORITY] #17 lem keo — Part A" },
                new VoiceTestCase { Stt=2,  InputText="bê bê mười tám",        ExpectedPart="B", ExpectedError="18", Note="[PRIORITY] #18 hở keo — Part B" },
                new VoiceTestCase { Stt=3,  InputText="đê đê hai mươi mốt",    ExpectedPart="D", ExpectedError="21", Note="[PRIORITY] #21 vệ sinh — Part D" },

                // ── Số đơn (1–9) ──────────────────────────────────────────────────────────
                new VoiceTestCase { Stt=4,  InputText="xê xê một",             ExpectedPart="C", ExpectedError="1",  Note="#1 Part C" },
                new VoiceTestCase { Stt=5,  InputText="bê bê hai",             ExpectedPart="B", ExpectedError="2",  Note="#2 Part B" },
                new VoiceTestCase { Stt=6,  InputText="a a ba",                ExpectedPart="A", ExpectedError="3",  Note="#3 Part A" },
                new VoiceTestCase { Stt=7,  InputText="xê xê bốn",             ExpectedPart="C", ExpectedError="4",  Note="#4 Part C" },
                new VoiceTestCase { Stt=8,  InputText="a a năm",               ExpectedPart="A", ExpectedError="5",  Note="#5 Part A" },
                new VoiceTestCase { Stt=9,  InputText="bê bê sáu",             ExpectedPart="B", ExpectedError="6",  Note="#6 Part B" },
                new VoiceTestCase { Stt=10, InputText="ê ê tám",               ExpectedPart="E", ExpectedError="8",  Note="#8 Part E" },
                new VoiceTestCase { Stt=11, InputText="ép ép chín",            ExpectedPart="F", ExpectedError="9",  Note="#9 Part F" },

                // ── Số 2 chữ số ───────────────────────────────────────────────────────────
                new VoiceTestCase { Stt=12, InputText="a a mười hai",          ExpectedPart="A", ExpectedError="12", Note="#12 Part A" },
                new VoiceTestCase { Stt=13, InputText="ê ê hai mươi hai",      ExpectedPart="E", ExpectedError="22", Note="#22 Part E" },
                new VoiceTestCase { Stt=14, InputText="đê đê hai mươi ba",     ExpectedPart="D", ExpectedError="23", Note="#23 Part D" },
                new VoiceTestCase { Stt=15, InputText="ép ép bảy chín",        ExpectedPart="F", ExpectedError="79", Note="#79 Part F — 'bảy/bày chín' tắt, rule map cả 2 dạng thanh điệu" },
                new VoiceTestCase { Stt=16, InputText="bê bê tám hai",         ExpectedPart="B", ExpectedError="82", Note="#82 Part B — 'tám hai' ngắn gọn, tránh Whisper nhầm thành 802" },
                new VoiceTestCase { Stt=17, InputText="bê bê tám mươi hai",    ExpectedPart="B", ExpectedError="82", Note="#82 Part B — dạng đầy đủ để verify thêm" },
            };

        public VoiceAutoTestRunner(VoiceEngine engine, Action<string> logAction, string tempDir = null)
        {
            _engine  = engine;
            _log     = logAction ?? Console.WriteLine;
            _tempDir = tempDir ?? Path.Combine(Path.GetTempPath(), "VoiceAutoTest");
            Directory.CreateDirectory(_tempDir);
        }

        // ──────────────────────────────────────────────────────────────────
        // CHẠY TEST
        // ──────────────────────────────────────────────────────────────────

        public async Task<List<VoiceTestCaseResult>> RunAllAsync(
            IReadOnlyList<VoiceTestCase> cases = null,
            IProgress<(int current, int total)> progress = null)
        {
            cases ??= DefaultTestCases;
            var results = new List<VoiceTestCaseResult>();
            int idx = 0;

            foreach (var tc in cases)
            {
                idx++;
                progress?.Report((idx, cases.Count));
                _log($"\n▶ [{tc.Stt:D2}/{cases.Count}] \"{tc.InputText}\"");

                var result = await RunSingleAsync(tc);
                results.Add(result);

                string icon = result.IsPass ? "✓ PASS" : "✗ FAIL";
                _log($"  {icon}  Part={result.ActualPart ?? "-"} Error={result.ActualError ?? "-"} Action={result.ActualAction ?? "-"}  ({result.Confidence*100:F0}%) - {result.ProcessingTimeSec:F2}s");

                if (!result.IsPass)
                {
                    if (!result.PartOk)   _log($"  ⚠ Part   Expected={result.ExpectedPart}  Got={result.ActualPart ?? "(không nhận ra)"}");
                    if (!result.ErrorOk)  _log($"  ⚠ Error  Expected={result.ExpectedError} Got={result.ActualError ?? "(không nhận ra)"}");
                    if (!result.ActionOk) _log($"  ⚠ Action Expected={result.ExpectedAction} Got={result.ActualAction ?? "(không nhận ra)"}");
                }
            }

            PrintSummary(results);
            return results;
        }

        public async Task<VoiceTestCaseResult> RunSingleAsync(VoiceTestCase tc)
        {
            var result = new VoiceTestCaseResult
            {
                Stt            = tc.Stt,
                InputText      = tc.InputText,
                ExpectedPart   = tc.ExpectedPart,
                ExpectedError  = tc.ExpectedError,
                ExpectedAction = tc.ExpectedAction,
                Note           = tc.Note,
            };

            string wavPath = Path.Combine(_tempDir, $"test_{tc.Stt:D2}.wav");
            var caseSw = System.Diagnostics.Stopwatch.StartNew();

            try
            {
                _log($"  ⬇ Tải Google TTS: \"{tc.InputText}\"...");
                await GenerateWavAsync(tc.InputText, wavPath);
            }
            catch (Exception ex)
            {
                _log($"  ✗ Lỗi TTS: {ex.Message}");
                result.RecognizedText = $"[TTS Error] {ex.Message}";
                return result;
            }

            VoiceMatchResult capturedFirst = null;
            var allMatches = new List<VoiceMatchResult>();

            void OnRecognized(object s, VoiceMatchResult m)
            {
                allMatches.Add(m);
                if (capturedFirst == null) capturedFirst = m;
            }

            _engine.CommandRecognized += OnRecognized;
            try
            {
                await _engine.RecognizeFromFileAsync(wavPath);
                await Task.Delay(200);
            }
            finally
            {
                _engine.CommandRecognized -= OnRecognized;
            }

            if (allMatches.Count == 0)
            {
                result.RecognizedText = "(không nhận ra)";
                result.Confidence     = 0;
                return result;
            }

            result.RecognizedText = capturedFirst?.RecognizedText ?? "";

            foreach (var m in allMatches)
            {
                if (m.ParsedCommand == null) continue;

                switch (m.ParsedCommand.Kind)
                {
                    case VoiceCommandKind.Part:
                        result.ActualPart ??= m.ParsedCommand.PartCode;
                        break;
                    case VoiceCommandKind.Error:
                        result.ActualError ??= m.ParsedCommand.ErrorCode;
                        break;
                    case VoiceCommandKind.Action:
                        result.ActualAction ??= m.ParsedCommand.ActionType;
                        break;
                    case VoiceCommandKind.Composite:
                        result.ActualPart  ??= m.ParsedCommand.PartCode;
                        result.ActualError ??= m.ParsedCommand.ErrorCode;
                        break;
                }

                result.Confidence = Math.Max(result.Confidence, m.ConfidenceScore);
            }

            caseSw.Stop();
            result.ProcessingTimeSec = caseSw.Elapsed.TotalSeconds;

            return result;
        }

        // ──────────────────────────────────────────────────────────────────
        // GOOGLE TTS → WAV
        // ──────────────────────────────────────────────────────────────────

        private static async Task GenerateWavAsync(string text, string outputPath)
        {
            byte[] mp3Bytes = await DownloadGoogleTtsAsync(text);

            using var mp3Stream  = new MemoryStream(mp3Bytes);
            using var mp3Reader  = new Mp3FileReader(mp3Stream);
            var targetFormat     = new WaveFormat(16000, 16, 1);

            using var resampler  = new MediaFoundationResampler(mp3Reader, targetFormat);
            resampler.ResamplerQuality = 60;
            WaveFileWriter.CreateWaveFile(outputPath, resampler);
        }

        private static async Task<byte[]> DownloadGoogleTtsAsync(string text)
        {
            string encoded = Uri.EscapeDataString(text);
            string url = $"https://translate.google.com/translate_tts?ie=UTF-8&q={encoded}&tl=vi&client=tw-ob&ttsspeed=0.8";

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 "
              + "(KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            request.Headers.Add("Referer", "https://translate.google.com/");

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }

        // ──────────────────────────────────────────────────────────────────
        // IN KẾT QUẢ
        // ──────────────────────────────────────────────────────────────────

        private void PrintSummary(List<VoiceTestCaseResult> results)
        {
            int pass   = results.Count(r => r.IsPass);
            double pct = results.Count == 0 ? 0 : pass * 100.0 / results.Count;

            var sb = new StringBuilder();
            sb.AppendLine("\n══════════════════════════════════════════════");
            sb.AppendLine($"  KẾT QUẢ AUTO TEST: {pass}/{results.Count} PASS  ({pct:F0}%)");
            sb.AppendLine("══════════════════════════════════════════════");

            foreach (var r in results.Where(r => !r.IsPass))
            {
                sb.AppendLine($"  FAIL [{r.Stt:D2}] \"{r.InputText}\"");
                sb.AppendLine($"       Nghe: {r.RecognizedText ?? "-"}");
                if (!r.PartOk)   sb.AppendLine($"       Part   exp={r.ExpectedPart}  got={r.ActualPart ?? "-"}");
                if (!r.ErrorOk)  sb.AppendLine($"       Lỗi#   exp={r.ExpectedError} got={r.ActualError ?? "-"}");
                if (!r.ActionOk) sb.AppendLine($"       Action exp={r.ExpectedAction} got={r.ActualAction ?? "-"}");
            }

            sb.AppendLine("══════════════════════════════════════════════");
            _log(sb.ToString());
        }

        public void SaveReport(List<VoiceTestCaseResult> results, string outputPath)
        {
            string dir = Path.GetDirectoryName(outputPath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            int pass   = results.Count(r => r.IsPass);
            double pct = pass * 100.0 / Math.Max(1, results.Count);

            var sb = new StringBuilder();
            sb.AppendLine($"# Voice Auto Test Report — {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine($"> **{pass}/{results.Count} PASS ({pct:F0}%)**");
            sb.AppendLine();

            sb.AppendLine("| # | Voice Mẫu | Whisper Nghe Được | Part | Lỗi# | Action | Kỳ vọng | ✓? | ⏱ |");
            sb.AppendLine("|---|-----------|-------------------|------|------|--------|---------|-----|-----|");

            foreach (var r in results)
            {
                string status = r.IsPass ? "✓" : "✗";
                string heard  = r.RecognizedText ?? "";
                string exp    = "";
                if (!string.IsNullOrEmpty(r.ExpectedPart) || !string.IsNullOrEmpty(r.ExpectedError))
                    exp = $"Lỗi·{r.ExpectedPart}·#{r.ExpectedError}·{r.ExpectedAction}";
                else if (!string.IsNullOrEmpty(r.ExpectedAction))
                    exp = r.ExpectedAction;

                sb.AppendLine(
                    $"| {r.Stt:D2} " +
                    $"| {r.InputText} " +
                    $"| {heard} " +
                    $"| {r.ActualPart ?? "-"} " +
                    $"| {r.ActualError ?? "-"} " +
                    $"| {r.ActualAction ?? "-"} " +
                    $"| {exp} " +
                    $"| {status} " +
                    $"| {r.ProcessingTimeSec:F1}s |"
                );
            }

            var fails = results.Where(r => !r.IsPass).ToList();
            if (fails.Any())
            {
                sb.AppendLine();
                sb.AppendLine("## ✗ FAIL Details");
                sb.AppendLine();
                foreach (var r in fails)
                {
                    sb.AppendLine("```");
                    sb.AppendLine($"[{r.Stt:D2}] {r.InputText}");
                    sb.AppendLine($"     Nghe được : {r.RecognizedText ?? "(không nhận ra)"}");
                    if (!r.PartOk)   sb.AppendLine($"     Part   exp={r.ExpectedPart}  got={r.ActualPart ?? "-"}");
                    if (!r.ErrorOk)  sb.AppendLine($"     Lỗi#   exp={r.ExpectedError} got={r.ActualError ?? "-"}");
                    if (!r.ActionOk) sb.AppendLine($"     Action exp={r.ExpectedAction} got={r.ActualAction ?? "-"}");
                    sb.AppendLine("```");
                }
            }

            File.WriteAllText(outputPath, sb.ToString(), Encoding.UTF8);
            _log($"[Report] Đã lưu: {outputPath}");
        }
    }
}
