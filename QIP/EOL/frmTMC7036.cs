using ConnectionClass.Oracle;
using GlobalFunction;
using Microsoft.VisualBasic.Logging;
using NETTMC.VoiceRecognition;
using QIP.EOL.Popup;

//using DocumentFormat.OpenXml.Office2010.CustomUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
//using DevExpress.XtraEditors;
using System.IO.Ports;
using System.Linq;
using System.Net;
//using DevExpress.XtraEditors.Camera;
using System.Net;
//using DevExpress.XtraEditors.Camera;
using System.Net;
//using DevExpress.XtraEditors.Camera;
using System.Net;
using System.Net;
using System.Net.Http;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoiceTest;
using VoiceWhisperApp2;
using static GlobalFunction.PublicFunction;
using static QIP.EOL.frmTMC7032;
using Label = System.Windows.Forms.Label;

namespace QIP.EOL
{
    public partial class frmTMC7036 : UserControl
    {
        public bool isPopUpAlarm;
        SYSTEMTIME st = new SYSTEMTIME();
        private CRUDOracle crud;
        private bool _bStartedRecording = false;
        private static string ScanIP;
        private static string LineName;
        private static string C_Line;
        //private static string spLine;
        public static string ipAddress;
        private DataTable dtStopLine = new DataTable();
        private DataTable dtAlarmReturn = new DataTable();
        public static string spDeptCode;
        private Point MouseDownLocation;
        bool isstartcountreason;
        private int sensorCount;
        private int finishedCount;
        private string finishedCountScan;
        private bool MoveControl;
        private int countSensor;
        private int countSensorQC;
        private string datarecive;
        private int TotalDefect;
        private int MetalCount;
        private static string partID;
        private static string reasonID;
        private static string LeftOrRight;
        private static string DefectNameCapture;
        private static string DefectIDCapture;
        public static int ProdHour;
        public DataTable productionData;
        public static int SENSORCOUNT;
        public static DataTable errorTouch;
        public int countDefectContinuos;
        public float tongProd = 0;
        private static string Ipaddress;
        private static string RecievedIpaddress;
        private static string Comname;
        public static string c_group;
        public bool MQTTConnected = false;
        public string MQTTClient = "";
        GlobalFunction.PublicFunction etc = new GlobalFunction.PublicFunction();

        private VoiceEngine _voiceEngine;
        private bool _isRecordingVoice = false;
        private CancellationTokenSource _autoLoopCts;

        // ── Voice log (A36) ───────────────────────────────────
        private static readonly string VoiceLogDir = EolCommonHelper.GetVoiceLogDir();
        private System.IO.StreamWriter _vlog;
        private string _vlogPath;
        private int _vlogSeq;
        private readonly object _vlogLock = new object();
        // ───────────────────────────────────────────────────

        public VoiceActionSupport SupportedActions => VoiceActionSupport.FullEOL;

        private VoiceRecognitionService _voice;

        private DateTime _lastVoiceTime = DateTime.MinValue;
        private Button _micButton = null!;
        private Label _resultLabel = null!;
        //private VoiceRecognitionService _voice;
        //private VoiceCommandParser _parser;

