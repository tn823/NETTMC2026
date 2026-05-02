using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using NETTMC.VoiceRecognition;
using System.Linq;

namespace VoiceTest
{
    public partial class VoiceTestForm : Form, IVoiceEnabledForm
    {
        private VoiceEngine _voiceEngine;
        private bool _isRecordingVoice = false;
        private bool _isAutoMode       = false;   // Chế độ vòng lặp tự động
        private bool _isAutoTesting    = false;   // Chế độ chạy test tự động

        // Auto-loop file log
        private StreamWriter _autoLoopLogWriter;
        private string _autoLoopLogPath;
        private int _autoLoopSeq;

        public VoiceTestForm()
        {
            InitializeComponent();
            _voiceEngine = new VoiceEngine();
            _voiceEngine.SetCommandDefinitions(BuildVoiceCommands());

            _voiceEngine.LogMessage       += (s, msg) => Log(msg);
            _voiceEngine.CommandRecognized += VoiceEngine_CommandRecognized;
            _voiceEngine.StateChanged      += VoiceEngine_StateChanged;

            buttonVoiceTest.Click     += buttonVoiceTest_Click;
            buttonAutoLoop.Click      += buttonAutoLoop_Click;
            buttonRunAutoTest.Click   += buttonRunAutoTest_Click;
            this.FormClosing          += VoiceTestForm_FormClosing;
            this.Load                 += VoiceTestForm_Load;
        }

        // ── Khởi tạo ────────────────────────────────────────────────────
        private async void VoiceTestForm_Load(object sender, EventArgs e)
        {
            try
            {
                Log("Đang khởi tạo Whisper...");
                await _voiceEngine.InitializeAsync();
                Log("✓ Mô hình Voice Whisper sẵn sàng. Bắt đầu test.");
                SetButtonsEnabled(true);
            }
            catch (Exception ex)
            {
                Log("✗ Lỗi khởi tạo Whisper: " + ex.Message);
            }
        }

        // ── IVoiceEnabledForm ────────────────────────────────────────────
        public VoiceActionSupport SupportedActions => VoiceActionSupport.FullEOL;

