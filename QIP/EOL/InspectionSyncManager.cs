using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;


namespace QIP.EOL
{
    public class InspectionSyncManager : IDisposable
    {
        // ── Cấu hình ────────────────────────────────────────────────
        private const string FOLDER_PENDING = "InspectionPending";
        private const string FOLDER_DONE = "InspectionDone";
        private const string FOLDER_ERROR = "InspectionError";
        private const string FILE_NAME = "queue";
        private const int SYNC_INTERVAL_MS = 30_000;   // 30 giây
        private const int MAX_RETRY = 5;         // thử lại tối đa 5 lần

        // ── Separator dòng trong file ────────────────────────────────
        // Format 1 dòng: GUID|DEPT_CD|LINE_CD|C_STYLE|ORDER_NO|PO_NUM|RESULT|D_GATHER|IP|DEFECT_LIST|RETRY_COUNT
        private const char COL_SEP = '|';
        private const char LIST_SEP = ';';  // dùng ; phân cách items trong DEFECT_LIST (vì | đã dùng)

        private readonly System.Threading.Timer _timer;
        private readonly object _fileLock = new object();   // lock khi đọc/ghi file
        private readonly object _syncLock = new object();   // lock khi đang sync, tránh 2 timer chồng nhau
        private bool _isSyncing = false;
        private bool _disposed = false;

        // Inject từ ngoài vào
        private readonly Action<string, Dictionary<string, object>> _executeProc;
        private readonly Func<string, string, string, bool> _writeToFile;
        private readonly Func<string, string, string[]> _readFromFile;

        public InspectionSyncManager(
            Action<string, Dictionary<string, object>> executeProc,
            Func<string, string, string, bool> writeToFile,
            Func<string, string, string[]> readFromFile)
        {
            _executeProc = executeProc;
            _writeToFile = writeToFile;
            _readFromFile = readFromFile;

            // Khởi động timer sync background
            _timer = new System.Threading.Timer(_ => SyncToDatabase(), null,
                         TimeSpan.FromSeconds(10),           // lần đầu sau 10s
                         TimeSpan.FromMilliseconds(SYNC_INTERVAL_MS));
        }

