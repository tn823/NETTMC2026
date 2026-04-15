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
using System.Net;

namespace QIP.EOL.Popup
{
    public partial class SendPictureDefectEOL : Form
    {

        private CRUDOracle crud;

        private string ftp_server;
        private string ftp_id;
        private string ftp_pass;

        public string IPADDRESS
        {
            get;
            set;
        }

        public string LINENAME
        {
            get;
            set;
        }
        public string REASONID
        {
            get;
            set;
        }
        private string filePath1;
        private string filePath2;


        public SendPictureDefectEOL()
        {
            InitializeComponent();
            filePath1 = "";
            filePath2 = "";
        }


        private void SendPictureDefectEOL_Load(object sender, EventArgs e)
        {
            crud = new CRUDOracle("VSMES");

            ftp_server = "ftp://192.168.1.15/EOL_DEFECT_IMAGE";
            ftp_id = "mes";
            ftp_pass = "!saigon3535!";

            Popup.SelectReasonStopLine stopLine = new Popup.SelectReasonStopLine();
            stopLine.dtReason = ProcessTableReason();
            stopLine.ShowDialog(this);
            if (stopLine.ReturnSelection is null || stopLine.ReturnSelection == "")
            {
                this.Close();
            }
            else
            {
                this.btnChonReason.Text = stopLine.ReturnSelection;
                REASONID = GetDataReasonID(stopLine.ReturnSelection);
            }

        }

        private void btnTake1_Click(object sender, EventArgs e)
        {
            OpenCamera camera = new OpenCamera();
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Snapshots";
            camera.cameraId = "1";
            camera.ShowDialog(this);
            if (camera.filepath != null && camera.filepath.ToString() != "")
            {
                pictureBox1.Image = Image.FromFile(path + "\\" + camera.filepath);
                filePath1 = camera.filepath;
            }
            else
            {

            }
        }

        private void btnTake2_Click(object sender, EventArgs e)
        {
            OpenCamera camera = new OpenCamera();
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Snapshots";
            camera.cameraId = "2";
            camera.ShowDialog(this);
            if (camera.filepath != null && camera.filepath.ToString() != "")
            {
                pictureBox2.Image = Image.FromFile(path + "\\" + camera.filepath);
                filePath2 = camera.filepath;
            }
            else
            {

            }
        }

        private void simpleButton2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(REASONID))
            {
                if (!string.IsNullOrEmpty(filePath1) || !string.IsNullOrEmpty(filePath2))
                {
                    if (crud.dac.IUExcuteWithQueryReturn("INSERT INTO TRTB_M_PICTURE_DEFECT_LOG SELECT TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + LINENAME + "','" + IPADDRESS + "', '" + filePath1 + "','" + filePath2 + "', '" + REASONID + "' FROM DUAL"))
                    {
                        try
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Snapshots";
                            string From = path + "\\" + filePath1;
                            string From2 = path + "\\" + filePath2;
                            using (WebClient client = new WebClient())
                            {
                                client.Credentials = new NetworkCredential(ftp_id, ftp_pass);
                                if (filePath1 != "")
                                {
                                    client.UploadFile(ftp_server + "/" + filePath1, WebRequestMethods.Ftp.UploadFile, From);
                                }
                                if (filePath2 != "")
                                {
                                    client.UploadFile(ftp_server + "/" + filePath2, WebRequestMethods.Ftp.UploadFile, From2);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error occurs, check error code and try again " + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK);
                        }
                        this.Close();
                    }
                    else
                    {

                    }
                }
                else if (REASONID == "24")
                {
                    if (crud.dac.IUExcuteWithQueryReturn("INSERT INTO TRTB_M_PICTURE_DEFECT_LOG SELECT TO_CHAR(SYSDATE,'YYYYMMDDHH24MISS'),'" + LINENAME + "','" + IPADDRESS + "', '" + filePath1 + "','" + filePath2 + "', '" + REASONID + "' FROM DUAL"))
                    {
                        try
                        {
                            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Snapshots";
                            string From = path + "\\" + filePath1;
                            string From2 = path + "\\" + filePath2;
                            using (WebClient client = new WebClient())
                            {
                                client.Credentials = new NetworkCredential(ftp_id, ftp_pass);
                                if (filePath1 != "")
                                {
                                    client.UploadFile(ftp_server + "/" + filePath1, WebRequestMethods.Ftp.UploadFile, From);
                                }
                                if (filePath2 != "")
                                {
                                    client.UploadFile(ftp_server + "/" + filePath2, WebRequestMethods.Ftp.UploadFile, From2);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error occurs, check error code and try again " + Environment.NewLine + ex.Message, "Error", MessageBoxButtons.OK);
                        }
                        this.Close();
                    }
                    else
                    {

                    }
                }
                else
                {
                    MessageBox.Show(" Vui lòng chụp hình ảnh lỗi!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show(" Vui lòng chọn lỗi!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnChonReason_Click(object sender, EventArgs e)
        {

        }

        private DataTable ProcessTableReason()
        {
            DataTable oldDataReason = GetDataReason();
            if (oldDataReason == null || oldDataReason.Rows.Count <= 0)
            {
                return null;
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("col1");
            dt.Columns.Add("col2");
            dt.Columns.Add("col3");
            dt.Columns.Add("col4");
            int countcol = 0;
            DataRow dr = dt.Rows.Add();
            for (int a = 0; a < oldDataReason.Rows.Count; a++)
            {
                countcol = countcol + 1;

                switch (countcol)
                {
                    case 1:
                        dr[0] = oldDataReason.Rows[a]["REASON_VN"].ToString();
                        break;
                    case 2:
                        dr[1] = oldDataReason.Rows[a]["REASON_VN"].ToString();
                        break;
                    case 3:
                        dr[2] = oldDataReason.Rows[a]["REASON_VN"].ToString();
                        break;
                    case 4:
                        dr[3] = oldDataReason.Rows[a]["REASON_VN"].ToString();
                        break;
                }
                if (countcol == 4)
                {
                    dr = dt.Rows.Add();
                    countcol = 0;
                }
            }
            return dt;
        }
        private DataTable GetDataReason()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT PART_ID, REASON_ID, REASON_SHORT, REASON_EN, REASON_VN                                ");
            query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@INF_M_E                                                       ");
            query.AppendLine("WHERE DEPT_CODE = 'ASS' AND REASON_ID <= 24                                                  ");

            query.AppendLine("  ");
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            return dt;
        }

        private string GetDataReasonID(string reason_id)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT  REASON_ID, REASON_EN, REASON_VN                                                      ");
            query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@INF_M_E                                                         ");
            query.AppendLine("WHERE DEPT_CODE = 'ASS' AND REASON_ID <= 24                                                  ");
            query.AppendLine(" AND REASON_VN = '" + reason_id + "'                                                         ");
            query.AppendLine("  ");

            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["REASON_ID"].ToString();
            }
            else
            {
                return "";
            }

        }
    }
}
