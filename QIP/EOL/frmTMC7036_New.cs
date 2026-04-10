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

namespace QIP.EOL
{
    public partial class frmTMC7036_New : UserControl
    {
        public frmTMC7036_New()
        {
            InitializeComponent();
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

        private void frmTMC7036_New_Load(object sender, EventArgs e)
        {
            GetSetButtonLocation();
        }
        private void BindingControl()
        {
            //SetLineName(ipAddress);
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

            //txtSensorCount.Text = sensorCount.ToString();
            //if (serialPort1 == null)
            //{
            //    serialPort1 = new SerialPort();
            //}

            //SetComport();
            //this.btnCapture.Enabled = false;

            int xPic = picShoeImage.Location.X;
            int yPic = picShoeImage.Location.Y;
            int wPic = picShoeImage.Width;
            int hPic = picShoeImage.Height;
        }

        private void button4_Click(object sender, EventArgs e)
        {

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
    }
}
