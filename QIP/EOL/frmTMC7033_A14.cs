using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionClass.Oracle;
using static GlobalFunction.PublicFunction;

namespace QIP.EOL
{
    public partial class frmTMC7033_A14 : UserControl
    {

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
        Dictionary<string, string> Reason = new Dictionary<string, string>();
        GlobalFunction.PublicFunction etc = new GlobalFunction.PublicFunction();
        SYSTEMTIME st = new SYSTEMTIME();


        //main
        public frmTMC7033_A14()
        {
            InitializeComponent();
            crud = new CRUDOracle("VSMES");
        }              

        private void frmTMC7033_A14_Load(object sender, EventArgs e)
        {
            ipAddress = GlobalFunction.PublicFunction.myIpaddress;
            TryToUpdateSystemDateTime();
            BindingControl();
            try
            {

                #region OnlineIfCannotUseExcelLocal
                //GetLineName(ipAddress);
                //GetError(spDeptCode);
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



                //SetTouchCount();
                //BindTouchCount(TouchCount);
                //MQTT_Init();

            }
            catch (Exception ex)
            {


                ShowMessage(ex.ToString() + Environment.NewLine + "MỞ CHUONG TRÌNH KHÔNG ÐƯỢC...RỚT MẠNG HOẶC CHUONG TRÌNH LỖI RỒI. " + Environment.NewLine + " CHƯƠNG TRÌNH SẼ TẮT, THÌ MỞ LẠI. " +
                     Environment.NewLine + " KHÔNG ÐUỢC THÌ GỌI IT " +
                     Environment.NewLine + " SDT : 0903518945. CÁM ON NHIỀU", Color.Red);
                Application.Exit();
            }
        }


        private void ShowMessage(string message, Color color)
        {
            memoEditMessage.Text = "";
            memoEditMessage.Text = message;
            memoEditMessage.ForeColor = color;
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
        private void BindingControl()
        {
            try
            {
                int xPic = pictureShoes.Location.X;
                int yPic = pictureShoes.Location.Y;
                int wPic = pictureShoes.Width;
                int hPic = pictureShoes.Height;
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
        //            lblLineInfo.Text = "Err";
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
        //                lblLineInfo.Text = LineName;
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
        //            DataTable dtLineName = GetDataTable(grdvOfflineData);
        //            LineName = dtLineName.Rows[0]["SHOW_LINE"].ToString();
        //            spLine = dt.Rows[0]["C_COMCODE"].ToString();
        //            lblLineInfo.Text = LineName;
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
        //            lblLineInfo.Text = LineName;
        //        }
        //        if (lblLineInfo.Text != "P114")
        //        {
        //            lblSensorCount.Visible = false;
        //            //label1.Visible = false;
        //        }
        //    });
        //    b.RunWorkerAsync();
        //}
        //private void GetError(string type)
        //{
        //    DataTable dt = new DataTable();

        //    var b = new BackgroundWorker();
        //    b.DoWork += new DoWorkEventHandler(
        //        delegate (object sender, DoWorkEventArgs e)
        //        {
        //            StringBuilder query = new StringBuilder();
        //            query.AppendLine("");
        //            query.AppendLine("        SELECT PART_ID, REASON_ID, REASON_SHORT, REASON_EN, REASON_VN                                ");
        //            query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@inf_m_e                                                                  ");
        //            query.AppendLine("WHERE DEPT_CODE = '" + type + "' AND REASON_ID <= 82                                                  ");
        //            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
        //            e.Result = dt;
        //            dt = (DataTable)e.Result;
        //        }
        //    );
        //    b.RunWorkerCompleted += new RunWorkerCompletedEventHandler(delegate (object sender, RunWorkerCompletedEventArgs e)
        //    {
        //        if (e.Error != null)
        //        {
        //            lblLineInfo.Text = "Err";
        //            if (File.Exists(Application.StartupPath + "\\ErrorButton.xls"))
        //            {
        //                var source = new ExcelDataSource();
        //                source.FileName = Application.StartupPath + "\\ErrorButton.xls";
        //                var worksheetSettings = new ExcelWorksheetSettings("Sheet");
        //                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
        //                source.Fill();
        //                grdOfflineData.DataSource = source;
        //                DataTable dtErrorButton = GetDataTable(grdvOfflineData);
        //                SetErrorToButton(type, dtErrorButton);
        //                ConffigErrorButton(false);
        //            }
        //        }
        //        dt = (DataTable)e.Result;

        //        if (dt == null || dt.Rows.Count < 0)
        //        {
        //            if (File.Exists(Application.StartupPath + "\\ErrorButton.xls"))
        //            {
        //                var source = new ExcelDataSource();
        //                source.FileName = Application.StartupPath + "\\ErrorButton.xls";
        //                var worksheetSettings = new ExcelWorksheetSettings("Sheet");
        //                source.SourceOptions = new ExcelSourceOptions(worksheetSettings);
        //                source.Fill();
        //                grdOfflineData.DataSource = source;
        //                DataTable dtErrorButton = GetDataTable(grdvOfflineData);
        //                SetErrorToButton(type, dtErrorButton);
        //                ConffigErrorButton(false);
        //            }
        //        }
        //        else if (dt.Rows.Count > 0)
        //        {
        //            this.grdOfflineData.DataSource = dt;
        //            XlsExportOptions options = new XlsExportOptions();
        //            if (File.Exists(Application.StartupPath + "\\ErrorButton.xls"))
        //            {
        //                File.Delete(Application.StartupPath + "\\ErrorButton.xls");
        //            }
        //            else
        //            {

        //            }
        //            this.grdOfflineData.ExportToXls(Application.StartupPath + "\\ErrorButton.xls");
        //            SetErrorToButton(type, dt);
        //            ConffigErrorButton(false);
        //        }
        //    });
        //    b.RunWorkerAsync();
        //}
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

        private void btnChonModel_Click(object sender, EventArgs e)
        {

        }
        private void chkPlanOneMonth_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: xử lý
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            // TODO: xử lý
        }

        private void txtTime_Click(object sender, EventArgs e)
        {

        }

        private void lblFailTotal_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void txtTime_Click_1(object sender, EventArgs e)
        {

        }

        private void lblPart1_Click(object sender, EventArgs e)
        {

        }


        private void simpleButton23_Click(object sender, EventArgs e) { }

        private void btnRePass_Click(object sender, EventArgs e) { }
        private void btnPass_Click(object sender, EventArgs e) { }
        private void btnFail_Click(object sender, EventArgs e) { }
        private void btnReFail_Click(object sender, EventArgs e) { }
        private void btnClear_Click(object sender, EventArgs e) { }
        private void simpleButton14_Click(object sender, EventArgs e) { }
        private void btn_reasonCode1_Click(object sender, EventArgs e) { }
        private void btn_reasonCode2_Click(object sender, EventArgs e)
        { }
        private void btn_reasonCode3_Click(object sender, EventArgs e)
        { }
        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void btnSPCCleanliness_Click(object sender, EventArgs e) { }
        private void btnSPCStitching_Click(object sender, EventArgs e) { }
        private void btnSPCBonding_Click(object sender, EventArgs e) { }
        private void labelControl2_Click(object sender, EventArgs e)
        {

        }
        private void labelControl7_Click(object sender, EventArgs e)
        {

        }
    }
}
