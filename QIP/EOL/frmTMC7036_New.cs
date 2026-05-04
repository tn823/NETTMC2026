using ConnectionClass.Oracle;
using NETTMC.VoiceRecognition;
using QIP.EOL.Popup;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VoiceWhisperApp2;
using static DEV.RFID.RFID_Alarm;
using static GlobalFunction.PublicFunction;



namespace QIP.EOL
{
    public partial class frmTMC7036_New : UserControl
    {
        CRUDOracle crud = new CRUDOracle("VSMES");
        public static string ipAddress;
        private static string C_Line;
        private static string LineName;
        bool isstartcountreason;
        public bool MQTTConnected = false;
        private string currentGroup = "";
        public static string spDeptCode;
        private static string reasonID;
        public static DataTable errorTouch;
        private static string RecievedIpaddress;
        private static readonly Color ReasonButtonDefaultColor = Color.PaleTurquoise;
        private static readonly Color ReasonButtonSelectedColor = Color.FromArgb(224, 224, 224);
        private static readonly Color PassButtonColor = Color.Blue;
        private static readonly Color FailButtonColor = Color.FromArgb(231, 76, 60);
        private static readonly Color ClearButtonColor = Color.Gray;

        GlobalFunction.PublicFunction etc = new GlobalFunction.PublicFunction();
        Dictionary<string, string> Reason = new Dictionary<string, string>();


        private VoiceRecognitionService _voice;


        private Button _micButton = null!;
        private Label _resultLabel = null!;
        //private VoiceRecognitionService _voice;
        //private VoiceCommandParser _parser;
        public frmTMC7036_New()
        {
            InitializeComponent();
            InitializeActionButtons();

            BuildMicButton();           // Tạo nút mic trong form này

           
        }

        private void tableLayoutPanel3_Paint(object sender, PaintEventArgs e)
        {

        }
        
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void tableLayoutPanel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

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
        private void GetSetButtonLocation()
        {
            //lblPart1.Parent = picShoeImage;
            //lblPart2.Parent = picShoeImage;
            //lblPart3.Parent = picShoeImage;
            //lblPart4.Parent = picShoeImage;
            BindingControl();
            ReadLocation();

        }
        
        SerialPort serialPort1;
        private void frmTMC7036_New_Load(object sender, EventArgs e)
        {
            
            lblTop1Defect.Text = "";
            lblTop2Defect.Text = "";
            lblTop3Defect.Text = "";
            lblFirstDefect.Text = "";
            lblReDefect.Text = "";
            lblTotalDefect.Text = "";
            lblEOLQCDDPM.Text = "";
            lblSensorCount.Text = "";
            txtPo.Enabled = false;
            lblRFT.Text = "";
            sensorCount = 0;
            finishedCount = 0;
            countSensor = 0;
            countSensorQC = 0;
            
            ipAddress = GlobalFunction.PublicFunction.myIpaddress;
            ipAddress = "192.168.0.85";
            GetLineName(ipAddress);
            spDeptCode = "STF";
            //SetErrorToButton("STF");
            pictureEdit2.Focus();
            //countDefectContinuos = 0;
            crud = new CRUDOracle("VSMES");

            //spDeptCode = "STF";
            GetSetButtonLocation();
            //timer1.Enabled = true;
            //timer2.Enabled = true;
            StringBuilder query = new StringBuilder();
            sensorCount = 0;
            //finishedCount = 0;
            //countSensor = 0;
            //countSensorQC = 0;
            BindingControl();
            GetSetButtonLocation();
            ConffigErrorButton(false);
            SetInspectionActionState(true, false, true, false);
            //timer1.Enabled = true;
            timer2.Enabled = true;
            //GetLineName(ipAddress);
            TryToUpdateSystemDateTime();
            //InitSerial();
            SetErrorToButton("STF");
            errorTouch = new DataTable();
            errorTouch.Columns.Add("C_LINE");
            errorTouch.Columns.Add("PART_ID");
            errorTouch.Columns.Add("REASON_ID");
            errorTouch.Columns.Add("MES_GROUP_SUM");
            errorTouch.Columns.Add("USER_ID");
            errorTouch.Columns.Add("IP_ADDRESS");
            SetTouchCount();
            BindTouchCount(TouchCount);
            setDataProduction();
            // ── Voice ──────────────────────────────────────────────
            _voice = new VoiceRecognitionService(
    new[] { "pass", "repass", "fail", "refail" },
    enableTts: false
);

            _voice.MessageLogged += (_, e) =>
            {
                if (InvokeRequired) Invoke(() => ShowMessage(e.Message, e.Color));
                else ShowMessage(e.Message, e.Color);
            };

            _voice.RecognitionCompleted += (_, e) =>
            {
                if (InvokeRequired) Invoke(() => HandleVoiceCommand(e.RawText));
                else HandleVoiceCommand(e.RawText);
            };

            _ = _voice.InitializeAsync();
        }
        private void HandleVoiceCommand(string rawText)
        {
            string s = rawText.ToLowerInvariant().Trim();

            if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*pass)\b"))
            {
                btnRePass.PerformClick();
                ShowMessage("✔ REPASS", Color.LimeGreen);
                return;
            }

            if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bpass\b"))
            {
                btnPass.PerformClick();
                ShowMessage("✔ PASS", Color.LimeGreen);
                return;
            }

            bool isRefail = System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*fail)\b");
            bool isFail = !isRefail && System.Text.RegularExpressions.Regex.IsMatch(s, @"\bfail\b");