        public IReadOnlyCollection<VoiceCommandDefinition> BuildVoiceCommands()
        {
            var list = new List<VoiceCommandDefinition>();

            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "A", DisplayText = "A", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("A") ? VoiceAliasHelper.LetterViAliases["A"] : new[] { "a" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "B", DisplayText = "B", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("B") ? VoiceAliasHelper.LetterViAliases["B"] : new[] { "bê" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "C", DisplayText = "C", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("C") ? VoiceAliasHelper.LetterViAliases["C"] : new[] { "xê" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "D", DisplayText = "D", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("D") ? VoiceAliasHelper.LetterViAliases["D"] : new[] { "dê" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "E", DisplayText = "E", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("E") ? VoiceAliasHelper.LetterViAliases["E"] : new[] { "e" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "F", DisplayText = "F", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("F") ? VoiceAliasHelper.LetterViAliases["F"] : new[] { "ép" } });

            for (int i = 1; i <= 82; i++)
            {
                string code = i.ToString();
                list.Add(new VoiceCommandDefinition
                {
                    Kind        = VoiceCommandKind.Error,
                    Code        = code,
                    DisplayText = code,
                    Aliases     = VoiceAliasHelper.BuildReasonAliases(code, "").ToArray()
                });
            }

            foreach (var action in VoiceAliasHelper.BuildActionCommands(SupportedActions))
                list.Add(action);

            return list;
        }

        public void SelectPart(string partCode)   => Log($"[HÀNH ĐỘNG] Chọn bộ phận: {partCode}");
        public void SelectError(string errorCode)  => Log($"[HÀNH ĐỘNG] Chọn lỗi: {errorCode}");
        public void ConfirmAction(string actionType) => Log($"[HÀNH ĐỘNG] Xác nhận: {actionType}");

        // ── Log helper ──────────────────────────────────────────────────
        private void Log(string message)
        {
            if (InvokeRequired) { Invoke(new Action(() => Log(message))); return; }
            textBoxVoiceTest.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void SetButtonsEnabled(bool ready)
        {
            if (InvokeRequired) { Invoke(new Action(() => SetButtonsEnabled(ready))); return; }
            buttonVoiceTest.Enabled   = ready;
            buttonAutoLoop.Enabled    = ready;
            buttonRunAutoTest.Enabled = ready;
        }

        // ── Sự kiện Engine ──────────────────────────────────────────────
        private void VoiceEngine_CommandRecognized(object sender, VoiceMatchResult match)
        {
            Log($"\n==========================================");
            Log($"[KẾT QUẢ NHẬN DIỆN]");
            Log($" - Văn bản người dùng nói: '{match.RecognizedText}'");
            Log($" - Lệnh khớp trong từ điển: '{match.MatchedCommand}'");

            if (match.ParsedCommand != null)
            {
                Log($" - Phân loại: {match.ParsedCommand.Kind}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Part || match.ParsedCommand.Kind == VoiceCommandKind.Composite)
                    Log($" - Mã Bộ Phận: {match.ParsedCommand.PartCode}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Error || match.ParsedCommand.Kind == VoiceCommandKind.Composite)
                    Log($" - Mã Lỗi: {match.ParsedCommand.ErrorCode}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Action)
                    Log($" - Hành Động: {match.ParsedCommand.ActionType}");
            }

            Log($" - Độ tự tin: {match.ConfidenceScore * 100:F0}%");
            Log($"==========================================\n");

            // Ghi vào file log auto-loop nếu writer còn mở
            if (_autoLoopLogWriter != null)
                WriteAutoLoopEntry(match);
        }

        private void VoiceEngine_StateChanged(object sender, VoiceEngineState state)
        {
            if (InvokeRequired) { Invoke(new Action(() => VoiceEngine_StateChanged(sender, state))); return; }
            _isRecordingVoice = (state == VoiceEngineState.Recording || state == VoiceEngineState.Processing);
            buttonVoiceTest.BackColor = (state == VoiceEngineState.Recording) ? Color.Red : Color.White;
        }

        // ── Nút Push-to-Talk thủ công ────────────────────────────────────
        private async void buttonVoiceTest_Click(object sender, EventArgs e)
        {
            if (_voiceEngine.IsRecording)
                await _voiceEngine.StopAndRecognizeAsync();
            else
                await _voiceEngine.StartSmartPushToTalkAsync();
        }

        // ── Nút Auto-Loop (5s thu / 5s nghỉ) ────────────────────────────
        private async void buttonAutoLoop_Click(object sender, EventArgs e)
        {
            _isAutoMode = !_isAutoMode;

            if (_isAutoMode)
            {
                _voiceEngine.IsAutoLoopMode = true;
                buttonAutoLoop.Text      = "⏹ DỮNG AUTO";
                buttonAutoLoop.BackColor = Color.OrangeRed;

                // Tạo file log MD
                string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test", "log");
                Directory.CreateDirectory(logDir);
                _autoLoopLogPath = Path.Combine(logDir, $"autoloop_{DateTime.Now:yyyyMMdd_HHmmss}.md");
                _autoLoopSeq = 0;
                _autoLoopLogWriter = new StreamWriter(_autoLoopLogPath, false, System.Text.Encoding.UTF8);
                _autoLoopLogWriter.WriteLine($"# Auto-Loop Log — {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                _autoLoopLogWriter.WriteLine();
                _autoLoopLogWriter.WriteLine("| # | Thời gian | Whisper Nghe Được | Part | Lỗi# | Action | Độ tin | ⏱ | Ghi chú |");
                _autoLoopLogWriter.WriteLine("|---|-----------|-------------------|------|------|--------|--------|-----|--------|");
                _autoLoopLogWriter.Flush();

                Log("\n[AUTO-LOOP] Bắt đầu chế độ thu âm tự động (5s thu / 5s nghỉ)...");
                Log($"[AUTO-LOOP] Log file: {_autoLoopLogPath}");
                Log("[AUTO-LOOP] Bộ lọc nghiêm ngặt BẬT: chỉ xử lý khi có Action (pass/fail/clear...)");
                _ = RunAutoLoopAsync();
            }
            else
            {
                _isAutoMode = false;
                _voiceEngine.IsAutoLoopMode = false;
                buttonAutoLoop.Text      = "▶ AUTO LOOP";
                buttonAutoLoop.BackColor = Color.LightGreen;
                Log("[AUTO-LOOP] Đã dừng. Đợi kết quả cuối...");
                // Delay 3s để cho phép in-flight RecognizeAudioAsync hoàn tất và fire CommandRecognized
                _ = Task.Delay(3000).ContinueWith(_ => {
                    FinalizeAutoLoopLog();
                    if (InvokeRequired) Invoke(new Action(() => Log("[AUTO-LOOP] Đã lưu log xong.")));
                    else Log("[AUTO-LOOP] Đã lưu log xong.");
                });
            }
        }

        /// <summary>Lưu 1 dòng kết quả vào file auto-loop log</summary>
        private void WriteAutoLoopEntry(VoiceMatchResult match)
        {
            if (_autoLoopLogWriter == null) return;
            _autoLoopSeq++;

            string text = match.RecognizedText ?? "";
            string part = "-", error = "-", action = "-", note = "";
            double conf = match.ConfidenceScore;

            if (match.ParsedCommand != null)
            {
                var cmd = match.ParsedCommand;
                if (cmd.Kind == VoiceCommandKind.Part || cmd.Kind == VoiceCommandKind.Composite)
                    part = cmd.PartCode ?? "-";
                if (cmd.Kind == VoiceCommandKind.Error || cmd.Kind == VoiceCommandKind.Composite)
                    error = cmd.ErrorCode ?? "-";
                if (cmd.Kind == VoiceCommandKind.Action)
                    action = cmd.ActionType ?? "-";
                note = cmd.Kind.ToString();
            }
            else
            {
                note = match.MatchedCommand ?? "";
            }

            _autoLoopLogWriter.WriteLine(
                $"| {_autoLoopSeq:D2} " +
                $"| {DateTime.Now:HH:mm:ss} " +
                $"| {text} " +
                $"| {part} " +
                $"| {error} " +
                $"| {action} " +
                $"| {conf:P0} " +
                $"| {match.ProcessingTimeSec:F2}s " +
                $"| {note} |");
            _autoLoopLogWriter.Flush();
        }

        private void FinalizeAutoLoopLog()
        {
            if (_autoLoopLogWriter == null) return;
            _autoLoopLogWriter.WriteLine();
            _autoLoopLogWriter.WriteLine($"> Tổng cộng: **{_autoLoopSeq}** lệnh được ghi nhận.");
            _autoLoopLogWriter.Flush();
            _autoLoopLogWriter.Close();
            _autoLoopLogWriter = null;
            if (_autoLoopSeq > 0)
                Log($"[AUTO-LOOP] Đã lưu log: {_autoLoopLogPath}");
        }

        private async Task RunAutoLoopAsync()
        {
            while (_isAutoMode)
            {
                // ── Thu âm 5s ──
                if (InvokeRequired) Invoke(new Action(() => {
                    buttonVoiceTest.BackColor = Color.Red;
                    buttonVoiceTest.Text      = "🔴 ĐANG THU ÂM";
                }));
                else {
                    buttonVoiceTest.BackColor = Color.Red;
                    buttonVoiceTest.Text      = "🔴 ĐANG THU ÂM";
                }
                Log("[AUTO] ▶ ĐANG THU ÂM... (tối đa 5 giây)");

                await _voiceEngine.StartSmartPushToTalkAsync(5000);

                // ── Nghỉ 5s ──
                if (InvokeRequired) Invoke(new Action(() => {
                    buttonVoiceTest.BackColor = Color.White;
                    buttonVoiceTest.Text      = "🎤";
                }));
                else {
                    buttonVoiceTest.BackColor = Color.White;
                    buttonVoiceTest.Text      = "🎤";
                }
                Log("[AUTO] ⏸ NGHỈ 5 GIÂY...");
                await Task.Delay(5000);
            }

            // Reset UI khi kết thúc
            if (InvokeRequired) Invoke(new Action(() => {
                buttonVoiceTest.BackColor = Color.White;
                buttonVoiceTest.Text      = "🎤";
                buttonAutoLoop.Text       = "▶ AUTO LOOP";
                buttonAutoLoop.BackColor  = Color.LightGreen;
            }));
            else {
                buttonVoiceTest.BackColor = Color.White;
                buttonVoiceTest.Text      = "🎤";
                buttonAutoLoop.Text       = "▶ AUTO LOOP";
                buttonAutoLoop.BackColor  = Color.LightGreen;
            }
        }

        // ── Nút Chạy Test Tự Động ──────────────────────────────────────
        private async void buttonRunAutoTest_Click(object sender, EventArgs e)
        {
            if (_isAutoTesting) return;
            _isAutoTesting = true;
            SetButtonsEnabled(false);

            Log("\n══════════════════════════════════════════════");
            Log("  CHẠY TEST TỰ ĐỘNG (TTS → Whisper → So sánh)");
            Log("══════════════════════════════════════════════");

            try
            {
                string tempDir    = Path.Combine(Application.StartupPath, "test", "wav_temp");
                string reportPath = Path.Combine(Application.StartupPath, "test", "log",
                                        $"autotest_{DateTime.Now:yyyyMMdd_HHmmss}.md");

                var runner = new VoiceAutoTestRunner(_voiceEngine, Log, tempDir);

                var progress = new Progress<(int current, int total)>(p =>
                {
                    if (InvokeRequired) Invoke(new Action(() =>
                        buttonRunAutoTest.Text = $"⏳ {p.current}/{p.total}"));
                    else
                        buttonRunAutoTest.Text = $"⏳ {p.current}/{p.total}";
                });

                var results = await runner.RunAllAsync(progress: progress);
                runner.SaveReport(results, reportPath);
            }
            catch (Exception ex)
            {
                Log($"✗ Lỗi Auto Test: {ex.Message}");
            }
            finally
            {
                _isAutoTesting = false;
                SetButtonsEnabled(true);
                if (InvokeRequired) Invoke(new Action(() => buttonRunAutoTest.Text = "🧪 AUTO TEST"));
                else buttonRunAutoTest.Text = "🧪 AUTO TEST";
            }
        }

        // ── Đóng form ───────────────────────────────────────────────────
        private void VoiceTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isAutoMode = false;
            FinalizeAutoLoopLog();
            _voiceEngine?.Dispose();
        }
    }
}