        // ────────────────────────────────────────────────────────────
        // PUBLIC: Gọi khi QC bấm Pass hoặc Fail
        // defectList format: "REASON_ID|PART_ID|QTY,REASON_ID|PART_ID|QTY"
        //   → trước khi lưu file, dấu ',' giữa các item được đổi thành ';'
        //   → khi gửi DB sẽ đổi lại thành ','
        // ────────────────────────────────────────────────────────────
        public bool Submit(
            string deptCd, string lineCd, string cStyle,
            string orderNo, string poNum, string result,
            string dGather, string ipAddress,
            string defectList = null)
        {
            try
            {
                // Tạo GUID duy nhất cho record này
                string guid = Guid.NewGuid().ToString();

                // Chuẩn hoá defectList: đổi ',' thành ';' để không xung đột column separator
                string defectSafe = string.IsNullOrWhiteSpace(defectList)
                    ? ""
                    : defectList.Replace(",", LIST_SEP.ToString());

                // Ghi 1 dòng vào file pending
                // Format: GUID|DEPT|LINE|STYLE|ORDER|PO|RESULT|DGATHER|IP|DEFECTS|RETRY
                string line = string.Join(COL_SEP.ToString(), new[]
                {
                guid, deptCd, lineCd, cStyle, orderNo,
                poNum, result, dGather, ipAddress,
                defectSafe, "0"   // retry count = 0
            });

                lock (_fileLock)
                {
                    bool ok = _writeToFile(line, FOLDER_PENDING, FILE_NAME);
                    if (!ok) throw new IOException("Ghi file pending that bai");
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu local: " + ex.Message);
                return false;
            }
        }

        // ────────────────────────────────────────────────────────────
        // INTERNAL: Chạy background, đọc pending → gửi DB
        // ────────────────────────────────────────────────────────────
        private void SyncToDatabase()
        {
            // Tránh 2 timer chạy đồng thời
            lock (_syncLock)
            {
                if (_isSyncing) return;
                _isSyncing = true;
            }

            try
            {
                string[] lines;
                lock (_fileLock)
                {
                    lines = _readFromFile(FOLDER_PENDING, FILE_NAME);
                }

                if (lines == null || lines.Length == 0) return;

                // Danh sách dòng sẽ ghi lại vào pending (những dòng chưa sync xong)
                var remainPending = new List<string>();
                var doneLines = new List<string>();
                var errorLines = new List<string>();

                foreach (string rawLine in lines)
                {
                    if (string.IsNullOrWhiteSpace(rawLine)) continue;

                    string processedLine = TrySyncOneLine(rawLine, out bool success, out bool maxRetryReached);

                    if (success)
                    {
                        // Ghi vào done để đối chiếu số lượng
                        doneLines.Add(rawLine);
                    }
                    else if (maxRetryReached)
                    {
                        // Quá số lần retry → chuyển sang error để xem xét thủ công
                        errorLines.Add(rawLine);
                    }
                    else
                    {
                        // Còn retry được → giữ lại pending với retry count tăng lên
                        remainPending.Add(processedLine);
                    }
                }

                // ── Ghi lại file: xoá cũ, ghi mới ──────────────────
                lock (_fileLock)
                {
                    foreach (string d in doneLines)
                    {
                        // Done lưu theo ngày để dễ đối soát
                        string doneFile = "done_" + DateTime.Now.ToString("yyyyMMdd");
                        _writeToFile(d, FOLDER_DONE, doneFile);
                    }

                    foreach (string e in errorLines)
                    {
                        string errFile = "error_" + DateTime.Now.ToString("yyyyMMdd");
                        _writeToFile(e, FOLDER_ERROR, errFile);
                    }
                }
            }
            catch { /* silent: sẽ retry ở lần timer sau */ }
            finally
            {
                lock (_syncLock) { _isSyncing = false; }
            }
        }

        // ────────────────────────────────────────────────────────────
        // Xử lý 1 dòng: parse → gọi DB → trả về kết quả
        // ────────────────────────────────────────────────────────────
        private string TrySyncOneLine(string rawLine, out bool success, out bool maxRetryReached)
        {
            success = false;
            maxRetryReached = false;

            string[] cols = rawLine.Split(COL_SEP);
            // Cần đủ 11 cột
            if (cols.Length < 11)
            {
                maxRetryReached = true;   // dòng lỗi format → đẩy sang error
                return rawLine;
            }

            string guid = cols[0].Trim();
            string deptCd = cols[1].Trim();
            string lineCd = cols[2].Trim();
            string cStyle = cols[3].Trim();
            string orderNo = cols[4].Trim();
            string poNum = cols[5].Trim();
            string result = cols[6].Trim();
            string dGather = cols[7].Trim();
            string ipAddress = cols[8].Trim();
            string defectSafe = cols[9].Trim();
            int retryCount = int.TryParse(cols[10].Trim(), out int r) ? r : 0;

            if (retryCount >= MAX_RETRY)
            {
                maxRetryReached = true;
                return rawLine;
            }

            // Đổi ';' về ',' để gửi cho procedure
            string defectList = defectSafe.Replace(LIST_SEP.ToString(), "|");
            defectList = defectList.Replace("&", ",");
            try
            {
                var prms = new Dictionary<string, object>
            {
                { "P_DEPT_CD",     deptCd     },
                { "P_LINE_CD",     lineCd     },
                { "P_C_STYLE",     cStyle     },
                { "P_ORDER_NO",    orderNo    },
                { "P_PO_NUM",      poNum      },
                { "P_RESULT",      result     },
                { "P_D_GATHER",    dGather    },
                { "P_IP_ADDRESS",  ipAddress  },
                { "P_LOCAL_GUID",  guid       },
                { "P_DEFECT_LIST", string.IsNullOrEmpty(defectList) ? null  : defectList }
            };

                _executeProc("MES.SP_INSPECTION_SUBMIT", prms);

                // Kiểm tra output: P_ERR_CODE = 0 hoặc ALREADY_SYNCED đều coi là thành công
                int errCode = prms.ContainsKey("P_ERR_CODE") && prms["P_ERR_CODE"] != DBNull.Value
                    ? Convert.ToInt32(prms["P_ERR_CODE"]) : -99;

                string errMsg = prms.ContainsKey("P_ERR_MSG") && prms["P_ERR_MSG"] != DBNull.Value
                    ? prms["P_ERR_MSG"].ToString() : "";

                if (errCode == 0)
                {
                    success = true;
                    return rawLine;
                }

                // Nếu lỗi logic (không phải lỗi mạng) → không retry nữa
                if (errCode < 0 && errCode >= -4)
                {
                    maxRetryReached = true;
                    return rawLine;
                }
            }
            catch { /* lỗi kết nối → tăng retry */ }

            // Tăng retry count và trả dòng đã cập nhật
            cols[10] = (retryCount + 1).ToString();
            return string.Join(COL_SEP.ToString(), cols);
        }

        // ────────────────────────────────────────────────────────────
        // Xoá file cũ và ghi lại danh sách mới (dùng khi rewrite pending)
        // ────────────────────────────────────────────────────────────
        private void OverwriteFile(string folder, string filename, List<string> lines)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory
                            + "\\Config\\" + folder + "\\" + filename + ".txt";

                if (lines.Count == 0)
                {
                    if (File.Exists(path)) File.Delete(path);
                    return;
                }

                // Ghi vào file tạm trước, sau đó rename → tránh mất dữ liệu nếu crash giữa chừng
                string tmpPath = path + ".tmp";
                File.WriteAllLines(tmpPath, lines);
                if (File.Exists(path)) File.Delete(path);
                File.Move(tmpPath, path);
            }
            catch { /* giữ nguyên file cũ nếu lỗi */ }
        }

        // ────────────────────────────────────────────────────────────
        // Đối soát: so sánh số dòng pending+done với DB
        // Gọi khi cần kiểm tra cuối ca / cuối ngày
        // ────────────────────────────────────────────────────────────
        public (int localTotal, int pendingCount, int doneCount) GetLocalStats(string date = null)
        {
            string d = date ?? DateTime.Now.ToString("yyyyMMdd");
            lock (_fileLock)
            {
                var pending = _readFromFile(FOLDER_PENDING, FILE_NAME) ?? new string[0];
                var done = _readFromFile(FOLDER_DONE, "done_" + d) ?? new string[0];

                int p = 0; foreach (var l in pending) if (!string.IsNullOrWhiteSpace(l)) p++;
                int ok = 0; foreach (var l in done) if (!string.IsNullOrWhiteSpace(l)) ok++;

                return (p + ok, p, ok);
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _timer?.Dispose();
            }
        }
    }
}