        Dictionary<string, string> Reason = new Dictionary<string, string>();
        List<string> linesensor = new List<string>();
        List<string> lineqc = new List<string>();
        [DllImport("uio.dll")]
        private static extern int usb_io_init(int pID);
        [DllImport("uio.dll")]
        private static extern void set_usb_events(int hWnd);
        [DllImport("uio.dll")]
        private static extern void get_usb_input(int lParam, ref USB_INPUT uInput);
        [DllImport("uio.dll")]
        private static extern bool usb_io_output(int pID, int cmd, int io1, int io2, int io3, int io4);
        [DllImport("uio.dll")]
        private static extern bool usb_io_reset(int pID);
        [DllImport("uio.dll")]
        private static extern bool usb_in_request(int pID);
        public struct USB_INPUT     // UIO ÀÔ·Â ÆÐÅ¶À¸·ÎºÎÅÍ µ¥ÀÌÅ¸¸¦ ¾ò±â À§ÇÑ ±¸Á¶Ã¼ 
        {
            public int ProductID;   // ÀåÄ¡ ID 
            public Byte Status;     // ÆÐÅ¶ ¼ö½Å »óÅÂ°ª  0=ÀÔ·Â º¯È­¿¡ ÀÇÇÑ ¼ö½Å, 1=µ¥ÀÌÅ¸ ÀçÀü¼Û ¿ä±¸¿¡ ÀÇÇÑ ¼ö½Å 
            public Byte Button;     // ÀÔ·Â ¹öÅÏ°ª
            public Byte Output;     // USB ÀåÄ¡ÀÇ ÀÔÃâ·Â »óÅÂ°ª
            public Byte Mask;       // Æ÷Æ®ÀÇ ÀÔÃâ·Â ¼³Á¤°ª. bit°ªÀÌ '0'ÀÌ¸é Ãâ·Â, '1'ÀÌ¸é ÀÔ·Â
        };
        //private void turnRedAndSound(bool onOff)
        //{
        //    int selectID;
        //    int blink;
        //    bool result;
        //    selectID = 0;
        //    if (onOff)
        //    {
        //        selectID = Convert.ToInt32("261", 16);
        //        blink = Convert.ToInt32("5", 10) * 16;
        //        blink += Convert.ToInt32("5", 10);
        //        string.Format("usb_io_output(0x{0:x},0x{1:x},2,0,0,0)", selectID, blink);
        //        result = usb_io_output(selectID, blink, 2, 0, 0, 0);
        //
        //
        //        selectID = Convert.ToInt32("261", 16);
        //        blink = Convert.ToInt32("5", 10) * 16;
        //        blink += Convert.ToInt32("5", 10);
        //        string.Format("usb_io_output(0x{0:x},0x{1:x},1,0,0,0)", selectID, blink);
        //        result = usb_io_output(selectID, blink, 1, 0, 0, 0);
        //        if (!result)
        //        {
        //            set_usb_events(this.Handle.ToInt32());
        //            turnRedAndSound(true);
        //        }
        //    }
        //    else
        //    {
        //        selectID = Convert.ToInt32("261", 16);
        //        string.Format("usb_io_output(0x{0:x},0,-1,0,0,0)", selectID);
        //        result = usb_io_output(selectID, 0, -1, 0, 0, 0);
        //
        //        Convert.ToInt32("261", 16);
        //        string.Format("usb_io_output(0x{0:x},0,-2,0,0,0)", selectID);
        //        result = usb_io_output(selectID, 0, -2, 0, 0, 0);
        //        if (!result)
        //        {
        //            set_usb_events(this.Handle.ToInt32());
        //            turnRedAndSound(false);
        //        }
        //    }
        //
        //}
        public frmTMC7036()
        {
            InitializeComponent();
            //InitializeComponent();
            crud = new CRUDOracle("VSMES");
            //dtReason = new DataTable();
            //dtReason.Columns.Add("PART");
            //dtReason.Columns.Add("REASON");
            //dtReason.Columns.Add("USE_YN");


            BuildMicButton();
            Disposed += (s, e) => { _autoLoopCts?.Cancel(); VlogFinalize(); _voiceEngine?.Dispose(); };
        }
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
        SerialPort serialPort1;
        private void frmTMC7036_Load(object sender, EventArgs e)
        {
            spDeptCode = "STF";
            SetErrorToButton(spDeptCode);
            pictureEdit2.Focus();
            countDefectContinuos = 0;
            TryToUpdateSystemDateTime();
            ipAddress = GlobalFunction.PublicFunction.myIpaddress;
            //setScanIP();
            //ipAddress = "192.168.0.246";
            //string newFileName = "";
            //string locationFile = "";
            //UploadFile("ftp://192.168.1.15/Mes/BTS/" + newFileName, locationFile);]
            timer1.Enabled = true;
            timer2.Enabled = true;
            StringBuilder query = new StringBuilder();
            sensorCount = 0;
            finishedCount = 0;
            countSensor = 0;
            countSensorQC = 0;
            BindingControl();
            GetSetButtonLocation();
            ConffigErrorButton(false);
            errorTouch = new DataTable();
            errorTouch.Columns.Add("line");
            errorTouch.Columns.Add("part");
            errorTouch.Columns.Add("reason");
            errorTouch.Columns.Add("model");
            errorTouch.Columns.Add("leftright");
            errorTouch.Columns.Add("ip");
            SetTouchCount();
            setDataProduction();
            //ProcessTopDefect();
            if (this.Width > 1366)
            {
                lblPart1.Font = new Font("Arial", 175);
                lblPart2.Font = new Font("Arial", 175);
                lblPart3.Font = new Font("Arial", 175);
                lblPart4.Font = new Font("Arial", 175);
            }
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {

            }
            MQTT_Init();
            //BroadCastCamera();

            InitializeVoiceEngine();
             //── Voice ──────────────────────────────────────────────
            _voice = new VoiceRecognitionService(
                commands: new[]
                {
                    "A","B","C","D",
                    "pass","fail","repass","refail","39","40","41","42","43","44","45","46","47","48","49","50","51","52","53","54","55"
                },
                enableTts: false
            );

            _voice.MessageLogged += (_, e) =>
            {
                if (InvokeRequired) Invoke(() => ShowMessage(e.Message, e.Color));
                else ShowMessage(e.Message, e.Color);
            };

            // Bỏ cái cũ, thêm cái mới
            _voice.RecognitionCompleted -= OnRecognitionCompleted;
            _voice.RecognitionCompleted += (_, e) =>
            {
                SafeInvoke(() =>
                {
                    _micButton.Enabled = true;
                    _micButton.Text = "🎙 Giọng nói";
                    _micButton.BackColor = SystemColors.Control;
                });

                if (string.IsNullOrWhiteSpace(e.RawText)) return;
                if ((DateTime.Now - _lastVoiceTime).TotalMilliseconds < 800) return;
                _lastVoiceTime = DateTime.Now;

                string normalized = NormalizeVoice(e.RawText);
                if (string.IsNullOrWhiteSpace(normalized)) return;

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"[Voice RAW] {e.RawText}");
                System.Diagnostics.Debug.WriteLine($"[Voice NRM] {normalized}");

                lock (_vlogLock)
                {
                    if (_vlog != null)
                    {
                        _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] RAW: {e.RawText}");
                        _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] NRM: {normalized}");
                        _vlog.Flush();
                    }
                    else if (_vlogPath != null)
                    {
                        // _vlog chưa mở (chưa bấm Auto Voice) → ghi thẳng vào file
                        System.IO.File.AppendAllText(
                            _vlogPath,
                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] RAW: {e.RawText}\n" +
                            $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] NRM: {normalized}\n",
                            System.Text.Encoding.UTF8);
                    }
                }

                if (!TryParseVoiceCommand(normalized,
                        out string position, out string errorCode, out string action))
                {
                    SafeInvoke(() => ShowMessage($"❌ Không khớp: '{normalized}'  (raw: '{e.RawText}')", Color.Gray));
                    return;
                }

                SafeInvoke(() =>
                {
                    ShowMessage($"✔ {position} | {errorCode} | {action}", Color.Green);
                    ProcessCommand(position, errorCode, action);
                });
            };




            _ = _voice.InitializeAsync();
        }
        // Thay thế đoạn match cũ trong RecognitionCompleted
        private bool TryParseVoiceCommand(string normalized,
            out string position, out string errorCode, out string action)
        {
            position = errorCode = action = "";

            // Trích từng thành phần độc lập — không cần đúng thứ tự
            var posMatch = Regex.Match(normalized, @"\b(a|b|c|d)\b");
            var errorMatch = Regex.Match(normalized, @"\b(3[9]|4[0-9]|5[0-5])\b");
            var actionMatch = Regex.Match(normalized, @"\b(repass|refail|pass|fail)\b");
            // repass/refail phải check trước vì chứa "pass"/"fail"

            if (!posMatch.Success || !errorMatch.Success || !actionMatch.Success)
                return false;

            position = posMatch.Value.ToUpper();
            errorCode = errorMatch.Value;
            action = actionMatch.Value;
            return true;
        }
        private async void MQTT_Init()
        {
            string MQTTServer = "";
            StringBuilder qry = new StringBuilder();
            qry.AppendLine(" SELECT ");
            qry.AppendLine(" (SELECT NVL(N_ANDON,'0') FROM TRTB_M_COMMON WHERE N_COMNAME = '" + ipAddress + "' AND C_COMCODE LIKE 'STF%' AND N_ANDON IS NOT NULL) CLIENT  ");
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
                        btn_reasonCode2.BackColor = System.Drawing.Color.Orange;
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
                        btn_reasonCode3.BackColor = System.Drawing.Color.DarkGreen;
                    });
                }
            }
        }
        //private void setScanIP()
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.AppendLine("");
        //    //query.AppendLine("            SELECT N_COMNAME FROM TRTB_M_COMMON WHERE C_COMCODE IN (                 ");
        //    //query.AppendLine("SELECT SUBSTR(C_COMCODE,4,LENGTH(C_COMCODE)) SUBLINE                                 ");
        //    //query.AppendLine("  From TRTB_M_COMMON                                                                 ");
        //    //query.AppendLine(" WHERE C_GROUP = 'BTS'                                                               ");
        //    //query.AppendLine(" AND N_COMNAME = '" + ipAddress + "')                                                ");
        //    query.AppendLine("            SELECT      NVL (C_COMCODE, 'X') N_COMNAME     ");
        //    query.AppendLine("  FROM TRTB_M_COMMON                             ");
        //    query.AppendLine(" WHERE     C_GROUP = 'BTS'                       ");
        //    query.AppendLine("       AND N_COMNAME = '" + ipAddress + "'       ");




        //    DataTable dt = new DataTable();
        //    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
        //    if (dt.Rows.Count > 0)
        //    {
        //        ScanIP = dt.Rows[0]["N_COMNAME"].ToString();
        //    }

        //}
        //private void SetSpLine(string ip)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.AppendLine("");

        //    query.AppendLine("SELECT SUBSTR(C_COMCODE,4,4) C_COMCODE,N_COMNAME                                                               ");
        //    query.AppendLine("  From TRTB_M_COMMON                                                                                           ");
        //    query.AppendLine(" WHERE C_GROUP = 'BTS'                                                                                         ");
        //    query.AppendLine("   AND N_COMNAME = '" + ip + "'                                                                                ");

        //    DataTable dt = new DataTable();
        //    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
        //    if (dt.Rows.Count > 0)
        //    {
        //        spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //    }
        //    else
        //    {
        //        XtraMessageBox.Show("Chưa khai báo trên TRTB_M_COMMON. Liên hệ IT" + Environment.NewLine + "Did not registered IP address. Contact IT-team", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        this.Hide();
        //        Application.Exit();
        //    }
        //}
        private void SetLineName(string ip)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");




            //query.AppendLine("   SELECT SUBSTR(C_COMCODE,4,4),                                                                    ");
            //query.AppendLine(" DECODE(SUBSTR(C_COMCODE,6,1),'A','P1','B','P2','C','P3','D','D','E','E','F','F','PP')||            ");
            //query.AppendLine("   CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A' THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55)             ");
            //query.AppendLine("        Else '0'||SUBSTR(C_COMCODE,7,1)                                                             ");
            //query.AppendLine("    END || CASE WHEN LENGTH(C_COMCODE) = 8 THEN 'B' ELSE 'A' END    SHOW_LINE                       ");
            //query.AppendLine("    FROM (                                                                                          ");
            //query.AppendLine("          SELECT SUBSTR(C_COMCODE,1,8) C_COMCODE,N_COMNAME                                          ");
            //query.AppendLine("            From TRTB_M_COMMON                                                                      ");
            //query.AppendLine("           WHERE C_GROUP = 'BTS'                                                                    ");
            //query.AppendLine("             AND N_COMNAME = '" + ip + "'                                                           ");
            //query.AppendLine("         )                                                                                          ");

            query.AppendLine("    SELECT SUBSTR (C_COMCODE, 4, 4) C_LINE,                                 ");
            query.AppendLine("         DECODE (SUBSTR (C_COMCODE, 6, 1),                                  ");
            query.AppendLine("                 'A', 'P1',                                                 ");
            query.AppendLine("                 'B', 'P2',                                                 ");
            query.AppendLine("                 'C', 'P3',                                                 ");
            query.AppendLine("                 'D', 'P4',                                                 ");
            query.AppendLine("                 'E', 'P5',                                                 ");
            query.AppendLine("                 'F', 'P6',                                                 ");
            query.AppendLine("                 SUBSTR (C_COMCODE, 6, 1))                                  ");
            query.AppendLine("      || CASE                                                               ");
            query.AppendLine("            WHEN SUBSTR (C_COMCODE, 7, 1) >= 'A'                            ");
            query.AppendLine("            THEN                                                            ");
            query.AppendLine("               TO_CHAR (ASCII (SUBSTR (C_COMCODE, 7, 1)) - 55)              ");
            query.AppendLine("            WHEN SUBSTR (C_COMCODE, 4, 1) = 'N'                             ");
            query.AppendLine("            THEN                                                            ");
            query.AppendLine("               '0' || SUBSTR (C_COMCODE, 7, 1)                              ");
            query.AppendLine("         END                                                                ");
            query.AppendLine("         SHOW_LINE                                                          ");
            query.AppendLine(" FROM (SELECT SUBSTR (C_COMCODE, 1, 8) C_COMCODE, N_COMNAME                 ");
            query.AppendLine("         FROM TRTB_M_COMMON                                                 ");
            query.AppendLine("        WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + ip + "')                 ");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt.Rows.Count > 0)
            {
                LineName = dt.Rows[0]["SHOW_LINE"].ToString(); //P503
                C_Line = dt.Rows[0]["C_LINE"].ToString(); //NSE3
                txtLineName.Text = LineName;
            }
            else
            {
                MessageBox.Show("Chưa khai báo trên TRTB_M_COMMON. Liên hệ IT" + Environment.NewLine + "Did not registered IP address. Contact IT-team", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Hide();
                Application.Exit();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }
            else
            {
                backgroundWorker1.CancelAsync();
            }


        }
        private void SetAlarm(string header, string message, int longtime)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("SELECT * FROM                                                                ");
            query.AppendLine("TRTB_M_BTS_STOPLINE_NOTIFY                                                   ");
            query.AppendLine("WHERE C_DATE = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')                       ");
            query.AppendLine("AND C_LINE = '" + txtLineName.Text + "'                                      ");

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
                query.AppendLine("VALUES ( '" + txtLineName.Text + "',                                                              ");
                query.AppendLine(" TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),                                                             ");
                query.AppendLine(" '" + ipAddress + "','" + txtRFTValue.Text + "',                                                  ");
                query.AppendLine(" 'N',                                                                                             ");
                query.AppendLine(" 'N',                                                                                             ");
                query.AppendLine(" '" + header + "',                                                                                ");
                query.AppendLine(" '" + message + "',                                                                                ");
                query.AppendLine(" '192.168.5.167',                                                                                 ");
                query.AppendLine(" " + longtime + ")                                                                                ");
                crud.dac.IUExcuteWithQuery(query.ToString());
            }

        }
        private void BindingControl()
        {
            SetLineName(ipAddress);
            //SetSpLine(ipAddress);
            this.btnFT.PerformClick();
            var pos = this.PointToScreen(lblPart1.Location);
            pos = picShoeImage.PointToClient(pos);
            lblPart1.Parent = picShoeImage;
            lblPart1.BackColor = Color.Transparent;
            lblPart1.ForeColor = Color.Red;

            var pos2 = this.PointToScreen(lblPart2.Location);
            pos2 = picShoeImage.PointToClient(pos2);
            lblPart2.Parent = picShoeImage;
            lblPart2.BackColor = Color.Transparent;
            lblPart2.ForeColor = Color.Red;

            var pos3 = this.PointToScreen(lblPart3.Location);
            pos3 = picShoeImage.PointToClient(pos3);
            lblPart3.Parent = picShoeImage;
            lblPart3.BackColor = Color.Transparent;
            lblPart3.ForeColor = Color.Red;

            var pos4 = this.PointToScreen(lblPart4.Location);
            pos4 = picShoeImage.PointToClient(pos4);
            lblPart4.Parent = picShoeImage;
            lblPart4.BackColor = Color.Transparent;
            lblPart4.ForeColor = Color.Red;

            txtSensorCount.Text = sensorCount.ToString();
            SetComport();


            //this.btnCapture.Enabled = false;
            if (serialPort1 == null)
            {
                serialPort1 = new SerialPort();
            }


        }
        private void SetLabelPosition()
        {
            int w = picShoeImage.Width;
            int h = picShoeImage.Height;

            // chia 4 cột
            int colWidth = w / 4;

            // label 1 (cột 1 - giữa)
            lblPart1.Left = colWidth / 2 - lblPart1.Width / 2;
            lblPart1.Top = h / 2;

            // label 2
            lblPart2.Left = colWidth + colWidth / 2 - lblPart2.Width / 2;
            lblPart2.Top = h / 2;

            // label 3
            lblPart3.Left = colWidth * 2 + colWidth / 2 - lblPart3.Width / 2;
            lblPart3.Top = h / 2;

            // label 4
            lblPart4.Left = colWidth * 3 + colWidth / 2 - lblPart4.Width / 2;
            lblPart4.Top = h / 2;
        }
        private void chkVN_CheckedChanged(object sender, EventArgs e)
        {
            SetErrorToButton("STF");
            if (chkVN.Checked)
            {
                chkEng.Checked = false;
            }
            else
            {
                chkEng.Checked = true;
            }
        }
        private void chkEng_CheckedChanged(object sender, EventArgs e)
        {
            SetErrorToButton("STF");
            if (chkEng.Checked)
            {
                chkVN.Checked = false;
            }
            else
            {
                chkVN.Checked = true;
            }
        }
        private void ConffigErrorButton(bool visible)
        {
            foreach (Button btnID in tableLayoutError.Controls)
            {
                btnID.Enabled = visible;
                btnID.ForeColor = visible ? Color.Black : Color.Red;
            }
            foreach (Control a in tableLayoutError1.Controls)
            {
                if (a is Button btnID)
                {
                    btnID.Enabled = visible;
                    btnID.ForeColor = visible ? Color.Black : Color.DarkGray;
                }
            }
        }
        private void SetErrorToButton(string type)
        {
            DataTable DefectLibary = GetError(type);
            if (DefectLibary == null) return;

            // tableLayoutError
            foreach (Button btnID in tableLayoutError.Controls)
            {
                foreach (DataRow dr in DefectLibary.Rows)
                {
                    if (btnID.AccessibleName == dr["REASON_ID"].ToString())
                    {
                        string text = chkVN.Checked
                            ? dr["REASON_VN"].ToString()
                            : dr["REASON_EN"].ToString();
                        btnID.Text = text;
                        if (Reason.ContainsKey(btnID.AccessibleName))
                            Reason[btnID.AccessibleName] = text;
                        else
                            Reason.Add(btnID.AccessibleName, text);
                    }
                }
            }

            // tableLayoutError1
            foreach (Control a in tableLayoutError1.Controls)
            {
                if (a is Button btnID)
                {
                    foreach (DataRow dr in DefectLibary.Rows)
                    {
                        if (btnID.AccessibleName == dr["REASON_ID"].ToString())
                        {
                            string text = chkVN.Checked
                                ? dr["REASON_VN"].ToString()
                                : dr["REASON_EN"].ToString();
                            btnID.Text = text;
                            if (Reason.ContainsKey(btnID.AccessibleName))
                                Reason[btnID.AccessibleName] = text;
                            else
                                Reason.Add(btnID.AccessibleName, text);
                        }
                    }
                }
            }

            // tablelayoutErrorCamera
            //foreach (Button btnID in tablelayoutErrorCamera.Controls)
            //{
            //    foreach (DataRow dr in DefectLibary.Rows)
            //    {
            //        if (btnID.AccessibleName == dr["REASON_ID"].ToString())
            //        {
            //            btnID.Text = chkVN.Checked
            //                ? dr["REASON_VN"].ToString()
            //                : dr["REASON_EN"].ToString();
            //        }
            //    }
            //}
        }
        private DataTable GetError(string type)
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("        SELECT PART_ID, REASON_ID, REASON_SHORT, REASON_EN, REASON_VN          ");
            query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@inf_m_e                                           ");
            query.AppendLine("WHERE DEPT_CODE = '" + type + "' AND REASON_ID > 38                            ");
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                MessageBox.Show("Can not find Defect Reason. Contact IT", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return dt;
            }
        }
        private void btnFT_Click(object sender, EventArgs e)
        {
            spDeptCode = "STF";
            btnFT.BackColor = Color.GreenYellow;

            btnCSS.BackColor = Color.FromArgb(0, 0, 0, 0);
            btnCSU.BackColor = Color.FromArgb(0, 0, 0, 0);
            SetErrorToButton(spDeptCode);
        }
        private void btnCSU_Click(object sender, EventArgs e)
        {
            //spDeptCode = "CSU";
            //btnCSU.Appearance.BackColor = Color.GreenYellow;
            //btnCSU.Appearance.BackColor2 = Color.GreenYellow;
            //btnCSS.Appearance.BackColor = Color.FromArgb(0, 0, 0, 0);
            //btnCSS.Appearance.BackColor2 = Color.FromArgb(0, 0, 0, 0);
            //btnFT.Appearance.BackColor = Color.FromArgb(0, 0, 0, 0);
            //btnFT.Appearance.BackColor2 = Color.FromArgb(0, 0, 0, 0);
        }
        private void btnCSS_Click(object sender, EventArgs e)
        {
            //spDeptCode = "CSS";
            //btnCSS.Appearance.BackColor = Color.GreenYellow;
            //btnCSS.Appearance.BackColor2 = Color.GreenYellow;
            //btnCSU.Appearance.BackColor = Color.FromArgb(0, 0, 0, 0);
            //btnCSU.Appearance.BackColor2 = Color.FromArgb(0, 0, 0, 0);
            //btnFT.Appearance.BackColor = Color.FromArgb(0, 0, 0, 0);
            //btnFT.Appearance.BackColor2 = Color.FromArgb(0, 0, 0, 0);
        }
        private void btnModel_Click(object sender, EventArgs e)
        {
            EOL.SelectModel model = new SelectModel();
            model.dtModel = ProcessTableModel();
            model.ShowDialog(this);
            this.btnModel.Text = model.ReturnSelection;
            if (btnModel.Text != "" && btnModel.Text != "CHỌN MODEL")
            {
                Bitmap bm = ByteToImage(GetImgByte("ftp://192.168.1.15/Mes/BTS/" + spDeptCode + "_" + btnModel.Text.Replace("/", "") + ".jpg"))
                ;
                if (bm != null)
                {
                    this.picShoeImage.Image = bm;

                }
                else
                {
                    //picShoeImage.Image = TouchDefect.Properties.Resources.sSTF_3;
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
                        dr[0] = oldDataModel.Rows[a]["GROUP_SUM"].ToString();
                        break;
                    case 2:
                        dr[1] = oldDataModel.Rows[a]["GROUP_SUM"].ToString();
                        break;
                    case 3:
                        dr[2] = oldDataModel.Rows[a]["GROUP_SUM"].ToString();
                        break;
                    case 4:
                        dr[3] = oldDataModel.Rows[a]["GROUP_SUM"].ToString();
                        break;
                    case 5:
                        dr[4] = oldDataModel.Rows[a]["GROUP_SUM"].ToString();
                        break;
                }
                if (countcol == 5)
                {
                    dr = dt.Rows.Add();
                    countcol = 0;
                }
            }
            return dt;
        }
        private DataTable GetDataModel()
        {
            StringBuilder query = new StringBuilder();
            //if (radioGroup1.Properties.Items[radioGroup1.SelectedIndex].Description == "ADIDAS")
            //{

            //    query.AppendLine("");
            //    query.AppendLine("SELECT GROUP_SUM,ROWNUM FROM(                                                 ");
            //    query.AppendLine("        SELECT DISTINCT GROUP_SUM                                             ");
            //    query.AppendLine("        FROM MES.TB_ORDER_TRACKING@inf_m_e A, MES.MES_MODEL@inf_m_e B         ");
            //    query.AppendLine("       Where a.GROUP_SUM = B.MES_GROUP_SUM                                    ");
            //    query.AppendLine("         AND B.MES_BUYER_CODE IN ('ADIDAS')                                   ");
            //    query.AppendLine("        ORDER BY 1)                                                           ");
            //}
            //else
            {
                query = new StringBuilder();
                query.AppendLine("");
                query.AppendLine("SELECT GROUP_SUM,ROWNUM FROM(                                                 ");
                query.AppendLine("        SELECT DISTINCT GROUP_SUM                                             ");
                query.AppendLine("        FROM MES.TB_ORDER_TRACKING@inf_m_e A, MES.MES_MODEL@inf_m_e B         ");
                query.AppendLine("       Where a.GROUP_SUM = B.MES_GROUP_SUM                                    ");
                query.AppendLine("         AND B.MES_BUYER_CODE IN ('NB')                                       ");
                query.AppendLine("        ORDER BY 1)                                                           ");
            }

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            return dt;
        }

        //private DataTable ProcessTopDefect()
        //{
        //    DataTable DataTopDefect = GetDataTopDefect(C_Line);
        //    if (DataTopDefect == null || DataTopDefect.Rows.Count <= 0)
        //    {
        //        return null;
        //    }
        //    DataTable dt1 = new DataTable();
        //    dt1.Columns.Add("col1");
        //    dt1.Columns.Add("col2");
        //    dt1.Columns.Add("col3");
        //    int countcol = 0;
        //    DataRow dr = dt1.Rows.Add();
        //    for (int a = 0; a <= dt1.Rows.Count; a++)
        //    {
        //        countcol = countcol + 1;
        //        switch (countcol)
        //        {
        //            case 1:
        //                dr[0] = DataTopDefect.Rows[a]["TOP_DEFECT"].ToString();
        //                break;
        //            case 2:
        //                dr[1] = DataTopDefect.Rows[a]["TOP_DEFECT"].ToString();
        //                break;
        //            case 3:
        //                dr[2] = DataTopDefect.Rows[a]["TOP_DEFECT"].ToString();
        //                break;
        //        }
        //        if (countcol == 3)
        //        {
        //            dr = dt1.Rows.Add();
        //            countcol = 0;
        //        }
        //    }
        //    return dt1;
        //}

        private DataTable GetDataTopDefect(string line)
        {
            StringBuilder query = new StringBuilder();
            {
                query.AppendLine("");
                query.AppendLine("  SELECT A.C_LINE,                                                         ");
                query.AppendLine("         A.REASON_ID,                                                      ");
                query.AppendLine("         A.COUNT_DEFECT,                                                   ");
                query.AppendLine("            A.REASON_EN                                                    ");
                query.AppendLine("         || ' ('                                                           ");
                query.AppendLine("         || ROUND (COUNT_DEFECT / TOP_DEFECT * 1000000.0, 1)               ");
                query.AppendLine("         || ')'                                                           ");
                query.AppendLine("            TOP_DEFECT_EN,                                                 ");
                query.AppendLine("            A.REASON_VN                                                    ");
                query.AppendLine("         || ' ('                                                           ");
                query.AppendLine("         || ROUND (COUNT_DEFECT / TOP_DEFECT * 1000000.0, 1)               ");
                query.AppendLine("         || ')'                                                           ");
                query.AppendLine("            TOP_DEFECT_VN                                                  ");
                query.AppendLine("    FROM (  SELECT C_LINE,                                                 ");
                query.AppendLine("                   A.REASON_ID,                                            ");
                query.AppendLine("                   SUM (Q_COUNT) COUNT_DEFECT,                             ");
                query.AppendLine("                   REASON_EN,                                              ");
                query.AppendLine("                   REASON_VN                                               ");
                query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B   ");
                query.AppendLine("             WHERE     D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'  ");
                query.AppendLine("                   AND C_LINE LIKE '" + C_Line + "'                        ");
                query.AppendLine("                   AND A.REASON_ID = B.REASON_ID                           ");
                query.AppendLine("                   AND B.DEPT_CODE = 'STF'                                 ");
                query.AppendLine("          GROUP BY C_LINE,                                                 ");
                query.AppendLine("                   A.REASON_ID,                                            ");
                query.AppendLine("                   B.REASON_EN,                                            ");
                query.AppendLine("                   REASON_VN) A,                                           ");
                query.AppendLine("         (  SELECT SUM (Q_COUNT) TOP_DEFECT                                ");
                query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B   ");
                query.AppendLine("             WHERE     D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'  ");
                query.AppendLine("                   AND C_LINE LIKE '" + C_Line + "'                            ");
                query.AppendLine("                   AND A.REASON_ID = B.REASON_ID                           ");
                query.AppendLine("                   AND B.DEPT_CODE = 'STF'                                 ");
                query.AppendLine("          GROUP BY C_LINE) G                                               ");
                query.AppendLine("ORDER BY 3 DESC                                                            ");



            }

            DataTable dt1 = new DataTable();
            dt1 = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            return dt1;
        }


        private bool ConnectComport(string portName)
        {
            try
            {
                if (serialPort1 == null)
                {
                    serialPort1 = new SerialPort();
                }

                serialPort1.PortName = portName;
                serialPort1.Open();
                serialPort1.DtrEnable = true;
                serialPort1.RtsEnable = true;

                if (serialPort1.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("does not exist."))
                {
                    return false;
                }
                return false;
            }
        }
        private void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            string value = serialPort1.ReadExisting();


            string value2 = value.Trim();

            try
            {
                string[] stringSeparators = new string[] { "\r\n" };
                string[] lines = value2.Split(stringSeparators, StringSplitOptions.None);


                for (int i = 0; i < lines.Count(); i++)
                {
                    datarecive = lines[i].ToString();
                    if (datarecive == "@")
                    {
                        if (linesensor.Count() > 70 && linesensor.Count < 240000)
                        {
                            countSensor = countSensor + 1;
                            SetMessageBoard("Kiểm tra Giày", false, 500);
                        }
                        //else if (linesensor.Count() > 120000)
                        //{
                        //    SetMessageBoard("Lấy Giày Ra(Right)", true, 500);
                        //    return;
                        //}
                        else if (linesensor.Count() < 70 && linesensor.Count() > 15)
                        {
                            SetMessageBoard("Đừng đụng Sensor(Right)", true, 500);
                            linesensor.Clear();
                            countSensor = 0;
                            return;
                        }
                        linesensor.Clear();
                        if (countSensor > 0)
                        {
                            UpdateExecuteSensor("MD");
                        }
                        sensorCount = sensorCount + countSensor;
                        countSensor = 0;
                    }
                    else if (datarecive == "!")
                    {
                        if (lineqc.Count() > 50 && lineqc.Count < 240000)
                        {
                            countSensorQC = countSensorQC + 1;
                            SetMessageBoard("Kiểm tra Giày", false, 500);
                        }
                        //else if (lineqc.Count() > 120000)
                        //{
                        //    SetMessageBoard("Lấy Giày Ra(Left)", true, 500);
                        //    return;
                        //}
                        else if (lineqc.Count() < 50 && lineqc.Count() > 1)
                        {
                            SetMessageBoard("Đừng đụng Sensor(Left)", true, 500);
                            lineqc.Clear();
                            countSensorQC = 0;
                            return;
                        }
                        lineqc.Clear();
                        if (countSensorQC > 0)
                        {
                            UpdateExecuteSensor("QC");
                        }
                        finishedCount = finishedCount + countSensorQC;

                        countSensorQC = 0;
                    }
                    else if (datarecive == "1")
                    {
                        linesensor.Add(datarecive);
                        if (linesensor.Count() > 300000)
                        {
                            SetMessageBoard("Lấy Giày Ra(Right)", true, 500);
                            return;
                        }
                    }
                    else if (datarecive == "2")
                    {
                        lineqc.Add(datarecive);
                        if (lineqc.Count() > 300000)
                        {
                            SetMessageBoard("Lấy Giày Ra(Left)", true, 500);
                            return;
                        }

                    }
                }

                //totalcount = int.Parse(value2) + totalcount;
                //int.Parse(value);
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://192.168.1.7:8080/test/arduino/ircount?ip=" + ipaddress + "&qty=" + value2);
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //string content = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (Exception ex)
            {

            }
        }
        private void HandleSerialData(byte[] respBuffer)
        {
            //I want to take what is in the buffer and combine it with another array
            byte[] AddOn = { 0x01, 0x02 };
            byte[] Combo = { AddOn[1], AddOn[2], respBuffer[0] };
        }

        delegate void SetTextCallback(string message, bool error, int duration);
        public void SetMessageBoard(string message, bool error, int duration)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtMessage.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetMessageBoard);
                this.Invoke(d, new object[] { message, error, duration });
            }
            else
            {
                txtMessage.Text = message;

                if (error)
                {
                    timerSuccess.Enabled = false;
                    timerError.Enabled = true;
                    timerError.Interval = duration;
                }
                else
                {
                    timerError.Enabled = false;
                    timerSuccess.Enabled = true;
                    timerSuccess.Interval = duration;
                }
            }
        }
        private DataTable GetProdInformation()
        {
            StringBuilder query = new StringBuilder();

            //query.AppendLine("            SELECT X.C_LINE, NVL(X.PROD_QTY,0) PROD_QTY, REASON_ID, A.DEFECT_SUM, B.REASON_CNT,                       ");                        
            //query.AppendLine("     ROUND((A.DEFECT_SUM/X.PROD_QTY)*100,2) PER_DEFECT, ROUND((B.REASON_CNT/X.PROD_QTY)*100,2) PER_REASON,            ");                        
            //query.AppendLine("      CASE WHEN ROUND((A.DEFECT_SUM/X.PROD_QTY)*100,2) >= 10 THEN 'Y' ELSE 'N' END PROD_MARK,                         ");                        
            //query.AppendLine("      CASE WHEN ROUND((B.REASON_CNT/X.PROD_QTY)*100,2) >= 5 THEN 'Y' ELSE 'N' END DEFECT_MARK                         ");                        
            //query.AppendLine(" FROM (SELECT C_LINE,DEPT_CODE,SUM(Q_PROD)   PROD_QTY FROM                                                            ");
            //query.AppendLine(" TRTB_M_PROD_IP WHERE IP_ADDRESS = '" + ScanIP + "'                                                                    ");
            //query.AppendLine(" AND D_GATHER LIKE TO_CHAR(SYSDATE,'YYYYMMDD')||'%'                                                               ");
            //query.AppendLine(" GROUP BY     C_LINE,DEPT_CODE                                                                                        ");                      
            //query.AppendLine("      ) X,                                                                                                            ");                        
            //query.AppendLine("      (SELECT A.C_LINE, COUNT(*) DEFECT_SUM                                                                           ");                        
            //query.AppendLine("         From TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                   ");                        
            //query.AppendLine("        WHERE A.REASON_ID = B.REASON_ID                                                                               ");                        
            //query.AppendLine("          AND A.D_GATHER LIKE TO_CHAR(SYSDATE,'YYYYMMDD')||'%'                                                    ");                            
            //query.AppendLine("          AND A.C_LINE LIKE '" + spLine + "'                                                                                    ");              
            //query.AppendLine("          AND B.DEPT_CODE = '" + spDeptCode +"'                                                                                     ");
            //query.AppendLine("          AND A.IP_ADDRESS = '" + ipAddress + "'                                                                          ");                    
            //query.AppendLine("        GROUP BY A.C_LINE                                                                                             ");                        
            //query.AppendLine("      ) A,                                                                                                            ");                        
            //query.AppendLine("      (SELECT A.C_LINE, A.REASON_ID, COUNT(*) REASON_CNT                                                              ");                        
            //query.AppendLine("         From TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                   ");                        
            //query.AppendLine("        WHERE A.REASON_ID = B.REASON_ID                                                                               ");                        
            //query.AppendLine("          AND A.D_GATHER LIKE TO_CHAR(SYSDATE,'YYYYMMDD')||'%'                                                    ");
            //query.AppendLine("          AND A.C_LINE LIKE '" + spLine + "'                                                                                    ");              
            //query.AppendLine("          AND B.DEPT_CODE = '" + spDeptCode + "'                                                                                     ");
            //query.AppendLine("          AND A.IP_ADDRESS = '" + ipAddress + "'                                                                          ");        
            //query.AppendLine("        GROUP BY A.C_LINE, A.REASON_ID                                                                                ");                        
            //query.AppendLine("      ) B                                                                                                             ");                        
            //query.AppendLine("WHERE X.C_LINE = A.C_LINE(+)                                                                                          ");                        
            //query.AppendLine("  AND X.C_LINE = B.C_LINE(+)                                                                                          ");
            //query.AppendLine("ORDER BY 1,3                                                                                                          ");

            /* Formatted on 12/21/2019 8:23:30 AM (QP5 v5.252.13127.32847) */

            //string brand = "";
            //brand = radioGroup1.Properties.Items[radioGroup1.SelectedIndex].Description;
            //string brand = "NB";

            //query.AppendLine("  SELECT X.C_LINE,                                                                                                       ");
            //query.AppendLine("         X.PROD_QTY,                                                                                                     ");
            //query.AppendLine("         REASON_ID,                                                                                                      ");
            //query.AppendLine("         A.DEFECT_SUM,                                                                                                   ");
            //query.AppendLine("         B.REASON_CNT,                                                                                                   ");
            //query.AppendLine("         ROUND ( (A.DEFECT_SUM / X.PROD_QTY) * 100, 2) PER_DEFECT,                                                       ");
            //query.AppendLine("         ROUND ( (B.REASON_CNT / X.PROD_QTY) * 100, 2) PER_REASON,                                                       ");
            //query.AppendLine("         CASE                                                                                                            ");
            //query.AppendLine("            WHEN ROUND ( (A.DEFECT_SUM / X.PROD_QTY) * 100, 2) >= 10 THEN 'Y'                                            ");
            //query.AppendLine("            ELSE 'N'                                                                                                     ");
            //query.AppendLine("         END                                                                                                             ");
            //query.AppendLine("            PROD_MARK,                                                                                                   ");
            //query.AppendLine("         CASE                                                                                                            ");
            //query.AppendLine("            WHEN ROUND ( (B.REASON_CNT / X.PROD_QTY) * 100, 2) >= 5 THEN 'Y'                                             ");
            //query.AppendLine("            ELSE 'N'                                                                                                     ");
            //query.AppendLine("         END                                                                                                             ");
            //query.AppendLine("            DEFECT_MARK                                                                                                  ");
            //query.AppendLine("    FROM (  SELECT A.C_LOCATION, A.C_LINE, SUM (PROD_QTY) PROD_QTY                                                       ");
            //query.AppendLine("              FROM (  SELECT B.C_JOBORDER_NO,                                                                            ");
            //query.AppendLine("                             C.C_LOCATION,                                                                               ");
            //query.AppendLine("                             C.C_PROD_LINE C_LINE,                                                                       ");
            //query.AppendLine("                             SUM (B.Q_QTY) PROD_QTY                                                                      ");
            //query.AppendLine("                        FROM TRTB_M_CARD B, TRTB_M_PROD_YIELD C                                                          ");
            //query.AppendLine("                       WHERE     B.I_CARD_NO = C.I_CARD_NO                                                               ");
            //query.AppendLine("                             AND C.D_GATHER BETWEEN    TO_CHAR (SYSDATE,                                                 ");
            //query.AppendLine("                                                                'YYYYMMDD')                                              ");
            //query.AppendLine("                                                    || '000000'                                                          ");
            //query.AppendLine("                                                AND    TO_CHAR (SYSDATE,                                                 ");
            //query.AppendLine("                                                                'YYYYMMDD')                                              ");
            //query.AppendLine("                                                    || '235959'                                                          ");
            //query.AppendLine("                             AND C.C_LOCATION = 'SSFA'                                                                   ");
            //query.AppendLine("                             AND C_PROD_LINE =                                                                           ");
            //query.AppendLine("                                       DECODE (SUBSTR ('" + C_Line + "', 3, 1),                                          ");
            //query.AppendLine("                                               'A', 'P1',                                                                ");
            //query.AppendLine("                                               'B', 'P2',                                                                ");
            //query.AppendLine("                                               'C', 'P3',                                                                ");
            //query.AppendLine("                                               'D', 'P4',                                                                ");
            //query.AppendLine("                                               'E', 'P5',                                                                ");
            //query.AppendLine("                                               'F', 'P6',                                                                ");
            //query.AppendLine("                                               '" + C_Line + "')                                                         ");
            //query.AppendLine("                                    || CASE                                                                              ");
            //query.AppendLine("                                          WHEN SUBSTR ('" + C_Line + "', 4, 1) >= 'A'                                    ");
            //query.AppendLine("                                          THEN                                                                           ");
            //query.AppendLine("                                             TO_CHAR (                                                                   ");
            //query.AppendLine("                                                ASCII (SUBSTR ('" + C_Line + "', 4, 1)) - 55)                            ");
            //query.AppendLine("                                          WHEN SUBSTR ('" + C_Line + "', 1, 1) = 'N'                                     ");
            //query.AppendLine("                                          THEN                                                                           ");
            //query.AppendLine("                                             '0' || SUBSTR ('" + C_Line + "', 4, 1)                                      ");
            //query.AppendLine("                                          ELSE                                                                           ");
            //query.AppendLine("                                             ''                                                                          ");
            //query.AppendLine("                                       END                                                                               ");
            //query.AppendLine("                    GROUP BY B.C_JOBORDER_NO, C.C_LOCATION, C.C_PROD_LINE) A,                                            ");
            //query.AppendLine("                   TRTB_M_PROD_PLAN B,                                                                                   ");
            //query.AppendLine("                   MES.MES_MODEL@INF_M_E D                                                                               ");
            //query.AppendLine("             WHERE     a.C_JOBORDER_NO = B.C_JOBORDER_NO                                                                 ");
            //query.AppendLine("                   AND B.C_STYLE = D.MES_STYLE_NO                                                                        ");
            //query.AppendLine("                   AND D.MES_BUYER_CODE IN ('NB')                                                             ");
            //query.AppendLine("          GROUP BY A.C_LOCATION, A.C_LINE ) X,                                                                           ");
            //query.AppendLine("         (  SELECT A.C_LINE, COUNT (*) DEFECT_SUM                                                                        ");
            //query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                 ");
            //query.AppendLine("             WHERE     A.REASON_ID = B.REASON_ID                                                                         ");
            //query.AppendLine("                   AND A.D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'                                              ");
            //query.AppendLine("                   AND A.C_LINE LIKE '" + C_Line + "'                                                                    ");
            //query.AppendLine("                   AND B.DEPT_CODE = '" + spDeptCode + "'                                                                ");
            //query.AppendLine("          GROUP BY A.C_LINE) A,                                                                                          ");
            //query.AppendLine("         (  SELECT A.C_LINE, A.REASON_ID, COUNT (*) REASON_CNT                                                           ");
            //query.AppendLine("              FROM TRTB_M_BTS_COUNT3 A, mes.TRTB_M_BTS_REASON3@inf_m_e B                                                 ");
            //query.AppendLine("             WHERE     A.REASON_ID = B.REASON_ID                                                                         ");
            //query.AppendLine("                   AND A.D_GATHER LIKE TO_CHAR (SYSDATE, 'YYYYMMDD') || '%'                                              ");
            //query.AppendLine("                   AND A.C_LINE LIKE '" + C_Line + "'                                                                    ");
            //query.AppendLine("                   AND B.DEPT_CODE = '" + spDeptCode + "'                                                                ");
            //query.AppendLine("          GROUP BY A.C_LINE, A.REASON_ID) B                                                                              ");
            //query.AppendLine("   WHERE X.C_LINE = A.C_LINE(+) AND X.C_LINE = B.C_LINE(+)                                                               ");
            //query.AppendLine("ORDER BY 1, 3                                                                                                            ");

            query.AppendLine("                                                                                  ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("                              SELECT  A.C_LINE, SUM (PROD_QTY) PROD_QTY          ");
            query.AppendLine("             FROM (SELECT B.C_JOBORDER_NO,                                       ");
            query.AppendLine("                            C.C_PROD_LINE C_LINE,                                ");
            query.AppendLine("                            SUM (B.Q_QTY) PROD_QTY                               ");
            query.AppendLine("                       FROM TRTB_M_CARD B, TRTB_M_PROD_YIELD C                   ");
            query.AppendLine("                      WHERE     B.I_CARD_NO = C.I_CARD_NO                        ");
            query.AppendLine("                            AND C.D_GATHER BETWEEN    TO_CHAR (SYSDATE,          ");
            query.AppendLine("                                                               'YYYYMMDD')       ");
            query.AppendLine("                                                   || '000000'                   ");
            query.AppendLine("                                               AND    TO_CHAR (SYSDATE,          ");
            query.AppendLine("                                                               'YYYYMMDD')       ");
            query.AppendLine("                                                   || '235959'                   ");
            query.AppendLine("                            AND C.C_LOCATION = 'SSFA'                            ");
            query.AppendLine("                            AND C_PROD_LINE = '" + LineName + "'                 ");
            query.AppendLine("                   GROUP BY B.C_JOBORDER_NO, C.C_LOCATION, C.C_PROD_LINE  ) A,   ");
            query.AppendLine("                  TRTB_M_PROD_PLAN B,                                            ");
            query.AppendLine("                  MES.MES_MODEL@INF_M_E D                                        ");
            query.AppendLine("            WHERE     a.C_JOBORDER_NO = B.C_JOBORDER_NO                          ");
            query.AppendLine("                  AND B.C_STYLE = D.MES_STYLE_NO                                 ");
            query.AppendLine("                  AND D.MES_BUYER_CODE IN ('NB')                                 ");
            query.AppendLine("         GROUP BY  A.C_LINE                                                      ");





            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            productionData = dt;

            return dt;
        }
        private void setProductionAtTime()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("select * from TRTB_M_PROD_IP where d_gather like '" + DateTime.Now.ToString("yyyyMMddHH") + "%' and c_line = '" + C_Line + "' AND IP_ADDRESS = '" + ScanIP + "'");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt.Rows.Count > 0)
            {
                ProdHour = Convert.ToInt32(dt.Rows[0]["Q_PROD"].ToString());
            }
            else
            {
                ProdHour = 0;
            }
        }
        private void Production()
        {
            txtSensorCount.Text = sensorCount.ToString();
            int a = txtProdQty.ToString().Length;
            if (tongProd != 0)
            {
                this.txtDefectQty.Text = "TOTAL : " + TotalDefect + " (" + Math.Round(((float)TotalDefect / tongProd * 1000000.0), 1) + " )";
            }
            else
            {
                this.txtDefectQty.Text = "TOTAL : " + TotalDefect;
            }

            DataTable topdefect = GetDataTopDefect(C_Line);

            if (chkVN.Checked)
            {
                if (topdefect.Rows.Count > 0)
                {
                    lblError1.Text = topdefect.Rows[0]["TOP_DEFECT_VN"].ToString();
                }
                else
                {
                    lblError1.Text = "";
                }
                if (topdefect.Rows.Count > 1)
                {
                    lblError2.Text = topdefect.Rows[1]["TOP_DEFECT_VN"].ToString();
                }
                else
                {
                    lblError2.Text = "";
                }
                if (topdefect.Rows.Count > 2)
                {
                    lblError3.Text = topdefect.Rows[2]["TOP_DEFECT_VN"].ToString();
                }
                else
                {
                    lblError3.Text = "";
                }
            }
            else
            {
                if (topdefect.Rows.Count > 0)
                {
                    lblError1.Text = topdefect.Rows[0]["TOP_DEFECT_EN"].ToString();
                }
                else
                {
                    lblError1.Text = "";
                }
                if (topdefect.Rows.Count > 1)
                {
                    lblError2.Text = topdefect.Rows[1]["TOP_DEFECT_EN"].ToString();
                }
                else
                {
                    lblError2.Text = "";
                }
                if (topdefect.Rows.Count > 2)
                {
                    lblError3.Text = topdefect.Rows[2]["TOP_DEFECT_EN"].ToString();
                }
                else
                {
                    lblError3.Text = "";
                }
            }


        }
        private void BindButton(DataTable dt)
        {
            string oldbutton;
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["PER_REASON"].ToString() != "")
                    {
                        if (Convert.ToInt32(dt.Rows[i]["PER_REASON"]) >= 2 && Convert.ToInt32(dt.Rows[i]["PER_REASON"]) < 5)
                        {
                            foreach (Button btnID in tableLayoutError.Controls)
                            {
                                if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                                {
                                    btnID.BackColor = Color.Yellow;
                                    //btnID.Appearance.BackColor2 = Color.Yellow;
                                    Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                                    btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                                }

                            }
                            foreach (Button btnID in tableLayoutError1.Controls)
                            {
                                if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                                {
                                    btnID.BackColor = Color.Yellow;
                                    //btnID.Appearance.BackColor2 = Color.Yellow;
                                    Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                                    btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                                }
                            }
                            //foreach (Button btnID in tablelayoutErrorCamera.Controls)
                            //{
                            //    if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                            //    {
                            //        btnID.Appearance.BackColor = Color.Yellow;
                            //        btnID.Appearance.BackColor2 = Color.Yellow;
                            //        Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                            //        btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                            //    }
                            //}
                        }
                        else if (Convert.ToInt32(dt.Rows[i]["PER_REASON"]) >= 5)
                        {
                            foreach (Button btnID in tableLayoutError.Controls)
                            {
                                if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                                {
                                    btnID.BackColor = Color.OrangeRed;
                                    //btnID.Appearance.BackColor2 = Color.OrangeRed;
                                    Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                                    btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                                }

                            }
                            foreach (Button btnID in tableLayoutError1.Controls)
                            {
                                if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                                {
                                    btnID.BackColor = Color.OrangeRed;
                                    //btnID.Appearance.BackColor2 = Color.OrangeRed;
                                    Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                                    btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                                }
                            }
                            //foreach (Button btnID in tablelayoutErrorCamera.Controls)
                            //{
                            //    if (btnID.AccessibleName == dt.Rows[i]["REASON_ID"].ToString())
                            //    {
                            //        btnID.Appearance.BackColor = Color.OrangeRed;
                            //        btnID.Appearance.BackColor2 = Color.OrangeRed;
                            //        Reason.TryGetValue(btnID.AccessibleName, out oldbutton);

                            //        btnID.Text = oldbutton + " " + dt.Rows[i]["PER_REASON"].ToString() + "%";
                            //    }
                            //}
                        }
                    }

                }
            }
        }
        private void SetComport()
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Count() == 0)
            {
                txtStatus.Text = "Disconnected";
                panelStatus.BackColor = Color.Red;

                txtStatus.ForeColor = Color.White;
                //MessageBox.Show("Check Sensor Usb Cable.Kiểm tra cổng USB dây sensor có cắm chưa ?");
                return;
            }
            else
            {
                foreach (string port in ports)
                {
                    if (ConnectComport(port))
                    {
                        txtStatus.Text = "Connected";
                        panelStatus.BackColor = Color.FromArgb(235, 236, 239);
                        txtStatus.ForeColor = Color.Green;
                        //MessageBox.Show("Connected " + port + " successfully Kết nối " + port + " thành công ");
                        break;
                    }
                }
                if (serialPort1.IsOpen == false)
                {
                    txtStatus.Text = "Not Connected";
                    panelStatus.BackColor = Color.Red;

                    txtStatus.ForeColor = Color.White;
                    //MessageBox.Show("Can not connect any COM port");
                    return;
                }
            }
        }
        public void CloseSerial()
        {
            try
            {
                if (serialPort1 != null)
                {
                    serialPort1.DataReceived -= SerialPort1_DataReceived;

                    if (serialPort1.IsOpen)
                        serialPort1.Close();

                    serialPort1.Dispose();
                }
            }
            catch { }
        }
        private void SerialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                string value = serialPort1.ReadExisting();
                string[] lines = value.Trim().Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var datarecive in lines)
                {
                    if (datarecive == "@")
                    {
                        if (linesensor.Count > 70 && linesensor.Count < 240000)
                        {
                            countSensor++;
                            SafeUI(() => SetMessageBoard("Kiểm tra Giày", false, 500));
                        }
                        else if (linesensor.Count < 70 && linesensor.Count > 15)
                        {
                            SafeUI(() => SetMessageBoard("Đừng đụng Sensor(Right)", true, 500));
                            linesensor.Clear();
                            countSensor = 0;
                            return;
                        }

                        linesensor.Clear();

                        if (countSensor > 0)
                            UpdateExecuteSensor("MD");

                        sensorCount += countSensor;
                        countSensor = 0;
                    }
                    else if (datarecive == "!")
                    {
                        if (lineqc.Count > 50 && lineqc.Count < 240000)
                        {
                            countSensorQC++;
                            SafeUI(() => SetMessageBoard("Kiểm tra Giày", false, 500));
                        }
                        else if (lineqc.Count < 50 && lineqc.Count > 1)
                        {
                            SafeUI(() => SetMessageBoard("Đừng đụng Sensor(Left)", true, 500));
                            lineqc.Clear();
                            countSensorQC = 0;
                            return;
                        }

                        lineqc.Clear();

                        if (countSensorQC > 0)
                            UpdateExecuteSensor("QC");

                        finishedCount += countSensorQC;
                        countSensorQC = 0;
                    }
                    else if (datarecive == "1")
                    {
                        linesensor.Add(datarecive);

                        if (linesensor.Count > 300000)
                        {
                            SafeUI(() => SetMessageBoard("Lấy Giày Ra(Right)", true, 500));
                            return;
                        }
                    }
                    else if (datarecive == "2")
                    {
                        lineqc.Add(datarecive);

                        if (lineqc.Count > 300000)
                        {
                            SafeUI(() => SetMessageBoard("Lấy Giày Ra(Left)", true, 500));
                            return;
                        }
                    }
                }
            }
            catch
            {
            }
        }
        private void SafeUI(Action action)
        {
            if (this.InvokeRequired)
                this.Invoke(action);
            else
                action();
        }
        //private void SetMessageBoard(string message,bool error,int duration)
        //{
        //    txtMessage.Text = message;

        //    if(error)
        //    {
        //        timerError.Enabled = true;
        //        timerError.Interval = duration;
        //    }
        //    else
        //    {
        //        timerSuccess.Enabled = true;
        //        timerSuccess.Interval = duration;
        //    }
        //}
        private void SetMessageBoard(bool disable)
        {
            if (disable)
            {
                timerError.Enabled = false;
                timerSuccess.Enabled = false;
                txtMessage.Text = "THÔNG BÁO";
                txtMessage.ForeColor = Color.Red;
            }
        }
        private void timer3_Tick(object sender, EventArgs e)
        {
            timerStopMessage.Enabled = true;
            if (txtMessage.ForeColor != Color.Red)
            {
                txtMessage.ForeColor = Color.Red;
            }
            else
            {
                txtMessage.ForeColor = Color.White;
            }
        }
        private void timerSuccess_Tick(object sender, EventArgs e)
        {
            timerStopMessage.Enabled = true;
            if (txtMessage.ForeColor != Color.Green)
            {
                txtMessage.ForeColor = Color.Green;
            }
            else
            {
                txtMessage.ForeColor = Color.White;
            }
        }
        private void lblControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                MouseDownLocation = e.Location;
            }
        }
        private void lblControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (MoveControl == false)
            {
                return;
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Label lbl = (Label)sender;
                lbl.Left = e.X + lbl.Left - MouseDownLocation.X;
                lbl.Top = e.Y + lbl.Top - MouseDownLocation.Y;

            }
        }
        private void GetSetButtonLocation(bool clear, string lblName, int x, int y)
        {
            try
            {
                if (clear)
                {
                    File.Create(Application.StartupPath + "\\ButtonLocation.txt").Close();
                }
                StreamWriter sw = new StreamWriter(Application.StartupPath + "\\ButtonLocation.txt");
                sw.WriteLine("lblPart1" + ":" + lblPart1.Left + "-" + lblPart1.Top);
                sw.WriteLine("lblPart2" + ":" + lblPart2.Left + "-" + lblPart2.Top);
                sw.WriteLine("lblPart3" + ":" + lblPart3.Left + "-" + lblPart3.Top);
                sw.WriteLine("lblPart4" + ":" + lblPart4.Left + "-" + lblPart4.Top);
                sw.Close();
            }
            catch (Exception ex)
            {

            }

        }

        private void InsertDefectLogTextFile(string spLine, string partID, string reasonID, string model, string LeftOrRight, string ipAddress)
        {
            try
            {
                File.Create(Application.StartupPath + "\\DefectLogDaily\\DefectLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt").Close();
                StreamWriter sw = new StreamWriter(Application.StartupPath + "\\ButtonLocation.txt");
                sw.WriteLine("lblPart1" + ":" + lblPart1.Left + "-" + lblPart1.Top);
                sw.WriteLine("lblPart2" + ":" + lblPart2.Left + "-" + lblPart2.Top);
                sw.WriteLine("lblPart3" + ":" + lblPart3.Left + "-" + lblPart3.Top);
                sw.WriteLine("lblPart4" + ":" + lblPart4.Left + "-" + lblPart4.Top);
                sw.Close();
            }
            catch (Exception ex)
            {

            }

        }
        private void GetSetButtonLocation()
        {
            ReadLocation();

        }
        private void ReadLocation()
        {
            Dictionary<string, int> location = new Dictionary<string, int>();
            string locationXY;
            string[] locationsXY;
            string line;
            try
            {
                StreamReader sr = new StreamReader(Application.StartupPath + "\\ButtonLocation.txt");
                line = sr.ReadLine();
                while (line != null)
                {
                    switch (line.Substring(0, 9))
                    {
                        case "lblPart1:":
                            locationXY = line.Substring(9, line.Length - 9);
                            locationsXY = locationXY.Split('-');
                            lblPart1.Left = Convert.ToInt32(locationsXY[0]);
                            lblPart1.Top = Convert.ToInt32(locationsXY[1]);
                            line = sr.ReadLine();
                            break;
                        case "lblPart2:":
                            locationXY = line.Substring(9, line.Length - 9);
                            locationsXY = locationXY.Split('-');
                            lblPart2.Left = Convert.ToInt32(locationsXY[0]);
                            lblPart2.Top = Convert.ToInt32(locationsXY[1]);
                            line = sr.ReadLine();
                            break;
                        case "lblPart3:":
                            locationXY = line.Substring(9, line.Length - 9);
                            locationsXY = locationXY.Split('-');
                            lblPart3.Left = Convert.ToInt32(locationsXY[0]);
                            lblPart3.Top = Convert.ToInt32(locationsXY[1]);
                            line = sr.ReadLine();
                            break;
                        case "lblPart4:":
                            locationXY = line.Substring(9, line.Length - 9);
                            locationsXY = locationXY.Split('-');
                            lblPart4.Left = Convert.ToInt32(locationsXY[0]);
                            lblPart4.Top = Convert.ToInt32(locationsXY[1]);
                            line = sr.ReadLine();
                            break;
                    }
                }
                sr.Close();
            }
            catch (Exception ex)
            {

            }
        }
        private void lblPart_MouseUp(object sender, MouseEventArgs e)
        {
            GetSetButtonLocation(true, lblPart1.Name, lblPart1.Left, lblPart1.Top);
        }
        private void timerStopMessage_Tick(object sender, EventArgs e)
        {
            txtMessage.Text = "THÔNG BÁO";
            txtMessage.ForeColor = Color.Red;
            timerStopMessage.Enabled = false;
            timerSuccess.Enabled = false;
            timerError.Enabled = false;
        }
        private void lblPart3_Click(object sender, EventArgs e)
        {
            LeftOrRight = "";
            if (btnModel.Text == "" || btnModel.Text == "CHỌN MODEL")
            {
                MessageBox.Show("Chọn Model trước khi chấm lỗi. !! Please choose model", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            lblPart1.ForeColor = Color.Red;
            lblPart2.ForeColor = Color.Red;
            lblPart3.ForeColor = Color.Red;
            lblPart4.ForeColor = Color.Red;


            Label lbl = (Label)sender;
            lbl.ForeColor = Color.Green;
            ConffigErrorButton(true);
            timerTouch.Enabled = true;
            partID = lbl.AccessibleName;

        }
        private void timerTouch_Tick(object sender, EventArgs e)
        {
            lblPart1.ForeColor = Color.Red;
            lblPart2.ForeColor = Color.Red;
            lblPart3.ForeColor = Color.Red;
            lblPart4.ForeColor = Color.Red;
            ConffigErrorButton(false);
            StringBuilder query = new StringBuilder();
            errorTouch.Rows.Clear();
            timerTouch.Enabled = false;

        }
        private void SetTouchCount()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            DataTable dt = new DataTable();
            // data from sqlLite
            //dt = sqllite.LoadDataRealtime(btnModel.Text);
            //query.AppendLine("SELECT C.C_LINE C_LINE, NVL(COUNT(*),0) CNT                                                                                               ");
            //query.AppendLine("        FROM MES.TRTB_M_BTS_PART@INF_M_E A, MES.TRTB_M_BTS_REASON3@INF_M_E B, MES.TRTB_M_BTS_COUNT3 C                                     ");
            //query.AppendLine("       Where a.PART_ID    = C.PART_ID                                                                                                     ");
            //query.AppendLine("         AND B.REASON_ID  = C.REASON_ID                                                                                                   ");
            //query.AppendLine("         AND A.DEPT_CODE  = '" + spDeptCode + "'                                                                                          ");
            //query.AppendLine("         AND B.DEPT_CODE  = '" + spDeptCode + "'                                                                                          ");
            //query.AppendLine("         AND C.C_LINE     = '" + spLine + "'                                                                                              ");
            //query.AppendLine("         AND C.IP_ADDRESS = '" + ipAddress + "'                                                                                           ");
            //query.AppendLine("         AND C.D_GATHER BETWEEN TO_CHAR(SYSDATE,'yyyyMMdd') ||'000001' AND TO_CHAR(SYSDATE,'yyyyMMdd') ||'235959'                         ");
            //query.AppendLine("       GROUP BY C.C_LINE                                                                                                                  ");

            query.AppendLine("");

            query.AppendLine("            SELECT COUNT (*)  CNT                                     ");
            query.AppendLine("  FROM MES.TRTB_M_BTS_COUNT3                                          ");
            query.AppendLine(" WHERE     C_LINE = '" + C_Line + "'                                  ");
            query.AppendLine("       AND D_GATHER BETWEEN TO_CHAR (SYSDATE, 'yyyyMMdd') || '000000' ");
            query.AppendLine("                        AND TO_CHAR (SYSDATE, 'yyyyMMdd') || '235959' ");

            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            if (dt.Rows.Count > 0)
            {
                TotalDefect = Convert.ToInt32(dt.Rows[0]["CNT"].ToString());
            }
            else
            {
                TotalDefect = 0;
            }
        }
        private void btnError_Click(object sender, EventArgs e)
        {


            TotalDefect = TotalDefect + 1;

            int a = txtProdQty.ToString().Length;
            if (tongProd != 0)
            {
                this.txtDefectQty.Text = "TOTAL : " + TotalDefect + "(" + Math.Round(((float)TotalDefect / tongProd * 100), 2) + "%)";
            }
            else
            {
                this.txtDefectQty.Text = "TOTAL : " + TotalDefect;
            }


            //this.txtDefectQty.Text = "TOTAL : " + (float)TotalDefect / Convert.ToSingle(txtProdQty.Text) * 100 + "%";
            ConffigErrorButton(false);
            Button btn = (Button)sender;
            reasonID = btn.AccessibleName;

            errorTouch.Rows.Add(new object[] { C_Line, partID, reasonID, btnModel.Text, LeftOrRight, ipAddress });
            txtReady.Text = "OK";
            if (errorTouch.Rows.Count > 0)
            {
                for (int i = 0; i < errorTouch.Rows.Count; i++)
                {
                    //sqllite.InsertTouchError(DateTime.Now.AddSeconds(i).ToString("yyyyMMddHHmmss"), partID, reasonID, spLine, btnModel.Text, spDeptCode, LeftOrRight);
                    //DataTable DT = sqllite.LoadDataRealtime(btnModel.Text);


                    StringBuilder query = new StringBuilder();
                    query.AppendLine("");
                    query.AppendLine("INSERT INTO MES.TRTB_M_BTS_COUNT3(D_GATHER, C_LINE,                                                                       ");
                    query.AppendLine(" PART_ID, REASON_ID, MES_GROUP_SUM, USER_ID, IP_ADDRESS)                                                                  ");
                    query.AppendLine("VALUES (TO_CHAR(SYSDATE + (" + i + "/86400),'YYYYMMDDHH24MISS'), '" + C_Line + "',                                        ");
                    query.AppendLine(" '" + partID + "', '" + reasonID + "','" + btnModel.Text + "','" + LeftOrRight + "','" + ipAddress + "')                  ");

                    if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                    {

                    }
                    else
                    {
                        break;
                    }
                }

            }
            Production();
            errorTouch.Rows.Clear();
        }
        private void btnLeft_Click(object sender, EventArgs e)
        {
            LeftOrRight = "1";
        }
        private void btnRight_Click(object sender, EventArgs e)
        {
            LeftOrRight = "2";
            UpdateExecuteSensor("MD");
        }
        public static Bitmap ByteToImage(byte[] blob)
        {
            if (blob == null)
            {
                MessageBox.Show("No Image. Không có hình giày này");

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
                return null;
            }
        }
        public object GetPDF(string ftpFilePath)
        {
            try
            {
                WebClient ftpClient = new WebClient();
                ftpClient.Credentials = new NetworkCredential("mes", "!saigon3535!");

                var a = ftpClient.DownloadData(ftpFilePath);
                return a;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool UploadFile(string filePath, string filename)
        {
            try
            {
                WebClient ftpClient = new WebClient();
                ftpClient.Credentials = new NetworkCredential("mes", "!saigon3535!");

                ftpClient.UploadFile(filePath, filename);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private void frmTMC7031_Resize(object sender, EventArgs e)
        {
            if (this.Width > 900)
            {
                lblPart1.Font = new Font("Arial", 175);
                lblPart2.Font = new Font("Arial", 175);
                lblPart3.Font = new Font("Arial", 175);
                lblPart4.Font = new Font("Arial", 175);
            }
        }
        private void simpleButton32_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            //this.btnCapture.Enabled = true;
            DefectIDCapture = btn.AccessibleName;
            DefectNameCapture = btn.Text;
        }
        private void btnCapture_Click(object sender, EventArgs e)
        {


        }
        //private void dockPanel1_Expanding(object sender, DevExpress.XtraBars.Docking.DockPanelCancelEventArgs e)
        //{
        //    btnCapture.Enabled = false;

        //    if (btnModel.Text == "CHỌN MODEL" || btnModel.Text == "")
        //    {
        //        foreach (SimpleButton btnID in tablelayoutErrorCamera.Controls)
        //        {
        //            btnID.Enabled = false;
        //        }
        //    }
        //    else
        //    {
        //        foreach (SimpleButton btnID in tablelayoutErrorCamera.Controls)
        //        {
        //            btnID.Enabled = true;
        //        }
        //    }
        //}
        private void btnBA_Click(object sender, EventArgs e)
        {

        }
        private void btnCheckStep_Click(object sender, EventArgs e)
        {
        }
        private void txtMetalDetect_Click(object sender, EventArgs e)
        {
            MetalCount = MetalCount + 1;
            txtMetalDetect.Text = MetalCount.ToString();
            UpdateExecuteSensor("MDI");
        }
        private void UpdateExecuteSensor(string sensor)
        {
            int qcCount = 0;
            int mdCount = 0;
            int mdInput = 0;
            int v_count = 0;
            int checkNDCount = 0;
            StringBuilder queryCount = new StringBuilder();
            queryCount.AppendLine("");
            queryCount.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_SENSOR_COUNT WHERE                           ");
            queryCount.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                                            ");
            queryCount.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
            queryCount.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
            queryCount.AppendLine(" AND   TIME_HOUR = '" + DateTime.Now.ToString("yyyyMMddHHmmss") + "'         ");
            queryCount.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(queryCount.ToString());


            StringBuilder queryCount2 = new StringBuilder();
            setProductionAtTime();
            queryCount2.AppendLine("");
            queryCount2.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_SENSOR_COUNT WHERE                           ");
            queryCount2.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                                            ");
            queryCount2.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
            queryCount2.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
            queryCount2.AppendLine(" AND   TIME_HOUR LIKE '" + DateTime.Now.ToString("yyyyMMddHH") + "%'         ");
            queryCount2.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            DataTable dt2 = new DataTable();
            dt2 = crud.dac.DtSelectExcuteWithQuery(queryCount2.ToString());

            if (dt.Rows.Count > 0)
            {
                v_count = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            if (dt2.Rows.Count > 0)
            {
                checkNDCount = Convert.ToInt32(dt2.Rows[0][0].ToString());
            }


            if (sensor == "QC")
            {
                qcCount = 1;
                mdCount = 0;
                mdInput = 0;
            }
            else if (sensor == "MD")
            {
                qcCount = 0;
                mdCount = 1;
                mdInput = 0;
            }
            else if (sensor == "MDI")
            {
                qcCount = 0;
                mdCount = 0;
                mdInput = 1;
            }
            if (v_count == 0)
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("");
                query.AppendLine("INSERT INTO MES.TRTB_M_SENSOR_COUNT (                             ");
                query.AppendLine("   MES_MODEL, IP_ADDRESS, DEPT_CODE,                              ");
                query.AppendLine("   C_LINE, TIME_HOUR, TIME_DATE,                                  ");
                query.AppendLine("   SENSOR_MD, SENSOR_QC, MD_QC_INPUT,                             ");
                query.AppendLine("   IN_DT)                                                         ");
                query.AppendLine("VALUES ( '" + btnModel.Text + "',                                 ");
                query.AppendLine(" '" + ipAddress + "',                                             ");
                query.AppendLine(" '" + spDeptCode + "',                                            ");
                query.AppendLine(" '" + C_Line + "',                                                ");
                query.AppendLine(" '" + DateTime.Now.ToString("yyyyMMddHHmmss") + "',               ");
                query.AppendLine(" '" + DateTime.Now.ToString("yyyyMMdd") + "',                     ");
                query.AppendLine(" " + mdCount + ",                                                 ");
                query.AppendLine(" " + qcCount + ",                                                 ");
                query.AppendLine(" " + mdInput + ",                                                 ");
                query.AppendLine(" sysdate)                                                         ");

                crud.dac.IUExcuteWithQuery(query.ToString());
            }

        }
        private void UpdateQCCount()
        {
            int v_count = 0;
            StringBuilder queryCount = new StringBuilder();
            queryCount.AppendLine("");
            queryCount.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_SENSOR_COUNT WHERE                           ");
            queryCount.AppendLine("       IP_ADDRESS = '" + ipAddress + "'                                      ");
            queryCount.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
            queryCount.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
            queryCount.AppendLine(" AND   TIME_HOUR = '" + DateTime.Now.ToString("HH") + "'                     ");
            queryCount.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(queryCount.ToString());
            if (dt.Rows.Count > 0)
            {
                v_count = Convert.ToInt32(dt.Rows[0][0].ToString());
            }

            if (v_count == 0)
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("");
                query.AppendLine("INSERT INTO MES.TRTB_M_SENSOR_COUNT (                             ");
                query.AppendLine("   MES_MODEL, IP_ADDRESS, DEPT_CODE,                              ");
                query.AppendLine("   C_LINE, TIME_HOUR, TIME_DATE,                                  ");
                query.AppendLine("   SENSOR_MD, SENSOR_QC, MD_QC_INPUT,                             ");
                query.AppendLine("   IN_DT)                                                         ");
                query.AppendLine("VALUES ( '" + btnModel.Text + "',                                 ");
                query.AppendLine(" '" + ipAddress + "',                                             ");
                query.AppendLine(" '" + spDeptCode + "',                                            ");
                query.AppendLine(" '" + C_Line + "',                                                ");
                query.AppendLine(" '" + DateTime.Now.ToString("HH") + "',                           ");
                query.AppendLine(" '" + DateTime.Now.ToString("yyyyMMdd") + "',                     ");
                query.AppendLine(" '" + txtSensorCount.Text + "',                                   ");
                query.AppendLine(" '" + txtPassQC.Text + "',                                        ");
                query.AppendLine(" '" + txtMetalDetect.Text + "',                                   ");
                query.AppendLine(" sysdate)                                                         ");

                crud.dac.IUExcuteWithQuery(query.ToString());
            }
            else
            {
                StringBuilder queryUpdate = new StringBuilder();
                queryUpdate.AppendLine("");
                queryUpdate.AppendLine("UPDATE  MES.TRTB_M_SENSOR_COUNT                                              ");
                queryUpdate.AppendLine("SET SENSOR_MD = " + txtSensorCount.Text + ",                                 ");
                queryUpdate.AppendLine(" SENSOR_QC = " + txtPassQC.Text + ",                                         ");
                queryUpdate.AppendLine(" MD_QC_INPUT = " + txtMetalDetect.Text + "                                   ");
                queryUpdate.AppendLine(" WHERE                                                                       ");
                queryUpdate.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                                            ");
                queryUpdate.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
                queryUpdate.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
                queryUpdate.AppendLine(" AND   TIME_HOUR = '" + DateTime.Now.ToString("HH") + "'                     ");
                queryUpdate.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
                crud.dac.IUExcuteWithQuery(queryUpdate.ToString());
            }
        }
        private void GetAllValueOfSensor()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("SELECT SUM(SENSOR_MD)SENSOR_MD,                           ");
            query.AppendLine("SUM(SENSOR_QC)SENSOR_QC                                   ");
            query.AppendLine(",SUM(MD_QC_INPUT)MD_QC_INPUT                              ");
            query.AppendLine("FROM MES.TRTB_M_SENSOR_COUNT                                  ");
            query.AppendLine("WHERE                                                     ");
            //query.AppendLine(" MES_MODEL = '" + btnModel.Text + "'                       ");
            query.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                         ");
            query.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                   ");
            query.AppendLine(" AND   C_LINE = '" + C_Line + "'                          ");
            query.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            query.AppendLine("GROUP BY IP_ADDRESS,DEPT_CODE,C_LINE                      ");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt.Rows.Count > 0)
            {
                //this.txtMetalDetect.Text = dt.Rows[0]["MD_QC_INPUT"].ToString();
                MetalCount = Convert.ToInt32(dt.Rows[0]["MD_QC_INPUT"].ToString());
                finishedCount = Convert.ToInt32(dt.Rows[0]["SENSOR_QC"].ToString());
                sensorCount = Convert.ToInt32(dt.Rows[0]["SENSOR_MD"].ToString());
            }
        }
        //private void dockPanel1_Collapsed(object sender, DevExpress.XtraBars.Docking.DockPanelEventArgs e)
        //{
        //    if (cameraControl1.Device != null)
        //    {
        //        cameraControl1.Device.Stop();
        //    }
        //}
        private void lblPart_Move(object sender, EventArgs e)
        {
            if (MoveControl == true)
            {
                Label lbl = (Label)sender;
                if (lbl.Name == "lblPart1")
                {
                    lblPartCount1.Top = lbl.Top + (lbl.Height / 2);
                    lblPartCount1.Left = lbl.Left + (lbl.Width + lbl.Width / 4);
                }
                else if (lbl.Name == "lblPart2")
                {
                    lblPartCount2.Top = lbl.Top + (lbl.Height / 2);
                    lblPartCount2.Left = lbl.Left + (lbl.Width + lbl.Width / 4);
                }
                else if (lbl.Name == "lblPart3")
                {
                    lblPartCount3.Top = lbl.Top + (lbl.Height / 2);
                    lblPartCount3.Left = lbl.Left + (lbl.Width + lbl.Width / 4);
                }
                else if (lbl.Name == "lblPart4")
                {
                    lblPartCount4.Top = lbl.Top + (lbl.Height / 2);
                    lblPartCount4.Left = lbl.Left + (lbl.Width + lbl.Width / 4);
                }
            }

        }
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //GetProdInformation();
            //SetTouchCount();
            //GetAllValueOfSensor();
            //backgroundWorker1.ReportProgress(100);
        }

        private void fakeRFT_Tick(object sender, EventArgs e)
        {
            int qcCount = 0;
            int mdCount = 0;
            int mdInput = 0;
            int v_count = 0;
            int checkNDCount = 0;
            StringBuilder queryCount = new StringBuilder();
            queryCount.AppendLine("");
            queryCount.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_SENSOR_COUNT WHERE                           ");
            queryCount.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                                            ");
            queryCount.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
            queryCount.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
            queryCount.AppendLine(" AND   TIME_HOUR = '" + DateTime.Now.ToString("yyyyMMddHHmmss") + "'         ");
            queryCount.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(queryCount.ToString());


            StringBuilder queryCount2 = new StringBuilder();
            setProductionAtTime();
            queryCount2.AppendLine("");
            queryCount2.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_SENSOR_COUNT WHERE                           ");
            queryCount2.AppendLine(" IP_ADDRESS = '" + ipAddress + "'                                            ");
            queryCount2.AppendLine(" AND   DEPT_CODE = '" + spDeptCode + "'                                      ");
            queryCount2.AppendLine(" AND   C_LINE = '" + C_Line + "'                                             ");
            queryCount2.AppendLine(" AND   TIME_HOUR LIKE '" + DateTime.Now.ToString("yyyyMMddHH") + "%'         ");
            queryCount2.AppendLine(" AND   TIME_DATE = '" + DateTime.Now.ToString("yyyyMMdd") + "'               ");
            DataTable dt2 = new DataTable();
            dt2 = crud.dac.DtSelectExcuteWithQuery(queryCount2.ToString());

            if (dt.Rows.Count > 0)
            {
                v_count = Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            if (dt2.Rows.Count > 0)
            {
                checkNDCount = Convert.ToInt32(dt2.Rows[0][0].ToString());
            }
            if (checkNDCount > 0)
            {
                Decimal a = Convert.ToDecimal((ProdHour * 1.0) / (checkNDCount * 1.0));
                if (a >= Convert.ToDecimal(0.75))
                {
                    if (v_count == 0)
                    {
                        StringBuilder query = new StringBuilder();
                        query.AppendLine("");
                        query.AppendLine("INSERT INTO MES.TRTB_M_SENSOR_COUNT (                             ");
                        query.AppendLine("   MES_MODEL, IP_ADDRESS, DEPT_CODE,                              ");
                        query.AppendLine("   C_LINE, TIME_HOUR, TIME_DATE,                                  ");
                        query.AppendLine("   SENSOR_MD, SENSOR_QC, MD_QC_INPUT,                             ");
                        query.AppendLine("   IN_DT)                                                         ");
                        query.AppendLine("VALUES ( '" + btnModel.Text + "',                                 ");
                        query.AppendLine(" '" + ipAddress + "',                                             ");
                        query.AppendLine(" '" + spDeptCode + "',                                            ");
                        query.AppendLine(" '" + C_Line + "',                                                ");
                        query.AppendLine(" '" + DateTime.Now.ToString("yyyyMMddHHmmss") + "',               ");
                        query.AppendLine(" '" + DateTime.Now.ToString("yyyyMMdd") + "',                     ");
                        query.AppendLine(" 1,                                                 ");
                        query.AppendLine(" " + qcCount + ",                                                 ");
                        query.AppendLine(" " + mdInput + ",                                                 ");
                        query.AppendLine(" sysdate)                                                         ");

                        crud.dac.IUExcuteWithQuery(query.ToString());
                    }
                }
            }



        }
        private void txtStatus_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Now You Can Move Control");
            MoveControl = true;
        }

        void setDataProduction()
        {
            Production();
            SetTouchCount();
            GetProdInformation();
        }

        private void BindProdPlanRate()
        {
            try
            {
                StringBuilder query = new StringBuilder();


                query.AppendLine("               SELECT BRAND,                                                                  ");
                query.AppendLine("                      C_LINE,                                                                 ");
                query.AppendLine("                      C_SHIFT,                                                                ");
                query.AppendLine("                      Q_CAPA,                                                                 ");
                query.AppendLine("                      TTL_QTY,                                                                ");
                query.AppendLine("                      RUNING_RATE,                                                            ");
                query.AppendLine("                      WORKING_HOUR,                                                           ");
                query.AppendLine("                      Q_CAPA_CUR,                                                             ");
                query.AppendLine("                      CASE                                                                    ");
                query.AppendLine("         WHEN Q_CAPA_CUR > 0 THEN ROUND((TTL_QTY / Q_CAPA_CUR) *100, 1)                       ");
                query.AppendLine("         ELSE 0                                                                               ");
                query.AppendLine("      END                                                                                     ");
                query.AppendLine("         RATE_CUR                                                                             ");
                query.AppendLine(" FROM(SELECT BRAND,                                                                           ");
                query.AppendLine("              C_LINE,                                                                         ");
                query.AppendLine("              C_SHIFT,                                                                        ");
                query.AppendLine("              Q_CAPA,                                                                         ");
                query.AppendLine("              TTL_QTY,                                                                        ");
                query.AppendLine("              RUNING_RATE,                                                                    ");
                query.AppendLine("              ROUND(                                                                          ");
                query.AppendLine("                 CASE                                                                         ");
                query.AppendLine("                    WHEN TO_CHAR(SYSDATE, 'HH24MI') > '1230'                                  ");
                query.AppendLine("                    THEN                                                                      ");
                query.AppendLine("                           24                                                                 ");
                query.AppendLine("                         * (SYSDATE                                                           ");
                query.AppendLine("                            - TO_DATE(                                                        ");
                query.AppendLine("                                 TO_CHAR(SYSDATE, 'YYYYMMDD') || '073000',                    ");
                query.AppendLine("                                 'YYYYMMDDHH24MISS'))                                         ");
                query.AppendLine("                       - 1                                                                    ");
                query.AppendLine("                    ELSE                                                                      ");
                query.AppendLine("                           24                                                                 ");
                query.AppendLine("                         * (SYSDATE                                                           ");
                query.AppendLine("                            - TO_DATE(                                                        ");
                query.AppendLine("                                 TO_CHAR(SYSDATE, 'YYYYMMDD') || '073000',                    ");
                query.AppendLine("                                 'YYYYMMDDHH24MISS'))                                         ");
                query.AppendLine("                 END,                                                                         ");
                query.AppendLine("                 1)                                                                           ");
                query.AppendLine("                 WORKING_HOUR,                                                                ");
                query.AppendLine("              ROUND(                                                                          ");
                query.AppendLine("                   Q_CAPA                                                                     ");
                query.AppendLine("                 / 8                                                                          ");
                query.AppendLine("                 * ROUND(                                                                     ");
                query.AppendLine("                      CASE                                                                    ");
                query.AppendLine("                         WHEN TO_CHAR(SYSDATE, 'HH24MI') > '1230'                             ");
                query.AppendLine("                         THEN                                                                 ");
                query.AppendLine("                                24                                                            ");
                query.AppendLine("                              * (SYSDATE                                                      ");
                query.AppendLine("                                 - TO_DATE(                                                   ");
                query.AppendLine("                                         TO_CHAR(SYSDATE, 'YYYYMMDD')                         ");
                query.AppendLine("                                      || '073000',                                            ");
                query.AppendLine("                                      'YYYYMMDDHH24MISS'))                                    ");
                query.AppendLine("                            - 1                                                               ");
                query.AppendLine("                         WHEN TO_CHAR(SYSDATE, 'HH24MI') > '1630'  THEN 8                     ");
                query.AppendLine("                         ELSE                                                                 ");
                query.AppendLine("                                24                                                            ");
                query.AppendLine("                              * (SYSDATE                                                      ");
                query.AppendLine("                                 - TO_DATE(                                                   ");
                query.AppendLine("                                         TO_CHAR(SYSDATE, 'YYYYMMDD')                         ");
                query.AppendLine("                                      || '073000',                                            ");
                query.AppendLine("                                      'YYYYMMDDHH24MISS'))                                    ");
                query.AppendLine("                      END,                                                                    ");
                query.AppendLine("                      1),                                                                     ");
                query.AppendLine("                 0)                                                                           ");
                query.AppendLine("                 Q_CAPA_CUR                                                                   ");
                query.AppendLine("         FROM V_STF_PLAN_PROD) WHERE C_LINE = '" + txtLineName.Text + "'                                                                ");
                DataTable dt = new DataTable();
                dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                if (dt.Rows.Count > 0)
                {
                    lblTTLPlan.Text = dt.Rows[0][3].ToString();
                    lblTTLProd.Text = dt.Rows[0][4].ToString();
                    lblTTLRate.Text = dt.Rows[0][5].ToString() + " %";

                    lblCurProd.Text = dt.Rows[0][4].ToString();
                    lblCurPlan.Text = dt.Rows[0][7].ToString();
                    lblCurRate.Text = dt.Rows[0][8].ToString() + " %";

                    decimal a = 0;
                    decimal.TryParse(dt.Rows[0][5].ToString(), out a);
                    decimal b = 0;
                    decimal.TryParse(dt.Rows[0][8].ToString(), out b);

                    if (a < 50)
                    {
                        lblTTLRate.ForeColor = Color.Red;
                        lblTTLProd.ForeColor = Color.Red;
                    }
                    else if (a >= 50 && a < 90)
                    {
                        lblTTLRate.ForeColor = Color.Yellow;
                        lblTTLProd.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        lblTTLProd.ForeColor = Color.Green;
                        lblTTLRate.ForeColor = Color.Green;
                    }



                    if (b < 50)
                    {
                        lblCurRate.ForeColor = Color.Red;
                        lblCurProd.ForeColor = Color.Red;
                    }
                    else if (b >= 50 && b < 90)
                    {
                        lblCurRate.ForeColor = Color.Yellow;
                        lblCurProd.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        lblCurProd.ForeColor = Color.Green;
                        lblCurRate.ForeColor = Color.Green;
                    }
                }
                label7.Text = "Remark : ";
                query = new StringBuilder();

                query.AppendLine(" select C_REMARKS from TRTB_M_REMARK_MANAGER where ymd in (                              ");
                query.AppendLine(" select max(ymd) from TRTB_M_REMARK_MANAGER                                              ");
                query.AppendLine(" WHERE TMU_MENU = 'TMU2163_1'  and LINE_CODE = '" + txtLineName.Text + "'                ");
                query.AppendLine(" and ymd >= to_char(sysdate - 30, 'yyyymmdd'))                                           ");
                query.AppendLine(" and TMU_MENU = 'TMU2163_1'  and LINE_CODE = '" + txtLineName.Text + "'                  ");
                query.AppendLine(" and ymd >= to_char(sysdate - 30, 'yyyymmdd')                                            ");


                DataTable dt2 = new DataTable();
                dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                if (dt.Rows.Count > 0)
                {
                    label7.Text = "Remark : " + dt.Rows[0]["C_REMARKS"].ToString();
                }
                else
                {
                    label7.Text = "Remark : ";
                }
            }
            catch (Exception ex)
            {

            }

        }
        private void timer3_Tick_1(object sender, EventArgs e)
        {
            setDataProduction();
            BindProdPlanRate();
            DataTable dt = new DataTable();

            if (productionData != null && productionData.Rows.Count > 0)
            {
                dt = productionData;

                //BindButton(productionData);
            }
            if (dt.Rows.Count > 0)
            {
                this.txtProdQty.Text = "SX : " + dt.Rows[0]["PROD_QTY"].ToString();
                if (dt.Rows[0]["PROD_QTY"].ToString() == "")
                {
                    tongProd = 0;
                }
                else
                {
                    tongProd = float.Parse(dt.Rows[0]["PROD_QTY"].ToString());
                }

                finishedCountScan = dt.Rows[0]["PROD_QTY"].ToString();
            }
            else
            {
                this.txtProdQty.Text = "SX : 0";
                tongProd = 0;
                finishedCountScan = "0";
            }
            txtReady.Text = "READY";

            if (btnModel.Text != "CHỌN MODEL" || btnModel.Text != "")
            {
                //UpdateExecuteSensor();
            }
        }
        private bool checkResolveLastNotify()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT IS_RESOLVE FROM MES.TRTB_M_BTS_STOPLINE_NOTIFY WHERE IP_ADDRESS = '" + ipAddress + "' AND C_DATE IN ( SELECT MAX(C_DATE) FROM MES.TRTB_M_BTS_STOPLINE_NOTIFY WHERE IP_ADDRESS = '" + ipAddress + "' )");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["IS_RESOLVE"].ToString() == "Y")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        private bool getLastNotify2()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT * FROM MES.TRTB_M_BTS_STOPLINE_NOTIFY WHERE IP_ADDRESS = '" + ipAddress + "' AND( C_DATE >= '" + DateTime.Now.AddMinutes(-10).ToString("yyyyMMddHHmmss") + "') ");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void turnGreen(bool onOff)
        {
            int selectID;
            bool result;
            if (onOff)
            {
                selectID = 0;
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,4,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, 4, 0, 0, 0);
            }
            else
            {
                selectID = 0;
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,-4,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, -4, 0, 0, 0);
            }
        }
        private void turnRedAndSound(bool onOff)
        {
            int selectID;
            int blink;
            bool result;
            selectID = 0;
            if (onOff)
            {
                selectID = Convert.ToInt32("261", 16);
                blink = Convert.ToInt32("5", 10) * 16;
                blink += Convert.ToInt32("5", 10);
                string.Format("usb_io_output(0x{0:x},0x{1:x},2,0,0,0)", selectID, blink);
                result = usb_io_output(selectID, blink, 2, 0, 0, 0);


                selectID = Convert.ToInt32("261", 16);
                blink = Convert.ToInt32("5", 10) * 16;
                blink += Convert.ToInt32("5", 10);
                string.Format("usb_io_output(0x{0:x},0x{1:x},1,0,0,0)", selectID, blink);
                result = usb_io_output(selectID, blink, 1, 0, 0, 0);
                if (!result)
                {
                    set_usb_events(this.Handle.ToInt32());

                }
            }
            else
            {
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,-1,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, -1, 0, 0, 0);

                Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,-2,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, -2, 0, 0, 0);
                if (!result)
                {
                    set_usb_events(this.Handle.ToInt32());

                }
            }

        }
        private void turnSoundOndOff(bool onOff)
        {
            int selectID;
            bool result;
            int blink;
            if (!onOff)
            {
                selectID = 0;
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,-1,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, -1, 0, 0, 0);
            }
            else
            {
                selectID = Convert.ToInt32("261", 16);
                blink = Convert.ToInt32("5", 10) * 16;
                blink += Convert.ToInt32("5", 10);
                string.Format("usb_io_output(0x{0:x},0x{1:x},1,0,0,0)", selectID, blink);
                result = usb_io_output(selectID, blink, 1, 0, 0, 0);
            }

        }
        private void turnYellowOnOff(bool onOff)
        {
            int selectID;
            bool result;

            if (onOff)
            {
                selectID = 0;
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,3,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, 3, 0, 0, 0);
            }
            else
            {
                selectID = 0;
                selectID = Convert.ToInt32("261", 16);
                string.Format("usb_io_output(0x{0:x},0,-3,0,0,0)", selectID);
                result = usb_io_output(selectID, 0, -3, 0, 0, 0);
            }
        }
        public string alarmGather;

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
        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();
            errorTouch.Rows.Clear();
            timerTouch.Enabled = false;

        }
        //private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
        //    progressBarControl1.EditValue = e.ProgressPercentage;
        //}
        //private void backgroundWorker2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    progressBarControl1.EditValue = 0;
        //}
        private void timerSyncOracle_Tick(object sender, EventArgs e)
        {
            //if (!backgroundWorker2.IsBusy)
            //{
            //    backgroundWorker2.RunWorkerAsync();
            //}
            //else
            //{
            //    backgroundWorker2.CancelAsync();
            //}
        }

        private async void txtTime_Click(object sender, EventArgs e)
        {
            await SendNotification("", "Hello", "hello", "");
        }
        public async Task SendNotification(string to, string title, string body, string tag)
        {
            HttpClient client = new HttpClient();
            try
            {
                var values = new Dictionary<string, string>
                    {
                        { "to", to },
                        { "title", title },
                        { "body", body },
                        { "tag", tag }
                    };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://192.168.1.7:8080/test/arduino/sendNotification", content);

                //return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                // return ex.Message.ToString();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void btn_reasonCode1_Click(object sender, EventArgs e)
        {
            string onoff = "on";
            try
            {

                if (btn_reasonCode1.Text.ToString().Contains("Waiting"))
                {
                    onoff = "off";
                    //TurnOffAndon("R");
                    //var result = TurnOffAndonAsync("R");
                    ThreadSafe(() =>
                    {
                        btn_reasonCode1.Text = "(Andon) Gọi QA ";
                        timer_BlinkButtonRed.Enabled = false;
                        btn_reasonCode1.BackColor = System.Drawing.Color.DarkRed;
                        //btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.DarkRed;
                    });
                }
                else if (btn_reasonCode1.Text.ToString().Contains("Calling"))
                {
                    onoff = "off";
                    //TurnOffAndon("R");
                    var result = TurnOffAndonAsync("R");
                    ThreadSafe(() =>
                    {
                        btn_reasonCode1.Text = "(Andon) Gọi QA " + Environment.NewLine + "Waiting";
                        timer_BlinkButtonRed.Enabled = false;
                        btn_reasonCode1.BackColor = System.Drawing.Color.DarkRed;
                        //btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.DarkRed;
                    });
                }
                else
                {
                    //TurnOnAndon("R");
                    var result = TurnOnAndonAsync("R");
                    if (result.IsCompleted)
                    {
                        btn_reasonCode1.Text = "(Andon) Gọi QA " + Environment.NewLine + "Calling";
                        timer_BlinkButtonRed.Enabled = true;
                    }
                }
                using (TimedWebClient wc = new TimedWebClient { Timeout = 2000 })
                {
                    WriteAndonToLogFile(onoff, "3");
                    //TunrOnOfAlarmSound(false, "three", 1);
                    string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, RecievedIpaddress, onoff, "3");
                    //string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, "", onoff, "3");
                    var json = wc.DownloadString(a);
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine("Cannot turnoff ---- " + ex.Message);
                //lblMessage.Text = "Không gọi được " + ex.Message;
            }
        }

        private void btn_reasonCode2_Click(object sender, EventArgs e)
        {
            string onoff = "on";

            try
            {
                if (btn_reasonCode2.Text.ToString().Contains("Waiting"))
                {
                    onoff = "off";
                    //TurnOffAndon("Y");
                    //var result = TurnOffAndonAsync("Y");

                    ThreadSafe(() =>
                    {
                        btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì";
                        timer_BlinkButtonYellow.Enabled = false;
                        btn_reasonCode2.BackColor = System.Drawing.Color.Orange;
                        //btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.Orange;
                    });
                }
                else if (btn_reasonCode2.Text.ToString().Contains("Calling"))
                {
                    onoff = "off";
                    //TurnOffAndon("R");
                    var result = TurnOffAndonAsync("Y");
                    ThreadSafe(() =>
                    {
                        btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì " + Environment.NewLine + "Waiting";
                        timer_BlinkButtonYellow.Enabled = false;
                        btn_reasonCode2.BackColor = System.Drawing.Color.DarkRed;
                        //btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.DarkRed;
                    });
                }
                else
                {

                    var result = TurnOnAndonAsync("Y");
                    if (result.IsCompleted)
                    {
                        btn_reasonCode2.Text = "(Andon) Gọi Bảo Trì" + Environment.NewLine + "Calling";
                        timer_BlinkButtonYellow.Enabled = true;
                    }

                    //TurnOnAndon("Y");
                }
                using (TimedWebClient wc = new TimedWebClient { Timeout = 2000 })
                {
                    WriteAndonToLogFile(onoff, "2");
                    string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, RecievedIpaddress, onoff, "2");
                    //string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, "", onoff, "2");
                    //TunrOnOfAlarmSound(false, "two", 1);
                    var json = wc.DownloadString(a);
                }
            }
            catch (Exception ex)
            {

                //lblMessage.Text = "Không gọi được " + ex.Message;
            }
        }

        private void btn_reasonCode3_Click(object sender, EventArgs e)
        {
            string onoff = "on";
            try
            {
                if (btn_reasonCode3.Text.ToString().Contains("Waiting"))
                {
                    onoff = "off";
                    //TurnOffAndon("G");
                    //var result = TurnOffAndonAsync("G");

                    ThreadSafe(() =>
                    {
                        btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất";
                        timer_BlinkButtonGreen.Enabled = false;
                        btn_reasonCode3.BackColor = System.Drawing.Color.DarkGreen;
                        //btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.DarkGreen;
                    });
                }
                else if (btn_reasonCode3.Text.ToString().Contains("Calling"))
                {
                    onoff = "off";
                    //TurnOffAndon("R");
                    var result = TurnOffAndonAsync("G");
                    ThreadSafe(() =>
                    {
                        btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Waiting";
                        timer_BlinkButtonGreen.Enabled = false;
                        btn_reasonCode3.BackColor = System.Drawing.Color.DarkGreen;
                        //btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.DarkGreen;
                    });
                }
                else
                {
                    var result = TurnOnAndonAsync("G");
                    if (result.IsCompleted)
                    {
                        btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Calling";
                        timer_BlinkButtonGreen.Enabled = true;
                    }

                    //TurnOnAndon("G");
                }
                using (TimedWebClient wc = new TimedWebClient { Timeout = 2000 })
                {
                    string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, RecievedIpaddress, onoff, "1");
                    //string a = String.Format("http://192.168.1.7:8080/test/ARDUINO/executeAndonOnOff?ipadd='{0}'&comname={1}&recieveip='{2}'&onoff={3}&reasoncd={4}", ipAddress, Comname, "", onoff, "1");
                    //TunrOnOfAlarmSound(false, "one", 1);
                    var json = wc.DownloadString(a);
                }
            }
            catch (Exception ex)
            {

                //lblMessage.Text = "Không gọi được " + ex.Message;
            }
        }
        public async Task<bool> TurnOffAllAlarm()
        {
            bool result = false;

            if (MQTTConnected)
            {
                result = await MQTT.Main.PublishAsync("R_OFF", true, 1);
                result = await MQTT.Main.PublishAsync("Y_OFF", true, 1);
                result = await MQTT.Main.PublishAsync("G_OFF", true, 1);
            }
            return result;
        }
        private async Task<bool> TurnOffAndonAsync(string lightColor)
        {
            bool result = false;

            if (MQTTConnected)
            {
                if (lightColor == "R")
                {
                    result = await MQTT.Main.PublishAsync("R_OFF", true, 1);

                }
                if (lightColor == "Y")
                {
                    result = await MQTT.Main.PublishAsync("Y_OFF", true, 1);
                }
                if (lightColor == "G")
                {
                    result = await MQTT.Main.PublishAsync("G_OFF", true, 1);
                }
            }
            return result;
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
        private void ThreadSafe(MethodInvoker method)
        {
            try
            {
                if (InvokeRequired) Invoke(method);
                else method();
            }
            catch (ObjectDisposedException) { }
        }
        private async Task<bool> TurnOnAndonAsync(string lightColor)
        {
            bool result = false;

            if (MQTTConnected)
            {
                if (lightColor == "R")
                {
                    result = await MQTT.Main.PublishAsync("R_ON", true, 1);
                }
                if (lightColor == "Y")
                {
                    result = await MQTT.Main.PublishAsync("Y_ON", true, 1);
                }
                if (lightColor == "G")
                {
                    result = await MQTT.Main.PublishAsync("G_ON", true, 1);
                }
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

        private void timer_BlinkButtonRed_Tick(object sender, EventArgs e)
        {
            if (btn_reasonCode1.BackColor == System.Drawing.Color.DarkRed)
            {
                this.btn_reasonCode1.Invoke(new Action(() =>
                {
                    btn_reasonCode1.BackColor = System.Drawing.Color.IndianRed;
                    //btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.IndianRed;
                }));

            }
            else
            {
                this.btn_reasonCode1.Invoke(new Action(() =>
                {
                    btn_reasonCode1.BackColor = System.Drawing.Color.DarkRed;
                    //btn_reasonCode1.Appearance.BackColor2 = System.Drawing.Color.DarkRed;
                }));

            }
        }

        private void timer_BlinkButtonYellow_Tick(object sender, EventArgs e)
        {
            if (btn_reasonCode2.BackColor == System.Drawing.Color.Orange)
            {
                this.btn_reasonCode2.Invoke(new Action(() =>
                {
                    btn_reasonCode2.BackColor = System.Drawing.Color.OrangeRed;
                    //btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.OrangeRed;
                }));

            }
            else
            {
                this.btn_reasonCode2.Invoke(new Action(() =>
                {
                    btn_reasonCode2.BackColor = System.Drawing.Color.Orange;
                    //btn_reasonCode2.Appearance.BackColor2 = System.Drawing.Color.Orange;
                }));

            }
        }

        private void timer_BlinkButtonGreen_Tick(object sender, EventArgs e)
        {
            if (btn_reasonCode3.BackColor == System.Drawing.Color.ForestGreen)
            {
                this.btn_reasonCode3.Invoke(new Action(() =>
                {
                    btn_reasonCode3.BackColor = System.Drawing.Color.LightGreen;
                    //btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.LightGreen;
                }));

            }
            else
            {
                this.btn_reasonCode3.Invoke(new Action(() =>
                {
                    btn_reasonCode3.BackColor = System.Drawing.Color.ForestGreen;
                    //btn_reasonCode3.Appearance.BackColor2 = System.Drawing.Color.ForestGreen;
                }));

            }
        }

        private void pictureEdit2_EditValueChanged(object sender, EventArgs e)
        {

        }

        private void pictureEdit2_Click(object sender, EventArgs e)
        {
            processStopLine("ABC", "123", "123", "OK");
        }
        private async void processStopLine(string partname, string partid, string reasonid, string reason)
        {
            await Task.Run(() =>
            {
                if (InsertIntoLogHistory(reasonid))
                {
                    //SetAlarm(lblLineInfo.Text + " Stop line", "Part : " + partname + " Defect " + reason + " now over 6 times. Please check it", 10, reason);
                    SetAlarm(txtLineName.Text + " Stop line ", " Reason : " + reason + ". Please check it", 10, reason);
                    var resultStoplineOn = TurnOnStopLineAsync();
                    if (resultStoplineOn.IsCompleted)
                    {
                        var resultAndonOn = TurnOnAndonAsync("R");
                    }

                    ThreadSafe(() =>
                    {
                        using (frmTMC7032_MsgAlarm alarm = new frmTMC7032_MsgAlarm())
                        {
                            alarm.IPADDRESS = ipAddress;
                            alarm.IsStopLine = true;
                            alarm.MessageText = txtLineName.Text + " Stop line " + " Reason : " + reason + ". Please check it";
                            alarm.ShowDialog(this);
                        }
                    });

                    isstartcountreason = false;
                    var resultStoplineOff = TurnOffStopLineAsync();
                    if (resultStoplineOff.IsCompleted)
                    {
                        var resultAndonOff = TurnOffAndonAsync("R");
                    }
                }
            });
        }
        private void SetAlarm(string header, string message, int longtime, string reasonID)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("SELECT * FROM                                                                ");
            query.AppendLine("TRTB_M_BTS_STOPLINE_NOTIFY                                                   ");
            query.AppendLine("WHERE C_DATE = TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS')                       ");
            query.AppendLine("AND C_LINE = '" + txtLineName.Text + "'                                      ");

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
                query.AppendLine("VALUES ( '" + txtLineName.Text + "',                                                              ");
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
        private bool InsertIntoLogHistory(string reasonID)
        {
            StringBuilder query = new StringBuilder();
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery("SELECT MAX(SEQ) + 1 CNT FROM MES.TRTB_BTS_LOG_REASON_HISTORY ");

            if (dt.Rows.Count > 0)
            {
                if (ipAddress == "192.168.1.158")
                {
                    C_Line = "NSA1";
                    LineName = "P101";
                }

                int seq = Convert.ToInt32(dt.Rows[0]["CNT"].ToString());

                query = new StringBuilder();
                query.AppendLine("                INSERT INTO MES.TRTB_BTS_LOG_REASON_HISTORY (                                  ");
                query.AppendLine("   SEQ, D_GATHER, C_LINE,                                                                      ");
                query.AppendLine("   LOCK_TIME, IS_LOCK,                                                                         ");
                query.AppendLine("   REASON_ID)                                                                                  ");
                query.AppendLine("SELECT " + seq + ",a.D_GATHER,'" + C_Line + "',SYSDATE,'Y',A.REASON_ID                         ");
                query.AppendLine("    FROM TRTB_M_BTS_COUNT3 A, MES.TRTB_M_BTS_REASON3@inf_m_e B                                 ");
                query.AppendLine("   WHERE     A.REASON_ID = B.REASON_ID                                                         ");
                query.AppendLine("         AND SUBSTR (A.D_GATHER, 1, 8) = TO_CHAR (SYSDATE, 'YYYYMMDD')                         ");
                query.AppendLine("         AND A.C_LINE IN ( '" + C_Line + "','" + LineName + "' )                               ");
                query.AppendLine("         AND A.D_GATHER <= TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS')                               ");
                query.AppendLine("         AND A.D_GATHER > TO_CHAR (SYSDATE - (0.5 / 24), 'YYYYMMDDHH24MISS')                   ");
                query.AppendLine("         AND A.REASON_ID = " + reasonID + " AND DEPT_CODE = 'STF'                              ");

                crud.dac.IUExcuteWithQuery(query.ToString());
                query = new StringBuilder();


                query.AppendLine(" insert into MES.TRTB_ANDON_SYSTEM_LOG(D_GATHER, IP_ADDRESS, LINE_NM,");
                query.AppendLine(" IP_ADDRESS_CALL, TIME_START_CALL, ");
                query.AppendLine(" REASON_CODE, INPUT_DESCRIPTION, MSG_STATUS) values( ");
                query.AppendLine(" to_char(sysdate, 'YYYYMMDDHH24MISS'), '" + ipAddress + "', 'STF' || '" + C_Line + "', ");
                query.AppendLine("    '" + ipAddress + "', SYSDATE, ");
                query.AppendLine("    5, '', 'NEW') ");

                crud.dac.IUExcuteWithQuery(query.ToString());

                //Console.WriteLine($"TRTB_BTS_LOG_REASON_HISTORY: SEQ: {seq}, reasonID: {reasonID}, spLine: {spLine}");
                //Console.WriteLine($"TRTB_ANDON_SYSTEM_LOG: ipAddress: {ipAddress}, RecievedIpaddress: {RecievedIpaddress}, spLine: {spLine}");

                return true;
            }
            return false;
        }

        private void timer_CheckStopLine_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorkerStopLine.IsBusy)
            {
                backgroundWorkerStopLine.RunWorkerAsync();
            }
        }

        private void backgroundWorkerStopLine_DoWork(object sender, DoWorkEventArgs e)
        {
            checkAlarmReturn();
            GetDataStopLine();
        }

        private void backgroundWorkerStopLine_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }
        private DataTable GetDataStopLine()
        {
            dtStopLine = new DataTable();
            StringBuilder query = new StringBuilder();

            // 20241220 ĐÓNG LẠI 
            if (txtLineName.Text == "P107")
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
                query.AppendLine("         AND C.DEPT_CODE = 'STF'                                                                                                         ");
                query.AppendLine("         AND A.C_LINE IN ( '" + C_Line + "','" + LineName + "' )                                                                         ");
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
        private DataTable checkAlarmReturn()
        {
            DataTable dt = new DataTable();
            StringBuilder query = new StringBuilder();

            if (ipAddress == "192.168.1.158")
            {
                C_Line = "NSA1";
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
            query.AppendLine("         AND A.C_LINE IN ( '" + C_Line + "','" + LineName + "' )                                                   ");
            query.AppendLine("         AND A.D_GATHER <= TO_CHAR (SYSDATE, 'YYYYMMDDHH24MISS')                                                   ");
            query.AppendLine("         AND A.D_GATHER > TO_CHAR (SYSDATE - (0.5 / 24), 'YYYYMMDDHH24MISS')                                         ");
            query.AppendLine(" AND (A.D_GATHER,A.C_LINE,A.REASON_ID) NOT IN (  SELECT D_GATHER,C_LINE,REASON_ID FROM TRTB_BTS_LOG_REASON_HISTORY ");
            query.AppendLine(" WHERE D_GATHER > TO_CHAR (SYSDATE, 'YYYYMMDD') || '000000')                                                       ");
            query.AppendLine("GROUP BY A.REASON_ID, REASON_EN, REASON_VN                                                                         ");

            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            dtAlarmReturn = dt;
            return dtAlarmReturn;
        }
        private void ConffigErrorButtonColor(string id, System.Drawing.Color c)
        {
            foreach (var a in tableLayoutError1.Controls)
            {
                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
                {
                    Button btnID = (Button)a;
                    if (btnID.AccessibleName == id)
                    {
                        btnID.BackColor = c;
                        //btnID.Appearance.BackColor2 = c;
                    }
                }
            }
            foreach (var a in tableLayoutError.Controls)
            {
                if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
                {
                    Button btnID = (Button)a;
                    if (btnID.AccessibleName == id)
                    {
                        btnID.BackColor = c;
                        //btnID.Appearance.BackColor2 = c;
                    }
                }
            }

        }
        private void TurnOnStopLine(string partname, string partid, string reasonid, string reason)
        {

            //processStopLine( partname,  partid,  reasonid,  reason);
            if (txtLineName.Text == "P107")
            {
                processStopLine("Sensor Stop Line", "", "1", "Sensor Stop Line Over 20 Pairs per hour");
            }
            else
            {
                processStopLine(partname, partid, reasonid, reason);
            }

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
                    if (txtLineName.Text == "P107")
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

        private void txtDefectQty_Click(object sender, EventArgs e)
        {

        }

        private void lblError1_Click(object sender, EventArgs e)
        {

        }

        private void txtMessage_Click(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bm = null;
                Popuppdf pdf = new Popuppdf();
                string path = ("ftp://" + etc.FileServerPath + @"/Mes/BTS/PDF/Go-No_Go_Standard.pdf");
                //pdf.pdfid = "1";
                pdf.filepath = path;
                pdf.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải file PDF. Rớt mạng!!!\n" + ex.Message, "Rớt mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void labelControl2_Click(object sender, EventArgs e)
        {

        }

        private void lblTTLPlan_Click(object sender, EventArgs e)
        {

        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void lblPart1_AutoSizeChanged(object sender, EventArgs e)
        {

        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (_voiceEngine == null)
            {
                ShowMessage("VoiceWhisper chưa sẵn sàng.", Color.Red);
                return;
            }

            // Toggle: nếu đang chạy thì dừng
            if (_autoLoopCts != null)
            {
                _autoLoopCts.Cancel();
                _autoLoopCts = null;
                _voiceEngine.IsAutoLoopMode = false;
                //btnVoiceWhisper.Text      = "🎤";
                btnVoiceWhisper.BackColor = SystemColors.Control;
                ShowMessage("[🛑 Auto Voice] Đã dừng. Đợi kết quả cuối...", Color.Gray);
                _ = Task.Delay(3000).ContinueWith(_ => VlogFinalize());
                return;
            }

            // Bắt đầu vòng lặp auto
            RefreshVoiceCommandDefinitions();
            _voiceEngine.IsAutoLoopMode = true;
            _autoLoopCts = new CancellationTokenSource();
            //btnVoiceWhisper.Text      = "⏹ DỮNG VOICE";
            btnVoiceWhisper.BackColor = Color.OrangeRed;

            // Mở log file — append vào file theo ngày, không tạo file mới mỗi lần chạy
            try
            {
                System.IO.Directory.CreateDirectory(VoiceLogDir);
                _vlogPath = System.IO.Path.Combine(VoiceLogDir, $"A36(thường)_voiceLog.log");
                _vlogSeq = 0;
                _vlog = new System.IO.StreamWriter(_vlogPath, append: true, System.Text.Encoding.UTF8);
                _vlog.WriteLine(new string('=', 80));
                _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ================ Session bắt đầu ================");
                _vlog.Flush();
            }
            catch { }

            ShowMessage("[▶ Auto Voice] Bắt đầu thu âm tự động...", Color.Green);

            var token = _autoLoopCts.Token;
            _ = RunVoiceAutoLoopAsync(token);
        }
        private async Task RunVoiceAutoLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (this.InvokeRequired)
                        this.Invoke(new Action(() => ShowMessage("🔴 Đang nghe... (tối đa 5 giây)", Color.OrangeRed)));
                    else
                        ShowMessage("🔴 Đang nghe... (tối đa 5 giây)", Color.OrangeRed);

                    await _voiceEngine.StartSmartPushToTalkAsync(5000);

                    if (token.IsCancellationRequested) break;

                    // ── Đợi xử lý xong (engine chuyển về Ready) rồi nghỉ 5 giây ──
                    if (this.InvokeRequired)
                        this.Invoke(new Action(() => ShowMessage("⏸ Xử lý... sau đó nghỉ 5 giây", Color.Blue)));
                    else
                        ShowMessage("⏸ Xử lý... sau đó nghỉ 5 giây", Color.Blue);

                    await Task.Delay(5000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (this.InvokeRequired)
                        this.Invoke(new Action(() => ShowMessage("Voice lỗi: " + ex.Message, Color.Red)));
                    else
                        ShowMessage("Voice lỗi: " + ex.Message, Color.Red);
                    await Task.Delay(2000); // nếu lỗi thì đợi 2s rồi thử lại
                }
            }

            // Cleanup sau khi dừng
            _voiceEngine.IsAutoLoopMode = false;
            if (this.InvokeRequired)
                this.Invoke(new Action(() =>
                {
                    //btnVoiceWhisper.Text      = "🎤";
                    btnVoiceWhisper.BackColor = SystemColors.Control;
                }));
            else
            {
                //btnVoiceWhisper.Text      = "🎤";
                btnVoiceWhisper.BackColor = SystemColors.Control;
            }
        }

        private async void InitializeVoiceEngine()
        {
            _voiceEngine = new VoiceEngine(0.78)
            {
                InputGain = 3.5,
                RmsVoiceThreshold = 0.004,
                VadVoiceFrameRatio = 0.25,
                MinimumVoiceDurationMs = 120,
                SilenceAfterVoiceDurationMs = 1200,
                EmitBatchCommandResult = true
            };
            _voiceEngine.CommandRecognized += _voiceEngine_CommandRecognized;
            _voiceEngine.StateChanged += _voiceEngine_StateChanged;
            _voiceEngine.LogMessage += _voiceEngine_LogMessage;
            RefreshVoiceCommandDefinitions();

            try
            {
                await _voiceEngine.InitializeAsync();
                ShowMessage("✓ Voice đã sẵn sàng.", Color.Green);
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi khởi tạo VoiceEngine: " + ex.Message, Color.Red);
            }
        }

        private void _voiceEngine_LogMessage(object sender, string e)
        {
            // File A36 chỉ giữ log kết quả dạng ngắn gọn; engine log chỉ giữ lỗi nghiêm trọng.
            if (string.IsNullOrWhiteSpace(e)) return;
            if (e.StartsWith("===") || e.StartsWith("---")) return;
            if (!e.StartsWith("[Voice Error]")) return;
            lock (_vlogLock)
            {
                if (_vlog != null)
                {
                    _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ENGINE : {e}");
                    _vlog.Flush();
                }
            }
        }

        private void _voiceEngine_StateChanged(object sender, VoiceEngineState e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => _voiceEngine_StateChanged(sender, e)));
                return;
            }

            switch (e)
            {
                case VoiceEngineState.Ready:
                    btnVoiceWhisper.Enabled = true;
                    break;
                case VoiceEngineState.Recording:
                    btnVoiceWhisper.Enabled = false;
                    break;
                case VoiceEngineState.Processing:
                    btnVoiceWhisper.Enabled = false;
                    break;
                case VoiceEngineState.Error:
                    btnVoiceWhisper.Enabled = true;
                    if (!string.IsNullOrWhiteSpace(_voiceEngine?.LastErrorMessage))
                    {
                        ShowMessage("Voice Engine loi: " + _voiceEngine.LastErrorMessage, Color.Red);
                        break;
                    }
                    ShowMessage("⚠ Voice Engine lỗi!", Color.Red);
                    break;
            }
        }

        private void _voiceEngine_CommandRecognized(object sender, VoiceMatchResult e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ProcessVoiceCommand(e)));
            }
            else
            {
                ProcessVoiceCommand(e);
            }
        }

        private void ProcessVoiceCommand(VoiceMatchResult result)
        {
            if (result?.MatchedCommand?.StartsWith("[Bỏ qua]") == true)
            {
                ShowMessage($"[Voice] Bỏ qua — '{result.RecognizedText}'", Color.Gray);
                VlogEntry(result, "[Bỏ qua]");
                return;
            }

            if (result?.ParsedCommands?.Any(c => c?.IsSuccess == true) == true)
            {
                foreach (var command in result.ParsedCommands.Where(c => c?.IsSuccess == true))
                {
                    if (!string.IsNullOrWhiteSpace(command.PartCode))
                        SelectPart(command.PartCode);

                    if (!string.IsNullOrWhiteSpace(command.ErrorCode))
                        SelectError(command.ErrorCode);

                    // ── Nếu không có action → tự động FAIL ──────────────────────
                    string action = string.IsNullOrWhiteSpace(command.ActionType)
                        ? "fail"
                        : command.ActionType;
                    ConfirmAction(action);
                }

                ShowMessage($"[Voice OK] {BuildVoiceResultSummary(result)}", Color.Green);
                VlogEntry(result, "OK");
                return;
            }

            if (result?.ParsedCommand?.IsSuccess == true)
            {
                VoiceCommandMatch command = result.ParsedCommand;

                if (!string.IsNullOrWhiteSpace(command.PartCode))
                    SelectPart(command.PartCode);

                if (!string.IsNullOrWhiteSpace(command.ErrorCode))
                    SelectError(command.ErrorCode);

                // ── Nếu không có action → tự động FAIL ──────────────────────────
                string action = string.IsNullOrWhiteSpace(command.ActionType)
                    ? "fail"
                    : command.ActionType;
                ConfirmAction(action);

                ShowMessage($"[Voice OK] {command.ToDisplayText()} ({command.ConfidenceScore:P0})", Color.Green);
                VlogEntry(result, "OK");
                return;
            }

            if (result.IsSuccess)
            {
                ShowMessage($"[Voice Match] Lệnh: {result.MatchedCommand} ({result.ConfidenceScore:P0})", Color.Green);
                VlogEntry(result, "Match");
            }
            else
            {
                ShowMessage($"[Voice Fail] Không nhận diện được. Nghe được: '{result.RecognizedText}'", Color.Red);
                VlogEntry(result, "Fail");
            }
        }

        private void VlogEntry(VoiceMatchResult r, string status)
        {
            lock (_vlogLock)
            {
                if (_vlog == null) return;
                _vlogSeq++;

                _vlog.WriteLine(BuildCompactVoiceLogLine(r, status));
                _vlog.Flush();
            }
        }

        private string BuildCompactVoiceLogLine(VoiceMatchResult r, string status)
        {
            string finalStatus = BuildVoiceFinalStatus(r, status);
            string elapsed = $"{r?.ProcessingTimeSec ?? 0:F2}s";

            if (IsNoSpeechLog(r))
            {
                return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] | Không nghe đc gì → Bỏ qua  ({elapsed})";
            }

            string heard = NormalizeVoiceLogText(r?.RecognizedText);
            string cleaned = NormalizeVoiceLogText(r?.CleanedText);
            if (cleaned == "Không nghe đc gì")
            {
                cleaned = heard;
            }

            return $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] | {heard} → {cleaned} → {BuildVoiceResultSummary(r)} → {finalStatus}  ({elapsed})";
        }

        private string BuildVoiceResultSummary(VoiceMatchResult r)
        {
            var commands = r?.ParsedCommands?.Where(c => c != null && c.IsSuccess).ToList()
                ?? new List<VoiceCommandMatch>();

            if (commands.Count == 0 && r?.ParsedCommand?.IsSuccess == true)
            {
                commands.Add(r.ParsedCommand);
            }

            string part = commands.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.PartCode))?.PartCode;
            string error = string.Join(",", commands
                .Where(c => !string.IsNullOrWhiteSpace(c.ErrorCode))
                .Select(c => c.ErrorCode)
                .Distinct());
            string action = commands.FirstOrDefault(c => !string.IsNullOrWhiteSpace(c.ActionType))?.ActionType;

            return $"part: {NoneIfEmpty(part)} | error:{NoneIfEmpty(error)} | action:{NoneIfEmpty(action)}";
        }

        private static string BuildVoiceFinalStatus(VoiceMatchResult r, string status)
        {
            string matched = r?.MatchedCommand ?? "";
            if (matched.StartsWith("[Bỏ qua]"))
            {
                if (matched.Contains("Không nghiệp vụ")) return "Bỏ qua: không nghiệp vụ";
                if (matched.Contains("Thiếu Action")) return "Bỏ qua: thiếu action";
                return "Bỏ qua";
            }

            if (status == "OK" || r?.ParsedCommand?.IsSuccess == true || r?.ParsedCommands?.Any(c => c?.IsSuccess == true) == true)
            {
                return "OK";
            }

            return status == "Fail" ? "Không khớp" : status;
        }

        private static string NormalizeVoiceLogText(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "Không nghe đc gì";
            string text = value.Trim();
            string compact = new string(text.Where(ch => !char.IsWhiteSpace(ch)).ToArray());
            if (compact.Length == 0) return "Không nghe đc gì";

            bool onlyPlaceholders = compact.All(ch =>
                ch == '-' || ch == '_' || ch == '.' || ch == ',' ||
                ch == '[' || ch == ']' || ch == '(' || ch == ')' || ch == '…');

            return onlyPlaceholders ? "Không nghe đc gì" : text;
        }

        private static bool IsNoSpeechLog(VoiceMatchResult r)
        {
            if (r == null) return true;
            string matched = r.MatchedCommand ?? "";
            if (matched.Contains("Whisper noise") || matched.Contains("Chỉ từ kết thúc")) return true;
            return NormalizeVoiceLogText(r.RecognizedText) == "Không nghe đc gì"
                && NormalizeVoiceLogText(r.CleanedText) == "Không nghe đc gì";
        }

        private static string NoneIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "none" : value;
        }

        private void VlogFinalize()
        {
            lock (_vlogLock)
            {
                if (_vlog == null) return;
                _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Session kết thúc — {_vlogSeq} lượt nhận diện.");
                _vlog.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ================ Het session ================");
                _vlog.Flush();
                _vlog.Close();
                _vlog = null;
            }
            string path = _vlogPath;
            if (path != null)
            {
                void ShowLog() => ShowMessage($"[Log] {System.IO.Path.GetFileName(path)}", Color.DimGray);
                if (this.InvokeRequired) this.Invoke(new Action(ShowLog)); else ShowLog();
            }
        }

        private void RefreshVoiceCommandDefinitions()
        {
            _voiceEngine?.SetCommandDefinitions(BuildVoiceCommands());
        }

        public IReadOnlyCollection<VoiceCommandDefinition> BuildVoiceCommands()
        {
            var commands = new List<VoiceCommandDefinition>();

            // ── Part labels (A-F) ────────────────────────────────────────────
            foreach (var (label, code, display, extra) in GetPartLabels())
            {
                var aliases = new List<string> { code, "điểm " + code, "vị trí " + code, "part " + code };
                if (extra != null) aliases.AddRange(extra);
                commands.Add(new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Part,
                    Code = code,
                    DisplayText = display,
                    Aliases = aliases
                });
            }

            // ── REASON_ID cố định cho A36 ────────────────────────────────────
            // Chỉ nhận 16 mã lỗi thực tế dùng tại A36.

            // → Whisper sẽ ưu tiên nhận diện 3 mã lỗi thường xuyên nhất này.
            var a14ReasonIds = new[] { "39", "40", "41", "42", "43", "44", "45", "46", "47", "48", "49", "50", "51", "52", "53", "54", "55" };

            // Lấy button text từ form để giữ đúng tên lỗi thực tế
            var buttonMap = GetReasonButtons()
    .Where(b => !string.IsNullOrWhiteSpace(b.AccessibleName))
    .GroupBy(b => b.AccessibleName)
    .ToDictionary(g => g.Key, g =>
        string.IsNullOrWhiteSpace(g.First().Text) ? g.Key : g.First().Text.Replace(Environment.NewLine, " "));

            foreach (string reasonId in a14ReasonIds)
            {
                // Lấy display text từ button nếu có, fallback về reasonId
                buttonMap.TryGetValue(reasonId, out string displayText);
                displayText ??= reasonId;

                commands.Add(new VoiceCommandDefinition
                {
                    Kind = VoiceCommandKind.Error,
                    Code = reasonId,
                    DisplayText = displayText,
                    Aliases = VoiceAliasHelper.BuildReasonAliases(reasonId, displayText)
                });
            }

            commands.AddRange(VoiceAliasHelper.BuildActionCommands(SupportedActions));
            return commands;
        }


        public void SelectPart(string partCode)
        {
            var entry = GetPartLabels()
                .FirstOrDefault(p => string.Equals(p.Code, partCode, StringComparison.OrdinalIgnoreCase));

            if (entry.Label == null)
            {
                ShowMessage("Voice: không tìm thấy part " + partCode, Color.Red);
                return;
            }

            lblPart3_Click(entry.Label, EventArgs.Empty);
        }

        public void SelectError(string errorCode)
        {
            if (string.IsNullOrWhiteSpace(partID))
            {
                ShowMessage("Voice: chọn part trước khi chọn lỗi.", Color.Red);
                return;
            }

            Button reasonButton = GetReasonButtons()
                .FirstOrDefault(button => string.Equals(button.AccessibleName, errorCode, StringComparison.OrdinalIgnoreCase));

            if (reasonButton == null)
            {
                ShowMessage("Voice: không tìm thấy mã lỗi " + errorCode, Color.Red);
                return;
            }

            btnError_Click(reasonButton, EventArgs.Empty);
        }
        private void ShowMessage(string message, Color color)
        {
            txtMessage.Text = message;
            txtMessage.ForeColor = color;

            //timerStopMessage.Interval = 3000; // 3 giây
            timerStopMessage.Start();
        }
        public void ConfirmAction(string actionType)
        {
            //switch ((actionType ?? string.Empty).ToLowerInvariant())
            //{
            //    case "pass":
            //        btnPass_Click(btnPass, EventArgs.Empty);
            //        break;
            //    case "re-pass":
            //        btnRePass_Click(btnRePass, EventArgs.Empty);
            //        break;
            //    case "fail":
            //        btnFail_Click(btnFail, EventArgs.Empty);
            //        break;
            //    case "re-fail":
            //        button4_Click(btnReFail, EventArgs.Empty);
            //        break;
            //    case "clear":
            //        btnClear_Click(btnClear, EventArgs.Empty);
            //        break;
            //    default:
            //        ShowMessage("Voice: không hỗ trợ lệnh " + actionType, Color.Red);
            //        break;
            //}
        }

        // ── Part labels cho A36─────────────────────────────────────────────
        // Alias: ưu tiên cách đọc ĐÔI (bê bê, xê xê...) để Whisper nhận Part rõ hơn.
        // Kỹ thuật "đọc đôi" (AA, BB...): Whisper nhận ký tự đơn rất kém,
        // nhưng lặp 2 lần cụm âm tiết rõ (bê bê, xê xê) tăng accuracy lên đáng kể.
        private IEnumerable<(Label Label, string Code, string Display, string[] ExtraAliases)> GetPartLabels()
        {
            yield return (lblPart1, "A", "Part A", new[] { "a", "à", "ạ", "á", "Ah", "ah", "a a", "lỗi a" });
            yield return (lblPart2, "B", "Part B", new[] { "bê", "bờ", "bê bê", "bờ bờ", "b b", "bb" });
            yield return (lblPart3, "C", "Part C", new[] { "xê", "sê", "se", "xê xê", "sê sê", "c c" });
            yield return (lblPart4, "D", "Part D", new[] { "đê", "đề", "đê đê", "đề đề", "d d" });

        }

        // ── VOICE AUTO TEST ──────────────────────────────────────────────────
        // Gọi từ button btnVoiceAutoTest (thêm button vào Designer hoặc dùng context menu)
        // ─────────────────────────────────────────────────────────────────────
        public async Task RunVoiceAutoTestAsync(
            IReadOnlyList<VoiceTestCase> cases = null,
            IProgress<(int cur, int total)> progress = null)
        {
            if (_voiceEngine == null)
            {
                ShowMessage("VoiceEngine chưa sẵn sàng.", Color.Red);
                return;
            }

            // Dừng auto loop nếu đang chạy
            if (_autoLoopCts != null)
            {
                _autoLoopCts.Cancel();
                _autoLoopCts = null;
                _voiceEngine.IsAutoLoopMode = false;
                await Task.Delay(500); // chờ engine về trạng thái Ready
            }

            ShowMessage("Đang chạy Voice Auto Test...", Color.Blue);

            string tempDir = System.IO.Path.Combine(Application.StartupPath, "VoiceTestTemp");
            string reportDir = System.IO.Path.Combine(Application.StartupPath, "test", "log");

            var runner = new VoiceAutoTestRunner(
                engine: _voiceEngine,
                logAction: msg => ThreadSafe(() => ShowMessage(msg, Color.DarkSlateGray)),
                tempDir: tempDir
            );

            try
            {
                var results = await runner.RunAllAsync(cases, progress);

                int pass = results.Count(r => r.IsPass);
                Color statusColor = (pass == results.Count) ? Color.Green : Color.OrangeRed;

                // Lưu báo cáo markdown
                System.IO.Directory.CreateDirectory(reportDir);
                string reportPath = System.IO.Path.Combine(
                    reportDir,
                    $"A14_autotest_{DateTime.Now:yyyyMMdd_HHmmss}.md");
                runner.SaveReport(results, reportPath);

                ShowMessage(
                    $"Auto Test: {pass}/{results.Count} PASS  |  Báo cáo: {System.IO.Path.GetFileName(reportPath)}",
                    statusColor);
            }
            catch (Exception ex)
            {
                ShowMessage("Auto Test lỗi: " + ex.Message, Color.Red);
            }
        }
        private async Task InitVoiceAsync()
        {
            _micButton.Enabled = false;
            _micButton.Text = "⏳ Đang load…";
            try
            {
                await _voice.InitializeAsync();
                SafeInvoke(() =>
                {
                    _micButton.Enabled = true;
                    _micButton.Text = "🎙 Giọng nói";
                });
            }
            catch
            {
                SafeInvoke(() => _micButton.Text = "✘ Lỗi model");
            }
        }

        // ── Nút mic ───────────────────────────────────────────────────────

        private void MicButton_Click(object? sender, EventArgs e)
        {
            if (!_voice.IsModelReady) return;

            if (!_voice.IsRecording)
            {
                // Bắt đầu ghi
                _voice.StartRecording();
                _micButton.Text = "⏹ Dừng";
                _micButton.BackColor = Color.FromArgb(57, 211, 140);
                _resultLabel.Text = "Đang nghe…";
            }
            else
            {
                // Dừng & nhận dạng (async, kết quả trả về qua event)
                _micButton.Enabled = false;
                _micButton.Text = "⚙ Đang xử lý…";
                _ = _voice.StopAndRecognizeAsync();
            }
        }

        // ── Xử lý kết quả ─────────────────────────────────────────────────

        private void OnRecognitionCompleted(object? sender, RecognitionResultEventArgs e)
        {
            //SafeInvoke(() =>
            //{
            //    _micButton.Enabled = true;
            //    _micButton.Text = "🎙 Giọng nói";
            //    _micButton.BackColor = SystemColors.Control;

            //    if (e.IsMatched)
            //    {
            //        _resultLabel.Text = $"✔ {e.MatchedCommand}  ({e.MatchScore:P0})";
            //        _resultLabel.ForeColor = Color.Green;

            //        // Xử lý lệnh theo nghiệp vụ của form này
            //        HandleCommand(e.MatchedCommand!);
            //    }
            //    else
            //    {
            //        string info = e.MatchedCommand is not null
            //            ? $"Gần nhất: \"{e.MatchedCommand}\" ({e.MatchScore:P0}) – dưới ngưỡng"
            //            : "Không khớp lệnh nào";
            //        _resultLabel.Text = $"⚠ {info}";
            //        _resultLabel.ForeColor = Color.OrangeRed;
            //    }
            //});

            //SafeInvoke(() =>
            //{
            //    _micButton.Enabled = true;
            //    _micButton.Text = "🎙 Giọng nói";
            //    _micButton.BackColor = SystemColors.Control;

            //    if (!string.IsNullOrWhiteSpace(e.RawText))
            //    {
            //        string normalized = NormalizeVoice(e.RawText);
            //        ProcessVoiceCommand(normalized);
            //    }
            //});

            if (e.IsMatched && !string.IsNullOrWhiteSpace(e.MatchedCommand))
            {
                var parts = e.MatchedCommand.Split(' ');
                if (parts.Length == 3)
                {
                    string position = parts[0]; // Vị trí (A, B, C, D)
                    string errorCode = parts[1]; // Mã lỗi (AccessibleName của button)
                    string action = parts[2];    // Hành động (fail, repair)

                    ProcessCommand(position, errorCode, action);
                }
            }
            else
            {
                txtMessage.Text = "Không nhận dạng được lệnh. Vui lòng thử lại.";
            }
        }
        private void ProcessCommand(string position, string errorCode, string action)
        {
            Label? targetLabel = position switch
            {
                "A" => lblPart1,
                "B" => lblPart2,
                "C" => lblPart3,
                "D" => lblPart4,
                _ => null
            };

            if (targetLabel != null)
            {
                targetLabel.ForeColor = Color.Red; // Highlight vị trí
            }

            Button? targetButton = this.Controls
                .OfType<Button>()
                .FirstOrDefault(btn => btn.AccessibleName == errorCode);

            if (targetButton != null)
            {
                targetButton.PerformClick(); // Nhấn nút tương ứng với mã lỗi
            }

            txtMessage.Text = $"Vị trí: {position}, Mã lỗi: {errorCode}, Hành động: {action}";
        }
        //private void ProcessVoiceCommand(string s)
        //{
        //    // ── Repass trước (phải check trước pass) ────────────────────
        //    if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*pass)\b"))
        //    {
        //        btnRePass_Click(btnRePass, EventArgs.Empty);
        //        ShowMessage("✔ REPASS", Color.LimeGreen);
        //        return;
        //    }

        //    // ── Pass ─────────────────────────────────────────────────────
        //    if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bpass\b"))
        //    {
        //        btnPass_Click(btnPass, EventArgs.Empty);
        //        ShowMessage("✔ PASS", Color.LimeGreen);
        //        return;
        //    }

        //    // ── Refail / Fail ─────────────────────────────────────────────
        //    bool isRefail = System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*fail)\b");
        //    bool isFail = !isRefail && System.Text.RegularExpressions.Regex.IsMatch(s, @"\bfail\b");

        //    if (isFail || isRefail)
        //    {
        //        // Vị trí
        //        var partMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b([abcd])\b");
        //        if (partMatch.Success)
        //        {
        //            Label targetLbl = partMatch.Value.ToUpper() switch
        //            {
        //                "A" => lblPart1,
        //                "B" => lblPart2,
        //                "C" => lblPart3,
        //                "D" => lblPart4,
        //                _ => null
        //            };
        //            if (targetLbl != null)
        //                lblPart3_Click(targetLbl, EventArgs.Empty);
        //        }

        //        // Số lỗi + Fail — delay để lblPart xử lý xong
        //        var numMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b(\d{2,3})\b");
        //        Task.Delay(200).ContinueWith(_ =>
        //        {
        //            SafeInvoke(() =>
        //            {
        //                if (numMatch.Success)
        //                {
        //                    var errBtn = GetReasonButtons()
        //                                     .FirstOrDefault(b => b.AccessibleName == numMatch.Value);
        //                    if (errBtn != null)
        //                        btnError_Click(errBtn, EventArgs.Empty);
        //                }

        //                Task.Delay(100).ContinueWith(__ =>
        //                {
        //                    SafeInvoke(() =>
        //                    {
        //                        if (isRefail) button4_Click(button4, EventArgs.Empty);
        //                        else btnFail_Click(btnFail, EventArgs.Empty);
        //                        ShowMessage($"✔ {(isRefail ? "REFAIL" : "FAIL")}", Color.OrangeRed);
        //                    });
        //                });
        //            });
        //        });
        //        return;
        //    }

        //    ShowMessage($"❓ Không nhận ra: \"{s}\"", Color.Orange);
        //}
        private IEnumerable<Button> GetReasonButtons()
        {
            Control[] reasonContainers =
            {
                tableLayoutError1,
                tableLayoutError
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

        //private string NormalizeVoice(string s)
        //{

        //    s = Regex.Replace(s, @"[^\w\s]", " "); // bỏ ký tự rác
        //    s = Regex.Replace(s, @"\s+", " ");
        //    if (s.Split(' ').Length > 6) return "";
        //    s = s.ToLowerInvariant().Trim();

        //    // ── Vị trí — nói kèm từ cho dễ nhận ────────────────────────
        //    // Nói "alpha" / "an" / "một" → A
        //    s = s.Replace("alpha", "a");
        //    s = s.Replace(" an ", " a ");
        //    s = s.Replace("a a", "a");
        //    s = s.Replace("vùng 1", "a");
        //    s = s.Replace("phần 1", "a");
        //    s = s.Replace("một", "a");  // nếu chỉ có 1 chữ số

        //    // Nói "bravo" / "bê" / "hai" → B  
        //    s = s.Replace("bravo", "b");
        //    s = s.Replace("bê bê", "b");
        //    s = s.Replace("bê", "b");
        //    s = s.Replace("vùng 2", "b");
        //    s = s.Replace("phần 2", "b");

        //    // Nói "charlie" / "xê" / "cê" / "ba" → C
        //    s = s.Replace("charlie", "c");
        //    s = s.Replace("xê xê", "c");
        //    s = s.Replace("xê", "c");
        //    s = s.Replace("cê cê", "c");
        //    s = s.Replace("cê", "c");
        //    s = s.Replace("vùng 3", "c");
        //    s = s.Replace("phần 3", "c");

        //    // Nói "delta" / "đê" / "bốn" → D
        //    s = s.Replace("delta", "d");
        //    s = s.Replace("đê đê", "d");
        //    s = s.Replace("đê", "d");
        //    s = s.Replace("vùng 4", "d");
        //    s = s.Replace("phần 4", "d");

        //    // ── Pass / Repass ────────────────────────────────────────────
        //    s = s.Replace("tái đạt", "repass");
        //    s = s.Replace("re đạt", "repass");
        //    s = s.Replace("đạt lại", "repass");
        //    s = s.Replace("đạt", "pass");
        //    s = s.Replace("pát", "pass");
        //    s = s.Replace("pas", "pass");

        //    // ── Fail / Refail ────────────────────────────────────────────
        //    s = s.Replace("tái rớt", "refail");
        //    s = s.Replace("re rớt", "refail");
        //    s = s.Replace("rớt lại", "refail");
        //    s = s.Replace("tái lỗi", "refail");
        //    s = s.Replace("re lỗi", "refail");
        //    s = s.Replace("rớt", "fail");
        //    s = s.Replace("lỗi", "fail");
        //    s = s.Replace("feel", "fail");
        //    s = s.Replace("fell", "fail");
        //    s = s.Replace("phil", "fail");
        //    s = s.Replace("fai", "fail");

        //    // ── Số ──────────────────────────────────────────────────────
        //    s = s.Replace("ba mươi chín", "39");

        //    s = s.Replace("bốn mươi mốt", "41");
        //    s = s.Replace("bốn mươi hai", "42");
        //    s = s.Replace("bốn mươi ba", "43");
        //    s = s.Replace("bốn mươi bốn", "44");
        //    s = s.Replace("bốn mươi lăm", "45");
        //    s = s.Replace("bốn mươi sáu", "46");
        //    s = s.Replace("bốn mươi bảy", "47");
        //    s = s.Replace("bốn mươi tám", "48");
        //    s = s.Replace("bốn mươi chín", "49");
        //    s = s.Replace("bốn mươi", "40");

        //    s = s.Replace("năm mươi mốt", "51");
        //    s = s.Replace("năm mươi hai", "52");
        //    s = s.Replace("năm mươi ba", "53");
        //    s = s.Replace("năm mươi bốn", "54");
        //    s = s.Replace("năm mươi lăm", "55");
        //    s = s.Replace("năm mươi", "50");

        //    return s;
        //}

        private string NormalizeVoice(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return "";

            s = s.ToLowerInvariant().Trim();
            s = Regex.Replace(s, @"[^\wÀ-ỹ\s]", " "); // giữ tiếng Việt
            s = Regex.Replace(s, @"\s+", " ").Trim();

            // ── Số: PHẢI replace chuỗi DÀI trước, ngắn sau ──────────────────
            // Nếu replace "bốn mươi" trước → "bốn mươi mốt" bị sai thành "40 mốt"
            var numberMap = new (string spoken, string digit)[]
            {
        ("ba mươi chín",   "39"),
        ("bốn mươi mốt",  "41"), ("bốn mươi hai",  "42"),
        ("bốn mươi ba",   "43"), ("bốn mươi bốn",  "44"),
        ("bốn mươi lăm",  "45"), ("bốn mươi sáu",  "46"),
        ("bốn mươi bảy",  "47"), ("bốn mươi tám",  "48"),
        ("bốn mươi chín", "49"), ("bốn mươi",      "40"),
        ("năm mươi mốt",  "51"), ("năm mươi hai",  "52"),
        ("năm mươi ba",   "53"), ("năm mươi bốn",  "54"),
        ("năm mươi lăm",  "55"), ("năm mươi",      "50"),
            };
            foreach (var (spoken, digit) in numberMap)
                s = s.Replace(spoken, digit);

            // ── Vị trí ───────────────────────────────────────────────────────
            s = Regex.Replace(s, @"\b(alpha|ah|à|ạ|á|a a|lỗi a|phần 1|vùng 1|một)\b", "a");
            s = Regex.Replace(s, @"\b(bravo|bê bê|bê|bờ bờ|bờ|b b|bb|phần 2|vùng 2)\b", "b");
            s = Regex.Replace(s, @"\b(charlie|xê xê|xê|sê sê|sê|cê cê|cê|c c|phần 3|vùng 3)\b", "c");
            s = Regex.Replace(s, @"\b(delta|đê đê|đê|đề đề|đề|d d|phần 4|vùng 4)\b", "d");

            // ── Action: repass/refail TRƯỚC pass/fail ────────────────────────
            s = Regex.Replace(s, @"\b(tái đạt|re đạt|đạt lại|tái pass|re pass)\b", "repass");
            s = Regex.Replace(s, @"\b(tái rớt|re rớt|rớt lại|tái lỗi|re lỗi|tái fail|re fail)\b", "refail");
            s = Regex.Replace(s, @"\b(đạt|pát|pas|pass)\b", "pass");
            s = Regex.Replace(s, @"\b(rớt|lỗi|feel|fell|phil|fai|fail)\b", "fail");

            // Bỏ từ nhiễu thường gặp từ Whisper
            s = Regex.Replace(s, @"\b(ừ|uh|um|okay|ok|vâng|dạ|thì|là|ở)\b", "");
            s = Regex.Replace(s, @"\s+", " ").Trim();

            return s;
        }
        private void OnModelStatus(object? sender, ModelStatusEventArgs e)
        {
            // Tuỳ ý – ghi log, hiện tooltip, cập nhật status bar…
            SafeInvoke(() => Text = $"Form – {e.Message}");
        }

        // ── Nghiệp vụ riêng của form ───────────────────────────────────────

        private void HandleCommand(string command)
        {
            // TODO: tuỳ từng form làm gì với lệnh nhận được
            switch (command)
            {
                case "Lỗi 1": /* ... */ break;
                case "Lỗi 2": /* ... */ break;
                case "Hở keo": /* ... */ break;
            }
        }

        // ── UI helpers ─────────────────────────────────────────────────────

        private void BuildMicButton()
        {
            _micButton = new Button
            {
                Text = "🎙 Giọng nói",
                Size = new Size(130, 40),
                Location = new Point(16, 16)
            };
            _micButton.Click += MicButton_Click;

            _resultLabel = new Label
            {
                Location = new Point(16, 64),
                AutoSize = true,
                Font = new Font("Segoe UI", 10f),
                ForeColor = SystemColors.ControlText
            };

            Controls.Add(_micButton);
            Controls.Add(_resultLabel);
        }

        private void SafeInvoke(Action a)
        {
            if (IsDisposed) return;
            if (InvokeRequired) Invoke(a); else a();
        }

        private void btnVoiceWhisper_Click(object sender, EventArgs e)
        {

        }
    }
}