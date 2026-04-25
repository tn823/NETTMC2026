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

namespace QIP.EOL.Popup
{
    public partial class Popuppdf : Form
    {
        public string filepath { get; set; }
        private string tempPdfPath = "";
        public Popuppdf()
        {
            InitializeComponent();
        }

        private async void Popuppdf_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;

            try
            {
                if (string.IsNullOrEmpty(filepath))
                {
                    MessageBox.Show("Không có đường dẫn PDF");
                    return;
                }

                string ftpPath = filepath;
                string localPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".pdf");
                tempPdfPath = localPath;

                using (WebClient client = new WebClient())
                {
                    client.Credentials = new NetworkCredential("mes", "!saigon3535!");
                    client.DownloadFile(ftpPath, localPath);
                }

                if (File.Exists(localPath))
                {
                    await webView21.EnsureCoreWebView2Async(null);
                    webView21.Source = new Uri(localPath);
                }
                else
                {
                    MessageBox.Show("Download xong nhưng không thấy file!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể tải file PDF.\n" + ex.Message);
                this.Close();
            }
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            try
            {
                if (webView21 != null)
                {
                    webView21.Dispose();
                }

                if (!string.IsNullOrEmpty(tempPdfPath) && File.Exists(tempPdfPath))
                {
                    File.Delete(tempPdfPath);
                }
            }
            catch { }

            base.OnFormClosed(e);
        }
    }
}
