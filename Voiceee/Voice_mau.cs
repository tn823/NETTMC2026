using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Whisper.net;
using Whisper.net.Ggml;

namespace Voiceee
{
    public partial class Voice_mau : Form
    {
        public Voice_mau()
        {
            //InitializeComponent();
            InitializeUI();
            InitializeTTS();
            _ = InitializeWhisperAsync();
        }
        // ── Audio ────────────────────────────────────────────────────────────────
        private WaveInEvent waveIn;
        private MemoryStream audioBuffer;
        private WaveFileWriter waveWriter;
        private bool isRecording;
        private CancellationTokenSource cts;

        // ── Whisper ──────────────────────────────────────────────────────────────
        private WhisperFactory whisperFactory;
        private WhisperProcessor whisperProcessor;
        private bool modelReady;
        private const string MODEL_PATH = "whisper-model.bin";
        private const GgmlType MODEL_TYPE = GgmlType.Base; // ~466 MB, best VI accuracy

        // ── TTS ──────────────────────────────────────────────────────────────────
        private SpeechSynthesizer tts;

        // ── Config ───────────────────────────────────────────────────────────────
        private const double THRESHOLD = 0.80;
        private const int SAMPLE_RATE = 16000;
        private const int RECORD_MS = 4000; // auto-submit after 4s silence

        private readonly List<string> commandList = new()
        {
            "Lỗi 1", "Lỗi 2", "Lỗi 3", "Lỗi dơ", "Hở keo"
        };
        // ── UI ───────────────────────────────────────────────────────────────────
        private Panel headerPanel, statusPanel, commandsPanel, logPanel, bottomBar;
        private Label titleLabel, subtitleLabel, statusDot, statusLabel;
        private Label confidenceLabel, commandsTitle, logTitle, modelStatusLabel;
        private RichTextBox logBox;
        private FlowLayoutPanel tagContainer;
        private ProgressBar confidenceBar;
        private Button startButton, stopButton, clearButton, testButton;
        private TextBox manualInput;
        private ProgressBar downloadProgress;
        private Label downloadLabel;
        // ── Colors ───────────────────────────────────────────────────────────────
        private readonly Color BgDark = Color.FromArgb(11, 15, 27);
        private readonly Color BgCard = Color.FromArgb(18, 24, 42);
        private readonly Color BgPanel = Color.FromArgb(22, 30, 52);
        private readonly Color AccentBlue = Color.FromArgb(56, 139, 253);
        private readonly Color AccentCyan = Color.FromArgb(0, 212, 200);
        private readonly Color AccentRed = Color.FromArgb(255, 74, 74);
        private readonly Color AccentGreen = Color.FromArgb(57, 211, 140);
        private readonly Color AccentAmber = Color.FromArgb(255, 183, 77);
        private readonly Color TextPrimary = Color.FromArgb(225, 232, 255);
        private readonly Color TextMuted = Color.FromArgb(95, 115, 165);
        private readonly Color BorderColor = Color.FromArgb(38, 52, 88);

        // ════════════════════════════════════════════════════════════════════════
       
