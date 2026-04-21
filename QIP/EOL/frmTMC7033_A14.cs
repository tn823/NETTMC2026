using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using ConnectionClass.Oracle;
using static GlobalFunction.PublicFunction;
using System.Diagnostics;
using GlobalFunction;

namespace QIP.EOL
{
    public partial class frmTMC7033_A14 : UserControl
    {
        private bool isRed = true;
        public static string ipAddress;
        public static string spDeptCode = "ASS";
        private static int TotalDefect;
        private static int TotalPass;
        public static string RecievedIpaddress;
        public static string Comname;
        bool isstartcountreason;
        private CRUDOracle crud;
        private DataTable dtReason;
        private DataTable ProductionInformation;
        private DataTable TouchCount;
        private DataTable TopDefect;
        private DataTable ErrorCount;
        private DataTable RFT_DPPM;
        private DataTable Top3DPPM;
        private DataTable defectLibrary;
        private DataTable dtStopLine = new DataTable();
        private DataTable dtAlarmReturn = new DataTable();
        private string finishedCountScan;
        public string alarmGather;
        private static string partID;
        private static List<string> btnfail;
        private static string LeftOrRight;
        private static string reasonID;
        private string Mes_Group_Sum;
        private static string LineName;
        private DataTable dtJSI = new DataTable();
        private string stSensorCount = "";
        private static string spLine;
        public bool MQTTConnected = false;
        public string MQTTClient = "";
        //private static readonly Color ReasonButtonDefaultColor = Color.FromArgb(192, 255, 255);
        private static readonly Color ReasonButtonDefaultColor = Color.AliceBlue;
        private static readonly Color ReasonButtonSelectedColor = Color.FromArgb(224, 224, 224);
        private static readonly Color PassButtonColor = Color.Blue;
        private static readonly Color FailButtonColor = Color.FromArgb(231, 76, 60);
        private static readonly Color ClearButtonColor = Color.Gray;
        Dictionary<string, string> Reason = new Dictionary<string, string>();
        GlobalFunction.PublicFunction etc = new GlobalFunction.PublicFunction();
        SYSTEMTIME st = new SYSTEMTIME();
        #region TimeWebClient For Andon
        public class TimedWebClient : WebClient
        {
            public int Timeout { get; set; }
            public TimedWebClient()
            {
                this.Timeout = 500;
            }
            protected override WebRequest GetWebRequest(Uri address)
            {
                var objWebRequest = base.GetWebRequest(address);
                objWebRequest.Timeout = this.Timeout;
                return objWebRequest;
            }
            public new async Task<string> DownloadStringTaskAsync(Uri address)
            {
                var t = base.DownloadStringTaskAsync(address);
                if (await Task.WhenAny(t, Task.Delay(Timeout)) != t)
                    CancelAsync();
                return await t;
            }
        }
        #endregion
        public frmTMC7033_A14()
        {
            InitializeComponent();
            crud = new CRUDOracle("VSMES");
            InitializeActionButtons();
            pictureShoes.SizeChanged += pictureShoes_SizeChanged;
        }
        private void ShowMessage(string message, Color color)
        {
            memoEditMessage.Text = "";
            memoEditMessage.Text = message;
            memoEditMessage.ForeColor = color;
        }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void frmTMC7033_A14_Load(object sender, EventArgs e)
        {
            ipAddress = GlobalFunction.PublicFunction.myIpaddress;
            ipAddress = "192.168.31.249";
            TryToUpdateSystemDateTime();
            BindingControl();
            try
            {

                #region OnlineIfCannotUseExcelLocal
                GetLineName(ipAddress);
                GetError(spDeptCode);
                #endregion
                dtReason = new DataTable();
                dtReason.Columns.Add("PART");
                dtReason.Columns.Add("REASON");
                dtReason.Columns.Add("USE_YN");
                lblTop1Defect.Text = "";
                lblTop2Defect.Text = "";
                lblTop3Defect.Text = "";
                lblFirstDefect.Text = "";
                lblReDefect.Text = "";
                lblTotalDefect.Text = "";
                lblEOLQCDDPM.Text = "";
                lblSensorCount.Text = "";



                lblPart1.Font = new Font("Arial", 100);
                lblPart2.Font = new Font("Arial", 100);
                lblPart3.Font = new Font("Arial", 100);
                lblPart4.Font = new Font("Arial", 100);
                lblPart5.Font = new Font("Arial", 100);
                lblPart6.Font = new Font("Arial", 100);

                BindingControl();
                SetInspectionActionState(true, false, true, false);
                ConffigErrorButton(false);



                SetTouchCount();
                BindTouchCount(TouchCount);
                MQTT_Init();

            }
            catch (Exception ex)
            {


                ShowMessage(ex.ToString() + Environment.NewLine + "MỞ CHUONG TRÌNH KHÔNG ÐƯỢC...RỚT MẠNG HOẶC CHUONG TRÌNH LỖI RỒI. " + Environment.NewLine + " CHƯƠNG TRÌNH SẼ TẮT, THÌ MỞ LẠI. " +
                     Environment.NewLine + " KHÔNG ÐUỢC THÌ GỌI IT " +
                     Environment.NewLine + " SDT : 0903518945. CÁM ON NHIỀU", Color.Red);
                Application.Exit();
            }
        }
        private void pictureShoes_SizeChanged(object sender, EventArgs e)
        {
            if (!IsHandleCreated)
            {
                return;
            }

            BindingControl();
        }
        private Rectangle GetDisplayedImageBounds(PictureBox pictureBox)
        {
            if (pictureBox.Image == null || pictureBox.ClientSize.Width <= 0 || pictureBox.ClientSize.Height <= 0)
            {
                return pictureBox.ClientRectangle;
            }

            if (pictureBox.SizeMode != PictureBoxSizeMode.Zoom)
            {
                return pictureBox.ClientRectangle;
            }

            Size imageSize = pictureBox.Image.Size;
            float scale = Math.Min((float)pictureBox.ClientSize.Width / imageSize.Width,
                                   (float)pictureBox.ClientSize.Height / imageSize.Height);

            int scaledWidth = (int)Math.Round(imageSize.Width * scale);
            int scaledHeight = (int)Math.Round(imageSize.Height * scale);
            int offsetX = (pictureBox.ClientSize.Width - scaledWidth) / 2;
            int offsetY = (pictureBox.ClientSize.Height - scaledHeight) / 2;

            return new Rectangle(offsetX, offsetY, scaledWidth, scaledHeight);
        }
        private async void MQTT_Init()
        {
            string MQTTServer = "";
            StringBuilder qry = new StringBuilder();
            qry.AppendLine(" SELECT ");
            qry.AppendLine(" (SELECT NVL(N_ANDON,'0') FROM TRTB_M_COMMON WHERE N_COMNAME = '" + ipAddress + "' AND C_COMCODE LIKE 'ASS%' AND N_ANDON IS NOT NULL) CLIENT  ");
            qry.AppendLine(" , (SELECT NVL(M_VALUE,'0') FROM APPS.M_PLAN_CONFIG@INF_M_E  WHERE M_ITEM = 'ANDON' AND M_TITLE = 'MQTT' ) SERVER     ");
            qry.AppendLine(" FROM DUAL  ");

            var _dt = crud.dac.DtSelectExcuteWithQuery(qry.ToString());
            if (_dt != null && _dt.Rows.Count > 0)
            {
                MQTTServer = _dt.Rows[0]["SERVER"].ToString();
                MQTTClient = _dt.Rows[0]["CLIENT"].ToString().ToUpper();
            }
            if (MQTTClient != "0")
            {
                if (await MQTT.Main.Setup(MQTTServer, "itminh", "samho2024@", MQTTClient + "/CALL", MQTTClient + "_OUT"))
                {
                    MQTTConnected = true;
                    MQTT.Main.CallBackEvent += CallbackMsg;
                }
                else MQTTConnected = false;
            }
        }
        private void BindTouchCount(DataTable dt)
        {
            int offlineDefect = 0;
            TotalDefect = Convert.ToInt32(dt.Rows[0]["Q_FAIL"].ToString());
            offlineDefect = Convert.ToInt32(lblFailTotal.Text.Substring(7, lblFailTotal.Text.Length - 7));
            lblSyncStatus.Text = TotalDefect + "/" + offlineDefect;
            this.lblFailTotal.Text = "FAIL : " + TotalDefect;
            TotalPass = Convert.ToInt32(dt.Rows[0]["Q_PASS"].ToString());
            this.lblPassTotal.Text = "PASS: " + TotalPass;
            lblRFT.Text = Math.Round(offlineDefect * 1.0 / (offlineDefect + TotalPass) * 100.0, 1) + " %";
            if (lblRFT.Text == "NaN %")
            {
                lblRFT.Text = "0 %";
            }
            if (TotalDefect < offlineDefect)
            {
                if (backgroundSyncData.IsBusy)
                {

                }
                else
                {
                    backgroundSyncData.RunWorkerAsync();
                }
            }
        }


        //===============================================================================



        private void GetLineName(string ip)
        {
            //Debug.WriteLine("Get Line Name with IP: " + ip);
            //string cacheFile = Path.Combine(Application.StartupPath, "LineName.csv");

            DataTable dt = new DataTable();
            var b = new BackgroundWorker();

            b.DoWork += (sender, e) =>
            {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("");
                    query.AppendLine("SELECT SUBSTR(C_COMCODE,4,4) C_COMCODE,                                                                               ");
                    query.AppendLine("  case when SUBSTR(C_COMCODE,4,2) = 'P7' THEN SUBSTR(C_COMCODE,4,4)                                                   ");
                    query.AppendLine("     ELSE                                                                                                             ");
                    query.AppendLine("         DECODE(SUBSTR(C_COMCODE, 6, 1), 'A', 'P1', 'B', 'P2', 'C', 'P3', 'D', 'P4', 'E', 'P5', 'F', 'P6', 'PP') ||   ");
                    query.AppendLine("         CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A'                                                                       ");
                    query.AppendLine("                   THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55)                                                      ");
                    query.AppendLine("             Else '0' || SUBSTR(C_COMCODE, 7, 1)                                                                      ");
                    query.AppendLine("         END                                                                                                          ");
                    query.AppendLine("END SHOW_LINE                                                                                                         ");
                    query.AppendLine("    FROM (                                                                                                            ");
                    query.AppendLine("          SELECT SUBSTR(C_COMCODE,1,7) C_COMCODE,N_COMNAME                                                            ");
                    query.AppendLine("            From TRTB_M_COMMON                                                                                        ");
                    query.AppendLine("           WHERE C_GROUP = 'BTS'                                                                                      ");
                    query.AppendLine("             AND N_COMNAME = '" + ip + "'                                                                             ");
                    query.AppendLine("         )                                                                                                            ");
                dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                e.Result = dt;
            };

            b.RunWorkerCompleted += (sender, e) =>
            {
                string cacheFile = Path.Combine(Application.StartupPath, "LineName.csv");
                // [TH1] Lỗi kết nối DB → đọc cache CSV
                if (e.Error != null || e.Result == null)
                {
                    Debug.WriteLine("GetLineName: DB error, trying cache. " + e.Error?.Message);
                    lblLineInfo.Text = "Offline";

                    if (File.Exists(cacheFile))
                    {
                        DataTable dtCache = EolCommonHelper.ReadLineNameFromCsv(cacheFile);
                        if (dtCache != null && dtCache.Rows.Count > 0)
                        {
                            LineName = dtCache.Rows[0]["SHOW_LINE"].ToString();
                            spLine = dtCache.Rows[0]["C_COMCODE"].ToString();
                            lblLineInfo.Text = LineName;
                        }
                    }
                    return;
                }

                dt = (DataTable)e.Result;

                // [TH2] DB trả về null hoặc rỗng → đọc cache CSV
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Debug.WriteLine("GetLineName: DB returned empty, trying cache.");
                    if (File.Exists(cacheFile))
                    {
                        DataTable dtCache = EolCommonHelper.ReadLineNameFromCsv(cacheFile);
                        if (dtCache != null && dtCache.Rows.Count > 0)
                        {
                            LineName = dtCache.Rows[0]["SHOW_LINE"].ToString();
                            spLine = dtCache.Rows[0]["C_COMCODE"].ToString();
                            lblLineInfo.Text = LineName;
                        }
                    }
                    return;
                }

                // [TH3] DB trả dữ liệu hợp lệ → gán giá trị và lưu cache CSV
                spLine = dt.Rows[0]["C_COMCODE"].ToString();
                LineName = dt.Rows[0]["SHOW_LINE"].ToString();
                lblLineInfo.Text = LineName;

                // Lưu cache CSV để dùng khi offline
                EolCommonHelper.SaveLineNameToCsv(dt, cacheFile);

                // Ẩn sensor nếu không phải line P114
                if (lblLineInfo.Text != "P114")
                {
                    lblSensorCount.Visible = false;
                }

                Debug.WriteLine($"GetLineName OK: spLine={spLine}, LineName={LineName}");
            };
            b.RunWorkerAsync();
        }

        

        //========================================================================================================

        private void GetError(string type)
        {
            string cacheFile = Path.Combine(Application.StartupPath, "ErrorButton.csv");

            DataTable dt = new DataTable();

            var b = new BackgroundWorker();
            b.DoWork += new DoWorkEventHandler(
                delegate (object sender, DoWorkEventArgs e)
                {
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("");
                    query.AppendLine("        SELECT PART_ID, REASON_ID, REASON_SHORT, REASON_EN, REASON_VN                                ");
                    query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@inf_m_e                                                                  ");
                    query.AppendLine("WHERE DEPT_CODE = '" + type + "' AND REASON_ID <= 82                                                  ");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    e.Result = dt;
                    dt = (DataTable)e.Result;
                }
            );


            b.RunWorkerCompleted += (sender, e) =>
            {
                // ── [TH1] Lỗi kết nối DB → đọc cache CSV ──────────────────────────
                if (e.Error != null || e.Result == null)
                {
                    Debug.WriteLine("[GetError][Completed] TH1 – DB lỗi: " + e.Error?.Message);
                    if (File.Exists(cacheFile))
                    {
                        DataTable dtCache = EolCommonHelper.ReadErrorButtonFromCsv(cacheFile);
                        if (dtCache != null && dtCache.Rows.Count > 0)
                        {
                            defectLibrary = dtCache.Copy();
                            SetErrorToButton(type, dtCache);
                            ConffigErrorButton(false);
                            Debug.WriteLine("[GetError][Completed] Đọc cache thành công: " + dtCache.Rows.Count + " rows");
                        }
                    }
                    else
                    {
                        Debug.WriteLine("[GetError][Completed] Không có cache file: " + cacheFile);
                    }
                    return;
                }

                dt = (DataTable)e.Result;

                // ── [TH2] DB trả 0 rows → đọc cache CSV ────────────────────────────
                if (dt == null || dt.Rows.Count <= 0)
                {
                    Debug.WriteLine("[GetError][Completed] TH2 – DB trả 0 rows, thử cache");
                    if (File.Exists(cacheFile))
                    {
                        DataTable dtCache = EolCommonHelper.ReadErrorButtonFromCsv(cacheFile);
                        if (dtCache != null && dtCache.Rows.Count > 0)
                        {
                            defectLibrary = dtCache.Copy();
                            SetErrorToButton(type, dtCache);
                            ConffigErrorButton(false);
                        }
                    }
                    return;
                }

                // ── [TH3] DB trả data hợp lệ → gán nút + lưu cache ─────────────────
                Debug.WriteLine("[GetError][Completed] TH3 – DB OK: " + dt.Rows.Count + " rows");
                defectLibrary = dt.Copy();
                SetErrorToButton(type, dt);
                ConffigErrorButton(false);
                EolCommonHelper.SaveErrorButtonToCsv(dt, cacheFile);
            };
            b.RunWorkerAsync();
        }
        


        //========================================================================================================



        //private void ConffigErrorButton(bool visible)
        //{
        //    foreach (var p in tableLayoutPanel2.Controls)
        //    {
        //        if (p.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            PanelControl panel = (PanelControl)p;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btn = (SimpleButton)a;
        //                    btn.Enabled = visible;
        //                }
        //            }
        //        }

        //    }
        //    foreach (var p in tableLayoutErrorLeft.Controls)
        //    {
        //        if (p.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            PanelControl panel = (PanelControl)p;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btn = (SimpleButton)a;
        //                    btn.Enabled = visible;
        //                }
        //            }
        //        }

        //    }
        //    foreach (var p in tableLayoutErrorRight.Controls)
        //    {
        //        if (p.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            PanelControl panel = (PanelControl)p;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btn = (SimpleButton)a;
        //                    btn.Enabled = visible;
        //                }
        //            }
        //        }

        //    }

        //}
        private void ConffigErrorButton(bool visible)
        {
            foreach (Button btn in GetReasonButtons())
            {
                btn.Enabled = visible;
            }
        }

        private IEnumerable<Button> GetReasonButtons()
        {
            Control[] reasonContainers =
            {
                panelControl8,
                panelControl7,
                panelControl9,
                panelControl10,
                tableLayoutErrorLeft,
                tableLayoutErrorRight
            };

            foreach (Control container in reasonContainers)
            {
                foreach (Button btn in GetButtonsRecursive(container))
                {
                    yield return btn;
                }
            }
        }

        private IEnumerable<Button> GetButtonsRecursive(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    yield return btn;
                }

                if (!ctrl.HasChildren)
                {
                    continue;
                }

