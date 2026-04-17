using ConnectionClass.Oracle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GlobalFunction.PublicFunction;



namespace DEV.RFID
{
    public partial class RFID_Alarm : UserControl
    {
        private CRUDOracle crud;

        public RFID_Alarm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DataBindMainGrid();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            GlobalFunction.PublicFunction pub = new GlobalFunction.PublicFunction();
            pub.ExportToExcelFile(gridControl2, "Sheet1");
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            using (TimedWebClient wc = new TimedWebClient { Timeout = 2000 })
            {


                string a = String.Format("http://192.168.10.237:88/?button1on");
                //TunrOnOfAlarmSound(false, "one", 1);
                var json = wc.DownloadString(a);
            }
        }
        public class TimedWebClient : WebClient
        {
            // Timeout in milliseconds, default = 600,000 msec
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

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            using (TimedWebClient wc = new TimedWebClient { Timeout = 2000 })
            {

                string a = String.Format("http://192.168.10.237:88/?button1off");
                //TunrOnOfAlarmSound(false, "one", 1);
                var json = wc.DownloadString(a);
            }
        }

        private void RFID_Alarm_Load(object sender, EventArgs e)
        {
            crud = new CRUDOracle("VSMES");

        }
        private void DataBindMainGrid()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT A.GATE_NO,A.GATE_NAME, DATESTR, B.RFID_CODE,E.MODEL_NAME");
            query.AppendLine("FROM trtb_rfid_gate A,");
            query.AppendLine("     (SELECT RFID_CODE, DATESTR, DETECT_GATE_CODE");
            query.AppendLine("        FROM TRTB_RFID_MOVEMENT");
            query.AppendLine("       WHERE (DATESTR, DETECT_GATE_CODE) IN");
            query.AppendLine("             (SELECT MAX(DATESTR), DETECT_GATE_CODE");
            query.AppendLine("                FROM TRTB_RFID_MOVEMENT");
            query.AppendLine("               WHERE DATESTR >= TO_CHAR((SYSDATE - 5 / 24 / 60),'YYYYMMDDHH24MISS')");
            query.AppendLine("            GROUP BY DETECT_GATE_CODE)) B,");
            query.AppendLine("     TRTB_RFID_INFORMATION C, CFM_AD_BARCODE D, CFM_AD_PROGRESS E");
            query.AppendLine("WHERE A.GATE_NO = B.DETECT_GATE_CODE(+)");
            query.AppendLine("AND B.RFID_CODE = C.RFID_CODE(+)");
            query.AppendLine("AND C.CFM_BARCODE = D.I_CARD_NO(+)");
            query.AppendLine("AND D.CFM_ID = E.CFM_ID(+)");
            query.AppendLine("ORDER BY 1");

            DataTable dt = new DataTable();
            crud = new CRUDOracle("VSMES");
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            gridControl1.DataSource = dt;

            gridControl1.ReadOnly = true;

            gridControl1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void DataBindMainGridDetail(string gateNo)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("SELECT A.GATE_NO,A.GATE_NAME, TO_DATE(DATESTR, 'YYYYMMDDHH24MISS') C_DATE");
            query.AppendLine(", B.RFID_CODE,E.MODEL_NAME,E.SEASON,E.CATEGORY,E.STYLE_NO ARTICLE, E.GENDER,E.C_SIZE");
            query.AppendLine("FROM trtb_rfid_gate A,");
            query.AppendLine("     (SELECT * FROM TRTB_RFID_MOVEMENT");
            query.AppendLine("      WHERE DETECT_GATE_CODE = '" + gateNo + "'");
            query.AppendLine("      AND DATESTR >= TO_CHAR(SYSDATE,'YYYYMMDD') || '000000') B,");
            query.AppendLine("     TRTB_RFID_INFORMATION C, CFM_AD_BARCODE D, CFM_AD_PROGRESS E");
            query.AppendLine("WHERE A.GATE_NO = B.DETECT_GATE_CODE");
            query.AppendLine("AND B.RFID_CODE = C.RFID_CODE(+)");
            query.AppendLine("AND C.CFM_BARCODE = D.I_CARD_NO(+)");
            query.AppendLine("AND D.CFM_ID = E.CFM_ID(+)");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());

            gridControl2.DataSource = null;
            gridControl2.Rows.Clear();
            gridControl2.Columns.Clear();

            gridControl2.DataSource = dt;

            gridControl2.ReadOnly = true;

            gridControl2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridControl2.AllowUserToAddRows = false;
            gridControl2.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void gridControl1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var row = gridControl1.Rows[e.RowIndex];

                if (row.Cells["GATE_NO"].Value != null)
                {
                    string gateNo = row.Cells["GATE_NO"].Value.ToString();
                    DataBindMainGridDetail(gateNo);
                }
            }
        }

        private void gridControl1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            var row = gridControl1.Rows[e.RowIndex];

            if (row.Cells["RFID_CODE"].Value != null)
            {
                if (!string.IsNullOrEmpty(row.Cells["RFID_CODE"].Value.ToString()))
                {
                    row.DefaultCellStyle.BackColor = Color.LightPink;
                }
            }
        }
    }
}
