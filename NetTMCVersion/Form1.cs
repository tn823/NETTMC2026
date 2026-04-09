using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace NetTMCVersion
{
    public partial class Form1 : Form
    {
        // ─── FTP Config ────────────────────────────────────────────────────────────
        private string ftp_server;
        private string ftp_id;
        private string ftp_pass;
        private string start_app;
        private string filsListPath;

        // ─── File tracking ─────────────────────────────────────────────────────────
        private Dictionary<string, string> currentFileList;   // version hiện tại trên máy
        private Dictionary<string, string> newFileList;       // version mới từ server
        private List<string> targetFileList;                  // danh sách file cần download

        // ─── Progress tracking ─────────────────────────────────────────────────────
        private int total;
        private long contentsFileSize;

        // ─── Retry config ──────────────────────────────────────────────────────────
        private const int MAX_RETRY = 3;    // số lần thử lại tối đa mỗi file
        private const int RETRY_DELAY_MS = 2000; // chờ 2 giây trước khi retry
        private const int FTP_TIMEOUT_MS = 30000;// timeout kết nối FTP (30s)
        private int currentRetryCount = 0;
        private string currentDownloadFile = string.Empty;

        // ─── Misc ──────────────────────────────────────────────────────────────────
        private string strConnection = string.Empty;

        // ───────────────────────────────────────────────────────────────────────────
        public Form1()
        {
            InitializeComponent();
            this.targetFileList = new List<string>();
            // Cho phép TLS 1.2 — cần thiết nếu sau này chuyển sang HTTPS/FTPS
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  LOAD EVENTS
        // ══════════════════════════════════════════════════════════════════════════
        private void Form1_Load(object sender, EventArgs e)
        {
            // Đọc config
            ftp_server = ConfigurationManager.AppSettings["FTP_SERVER"];
            ftp_id = ConfigurationManager.AppSettings["FTP_ID"];
            ftp_pass = ConfigurationManager.AppSettings["FTP_PASS"];
            filsListPath = ConfigurationManager.AppSettings["FILE_LIST"];
            start_app = ConfigurationManager.AppSettings["START"];
            strConnection = ConfigurationManager.AppSettings["ConnectionString"];

            // Validate config không được rỗng
            if (string.IsNullOrWhiteSpace(ftp_server) ||
                string.IsNullOrWhiteSpace(ftp_id) ||
                string.IsNullOrWhiteSpace(ftp_pass) ||
                string.IsNullOrWhiteSpace(filsListPath) ||
                string.IsNullOrWhiteSpace(start_app))
            {
                MessageBox.Show("Cấu hình App.config không đầy đủ. Kiểm tra lại FTP_SERVER / FTP_ID / FTP_PASS / FILE_LIST / START.",
                                "NetTMC – Config Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
                return;
            }

            if (isValidConnection(ftp_server, ftp_id, ftp_pass))
            {
                try
                {
                    loadFileList();           // đọc version hiện tại trên máy
                    BeginDownloadFileList();  // download file list mới từ server
                }
                catch (Exception ex)
                {
                    ShowError("Lỗi kết nối FTP hoặc không tìm thấy file list.\n" + ex.Message
                              + "\n" + ftp_server + "/" + filsListPath);
                    Application.Exit();
                }
            }
            else
            {
                // Không kết nối được → chạy chương trình luôn nếu có
                TryStartAppWithoutUpdate();
            }
        }

        private void Update_Shown(object sender, EventArgs e)
        {
            // Căn giữa màn hình
            Screen screen = Screen.FromControl(this);
            Rectangle area = screen.WorkingArea;
            this.Location = new Point(
                Math.Max(area.X, area.X + (area.Width - this.Width) / 2),
                Math.Max(area.Y, area.Y + (area.Height - this.Height) / 2)
            );
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  FILE LIST – ĐỌC LOCAL
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Đọc file list version đang có trên máy (nếu tồn tại).
        /// </summary>
        private void loadFileList()
        {
            currentFileList = new Dictionary<string, string>();
            string localPath = LocalPath(filsListPath);

            if (!File.Exists(localPath))
                return; // lần đầu cài → currentFileList rỗng → sẽ download tất cả

            try
            {
                XmlDocument doc = ReadXMLFile(localPath);
                foreach (XmlNode node in doc.SelectNodes("files/file"))
                {
                    string name = node.Attributes["name"]?.Value;
                    string version = node.Attributes["version"]?.Value;
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(version))
                        currentFileList[name] = version;
                }
            }
            catch (Exception ex)
            {
                // File list bị hỏng → bỏ qua, coi như lần đầu cài
                LogError("loadFileList", ex);
                currentFileList.Clear();
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  FILE LIST – DOWNLOAD TỪ SERVER
        // ══════════════════════════════════════════════════════════════════════════

        private void BeginDownloadFileList()
        {
            SetStatus("Đang kết nối server...");
            progBar.Value = 0;

            long length = GetFtpFileSize(filsListPath);
            if (length < 0)
            {
                ShowError("Không lấy được kích thước file list trên FTP.");
                Application.Exit();
                return;
            }
            contentsFileSize = length;

            string tempPath = LocalPath("temp_" + filsListPath);
            DeleteSafe(tempPath); // xóa temp cũ nếu còn

            using (WebClient client = CreateWebClient())
            {
                client.DownloadFileCompleted += OnFileListDownloadCompleted;
                client.DownloadProgressChanged += OnDownloadProgressChanged;
                client.DownloadFileAsync(FtpUri(filsListPath), tempPath);
            }
        }

        private void OnFileListDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string tempPath = LocalPath("temp_" + filsListPath);

            if (e.Error != null)
            {
                LogError("FileList download", e.Error);
                DeleteSafe(tempPath);
                RetryDownloadFileList();
                return;
            }
            if (e.Cancelled)
            {
                DeleteSafe(tempPath);
                TryStartAppWithoutUpdate();
                return;
            }

            // Kiểm tra file temp có hợp lệ không
            if (!IsValidXmlFile(tempPath))
            {
                DeleteSafe(tempPath);
                RetryDownloadFileList();
                return;
            }

            // Parse file list mới
            try
            {
                progBar.Value = 0;
                newFileList = new Dictionary<string, string>();

                XmlDocument doc = ReadXMLFile(tempPath);
                foreach (XmlNode node in doc.SelectNodes("files/file"))
                {
                    string name = node.Attributes["name"]?.Value;
                    string version = node.Attributes["version"]?.Value;
                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(version))
                        newFileList[name] = version;
                }

                compareDictionary();  // so sánh version
                fileExistCheck();     // kiểm tra file có tồn tại không
                RemoveIncompleteFiles(); // xóa file download dở từ lần trước

                total = targetFileList.Count;
                SetStatus("Tổng số file cần cập nhật: " + total);
                StartDownload();
            }
            catch (Exception ex)
            {
                LogError("OnFileListDownloadCompleted parse", ex);
                DeleteSafe(tempPath);
                RetryDownloadFileList();
            }
        }

        private int fileListRetryCount = 0;
        private void RetryDownloadFileList()
        {
            fileListRetryCount++;
            if (fileListRetryCount > MAX_RETRY)
            {
                ShowError("Không thể tải file list sau " + MAX_RETRY + " lần thử. Kiểm tra kết nối mạng.");
                TryStartAppWithoutUpdate();
                return;
            }
            SetStatus($"Thử lại lần {fileListRetryCount}/{MAX_RETRY}...");
            System.Threading.Thread.Sleep(RETRY_DELAY_MS);
            BeginDownloadFileList();
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  SO SÁNH VERSION
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// So sánh version: dùng System.Version để so sánh chuẩn (1.0.0.1 vs 1.0.0.2).
        /// Fallback về so sánh string nếu không parse được.
        /// Tốt hơn so sánh string thuần vì "1.0.10" > "1.0.9" (string so sánh sẽ sai).
        /// </summary>
        private void compareDictionary()
        {
            foreach (var newEntry in newFileList)
            {
                if (currentFileList.TryGetValue(newEntry.Key, out string currentVer))
                {
                    // Cả hai đều tồn tại → so sánh version
                    if (!IsSameOrNewer(currentVer, newEntry.Value))
                        AddToTarget(newEntry.Key);
                }
                else
                {
                    // File mới hoàn toàn → thêm vào danh sách download
                    AddToTarget(newEntry.Key);
                }
            }
        }

        /// <summary>
        /// Trả về true nếu localVer >= serverVer (không cần update).
        /// </summary>
        private bool IsSameOrNewer(string localVer, string serverVer)
        {
            if (System.Version.TryParse(localVer, out System.Version local) &&
                System.Version.TryParse(serverVer, out System.Version server))
            {
                return local >= server;
            }
            // Fallback: so sánh string (giữ tương thích với format cũ)
            return string.Equals(localVer, serverVer, StringComparison.OrdinalIgnoreCase);
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  KIỂM TRA FILE TỒN TẠI & TÍNH TOÀN VẸN
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Thêm vào target list những file có trong newFileList nhưng chưa có trên máy.
        /// </summary>
        private void fileExistCheck()
        {
            foreach (var file in newFileList)
            {
                string fullPath = LocalPath(file.Key);
                if (!File.Exists(fullPath))
                    AddToTarget(file.Key);
            }
        }

        /// <summary>
        /// Xóa những file bị download dở (kích thước 0 hoặc không hợp lệ).
        /// Khi mở lại chương trình, các file này sẽ được download lại.
        /// </summary>
        private void RemoveIncompleteFiles()
        {
            foreach (string fileName in targetFileList.ToList()) // ToList để tránh modify khi iterate
            {
                string fullPath = LocalPath(fileName);
                if (File.Exists(fullPath))
                {
                    FileInfo fi = new FileInfo(fullPath);
                    if (fi.Length == 0)
                    {
                        DeleteSafe(fullPath);
                        LogInfo($"Đã xóa file rỗng/lỗi: {fileName}");
                    }
                }
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  DOWNLOAD FILE CHÍNH
        // ══════════════════════════════════════════════════════════════════════════

        private void StartDownload()
        {
            if (targetFileList.Count > 0)
            {
                currentRetryCount = 0;
                currentDownloadFile = targetFileList[0];
                DownloadFile(currentDownloadFile);
            }
            else
            {
                CompleteUpdate();
            }
        }

        private void DownloadFile(string fileName)
        {
            try
            {
                progBar.Value = 0;
                SetStatus($"Đang tải: {fileName} [{total - targetFileList.Count + 1}/{total}]");

                // Kiểm tra file có tồn tại trên FTP không
                long length = GetFtpFileSize(fileName);
                if (length < 0)
                {
                    // File không tồn tại trên server → bỏ qua
                    LogInfo($"File không tồn tại trên server, bỏ qua: {fileName}");
                    targetFileList.Remove(fileName);
                    StartDownload();
                    return;
                }
                contentsFileSize = length;

                string destPath = LocalPath(fileName);
                EnsureDirectory(destPath);

                // Xóa file cũ (hoặc dở dang) trước khi download
                DeleteSafe(destPath);

                using (WebClient client = CreateWebClient())
                {
                    client.DownloadFileCompleted += OnFileDownloadCompleted;
                    client.DownloadProgressChanged += OnDownloadProgressChanged;
                    client.DownloadFileAsync(FtpUri(fileName), destPath);
                }
            }
            catch (Exception ex)
            {
                LogError($"DownloadFile [{fileName}]", ex);
                RetryCurrentFile();
            }
        }

        private void OnFileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string fileName = currentDownloadFile;
            string destPath = LocalPath(fileName);

            // ── Trường hợp lỗi mạng ────────────────────────────────────────────
            if (e.Error != null)
            {
                LogError($"Download lỗi [{fileName}]", e.Error);
                DeleteSafe(destPath); // xóa file dở dang
                RetryCurrentFile();
                return;
            }

            // ── Bị hủy ─────────────────────────────────────────────────────────
            if (e.Cancelled)
            {
                DeleteSafe(destPath);
                TryStartAppWithoutUpdate();
                return;
            }

            // ── Kiểm tra file sau khi download ─────────────────────────────────
            if (!File.Exists(destPath) || new FileInfo(destPath).Length == 0)
            {
                LogInfo($"File download không đầy đủ, thử lại: {fileName}");
                DeleteSafe(destPath);
                RetryCurrentFile();
                return;
            }

            // ── Thành công → tiếp tục file tiếp theo ───────────────────────────
            currentRetryCount = 0;
            targetFileList.Remove(fileName);
            SetStatus($"Đã tải {total - targetFileList.Count}/{total} file");
            StartDownload();
        }

        /// <summary>
        /// Retry file hiện tại, tối đa MAX_RETRY lần.
        /// </summary>
        private void RetryCurrentFile()
        {
            currentRetryCount++;
            if (currentRetryCount > MAX_RETRY)
            {
                ShowError($"Không thể tải file '{currentDownloadFile}' sau {MAX_RETRY} lần thử.\n"
                         + "Kiểm tra kết nối mạng hoặc liên hệ quản trị viên.");
                TryStartAppWithoutUpdate();
                return;
            }

            SetStatus($"Thử lại lần {currentRetryCount}/{MAX_RETRY}: {currentDownloadFile}");
            System.Threading.Thread.Sleep(RETRY_DELAY_MS);
            DownloadFile(currentDownloadFile);
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  PROGRESS
        // ══════════════════════════════════════════════════════════════════════════

        private void OnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double totalBytes = contentsFileSize > 0 ? contentsFileSize : 1;
            double percentage = e.BytesReceived / totalBytes * 100.0;
            label1.Text = $"Đã tải {FormatBytes(e.BytesReceived)} / {FormatBytes(contentsFileSize)}";
            progBar.Value = Math.Min(100, (int)Math.Truncate(percentage));
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  HOÀN TẤT UPDATE
        // ══════════════════════════════════════════════════════════════════════════

        private void CompleteUpdate()
        {
            try
            {
                string localList = LocalPath(filsListPath);
                string tempList = LocalPath("temp_" + filsListPath);

                // Thay thế file list cũ bằng file list mới
                DeleteSafe(localList);
                if (File.Exists(tempList))
                {
                    File.Move(tempList, localList);
                }

                SetStatus("Cập nhật hoàn tất. Đang khởi động chương trình...");
                System.Diagnostics.Process.Start(start_app);
                Application.Exit();
            }
            catch (Exception ex)
            {
                ShowError("Không thể hoàn tất cập nhật hoặc khởi động chương trình.\n" + ex.Message);
                Application.Exit();
            }
        }

        /// <summary>
        /// Chạy chương trình mà không update (khi mất mạng hoặc FTP lỗi).
        /// </summary>
        private void TryStartAppWithoutUpdate()
        {
            try
            {
                DeleteSafe(LocalPath("temp_" + filsListPath));
                if (File.Exists(start_app) ||
                    File.Exists(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), start_app)))
                {
                    System.Diagnostics.Process.Start(start_app);
                }
                else
                {
                    ShowError("Không tìm thấy chương trình chính: " + start_app);
                }
            }
            catch (Exception ex)
            {
                ShowError("Không thể khởi động chương trình: " + ex.Message);
            }
            finally
            {
                Application.Exit();
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  FTP HELPERS
        // ══════════════════════════════════════════════════════════════════════════

        /// <summary>
        /// Kiểm tra kết nối FTP có hợp lệ không.
        /// </summary>
        private bool isValidConnection(string url, string user, string password)
        {
            try
            {
                FtpWebRequest req = CreateFtpRequest(url, WebRequestMethods.Ftp.ListDirectory, user, password);
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                    return true;
            }
            catch (Exception ex)
            {
                LogError("isValidConnection", ex);
                MessageBox.Show("Không thể kết nối FTP. Kiểm tra lại FTP hoặc cấu hình NetTMCVersion.\n" + ex.Message,
                                "NetTMC – Kết nối FTP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }

        /// <summary>
        /// Lấy kích thước file trên FTP. Trả về -1 nếu file không tồn tại.
        /// </summary>
        private long GetFtpFileSize(string fileName)
        {
            try
            {
                FtpWebRequest req = CreateFtpRequest(ftp_server + "/" + fileName,
                                                     WebRequestMethods.Ftp.GetFileSize,
                                                     ftp_id, ftp_pass);
                using (FtpWebResponse res = (FtpWebResponse)req.GetResponse())
                    return res.ContentLength;
            }
            catch (WebException ex) when
                  (ex.Response is FtpWebResponse ftpRes &&
                   ftpRes.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
            {
                return -1; // file không tồn tại
            }
            catch (Exception ex)
            {
                LogError($"GetFtpFileSize [{fileName}]", ex);
                return -1;
            }
        }

        private FtpWebRequest CreateFtpRequest(string url, string method, string user, string pass)
        {
            FtpWebRequest req = (FtpWebRequest)WebRequest.Create(url);
            req.Credentials = new NetworkCredential(user, pass);
            req.Method = method;
            req.Timeout = FTP_TIMEOUT_MS;
            req.ReadWriteTimeout = FTP_TIMEOUT_MS;
            req.KeepAlive = false; // tránh connection bị treo
            req.UsePassive = true;  // passive mode ổn định hơn trong NAT/firewall
            return req;
        }

        private WebClient CreateWebClient()
        {
            var client = new WebClient();
            client.Credentials = new NetworkCredential(ftp_id, ftp_pass);
            return client;
        }

        private Uri FtpUri(string fileName) => new Uri(ftp_server + "/" + fileName);

        // ══════════════════════════════════════════════════════════════════════════
        //  XML HELPERS
        // ══════════════════════════════════════════════════════════════════════════

        private static XmlDocument ReadXMLFile(string filePath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            return doc;
        }

        /// <summary>
        /// Kiểm tra file XML có parse được không (tránh dùng file lỗi).
        /// </summary>
        private static bool IsValidXmlFile(string filePath)
        {
            if (!File.Exists(filePath)) return false;
            if (new FileInfo(filePath).Length == 0) return false;
            try { ReadXMLFile(filePath); return true; }
            catch { return false; }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  ENCRYPTION (giữ nguyên, chỉ thêm ghi chú bảo mật)
        // ══════════════════════════════════════════════════════════════════════════
        // ⚠ LƯU Ý: DES là thuật toán yếu (key 56-bit). Nếu cần bảo mật cao hơn,
        //   hãy chuyển sang AES-256. Hiện tại giữ nguyên để tương thích.

        private static string Encrypt(string strEncrypt)
        {
            byte[] IV = { 12, 22, 32, 42, 52, 62, 72, 82 };
            byte[] key = Encoding.UTF8.GetBytes("Encoding");
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(key, IV), CryptoStreamMode.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(strEncrypt);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }

        private static string Decrypt(string strDecrypt)
        {
            byte[] IV = { 12, 22, 32, 42, 52, 62, 72, 82 };
            byte[] key = Encoding.UTF8.GetBytes("Encoding");
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(key, IV), CryptoStreamMode.Write))
            {
                byte[] data = Convert.FromBase64String(strDecrypt);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        // ══════════════════════════════════════════════════════════════════════════
        //  UTILITY
        // ══════════════════════════════════════════════════════════════════════════

        private string LocalPath(string fileName)
            => Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), fileName.Replace('/', '\\'));

        private void EnsureDirectory(string fullFilePath)
        {
            string dir = Path.GetDirectoryName(fullFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }

        private void DeleteSafe(string path)
        {
            try { if (File.Exists(path)) File.Delete(path); }
            catch (Exception ex) { LogError($"DeleteSafe [{path}]", ex); }
        }

        private void AddToTarget(string fileName)
        {
            if (!targetFileList.Contains(fileName))
                targetFileList.Add(fileName);
        }

        private static string FormatBytes(long bytes)
        {
            if (bytes >= 1_048_576) return $"{bytes / 1_048_576.0:F1} MB";
            if (bytes >= 1_024) return $"{bytes / 1_024.0:F1} KB";
            return $"{bytes} B";
        }

        private void SetStatus(string msg)
        {
            if (lblStatus.InvokeRequired)
                lblStatus.Invoke(new Action(() => lblStatus.Text = msg));
            else
                lblStatus.Text = msg;
        }

        private void ShowError(string msg)
            => MessageBox.Show(msg, "NetTMC – Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);

        /// <summary>
        /// Ghi log lỗi ra file (debug/production). Không crash nếu ghi thất bại.
        /// </summary>
        private void LogError(string context, Exception ex)
        {
            try
            {
                string logPath = Path.Combine(
                    Path.GetDirectoryName(Application.ExecutablePath), "update_error.log");
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] ERROR [{context}] {ex.Message}\n{ex.StackTrace}\n\n";
                File.AppendAllText(logPath, line, Encoding.UTF8);
            }
            catch { /* không được throw trong log */ }
        }

        private void LogInfo(string msg)
        {
            try
            {
                string logPath = Path.Combine(
                    Path.GetDirectoryName(Application.ExecutablePath), "update_error.log");
                string line = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] INFO  {msg}\n";
                File.AppendAllText(logPath, line, Encoding.UTF8);
            }
            catch { }
        }
    }
}