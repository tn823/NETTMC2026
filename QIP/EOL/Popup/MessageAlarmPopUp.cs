using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Media;
using QIP.Properties;
using ConnectionClass.Oracle;

namespace QIP.EOL.Popup
{
    public partial class MessageAlarmPopUp : Form
    {

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_APPCOMMAND = 0x319;
        private const int APPCOMMAND_VOLUME_MAX = 0x0a;

        GlobalFunction.PublicFunction etc = new GlobalFunction.PublicFunction();
        private CRUDOracle crud;

        public bool MQTTConnected = false;
        public string MQTTClient = "";
        public static string ipAddress;

        public string MessageText { get; set; }
        public string ImgPath1 { get; set; }
        public string ImgPath2 { get; set; }

        private bool isRed = true;

        public MessageAlarmPopUp()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (isRed)
            {
                lblMessage.BackColor = Color.Red;
                lblMessage.ForeColor = Color.White;
            }
            else
            {
                lblMessage.BackColor = Color.Transparent;
                lblMessage.ForeColor = Color.Black;
            }

            isRed = !isRed;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playaudio()
        {
            try
            {
                string filePath = Application.StartupPath + @"\JSISound.wav";

                if (File.Exists(filePath))
                {
                    SoundPlayer player = new SoundPlayer(filePath);
                    player.PlayLooping(); // 🔥 loop cho giống alarm thật
                }
            }
            catch
            {
            }
        }

        private void MessageAlarmPopUp_Load(object sender, EventArgs e)
        {
            crud = new CRUDOracle("AGMES");

            timer2.Enabled = true;
            timer1.Enabled = true;

            lblMessage.Text = MessageText;
            ipAddress = GlobalFunction.PublicFunction.myIpaddress;

            try
            {
                // 🔥 max volume
                SendMessageW(IntPtr.Zero, WM_APPCOMMAND, IntPtr.Zero, (IntPtr)APPCOMMAND_VOLUME_MAX);

                playaudio();

                Bitmap bm1 = ByteToImage(GetImgByte("ftp://" + etc.FileServerPath + @"/Mes/AGERROR/" + ImgPath1));
                Bitmap bm2 = ByteToImage(GetImgByte("ftp://" + etc.FileServerPath + @"/Mes/AGERROR/" + ImgPath2));

                if (bm1 != null || bm2 != null)
                {
                    pictureBox1.Image = bm1;
                    pictureBox2.Image = bm2;
                }
                else
                {
                    pictureBox1.Image = Resources.sASS_3;
                    pictureBox2.Image = Resources.sASS_3;
                }
            }
            catch
            {
            }
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            if (blob == null) return null;

            using (MemoryStream mStream = new MemoryStream(blob))
            {
                return new Bitmap(mStream);
            }
        }

        public byte[] GetImgByte(string ftpFilePath)
        {
            try
            {
                using (WebClient ftpClient = new WebClient())
                {
                    ftpClient.Credentials = new NetworkCredential("mes", "!angiang3535!");
                    return ftpClient.DownloadData(ftpFilePath);
                }
            }
            catch
            {
                return null;
            }
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
