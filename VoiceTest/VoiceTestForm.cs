using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NETTMC.VoiceRecognition;

namespace VoiceTest
{
    public partial class VoiceTestForm : Form, IVoiceEnabledForm
    {
        private VoiceEngine _voiceEngine;
        private bool _isRecordingVoice = false;

        public VoiceTestForm()
        {
            InitializeComponent();
            _voiceEngine = new VoiceEngine();
            // Đẩy dữ liệu giả lập vào engine
            _voiceEngine.SetCommandDefinitions(BuildVoiceCommands());

            // Đăng ký sự kiện (tên chuẩn của class VoiceEngine)
            _voiceEngine.LogMessage += (s, msg) => Log(msg);
            _voiceEngine.CommandRecognized += VoiceEngine_CommandRecognized;
            _voiceEngine.StateChanged += VoiceEngine_StateChanged;

            // Đăng ký sự kiện click cho button
            buttonVoiceTest.Click += buttonVoiceTest_Click;
            this.FormClosing += VoiceTestForm_FormClosing;
            this.Load += VoiceTestForm_Load;
        }

        private async void VoiceTestForm_Load(object sender, EventArgs e)
        {
            try
            {
                await _voiceEngine.InitializeAsync();
                Log("Đã khởi tạo mô hình Voice Whisper thành công. Bạn có thể bắt đầu thu âm.");
            }
            catch (Exception ex)
            {
                Log("Lỗi khởi tạo Whisper: " + ex.Message);
            }
        }

        public VoiceActionSupport SupportedActions => VoiceActionSupport.FullEOL;

        public IReadOnlyCollection<VoiceCommandDefinition> BuildVoiceCommands()
        {
            var list = new List<VoiceCommandDefinition>();
            // Load các bộ phận (Part)
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "A", DisplayText = "A", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("A") ? VoiceAliasHelper.LetterViAliases["A"] : new[] { "a" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "B", DisplayText = "B", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("B") ? VoiceAliasHelper.LetterViAliases["B"] : new[] { "bê" } });
            list.Add(new VoiceCommandDefinition { Kind = VoiceCommandKind.Part, Code = "C", DisplayText = "C", Aliases = VoiceAliasHelper.LetterViAliases.ContainsKey("C") ? VoiceAliasHelper.LetterViAliases["C"] : new[] { "xê" } });

            // Load toàn bộ 82 mã lỗi (Errors)
            for (int i = 1; i <= 82; i++)
            {
                string code = i.ToString();
                list.Add(new VoiceCommandDefinition 
                { 
                    Kind = VoiceCommandKind.Error, 
                    Code = code, 
                    DisplayText = code, 
                    Aliases = VoiceAliasHelper.BuildReasonAliases(code, "").ToArray() 
                });
            }
            
            // Action commands (pass, fail, repass, refail...)
            foreach(var action in VoiceAliasHelper.BuildActionCommands(SupportedActions))
            {
                list.Add(action);
            }
            return list;
        }

        public void SelectPart(string partCode)
        {
            Log($"[HÀNH ĐỘNG] Giao diện tự động chọn bộ phận: {partCode}");
        }

        public void SelectError(string errorCode)
        {
            Log($"[HÀNH ĐỘNG] Giao diện tự động chọn lỗi: {errorCode}");
        }

        public void ConfirmAction(string actionType)
        {
            Log($"[HÀNH ĐỘNG] Giao diện xác nhận phím bấm: {actionType}");
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => Log(message)));
                return;
            }
            textBoxVoiceTest.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        }

        private void VoiceEngine_CommandRecognized(object sender, VoiceMatchResult match)
        {
            Log($"\r\n==========================================");
            Log($"[KẾT QUẢ NHẬN DIỆN]");
            Log($" - Văn bản người dùng nói: '{match.RecognizedText}'");
            Log($" - Lệnh khớp trong từ điển: '{match.MatchedCommand}'");
            
            if (match.ParsedCommand != null)
            {
                Log($" - Phân loại: {match.ParsedCommand.Kind}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Part || match.ParsedCommand.Kind == VoiceCommandKind.Composite)
                    Log($" - Mã Bộ Phận trích xuất: {match.ParsedCommand.PartCode}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Error || match.ParsedCommand.Kind == VoiceCommandKind.Composite)
                    Log($" - Mã Lỗi trích xuất: {match.ParsedCommand.ErrorCode}");
                if (match.ParsedCommand.Kind == VoiceCommandKind.Action)
                    Log($" - Hành Động trích xuất: {match.ParsedCommand.ActionType}");
            }
            
            Log($" - Độ tự tin (Confidence): {match.ConfidenceScore * 100:F0}%");
            Log($"==========================================\r\n");
        }

        private void VoiceEngine_StateChanged(object sender, VoiceEngineState state)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => VoiceEngine_StateChanged(sender, state)));
                return;
            }
            _isRecordingVoice = (state == VoiceEngineState.Recording || state == VoiceEngineState.Processing);
            buttonVoiceTest.BackColor = (state == VoiceEngineState.Recording) ? Color.LightCoral : Color.White;
        }

        private async void buttonVoiceTest_Click(object sender, EventArgs e)
        {
            if (_voiceEngine.IsRecording)
            {
                await _voiceEngine.StopAndRecognizeAsync();
            }
            else
            {
                await _voiceEngine.StartSmartPushToTalkAsync();
            }
        }

        private void VoiceTestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _voiceEngine?.Dispose();
        }
    }
}