            if (isFail || isRefail)
            {
                // Bước 1 - click lblPart
                var partMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b([abcd])\b");
                if (partMatch.Success)
                {
                    Label targetLbl = partMatch.Value.ToUpper() switch
                    {
                        "A" => lblPart1,
                        "B" => lblPart2,
                        "C" => lblPart3,
                        "D" => lblPart4,
                        _ => null
                    };
                    if (targetLbl != null)
                        lblPart3_Click(targetLbl, EventArgs.Empty);
                }

                // Bước 2 - đợi lblPart xử lý xong rồi mới click lỗi và fail
                var numMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b(\d{2,3})\b");
                Task.Delay(200).ContinueWith(_ =>
                {
                    SafeInvoke(() =>
                    {
                        if (numMatch.Success)
                        {
                            var errBtn = GetReasonButtons()
                                             .FirstOrDefault(b => b.AccessibleName == numMatch.Value);
                            if (errBtn != null)
                                btnError_Click(errBtn, EventArgs.Empty);
                        }

                        // Bước 3 - click Fail/Refail
                        Task.Delay(100).ContinueWith(__ =>
                        {
                            SafeInvoke(() =>
                            {
                                if (isRefail) button4_Click(button4, EventArgs.Empty);
                                else btnFail_Click(btnFail, EventArgs.Empty);
                                ShowMessage($"✔ {(isRefail ? "REFAIL" : "FAIL")}", Color.OrangeRed);
                            });
                        });
                    });
                });
                return;
            }
        }
        private DataTable GetDataTable(DataGridView view)
        {
            DataTable dt = new DataTable();

            foreach (DataGridViewColumn col in view.Columns)
            {
                dt.Columns.Add(col.Name, col.ValueType ?? typeof(string));
            }

            foreach (DataGridViewRow row in view.Rows)
            {
                if (!row.IsNewRow)
                {
                    object[] rowValues = new object[view.Columns.Count];

                    for (int i = 0; i < view.Columns.Count; i++)
                    {
                        rowValues[i] = row.Cells[i].Value;
                    }

                    dt.Rows.Add(rowValues);
                }
            }

            return dt;
        }
        //private void GetLineName(string ip)
        //{
        //    DataTable dt = new DataTable();
        //    var b = new BackgroundWorker();
        //    b.DoWork += new DoWorkEventHandler(
        //        delegate (object sender, DoWorkEventArgs e)
        //        {
        //            StringBuilder query = new StringBuilder();
        //            query.AppendLine("");
        //            query.AppendLine("SELECT SUBSTR(C_COMCODE,4,4) C_COMCODE,                                                                               ");
        //            query.AppendLine("  case when SUBSTR(C_COMCODE,4,2) = 'P7' THEN SUBSTR(C_COMCODE,4,4)                                                   ");
        //            query.AppendLine("     ELSE                                                                                                             ");
        //            query.AppendLine("         DECODE(SUBSTR(C_COMCODE, 6, 1), 'A', 'P1', 'B', 'P2', 'C', 'P3', 'D', 'P4', 'E', 'P5', 'F', 'P6', 'PP') ||   ");
        //            query.AppendLine("         CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A'                                                                       ");
        //            query.AppendLine("                   THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55)                                                      ");
        //            query.AppendLine("             Else '0' || SUBSTR(C_COMCODE, 7, 1)                                                                      ");
        //            query.AppendLine("         END                                                                                                          ");
        //            query.AppendLine("END SHOW_LINE                                                                                                         ");
        //            query.AppendLine("    FROM (                                                                                                            ");
        //            query.AppendLine("          SELECT SUBSTR(C_COMCODE,1,7) C_COMCODE,N_COMNAME                                                            ");
        //            query.AppendLine("            From TRTB_M_COMMON                                                                                        ");
        //            query.AppendLine("           WHERE C_GROUP = 'BTS'                                                                                      ");
        //            query.AppendLine("             AND N_COMNAME = '" + ip + "'                                                                             ");
        //            query.AppendLine("         )                                                                                                            ");
        //            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
        //            e.Result = dt;
        //            dt = (DataTable)e.Result;
        //        }
        //    );
        //    b.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object sender, RunWorkerCompletedEventArgs e)
        //    {
        //        if (e.Error != null)
        //        {
        //            txtLineName.Text = "Err";
        //            if (File.Exists(Application.StartupPath + "\\LineName.xls"))
        //            {
        //                var source = new ExcelDataSource();
        //                source.FileName = Application.StartupPath + "\\LineName.xls";
        //                var worksheetSettings = new ExcelWorksheetSettings("Sheet");
        //                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
        //                source.Fill();
        //                grdOfflineData.DataSource = source;
        //                DataTable dtLineName = GetDataTable(grdvOfflineData);
        //                LineName = dtLineName.Rows[0]["SHOW_LINE"].ToString();
        //                spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //                txtLineName.Text = LineName;
        //            }
        //        }
        //        dt = (DataTable)e.Result;
        //        if (dt == null || dt.Rows.Count < 0)
        //        {
        //            var source = new ExcelDataSource();
        //            source.FileName = Application.StartupPath + "\\LineName.xls";
        //            var worksheetSettings = new ExcelWorksheetSettings("Sheet");
        //            source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
        //            source.Fill();
        //            grdOfflineData.DataSource = source;
        //            //DataTable dtLineName = GetDataTable(grdvOfflineData);
        //            //LineName = dtLineName.Rows[0]["SHOW_LINE"].ToString();
        //            spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //            txtLineName.Text = LineName;
        //        }
        //        else if (dt.Rows.Count > 0)
        //        {
        //            this.grdOfflineData.DataSource = dt;
        //            XlsExportOptions options = new XlsExportOptions();
        //            if (File.Exists(Application.StartupPath + "\\LineName.xls"))
        //            {
        //                File.Delete(Application.StartupPath + "\\LineName.xls");
        //            }
        //            else
        //            {

        //            }
        //            this.grdOfflineData.ExportToXls(Application.StartupPath + "\\LineName.xls");
        //            //spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //            spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //            LineName = dt.Rows[0]["SHOW_LINE"].ToString();
        //            txtLineName.Text = LineName;
        //        }
        //        //if (txtLineName.Text != "P114")
        //        //{
        //        //    lblSensorCount.Visible = false;
        //        //    //label1.Visible = false;
        //        //}
        //    });
        //    b.RunWorkerAsync();
        //}
        private void GetLineName(string ip)
        {
            var b = new BackgroundWorker();

            b.DoWork += (sender, e) =>
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine("SELECT SUBSTR(C_COMCODE,4,4) C_COMCODE,");
                query.AppendLine("CASE WHEN SUBSTR(C_COMCODE,4,2) = 'P7' THEN SUBSTR(C_COMCODE,4,4)");
                query.AppendLine("ELSE");
                query.AppendLine("DECODE(SUBSTR(C_COMCODE, 6, 1), 'A','P1','B','P2','C','P3','D','P4','E','P5','F','P6','PP') ||");
                query.AppendLine("CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A'");
                query.AppendLine("THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55)");
                query.AppendLine("ELSE '0' || SUBSTR(C_COMCODE,7,1)");
                query.AppendLine("END END SHOW_LINE");
                query.AppendLine("FROM (");
                query.AppendLine("SELECT SUBSTR(C_COMCODE,1,7) C_COMCODE, N_COMNAME");
                query.AppendLine("FROM TRTB_M_COMMON");
                query.AppendLine("WHERE C_GROUP = 'BTS'");
                query.AppendLine("AND N_COMNAME = '" + ip + "')");

                e.Result = crud?.dac?.DtSelectExcuteWithQuery(query.ToString());
            };

            b.RunWorkerCompleted += (sender, e) =>
            {
                string filePath = Application.StartupPath + "\\LineName.csv";
                DataTable dt = e.Result as DataTable;

                if (e.Error != null)
                {
                    MessageBox.Show(e.Error.ToString());
                    LoadFromCSV(filePath);
                    return;
                }

                if (dt == null || dt.Rows.Count == 0)
                {
                    LoadFromCSV(filePath);
                    return;
                }

                grdOfflineData.DataSource = dt;

                SaveCSV(dt, filePath);

                spLine = dt.Rows[0]["C_COMCODE"]?.ToString();
                LineName = dt.Rows[0]["SHOW_LINE"]?.ToString();
                txtLineName.Text = LineName;
            };

            b.RunWorkerAsync();
        }
        private void LoadFromCSV(string path)
        {
            if (!File.Exists(path))
            {
                txtLineName.Text = "No Data";
                return;
            }

            DataTable dt = ReadCSV(path);

            if (dt != null && dt.Rows.Count > 0)
            {
                spLine = dt.Rows[0]["C_COMCODE"]?.ToString();
                LineName = dt.Rows[0]["SHOW_LINE"]?.ToString();
                txtLineName.Text = LineName;

                grdOfflineData.DataSource = dt;
            }
            else
            {
                txtLineName.Text = "No Data";
            }
        }
        private DataTable ReadExcel(string path)
        {
            DataTable dt = new DataTable();

            string conn = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties='Excel 8.0;HDR=YES;'";

            using (OleDbConnection con = new OleDbConnection(conn))
            {
                con.Open();
                OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet$]", con);
                da.Fill(dt);
            }

            return dt;
        }
        private void BindingControl()
        {
            SetLineName(ipAddress);
            //SetSpLine(ipAddress);
            //this.btnFT.PerformClick();
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
            if (serialPort1 == null)
            {
                serialPort1 = new SerialPort();
            }

            SetComport();
            //this.btnCapture.Enabled = false;

            int xPic = picShoeImage.Location.X;
            int yPic = picShoeImage.Location.Y;
            int wPic = picShoeImage.Width;
            int hPic = picShoeImage.Height;
        }
        private bool ConnectComport(string portName)
        {
            try
            {
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
        //private void InitSerial()
        //{
        //    try
        //    {
        //        serialPort1 = new SerialPort();

        //        var ports = SerialPort.GetPortNames();

        //        if (ports.Length == 0)
        //        {
        //            MessageBox.Show("Không có cổng COM nào!");
        //            return;
        //        }

        //        MessageBox.Show("COM hiện có: " + string.Join(", ", ports));

        //        serialPort1.PortName = "COM3"; 

        //        serialPort1.BaudRate = 9600;
        //        serialPort1.DataReceived += SerialPort1_DataReceived;

        //        if (serialPort1.IsOpen)
        //            serialPort1.Close();

        //        serialPort1.Open();
        //    }
        //    catch (UnauthorizedAccessException)
        //    {
        //        MessageBox.Show("COM đang bị chiếm! (Access Denied)");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show("Lỗi COM: " + ex.Message);
        //    }
        //}
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
        private int countSensor;
        private int countSensorQC;
        private string datarecive;
        List<string> linesensor = new List<string>();
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
        private int finishedCount;
        private string finishedCountScan;
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
                query.AppendLine("VALUES ( '" + currentGroup + "',                                 ");
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
        public static int ProdHour;
        private static string ScanIP;
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
        List<string> lineqc = new List<string>();
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
        SYSTEMTIME st = new SYSTEMTIME();
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
        //private void SetLineName(string ip)
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.AppendLine("");




        //    //query.AppendLine("   SELECT SUBSTR(C_COMCODE,4,4),                                                                    ");
        //    //query.AppendLine(" DECODE(SUBSTR(C_COMCODE,6,1),'A','P1','B','P2','C','P3','D','D','E','E','F','F','PP')||            ");
        //    //query.AppendLine("   CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A' THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55)             ");
        //    //query.AppendLine("        Else '0'||SUBSTR(C_COMCODE,7,1)                                                             ");
        //    //query.AppendLine("    END || CASE WHEN LENGTH(C_COMCODE) = 8 THEN 'B' ELSE 'A' END    SHOW_LINE                       ");
        //    //query.AppendLine("    FROM (                                                                                          ");
        //    //query.AppendLine("          SELECT SUBSTR(C_COMCODE,1,8) C_COMCODE,N_COMNAME                                          ");
        //    //query.AppendLine("            From TRTB_M_COMMON                                                                      ");
        //    //query.AppendLine("           WHERE C_GROUP = 'BTS'                                                                    ");
        //    //query.AppendLine("             AND N_COMNAME = '" + ip + "'                                                           ");
        //    //query.AppendLine("         )                                                                                          ");

        //    query.AppendLine("    SELECT SUBSTR (C_COMCODE, 4, 4) C_LINE,                                 ");
        //    query.AppendLine("         DECODE (SUBSTR (C_COMCODE, 6, 1),                                  ");
        //    query.AppendLine("                 'A', 'P1',                                                 ");
        //    query.AppendLine("                 'B', 'P2',                                                 ");
        //    query.AppendLine("                 'C', 'P3',                                                 ");
        //    query.AppendLine("                 'D', 'P4',                                                 ");
        //    query.AppendLine("                 'E', 'P5',                                                 ");
        //    query.AppendLine("                 'F', 'P6',                                                 ");
        //    query.AppendLine("                 SUBSTR (C_COMCODE, 6, 1))                                  ");
        //    query.AppendLine("      || CASE                                                               ");
        //    query.AppendLine("            WHEN SUBSTR (C_COMCODE, 7, 1) >= 'A'                            ");
        //    query.AppendLine("            THEN                                                            ");
        //    query.AppendLine("               TO_CHAR (ASCII (SUBSTR (C_COMCODE, 7, 1)) - 55)              ");
        //    query.AppendLine("            WHEN SUBSTR (C_COMCODE, 4, 1) = 'N'                             ");
        //    query.AppendLine("            THEN                                                            ");
        //    query.AppendLine("               '0' || SUBSTR (C_COMCODE, 7, 1)                              ");
        //    query.AppendLine("         END                                                                ");
        //    query.AppendLine("         SHOW_LINE                                                          ");
        //    query.AppendLine(" FROM (SELECT SUBSTR (C_COMCODE, 1, 8) C_COMCODE, N_COMNAME                 ");
        //    query.AppendLine("         FROM TRTB_M_COMMON                                                 ");
        //    query.AppendLine("        WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + ip + "')                 ");

        //    DataTable dt = new DataTable();
        //    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
        //    if (dt.Rows.Count > 0)
        //    {
        //        LineName = dt.Rows[0]["SHOW_LINE"].ToString(); //P503
        //        C_Line = dt.Rows[0]["C_LINE"].ToString(); //NSE3
        //        txtLineName.Text = LineName;
        //    }
        //    else
        //    {
        //        MessageBox.Show("Chưa khai báo trên TRTB_M_COMMON. Liên hệ IT" + Environment.NewLine + "Did not registered IP address. Contact IT-team", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        this.Hide();
        //        Application.Exit();
        //    }
        //}

        private void SetLineName(string ip)
        {
            try
            {
                if (string.IsNullOrEmpty(ip))
                {
                    MessageBox.Show("IP bị null");
                    return;
                }

                if (crud == null || crud.dac == null)
                {
                    MessageBox.Show("crud chưa khởi tạo");
                    return;
                }

                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT SUBSTR (C_COMCODE, 4, 4) C_LINE,");
                query.AppendLine("       DECODE (SUBSTR (C_COMCODE, 6, 1),");
                query.AppendLine("               'A', 'P1','B', 'P2','C', 'P3','D', 'P4','E', 'P5','F', 'P6',");
                query.AppendLine("               SUBSTR (C_COMCODE, 6, 1))");
                query.AppendLine("       || CASE");
                query.AppendLine("            WHEN SUBSTR (C_COMCODE, 7, 1) >= 'A'");
                query.AppendLine("            THEN TO_CHAR (ASCII (SUBSTR (C_COMCODE, 7, 1)) - 55)");
                query.AppendLine("            WHEN SUBSTR (C_COMCODE, 4, 1) = 'N'");
                query.AppendLine("            THEN '0' || SUBSTR (C_COMCODE, 7, 1)");
                query.AppendLine("         END SHOW_LINE");
                query.AppendLine("FROM (SELECT SUBSTR (C_COMCODE, 1, 8) C_COMCODE, N_COMNAME");
                query.AppendLine("      FROM TRTB_M_COMMON");
                query.AppendLine("      WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + ip + "')");

                DataTable dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

                Debug.WriteLine("Query executed: " + dt);

                if (dt != null && dt.Rows.Count > 0)
                {
                    LineName = dt.Rows[0]["SHOW_LINE"]?.ToString();
                    C_Line = dt.Rows[0]["C_LINE"]?.ToString();

                    // tránh crash UI thread
                    if (txtLineName != null)
                    {
                        if (txtLineName.InvokeRequired)
                        {
                            txtLineName.Invoke(new Action(() =>
                            {
                                txtLineName.Text = LineName;
                            }));
                        }
                        else
                        {
                            txtLineName.Text = LineName;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Chưa khai báo IP trên hệ thống");


                    Application.Exit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi SetLineName: " + ex.Message);
            }
        }
        private DataTable ReadCSV(string path)
        {
            DataTable dt = new DataTable();
            var lines = File.ReadAllLines(path);

            if (lines.Length == 0) return dt;

            string[] headers = lines[0].Split(',');
            foreach (var h in headers)
                dt.Columns.Add(h);

            for (int i = 1; i < lines.Length; i++)
                dt.Rows.Add(lines[i].Split(','));

            return dt;
        }
        private void SaveCSV(DataTable dt, string path)
        {
            StringBuilder sb = new StringBuilder();

            foreach (DataColumn col in dt.Columns)
                sb.Append(col.ColumnName + ",");

            sb.AppendLine();

            foreach (DataRow row in dt.Rows)
            {
                foreach (var item in row.ItemArray)
                    sb.Append(item?.ToString() + ",");

                sb.AppendLine();
            }

            File.WriteAllText(path, sb.ToString());
        }
        private int PassQty()
        {
            return etc.CountPassFail("Pass", "PassBTS_" + DateTime.Now.ToString("yyyyMMdd"));
        }
        private int FailQty()
        {
            return etc.CountPassFail("Fail", "FailBTS_" + DateTime.Now.ToString("yyyyMMdd"));
        }
        private void BindTouchCount(DataTable dt)
        {

            int offlineDefect = 0;
            TotalDefect = Convert.ToInt32(dt.Rows[0]["Q_FAIL"].ToString());
            offlineDefect = Convert.ToInt32(lblFailTotal.Text.Substring(7, lblFailTotal.Text.Length - 7));
            //lblSyncStatus.Text = TotalDefect + "/" + offlineDefect;
            this.lblFailTotal.Text = "FAIL : " + TotalDefect;
            TotalPass = Convert.ToInt32(dt.Rows[0]["Q_PASS"].ToString());
            this.lblPassTotal.Text = "PASS: " + TotalPass;

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

        private void button4_Click(object sender, EventArgs e)
        {
            if (txtPo.Text == "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);

                return;
            }
            btnfail = new List<string>();

            if (errorTouch != null && errorTouch.Rows.Count > 0)
            {
                if (toggleSwitchOnline != null && toggleSwitchOnline.Checked)
                {
                    UpdatePassFailDirectToDB(false, "2");
                }
                else
                {
                    UpdateFail2();
                }
            }
            else
            {
                errorTouch?.Clear();
            }

            //this.lblFailTotal.Text = "FAIL: " + TotalDefect;

            btnPass.Enabled = true;
            btnFail.Enabled = false;
            btnRePass.Enabled = true;
            btnReFail.Enabled = false;

            ConffigErrorButton(false);

            lblPart1.ForeColor = System.Drawing.Color.Red;
            lblPart2.ForeColor = System.Drawing.Color.Red;
            lblPart3.ForeColor = System.Drawing.Color.Red;
            lblPart4.ForeColor = System.Drawing.Color.Red;

            if (!backgroundSyncData.IsBusy)
            {
                backgroundSyncData.RunWorkerAsync();
            }
        }
        private void UpdateFail2()
        {
            string C_STYLE = style;
            string groupsum = GROUPSUM;

            if (errorTouch == null || errorTouch.Rows.Count == 0)
                return;

            string po = txtPo.Text.Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(po))
                po = "N/A";

            int index = 0;

            foreach (DataRow row in errorTouch.Rows)
            {
                reasonID = row["REASON_ID"].ToString();
                partID = row["PART_ID"].ToString();

                string time = DateTime.Now.AddSeconds(index).ToString("yyyyMMddHHmmss");
                index++;

                string log = time + ";" +
                             spDeptCode + ";" +
                             LineName + ";" +
                             ipAddress + ";" +
                             C_STYLE + ";" +
                             partID + ";" +
                             reasonID + ";" +
                             groupsum + ";" +
                             "1" + ";" +
                             "0" + ";" +
                             "2" + ";" +
                             po;

                if (WriteFailToLogFile(log))
                {
                    TotalDefect++;
                    this.lblFailTotal.Text = "FAIL: " + TotalDefect;
                }
            }

            errorTouch.Rows.Clear();
        }
        private void SetButtonsEnabledRecursive(Control parent, bool enabled, Color foreColor)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Enabled = enabled;
                    btn.ForeColor = foreColor;
                }
                else if (ctrl is TableLayoutPanel tlp)
                {
                    SetButtonsEnabledRecursive(tlp, enabled, foreColor);
                }
                else if (ctrl.HasChildren)
                {
                    SetButtonsEnabledRecursive(ctrl, enabled, foreColor);
                }
            }
        }
        private void panel3_Paint(object sender, PaintEventArgs e)
        {

        }

        private void tableLayoutPanel10_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel21_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
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

        private void txtPo_TextChanged(object sender, EventArgs e)
        {
            string text = txtPo.Text.Trim();

            if (text == "" || text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                btnNhapPo.Text = "NHẬP PO";
                txtPo.Focus();
            }
            else
            {
                btnNhapPo.Text = "OK";
            }
        }
        bool isNhapMode = true;
        private void btnNhapPo_Click(object sender, EventArgs e)
        {
            if (isNhapMode)
            {
                txtPo.Enabled = true;
                txtPo.Text = "";
                txtPo.Focus();
                //OpenKeyboard();

                btnNhapPo.Text = "OK";
                isNhapMode = false;
                return;
            }

            string po = txtPo.Text.Trim();

            if (string.IsNullOrEmpty(po))
            {
                MessageBox.Show("Vui lòng nhập PO");
                return;
            }
            //if (!System.Text.RegularExpressions.Regex.IsMatch(txtPo.Text.Trim(), @"^\d+$"))
            //{
            //    MessageBox.Show("PO sai định dạng");
            //    txtPo.Focus();
            //    return;
            //}
            string groupName = "";
            string model = "";

            try
            {
                model = GetModelFromPO(po, out groupName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mạng, vui lòng gọi IT");
                return;
            }

            currentGroup = groupName;

            if (string.IsNullOrEmpty(model))
            {
                MessageBox.Show("Không tìm thấy PO này!!!");
                txtPo.Text = "";
                return;
            }

            LoadImageFromGroup(groupName);
            btnFail.Enabled = false;
            btnReFail.Enabled = false;
            btnNhapPo.Text = "NHẬP PO MỚI";
            isNhapMode = true;
            //CloseKeyboard();
            txtPo.Enabled = false;
            //btnNhapPo.Text = "NHẬP PO";
            //txtPo.Text = model;
        }
        private void LoadImageFromGroup(string groupName)
        {

            try
            {
                if (string.IsNullOrEmpty(groupName))
                {
                    ShowMessage("Không có GROUP_NAME để load hình", Color.Red);
                    return;
                }
                //CloseKeyboard();
                string fileName = FormatGroupForFTP(groupName); // format lại
                string ftpPath = $"ftp://192.168.1.15/MES/BTS/{spDeptCode}_{groupName}.jpg";

                // DEBUG
                //MessageBox.Show(ftpPath);

                Bitmap bm = ByteToImage(GetImgByte(ftpPath));

                if (bm != null)
                {
                    picShoeImage.Image = bm;
                    btnPass.Enabled = true;
                    btnRePass.Enabled = true;
                }
                else
                {
                    ShowMessage("Không tìm thấy hình cho MODEL: " + groupName, Color.Red);
                    btnPass.Enabled = false;
                    btnRePass.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi load hình: " + ex.Message, Color.Red);
                btnPass.Enabled = false;
                btnRePass.Enabled = false;
            }
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

        private void ShowMessage(string message, Color color)
        {
            txtMessage.Text = message;
            txtMessage.ForeColor = color;

            //timerStopMessage.Interval = 3000; // 3 giây
            timerStopMessage.Start();
        }
        private string FormatGroupForFTP(string group)
        {
            return group.Replace("/", "").Replace(" ", ""); // hoặc thêm logic khác nếu server khác
        }
        private string style;
        private string GROUPSUM;
        private string GetModelFromPO(string po, out string groupName)
        {
            groupName = "";

            string query = $@"
        SELECT C_STYLE, GROUP_NAME, GROUP_SUM
        FROM MES.TRTB_M_TRACKING_TMU3235
        WHERE PO_NUM = '{po}'";

            DataTable dt = crud.dac.DtSelectExcuteWithQuery(query);

            if (dt != null && dt.Rows.Count > 0)
            {
                GROUPSUM = dt.Rows[0]["GROUP_SUM"].ToString();
                style = dt.Rows[0]["C_STYLE"].ToString();
                groupName = dt.Rows[0]["GROUP_NAME"].ToString();
                return dt.Rows[0]["C_STYLE"].ToString();
            }

            return "";
        }

        private void timerStopMessage_Tick(object sender, EventArgs e)
        {
            //txtMessage.Text = "THÔNG BÁO";
            txtMessage.ForeColor = Color.Red;
            timerStopMessage.Enabled = false;
            timerSuccess.Enabled = false;
            timerError.Enabled = false;
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
        private static string partID;
        private string LeftOrRight;
        private void btnError_Click(object sender, EventArgs e)
        {
            SetInspectionActionState(false, true, false, true);


            Application.DoEvents();
            //TotalDefect = TotalDefect + 1;


            //if (tongProd != 0)
            //{
            //    this.txtDefectQty.Text = "TOTAL : " + TotalDefect + "(" + Math.Round(((float)TotalDefect / tongProd * 100), 2) + "%)";
            //}
            //else
            //{
            //    this.txtDefectQty.Text = "TOTAL : " + TotalDefect;
            //}

            ConffigErrorButton(false);

            Button btn = (Button)sender;
            // MessageBox.Show("Text: " + btn.Text + "\nID: " + btn.AccessibleName);

            reasonID = btn.AccessibleName;
            btn.BackColor = ReasonButtonSelectedColor;
            btn.ForeColor = Color.Black;


            errorTouch.Rows.Add(new object[] { C_Line, partID, reasonID, currentGroup, LeftOrRight, ipAddress });

            //DataRow dr = dtReason.NewRow();
            //dr["PART"] = partID;
            //dr["REASON"] = reasonID;
            //dr["USE_YN"] = "Y";
            //dtReason.Rows.Add(dr);
            txtReady.Text = "OK";


            if (!backgroundProduction.IsBusy)
            {
                backgroundProduction.RunWorkerAsync();
            }
        }
        // Đây là phần code từ 2019 không dùng vì 2019 layout 9 lồng layout 10 còn 2026 không lồng nhau nha
        private void ConffigErrorButton(bool visible)
        {
            foreach (Button btnID in GetReasonButtons())
            {
                btnID.Enabled = visible;
                btnID.BackColor = visible ? ReasonButtonDefaultColor : BlendColor(ReasonButtonDefaultColor, SystemColors.Control, 0.65f);
                btnID.ForeColor = visible ? Color.Black : Color.DarkGray;
            }
           
        }

        private IEnumerable<Button> GetReasonButtons()
        {
            Control[] reasonContainers =
            {
                tableLayoutPanel7,
                tableLayoutPanel9
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
            Button[] actionButtons = { btnPass, btnFail, btnRePass, btnReFail, btnClear };

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
                btn.BackColor = ReasonButtonDefaultColor;
                btn.ForeColor = SystemColors.ControlText;
            }
        }

        private void ResetPartLabelColors()
        {
            lblPart1.ForeColor = Color.Red;
            lblPart2.ForeColor = Color.Red;
            lblPart3.ForeColor = Color.Red;
            lblPart4.ForeColor = Color.Red;
        }

        
        private void backgroundProduction_DoWork(object sender, DoWorkEventArgs e)
        {
            Production();
        }
        private int sensorCount;
        private int TotalDefect;
        public float tongProd = 0;
        private void Production()
        {
            Action updateUI = () =>
            {
                txtSensorCount.Text = sensorCount.ToString();

                if (tongProd != 0)
                {
                    txtDefectQty.Text = "TOTAL : " + TotalDefect + " (" + Math.Round(((float)TotalDefect / tongProd * 1000000.0), 1) + " )";
                }
                else
                {
                    txtDefectQty.Text = "TOTAL : " + TotalDefect;
                }

                DataTable topdefect = GetDataTopDefect(C_Line);

                if (chkVN.Checked)
                {
                    lblTop1Defect.Text = topdefect.Rows.Count > 0 ? topdefect.Rows[0]["TOP_DEFECT_VN"].ToString() : "";
                    lblTop2Defect.Text = topdefect.Rows.Count > 1 ? topdefect.Rows[1]["TOP_DEFECT_VN"].ToString() : "";
                    lblTop3Defect.Text = topdefect.Rows.Count > 2 ? topdefect.Rows[2]["TOP_DEFECT_VN"].ToString() : "";
                }
                else
                {
                    lblTop1Defect.Text = topdefect.Rows.Count > 0 ? topdefect.Rows[0]["TOP_DEFECT_EN"].ToString() : "";
                    lblTop2Defect.Text = topdefect.Rows.Count > 1 ? topdefect.Rows[1]["TOP_DEFECT_EN"].ToString() : "";
                    lblTop3Defect.Text = topdefect.Rows.Count > 2 ? topdefect.Rows[2]["TOP_DEFECT_EN"].ToString() : "";
                }
            };

            if (this.InvokeRequired)
            {
                // this.Invoke(updateUI);
                this.BeginInvoke(updateUI);
            }
            else
            {
                updateUI();
            }
        }
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
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
        private void SetErrorToButton(string type)
        {
            DataTable DefectLibary = GetError(type);
            foreach (Button btnID in tableLayoutPanel7.Controls)
            {

                foreach (DataRow dr in DefectLibary.Rows)
                {
                    if (btnID.AccessibleName == dr["REASON_ID"].ToString())
                    {
                        if (chkVN.Checked)
                        {
                            btnID.Text = dr["REASON_VN"].ToString() + " (" + btnID.AccessibleName + ")"; if (Reason.ContainsKey(btnID.AccessibleName))
                            {
                                Reason[btnID.AccessibleName] = btnID.Text;
                                Console.WriteLine("BTN: " + btnID.Name + " | AccessibleName: " + btnID.AccessibleName);

                            }
                            else
                            {
                                Reason.Add(btnID.AccessibleName, btnID.Text);
                            }
                        }
                        else
                        {
                            btnID.Text = dr["REASON_EN"].ToString() + " (" + btnID.AccessibleName + ")"; if (Reason.ContainsKey(btnID.AccessibleName))
                            {
                                Reason[btnID.AccessibleName] = btnID.Text;

                            }
                            else
                            {
                                Reason.Add(btnID.AccessibleName, btnID.Text);
                            }
                        }
                    }
                }
            }
            //foreach (var a in tableLayoutPanel9.Controls)
            //{
            //    if (a.ToString() == "DevExpress.XtraEditors.SimpleButton")
            //    {
            //        SimpleButton btnID = (SimpleButton)a;
            //        foreach (DataRow dr in DefectLibary.Rows)
            //        {
            //            if (btnID.AccessibleName == dr["REASON_ID"].ToString().Trim())
            //            {
            //                if (chkVN.Checked)
            //                {
            //                    btnID.Text = dr["REASON_VN"].ToString();
            //                    if (Reason.ContainsKey(btnID.AccessibleName))
            //                    {
            //                        Reason[btnID.AccessibleName] = btnID.Text;

            //                    }
            //                    else
            //                    {
            //                        Reason.Add(btnID.AccessibleName, btnID.Text);
            //                    }
            //                }
            //                else
            //                {
            //                    btnID.Text = dr["REASON_EN"].ToString();
            //                    if (Reason.ContainsKey(btnID.AccessibleName))
            //                    {
            //                        Reason[btnID.AccessibleName] = btnID.Text;

            //                    }
            //                    else
            //                    {
            //                        Reason.Add(btnID.AccessibleName, btnID.Text);

            //                    }
            //                }
            //                break;
            //            }
            //        }
            //    }
            foreach (Control panel in tableLayoutPanel9.Controls)
            {
                if (panel is Button btnDirect)
                {
                    foreach (DataRow dr in DefectLibary.Rows)
                    {
                        if (btnDirect.AccessibleName.Trim() == dr["REASON_ID"].ToString().Trim())
                        {
                            btnDirect.Text = chkVN.Checked
     ? dr["REASON_VN"].ToString() + " (" + btnDirect.AccessibleName + ")"
     : dr["REASON_EN"].ToString() + " (" + btnDirect.AccessibleName + ")";

                            if (Reason.ContainsKey(btnDirect.AccessibleName))
                                Reason[btnDirect.AccessibleName] = btnDirect.Text;
                            else
                                Reason.Add(btnDirect.AccessibleName, btnDirect.Text);

                            break;
                        }
                    }
                }

                foreach (Control a in panel.Controls)
                {
                    if (a is Button btnID)

                    {

                        foreach (DataRow dr in DefectLibary.Rows)
                        {
                            if (btnID.AccessibleName.Trim() == dr["REASON_ID"].ToString().Trim())
                            {
                                btnID.Text = chkVN.Checked
     ? dr["REASON_VN"].ToString() + " (" + btnID.AccessibleName + ")"
     : dr["REASON_EN"].ToString() + " (" + btnID.AccessibleName + ")";

                                if (Reason.ContainsKey(btnID.AccessibleName))
                                    Reason[btnID.AccessibleName] = btnID.Text;
                                else
                                    Reason.Add(btnID.AccessibleName, btnID.Text);

                                break;
                            }
                        }
                    }
                }
            }


            //}
            //foreach (SimpleButton btnID in tablelayoutErrorCamera.Controls)
            //{
            //    foreach (DataRow dr in DefectLibary.Rows)
            //    {
            //        if (btnID.AccessibleName == dr["REASON_ID"].ToString())
            //        {
            //            if (chkVN.Checked)
            //            {
            //                btnID.Text = dr["REASON_VN"].ToString();
            //            }
            //            else
            //            {
            //                btnID.Text = dr["REASON_EN"].ToString();
            //            }
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

        private void lblPart3_Click(object sender, EventArgs e)
        {
            LeftOrRight = "";
            if (txtPo.Text == "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                MessageBox.Show("Nhập PO trước khi chấm lỗi. !! Please fill in PO", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            ResetPartLabelColors();
            SetInspectionActionState(false, false, false, false);

            Label lbl = (Label)sender;
            lbl.ForeColor = Color.Green;
            ConffigErrorButton(true);
            ResetReasonButtonColors();
            timerTouch.Enabled = true;
            partID = lbl.AccessibleName;
        }

        private void timerTouch_Tick(object sender, EventArgs e)
        {
            ResetPartLabelColors();
            ConffigErrorButton(false);
            ResetReasonButtonColors();
            StringBuilder query = new StringBuilder();
            errorTouch.Rows.Clear();
            timerTouch.Enabled = false;
            SetInspectionActionState(true, false, true, false);
        }
        private static int TotalPass;
        private void btnPass_Click(object sender, EventArgs e)
        {
            if (txtPo.Text == "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);

                return;
            }
            string C_STYLE = style;
            string po = txtPo.Text.Trim().Replace("'", "''");
            double a = new Random().Next(0, 60);

            if (toggleSwitchOnline.Checked)
            {
                UpdatePassFailDirectToDB(true, "1");
            }
            else
            {



                if (WritePassToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + "STF" + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + "0" + ";" + "0" + ";" + "0" + ";" + 1 + ";" + 0 + ";" + "1" + ";" + po))
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
        private string Mes_Group_Sum;
        private static string spLine;
        //private static int TotalPass;
        private bool UpdatePassFailDirectToDB(bool PassorFail, string seq)
        {
            string C_STYLE = currentGroup;
            string po = txtPo.Text.Trim().Replace("'", "''");
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
                        query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_PASS,SEQ,I_PO_NO)");
                        query.AppendLine("VALUES( '" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "'");
                        query.AppendLine("       ,'" + spDeptCode + "'");
                        query.AppendLine("       ,'" + LineName + "'");
                        query.AppendLine("       ,'" + ipAddress + "'");
                        query.AppendLine("       ,'" + C_STYLE + "'");
                        query.AppendLine("       ,'0'");
                        query.AppendLine("       ,'0'");
                        query.AppendLine("       ,'" + eol_Sequence + "'");
                        query.AppendLine("       ,'1','" + seq + "'  ");
                        query.AppendLine("       ,'" + po + "'");
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
                    query.AppendLine(" PART_ID, REASON_ID, MES_GROUP_SUM, USER_ID, IP_ADDRESS,I_PO_NO)                                                                  ");
                    query.AppendLine("VALUES ('" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "', '" + spLine + "',                                                                            ");
                    query.AppendLine(" '" + partID + "', '" + reasonID + "','" + Mes_Group_Sum + "','" + LeftOrRight + "','" + ipAddress + "','" + po + "')                ");

                    if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                    {

                        if (C_STYLE != "")
                        {
                            query = new StringBuilder();
                            query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_FAIL,SEQ,I_PO_NO)              ");
                            query.AppendLine(" VALUES( '" + DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + "'                                                                                                          ");
                            query.AppendLine("        ,'" + spDeptCode + "'                                                                                                          ");
                            query.AppendLine("        ,'" + LineName + "'                                                                                                            ");
                            query.AppendLine("        ,'" + ipAddress + "'                                                                                                           ");
                            query.AppendLine("        ,'" + C_STYLE + "'                                                                                                       ");
                            query.AppendLine("        ,'" + partID + "'                                                                                                              ");
                            query.AppendLine("        ,'" + reasonID + "'                                                                                                            ");
                            query.AppendLine("        ,'" + eol_Sequence + "'                                                                                                        ");
                            query.AppendLine("        ,'1','" + seq + "'  ,'" + po + "'                                                                                                                         ");
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
                //txtMessage.Text = "Sync data to server...";
            }
            else if (e.ProgressPercentage == 100)
            {
                //ShowMessage("Sync data to server successfully", Color.Blue);
            }
            if (!CheckNetworkConnection())
            {
                //ShowMessage("Rớt mạng rồi", Color.Red);
            }
            else
            {
                //ShowMessage("THÔNG BÁO", Color.Blue);
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
            try
            {
                string[] PassArray = etc.ReadFromFileNotMesage("Pass", "PassBTS_" + DateTime.Now.ToString("yyyyMMdd"));
                if (PassArray != null)
                {
                    for (int i = 0; i < PassArray.Count(); i++)
                    {
                        string[] passParamenter = PassArray[i].ToString().Split(';');
                        string po = "";
                        if (passParamenter.Count() >= 12)
                        {
                            P_d_gather = ""; P_spDeptCode = ""; P_LineName = ""; P_ipAddress = ""; P_C_STYLE = "";
                            P_d_gather = passParamenter[0];
                            P_spDeptCode = passParamenter[1];
                            P_LineName = passParamenter[2];
                            P_ipAddress = passParamenter[3];
                            P_C_STYLE = passParamenter[4];
                            P_Inserted = passParamenter[9];
                            P_Seq = passParamenter[10];
                            po = passParamenter[11];

                            if (P_Inserted == "0")
                            {
                                StringBuilder strSql = new StringBuilder();
                                strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval, TO_CHAR(sysdate,'yyyyMMddHH24MISS') D_GATHER FROM DUAL");

                                DataTable dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    eol_Sequence = dt.Rows[0][0].ToString();
                                }
                                else return false;

                                if (P_C_STYLE != "")
                                {
                                    StringBuilder query = new StringBuilder();
                                    query.AppendLine("INSERT INTO EOL_DEFECT_GATHER (D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_PASS,SEQ,I_PO_NO)");
                                    query.AppendLine("VALUES( '" + P_d_gather + "'");
                                    query.AppendLine("       ,'" + P_spDeptCode + "'");
                                    query.AppendLine("       ,'" + P_LineName + "'");
                                    query.AppendLine("       ,'" + P_ipAddress + "'");
                                    query.AppendLine("       ,'" + P_C_STYLE + "'");
                                    query.AppendLine("       ,'0'");
                                    query.AppendLine("       ,'0'");
                                    query.AppendLine("       ,'" + eol_Sequence + "'");
                                    query.AppendLine("       ,'1','" + P_Seq + "','" + po + "'");
                                    query.AppendLine("      )");

                                    if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                    {
                                        passParamenter[9] = "1";
                                        WritePassToLogTempFile(string.Join(";", passParamenter));
                                    }
                                    else
                                    {
                                        WritePassToLogTempFile(PassArray[i].ToString());
                                    }
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

                        if (failParamenter.Length >= 11)
                        {
                            string po = "";
                            if (failParamenter.Length >= 11 && !string.IsNullOrEmpty(failParamenter[11]))
                                po = failParamenter[11];
                            else if (failParamenter.Length > 5)
                                po = failParamenter[5];

                            if (string.IsNullOrEmpty(po))
                                po = "N/A";

                            P_d_gather = failParamenter[0];
                            P_spDeptCode = failParamenter[1];
                            P_LineName = failParamenter[2];
                            P_ipAddress = failParamenter[3];
                            P_C_STYLE = failParamenter[4];

                            partID = failParamenter[5];
                            reasonID = failParamenter[6];
                            Mes_Group_Sum = failParamenter[7];

                            P_Inserted = failParamenter[9];
                            P_Seq = failParamenter.Length > 10 ? failParamenter[10] : "0";

                            if (P_Inserted == "0")
                            {
                                StringBuilder strSql = new StringBuilder();
                                strSql.AppendLine("SELECT MES.EOL_DEFECT_GATHER_S.nextval FROM DUAL");

                                DataTable dt = crud.dac.DtSelectExcuteWithQuery(strSql.ToString());
                                if (dt.Rows.Count > 0)
                                {
                                    eol_Sequence = dt.Rows[0][0].ToString();
                                }
                                else return false;
                                string mappedLine = GetLineNameSync(P_ipAddress);
                                if (string.IsNullOrEmpty(mappedLine)) mappedLine = P_LineName;
                                StringBuilder query = new StringBuilder();
                                query.AppendLine("INSERT INTO MES.TRTB_M_BTS_COUNT3");
                                query.AppendLine("(D_GATHER, C_LINE, PART_ID, REASON_ID, Q_COUNT, IP_ADDRESS, MES_GROUP_SUM, USER_ID, I_PO_NO)");
                                query.AppendLine("VALUES");
                                query.AppendLine("('" + P_d_gather + "'");
                                query.AppendLine(",'" + mappedLine + "'");
                                query.AppendLine(",'" + partID + "'");
                                query.AppendLine(",'" + reasonID + "'");
                                query.AppendLine(",'1'");
                                query.AppendLine(",'" + P_ipAddress + "'");
                                query.AppendLine(",'" + GROUPSUM + "'");
                                query.AppendLine(",'" + LeftOrRight + "'");
                                query.AppendLine(",'" + po + "')");

                                if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                {
                                    if (!string.IsNullOrEmpty(P_C_STYLE))
                                    {
                                        query = new StringBuilder();

                                        query.AppendLine("INSERT INTO EOL_DEFECT_GATHER");
                                        query.AppendLine("(D_GATHER, DEPT_CODE, C_LINE, IP_ADDRESS, C_STYLE, PART_ID, REASON_ID, SEQUENCE_ID, Q_PASS, Q_FAIL, SEQ, I_PO_NO)");
                                        query.AppendLine("VALUES");
                                        query.AppendLine("('" + P_d_gather + "'");
                                        query.AppendLine(",'" + P_spDeptCode + "'");
                                        query.AppendLine(",'" + P_LineName + "'");
                                        query.AppendLine(",'" + P_ipAddress + "'");
                                        query.AppendLine(",'" + P_C_STYLE + "'");
                                        query.AppendLine(",'" + partID + "'");
                                        query.AppendLine(",'" + reasonID + "'");
                                        query.AppendLine(",'" + eol_Sequence + "'");
                                        query.AppendLine(",'0'");
                                        query.AppendLine(",'1'");
                                        query.AppendLine(",'" + P_Seq + "'");
                                        query.AppendLine(",'" + po + "')");

                                        if (crud.dac.IUExcuteWithQueryReturn(query.ToString()))
                                        {
                                            failParamenter[9] = "1";
                                            WriteFailToLogTempFile(string.Join(";", failParamenter));
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
                else return false;

                return true;
            }
            catch (Exception)
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
        private string GetLineNameSync(string ip)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT SUBSTR(C_COMCODE,4,4) C_COMCODE, ");
            query.AppendLine("  CASE WHEN SUBSTR(C_COMCODE,4,2) = 'P7' THEN SUBSTR(C_COMCODE,4,4) ");
            query.AppendLine("       ELSE DECODE(SUBSTR(C_COMCODE, 6, 1), 'A','P1','B','P2','C','P3','D','P4','E','P5','F','P6','PP') || ");
            query.AppendLine("            CASE WHEN SUBSTR(C_COMCODE,7,1) >= 'A' THEN TO_CHAR(ASCII(SUBSTR(C_COMCODE,7,1))-55) ");
            query.AppendLine("                 ELSE '0' || SUBSTR(C_COMCODE,7,1) END ");
            query.AppendLine("  END SHOW_LINE ");
            query.AppendLine("FROM (SELECT SUBSTR(C_COMCODE,1,7) C_COMCODE, N_COMNAME ");
            query.AppendLine("      FROM TRTB_M_COMMON ");
            query.AppendLine("      WHERE C_GROUP='BTS' AND N_COMNAME='" + ip + "')");

            DataTable dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
                return dt.Rows[0]["C_COMCODE"].ToString();
            return null;
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
                this.lblProdTotal.Text = "SX : " + dt.Rows[0]["PROD_QTY"].ToString();
                this.lblPassTotal.Text = "PASS: " + TotalPass;
                //TotalPass = Convert.ToInt32(dt.Rows[0]["Q_PASS"].ToString());
                this.lblFailTotal.Text = "FAIL : " + TotalDefect;
                //this.lblFailTotal.Text = "FAIL : " + TotalDefect;
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

            if (txtPo.Text != "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                //UpdateExecuteSensor();
            }
        }
        //private string finishedCountScan;
        void setDataProduction()
        {
            Production();
            SetTouchCount();
            GetProdInformation();
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
            DataTable offline_TouchCount = new DataTable();
            offline_TouchCount.Columns.Add("Q_TOTAL");
            offline_TouchCount.Columns.Add("Q_PASS");
            offline_TouchCount.Columns.Add("Q_FAIL");

            int total = PassQty() + FailQty();
            offline_TouchCount.Rows.Add(total, PassQty(), FailQty());
            TouchCount = offline_TouchCount;
        }
        private DataTable TouchCount;

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
        public DataTable productionData;
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
                //label7.Text = "Remark : ";
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
                    label21.Text = "Remark : " + dt.Rows[0]["C_REMARKS"].ToString();
                }
                else
                {
                    label21.Text = "Remark : ";
                }
            }
            catch (Exception ex)
            {

            }

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
            query.AppendLine("                 'STF'                                                                                                         ");
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
        private DataTable ErrorCount;
        private DataTable RFT_DPPM;
        private DataTable Top3DPPM;
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
            query.AppendLine("where a.reason_id = b.reason_id  AND DEPT_CODE = 'STF'                                        ");
            query.AppendLine(" and ROWNUM <= 3                                                         ");
            query.AppendLine("order by 3 desc                                                          ");





            Top3DPPM = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            return Top3DPPM;

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
        private void backgroundOracle_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 10)
            {
                //ShowMessage("Background worker oracle running....", Color.Blue);
            }
            else if (e.ProgressPercentage == 100)
            {
                //ShowMessage("Background worker oracle finish", Color.Blue);
            }
        }

        private void backgroundOracle_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BindProductQTY();
            BindTop3DDPMRFT();
            ConfigCountErrorButton(ErrorCount);
        }
        private void ResetLabelCount(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    if (!string.IsNullOrEmpty(lbl.AccessibleName) && lbl.AccessibleName.StartsWith("C"))
                    {
                        lbl.Text = "0";
                    }
                }

                if (ctrl.HasChildren)
                {
                    ResetLabelCount(ctrl);
                }
            }
        }
        private void ConfigCountErrorButton(DataTable dataErrorCnt)
        {
            try
            {
                tableLayoutPanel2.SuspendLayout();
                tableLayoutPanel7.SuspendLayout();
                tableLayoutPanel9.SuspendLayout();

                if (dataErrorCnt == null || dataErrorCnt.Rows.Count == 0)
                {
                    ResetLabelCount(tableLayoutPanel2);
                    ResetLabelCount(tableLayoutPanel7);
                    ResetLabelCount(tableLayoutPanel9);
                }
                else
                {
                    foreach (DataRow row in dataErrorCnt.Rows)
                    {
                        if (row["GR"] == null) continue;

                        string id = row["REASON_ID"].ToString();
                        int count = Convert.ToInt32(row["CNT"]);

                        if (row["GR"].ToString() == "ERROR")
                        {
                            BindCountToErrorLabel(id, count);
                        }
                        else if (row["GR"].ToString() == "PART")
                        {
                            BindCountToErrorPartLabel(id, count);
                        }
                    }
                }

                tableLayoutPanel2.ResumeLayout();
                tableLayoutPanel7.ResumeLayout();
                tableLayoutPanel9.ResumeLayout();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BindCountToErrorPartLabel(string id, int cnt)
        {
            foreach (var a in picShoeImage.Controls)
            {
                if (a.ToString() == "DevExpress.XtraEditors.LabelControl")
                {
                    Label lbl = (Label)a;
                    if (lbl.AccessibleName != null)
                    {
                        if (lbl.AccessibleName == "CP" + id)
                            lbl.Text = "(" + cnt + ")";
                    }
                    else
                    {
                        string sss = "";
                        sss = lbl.Name.ToString();
                    }
                }
            }
        }
        private void BindCountToErrorLabel(string id, int cnt)
        {
            tableLayoutPanel7.SuspendLayout();
            tableLayoutPanel9.SuspendLayout();

            UpdateLabelRecursive(tableLayoutPanel7, id, cnt);
            UpdateLabelRecursive(tableLayoutPanel9, id, cnt);

            tableLayoutPanel7.ResumeLayout();
            tableLayoutPanel9.ResumeLayout();
        }

        private void UpdateLabelRecursive(Control parent, string id, int cnt)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label lbl)
                {
                    if (lbl.AccessibleName == "C" + id)
                    {
                        lbl.Text = "(" + cnt + ")";
                    }
                }

                if (ctrl.HasChildren)
                {
                    UpdateLabelRecursive(ctrl, id, cnt);
                }
            }
        }
        private DataTable ProductionInformation;
        private void BindProductQTY()
        {
            if (ProductionInformation != null)
            {
                if (ProductionInformation.Rows.Count > 0)
                {
                    this.lblProdTotal.Text = "SX : " + ProductionInformation.Rows[0]["PROD_QTY"].ToString();
                    lblPassTotal.Text = "PASS: " + ProductionInformation.Rows[0]["PROD_QTY"].ToString();
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
                        //lbl1stPass.Text = "1st PASS : " + RFT_DPPM.Rows[0]["FIRST_INS_PASS"].ToString();
                        lblPassTotal.Text = "PASS : " + RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString();
                        TotalPass = Convert.ToInt32(RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString());
                        if (!int.TryParse(RFT_DPPM.Rows[0]["TOTAL_INS_PASS"].ToString(), out totalpass))
                        {
                            totalpass = 0;
                        }
                        TotalPass = totalpass;
                        //lblFailTotal.Text = "FAIL : " + RFT_DPPM.Rows[0]["TTL_DEFECT"].ToString();

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
                   // ShowMessage("No Data DPPM now.", Color.Red);
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void lblTop3Defect_Click(object sender, EventArgs e)
        {

        }

        private void timerBindProduction_Tick(object sender, EventArgs e)
        {
            if (!backgroundOracle.IsBusy)
            {
                backgroundOracle.RunWorkerAsync();
            }
        }
        private static string Comname;
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
                        //btn_reasonCode3.BackColor2 = System.Drawing.Color.DarkGreen;
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
                        //btn_reasonCode3.BackColor2 = System.Drawing.Color.DarkGreen;
                    });
                }
                else
                {
                    var result = TurnOnAndonAsync("G");
                    //if (result.IsCompleted)
                    //{
                    //    btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Calling";
                    //    timer_BlinkButtonGreen.Enabled = true;
                    //}
                    btn_reasonCode3.Text = "(Andon) Gọi Sản Xuất " + Environment.NewLine + "Calling";
                    timer_BlinkButtonGreen.Enabled = true;
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

        private static List<string> btnfail;
        private void btnFail_Click(object sender, EventArgs e)
        {
            if (txtPo.Text == "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                ShowMessage("Chọn Model trước khi chấm lỗi. !! Please choose model", Color.Red);

                return;
            }
            btnfail = new List<string>();
            if (errorTouch != null && errorTouch.Rows.Count > 0)
            {
                if (toggleSwitchOnline.Checked)
                {
                    UpdatePassFailDirectToDB(false, "1");
                }
                else
                {
                    UpdateFailFromErrorTouch();
                }
            }
            else
            {
                errorTouch?.Clear();
            }
            //this.lblFailTotal.Text = "FAIL :" + TotalDefect;

            //lblRFT.Text = Math.Round(TotalDefect * 1.0 / (TotalDefect + TotalPass * 1.0) * 100, 2) + " %";
            SetInspectionActionState(true, false, true, false);
            ConffigErrorButton(false);
            ResetPartLabelColors();
            ResetReasonButtonColors();
            if (backgroundSyncData.IsBusy)
            {

            }
            else
            {
                backgroundSyncData.RunWorkerAsync();
            }

        }
        private void UpdateFailFromErrorTouch()
        {
            string po = txtPo.Text.Trim();
            string C_STYLE = style;
            string groupsum = GROUPSUM;

            foreach (DataRow row in errorTouch.Rows)
            {
                string partID = row["PART_ID"].ToString();
                string reasonID = row["REASON_ID"].ToString();

                if (WriteFailToLogFile(
                    DateTime.Now.ToString("yyyyMMddHHmmss") + ";" +
                    spDeptCode + ";" +
                    LineName + ";" +
                    ipAddress + ";" +
                    C_STYLE + ";" +
                    partID + ";" +
                    reasonID + ";" +
                    groupsum + ";" +
                    1 + ";" +
                    0 + ";" +
                    "1" + ";" + po))
                {
                    TotalDefect++;
                }
            }

            lblFailTotal.Text = "FAIL :" + TotalDefect;

            errorTouch.Clear();
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

        private void btnRePass_Click(object sender, EventArgs e)
        {
            if (txtPo.Text == "" || txtPo.Text == "Nhấn vào NHẬP PO để nhập PO ->")
            {
                ShowMessage("Nhập PO trước khi chấm lỗi. !! Please fill in PO", Color.Red);
                return;
            }
            string C_STYLE = style;
            double a = new Random().Next(0, 60);

            if (toggleSwitchOnline.Checked)
            {
                UpdatePassFailDirectToDB(true, "2");
            }
            else
            {
                if (WritePassToLogFile(DateTime.Now.AddSeconds(a).ToString("yyyyMMddHHmmss") + ";" + spDeptCode + ";" + LineName + ";" + ipAddress + ";" + C_STYLE + ";" + "0" + ";" + "0" + ";" + "0" + ";" + 1 + ";" + 0 + ";" + "2" + ";" + txtPo.Text))
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
        private DataTable dtReason;
        private void btnClear_Click(object sender, EventArgs e)
        {
            {
                bool visible = false;
                if (dtReason != null)
                {
                    dtReason.Clear();

                }
                SetInspectionActionState(true, false, true, false);
                ConffigErrorButton(false);
                ResetPartLabelColors();
                ResetReasonButtonColors();
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

        private void btn_reasonCode1_Click_1(object sender, EventArgs e)
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

        private void label7_Click(object sender, EventArgs e)
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

        private void label8_Click(object sender, EventArgs e)
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

        private void label11_Click_1(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
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

            SafeInvoke(() =>
            {
                _micButton.Enabled = true;
                _micButton.Text = "🎙 Giọng nói";
                _micButton.BackColor = SystemColors.Control;

                if (!string.IsNullOrWhiteSpace(e.RawText))
                {
                    string normalized = NormalizeVoice(e.RawText);
                    ProcessVoiceCommand(normalized);
                }
            });
        }
        private void ProcessVoiceCommand(string s)
        {
            // ── Repass trước (phải check trước pass) ────────────────────
            if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*pass)\b"))
            {
                btnRePass_Click(btnRePass, EventArgs.Empty);
                ShowMessage("✔ REPASS", Color.LimeGreen);
                return;
            }

            // ── Pass ─────────────────────────────────────────────────────
            if (System.Text.RegularExpressions.Regex.IsMatch(s, @"\bpass\b"))
            {
                btnPass_Click(btnPass, EventArgs.Empty);
                ShowMessage("✔ PASS", Color.LimeGreen);
                return;
            }

            // ── Refail / Fail ─────────────────────────────────────────────
            bool isRefail = System.Text.RegularExpressions.Regex.IsMatch(s, @"\b(re\s*fail)\b");
            bool isFail = !isRefail && System.Text.RegularExpressions.Regex.IsMatch(s, @"\bfail\b");

            if (isFail || isRefail)
            {
                // Vị trí
                var partMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b([abcd])\b");
                if (partMatch.Success)
                {
                    Label targetLbl = partMatch.Value.ToUpper() switch
                    {
                        "A" => lblPart1,
                        "B" => lblPart2,
                        "C" => lblPart3,
                        "D" => lblPart4,
                        _ => null
                    };
                    if (targetLbl != null)
                        lblPart3_Click(targetLbl, EventArgs.Empty);
                }

                // Số lỗi + Fail — delay để lblPart xử lý xong
                var numMatch = System.Text.RegularExpressions.Regex.Match(s, @"\b(\d{2,3})\b");
                Task.Delay(200).ContinueWith(_ =>
                {
                    SafeInvoke(() =>
                    {
                        if (numMatch.Success)
                        {
                            var errBtn = GetReasonButtons()
                                             .FirstOrDefault(b => b.AccessibleName == numMatch.Value);
                            if (errBtn != null)
                                btnError_Click(errBtn, EventArgs.Empty);
                        }

                        Task.Delay(100).ContinueWith(__ =>
                        {
                            SafeInvoke(() =>
                            {
                                if (isRefail) button4_Click(button4, EventArgs.Empty);
                                else btnFail_Click(btnFail, EventArgs.Empty);
                                ShowMessage($"✔ {(isRefail ? "REFAIL" : "FAIL")}", Color.OrangeRed);
                            });
                        });
                    });
                });
                return;
            }

            ShowMessage($"❓ Không nhận ra: \"{s}\"", Color.Orange);
        }
        private string NormalizeVoice(string s)
        {
            s = s.ToLowerInvariant().Trim();

            // ── Vị trí ──────────────────────────────────────────────────
            s = s.Replace("a a", "a");
            s = s.Replace("bê bê", "b"); s = s.Replace("bê", "b");
            s = s.Replace("xê xê", "c"); s = s.Replace("xê", "c");
            s = s.Replace("cê cê", "c"); s = s.Replace("cê", "c");
            s = s.Replace("đê đê", "d"); s = s.Replace("đê", "d");

            // ── Pass / Repass ────────────────────────────────────────────
            s = s.Replace("tái đạt", "repass");
            s = s.Replace("re đạt", "repass");
            s = s.Replace("đạt", "pass");
            s = s.Replace("pát", "pass");
            s = s.Replace("pas", "pass");

            // ── Fail / Refail ────────────────────────────────────────────
            s = s.Replace("tái rớt", "refail");
            s = s.Replace("re rớt", "refail");
            s = s.Replace("tái lỗi", "refail");
            s = s.Replace("re lỗi", "refail");
            s = s.Replace("rớt", "fail");
            s = s.Replace("feel", "fail");
            s = s.Replace("fell", "fail");
            s = s.Replace("phil", "fail");
            s = s.Replace("fai", "fail");

            // ── Số ──────────────────────────────────────────────────────
            s = s.Replace("ba mươi chín", "39");

            s = s.Replace("bốn mươi mốt", "41");
            s = s.Replace("bốn mươi hai", "42");
            s = s.Replace("bốn mươi ba", "43");
            s = s.Replace("bốn mươi bốn", "44");
            s = s.Replace("bốn mươi lăm", "45");
            s = s.Replace("bốn mươi sáu", "46");
            s = s.Replace("bốn mươi bảy", "47");
            s = s.Replace("bốn mươi tám", "48");
            s = s.Replace("bốn mươi chín", "49");
            s = s.Replace("bốn mươi", "40");

            s = s.Replace("năm mươi mốt", "51");
            s = s.Replace("năm mươi hai", "52");
            s = s.Replace("năm mươi ba", "53");
            s = s.Replace("năm mươi bốn", "54");
            s = s.Replace("năm mươi lăm", "55");
            s = s.Replace("năm mươi", "50");

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

        // ── Cleanup ────────────────────────────────────────────────────────

       
    }
}