        // ════════════════════════════════════════════════════════════════════════
        //  UI
        // ════════════════════════════════════════════════════════════════════════
        private void InitializeUI()
        {
            Text = "Voice Recognition – Whisper Offline";
            Size = new Size(880, 740);
            MinimumSize = new Size(760, 640);
            BackColor = BgDark;
            ForeColor = TextPrimary;
            Font = new Font("Segoe UI", 9.5f);
            StartPosition = FormStartPosition.CenterScreen;

            // ── Header ──────────────────────────────────────────────────────────
            headerPanel = MakePanel(DockStyle.Top, 82, BgCard);
            headerPanel.Paint += (s, e) =>
            {
                using var g = new LinearGradientBrush(Point.Empty,
                    new Point(headerPanel.Width, 0), AccentBlue, AccentCyan);
                e.Graphics.FillRectangle(g, 0, 0, headerPanel.Width, 3);
                using var pen = new Pen(Color.FromArgb(40, AccentBlue), 1);
                e.Graphics.DrawLine(pen, 0, headerPanel.Height - 1,
                                         headerPanel.Width, headerPanel.Height - 1);
            };
            titleLabel = MakeLabel("🎙  VOICE RECOGNITION – WHISPER OFFLINE",
                new Font("Segoe UI Semibold", 15f, FontStyle.Bold), TextPrimary,
                new Point(24, 16));
            subtitleLabel = MakeLabel("Nhận dạng tiếng Việt  •  Offline  •  Không cần đăng ký  •  Độ chính xác ≥ 80%",
                new Font("Segoe UI", 9f), TextMuted, new Point(26, 48));
            headerPanel.Controls.AddRange(new Control[] { titleLabel, subtitleLabel });

            // ── Model status bar ─────────────────────────────────────────────────
            var modelBar = MakePanel(DockStyle.Top, 36, Color.FromArgb(15, 20, 38));
            modelBar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(30, AccentBlue), 1);
                e.Graphics.DrawLine(pen, 0, modelBar.Height - 1, modelBar.Width, modelBar.Height - 1);
            };
            modelStatusLabel = MakeLabel("⏳  Model: đang khởi tạo…",
                new Font("Segoe UI", 8.5f), AccentAmber, new Point(16, 10));
            downloadLabel = MakeLabel("", new Font("Segoe UI", 8.5f), TextMuted, new Point(16, 10));
            downloadLabel.Visible = false;
            downloadProgress = new ProgressBar
            {
                Visible = false,
                Height = 4,
                Style = ProgressBarStyle.Continuous,
                Minimum = 0,
                Maximum = 100,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            modelBar.Controls.AddRange(new Control[] { modelStatusLabel, downloadLabel, downloadProgress });
            modelBar.Resize += (s, e) =>
            {
                downloadProgress.Width = modelBar.Width - 200;
                downloadProgress.Location = new Point(16, (modelBar.Height - 4) / 2);
                downloadLabel.Location = new Point(modelBar.Width - 180, 10);
            };

            // ── Status strip ─────────────────────────────────────────────────────
            statusPanel = MakePanel(DockStyle.Top, 48, BgPanel);
            statusPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderColor, 1);
                e.Graphics.DrawLine(pen, 0, statusPanel.Height - 1, statusPanel.Width, statusPanel.Height - 1);
            };
            statusDot = MakeLabel("●", new Font("Segoe UI", 14f), AccentRed, new Point(16, 12));
            statusLabel = MakeLabel("Đang chờ model…",
                new Font("Segoe UI", 10f), TextMuted, new Point(38, 15));
            confidenceLabel = MakeLabel("Conf: –",
                new Font("Segoe UI", 9f), TextMuted, Point.Empty);
            confidenceBar = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = 0,
                Height = 6,
                Style = ProgressBarStyle.Continuous,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            statusPanel.Controls.AddRange(new Control[]
                { statusDot, statusLabel, confidenceLabel, confidenceBar });
            statusPanel.Resize += (s, e) =>
            {
                confidenceLabel.Location = new Point(statusPanel.Width - 185, 15);
                confidenceBar.Width = 130;
                confidenceBar.Location = new Point(statusPanel.Width - 148,
                                                     (statusPanel.Height - 6) / 2);
            };

            // ── Commands ─────────────────────────────────────────────────────────
            commandsPanel = MakePanel(DockStyle.Top, 78, BgCard);
            commandsPanel.Paint += (s, e) =>
            {
                using var pen = new Pen(BorderColor, 1);
                e.Graphics.DrawLine(pen, 0, commandsPanel.Height - 1,
                                         commandsPanel.Width, commandsPanel.Height - 1);
            };
            commandsTitle = MakeLabel("LỆNH HỢP LỆ",
                new Font("Segoe UI", 7.5f, FontStyle.Bold), AccentCyan, new Point(16, 10));
            tagContainer = new FlowLayoutPanel
            {
                Location = new Point(16, 28),
                AutoSize = false,
                WrapContents = true,
                Height = 40,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
            };
            foreach (var cmd in commandList) tagContainer.Controls.Add(MakeTag(cmd));
            commandsPanel.Controls.AddRange(new Control[] { commandsTitle, tagContainer });
            commandsPanel.Resize += (s, e) => tagContainer.Width = commandsPanel.Width - 32;

            // ── Log ──────────────────────────────────────────────────────────────
            logPanel = MakePanel(DockStyle.Fill, 0, BgCard);
            logTitle = MakeLabel("NHẬT KÝ NHẬN DẠNG",
                new Font("Segoe UI", 7.5f, FontStyle.Bold), AccentCyan, new Point(16, 10));
            logBox = new RichTextBox
            {
                ReadOnly = true,
                BackColor = BgDark,
                ForeColor = TextPrimary,
                Font = new Font("Cascadia Code", 9f),
                BorderStyle = BorderStyle.None,
                ScrollBars = RichTextBoxScrollBars.Vertical,
                Anchor = AnchorStyles.None,
                Location = new Point(16, 30),
                DetectUrls = false
            };
            logPanel.Controls.AddRange(new Control[] { logTitle, logBox });
            logPanel.Resize += (s, e) =>
            {
                logBox.Width = logPanel.Width - 32;
                logBox.Height = logPanel.Height - 76;
            };

            // ── Bottom bar ───────────────────────────────────────────────────────
            bottomBar = MakePanel(DockStyle.Bottom, 62, BgPanel);
            bottomBar.Paint += (s, e) =>
            {
                using var pen = new Pen(Color.FromArgb(40, AccentBlue), 1);
                e.Graphics.DrawLine(pen, 0, 0, bottomBar.Width, 0);
            };

            startButton = MakeBtn("▶  BẮT ĐẦU", AccentGreen, Color.FromArgb(18, 38, 28));
            startButton.Location = new Point(16, 11);
            startButton.Enabled = false;
            startButton.Click += StartButton_Click;

            stopButton = MakeBtn("■  DỪNG LẠI", AccentRed, Color.FromArgb(38, 18, 18));
            stopButton.Location = new Point(150, 11);
            stopButton.Enabled = false;
            stopButton.Click += StopButton_Click;

            clearButton = MakeBtn("🗑  XÓA", TextMuted, BgPanel);
            clearButton.Size = new Size(90, 40);
            clearButton.Location = new Point(284, 11);
            clearButton.Click += (s, e) => logBox.Clear();

            var inputWrap = new Panel
            {
                BackColor = Color.Transparent,
                Size = new Size(310, 40),
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            manualInput = new TextBox
            {
                PlaceholderText = "Nhập text để test…",
                BackColor = BgDark,
                ForeColor = TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 9.5f),
                Size = new Size(210, 32),
                Location = new Point(0, 4)
            };
            testButton = MakeBtn("TEST", AccentBlue, Color.FromArgb(10, 20, 50));
            testButton.Size = new Size(88, 32);
            testButton.Location = new Point(218, 4);
            testButton.Click += TestButton_Click;
            inputWrap.Controls.AddRange(new Control[] { manualInput, testButton });

            bottomBar.Controls.AddRange(new Control[]
                { startButton, stopButton, clearButton, inputWrap });
            bottomBar.Resize += (s, e) =>
                inputWrap.Location = new Point(bottomBar.Width - 326, 11);

            // ── Assemble ─────────────────────────────────────────────────────────
            Controls.Add(logPanel);
            Controls.Add(commandsPanel);
            Controls.Add(statusPanel);
            Controls.Add(modelBar);
            Controls.Add(headerPanel);
            Controls.Add(bottomBar);

            Log("─────────────────────────────────────────", TextMuted);
            Log("  Whisper Offline STT  |  vi-VN  |  Ngưỡng 80%", AccentCyan);
            Log("─────────────────────────────────────────", TextMuted);
        }

        // ── Helpers ──────────────────────────────────────────────────────────────
        private Panel MakePanel(DockStyle dock, int h, Color bg)
        {
            var p = new Panel { Dock = dock, BackColor = bg };
            if (h > 0) p.Height = h;
            return p;
        }
        private Label MakeLabel(string text, Font font, Color color, Point loc)
            => new Label
            {
                Text = text,
                Font = font,
                ForeColor = color,
                AutoSize = true,
                Location = loc,
                BackColor = Color.Transparent
            };
        private Button MakeBtn(string text, Color fg, Color bg)
        {
            var b = new Button
            {
                Text = text,
                Size = new Size(122, 40),
                BackColor = bg,
                ForeColor = fg,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Color.FromArgb(55, fg);
            b.FlatAppearance.BorderSize = 1;
            b.FlatAppearance.MouseOverBackColor =
                Color.FromArgb(Math.Min(bg.R + 18, 255),
                               Math.Min(bg.G + 18, 255),
                               Math.Min(bg.B + 18, 255));
            return b;
        }
        private Label MakeTag(string text) => new Label
        {
            Text = text,
            AutoSize = true,
            BackColor = Color.FromArgb(28, 46, 86),
            ForeColor = AccentBlue,
            Font = new Font("Segoe UI Semibold", 9f),
            Padding = new Padding(8, 3, 8, 3),
            Margin = new Padding(0, 0, 6, 0),
            BorderStyle = BorderStyle.FixedSingle
        };

        // ════════════════════════════════════════════════════════════════════════
        //  WHISPER INIT
        // ════════════════════════════════════════════════════════════════════════
        private async Task InitializeWhisperAsync()
        {
            try
            {
                SetModelStatus("⬇  Kiểm tra model…", AccentAmber);

                // Download model if not present
                if (!File.Exists(MODEL_PATH))
                {
                    SetModelStatus("⬇  Đang tải model Whisper Small (~466 MB)…", AccentAmber);
                    ShowDownloadUI(true);
                    Log("⬇  Đang tải Whisper Small model. Vui lòng chờ…", AccentAmber);

                    using var httpClient = new System.Net.Http.HttpClient();
                    var downloader = new WhisperGgmlDownloader(httpClient);
                    await using var modelStream = await downloader.GetGgmlModelAsync(MODEL_TYPE);
                    //await using var modelStream = await WhisperGgmlDownloader.GetGgmlModelAsync(
                    //    MODEL_TYPE,
                    //    quantization: QuantizationType.NoQuantization,
                    //    cancellationToken: default);

                    await using var fileStream = File.OpenWrite(MODEL_PATH);
                    await modelStream.CopyToAsync(fileStream);

                    ShowDownloadUI(false);
                    Log("✔  Tải model xong!", AccentGreen);
                }

                // Load model
                SetModelStatus("⚙  Đang load model…", AccentAmber);
                Log("⚙  Đang khởi tạo Whisper…", TextMuted);

                whisperFactory = WhisperFactory.FromPath(MODEL_PATH);
                whisperProcessor = whisperFactory.CreateBuilder()
                    .WithLanguage("vi")
                    .WithThreads(Environment.ProcessorCount)
                    .WithSingleSegment()        // Chỉ 1 đoạn ngắn
                    .WithNoContext()            // Không cần context
                    .WithPrompt("Lỗi 1, Lỗi 2, Lỗi 3, Lỗi dơ, Hở keo") // Gợi ý từ vựng
                    .Build();

                modelReady = true;
                SetModelStatus("✔  Whisper Small  |  vi-VN  |  Offline  |  Sẵn sàng", AccentGreen);
                Log("✔  Model sẵn sàng! Nhấn START để bắt đầu.", AccentGreen);

                SafeInvoke(() =>
                {
                    startButton.Enabled = true;
                    statusLabel.Text = "Nhấn START để bắt đầu…";
                    statusLabel.ForeColor = TextMuted;
                });
            }
            catch (Exception ex)
            {
                SetModelStatus($"✘  Lỗi: {ex.Message}", AccentRed);
                Log($"✘  Lỗi khởi tạo Whisper: {ex.Message}", AccentRed);
            }
        }

        private void ShowDownloadUI(bool show)
        {
            SafeInvoke(() =>
            {
                downloadProgress.Visible = show;
                downloadLabel.Visible = show;
                if (show) downloadLabel.Text = "Đang tải…";
            });
        }

        private void SetModelStatus(string text, Color color)
        {
            SafeInvoke(() =>
            {
                modelStatusLabel.Text = text;
                modelStatusLabel.ForeColor = color;
            });
        }

        // ════════════════════════════════════════════════════════════════════════
        //  AUDIO RECORDING
        // ════════════════════════════════════════════════════════════════════════
        private void StartButton_Click(object sender, EventArgs e)
        {
            if (!modelReady) return;
            StartRecording();
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            StopRecordingAndRecognize();
        }

        private void StartRecording()
        {
            try
            {
                audioBuffer = new MemoryStream();
                waveIn = new WaveInEvent
                {
                    WaveFormat = new WaveFormat(SAMPLE_RATE, 16, 1),
                    BufferMilliseconds = 100
                };

                waveWriter = new WaveFileWriter(audioBuffer, waveIn.WaveFormat);
                waveIn.DataAvailable += OnAudioData;
                waveIn.RecordingStopped += OnRecordingStopped;
                waveIn.StartRecording();
                isRecording = true;

                SafeInvoke(() =>
                {
                    startButton.Enabled = false;
                    stopButton.Enabled = true;
                    statusDot.ForeColor = AccentGreen;
                    statusLabel.Text = "🎙  Đang ghi âm… (nhấn DỪNG khi nói xong)";
                    statusLabel.ForeColor = AccentGreen;
                });

                Log("🎙  Bắt đầu ghi âm…", AccentGreen);
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi mic: {ex.Message}", AccentRed);
            }
        }

        private void OnAudioData(object sender, WaveInEventArgs e)
        {
            waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void StopRecordingAndRecognize()
        {
            if (!isRecording) return;
            isRecording = false;
            waveIn?.StopRecording();
            SafeInvoke(() =>
            {
                stopButton.Enabled = false;
                statusLabel.Text = "⚙  Đang nhận dạng…";
                statusLabel.ForeColor = AccentAmber;
                statusDot.ForeColor = AccentAmber;
            });
            Log("⚙  Đang xử lý audio với Whisper…", AccentAmber);
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            waveWriter?.Flush();
            _ = RecognizeAudioAsync();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  WHISPER RECOGNITION
        // ════════════════════════════════════════════════════════════════════════
        private async Task RecognizeAudioAsync()
        {
            try
            {
                if (audioBuffer == null || audioBuffer.Length < 1000)
                {
                    Log("⚠  Audio quá ngắn.", AccentRed);
                    ReadyToRecord();
                    return;
                }

                audioBuffer.Position = 0;

                string recognizedText = "";
                double avgProb = 0;
                int segCount = 0;

                await foreach (var segment in whisperProcessor.ProcessAsync(audioBuffer))
                {
                    recognizedText += segment.Text + " ";
                    avgProb += segment.Probability;
                    segCount++;
                }

                recognizedText = recognizedText.Trim();
                if (segCount > 0) avgProb /= segCount;

                if (string.IsNullOrWhiteSpace(recognizedText))
                {
                    Log("⚠  Không nhận được gì.", AccentRed);
                    SpeakAsync("Không nghe thấy gì. Vui lòng thử lại.");
                    ReadyToRecord();
                    return;
                }

                Log($"🔊  Nghe được: \"{recognizedText}\"  (prob: {avgProb:P0})", TextPrimary);
                UpdateConfidence((int)(avgProb * 100));

                var (matched, score) = FindBestMatch(recognizedText);

                if (matched != null && score >= THRESHOLD)
                    HandleMatch(matched, score);
                else
                    HandleNoMatch(recognizedText, matched, score);
            }
            catch (Exception ex)
            {
                Log($"✘  Lỗi nhận dạng: {ex.Message}", AccentRed);
            }
            finally
            {
                ReadyToRecord();
            }
        }

        private void ReadyToRecord()
        {
            SafeInvoke(() =>
            {
                startButton.Enabled = true;
                stopButton.Enabled = false;
                statusDot.ForeColor = AccentBlue;
                statusLabel.Text = "Sẵn sàng — nhấn START để ghi âm tiếp";
                statusLabel.ForeColor = AccentBlue;
            });
        }

        // ════════════════════════════════════════════════════════════════════════
        //  MATCHING
        // ════════════════════════════════════════════════════════════════════════
        private (string best, double score) FindBestMatch(string input)
        {
            string normIn = Normalize(input);
            double best = 0;
            string match = null;

            foreach (var cmd in commandList)
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
            // common misrecognitions
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

        // ════════════════════════════════════════════════════════════════════════
        //  HANDLERS
        // ════════════════════════════════════════════════════════════════════════
        private void HandleMatch(string cmd, double score)
        {
            Log($"✔  Khớp lệnh: \"{cmd}\"  (score: {score:P0})", AccentGreen, bold: true);
            SafeInvoke(() =>
            {
                statusDot.ForeColor = AccentGreen;
                statusLabel.Text = $"✔  {cmd}";
                statusLabel.ForeColor = AccentGreen;
            });
            FlashTag(cmd, true);
            SpeakAsync($"Đã nhận lệnh {cmd}");
            Task.Delay(1800).ContinueWith(_ =>
            {
                if (!IsDisposed) SafeInvoke(() => FlashTag(cmd, false));
            });
        }

        private void HandleNoMatch(string input, string closest, double score)
        {
            string info = closest != null
                ? $"Gần nhất: \"{closest}\" ({score:P0}) — dưới ngưỡng {THRESHOLD:P0}"
                : "Không tìm thấy lệnh phù hợp";
            Log($"⚠  {info}", AccentRed);
            SafeInvoke(() =>
            {
                statusDot.ForeColor = AccentRed;
                statusLabel.Text = "⚠  Không khớp lệnh";
                statusLabel.ForeColor = AccentRed;
            });
            string valid = string.Join(", ", commandList);
            SpeakAsync($"Không nhận ra lệnh. Các lệnh hợp lệ gồm: {valid}");
        }

        private void FlashTag(string cmd, bool on)
        {
            foreach (Control c in tagContainer.Controls)
                if (c is Label lbl && lbl.Text == cmd)
                {
                    lbl.BackColor = on ? Color.FromArgb(25, 72, 45) : Color.FromArgb(28, 46, 86);
                    lbl.ForeColor = on ? AccentGreen : AccentBlue;
                    lbl.BorderStyle = on ? BorderStyle.Fixed3D : BorderStyle.FixedSingle;
                }
        }

        // ════════════════════════════════════════════════════════════════════════
        //  TTS
        // ════════════════════════════════════════════════════════════════════════
        private void InitializeTTS()
        {
            tts = new SpeechSynthesizer();
            tts.Rate = 0;
            tts.Volume = 100;
            foreach (var v in tts.GetInstalledVoices())
                if (v.VoiceInfo.Culture.Name.StartsWith("vi"))
                { tts.SelectVoice(v.VoiceInfo.Name); break; }
        }

        private void SpeakAsync(string text)
            => Task.Run(() => { try { tts.Speak(text); } catch { } });

        // ════════════════════════════════════════════════════════════════════════
        //  TEST
        // ════════════════════════════════════════════════════════════════════════
        private void TestButton_Click(object sender, EventArgs e)
        {
            string input = manualInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;
            Log($"🔤  Test: \"{input}\"", TextMuted);
            var (m, s) = FindBestMatch(input);
            UpdateConfidence((int)(s * 100));
            if (m != null && s >= THRESHOLD) HandleMatch(m, s);
            else HandleNoMatch(input, m, s);
            manualInput.Clear();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  LOG / UI HELPERS
        // ════════════════════════════════════════════════════════════════════════
        private void Log(string text, Color color, bool bold = false)
        {
            SafeInvoke(() =>
            {
                string ts = DateTime.Now.ToString("[HH:mm:ss]");
                logBox.SelectionStart = logBox.TextLength;
                logBox.SelectionColor = TextMuted;
                logBox.AppendText(ts + "  ");
                logBox.SelectionColor = color;
                if (bold) logBox.SelectionFont = new Font(logBox.Font, FontStyle.Bold);
                logBox.AppendText(text + "\n");
                if (bold) logBox.SelectionFont = logBox.Font;
                logBox.ScrollToCaret();
            });
        }

        private void UpdateConfidence(int val)
        {
            SafeInvoke(() =>
            {
                confidenceBar.Value = Math.Clamp(val, 0, 100);
                confidenceLabel.Text = $"Conf: {val}%";
                confidenceLabel.ForeColor = val >= 80 ? AccentGreen :
                                            val >= 50 ? AccentAmber : AccentRed;
            });
        }

        private void SafeInvoke(Action action)
        {
            if (IsDisposed) return;
            if (InvokeRequired) Invoke(action);
            else action();
        }

        // ════════════════════════════════════════════════════════════════════════
        //  CLEANUP
        // ════════════════════════════════════════════════════════════════════════
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            isRecording = false;
            try { waveIn?.StopRecording(); waveIn?.Dispose(); } catch { }
            try { waveWriter?.Dispose(); audioBuffer?.Dispose(); } catch { }
            try { whisperProcessor?.Dispose(); whisperFactory?.Dispose(); } catch { }
            try { tts?.Dispose(); } catch { }
            base.OnFormClosing(e);
        }
    }
}
