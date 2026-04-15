using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace QIP.EOL.Popup
{
    public partial class OpenCamera : Form
    {
        private FilterInfoCollection videoDevices;
        private VideoCaptureDevice videoSource;
        private Bitmap currentFrame;

        public OpenCamera()
        {
            InitializeComponent();
        }

        //public CameraDevice Device { get; set; }
        public string cameraId
        {
            get;
            set;
        }
        public string filepath
        {
            get;
            set;
        }
        //private void btnTakeAPhoto_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        string path = AppDomain.CurrentDomain.BaseDirectory + "\\Snapshots";
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }

        //        string filePath = Path.Combine(
        //        DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg");
        //        //cameraControl1.TakeSnapshot().Save(filePath, ImageFormat.Jpeg);
        //        if (cameraControl1.TakeSnapshot() != null)
        //        {
        //            //cameraControl1.TakeSnapshot().

        //            cameraControl1.TakeSnapshot().Save(path + "\\" + filePath, ImageFormat.Jpeg);
        //            filepath = filePath;
        //            cameraControl1.Stop();
        //            this.Close();
        //        }
        //        else
        //        {
        //            MessageBox.Show("Vui lòng kết nối camera ", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(" Lỗi " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}
        private void btnTakeAPhoto_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentFrame == null)
                {
                    MessageBox.Show("Vui lòng kết nối camera.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Snapshots");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";
                string fullPath = Path.Combine(path, fileName);

                currentFrame.Save(fullPath, ImageFormat.Jpeg);

                filepath = fileName;

                StopCamera();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        //private void OpenCamera_Load(object sender, EventArgs e)
        //{

        //}
        private void OpenCamera_Load(object sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy camera.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int selectedIndex = 0;

                if (!string.IsNullOrWhiteSpace(cameraId))
                {
                    for (int i = 0; i < videoDevices.Count; i++)
                    {
                        if (videoDevices[i].MonikerString.Contains(cameraId) ||
                            videoDevices[i].Name.Contains(cameraId))
                        {
                            selectedIndex = i;
                            break;
                        }
                    }
                }

                videoSource = new VideoCaptureDevice(videoDevices[selectedIndex].MonikerString);
                videoSource.NewFrame += VideoSource_NewFrame;
                videoSource.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi mở camera: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();

                if (pictureBoxCamera.InvokeRequired)
                {
                    pictureBoxCamera.Invoke(new Action(() =>
                    {
                        if (pictureBoxCamera.Image != null)
                        {
                            pictureBoxCamera.Image.Dispose();
                        }

                        pictureBoxCamera.Image = (Bitmap)frame.Clone();

                        if (currentFrame != null)
                        {
                            currentFrame.Dispose();
                        }

                        currentFrame = (Bitmap)frame.Clone();
                        frame.Dispose();
                    }));
                }
                else
                {
                    if (pictureBoxCamera.Image != null)
                    {
                        pictureBoxCamera.Image.Dispose();
                    }

                    pictureBoxCamera.Image = (Bitmap)frame.Clone();

                    if (currentFrame != null)
                    {
                        currentFrame.Dispose();
                    }

                    currentFrame = (Bitmap)frame.Clone();
                    frame.Dispose();
                }
            }
            catch
            {
            }
        }

        //private void OpenCamera_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    cameraControl1.Stop();
        //}
        private void OpenCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopCamera();
        }
        private void StopCamera()
        {
            try
            {
                if (videoSource != null)
                {
                    if (videoSource.IsRunning)
                    {
                        videoSource.SignalToStop();
                        videoSource.WaitForStop();
                    }

                    videoSource.NewFrame -= VideoSource_NewFrame;
                    videoSource = null;
                }

                if (pictureBoxCamera.Image != null)
                {
                    pictureBoxCamera.Image.Dispose();
                    pictureBoxCamera.Image = null;
                }

                if (currentFrame != null)
                {
                    currentFrame.Dispose();
                    currentFrame = null;
                }
            }
            catch
            {
            }
        }
    }
}