                foreach (Button nestedButton in GetButtonsRecursive(ctrl))
                {
                    yield return nestedButton;
                }
            }
        }

        private void SetInspectionActionState(bool passEnabled, bool failEnabled, bool rePassEnabled, bool reFailEnabled)
        {
            btnPass.Enabled = passEnabled;
            btnFail.Enabled = failEnabled;
            btnRePass.Enabled = rePassEnabled;
            btnReFail.Enabled = reFailEnabled;
            btnClear.Enabled = true;

            RestoreActionButtonColors();
        }

        private void RestoreActionButtonColors()
        {
            RestoreButtonColor(btnPass, PassButtonColor, Color.White);
            RestoreButtonColor(btnFail, FailButtonColor, Color.White);
            RestoreButtonColor(btnRePass, PassButtonColor, Color.White);
            RestoreButtonColor(btnReFail, FailButtonColor, Color.White);
            RestoreButtonColor(btnClear, ClearButtonColor, Color.White);
        }

        private void RestoreButtonColor(Button btn, Color backColor, Color foreColor)
        {
            btn.UseVisualStyleBackColor = false;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
        }

        private void InitializeActionButtons()
        {
            Button[] actionButtons = { btnPass, btnFail, btnRePass, btnReFail };

            foreach (Button btn in actionButtons)
            {
                btn.UseVisualStyleBackColor = false;
                btn.FlatStyle = FlatStyle.Flat;
                btn.FlatAppearance.BorderSize = 0;
                btn.Paint += ActionButton_Paint;
                btn.EnabledChanged += ActionButton_VisualStateChanged;
                btn.TextChanged += ActionButton_VisualStateChanged;
                btn.Resize += ActionButton_VisualStateChanged;
                btn.BackColorChanged += ActionButton_VisualStateChanged;
                btn.ForeColorChanged += ActionButton_VisualStateChanged;
            }
        }

        private void ActionButton_VisualStateChanged(object sender, EventArgs e)
        {
            if (sender is Button btn)
            {
                btn.Invalidate();
            }
        }

        private void ActionButton_Paint(object sender, PaintEventArgs e)
        {
            if (sender is not Button btn)
            {
                return;
            }

            Rectangle rect = btn.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            Color backgroundColor = btn.BackColor;
            Color textColor = btn.ForeColor;
            Color surfaceColor = btn.Parent?.BackColor ?? SystemColors.Control;

            if (!btn.Enabled)
            {
                backgroundColor = BlendColor(backgroundColor, surfaceColor, 0.7f);
                textColor = BlendColor(textColor, surfaceColor, 0.75f);
            }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (SolidBrush backgroundBrush = new SolidBrush(backgroundColor))
            using (Pen borderPen = new Pen(BlendColor(backgroundColor, Color.Black, btn.Enabled ? 0.15f : 0.05f)))
            {
                e.Graphics.FillRectangle(backgroundBrush, rect);
                e.Graphics.DrawRectangle(borderPen, 0, 0, rect.Width - 1, rect.Height - 1);
            }

            TextRenderer.DrawText(
                e.Graphics,
                btn.Text,
                btn.Font,
                rect,
                textColor,
                TextFormatFlags.HorizontalCenter |
                TextFormatFlags.VerticalCenter |
                TextFormatFlags.WordBreak);
        }

        private Color BlendColor(Color sourceColor, Color targetColor, float amount)
        {
            amount = Math.Max(0f, Math.Min(1f, amount));

            int r = (int)Math.Round(sourceColor.R + ((targetColor.R - sourceColor.R) * amount));
            int g = (int)Math.Round(sourceColor.G + ((targetColor.G - sourceColor.G) * amount));
            int b = (int)Math.Round(sourceColor.B + ((targetColor.B - sourceColor.B) * amount));

            return Color.FromArgb(r, g, b);
        }

        private void ResetReasonButtonColors()
        {
            foreach (Button btn in GetReasonButtons())
            {
                btn.UseVisualStyleBackColor = false;
                btn.FlatStyle = FlatStyle.Standard;
                //btn.BackColor = Color.FromArgb(192, 255, 255);
                btn.BackColor = Color.FromArgb(192, 255, 255);

                btn.ForeColor = SystemColors.ControlText;
            }
        }



        //========================================================================================================



        //private void SetErrorToButton(string type, DataTable DefectLibary)
        //{
        //    try
        //    {
        //        if (DefectLibary == null) return;
        //        foreach (var panel in tableLayoutPanel2.Controls)
        //        {
        //            if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //            {
        //                DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //                foreach (var a in pnl.Controls)
        //                {
        //                    if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                    {
        //                        SimpleButton btnID = (SimpleButton)a;
        //                        btnID.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //                        foreach (DataRow dr in DefectLibary.Rows)
        //                        {
        //                            if (btnID.AccessibleName == dr["REASON_ID"].ToString())
        //                            {
        //                                if (chkVN.Checked)
        //                                {
        //                                    btnID.Font = new Font("VNI-Times", 28);
        //                                    btnID.Text = dr["REASON_VN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    btnID.Text = dr["REASON_EN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        foreach (var panel in tableLayoutErrorLeft.Controls)
        //        {
        //            if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //            {
        //                DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //                foreach (var a in pnl.Controls)
        //                {
        //                    if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                    {
        //                        SimpleButton btnID = (SimpleButton)a;
        //                        btnID.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //                        foreach (DataRow dr in DefectLibary.Rows)
        //                        {
        //                            if (btnID.AccessibleName == dr["REASON_ID"].ToString())
        //                            {
        //                                if (chkVN.Checked)
        //                                {
        //                                    btnID.Font = new Font("VNI-Times", 28);
        //                                    btnID.Text = dr["REASON_VN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    btnID.Text = dr["REASON_EN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        foreach (var panel in tableLayoutErrorRight.Controls)
        //        {
        //            if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //            {
        //                DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //                foreach (var a in pnl.Controls)
        //                {
        //                    if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                    {
        //                        SimpleButton btnID = (SimpleButton)a;
        //                        btnID.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
        //                        foreach (DataRow dr in DefectLibary.Rows)
        //                        {
        //                            if (btnID.AccessibleName == dr["REASON_ID"].ToString())
        //                            {
        //                                if (chkVN.Checked)
        //                                {
        //                                    btnID.Font = new Font("VNI-Times", 28);
        //                                    btnID.Text = dr["REASON_VN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    btnID.Text = dr["REASON_EN"].ToString();
        //                                    if (Reason.ContainsKey(btnID.AccessibleName))
        //                                    {
        //                                        Reason[btnID.AccessibleName] = btnID.Text;

        //                                    }
        //                                    else
        //                                    {
        //                                        Reason.Add(btnID.AccessibleName, btnID.Text);
        //                                    }
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        string a = ex.Message;
        //    }
        //}


        //========================================================================================================



        private void SetErrorToButton(string type, DataTable DefectLibary)
        {
            try 
            {
                if (DefectLibary == null) return;
                Control[] containers = { tableLayoutPanel2, tableLayoutErrorLeft, tableLayoutErrorRight };
                foreach (Control container in containers)
                {
                    UpdateButtonsInContainer(container, DefectLibary);
                }
            }
            catch (Exception ex)
            {
                string a = ex.Message;
            }
        }

        private void UpdateButtonsInContainer(Control parent, DataTable DefectLibrary)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Nếu là Button (Standard WinForms)
                if (ctrl is Button btn)
                {
                    foreach (DataRow dr in DefectLibrary.Rows)
                    {
                        // Kiểm tra ID lỗi khớp với AccessibleName của Button
                        if (btn.AccessibleName == dr["REASON_ID"].ToString())
                        {
                            string reasonText = "";

                            if (chkVN.Checked)
                            {
                                btn.Font = new Font("VNI-Times", 24);
                                reasonText = dr["REASON_VN"].ToString();
                            }
                            else
                            {
                                // Trả về font mặc định hoặc font tiếng Anh nếu cần
                                btn.Font = new Font("Microsoft Sans Serif", 24);
                                reasonText = dr["REASON_EN"].ToString();
                            }

                            btn.Text = reasonText;

                            // Cập nhật Dictionary (Tự động thêm mới hoặc ghi đè nếu đã tồn tại)
                            if (!string.IsNullOrEmpty(btn.AccessibleName))
                            {
                                Reason[btn.AccessibleName] = reasonText;
                            }
                        }
                    }
                }

                // Nếu control này chứa các control con khác (như Panel, GroupBox...), tiếp tục tìm kiếm
                if (ctrl.HasChildren)
                {
                    UpdateButtonsInContainer(ctrl, DefectLibrary);
                }
            }
        }


        //========================================================================================================



        private void TryToUpdateSystemDateTime()
        {
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery("select SYSDATE from dual");
            if (dt.Rows.Count > 0)
            {
                DateTime d = Convert.ToDateTime(dt.Rows[0][0]).ToUniversalTime();
                st.wYear = (short)d.Year; // must be short
                st.wMonth = (short)d.Month;
                st.wDay = (short)d.Day;
                st.wHour = (short)d.Hour;
                st.wMinute = (short)d.Minute;
                st.wSecond = (short)d.Second;
                SetSystemTime(ref st);
            }
        }
        private void BindingControl()
        {
            try
            {
                Rectangle displayedImageBounds = GetDisplayedImageBounds(pictureShoes);
                int xPic = displayedImageBounds.X;
                int yPic = displayedImageBounds.Y;
                int wPic = displayedImageBounds.Width;
                int hPic = displayedImageBounds.Height;
                ///<sumary>Configuration Part Location on Shoe Picture
                ///</sumary>
                #region Configuration Part Location on Shoe Picture
                var pos = this.PointToScreen(lblPart1.Location);
                pos = pictureShoes.PointToClient(pos);
                lblPart1.BackColor = System.Drawing.Color.Transparent;
                lblPart1.ForeColor = System.Drawing.Color.Red;
                lblPart1.Parent = pictureShoes;
                lblPart1.Left = xPic + 55;
                lblPart1.Top = yPic + (hPic / 2) - 55;
                lblPartCount1.BackColor = System.Drawing.Color.Transparent;
                lblPartCount1.ForeColor = System.Drawing.Color.Red;

                var pos2 = this.PointToScreen(lblPart2.Location);
                pos2 = pictureShoes.PointToClient(pos2);
                lblPart2.BackColor = System.Drawing.Color.Transparent;
                lblPart2.ForeColor = System.Drawing.Color.Green;
                lblPartCount2.BackColor = System.Drawing.Color.Transparent;
                lblPartCount2.ForeColor = System.Drawing.Color.Red;
                lblPart2.Parent = pictureShoes;
                lblPart2.Left = xPic + wPic / 4;
                lblPart2.Top = yPic + hPic / 4;
                var pos3 = this.PointToScreen(lblPart3.Location);
                pos3 = pictureShoes.PointToClient(pos3);
                lblPart3.BackColor = System.Drawing.Color.Transparent;
                lblPart3.ForeColor = System.Drawing.Color.Blue;
                lblPartCount3.BackColor = System.Drawing.Color.Transparent;
                lblPartCount3.ForeColor = System.Drawing.Color.Red;
                lblPart3.Parent = pictureShoes;
                lblPart3.Left = xPic + (wPic / 2) - 30;
                lblPart3.Top = yPic - 10;
                var pos4 = this.PointToScreen(lblPart4.Location);
                pos4 = pictureShoes.PointToClient(pos4);
                lblPart4.BackColor = System.Drawing.Color.Transparent;
                lblPart4.ForeColor = System.Drawing.Color.Pink;
                lblPartCount4.BackColor = System.Drawing.Color.Transparent;
                lblPartCount4.ForeColor = System.Drawing.Color.Red;
                lblPart4.Parent = pictureShoes;
                lblPart4.Left = xPic + Convert.ToInt32(Math.Round(wPic / 1.4));
                lblPart4.Top = yPic + hPic / 20;
                var pos5 = this.PointToScreen(lblPart5.Location);
                pos5 = pictureShoes.PointToClient(pos5);
                lblPart5.BackColor = System.Drawing.Color.Transparent;
                lblPart5.ForeColor = System.Drawing.Color.Red;
                lblPartCount5.BackColor = System.Drawing.Color.Transparent;
                lblPartCount5.ForeColor = System.Drawing.Color.Red;
                lblPart5.Parent = pictureShoes;
                lblPart5.Left = xPic + wPic / 2;
                lblPart5.Top = yPic + hPic / 2;

                var pos6 = this.PointToScreen(lblPart6.Location);
                pos6 = pictureShoes.PointToClient(pos6);

                lblPart6.BackColor = System.Drawing.Color.Transparent;
                lblPart6.ForeColor = System.Drawing.Color.Gray;
                lblPartCount6.BackColor = System.Drawing.Color.Transparent;
                lblPartCount6.ForeColor = System.Drawing.Color.Red;
                lblPart6.Parent = pictureShoes;
                lblPart6.Left = xPic + Convert.ToInt32(Math.Round(wPic / 1.2));
                lblPart6.Top = yPic + hPic / 2;


                var posC1 = this.PointToScreen(lblPartCount1.Location);
                posC1 = pictureShoes.PointToClient(posC1);
                lblPartCount1.Parent = pictureShoes;
                lblPartCount1.BringToFront();

                var posC2 = this.PointToScreen(lblPartCount2.Location);
                posC2 = pictureShoes.PointToClient(posC2);
                lblPartCount2.Parent = pictureShoes;
                lblPartCount2.BringToFront();

                var posC3 = this.PointToScreen(lblPartCount3.Location);
                posC3 = pictureShoes.PointToClient(posC3);
                lblPartCount3.Parent = pictureShoes;
                lblPartCount3.BringToFront();

                var posC4 = this.PointToScreen(lblPartCount4.Location);
                posC4 = pictureShoes.PointToClient(posC4);
                lblPartCount4.Parent = pictureShoes;
                lblPartCount4.BringToFront();

                var posC5 = this.PointToScreen(lblPartCount5.Location);
                posC5 = pictureShoes.PointToClient(posC5);
                lblPartCount5.Parent = pictureShoes;
                lblPartCount5.BringToFront();



                var posC6 = this.PointToScreen(lblPartCount6.Location);
                posC6 = pictureShoes.PointToClient(posC6);
                lblPartCount6.Parent = pictureShoes;
                lblPartCount6.BringToFront();

                lblPartCount1.Left = lblPart1.Left + 40;
                lblPartCount1.Top = lblPart1.Top + 50;

                lblPartCount2.Left = lblPart2.Left + 40;
                lblPartCount2.Top = lblPart2.Top + 50;

                lblPartCount3.Left = lblPart3.Left + 40;
                lblPartCount3.Top = lblPart3.Top + 50;

                lblPartCount4.Left = lblPart4.Left + 40;
                lblPartCount4.Top = lblPart4.Top + 50;

                lblPartCount5.Left = lblPart5.Left + 40;
                lblPartCount5.Top = lblPart5.Top + 50;

                lblPartCount6.Left = lblPart6.Left + 40;
                lblPartCount6.Top = lblPart6.Top + 50;



                if (Screen.PrimaryScreen.WorkingArea.Height < 1000)
                {
                    TableLayoutRowStyleCollection styles = tableLayoutPanel1.RowStyles;
                    foreach (RowStyle style in styles)
                    {
                        // Set the row height to 20 pixels.
                        if (style.SizeType == SizeType.Absolute)
                        {

                            style.Height = 530;
                            lblPart1.Left = 10;
                            lblPart1.Top = 85;

                            lblPart2.Left = 130;
                            lblPart2.Top = 135;

                            lblPart3.Left = 300;
                            lblPart3.Top = 30;

                            lblPart4.Left = 400;
                            lblPart4.Top = 170;

                            lblPart5.Left = 560;
                            lblPart5.Top = 10;

                            lblPart6.Left = 600;
                            lblPart6.Top = 170;




                            lblPartCount1.Left = 20;
                            lblPartCount1.Top = 95;

                            lblPartCount2.Left = 140;
                            lblPartCount2.Top = 145;

                            lblPartCount3.Left = 310;
                            lblPartCount3.Top = 40;

                            lblPartCount4.Left = 410;
                            lblPartCount4.Top = 180;

                            lblPartCount5.Left = 570;
                            lblPartCount5.Top = 20;

                            lblPartCount6.Left = 610;
                            lblPartCount6.Top = 180;
                        }
                    }
                }
                #endregion
            }
            catch
            {

            }
        }
        private void SetTouchCount()
        {
            DataTable offline_TouchCount = new DataTable();
            offline_TouchCount.Columns.Add("Q_TOTAL");
            offline_TouchCount.Columns.Add("Q_PASS");
            offline_TouchCount.Columns.Add("Q_FAIL");

            int total = PassQty() + FailQty();
            offline_TouchCount.Rows.Add(total, PassQty(), FailQty());
            TouchCount = offline_TouchCount;

        }
        private void CallbackMsg(string msg)
        {
            if (msg.Length > 0)
            {
                var value = msg.Substring(0, msg.IndexOf("OK") + 2);
                Console.WriteLine(value);

                if (value == "R_ON_OK")
                {
                    ThreadSafe(() =>
                    {
                        btn_reasonCode1.Text = "(Andon) Gọi QA " + Environment.NewLine + "Calling";
                        this.timer_BlinkButtonRed.Enabled = true;
                    });

                }
                if (value == "R_OFF_OK")
                {
                    ThreadSafe(() =>
                    {
                        //btn_reasonCode1.Text = "(Andon) Gọi QA" + Environment.NewLine + "Waiting";
                        timer_BlinkButtonRed.Enabled = false;
                        btn_reasonCode1.BackColor = System.Drawing.Color.DarkRed;
                        btn_reasonCode1.ForeColor = System.Drawing.Color.White;
                    });
                }

                if (value == "Y_ON_OK")
                {
                    ThreadSafe(() =>
                    {
                        btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì" + Environment.NewLine + "Calling";
                        timer_BlinkButtonYellow.Enabled = true;
                    });
                }
                if (value == "Y_OFF_OK")
                {
                    ThreadSafe(() =>
                    {
                        //btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì" + Environment.NewLine + "Waiting";
                        timer_BlinkButtonYellow.Enabled = false;
                        //btn_reasonCode2.Appearance.BackColor = System.Drawing.Color.Orange;
                        //btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.Orange;
                        btn_reasonCode2.BackColor = System.Drawing.Color.Orange;
                        btn_reasonCode2.ForeColor = System.Drawing.Color.DarkRed;
                    });
                }

                if (value == "G_ON_OK")
                {
                    ThreadSafe(() =>
                    {
                        btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất" + Environment.NewLine + "Calling";
                        timer_BlinkButtonGreen.Enabled = true;
                    });
                }
                if (value == "G_OFF_OK")
                {
                    ThreadSafe(() =>
                    {
                        //btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất" + Environment.NewLine + "Waiting";
                        timer_BlinkButtonGreen.Enabled = false;
                        //btn_reasonCode3.Appearance.BackColor = System.Drawing.Color.DarkGreen;
                        //btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.DarkGreen;
                        btn_reasonCode3.BackColor = System.Drawing.Color.DarkGreen;
                        btn_reasonCode3.ForeColor = System.Drawing.Color.DarkRed;
                    });
                }
            }
        }
        private void ThreadSafe(MethodInvoker method)
        {
            try
            {
                if (InvokeRequired) Invoke(method);
                else method();
            }
            catch (ObjectDisposedException) { }
        }
        private int PassQty()
        {
            return etc.CountPassFail("Pass", "PassBTS_" + DateTime.Now.ToString("yyyyMMdd"));
        }
        private int FailQty()
        {
            return etc.CountPassFail("Fail", "FailBTS_" + DateTime.Now.ToString("yyyyMMdd"));
        }


        // =============================================================================



        //private DataTable GetDataTable(GridView view)
        //{
        //    DataTable dt = new DataTable();
        //    foreach (GridColumn c in view.Columns)
        //        dt.Columns.Add(c.FieldName, c.ColumnType);
        //    for (int r = 0; r < view.RowCount; r++)
        //    {
        //        object[] rowValues = new object[dt.Columns.Count];
        //        for (int c = 0; c < dt.Columns.Count; c++)
        //            rowValues[c] = view.GetRowCellValue(r, dt.Columns[c].ColumnName);
        //        dt.Rows.Add(rowValues);
        //    }
        //    return dt;
        //}
        private DataTable GetDataTable(DataGridView view)
        {
            DataTable dt = new DataTable();
            foreach (DataGridViewColumn c in view.Columns)
                dt.Columns.Add(c.Name, c.ValueType ?? typeof(string));
            for (int r = 0; r < view.RowCount; r++)
            {
                object[] rowValues = new object[dt.Columns.Count];
                for (int c = 0; c < dt.Columns.Count; c++)
                    rowValues[c] = view.Rows[r].Cells[c].Value ?? DBNull.Value;
                dt.Rows.Add(rowValues);
            }
            return dt;
        }


        // =============================================================================




        private void simpleButton14_Click(object sender, EventArgs e)
        {
            Popup.SendPictureDefectEOL sendpicture = new Popup.SendPictureDefectEOL();
            sendpicture.IPADDRESS = ipAddress;
            sendpicture.LINENAME = LineName;
            sendpicture.ShowDialog(this);
        }
        private void btnChonModel_Click(object sender, EventArgs e)
        {
            SelectModel model = new SelectModel();
            model.dtModel = ProcessTableModel();
            model.ShowDialog(this);
            this.btnChonModel.Text = model.ReturnSelection;

            if (this.btnChonModel.Text == "") this.btnChonModel.Text = "CHỌN MODEL";
            if (btnChonModel.Text != "" && btnChonModel.Text != "CHỌN MODEL")
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT MES_GROUP_SUM FROM MES.MES_MODEL@inf_m_e WHERE MES_STYLE_NO = '" + btnChonModel.Text + "'");
                //string modeltest = btnChonModel.Text;
                //Debug.WriteLine("Choose model: ", modeltest);
                DataTable dt = new DataTable();
                dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                Bitmap bm = null;
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        bm = ByteToImage(GetImgByte("ftp://" + etc.FileServerPath + @"/Mes/BTS/" + spDeptCode + "_" + dt.Rows[0]["MES_GROUP_SUM"].ToString().Replace("/", "") + ".jpg"));
                        //string testlog = "ftp://" + etc.FileServerPath + @"/Mes/BTS/" + spDeptCode + "_" + dt.Rows[0]["MES_GROUP_SUM"].ToString().Replace("/", "") + ".jpg";
                        //Debug.WriteLine("path modlle: ", testlog);

                        if (bm != null)
                        {
                            this.pictureShoes.Image = bm;
                        }
                        else
                        {
                            // pictureShoes.Image = QIP.Properties.Resources.sASS_3;
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowMessage("No Image. Không có hình giày này", Color.Red);


                        // pictureShoes.Image = QIP.Properties.Resources.sASS_3;
                    }
                }
                else
                {
                    ShowMessage("No Image. Không có hình giày này", Color.Red);
                    //pictureShoes.Image = QIP.Properties.Resources.sASS_3;
                }
            }
        }
        private DataTable ProcessTableModel()
        {
            DataTable oldDataModel = GetDataModel();
            if (oldDataModel == null || oldDataModel.Rows.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("col1");
            dt.Columns.Add("col2");
            dt.Columns.Add("col3");
            dt.Columns.Add("col4");
            dt.Columns.Add("col5");
            int countcol = 0;
            DataRow dr = dt.Rows.Add();
            for (int a = 0; a < oldDataModel.Rows.Count; a++)
            {
                countcol = countcol + 1;

                switch (countcol)
                {
                    case 1:
                        dr[0] = oldDataModel.Rows[a]["C_STYLE"].ToString();
                        break;
                    case 2:
                        dr[1] = oldDataModel.Rows[a]["C_STYLE"].ToString();
                        break;
                    case 3:
                        dr[2] = oldDataModel.Rows[a]["C_STYLE"].ToString();
                        break;
                    case 4:
                        dr[3] = oldDataModel.Rows[a]["C_STYLE"].ToString();
                        break;
                    case 5:
                        dr[4] = oldDataModel.Rows[a]["C_STYLE"].ToString();
                        break;
                }
                if (countcol == 5)
                {
                    countcol = 0;
                    // Chỉ thêm hàng mới nếu còn item phía sau
                    // (tránh thêm hàng trống khi item cuối vừa điền xong)
                    if (a < oldDataModel.Rows.Count - 1)
                        dr = dt.Rows.Add();
                }
            }
            return dt;
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            if (blob == null)
            {
                //ShowMessage("No Image. Không có hình giày này", Color.Red);
                return null;
            }
            MemoryStream mStream = new MemoryStream();
            byte[] pData = blob;
            mStream.Write(pData, 0, Convert.ToInt32(pData.Length));
            Bitmap bm = new Bitmap(mStream, false);
            mStream.Dispose();
            return bm;
        }
        public byte[] GetImgByte(string ftpFilePath)
        {
            try
            {
                WebClient ftpClient = new WebClient();
                ftpClient.Credentials = new NetworkCredential("mes", "!saigon3535!");

                byte[] imageByte = ftpClient.DownloadData(ftpFilePath);
                return imageByte;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
                return null;
            }
        }
        private DataTable GetDataModel()
        {
            StringBuilder query = new StringBuilder();
            int days = 7;
            if (chkPlanOneMonth.Checked) days = 30;

            if (ipAddress == "192.168.1.158")
            {
                spLine = "NSA1";
            }

            if (ipAddress == "192.168.3.59" && DateTime.Now.ToString("yyyyMMdd") == "20190824")
            {
                query.AppendLine("select * from (SELECT C_STYLE,MES_GROUP_SUM                                                                                                               ");
                query.AppendLine("         FROM (                                                                                                                            ");
                query.AppendLine("               SELECT A.C_LOCATION,                                                                                                        ");
                query.AppendLine("                      DECODE(SUBSTR(A.C_WORK_LINE,3,1),'A','P1','B','P2','C','P3','D','P4','E','P5','F','P6',A.C_WORK_LINE)||              ");
                query.AppendLine("                      CASE WHEN SUBSTR(A.C_WORK_LINE,4,1) >= 'A' THEN TO_CHAR(ASCII(SUBSTR(A.C_WORK_LINE,4,1))-55)                         ");
                query.AppendLine("                           WHEN SUBSTR(A.C_WORK_LINE,1,1)  = 'N' THEN '0'||SUBSTR(A.C_WORK_LINE,4,1)                                       ");
                query.AppendLine("                           Else ''                                                                                                         ");
                query.AppendLine("                       END C_LINE, C.C_STYLE,D.MES_GROUP_SUM                                                                               ");
                query.AppendLine("                 FROM TRTB_M_PROD_YIELD A, TRTB_M_CARD B, TRTB_M_PROD_PLAN C,MES.MES_MODEL@inf_m_e D                                       ");
                query.AppendLine("                Where a.I_CARD_NO = B.I_CARD_NO                                                                                            ");
                query.AppendLine("                  AND B.C_JOBORDER_NO = C.C_JOBORDER_NO                                                                                    ");
                query.AppendLine("                  AND C.C_STYLE = D.MES_STYLE_NO                                                                                           ");
                query.AppendLine("                  AND A.D_GATHER > TO_CHAR(SYSDATE-" + days + ",'YYYYMMDD')||'000000'                                                      ");
                query.AppendLine("                  AND A.C_LOCATION = 'ASEI' AND C_WORK_LINE = '" + spLine + "'                                                             ");
                query.AppendLine("                GROUP BY A.C_LOCATION, C.C_STYLE,D.MES_GROUP_SUM,                                                                          ");
                query.AppendLine("                      DECODE(SUBSTR(A.C_WORK_LINE,3,1),'A','P1','B','P2','C','P3','D','P4','E','P5','F','P6',A.C_WORK_LINE)||              ");
                query.AppendLine("                      CASE WHEN SUBSTR(A.C_WORK_LINE,4,1) >= 'A' THEN TO_CHAR(ASCII(SUBSTR(A.C_WORK_LINE,4,1))-55)                         ");
                query.AppendLine("                           WHEN SUBSTR(A.C_WORK_LINE,1,1)  = 'N' THEN '0'||SUBSTR(A.C_WORK_LINE,4,1)                                       ");
                query.AppendLine("                           Else '' END                                                                                                     ");
                query.AppendLine("              )                                                                                                                        ");
                query.AppendLine("        WHERE C_LINE = '" + LineName + "'                                                                                                  ");
                //query.AppendLine("        WHERE C_LINE = '" + spLine + "'                                                                                                  ");
                query.AppendLine("        ORDER BY 1 ) UNION ALL SELECT 'ML574D-ETE','MWL574 V2' FROM DUAL                                                                                                                         ");


            }
            else
            {

                query.AppendLine("            SELECT DISTINCT C.C_STYLE FROM TRTB_M_PROD_YIELD A                    ");
                query.AppendLine(", TRTB_M_CARD B                                                                   ");
                query.AppendLine(", TRTB_M_PROD_PLAN C                                                              ");
                query.AppendLine(" WHERE                                                                            ");
                query.AppendLine("A.I_CARD_NO = B.I_CARD_NO                                                         ");
                query.AppendLine("AND B.C_JOBORDER_NO = C.C_JOBORDER_NO                                             ");
                query.AppendLine("    AND A.D_GATHER > TO_CHAR(SYSDATE - " + days + ", 'YYYYMMDD') || '000000'      ");
                query.AppendLine("AND A.C_LOCATION = 'ASEI' AND C_WORK_LINE = '" + spLine + "'   ORDER BY 1         ");
            }
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            return dt;

        }
        private void chkPlanOneMonth_CheckedChanged(object sender, EventArgs e)
        {
            if (chkPlanOneMonth.Checked)
            {
                checkEdit2.Checked = false;
            }
        }
        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkEdit2.Checked)
            {
                chkPlanOneMonth.Checked = false;
            }
        }
        private void chkVN_CheckedChanged(object sender, EventArgs e)
        {
            if (chkVN.Checked)
            {
                chkEng.Checked = false;
                RefreshReasonButtonsLanguage();
            }
            else if (!chkEng.Checked)
            {
                chkEng.Checked = true;
            }
        }
        private void chkEng_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEng.Checked)
            {
                chkVN.Checked = false;
                RefreshReasonButtonsLanguage();
            }
            else if (!chkVN.Checked)
            {
                chkVN.Checked = true;
            }
        }
        private void RefreshReasonButtonsLanguage()
        {
            if (defectLibrary == null || defectLibrary.Rows.Count == 0)
            {
                return;
            }

            SetErrorToButton(spDeptCode, defectLibrary);
        }
        private void backgroundOracle_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundOracle.ReportProgress(10);
            GetProdInformation();
            //GetRFT_DPPM();

            GetAllData_20260310();
            GetTop3DPPM();
            GetDataTopDefect(spLine);
            GetErrorCount(spLine);
            backgroundOracle.ReportProgress(100);
        }
        private void backgroundOracle_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10)
            {
                ShowMessage("Background worker oracle running....", Color.Blue);
            }
            else if (e.ProgressPercentage == 100)
            {
                ShowMessage("Background worker oracle finish", Color.Blue);
            }
        }
        private void backgroundOracle_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BindProductQTY();
            BindTop3DDPMRFT();
            ConfigCountErrorButton(ErrorCount);
        }
        private DataTable GetProdInformation()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("            SELECT X.C_LINE, NVL(X.PROD_QTY,0) PROD_QTY, REASON_ID, A.DEFECT_SUM, B.REASON_CNT,                                                ");
            query.AppendLine("     ROUND((A.DEFECT_SUM/X.PROD_QTY)*100,2) PER_DEFECT, ROUND((B.REASON_CNT/X.PROD_QTY)*100,2) PER_REASON,                                     ");
            query.AppendLine("      CASE WHEN ROUND((A.DEFECT_SUM/X.PROD_QTY)*100,2) >= 10 THEN 'Y' ELSE 'N' END PROD_MARK,                                                  ");
            query.AppendLine("      CASE WHEN ROUND((B.REASON_CNT/X.PROD_QTY)*100,2) >= 5 THEN 'Y' ELSE 'N' END DEFECT_MARK                                                  ");
            query.AppendLine(" FROM (SELECT C_LINE, 'ASS' C_LOCATION, CASE WHEN NVL(SUM(Q_PROD),0) > 0 THEN NVL(SUM(Q_PROD),0) ELSE NVL(SUM(CNT),0) END PROD_QTY             ");
            query.AppendLine("         FROM (                                                                                                                                ");
            query.AppendLine("                                                                                                                                               ");
            query.AppendLine("               SELECT C_LINE, COUNT(*) CNT, 0 Q_PROD FROM TRTB_M_LINE_PROD                                                                     ");
            query.AppendLine("                WHERE D_GATHER >= TO_CHAR(SYSDATE,'YYYYMMDD')||'00'                                                                            ");
            query.AppendLine("                and D_GATHER <= TO_CHAR(SYSDATE,'YYYYMMDD')||'23'                                                                              ");
            query.AppendLine("                  AND C_LINE = '" + spLine + "'                                                                                                ");
            query.AppendLine("                GROUP BY C_LINE                                                                                                                ");
            query.AppendLine("                Union All                                                                                                                      ");
            query.AppendLine("               SELECT C_LINE, COUNT(*) CNT, 0 Q_PROD FROM TRTB_M_LINE_PROD_B                                                                   ");
            query.AppendLine("                WHERE D_GATHER >= TO_CHAR(SYSDATE,'YYYYMMDD')||'00'                                                                            ");
            query.AppendLine("                and D_GATHER <= TO_CHAR(SYSDATE,'YYYYMMDD')||'23'                                                                              ");
            query.AppendLine("                  AND C_LINE = '" + spLine + "'                                                                                                ");
            query.AppendLine("                GROUP BY C_LINE                                                                                                                ");
            query.AppendLine("                Union All                                                                                                                      ");
            query.AppendLine("               SELECT A.C_LINE, 0 Q_CNT, SUM(A.Q_PROD) Q_PROD                                                                                  ");
            query.AppendLine("                 FROM TRTB_M_PROD_IP A, TRTB_M_COMMON B                                                                                        ");
            query.AppendLine("                     ,(SELECT N_COMNAME, NVL(I_EMP_NO,'X') I_EMP_NO                                                                            ");
            query.AppendLine("                         From TRTB_M_COMMON                                                                                                    ");
            query.AppendLine("                        WHERE C_GROUP    = 'BTS'                                                                                               ");
            query.AppendLine("                          AND N_COMNAME  = '" + ipAddress + "'                                                                                 ");
            query.AppendLine("                        GROUP BY N_COMNAME, I_EMP_NO                                                                                           ");
            query.AppendLine("                      ) C                                                                                                                      ");
            query.AppendLine("                WHERE A.D_GATHER >= TO_CHAR(SYSDATE,'YYYYMMDD')||'00'                                                                          ");
            query.AppendLine("                and A.D_GATHER <= TO_CHAR(SYSDATE,'YYYYMMDD')||'23'                                                                            ");
            query.AppendLine("                  AND A.IP_ADDRESS = B.N_COMNAME                                                                                               ");
            query.AppendLine("                  AND B.N_COMNAME  = C.I_EMP_NO                                                                                                ");
            query.AppendLine("                  AND B.C_GROUP    = 'M69'                                                                                                     ");
            query.AppendLine("                  AND A.C_LINE     = '" + spLine + "'                                                                                          ");
            query.AppendLine("                GROUP BY A.C_LINE                                                                                                              ");
            query.AppendLine("              )                                                                                                                                ");
            query.AppendLine("        GROUP BY C_LINE                                                                                                                        ");
            query.AppendLine("      ) X,                                                                                                                                     ");
            query.AppendLine("      (SELECT A.C_LINE, COUNT(*) DEFECT_SUM                                                                                                    ");
            query.AppendLine("         From TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                                            ");
            query.AppendLine("        WHERE A.REASON_ID = B.REASON_ID                                                                                                        ");
            query.AppendLine("  AND A.D_GATHER >= TO_CHAR(SYSDATE,'YYYYMMDD')||'00'                                                                                          ");
            query.AppendLine("  and  A.D_GATHER <= TO_CHAR(SYSDATE,'YYYYMMDD')||'23'                                                                                         ");
            query.AppendLine("          AND A.C_LINE LIKE '" + spLine + "'                                                                                                   ");
            query.AppendLine("          AND B.DEPT_CODE = '" + spDeptCode + "'                                                                                               ");
            query.AppendLine("        GROUP BY A.C_LINE                                                                                                                      ");
            query.AppendLine("      ) A,                                                                                                                                     ");
            query.AppendLine("      (SELECT A.C_LINE, A.REASON_ID, COUNT(*) REASON_CNT                                                                                       ");
            query.AppendLine("         From TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                                            ");
            query.AppendLine("        WHERE A.REASON_ID = B.REASON_ID                                                                                                        ");
            query.AppendLine("  AND A.D_GATHER >= TO_CHAR(SYSDATE,'YYYYMMDD')||'00'                                                                                          ");
            query.AppendLine("  and  A.D_GATHER <= TO_CHAR(SYSDATE,'YYYYMMDD')||'23'                                                                                         ");
            query.AppendLine("          AND A.C_LINE LIKE '" + spLine + "'                                                                                                   ");
            query.AppendLine("          AND B.DEPT_CODE = '" + spDeptCode + "'                                                                                               ");
            query.AppendLine("        GROUP BY A.C_LINE, A.REASON_ID                                                                                                         ");
            query.AppendLine("      ) B                                                                                                                                      ");
            query.AppendLine("WHERE X.C_LINE = A.C_LINE(+)                                                                                                                   ");
            query.AppendLine("  AND X.C_LINE = B.C_LINE(+)                                                                                                                   ");
            query.AppendLine("ORDER BY 1,3                                                                                                                                   ");


            ProductionInformation = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return ProductionInformation;
        }
        private DataTable GetAllData()
        {
            int prodqty = 0;
            int.TryParse(finishedCountScan, out prodqty);
            StringBuilder query = new StringBuilder();
            query.AppendLine("            SELECT D_GATHER,                                                                                                  ");
            query.AppendLine("       DEPT,                                                                                                                  ");
            query.AppendLine("       C_LINE,                                                                                                                ");
            query.AppendLine("       IP_ADDRESS,                                                                                                            ");
            query.AppendLine("       Q_FAIL_1,                                                                                                              ");
            query.AppendLine("       Q_FAIL_2,                                                                                                              ");
            query.AppendLine("       TOTAL_INS_PASS,                                                                                                        ");
            query.AppendLine("       RE_INS_PASS,                                                                                                           ");
            query.AppendLine("       FIRST_INS_PASS,                                                                                                        ");
            query.AppendLine("       TTL_DEFECT,                                                                                                            ");
            query.AppendLine("       ROUND(100 - (Q_FAIL_1 / (Q_FAIL_1 + FIRST_INS_PASS) * 100), 2) RFT,                                                    ");
            query.AppendLine("       ROUND((Q_FAIL_2 / Q_FAIL_1) * 1000000,2) RE_INSP_DDPM,                                                                 ");
            query.AppendLine("       ROUND(TTL_DEFECT / ((Q_FAIL_1 + FIRST_INS_PASS) + (Q_FAIL_2 + RE_INS_PASS)) * 1000000) EOL_DPPM                        ");
            query.AppendLine("  FROM(SELECT TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS')                                                                            ");
            query.AppendLine("                     D_GATHER,                                                                                                ");
            query.AppendLine("                 'ASS'                                                                                                        ");
            query.AppendLine("                     DEPT,                                                                                                    ");
            query.AppendLine("                 C_LINE,                                                                                                      ");
            query.AppendLine("                 IP_ADDRESS,                                                                                                  ");
            query.AppendLine("                 SUM(CASE WHEN SEQ = 1 THEN Q_FAIL ELSE 0 END)                                                                ");
            query.AppendLine("                     Q_FAIL_1,                                                                                                ");
            query.AppendLine("                 SUM(CASE WHEN SEQ = 2 THEN Q_FAIL ELSE 0 END)                                                                ");
            query.AppendLine("                     Q_FAIL_2,                                                                                                ");
            query.AppendLine("                 " + prodqty + "                                                                                              ");
            query.AppendLine("                     TOTAL_INS_PASS,                                                                                          ");
            query.AppendLine("                   SUM(CASE WHEN SEQ = 1 THEN Q_FAIL ELSE 0 END)                                                              ");
            query.AppendLine("                 - SUM(CASE WHEN SEQ = 2 THEN Q_FAIL ELSE 0 END)                                                              ");
            query.AppendLine("                     RE_INS_PASS,                                                                                             ");
            query.AppendLine("                   " + prodqty + "                                                                                            ");
            query.AppendLine("                 - (SUM(CASE WHEN SEQ = 1 THEN Q_FAIL ELSE 0 END)                                                             ");
            query.AppendLine("                    - SUM(CASE WHEN SEQ = 2 THEN Q_FAIL ELSE 0 END))                                                          ");
            query.AppendLine("                     FIRST_INS_PASS,                                                                                          ");
            query.AppendLine("                 SUM(Q_FAIL)                                                                                                  ");
            query.AppendLine("                     TTL_DEFECT                                                                                               ");
            query.AppendLine("            FROM EOL_DEFECT_GATHER                                                                                            ");
            query.AppendLine("           WHERE     D_GATHER > TO_CHAR(SYSDATE, 'YYYYMMDD') || '000000'                                                      ");
            query.AppendLine("                 AND C_LINE = '" + LineName + "'                                                                              ");
            query.AppendLine("        GROUP BY C_LINE, IP_ADDRESS)                                                                                 ");


            RFT_DPPM = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return RFT_DPPM;
        }
        private DataTable GetAllData_20260310()
        {
            int prodqty = 0;
            int.TryParse(finishedCountScan, out prodqty);
            StringBuilder query = new StringBuilder();
            query.AppendLine("            SELECT D_GATHER,                                                                                                   ");
            query.AppendLine("DEPT,                                                                                                                          ");
            query.AppendLine("C_LINE,                                                                                                                        ");
            query.AppendLine("IP_ADDRESS,                                                                                                                    ");
            query.AppendLine("Q_FAIL_1,                                                                                                                      ");
            query.AppendLine("Q_FAIL_2,                                                                                                                      ");
            query.AppendLine("TOTAL_INS_PASS,                                                                                                                ");
            query.AppendLine("RE_INS_PASS,                                                                                                                   ");
            query.AppendLine("FIRST_INS_PASS,                                                                                                                ");
            query.AppendLine("TTL_DEFECT,                                                                                                                    ");
            query.AppendLine("ROUND(100 - (Q_FAIL_1 / (Q_FAIL_1 + FIRST_INS_PASS) * 100), 2) RFT,                                                            ");
            query.AppendLine("       ROUND((Q_FAIL_2 / Q_FAIL_1) * 1000000, 2) RE_INSP_DDPM,                                                                 ");
            query.AppendLine("       ROUND(TTL_DEFECT / ((Q_FAIL_1 + FIRST_INS_PASS) + (Q_FAIL_2 + RE_INS_PASS)) * 1000000) EOL_DPPM                         ");
            query.AppendLine("  FROM(SELECT TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS')                                                                             ");
            query.AppendLine("                     D_GATHER,                                                                                                 ");
            query.AppendLine("                 'ASS'                                                                                                         ");
            query.AppendLine("                     DEPT,                                                                                                     ");
            query.AppendLine("                 C_LINE,                                                                                                       ");
            query.AppendLine("                 IP_ADDRESS,                                                                                                   ");
            query.AppendLine("                 SUM(CASE WHEN SEQ = 1 THEN Q_FAIL ELSE 0 END)                                                                 ");
            query.AppendLine("                     Q_FAIL_1,                                                                                                 ");
            query.AppendLine("                 SUM(CASE WHEN SEQ = 2 THEN Q_FAIL ELSE 0 END)                                                                 ");
            query.AppendLine("                     Q_FAIL_2,                                                                                                 ");
            query.AppendLine("                 SUM(Q_PASS)                                                                                                   ");
            query.AppendLine("                     TOTAL_INS_PASS,                                                                                           ");
            query.AppendLine("                   SUM(CASE WHEN SEQ = 2 THEN Q_PASS ELSE 0 END)                                                               ");
            query.AppendLine("                     RE_INS_PASS,                                                                                              ");
            query.AppendLine("                   SUM(CASE WHEN SEQ = 1 THEN Q_PASS ELSE 0 END)                                                               ");
            query.AppendLine("                     FIRST_INS_PASS,                                                                                           ");
            query.AppendLine("                 SUM(Q_FAIL)                                                                                                   ");
            query.AppendLine("                     TTL_DEFECT                                                                                                ");
            query.AppendLine("            FROM EOL_DEFECT_GATHER                                                                                             ");
            query.AppendLine("           WHERE     D_GATHER > TO_CHAR(SYSDATE, 'YYYYMMDD') || '000000'                                                       ");
            query.AppendLine("                 AND C_LINE = '" + LineName + "'                                                                               ");
            query.AppendLine("        GROUP BY C_LINE, IP_ADDRESS)                                                                                           ");


            RFT_DPPM = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return RFT_DPPM;
        }
        private DataTable GetRFT_DPPM()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("            SELECT TTL_DEFECT, TTL_DEFECT_SEQ1, TTL_DEFECT_SEQ2,                                                                ");
            query.AppendLine("ROUND(TTL_DEFECT / (TTL_CHECK_SEQ1 + TTL_CHECK_SEQ2) * 1000000)DPPM,                                                           ");
            query.AppendLine("round(PASS_1ST / (PASS_1ST + FAIL_1ST) * 100, 2) RFT FROM(                                                                      ");
            query.AppendLine(" SELECT sum(q_fail) TTL_DEFECT                                                                                                  ");
            query.AppendLine(" , sum(case when seq = 1 then q_fail else 0 end )TTL_DEFECT_SEQ1                                                                ");
            query.AppendLine(",sum(case when seq = 2 then q_fail else 0 end )TTL_DEFECT_SEQ2                                                                  ");
            query.AppendLine(",sum(case when seq = 1 then q_fail + q_pass else 0 end )TTL_CHECK_SEQ1                                                          ");
            query.AppendLine(",sum(case when seq = 2 then q_fail + q_pass else 0 end )TTL_CHECK_SEQ2,                                                         ");
            query.AppendLine("sum(case when seq = 1 then q_pass else 0 end )PASS_1ST,                                                                         ");
            query.AppendLine("sum(case when seq = 1 then q_fail else 0 end )FAIL_1ST                                                                          ");
            query.AppendLine(" FROM EOL_DEFECT_GATHER WHERE D_GATHER > to_char(sysdate,'YYYYMMDD') || '000000' AND C_LINE = '" + LineName + "')                          ");








            RFT_DPPM = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return RFT_DPPM;

        }
        private DataTable GetTop3DPPM()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            query.AppendLine("            SELECT* FROM(                                                                                               ");
            query.AppendLine("SELECT B.C_LINE, REASON_ID, ROUND(SUM(TTL_DEFECT)/                                                                      ");
            query.AppendLine("  (MAX(TTL_CHECK_SEQ1) + MAX(TTL_CHECK_SEQ2)) * 1000000)DPPM FROM(                                                     ");
            query.AppendLine("  SELECT C_LINE,                                                                                                        ");
            query.AppendLine("  sum(case when seq = 1 then q_fail+q_pass else 0 end )TTL_CHECK_SEQ1                                                   ");
            query.AppendLine(",sum(case when seq = 2 then q_fail + q_pass else 0 end )TTL_CHECK_SEQ2                                                  ");
            query.AppendLine("   FROM EOL_DEFECT_GATHER WHERE D_GATHER > to_char(sysdate, 'YYYYMMDD') || '000000'                                     ");
            query.AppendLine(" AND C_LINE = '" + LineName + "' GROUP BY C_LINE)A,                                                                                 ");
            query.AppendLine(" (SELECT C_LINE, REASON_ID, SUM(Q_FAIL)TTL_DEFECT                                                                       ");
            query.AppendLine("  FROM EOL_DEFECT_GATHER WHERE D_GATHER > to_char(sysdate, 'YYYYMMDD') || '000000'                                      ");
            query.AppendLine("AND C_LINE = '" + LineName + "' GROUP BY C_LINE,REASON_ID) B                                                                        ");
            query.AppendLine(" WHERE A.C_LINE = B.C_LINE                                                                                              ");
            query.AppendLine(" GROUP BY B.C_LINE,REASON_ID                                                                                            ");
            query.AppendLine("           ORDER BY 2 DESC ) a,mes.trtb_m_bts_reason3 @inf_m_e b         ");
            query.AppendLine("where a.reason_id = b.reason_id  AND DEPT_CODE = 'ASS'                                        ");
            query.AppendLine(" and ROWNUM <= 3                                                         ");
            query.AppendLine("order by 3 desc                                                          ");





            Top3DPPM = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return Top3DPPM;

        }
        private void GetDataTopDefect(string line)
        {
            StringBuilder query = new StringBuilder();
            {
                query.AppendLine("");
                query.AppendLine("  SELECT A.C_LINE,                                                            ");
                query.AppendLine("         A.REASON_ID,                                                         ");
                query.AppendLine("         A.COUNT_DEFECT,                                                      ");
                query.AppendLine("            A.REASON_EN                                                       ");
                query.AppendLine("         || ' ('                                                              ");
                query.AppendLine("         || ROUND (COUNT_DEFECT / TOP_DEFECT * 100, 2)                        ");
                query.AppendLine("         || '%)'                                                              ");
                query.AppendLine("            TOP_DEFECT_EN,                                                    ");
                query.AppendLine("            A.REASON_VN                                                       ");
                query.AppendLine("         || ' ('                                                              ");
                query.AppendLine("         || ROUND (COUNT_DEFECT / TOP_DEFECT * 100, 2)                        ");
                query.AppendLine("         || '%)'                                                              ");
                query.AppendLine("            TOP_DEFECT_VN                                                     ");
                query.AppendLine("    FROM (  SELECT C_LINE,                                                    ");
                query.AppendLine("                   A.REASON_ID,                                               ");
                query.AppendLine("                   SUM (Q_COUNT) COUNT_DEFECT,                                ");
                query.AppendLine("                   REASON_EN,                                                 ");
                query.AppendLine("                   REASON_VN                                                  ");
                query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B      ");
                query.AppendLine("             WHERE     D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'     ");
                query.AppendLine("                   AND C_LINE LIKE '" + line + "'                             ");
                query.AppendLine("                   AND A.REASON_ID = B.REASON_ID                              ");
                query.AppendLine("                   AND B.DEPT_CODE = 'ASS'                                    ");
                query.AppendLine("          GROUP BY C_LINE,                                                    ");
                query.AppendLine("                   A.REASON_ID,                                               ");
                query.AppendLine("                   B.REASON_EN,                                               ");
                query.AppendLine("                   REASON_VN) A,                                              ");
                query.AppendLine("         (  SELECT SUM (Q_COUNT) TOP_DEFECT                                   ");
                query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B      ");
                query.AppendLine("             WHERE     D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'     ");
                query.AppendLine("                   AND C_LINE LIKE '" + line + "'                             ");
                query.AppendLine("                   AND A.REASON_ID = B.REASON_ID                              ");
                query.AppendLine("                   AND B.DEPT_CODE = 'ASS'                                    ");
                query.AppendLine("          GROUP BY C_LINE) G                                                  ");
                query.AppendLine("ORDER BY 3 DESC                                                               ");
            }
            DataTable dt = new DataTable();
            if (dt == null)
            {
            }
            else
            {
                TopDefect = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            }
        }
        private void GetErrorCount(string line)
        {
            StringBuilder query = new StringBuilder();
            {
                query.AppendLine("");

                query.AppendLine("                SELECT 'ERROR' GR,                                       ");
                query.AppendLine("CASE WHEN reason_id NOT IN (17,18,21) THEN 0 ELSE reason_id END reason_id ,count(*) cnt                                                   ");
                query.AppendLine("FROM MES.TRTB_M_BTS_COUNT3                                               ");
                query.AppendLine("where d_gather like TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'                 ");
                query.AppendLine("and c_line = '" + line + "'                                              ");
                query.AppendLine("group by CASE WHEN reason_id NOT IN (17,18,21) THEN 0 ELSE reason_id END                                                       ");
                query.AppendLine("union all                                                                ");
                query.AppendLine("SELECT 'PART' GR,                                                        ");
                query.AppendLine("part_id,count(*) cnt                                                     ");
                query.AppendLine("FROM MES.TRTB_M_BTS_COUNT3                                               ");
                query.AppendLine("where d_gather like TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'                 ");
                query.AppendLine("and c_line = '" + line + "'                                              ");
                query.AppendLine("group by part_id                                                         ");
                query.AppendLine("order by 1,2                                                             ");



            }
            DataTable dt = new DataTable();
            if (dt == null)
            {
            }
            else
            {
                ErrorCount = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            }
        }
        private void BindProductQTY()
        {
            if (ProductionInformation != null)
            {
                if (ProductionInformation.Rows.Count > 0)
                {
                    this.lblProdTotal.Text = "SX : " + ProductionInformation.Rows[0]["PROD_QTY"].ToString();
                    this.lblPassTotal.Text = "PASS: " + ProductionInformation.Rows[0]["PROD_QTY"].ToString();
                    this.lblFailTotal.Text = ProductionInformation.Rows[0]["DEFECT_SUM"].ToString();
                    this.lblFailTotal.Text = "FAIL : " + TotalDefect;
                    string a = ProductionInformation.Rows[0]["DEFECT_SUM"].ToString();
                    if (a == "")
                    {
                        a = "0";
                    }
                    if (TotalDefect > Convert.ToInt32(a))
                    {
                        if (backgroundSyncData.IsBusy)
                        {

                        }
                        else
                        {
                            backgroundSyncData.RunWorkerAsync();
                        }
                    }
                    lblSyncStatus.Text = TotalDefect + "/" + a;
                    //TotalPass = Convert.ToInt32(ProductionInformation.Rows[0]["PROD_QTY"].ToString());
                    finishedCountScan = ProductionInformation.Rows[0]["PROD_QTY"].ToString();
                }
            }
        }
        private void BindTop3DDPMRFT()
        {
            try
            {
                if (RFT_DPPM != null)
                {
                    if (RFT_DPPM.Rows.Count > 0)
                    {
                        int totalpass = 0;
                        lblTotalDefect.Text = RFT_DPPM.Rows[0]["TTL_DEFECT"].ToString();
                        lblFirstDefect.Text = RFT_DPPM.Rows[0]["Q_FAIL_1"].ToString();
                        lblReDefect.Text = RFT_DPPM.Rows[0]["Q_FAIL_2"].ToString();
                        lblRFT.Text = RFT_DPPM.Rows[0]["RFT"].ToString() + "%";
                        lblEOLQCDDPM.Text = RFT_DPPM.Rows[0]["EOL_DPPM"].ToString();
                        lbl1stPass.Text = "1st PASS : " + RFT_DPPM.Rows[0]["FIRST_INS_PASS"].ToString();
                        lblPassTotal.Text = "PASS : " + RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString();
                        TotalPass = Convert.ToInt32(RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString());
                        if (!int.TryParse(RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString(), out totalpass))
                        {
                            totalpass = 0;
                        }
                        TotalPass = totalpass;
                        lblFailTotal.Text = "FAIL : " + RFT_DPPM.Rows[0]["TTL_DEFECT"].ToString();

                        //int re_insp = 0;
                        //int first_insp_fail = 0;
                        //int prod_qty = 0;
                        //int first_insp_pass = 0;
                        //if(!int.TryParse(RFT_DPPM.Rows[0]["TTL_DEFECT_SEQ1"].ToString(), out first_insp_fail))
                        //{
                        //    first_insp_fail = 0;
                        //}    


                        //if (int.TryParse(ProductionInformation.Rows[0]["PROD_QTY"].ToString(),out prod_qty))
                        //{
                        //    if (int.TryParse(RFT_DPPM.Rows[0]["TTL_DEFECT_SEQ2"].ToString(), out re_insp))
                        //    {
                        //        first_insp_pass = prod_qty - re_insp;
                        //        this.lbl1stPass.Text = "1st PASS: " + first_insp_pass;
                        //        lblRFT.Text = Math.Round(first_insp_fail / (first_insp_fail + first_insp_pass) * 100.00, 2) + "%";
                        //    }
                        //    else
                        //    {

                        //    }
                        //}

                        ////lblRFT.Text = RFT_DPPM.Rows[0]["RFT"].ToString() + "%";
                        //lblEOLQCDDPM.Text = Convert.ToInt64(RFT_DPPM.Rows[0]["DPPM"]).ToString("N0");
                    }
                }
                if (Top3DPPM != null)
                {
                    if (Top3DPPM.Rows.Count > 0)
                    {
                        if (Top3DPPM.Rows.Count == 1)
                        {
                            lblTop1Defect.Text = Top3DPPM.Rows[0]["REASON_EN"].ToString(); lblTop1DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[0]["DPPM"]).ToString("N0");

                        }
                        else if (Top3DPPM.Rows.Count == 2)
                        {
                            lblTop1Defect.Text = Top3DPPM.Rows[0]["REASON_EN"].ToString(); lblTop1DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[0]["DPPM"]).ToString("N0");
                            lblTop2Defect.Text = Top3DPPM.Rows[1]["REASON_EN"].ToString(); lblTop2DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[1]["DPPM"]).ToString("N0");
                        }
                        else if (Top3DPPM.Rows.Count == 3)
                        {
                            lblTop1Defect.Text = Top3DPPM.Rows[0]["REASON_EN"].ToString(); lblTop1DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[0]["DPPM"]).ToString("N0");
                            lblTop2Defect.Text = Top3DPPM.Rows[1]["REASON_EN"].ToString(); lblTop2DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[1]["DPPM"]).ToString("N0");
                            lblTop3Defect.Text = Top3DPPM.Rows[2]["REASON_EN"].ToString(); lblTop3DefectDDPM.Text = Convert.ToInt64(Top3DPPM.Rows[2]["DPPM"]).ToString("N0");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Contains("DBNull"))
                {
                    ShowMessage("No Data DPPM now.", Color.Red);
                }
            }
        }


        //===============================================================================




        //private void ConfigCountErrorButton(DataTable dataErrorCnt)
        //{
        //    string id;
        //    int count = 0;
        //    try
        //    {
        //        if (dataErrorCnt == null || dataErrorCnt.Rows.Count == 0)
        //        {
        //            foreach (var panel in tableLayoutPanel2.Controls)
        //            {
        //                if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //                {
        //                    foreach (PanelControl pnl in tableLayoutPanel2.Controls)
        //                    {
        //                        foreach (var a in pnl.Controls)
        //                        {
        //                            if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                            {
        //                                LabelControl lbl = (LabelControl)a;
        //                                if (lbl.AccessibleName != null && lbl.AccessibleName.ToString().StartsWith("C"))
        //                                {
        //                                    lbl.Text = "0";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            foreach (var panel in tableLayoutErrorRight.Controls)
        //            {
        //                if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //                {
        //                    PanelControl pnl = (PanelControl)panel;
        //                    foreach (var a in pnl.Controls)
        //                    {
        //                        if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                        {
        //                            LabelControl lbl = (LabelControl)a;
        //                            if (lbl.AccessibleName != null && lbl.AccessibleName.ToString().StartsWith("C"))
        //                            {
        //                                lbl.Text = "0";
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //            foreach (var panel in tableLayoutErrorLeft.Controls)
        //            {
        //                if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //                {
        //                    PanelControl pnl = (PanelControl)panel;
        //                    foreach (var a in pnl.Controls)
        //                    {
        //                        if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                        {
        //                            LabelControl lbl = (LabelControl)a;
        //                            if (lbl.AccessibleName != null && lbl.AccessibleName.ToString().StartsWith("C"))
        //                            {
        //                                lbl.Text = "0";
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (dataErrorCnt.Rows.Count > 0)
        //            {
        //                for (int i = 0; i < dataErrorCnt.Rows.Count; i++)
        //                {
        //                    if (dataErrorCnt.Rows[i]["GR"] != null)
        //                    {
        //                        if (dataErrorCnt.Rows[i]["GR"].ToString() == "ERROR")
        //                        {
        //                            id = dataErrorCnt.Rows[i]["REASON_ID"].ToString();
        //                            count = Convert.ToInt32(dataErrorCnt.Rows[i]["CNT"].ToString());

        //                            BindCountToErrorLabel(id, count);
        //                        }
        //                        else if (dataErrorCnt.Rows[i]["GR"].ToString() == "PART")
        //                        {
        //                            id = dataErrorCnt.Rows[i]["REASON_ID"].ToString();
        //                            count = Convert.ToInt32(dataErrorCnt.Rows[i]["CNT"].ToString());
        //                            BindCountToErrorPartLabel(id, count);
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}


        //===============================================================================


        private void ConfigCountErrorButton(DataTable dataErrorCnt)
        {
            try
            {
                if (dataErrorCnt == null || dataErrorCnt.Rows.Count == 0)
                {
                    ResetCountLabels(tableLayoutPanel2);
                    ResetCountLabels(tableLayoutErrorRight);
                    ResetCountLabels(tableLayoutErrorLeft);
                    return;
                }

                foreach (DataRow row in dataErrorCnt.Rows)
                {
                    string gr = row["GR"]?.ToString();
                    string id = row["REASON_ID"]?.ToString();
                    int count = 0;

                    int.TryParse(row["CNT"]?.ToString(), out count);

                    if (gr == "ERROR")
                    {
                        BindCountToErrorLabel(id, count);
                    }
                    else if (gr == "PART")
                    {
                        BindCountToErrorPartLabel(id, count);
                    }
                }
            }
            catch
            {
            }
        }

        private void ResetCountLabels(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Panel panel)
                {
                    foreach (Control child in panel.Controls)
                    {
                        if (child is Label lbl &&
                            !string.IsNullOrEmpty(lbl.AccessibleName) &&
                            lbl.AccessibleName.StartsWith("C"))
                        {
                            lbl.Text = "0";
                        }
                    }
                }
            }
        }


        //===============================================================================




        //private void BindCountToErrorLabel(string id, int cnt)
        //{
        //    foreach (var panel in tableLayoutPanel2.Controls)
        //    {
        //        if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //            foreach (var a in pnl.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                {
        //                    LabelControl lbl = (LabelControl)a;
        //                    if (lbl.AccessibleName != null && lbl.AccessibleName == "C" + id)
        //                    {
        //                        lbl.Text = "(" + cnt + ")";
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    foreach (var panel in tableLayoutErrorLeft.Controls)
        //    {
        //        if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //            foreach (var a in pnl.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                {
        //                    LabelControl lbl = (LabelControl)a;
        //                    if (lbl.AccessibleName != null && lbl.AccessibleName == "C" + id)
        //                    {
        //                        lbl.Text = "(" + cnt + ")";
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //    foreach (var panel in tableLayoutErrorRight.Controls)
        //    {
        //        if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
        //            foreach (var a in pnl.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //                {
        //                    LabelControl lbl = (LabelControl)a;
        //                    if (lbl.AccessibleName != null && lbl.AccessibleName == "C" + id)
        //                    {
        //                        lbl.Text = "(" + cnt + ")";
        //                    }
        //                    else
        //                    {

        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        //private void BindCountToErrorPartLabel(string id, int cnt)
        //{
        //    foreach (var a in pictureShoes.Controls)
        //    {
        //        if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
        //        {
        //            LabelControl lbl = (LabelControl)a;
        //            if (lbl.AccessibleName != null)
        //            {
        //                if (lbl.AccessibleName == "CP" + id)
        //                    lbl.Text = "(" + cnt + ")";
        //            }
        //            else
        //            {
        //                string sss = "";
        //                sss = lbl.Name.ToString();
        //            }
        //        }
        //    }
        //}


        //===============================================================================



        private void BindCountToErrorLabel(string id, int cnt)
        {
            UpdateLabelTextByAccessibleName(tableLayoutPanel2, "C" + id, cnt);
            UpdateLabelTextByAccessibleName(tableLayoutErrorLeft, "C" + id, cnt);
            UpdateLabelTextByAccessibleName(tableLayoutErrorRight, "C" + id, cnt);
        }

        private void BindCountToErrorPartLabel(string id, int cnt)
        {
            UpdateLabelTextByAccessibleName(pictureShoes, "CP" + id, cnt);
        }

        private void UpdateLabelTextByAccessibleName(Control parent, string accessibleName, int cnt)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    if (!string.IsNullOrEmpty(lbl.AccessibleName) &&
                        lbl.AccessibleName == accessibleName)
                    {
                        lbl.Text = "(" + cnt + ")";
                    }
                }

                if (ctrl.HasChildren)
                {
                    UpdateLabelTextByAccessibleName(ctrl, accessibleName, cnt);
                }
            }
        }


        //===============================================================================




        private void timerBindProduction_Tick(object sender, EventArgs e)
        {
            if (!backgroundOracle.IsBusy)
            {
                backgroundOracle.RunWorkerAsync();
            }
            else
            {

            }
        }
        private void labelControl7_Click(object sender, EventArgs e)
        {

        }


        //===============================================================================




        //private void lblPart_Click(object sender, EventArgs e)
        //{
        //    if (btnChonModel.Text == "" || btnChonModel.Text == "Chọn MODEL")
        //    {
        //        ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);
        //        return;
        //    }
        //    lblPart1.ForeColor = System.Drawing.Color.Green;
        //    lblPart2.ForeColor = System.Drawing.Color.Green;
        //    lblPart3.ForeColor = System.Drawing.Color.Green;
        //    lblPart5.ForeColor = System.Drawing.Color.Green;
        //    lblPart4.ForeColor = System.Drawing.Color.Green;
        //    lblPart6.ForeColor = System.Drawing.Color.Green;

        //    Label lbl = (Label)sender;
        //    lbl.ForeColor = System.Drawing.Color.Red;// b?m l?i v? trí chuy?n sang d?

        //    btnPass.Enabled = false;
        //    btnFail.Enabled = false;
        //    btnRePass.Enabled = false;
        //    btnReFail.Enabled = false;
        //    ConffigErrorButton(true);
        //    partID = lbl.AccessibleName;

        //    foreach (var pnl in tableLayoutPanel2.Controls)
        //    {
        //        if (pnl.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl panel = (DevExpress.XtraEditors.PanelControl)pnl;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btnID = (SimpleButton)a;
        //                    btnID.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
        //                    btnID.Appearance.BackColor2 = System.Drawing.Color.FromArgb(192, 255, 255);
        //                }
        //            }
        //        }
        //    }
        //    foreach (var pnl in tableLayoutErrorLeft.Controls)
        //    {
        //        if (pnl.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl panel = (DevExpress.XtraEditors.PanelControl)pnl;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btnID = (SimpleButton)a;
        //                    btnID.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
        //                    btnID.Appearance.BackColor2 = System.Drawing.Color.FromArgb(192, 255, 255);
        //                }
        //            }
        //        }
        //    }


        //    foreach (var pnl in tableLayoutErrorRight.Controls)
        //    {
        //        if (pnl.ToString() == "DevExpress.XtraEditors.PanelControl")
        //        {
        //            DevExpress.XtraEditors.PanelControl panel = (DevExpress.XtraEditors.PanelControl)pnl;
        //            foreach (var a in panel.Controls)
        //            {
        //                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
        //                {
        //                    SimpleButton btnID = (SimpleButton)a;
        //                    btnID.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
        //                    btnID.Appearance.BackColor2 = System.Drawing.Color.FromArgb(192, 255, 255);
        //                }
        //            }
        //        }
        //    }


        //}


        //===============================================================================


        private void lblPart_Click(object sender, EventArgs e)
        {
            
            if (string.IsNullOrEmpty(btnChonModel.Text) || btnChonModel.Text == "Chọn MODEL")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);
                return;
            }

            
            Label[] allParts = { lblPart1, lblPart2, lblPart3, lblPart4, lblPart5, lblPart6 };
            foreach (var p in allParts) p.ForeColor = Color.Green;

            
            Label lbl = (Label)sender;
            lbl.ForeColor = Color.Red;
             
            
            SetInspectionActionState(false, false, false, false);

            
            ConffigErrorButton(true);
            partID = lbl.AccessibleName;

            
            ResetReasonButtonColors();
        }


        //===============================================================================



        private void simpleButton23_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (btn.AccessibleName == "")
            {
                Popup.SelectFail selectFail = new Popup.SelectFail();
                selectFail.ShowDialog(this);
                if (selectFail.returnData != null)
                {
                    SetInspectionActionState(false, true, false, true);
                    DataTable dt = selectFail.returnData;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["COUNT"].ToString() != "0")
                        {
                            string reasonCode = dt.Rows[i]["REASON_ID"].ToString();
                            DataRow dr = dtReason.NewRow();
                            dr["PART"] = partID;
                            dr["REASON"] = reasonCode;
                            dr["USE_YN"] = "Y";
                            dtReason.Rows.Add(dr);
                        }
                    }
                    btn.Enabled = false;
                }
            }
            else
            {

                SetInspectionActionState(false, true, false, true);
                if (btn.BackColor == ReasonButtonDefaultColor)
                {
                    btn.BackColor = ReasonButtonSelectedColor;
                    
                    UpdateDtReason(btn);
                }
                else
                {
                    btn.BackColor = ReasonButtonDefaultColor;
                    UpdateDtReason(btn);
                }
            }
        }
        private void UpdateDtReason(Button btnID)
        {
            reasonID = btnID.AccessibleName;
            if (dtReason.Rows.Count > 0)
            {
                DataRow findRow = dtReason.Select("PART = '" + partID + "' AND REASON = '" + reasonID + "'").FirstOrDefault();
                if (findRow != null)
                {
                    if (findRow["USE_YN"].ToString() == "N")
                    {
                        findRow["USE_YN"] = "Y";
                    }
                    else
                    {
                        findRow["USE_YN"] = "N";
                    }
                }
                else
                {
                    DataRow dr = dtReason.NewRow();
                    dr["PART"] = partID;
                    dr["REASON"] = reasonID;
                    dr["USE_YN"] = "Y";
                    dtReason.Rows.Add(dr);
                }
            }
            else
            {
                DataRow dr = dtReason.NewRow();
                dr["PART"] = partID;
                dr["REASON"] = reasonID;
                dr["USE_YN"] = "Y";
                dtReason.Rows.Add(dr);
            }
            //dtReason.Clear();
            gridControl1.DataSource = dtReason;
        }
        private void btnPass_Click(object sender, EventArgs e)
        {
            if (btnChonModel.Text == "" || btnChonModel.Text == "Chọn MODEL")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);

                return;
            }
            string C_STYLE = btnChonModel.Text;
            double a = new Random().Next(0, 60);

            if (toggleSwitchOnline.Checked)
            {
                UpdatePassFailDirectToDB(true, "1");
            }
            else
            {
                if (WritePassToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + spDeptCode + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + "0" + ";" + "0" + ";" + "0" + ";" + 1 + ";" + 0 + ";" + "1"))
                {
                    TotalPass = TotalPass + 1;
                    lblPassTotal.Text = "PASS:" + " " + TotalPass;
                }
            }
            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }
        private bool WritePassToLogFile(string content)
        {
            try
            {
                string filename;
                filename = "PassBTS_" + DateTime.Now.ToString("yyyyMMdd");
                etc.WriteToFile(content, "Pass", filename);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private void btnRePass_Click(object sender, EventArgs e)
        {
            if (btnChonModel.Text == "" || btnChonModel.Text == "Chọn MODEL")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);
                return;
            }
            string C_STYLE = btnChonModel.Text;
            double a = new Random().Next(0, 60);

            if (toggleSwitchOnline.Checked)
            {
                UpdatePassFailDirectToDB(true, "2");
            }
            else
            {
                if (WritePassToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + spDeptCode + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + "0" + ";" + "0" + ";" + "0" + ";" + 1 + ";" + 0 + ";" + "2"))
                {
                    TotalPass = TotalPass + 1;
                    lblPassTotal.Text = "PASS:" + " " + TotalPass;
                }
            }
            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }
        private bool UpdatePassFailDirectToDB(bool PassorFail, string seq)
        {
            string C_STYLE = btnChonModel.Text;
            string eol_Sequence = "";
            double a = new Random().Next(0, 60);
            try
            {
                if (PassorFail)
                {
                    StringBuilder strSql = new StringBuilder();
                    strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval, TO_CHAR(sysdate,'yyyyMMddHH24MISS') D_GATHER FROM DUAL");

                    DataTable dt = new DataTable();
                    dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        eol_Sequence = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        return false;
                    }
                    if (C_STYLE != "")
                    {
                        StringBuilder query = new StringBuilder();
                        query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_PASS,SEQ)");
                        query.AppendLine("VALUES( '" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "'");
                        query.AppendLine("       ,'" + spDeptCode + "'");
                        query.AppendLine("       ,'" + LineName + "'");
                        query.AppendLine("       ,'" + ipAddress + "'");
                        query.AppendLine("       ,'" + C_STYLE + "'");
                        query.AppendLine("       ,'0'");
                        query.AppendLine("       ,'0'");
                        query.AppendLine("       ,'" + eol_Sequence + "'");
                        query.AppendLine("       ,'1','" + seq + "'  ");
                        query.AppendLine("      )");

                        Console.WriteLine("Impact database " + query.ToString());
                        if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                        {
                            return true;
                        }
                        else
                        {
                            ShowMessage("Không Lưu Được", Color.Red);
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage("Error !!! Không lấy được mã giày ", Color.Red);
                        return false;
                    }
                }
                else
                {
                    StringBuilder strSql = new StringBuilder();

                    strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval, TO_CHAR(sysdate,'yyyyMMddHH24MISS') D_GATHER FROM DUAL");

                    DataTable dt = new DataTable();
                    dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        eol_Sequence = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        return false;
                    }
                    StringBuilder query = new StringBuilder();
                    query.AppendLine("");
                    query.AppendLine("INSERT INTO MES.TRTB_M_BTS_COUNT3(D_GATHER, C_LINE,                                                                       ");
                    query.AppendLine(" PART_ID, REASON_ID, MES_GROUP_SUM, USER_ID, IP_ADDRESS)                                                                  ");
                    query.AppendLine("VALUES ('" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "', '" + spLine + "',                                                                            ");
                    query.AppendLine(" '" + partID + "', '" + reasonID + "','" + Mes_Group_Sum + "','" + LeftOrRight + "','" + ipAddress + "')                ");

                    if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                    {

                        if (C_STYLE != "")
                        {
                            query = new StringBuilder();
                            query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_FAIL,SEQ)              ");
                            query.AppendLine(" VALUES( '" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "'                                                                                                          ");
                            query.AppendLine("        ,'" + spDeptCode + "'                                                                                                          ");
                            query.AppendLine("        ,'" + LineName + "'                                                                                                            ");
                            query.AppendLine("        ,'" + ipAddress + "'                                                                                                           ");
                            query.AppendLine("        ,'" + C_STYLE + "'                                                                                                       ");
                            query.AppendLine("        ,'" + partID + "'                                                                                                              ");
                            query.AppendLine("        ,'" + reasonID + "'                                                                                                            ");
                            query.AppendLine("        ,'" + eol_Sequence + "'                                                                                                        ");
                            query.AppendLine("        ,'1','" + seq + "'                                                                                                                           ");
                            query.AppendLine("                                                                                                                                       ");
                            query.AppendLine("       )                                                                                                                              ");
                            if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                            {
                                return true;
                            }
                            else
                            {
                                ShowMessage("Error !!! Không lưu được ", Color.Red);

                                return false;
                            }
                        }
                        else
                        {
                            ShowMessage("Error !!! Không lưu được ", Color.Red);
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage("Error !!! Không lưu được ", Color.Red);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Error !!! Không lưu được " + Environment.NewLine + ex.Message.ToString(), Color.Red);

                return false;
            }

        }



        //===============================================================================




        private void btnFail_Click(object sender, EventArgs e)
        {
            btnfail = new List<string>();
            DataRow dr = dtReason.Select("USE_YN = 'Y'").FirstOrDefault();
            if (dr != null)
            {
                if (toggleSwitchOnline.Checked)
                {
                    UpdatePassFailDirectToDB(false, "1");
                }
                else
                {
                    UpdateFail(btnfail);
                }
            }
            else
            {
                dtReason.Clear();
            }
            this.lblFailTotal.Text = "" + TotalDefect;

            lblRFT.Text = Math.Round(TotalDefect * 1.0 / (TotalDefect + TotalPass * 1.0) * 100, 2) + " %";
            SetInspectionActionState(true, false, true, false);
            ConffigErrorButton(false);
            lblPart1.ForeColor = System.Drawing.Color.Green;
            lblPart2.ForeColor = System.Drawing.Color.Green;
            lblPart3.ForeColor = System.Drawing.Color.Green;
            lblPart5.ForeColor = System.Drawing.Color.Green;
            lblPart4.ForeColor = System.Drawing.Color.Green;
            lblPart6.ForeColor = System.Drawing.Color.Green;
            //foreach (var panel in tableLayoutPanel2.Controls)
            //{
            //    if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
            //    {
            //        DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
            //        foreach (var a in pnl.Controls)
            //        {
            //            if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
            //            {
            //                SimpleButton btn = (SimpleButton)a;
            //                btn.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            //                btn.Appearance.BackColor2 = System.Drawing.Color.FromArgb(192, 255, 255);
            //            }
            //        }
            //    }
            //}
            ResetReasonButtonColors();

            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }



        //===============================================================================




        private void UpdateFail(List<string> btn)
        {

            string C_STYLE = btnChonModel.Text;
            foreach (DataRow dr in dtReason.Rows)
            {
                reasonID = dr["REASON"].ToString();
                partID = dr["PART"].ToString();
                double a = new Random().Next(0, 60);
                if (WriteFailToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + spDeptCode + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + partID + ";" + reasonID + ";" + Mes_Group_Sum + ";" + 1 + ";" + 0 + ";" + "1"))
                {
                    TotalDefect = TotalDefect + 1;
                    this.lblFailTotal.Text = "" + TotalDefect;
                }
            }
            dtReason.Clear();
        }
        private void UpdateFail2(List<string> btn)
        {

            string C_STYLE = btnChonModel.Text;
            foreach (DataRow dr in dtReason.Rows)
            {
                reasonID = dr["REASON"].ToString();
                partID = dr["PART"].ToString();
                double a = new Random().Next(0, 60);
                if (WriteFailToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + spDeptCode + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + partID + ";" + reasonID + ";" + Mes_Group_Sum + ";" + 1 + ";" + 0 + ";" + "2"))
                {
                    TotalDefect = TotalDefect + 1;
                    this.lblFailTotal.Text = "" + TotalDefect;
                }
            }
            dtReason.Clear();
        }
        private bool WriteFailToLogFile(string content)
        {
            try
            {
                string filename;
                filename = "FailBTS_" + DateTime.Now.ToString("yyyyMMdd");
                etc.WriteToFile(content, "Fail", filename);
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void btnReFail_Click(object sender, EventArgs e)
        {
            btnfail = new List<string>();
            DataRow dr = dtReason.Select("USE_YN = 'Y'").FirstOrDefault();
            if (dr != null)
            {
                if (toggleSwitchOnline.Checked)
                {
                    UpdatePassFailDirectToDB(false, "2");
                }
                else
                {
                    UpdateFail2(btnfail);
                }
            }
            else
            {
                dtReason.Clear();
            }
            this.lblFailTotal.Text = "" + TotalDefect;

            lblRFT.Text = Math.Round(TotalDefect * 1.0 / (TotalDefect + TotalPass * 1.0) * 100, 2) + " %";
            SetInspectionActionState(true, false, true, false);
            ConffigErrorButton(false);
            lblPart1.ForeColor = System.Drawing.Color.Green;
            lblPart2.ForeColor = System.Drawing.Color.Green;
            lblPart3.ForeColor = System.Drawing.Color.Green;
            lblPart5.ForeColor = System.Drawing.Color.Green;
            lblPart4.ForeColor = System.Drawing.Color.Green;
            lblPart6.ForeColor = System.Drawing.Color.Green;

            //foreach (var panel in tableLayoutPanel2.Controls)
            //{
            //    if (panel.ToString() == "DevExpress.XtraEditors.PanelControl")
            //    {
            //        DevExpress.XtraEditors.PanelControl pnl = (DevExpress.XtraEditors.PanelControl)panel;
            //        foreach (var a in pnl.Controls)
            //        {
            //            if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
            //            {
            //                SimpleButton btn = (SimpleButton)a;
            //                btn.Appearance.BackColor = System.Drawing.Color.FromArgb(192, 255, 255);
            //                btn.Appearance.BackColor2 = System.Drawing.Color.FromArgb(192, 255, 255);
            //            }
            //        }
            //    }
            //}
            ResetReasonButtonColors();



            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }


        //===============================================================================



        private void btnClear_Click(object sender, EventArgs e)
        {
            dtReason.Clear();
            gridControl1.DataSource = dtReason;
            SetInspectionActionState(true, false, true, false);
            ConffigErrorButton(false);
            lblPart1.ForeColor = System.Drawing.Color.Green;
            lblPart2.ForeColor = System.Drawing.Color.Green;
            lblPart3.ForeColor = System.Drawing.Color.Green;
            lblPart5.ForeColor = System.Drawing.Color.Green;
            lblPart4.ForeColor = System.Drawing.Color.Green;
            lblPart6.ForeColor = System.Drawing.Color.Green;

            ResetReasonButtonColors();

        }


        //===============================================================================



        private void btn_reasonCode2_Click(object sender, EventArgs e)
        {
            string text = "on";
            try
            {
                if (this.btn_reasonCode2.Text.ToString().Contains("Waiting"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("Y");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì";
                        this.timer_BlinkButtonYellow.Enabled = false;
                        this.btn_reasonCode2.BackColor = Color.Orange;
                        
                    });
                }
                else if (this.btn_reasonCode2.Text.ToString().Contains("Calling"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("SOUND");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì " + Environment.NewLine + "Waiting";
                        this.timer_BlinkButtonYellow.Enabled = false;
                        this.btn_reasonCode2.BackColor = Color.DarkRed;
                        
                    });
                }
                else
                {
                    Task task = this.TurnOnAndonAsync("Y");
                    this.TurnOnAndonAsync("SOUND");
                    if (task.IsCompleted)
                    {
                        this.btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì" + Environment.NewLine + "Calling";
                        this.timer_BlinkButtonYellow.Enabled = true;
                    }
                }
                using (frmTMC7033_A14.TimedWebClient timedWebClient = new frmTMC7033_A14.TimedWebClient
                {
                    Timeout = 2000
                })
                {
                    this.WriteAndonToLogFile(text, "2");
                    string text2 = string.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", new object[]
                    {
                frmTMC7033_A14.ipAddress,
                frmTMC7033_A14.Comname,
                frmTMC7033_A14.RecievedIpaddress,
                text,
                "2"
                    });
                    timedWebClient.DownloadString(text2);
                }
            }
            catch (Exception)
            {
            }
        }
        private void btn_reasonCode1_Click(object sender, EventArgs e)
        {
            string text = "on";
            try
            {
                if (this.btn_reasonCode1.Text.ToString().Contains("Waiting"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("R");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode1.Text = "(Andon) Gọi QA ";
                        this.timer_BlinkButtonRed.Enabled = false;
                        this.btn_reasonCode1.BackColor = Color.DarkRed;
                        
                    });
                }
                else if (this.btn_reasonCode1.Text.ToString().Contains("Calling"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("SOUND");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode1.Text = "(Andon) Gọi QA " + Environment.NewLine + "Waiting";
                        this.timer_BlinkButtonRed.Enabled = false;
                        this.btn_reasonCode1.BackColor = Color.DarkRed;
                        
                    });
                }
                else
                {
                    Task task = this.TurnOnAndonAsync("R");
                    this.TurnOnAndonAsync("SOUND");
                    if (task.IsCompleted)
                    {
                        this.btn_reasonCode1.Text = "(Andon) Gọi QA " + Environment.NewLine + "Calling";
                        this.timer_BlinkButtonRed.Enabled = true;
                    }
                }
                using (frmTMC7033_A14.TimedWebClient timedWebClient = new frmTMC7033_A14.TimedWebClient
                {
                    Timeout = 2000
                })
                {
                    this.WriteAndonToLogFile(text, "3");
                    string text2 = string.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", new object[]
                    {
                frmTMC7033_A14.ipAddress,
                frmTMC7033_A14.Comname,
                frmTMC7033_A14.RecievedIpaddress,
                text,
                "3"
                    });
                    timedWebClient.DownloadString(text2);
                }
            }
            catch (Exception)
            {
            }
        }
        private async Task<bool> TurnOnAndonAsync(string lightColor)
        {
            bool flag = false;
            if (this.MQTTConnected)
            {
                if (lightColor == "R")
                {
                    flag = await MQTT.Main.PublishAsync("R_ON", true, 1);
                }
                if (lightColor == "Y")
                {
                    flag = await MQTT.Main.PublishAsync("Y_ON", true, 1);
                }
                if (lightColor == "G")
                {
                    flag = await MQTT.Main.PublishAsync("G_ON", true, 1);
                }
                if (lightColor == "SOUND")
                {
                    flag = await MQTT.Main.PublishAsync("SOUND_ON", true, 1);
                }
            }
            return flag;
        }

        private async Task<bool> TurnOffAndonAsync(string lightColor)
        {
            bool flag = false;
            if (this.MQTTConnected)
            {
                if (lightColor == "R")
                {
                    flag = await MQTT.Main.PublishAsync("R_OFF", true, 1);
                }
                if (lightColor == "Y")
                {
                    flag = await MQTT.Main.PublishAsync("Y_OFF", true, 1);
                }
                if (lightColor == "G")
                {
                    flag = await MQTT.Main.PublishAsync("G_OFF", true, 1);
                }
                if (lightColor == "SOUND")
                {
                    flag = await MQTT.Main.PublishAsync("SOUND_OFF", true, 1);
                }
            }
            return flag;
        }
        private async Task<bool> TurnOnStopLineAsync()
        {
            bool result = false;

            if (MQTTConnected)
            {
                result = await MQTT.Main.PublishAsync("STOP_ON", true, 1);
            }
            return result;
        }
        private async Task<bool> TurnOffStopLineAsync()
        {
            bool result = false;

            if (MQTTConnected)
            {
                result = await MQTT.Main.PublishAsync("STOP_OFF", true, 1);
            }
            return result;
        }
        private bool WriteAndonToLogFile(string onff, string errorcode)
        {
            try
            {
                string filename; string content = "";
                content = onff + " " + errorcode;
                filename = "Andon" + DateTime.Now.ToString("yyyyMMdd");
                etc.WriteToFile(content, "Andon", filename);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private void backgroundSyncData_DoWork(object sender, DoWorkEventArgs e)
        {
            backgroundSyncData.ReportProgress(10);
            SyncDataToServer();
            backgroundSyncData.ReportProgress(100);
        }
        private void backgroundSyncData_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10)
            {
                ShowMessage("Sync data to server...", Color.Blue);
            }
            else if (e.ProgressPercentage == 100)
            {
                ShowMessage("Sync data to server successfully", Color.Blue);
            }
            if (!CheckNetworkConnection())
            {
                ShowMessage("Rớt mạng rồi", Color.Red);
            }
            else
            {
                ShowMessage("THÔNG BÁO", Color.Blue);
            }
        }
        private bool CheckNetworkConnection()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT SYSDATE FROM DUAL ");
                DataTable dt = new DataTable();
                dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
        private bool SyncDataToServer()
        {
            string eol_Sequence = "";
            string P_d_gather = "", P_spDeptCode = "", P_LineName = "", P_ipAddress = "", P_C_STYLE = "", P_Inserted = "", P_Seq = "";
            // Check File Pass
            try
            {
                string[] PassArray = etc.ReadFromFileNotMesage("Pass", "PassBTS_" + DateTime.Now.ToString("yyyyMMdd"));
                if (PassArray != null)
                {
                    for (int i = 0; i < PassArray.Count(); i++)
                    {
                        string[] passParamenter = PassArray[i].ToString().Split(';');

                        if (passParamenter.Count() == 11)
                        {
                            P_d_gather = ""; P_spDeptCode = ""; P_LineName = ""; P_ipAddress = ""; P_C_STYLE = "";
                            P_d_gather = passParamenter[0];
                            P_spDeptCode = passParamenter[1];
                            P_LineName = passParamenter[2];
                            P_ipAddress = passParamenter[3];
                            P_C_STYLE = passParamenter[4];
                            P_Inserted = passParamenter[9];
                            P_Seq = passParamenter[10];
                            double a = new Random().Next(0, 60);
                            if (P_Inserted == "0")
                            {
                                StringBuilder strSql = new StringBuilder();
                                strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval, TO_CHAR(sysdate,'yyyyMMddHH24MISS') D_GATHER FROM DUAL");

                                DataTable dt = new DataTable();
                                dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    eol_Sequence = dt.Rows[0][0].ToString();
                                }
                                else
                                {
                                    return false;
                                }
                                if (P_C_STYLE != "")
                                {
                                    StringBuilder query = new StringBuilder();
                                    query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_PASS,SEQ)");
                                    query.AppendLine("VALUES( '" + P_d_gather + "'");
                                    query.AppendLine("       ,'" + P_spDeptCode + "'");
                                    query.AppendLine("       ,'" + P_LineName + "'");
                                    query.AppendLine("       ,'" + P_ipAddress + "'");
                                    query.AppendLine("       ,'" + P_C_STYLE + "'");
                                    query.AppendLine("       ,'0'");
                                    query.AppendLine("       ,'0'");
                                    query.AppendLine("       ,'" + eol_Sequence + "'");
                                    query.AppendLine("       ,'1','" + P_Seq + "'");
                                    query.AppendLine("      )");

                                    Console.WriteLine("Impact database " + query.ToString());
                                    if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                    {
                                        passParamenter[9] = "1";
                                        string combine = string.Join(";", passParamenter);
                                        WritePassToLogTempFile(combine);
                                    }
                                    else
                                    {
                                        WritePassToLogTempFile(PassArray[i].ToString());
                                    }
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                WritePassToLogTempFile(PassArray[i].ToString());
                            }
                        }
                    }
                    etc.ReplaceTempFile("Pass");
                    etc.DeleteTempFile("Pass");
                }
                string[] FailArray = etc.ReadFromFileNotMesage("Fail", "FailBTS_" + DateTime.Now.ToString("yyyyMMdd"));
                if (FailArray != null)
                {
                    for (int i = 0; i < FailArray.Count(); i++)
                    {
                        P_d_gather = ""; P_spDeptCode = ""; P_LineName = ""; P_ipAddress = ""; P_C_STYLE = ""; P_Inserted = ""; P_Seq = "";
                        string[] failParamenter = FailArray[i].ToString().Split(';');
                        if (failParamenter.Count() == 11)
                        {
                            P_d_gather = failParamenter[0];
                            P_spDeptCode = failParamenter[1];
                            P_LineName = failParamenter[2];
                            P_ipAddress = failParamenter[3];
                            P_C_STYLE = failParamenter[4];
                            partID = failParamenter[5];
                            reasonID = failParamenter[6];
                            Mes_Group_Sum = failParamenter[7];
                            P_Inserted = failParamenter[9];
                            P_Seq = failParamenter[10];
                            if (P_Inserted == "0")
                            {
                                StringBuilder strSql = new StringBuilder();

                                strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval, TO_CHAR(sysdate,'yyyyMMddHH24MISS') D_GATHER FROM DUAL");

                                DataTable dt = new DataTable();
                                dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    eol_Sequence = dt.Rows[0][0].ToString();
                                }
                                else
                                {
                                    return false;
                                }
                                StringBuilder query = new StringBuilder();
                                query.AppendLine("");
                                query.AppendLine("INSERT INTO MES.TRTB_M_BTS_COUNT3(D_GATHER, C_LINE,                                                                       ");
                                query.AppendLine(" PART_ID, REASON_ID, MES_GROUP_SUM, USER_ID, IP_ADDRESS)                                                                  ");
                                query.AppendLine("VALUES ('" + P_d_gather + "', '" + spLine + "',                                                                            ");
                                query.AppendLine(" '" + partID + "', '" + reasonID + "','" + Mes_Group_Sum + "','" + LeftOrRight + "','" + P_ipAddress + "')                ");

                                if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                {

                                    if (P_C_STYLE != "")
                                    {
                                        query = new StringBuilder();
                                        query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_FAIL,SEQ)              ");
                                        query.AppendLine(" VALUES( '" + P_d_gather + "'                                                                                                          ");
                                        query.AppendLine("        ,'" + P_spDeptCode + "'                                                                                                          ");
                                        query.AppendLine("        ,'" + P_LineName + "'                                                                                                            ");
                                        query.AppendLine("        ,'" + P_ipAddress + "'                                                                                                           ");
                                        query.AppendLine("        ,'" + P_C_STYLE + "'                                                                                                       ");
                                        query.AppendLine("        ,'" + partID + "'                                                                                                              ");
                                        query.AppendLine("        ,'" + reasonID + "'                                                                                                            ");
                                        query.AppendLine("        ,'" + eol_Sequence + "'                                                                                                        ");
                                        query.AppendLine("        ,'1','" + P_Seq + "'                                                                                                                           ");
                                        query.AppendLine("                                                                                                                                       ");
                                        query.AppendLine("       )                                                                                                                              ");
                                        if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                        {
                                            failParamenter[9] = "1";
                                            string combine = string.Join(";", failParamenter);
                                            WriteFailToLogTempFile(combine);
                                        }
                                        else
                                        {
                                            WriteFailToLogTempFile(FailArray[i].ToString());
                                        }
                                    }
                                }
                                else
                                {
                                    WriteFailToLogTempFile(FailArray[i].ToString());
                                }

                            }
                            else
                            {
                                WriteFailToLogTempFile(FailArray[i].ToString());
                            }

                        }
                    }
                    etc.ReplaceTempFile("Fail");
                    etc.DeleteTempFile("Fail");
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                etc.ReplaceTempFile("Fail");
                etc.DeleteTempFile("Fail");
                etc.ReplaceTempFile("Pass");
                etc.DeleteTempFile("Pass");
                return false;
            }
        }
        private bool WritePassToLogTempFile(string content)
        {
            try
            {
                string filename;
                filename = "PassBTS_" + DateTime.Now.ToString("yyyyMMdd") + "_temp";
                etc.WriteToFile(content, "Pass", filename);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private bool WriteFailToLogTempFile(string content)
        {
            try
            {
                string filename;
                filename = "FailBTS_" + DateTime.Now.ToString("yyyyMMdd") + "_temp";
                etc.WriteToFile(content, "Fail", filename);
                return true;
            }
            catch
            {
                return false;
            }


        }
        private void btn_reasonCode3_Click(object sender, EventArgs e)
        {
            string text = "on";
            try
            {
                if (this.btn_reasonCode3.Text.ToString().Contains("Waiting"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("G");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất";
                        this.timer_BlinkButtonGreen.Enabled = false;
                        this.btn_reasonCode3.BackColor = Color.DarkGreen;
                    });
                }
                else if (this.btn_reasonCode3.Text.ToString().Contains("Calling"))
                {
                    text = "off";
                    this.TurnOffAndonAsync("SOUND");
                    this.ThreadSafe(delegate
                    {
                        this.btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Waiting";
                        this.timer_BlinkButtonGreen.Enabled = false;
                        this.btn_reasonCode3.BackColor = Color.DarkGreen;
                    });
                }
                else
                {
                    Task task = this.TurnOnAndonAsync("G");
                    this.TurnOnAndonAsync("SOUND");
                    if (task.IsCompleted)
                    {
                        this.btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Calling";
                        this.timer_BlinkButtonGreen.Enabled = true;
                    }
                }
                using (frmTMC7033_A14.TimedWebClient timedWebClient = new frmTMC7033_A14.TimedWebClient
                {
                    Timeout = 2000
                })
                {
                    string text2 = string.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", new object[]
                    {
                frmTMC7033_A14.ipAddress,
                frmTMC7033_A14.Comname,
                frmTMC7033_A14.RecievedIpaddress,
                text,
                "1"
                    });
                    timedWebClient.DownloadString(text2);
                }
            }
            catch (Exception)
            {
            }
        }


        //==========================================================================================================//
        //14/4/26


        private void btnSPCCleanliness_Click(object sender, EventArgs e)
        {
            EOL.Popup.SPCClean spcclean = new Popup.SPCClean();


            StringBuilder qry = new StringBuilder();
            qry.AppendLine(" SELECT * FROM V_PCHART");
            var _dt = crud.dac.DtSelectExcuteWithQuery(qry.ToString());

            if (_dt == null || _dt.Rows.Count == 0)
            {
                ShowMessage("Không có data SPC Clenliness", Color.Red);
                return;
            }
            else
            {
                spcclean.dtSPC = _dt;
                spcclean.Text = "SPC CLEANLINESS";
                spcclean.ShowDialog();
            }

        }
        private void btnSPCStitching_Click(object sender, EventArgs e)
        {
            EOL.Popup.SPCClean spcclean = new Popup.SPCClean();


            StringBuilder qry = new StringBuilder();
            qry.AppendLine(" SELECT * FROM V_PCHART_STT");
            var _dt = crud.dac.DtSelectExcuteWithQuery(qry.ToString());

            if (_dt == null || _dt.Rows.Count == 0)
            {
                ShowMessage("Không có data SPC Clenliness", Color.Red);
                return;
            }
            else
            {
                spcclean.dtSPC = _dt;
                spcclean.Text = "SPC STITCHING";
                spcclean.ShowDialog();
            }
        }
        private void btnSPCBonding_Click(object sender, EventArgs e)
        {
            EOL.Popup.SPCClean spcclean = new Popup.SPCClean();


            StringBuilder qry = new StringBuilder();
            qry.AppendLine(" SELECT * FROM V_PCHART_BONDING");
            var _dt = crud.dac.DtSelectExcuteWithQuery(qry.ToString());

            if (_dt == null || _dt.Rows.Count == 0)
            {
                ShowMessage("Không có data SPC Clenliness", Color.Red);
                return;
            }
            else
            {
                spcclean.dtSPC = _dt;
                spcclean.Text = "SPC BONDING";
                spcclean.ShowDialog();
            }
        }
        private void backgroundWorkerCheckAndon_DoWork(object sender, DoWorkEventArgs e)
        {
            stSensorCount = SensorCount();
            GetJSIData();
        }
        private string SensorCount()
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT TTL_COUNT FROM TRTB_M_SENSOR_COUNT2_TTL WHERE C_LINE = 'ASS' || '" + spLine + "' AND DATESTR = TO_CHAR(SYSDATE,'YYYYMMDD')  ");
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["TTL_COUNT"].ToString();
            }
            else
            {
                return "0";
            }

        }
        private DataTable GetJSIData()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("            SELECT                                                                      ");
            query.AppendLine("D_GATHER, C_LINE, DESCRIPTION,                                                          ");
            query.AppendLine("   D_PROGRESS, D_FINISH, IP_ADDRESS                                                     ");
            query.AppendLine("FROM MES.TRTB_M_ANDON_LOG WHERE C_LINE = '" + LineName + "'                                         ");
            query.AppendLine("AND D_GATHER > TO_CHAR(SYSDATE - INTERVAL '20' MINUTE,'YYYYMMDDHH24MISS')               ");
            query.AppendLine("AND D_FINISH IS NULL        ORDER BY 1                                                  ");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            dtJSI = dt;
            return null;
        }
        private void backgroundWorkerCheckAndon_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblSensorCount.Text = stSensorCount;
            if (dtJSI != null && dtJSI.Rows.Count > 0)
            {
                string message = dtJSI.Rows[0]["DESCRIPTION"].ToString();
                alarmGather = dtJSI.Rows[0]["D_GATHER"].ToString();
                string filePath = Application.StartupPath + @"\JSISound.mp3";

                try
                {
                    //mediaPlayer.Open(new Uri(filePath));
                    //mediaPlayer.Play();
                }
                catch (Exception ex)
                {

                }
                Popup.MessageAlarmPopUp messageShow = new Popup.MessageAlarmPopUp();
                messageShow.MessageText = message;
                messageShow.ShowDialog(this);
                if (UpdateAlarm(alarmGather))
                {

                }
            }
        }
        private bool UpdateAlarm(string DGather)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("UPDATE MES.TRTB_M_ANDON_LOG SET D_FINISH = TO_CHAR(SYSDATE,'HH24:MI:SS') WHERE ");
            query.AppendLine("D_GATHER <= '" + DGather + "' AND C_LINE = '" + LineName + "' AND D_FINISH IS NULL ");

            if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        //private void timer_BlinkButtonRed_Tick(object sender, EventArgs e)
        //{
        //        if (btn_reasonCode1.Appearance.BackColor2 == System.Drawing.Color.DarkRed)
        //    {
        //        this.btn_reasonCode1.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode1.Appearance.BackColor = System.Drawing.Color.IndianRed;
        //            btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.IndianRed;
        //        }));

        //    }
        //    else
        //    {
        //        this.btn_reasonCode1.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode1.Appearance.BackColor = System.Drawing.Color.DarkRed;
        //            btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.DarkRed;
        //        }));

        //    }
        //}
        //private void timer_BlinkButtonYellow_Tick(object sender, EventArgs e)
        //{
        //    if (btn_reasonCode2.Appearance.BackColor2 == System.Drawing.Color.Orange)
        //    {
        //        this.btn_reasonCode2.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode2.Appearance.BackColor = System.Drawing.Color.OrangeRed;
        //            btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.OrangeRed;
        //        }));

        //    }
        //    else
        //    {
        //        this.btn_reasonCode2.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode2.Appearance.BackColor = System.Drawing.Color.Orange;
        //            btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.Orange;
        //        }));

        //    }
        //}
        //private void timer_BlinkButtonGreen_Tick(object sender, EventArgs e)
        //{
        //        if (btn_reasonCode3.Appearance.BackColor2 == System.Drawing.Color.ForestGreen)
        //    {
        //        this.btn_reasonCode3.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode3.Appearance.BackColor = System.Drawing.Color.LightGreen;
        //            btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.LightGreen;
        //        }));

        //    }
        //    else
        //    {
        //        this.btn_reasonCode3.Invoke(new Action(() =>
        //        {
        //            btn_reasonCode3.Appearance.BackColor = System.Drawing.Color.ForestGreen;
        //            btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.ForestGreen;
        //        }));

        //    }
        //}
        private void timer_BlinkButtonRed_Tick(object sender, EventArgs e)
        {
            btn_reasonCode1.UseVisualStyleBackColor = false;
            if (btn_reasonCode1.BackColor == System.Drawing.Color.DarkRed)
            {
                btn_reasonCode1.BackColor = System.Drawing.Color.IndianRed;
                btn_reasonCode1.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                btn_reasonCode1.BackColor = System.Drawing.Color.DarkRed;
                btn_reasonCode1.ForeColor = System.Drawing.Color.White;
            }
        }

        private void timer_BlinkButtonYellow_Tick(object sender, EventArgs e)
        {
            btn_reasonCode2.UseVisualStyleBackColor = false;
            if (btn_reasonCode2.BackColor == System.Drawing.Color.Orange)
            {
                btn_reasonCode2.BackColor = System.Drawing.Color.OrangeRed;
                btn_reasonCode2.ForeColor = System.Drawing.Color.White;
            }
            else
            {
                btn_reasonCode2.BackColor = System.Drawing.Color.Orange;
                btn_reasonCode2.ForeColor = System.Drawing.Color.White;
            }
        }

        private void timer_BlinkButtonGreen_Tick(object sender, EventArgs e)
        {
            btn_reasonCode3.UseVisualStyleBackColor = false;
            if (btn_reasonCode3.BackColor == System.Drawing.Color.ForestGreen)
            {
                btn_reasonCode3.BackColor = System.Drawing.Color.LightGreen;
                btn_reasonCode3.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                btn_reasonCode3.BackColor = System.Drawing.Color.ForestGreen;
                btn_reasonCode3.ForeColor = System.Drawing.Color.White;
            }
        }




        private void timer_SyncData_Tick(object sender, EventArgs e)
        {
            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }
        private void backgroundWorkerStopLine_DoWork(object sender, DoWorkEventArgs e)
        {
            checkAlarmReturn();
            GetDataStopLine();
        }
        private DataTable checkAlarmReturn()
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();

            if (ipAddress == "192.168.1.158")
            {
                spLine = "NSA1";
                LineName = "P101";
            }
            query.AppendLine("");
            query.AppendLine("  SELECT A.REASON_ID,                                                                                              ");
            query.AppendLine("         REASON_EN,                                                                                                ");
            query.AppendLine("         REASON_VN,");
            query.AppendLine("         COUNT (A.REASON_ID)CNT                                                                                    ");
            query.AppendLine("    FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B                                                     ");
            query.AppendLine("   WHERE     A.REASON_ID = B.REASON_ID                                                                             ");
            query.AppendLine("         AND SUBSTR (A.D_GATHER, 1, 8) = TO_CHAR (SYSDATE, 'YYYYMMDD')                                             ");
            query.AppendLine("         AND A.C_LINE IN ( '" + spLine + "','" + LineName + "' )                                                   ");
            query.AppendLine("         AND A.D_GATHER <= TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS')                                                   ");
            query.AppendLine("         AND A.D_GATHER > TO_CHAR (SYSDATE - (0.5 / 24), 'YYYYMMDDHH24MISS')                                         ");
            query.AppendLine(" AND (A.D_GATHER,A.C_LINE,A.REASON_ID) NOT IN (  SELECT D_GATHER,C_LINE,REASON_ID FROM TRTB_BTS_LOG_REASON_HISTORY ");
            query.AppendLine(" WHERE D_GATHER > TO_CHAR (SYSDATE, 'YYYYMMDD') || '000000')                                                       ");
            query.AppendLine("GROUP BY A.REASON_ID, REASON_EN, REASON_VN                                                                         ");

            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            dtAlarmReturn = dt;
            return dtAlarmReturn;
        }
        private void ConffigErrorButtonColor(string id, Color c)
        {
            foreach (Control p in tableLayoutPanel2.Controls)
            {
                if (p is Panel panel)
                { 
                    foreach (Control a in panel.Controls)
                    {
                        if (a is Button btnID)
                        {
                            if (btnID.AccessibleName == id)
                            {
                                btnID.UseVisualStyleBackColor = false;
                                btnID.BackColor = c;
                                btnID.ForeColor = GetReadableTextColor(c);
                            }
                        }
                    }
                }
            }
        }

        private Color GetReadableTextColor(Color backgroundColor)
        {
            double brightness = (backgroundColor.R * 0.299) +
                                (backgroundColor.G * 0.587) +
                                (backgroundColor.B * 0.114);

            return brightness >= 186 ? Color.Black : Color.White;
        }
        private void backgroundWorkerStopLine_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {

            }
            else
            {
                if (dtAlarmReturn != null && dtAlarmReturn.Rows.Count > 0)
                {
                    for (int i = 0; i < dtAlarmReturn.Rows.Count; i++)
                    {
                        if (Convert.ToInt32(dtAlarmReturn.Rows[i]["CNT"]) == 4)
                        {
                            ConffigErrorButtonColor(dtAlarmReturn.Rows[i]["REASON_ID"].ToString(), System.Drawing.Color.Yellow);
                        }
                        if (Convert.ToInt32(dtAlarmReturn.Rows[i]["CNT"]) == 5)
                        {
                            ConffigErrorButtonColor(dtAlarmReturn.Rows[i]["REASON_ID"].ToString(), System.Drawing.Color.Red);
                        }
                    }
                }
                if (dtStopLine != null && dtStopLine.Rows.Count > 0)
                {
                    if (lblLineInfo.Text == "P114")
                    {
                        for (int i = 0; i < dtStopLine.Rows.Count; i++)
                        {
                            int count = 0;
                            int.TryParse(dtStopLine.Rows[i][0].ToString(), out count);
                            if (count >= 20)
                            {
                                TurnOnStopLine("", "", "", "");
                            }

                            //TurnOnStopLine(dtStopLine.Rows[i]["PART_ID"].ToString(), dtStopLine.Rows[i]["PART_EN"].ToString(), dtStopLine.Rows[i]["REASON_ID"].ToString(),dtStopLine.Rows[i]["REASON_EN"].ToString());
                        }
                    }
                    else
                    {
                        TurnOnStopLine(dtStopLine.Rows[0]["PART_ID"].ToString(), dtStopLine.Rows[0]["PART_EN"].ToString(), dtStopLine.Rows[0]["REASON_ID"].ToString(), dtStopLine.Rows[0]["REASON_EN"].ToString());
                    }
                }
            }
        }
        private void TurnOnStopLine(string partname, string partid, string reasonid, string reason)
        {

            //processStopLine( partname,  partid,  reasonid,  reason);
            if (lblLineInfo.Text == "P114")
            {
                processStopLine("Sensor Stop Line", "", "1", "Sensor Stop Line Over 20 Pairs per hour");
            }
            else
            {
                processStopLine(partname, partid, reasonid, reason);
            }

        }
        private bool InsertIntoLogHistory(string reasonID)
        {
            StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery("SELECT MAX(SEQ) + 1 CNT FROM MES.TRTB_BTS_LOG_REASON_HISTORY ");

            if (dt.Rows.Count > 0)
            {
                if (ipAddress == "192.168.1.158")
                {
                    spLine = "NSA1";
                    LineName = "P101";
                }

                int seq = Convert.ToInt32(dt.Rows[0]["CNT"].ToString());

                query = new StringBuilder();
                query.AppendLine("                INSERT INTO MES.TRTB_BTS_LOG_REASON_HISTORY (                                  ");
                query.AppendLine("   SEQ, D_GATHER, C_LINE,                                                                      ");
                query.AppendLine("   LOCK_TIME, IS_LOCK,                                                                         ");
                query.AppendLine("   REASON_ID)                                                                                  ");
                query.AppendLine("SELECT " + seq + ",a.D_GATHER,'" + spLine + "',SYSDATE,'Y',A.REASON_ID                         ");
                query.AppendLine("    FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B                                 ");
                query.AppendLine("   WHERE     A.REASON_ID = B.REASON_ID                                                         ");
                query.AppendLine("         AND SUBSTR (A.D_GATHER, 1, 8) = TO_CHAR (SYSDATE, 'YYYYMMDD')                         ");
                query.AppendLine("         AND A.C_LINE IN ( '" + spLine + "','" + LineName + "' )                               ");
                query.AppendLine("         AND A.D_GATHER <= TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS')                               ");
                query.AppendLine("         AND A.D_GATHER > TO_CHAR (SYSDATE - (0.5 / 24), 'YYYYMMDDHH24MISS')                   ");
                query.AppendLine("         AND A.REASON_ID = " + reasonID + " AND DEPT_CODE = 'ASS'                              ");

                crud.dac.IUExcuteWithQuery(query.ToString());
                query = new StringBuilder();


                query.AppendLine(" insert into MES.TRTB_ANDON_SYSTEM_LOG(D_GATHER, IP_ADDRESS, LINE_NM,");
                query.AppendLine(" IP_ADDRESS_CALL, TIME_START_CALL, ");
                query.AppendLine(" REASON_CODE, INPUT_DESCRIPTION, MSG_STATUS) values( ");
                query.AppendLine(" to_char(sysdate, 'YYYYMMDDHH24MISS'), '" + ipAddress + "', 'ASS' || '" + spLine + "', ");
                query.AppendLine("    '" + ipAddress + "', SYSDATE, ");
                query.AppendLine("    5, '', 'NEW') ");

                crud.dac.IUExcuteWithQuery(query.ToString());

                //Console.WriteLine($"TRTB_BTS_LOG_REASON_HISTORY: SEQ: {seq}, reasonID: {reasonID}, spLine: {spLine}");
                //Console.WriteLine($"TRTB_ANDON_SYSTEM_LOG: ipAddress: {ipAddress}, RecievedIpaddress: {RecievedIpaddress}, spLine: {spLine}");

                return true;
            }
            return false;
        }
        private void SetAlarm(string header, string message, int longtime, string reasonID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("SELECT * FROM                                                                ");
            query.AppendLine("TRTB_M_BTS_STOPLINE_NOTIFY                                                   ");
            query.AppendLine("WHERE C_DATE = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')                       ");
            query.AppendLine("AND C_LINE = '" + lblLineInfo.Text + "'                                      ");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            if (dt.Rows.Count > 0)
            {

            }
            else
            {
                query = new StringBuilder();

                query.AppendLine("INSERT INTO MES.TRTB_M_BTS_STOPLINE_NOTIFY (                                                      ");
                query.AppendLine("   C_LINE, C_DATE,IP_ADDRESS, RFT,                                                                ");
                query.AppendLine("   IS_RESOLVE, IS_NOTIFICATION, C_MESSAGE_TITLE,                                                  ");
                query.AppendLine("   C_MESSAGE_CONTENT, OBJECT_CD, TIME_ELAPSED                                                     ");
                query.AppendLine("   )                                                                                              ");
                query.AppendLine("VALUES ( '" + lblLineInfo.Text + "',                                                              ");
                query.AppendLine(" TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),                                                             ");
                query.AppendLine(" '" + ipAddress + "','',                                                                          ");
                query.AppendLine(" 'N',                                                                                             ");
                query.AppendLine(" 'N',                                                                                             ");
                query.AppendLine(" '" + header + "',                                                                                ");
                query.AppendLine(" '" + message + "',                                                                               ");
                query.AppendLine(" '192.168.4.226',                                                                                 ");
                query.AppendLine(" " + longtime + ")                                                                                ");
                crud.dac.IUExcuteWithQuery(query.ToString());
            }

        }
        private async void processStopLine(string partname, string partid, string reasonid, string reason)
        {
            await Task.Run(() =>
            {
                if (InsertIntoLogHistory(reasonid))
                {
                    //SetAlarm(lblLineInfo.Text + " Stop line", "Part : " + partname + " Defect " + reason + " now over 6 times. Please check it", 10, reason);
                    SetAlarm(lblLineInfo.Text + " Stop line ", " Reason : " + reason + ". Please check it", 10, reason);
                    var resultStoplineOn = TurnOnStopLineAsync();
                    if (resultStoplineOn.IsCompleted)
                    {
                        var resultAndonOn = TurnOnAndonAsync("R");
                        var resultAndonSound = TurnOnAndonAsync("SOUND");
                    }

                    ThreadSafe(() =>
                    {
                        using (frmTMC7032_MsgAlarm alarm = new frmTMC7032_MsgAlarm())
                        {
                            alarm.IPADDRESS = ipAddress;
                            alarm.IsStopLine = true;
                            alarm.MessageText = lblLineInfo.Text + " Stop line " + " Reason : " + reason + ". Please check it";
                            alarm.ShowDialog(this);
                        }
                    });

                    isstartcountreason = false;
                    var resultStoplineOff = TurnOffStopLineAsync();
                    if (resultStoplineOff.IsCompleted)
                    {
                        var resultAndonOff = TurnOffAndonAsync("R");
                        var resultAndonSoundOff = TurnOffAndonAsync("SOUND");
                    }
                }
            });
        }
        private DataTable GetDataStopLine()
        {
            dtStopLine = new DataTable();
            StringBuilder query = new StringBuilder();

            // 20241220 ĐÓNG LẠI 
            if (lblLineInfo.Text == "P114")
            {
                query.AppendLine("            SELECT                                                                   ");
                query.AppendLine(" COUNT(*)  CNT                                                                       ");
                query.AppendLine("FROM MES.TRTB_M_SENSOR_COUNT2                                                        ");
                query.AppendLine("where d_gather BETWEEN TO_CHAR(SYSDATE - 1 / 24, 'YYYYMMDDHH24MISS')                 ");
                query.AppendLine("AND TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')                                              ");
                query.AppendLine("AND IS_SOLVE = 'N'                                                                   ");
            }
            else
            {
                query.AppendLine("");
                query.AppendLine("            SELECT* FROM(                                                                                                                ");
                query.AppendLine(" SELECT A.PART_ID,                                                                                                                       ");
                query.AppendLine("       C.PART_EN,                                                                                                                        ");
                query.AppendLine("       A.REASON_ID,                                                                                                                      ");
                query.AppendLine("        REASON_EN,                                                                                                                       ");
                query.AppendLine("        REASON_VN,                                                                                                                       ");
                query.AppendLine("        COUNT (A.REASON_ID) CNT                                                                                                          ");
                query.AppendLine("                                                                                                                                         ");
                query.AppendLine("   FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3 @inf_m_e B ,                                                                         ");
                query.AppendLine("   MES.TRTB_M_BTS_PART @INF_M_E C                                                                                                        ");
                query.AppendLine("  WHERE     A.REASON_ID = B.REASON_ID                                                                                                    ");
                query.AppendLine("  AND A.PART_ID = C.PART_ID                                                                                                              ");
                query.AppendLine("        AND SUBSTR (A.D_GATHER, 1, 8) = TO_CHAR(SYSDATE, 'YYYYMMDD')                                                                     ");
                query.AppendLine("         AND C.DEPT_CODE = 'ASS'                                                                                                         ");
                query.AppendLine("         AND A.C_LINE IN ( '" + spLine + "','" + LineName + "' )                                                                         ");
                query.AppendLine("         AND A.D_GATHER <= TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS')                                                                          ");
                query.AppendLine("         AND A.D_GATHER > TO_CHAR(SYSDATE - (1 / 24), 'YYYYMMDDHH24MISS')                                                                ");
                query.AppendLine(" AND(A.D_GATHER, A.C_LINE, A.REASON_ID) NOT IN(SELECT D_GATHER, C_LINE, REASON_ID FROM TRTB_BTS_LOG_REASON_HISTORY                       ");
                query.AppendLine("  WHERE D_GATHER > TO_CHAR(SYSDATE, 'YYYYMMDD') || '000000' AND UNLOCK_EMPCD IS NULL)                                                    ");
                query.AppendLine("GROUP BY A.REASON_ID, REASON_EN, REASON_VN,A.PART_ID  ,C.PART_EN ) WHERE CNT >= 6                                                        ");

            }
            //query.AppendLine("");
            //query.AppendLine("            SELECT* FROM(                                                                                                                ");
            //query.AppendLine(" SELECT A.PART_ID,                                                                                                                       ");
            //query.AppendLine("       C.PART_EN,                                                                                                                        ");
            //query.AppendLine("       A.REASON_ID,                                                                                                                      ");
            //query.AppendLine("        REASON_EN,                                                                                                                       ");
            //query.AppendLine("        REASON_VN,                                                                                                                       ");
            //query.AppendLine("        COUNT (A.REASON_ID) CNT                                                                                                          ");
            //query.AppendLine("                                                                                                                                         ");
            //query.AppendLine("   FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3 @inf_m_e B ,                                                                         ");
            //query.AppendLine("   MES.TRTB_M_BTS_PART @INF_M_E C                                                                                                        ");
            //query.AppendLine("  WHERE     A.REASON_ID = B.REASON_ID                                                                                                    ");
            //query.AppendLine("  AND A.PART_ID = C.PART_ID                                                                                                              ");
            //query.AppendLine("        AND SUBSTR (A.D_GATHER, 1, 8) = TO_CHAR(SYSDATE, 'YYYYMMDD')                                                                     ");
            //query.AppendLine("         AND C.DEPT_CODE = 'ASS'                                                                                                         ");
            //query.AppendLine("         AND A.C_LINE IN ( '" + spLine + "','" + LineName + "' )                                                                         ");
            //query.AppendLine("         AND A.D_GATHER <= TO_CHAR(SYSDATE, 'YYYYMMDDHH24MISS')                                                                          ");
            //query.AppendLine("         AND A.D_GATHER > TO_CHAR(SYSDATE - (1 / 24), 'YYYYMMDDHH24MISS')                                                                ");
            //query.AppendLine(" AND(A.D_GATHER, A.C_LINE, A.REASON_ID) NOT IN(SELECT D_GATHER, C_LINE, REASON_ID FROM TRTB_BTS_LOG_REASON_HISTORY                       ");
            //query.AppendLine("  WHERE D_GATHER > TO_CHAR(SYSDATE, 'YYYYMMDD') || '000000' AND UNLOCK_EMPCD IS NULL)                                                    ");  
            //query.AppendLine("GROUP BY A.REASON_ID, REASON_EN, REASON_VN,A.PART_ID  ,C.PART_EN ) WHERE CNT >= 6                                                        ");

            dtStopLine = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            return dtStopLine;
        }
        private void timer_CheckStopLine_Tick(object sender, EventArgs e)
        {

            if (!backgroundWorkerStopLine.IsBusy)
            {
                backgroundWorkerStopLine.RunWorkerAsync();
            }
        }
        private void timerCheckAndon_Tick(object sender, EventArgs e)
        {
            txtTime.Text = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

            if (!backgroundWorkerCheckAndon.IsBusy)
            {
                backgroundWorkerCheckAndon.RunWorkerAsync();
            }
        }

        private void lblRFT_Click(object sender, EventArgs e)
        {

        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }

        private void lblLineInfo_Click(object sender, EventArgs e)
        {
            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }
















        //private void txtTime_Click(object sender, EventArgs e)
        //{

        //}

        //private void lblFailTotal_Click(object sender, EventArgs e)
        //{

        //}

        //private void label1_Click(object sender, EventArgs e)
        //{

        //}

        //private void txtTime_Click_1(object sender, EventArgs e)
        //{

        //}

        //private void lblPart1_Click(object sender, EventArgs e)
        //{

        //}


        //private void simpleButton23_Click(object sender, EventArgs e) { }

        //private void btnRePass_Click(object sender, EventArgs e) { }
        //private void btnPass_Click(object sender, EventArgs e) { }
        //private void btnFail_Click(object sender, EventArgs e) { }
        //private void btnReFail_Click(object sender, EventArgs e) { }
        //private void btnClear_Click(object sender, EventArgs e) { }

        //private void btn_reasonCode1_Click(object sender, EventArgs e) { }
        //private void btn_reasonCode2_Click(object sender, EventArgs e)
        //{ }
        //private void btn_reasonCode3_Click(object sender, EventArgs e)
        //{ }

        //private void btnSPCCleanliness_Click(object sender, EventArgs e) { }
        //private void btnSPCStitching_Click(object sender, EventArgs e) { }
        //private void btnSPCBonding_Click(object sender, EventArgs e) { }
        //private void labelControl2_Click(object sender, EventArgs e)
        //{

        //}

        //}

        private void lbl1stPass_Click(object sender, EventArgs e)
        {

        }
    }
}
