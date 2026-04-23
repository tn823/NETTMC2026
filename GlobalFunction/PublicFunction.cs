using ConnectionClass.Oracle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace GlobalFunction
{
    public class PublicFunction
    {
        public string FileServerPath = "" + ConnectionClass.Oracle.Site.FTPPath + "";
 
        public class DeptLineWorkEAM410
        {
            public string Deptcd;
            public string Linecd;
            public DataTable Dept
            {
                get
                {
                    return GetDept();
                }
            }
            public DataTable Line
            {
                get
                {
                    return GetLine();
                }
            }
            public DataTable Work
            {
                get
                {
                    return GetWork();
                }
            }
            public DataTable LineFromDept
            {
                get
                {
                    return GetLine(Deptcd);
                }
            }
            public DataTable WorFromDept
            {
                get
                {
                    return GetWork(Linecd);
                }
            }
            public DataTable GetDept()
            {
                DataTable dt = new DataTable();
                try
                {
                    CRUDOracle crud = new CRUDOracle("HRMS");
                    dt = crud.dac.DtSelectExcuteWithQuery("Select '%' DEPTCD,'%' DEPTNM from dual union all select DEPTCD,DEPTNM from EAM410 WHERE USEYN = 'Y' GROUP BY DEPTCD,DEPTNM ORDER BY 1");
                    return dt;
                }
                catch
                {
                    return dt;
                }
            }
            public DataTable GetLine()
            {
                DataTable dt = new DataTable();
                try
                {
                    CRUDOracle crud = new CRUDOracle("HRMS");
                    dt = crud.dac.DtSelectExcuteWithQuery("Select '%' LINECD,'%' TEAMNM from dual union all select LINECD,TEAMNM from EAM410 WHERE USEYN = 'Y' GROUP BY LINECD,TEAMNM  ORDER BY 1");
                    return dt;
                }
                catch
                {
                    return dt;
                }
            }
            public DataTable GetWork()
            {
                DataTable dt = new DataTable();
                try
                {
                    CRUDOracle crud = new CRUDOracle("HRMS");
                    dt = crud.dac.DtSelectExcuteWithQuery("Select '%' WORKCD,'%' WORKNM from dual union all select WORKCD,WORKNM from EAM410 WHERE USEYN = 'Y' GROUP BY WORKCD,WORKNM ORDER BY 1");
                    return dt;
                }
                catch
                {
                    return dt;
                }
            }
            public DataTable GetLine(string deptcd)
            {
                DataTable dt = new DataTable();
                try
                {
                    CRUDOracle crud = new CRUDOracle("HRMS");
                    dt = crud.dac.DtSelectExcuteWithQuery("Select '%' LINECD,'%' TEAMNM from dual union all select LINECD,TEAMNM from EAM410 WHERE USEYN = 'Y' AND DEPTCD LIKE '%" + deptcd + "' GROUP BY LINECD,TEAMNM  ORDER BY 1");
                    return dt;
                }
                catch
                {
                    return dt;
                }
            }
            public DataTable GetWork(string linecd)
            {
                DataTable dt = new DataTable();
                try
                {
                    CRUDOracle crud = new CRUDOracle("HRMS");
                    dt = crud.dac.DtSelectExcuteWithQuery("Select '%' WORKCD,'%' WORKNM from dual union all select WORKCD,WORKNM from EAM410 WHERE USEYN = 'Y' AND LINECD LIKE '%" + linecd + "' GROUP BY WORKCD,WORKNM ORDER BY 1");
                    return dt;
                }
                catch
                {
                    return dt;
                }
            }
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetSystemTime(ref SYSTEMTIME st);
        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }
        public bool WriteToFile(string content, string filename)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {
                    // Create a file to write to.   
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(content);
                        return true;
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(content);
                        return true;
                    }
                }
            }
            catch { return false; }
        }
        public bool ReplaceTempFile(string foldername)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepathtemp = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "BTS_" + DateTime.Now.ToString("yyyyMMdd") + "_temp" + ".txt";
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "BTS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(filepathtemp))
                {
                    return false;
                }
                else
                {
                    File.Copy(filepathtemp, filepath, true);
                    return true;
                }
            }
            catch { return false; }
        }
        public bool DeleteTempFile(string foldername)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepathtemp = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "BTS_" + DateTime.Now.ToString("yyyyMMdd") + "_temp" + ".txt";
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "BTS_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (File.Exists(filepathtemp))
                {
                    File.Delete(filepathtemp);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch { return false; }
        }
        public bool ReplaceTempFiles(string foldername)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepathtemp = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "_" + DateTime.Now.ToString("yyyyMMdd") + "_temp" + ".txt";
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (!File.Exists(filepathtemp))
                {
                    return false;
                }
                else
                {
                    File.Copy(filepathtemp, filepath, true);
                    return true;
                }
            }
            catch { return false; }
        }
        public bool DeleteTempFiles(string foldername)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepathtemp = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "_" + DateTime.Now.ToString("yyyyMMdd") + "_temp" + ".txt";
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + foldername + "_" + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                if (File.Exists(filepathtemp))
                {
                    File.Delete(filepathtemp);
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch { return false; }
        }
        public bool DeleteConfigFile(string filename)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + filename + ".txt";
                if (!File.Exists(path))
                {
                    return true;
                }
                else
                {
                    File.Delete(path);
                    return true;
                }

            }
            catch { return false; }
        }
        public bool WriteToFile(string content, string foldername, string filename)
        {
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername;
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {
                    // Create a file to write to.   
                    using (StreamWriter sw = File.CreateText(filepath))
                    {
                        sw.WriteLine(content);
                        return true;
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(filepath))
                    {
                        sw.WriteLine(content);
                        return true;
                    }
                }
            }
            catch { return false; }
        }
        public string[] ReadFromFile(string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {
                    MessageBox.Show("Không tìm thấy file " + filename + " trong đường dẫn " + filepath);
                    return null;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines;
                }
            }
            catch
            {
                return null;
            }
        }
        public string[] ReadFromFile(string foldername, string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {
                    MessageBox.Show("Không tìm thấy file " + filename + " trong đường dẫn " + filepath);
                    return null;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines;
                }
            }
            catch
            {
                return null;
            }
        }
        public string[] ReadFromFileNotMesage(string foldername, string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {

                    return null;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines;
                }
            }
            catch
            {
                return null;
            }
        }
        public string[] ReadLogNotMesage(string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {
                    return null;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines;
                }
            }
            catch
            {
                return null;
            }
        }
        public string[] ReadFromFileNotMesage(string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + filename + ".txt";
                Debug.WriteLine("Đường dẫn đang tìm: " + filepath);
                Debug.WriteLine("File tồn tại không? " + File.Exists(filepath));
                if (!File.Exists(filepath))
                {

                    return null;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines;
                }
            }
            catch
            {
                return null;
            }
        }
        public int CountPassFail(string foldername, string filename)
        {
            try
            {
                string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Config\\" + foldername + "\\" + filename + ".txt";
                if (!File.Exists(filepath))
                {

                    return 0;
                }
                else
                {
                    string[] lines = System.IO.File.ReadAllLines(filepath);
                    return lines.Count();
                }
            }
            catch
            {
                return 0;
            }
        }
        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString() == "192.168.1.55" || ip.ToString() == "192.168.1.196" || ip.ToString() == "192.168.0.118" || ip.ToString() == "192.168.1.150")
                    {
                        //return ip.ToString();
                        //return "192.168.0.85";
                        return "192.168.1.197";
                        //return "192.168.17.241";
                        //return "192.168.0.236";
                        //return "192.168.3.174";
                    }
                    else
                    {
                        return ip.ToString();
                    }
                }
            }
            MessageBox.Show("Network Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }

        public static string myIpaddress
        {
            set { }
            get
            {
                return GetLocalIPAddress();
            }
        }
        public static string userID
        {
            get;
            set;
        }
        private static string GetUserID()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (ip.ToString() == "192.168.1.193")
                    {
                        return "192.168.1.193";
                    }
                    else
                    {
                        return ip.ToString();
                    }
                }
            }
            MessageBox.Show("Network Error", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return "";
        }
        public static string MainErrorMessage
        {
            get;
            set;
        }
        public DateTime CurrentSystemTime()
        {
            try
            {
                DateTime curtime = Convert.ToDateTime(("Select to_char(sysdate,'DD/MM/YYYY HH24:MI:SS') from dual"));
                return DateTime.Now;
            }
            catch (Exception ex)
            {
                return DateTime.Now;
            }
        }
        public void SaveImageToCacheFolder(System.Drawing.Image savedimage, string stt, string filename)
        {
            int width = 1280;
            var height = 720; //succeeds at 65499, 65500
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\CacheImage\\" + filename;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = path + "\\" + stt + ".jpg";
            if (!File.Exists(filepath))
            {
                savedimage.Save(filepath, ImageFormat.Jpeg);
            }
            else
            {
                File.Delete(filepath);
                savedimage.Save(filepath, ImageFormat.Jpeg);
            }
        }
        public string LoadImageFromCacheFolder(string filename, string stt)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "CacheImage\\" + filename + "\\" + stt + ".jpg";
            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                return path;
            }

        }
        public string LoadImageFromServerFolder(string ServerPath, string filename, string stt)
        {
            string path = ServerPath + filename + "\\" + stt + ".jpg";

            try
            {
                using (GlobalFunction.NetUse.UNCAccessWithCredentials unc = new GlobalFunction.NetUse.UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(@"\\" + ConnectionClass.Oracle.Site.FTPPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                    {

                        if (!File.Exists(path))
                        {
                            return null;
                        }
                        else
                        {
                            return path;
                        }

                    }
                    else
                    {
                        try
                        {
                            if (!File.Exists(path))
                            {
                                return null;
                            }
                            else
                            {
                                return path;
                            }
                        }
                        catch
                        {
                            return null;
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public void CopyImageFromCacheFolderToServer(string ServerPath, string SourcePath, bool update, out bool Result, out string ErrorMessage)
        {
            try
            {
                CRUDOracle crud = new CRUDOracle("HRMS");
                using (GlobalFunction.NetUse.UNCAccessWithCredentials unc = new GlobalFunction.NetUse.UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                    {
                        if (!Directory.Exists(ServerPath))
                        {
                            Directory.CreateDirectory(ServerPath);
                        }
                        if (update)
                        {
                            Result = false;
                            ErrorMessage = "";
                        }
                        else
                        {
                            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(SourcePath);
                            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(ServerPath);

                            // Take a snapshot of the file system.  
                            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);
                            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);

                            //A custom file comparer defined below  
                            FileCompare myFileCompare = new FileCompare();

                            // This query determines whether the two folders contain  
                            // identical file lists, based on the custom file comparer  
                            // that is defined in the FileCompare class.  
                            // The query executes immediately because it returns a bool.  
                            bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

                            if (areIdentical == true)
                            {
                                Result = false;
                                ErrorMessage = "Two folders are same";
                            }
                            else
                            {
                                // Find the common files. It produces a sequence and doesn't
                                // execute until the foreach statement.  
                                var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                                var queryList1Only = (from file in list1
                                                      select file).Except(list2, myFileCompare);

                                foreach (var v in queryList1Only)
                                {
                                    //Console.WriteLine(v.FullName);
                                    if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                                    {
                                        FileInfo fi = new FileInfo(v.FullName);
                                        fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                        Result = true;
                                        ErrorMessage = "Copy Done";
                                    }

                                }
                            }
                            Result = true;
                            ErrorMessage = "Copy Done";
                        }

                    }
                    Result = false;
                    ErrorMessage = "Access Share Folder Fail";
                }

            }
            catch (Exception ex)
            {
                Result = false;
                ErrorMessage = ex.Message;
            }
        }
        public void CopyImageFromCacheFolderToServer(BackgroundWorker s, string ServerPath, string SourcePath, bool update, out bool Result, out string ErrorMessage)
        {
            try
            {
                using (GlobalFunction.NetUse.UNCAccessWithCredentials unc = new GlobalFunction.NetUse.UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                    {
                        if (!Directory.Exists(ServerPath))
                        {
                            Directory.CreateDirectory(ServerPath);
                        }
                        if (update)
                        {
                            Result = false;
                            ErrorMessage = "";
                        }
                        else
                        {
                            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(SourcePath);
                            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(ServerPath);

                            // Take a snapshot of the file system.  
                            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);
                            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);

                            //A custom file comparer defined below  
                            FileCompare myFileCompare = new FileCompare();

                            // This query determines whether the two folders contain  
                            // identical file lists, based on the custom file comparer  
                            // that is defined in the FileCompare class.  
                            // The query executes immediately because it returns a bool.  
                            bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

                            if (areIdentical == true)
                            {
                                Result = false;
                                ErrorMessage = "Two folders are same";
                            }
                            else
                            {
                                // Find the common files. It produces a sequence and doesn't
                                // execute until the foreach statement.  
                                var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                                var queryList1Only = (from file in list1
                                                      select file).Except(list2, myFileCompare);

                                for (int i = 0; i < queryList1Only.Count(); i++)
                                {
                                    var v = queryList1Only.ElementAt(i);
                                    //Console.WriteLine(v.FullName);
                                    if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                                    {
                                        FileInfo fi = new FileInfo(v.FullName);
                                        fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                        Result = true;
                                        ErrorMessage = "Copy Done";
                                        s.ReportProgress(i + Math.Abs(100 / (i + 1)));
                                    }

                                }
                                s.ReportProgress(100);
                            }
                            Result = true;
                            ErrorMessage = "Copy Done";
                        }

                    }
                    else
                    {
                        try
                        {
                            if (!Directory.Exists(ServerPath))
                            {
                                Directory.CreateDirectory(ServerPath);
                            }
                            if (update)
                            {
                                Result = false;
                                ErrorMessage = "";
                            }
                            else
                            {
                                System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(SourcePath);
                                System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(ServerPath);

                                // Take a snapshot of the file system.  
                                IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);
                                IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);

                                //A custom file comparer defined below  
                                FileCompare myFileCompare = new FileCompare();

                                // This query determines whether the two folders contain  
                                // identical file lists, based on the custom file comparer  
                                // that is defined in the FileCompare class.  
                                // The query executes immediately because it returns a bool.  
                                bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

                                if (areIdentical == true)
                                {
                                    Result = false;
                                    ErrorMessage = "Two folders are same";
                                }
                                else
                                {
                                    // Find the common files. It produces a sequence and doesn't
                                    // execute until the foreach statement.  
                                    var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                                    var queryList1Only = (from file in list1
                                                          select file).Except(list2, myFileCompare);

                                    for (int i = 0; i < queryList1Only.Count(); i++)
                                    {
                                        var v = queryList1Only.ElementAt(i);
                                        //Console.WriteLine(v.FullName);
                                        if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                                        {
                                            FileInfo fi = new FileInfo(v.FullName);
                                            fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                            Result = true;
                                            ErrorMessage = "Copy Done";
                                            s.ReportProgress(i + Math.Abs(100 / (i + 1)));
                                        }

                                    }
                                    s.ReportProgress(100);
                                }
                                Result = true;
                                ErrorMessage = "Copy Done";
                            }
                        }
                        catch
                        {
                            Result = false;
                            ErrorMessage = "Access Share Folder Fail";
                        }


                    }
                }

            }
            catch (Exception ex)
            {
                Result = false;
                ErrorMessage = ex.Message;
            }
        }
        public void CopyImageFromServerToCacheFolder(BackgroundWorker s, string ServerPath, string SourcePath, bool update, out bool Result, out string ErrorMessage)
        {
            try
            {
                using (GlobalFunction.NetUse.UNCAccessWithCredentials unc = new GlobalFunction.NetUse.UNCAccessWithCredentials())
                {
                    if (unc.NetUseWithCredentials(@"\\" + ConnectionClass.Oracle.Site.FTPPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                    {
                        if (!Directory.Exists(ServerPath))
                        {
                            Directory.CreateDirectory(ServerPath);
                        }
                        if (update)
                        {
                            Result = false;
                            ErrorMessage = "";
                        }
                        else
                        {
                            System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(SourcePath);
                            System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(ServerPath);

                            // Take a snapshot of the file system.  
                            IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);
                            IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);

                            //A custom file comparer defined below  
                            FileCompare myFileCompare = new FileCompare();

                            // This query determines whether the two folders contain  
                            // identical file lists, based on the custom file comparer  
                            // that is defined in the FileCompare class.  
                            // The query executes immediately because it returns a bool.  
                            bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

                            if (areIdentical == true)
                            {
                                Result = false;
                                ErrorMessage = "Two folders are same";
                            }
                            else
                            {
                                // Find the common files. It produces a sequence and doesn't
                                // execute until the foreach statement.  
                                var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                                var queryList1Only = (from file in list1
                                                      select file).Except(list2, myFileCompare);

                                for (int i = 0; i < queryList1Only.Count(); i++)
                                {
                                    var v = queryList1Only.ElementAt(i);
                                    //Console.WriteLine(v.FullName);
                                    if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                                    {
                                        FileInfo fi = new FileInfo(v.FullName);
                                        fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                        Result = true;
                                        ErrorMessage = "Copy Done";
                                        s.ReportProgress(i + Math.Abs(100 / (i + 1)));
                                    }

                                }
                                s.ReportProgress(100);
                            }
                            Result = true;
                            ErrorMessage = "Copy Done";
                        }

                    }
                    else
                    {
                        try
                        {
                            if (!Directory.Exists(ServerPath))
                            {
                                Directory.CreateDirectory(ServerPath);
                            }
                            if (update)
                            {
                                Result = false;
                                ErrorMessage = "";
                            }
                            else
                            {
                                System.IO.DirectoryInfo dir1 = new System.IO.DirectoryInfo(SourcePath);
                                System.IO.DirectoryInfo dir2 = new System.IO.DirectoryInfo(ServerPath);

                                // Take a snapshot of the file system.  
                                IEnumerable<System.IO.FileInfo> list1 = dir1.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);
                                IEnumerable<System.IO.FileInfo> list2 = dir2.GetFiles("*.jpg*", System.IO.SearchOption.AllDirectories);

                                //A custom file comparer defined below  
                                FileCompare myFileCompare = new FileCompare();

                                // This query determines whether the two folders contain  
                                // identical file lists, based on the custom file comparer  
                                // that is defined in the FileCompare class.  
                                // The query executes immediately because it returns a bool.  
                                bool areIdentical = list1.SequenceEqual(list2, myFileCompare);

                                if (areIdentical == true)
                                {
                                    Result = false;
                                    ErrorMessage = "Two folders are same";
                                }
                                else
                                {
                                    // Find the common files. It produces a sequence and doesn't
                                    // execute until the foreach statement.  
                                    var queryCommonFiles = list1.Intersect(list2, myFileCompare);

                                    var queryList1Only = (from file in list1
                                                          select file).Except(list2, myFileCompare);

                                    for (int i = 0; i < queryList1Only.Count(); i++)
                                    {
                                        var v = queryList1Only.ElementAt(i);
                                        //Console.WriteLine(v.FullName);
                                        if (unc.NetUseWithCredentials(@"\\" + FileServerPath + @"\net\SEC_IMAGE", "NETUSER", "VSFTP", "samho2020@@@"))
                                        {
                                            FileInfo fi = new FileInfo(v.FullName);
                                            fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                            Result = true;
                                            ErrorMessage = "Copy Done";
                                            s.ReportProgress(i + Math.Abs(100 / (i + 1)));
                                        }
                                        else
                                        {
                                            try
                                            {
                                                FileInfo fi = new FileInfo(v.FullName);
                                                fi.CopyTo(Path.Combine(ServerPath, fi.Name), true);
                                                Result = true;
                                                ErrorMessage = "Copy Done";
                                                s.ReportProgress(i + Math.Abs(100 / (i + 1)));

                                            }
                                            catch
                                            {

                                            }
                                        }

                                    }
                                    s.ReportProgress(100);
                                }
                                Result = true;
                                ErrorMessage = "Copy Done";
                            }
                        }
                        catch
                        {
                            Result = false;
                            ErrorMessage = "Access Share Folder Fail";
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                Result = false;
                ErrorMessage = ex.Message;
            }
        }
        public void LoadImageFromServerToCacheFolder()
        {

        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {

            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
        private void SaveImage(string filepath, System.Drawing.Image img)
        {
            // Get a bitmap.
            Bitmap bmp1 = new Bitmap(img);
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);

            // Create an Encoder object based on the GUID
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder =
                System.Drawing.Imaging.Encoder.Quality;

            // Create an EncoderParameters object.
            // An EncoderParameters object has an array of EncoderParameter
            // objects. In this case, there is only one
            // EncoderParameter object in the array.
            EncoderParameters myEncoderParameters = new EncoderParameters(1);

            //EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder,
            //    50L);
            //myEncoderParameters.Param[0] = myEncoderParameter;
            //bmp1.Save(filepath, jpgEncoder,
            //    myEncoderParameters);

            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 100L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bmp1.Save(filepath, jpgEncoder,
                myEncoderParameters);

            // Save the bitmap as a JPG file with zero quality level compression.
            //myEncoderParameter = new EncoderParameter(myEncoder, 0L);
            //myEncoderParameters.Param[0] = myEncoderParameter;
            //bmp1.Save(@"c:\TestPhotoQualityZero.jpg", jpgEncoder,
            //    myEncoderParameters);

        }
        public void TurnOffAllNetworkAdpter()
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("netsh", "interface show interface");
                psi.UseShellExecute = false;
                psi.RedirectStandardOutput = true;

                using (System.Diagnostics.Process startprocess = System.Diagnostics.Process.Start(psi))
                {
                    startprocess.WaitForExit();
                    string output = startprocess.StandardOutput.ReadToEnd();

                    MessageBox.Show(output);
                }
                //foreach (NetworkInterface adapter in adapters)
                //{
                //    networkInterfaceName = adapter.Name;
                //    Task TaskOne = Task.Factory.StartNew(() => DisableAdapter(networkInterfaceName));
                //    TaskOne.Wait();
                //    Task TaskTwo = Task.Factory.StartNew(() => EnableAdapter(networkInterfaceName));
                //    TaskTwo.Wait();
                //}               
            }
            catch (Exception e)
            {
                // Log Error Message
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = "NetworkAdaptersUtility";
                    if (e.GetType().IsAssignableFrom(typeof(System.IndexOutOfRangeException)))
                    {
                        eventLog.WriteEntry("No Network Interface Provided", EventLogEntryType.Error, 101, 1);
                    }
                    else
                    {
                        eventLog.WriteEntry(e.Message, EventLogEntryType.Error, 101, 1);
                    }
                }
            }
        }
        class FileCompare : System.Collections.Generic.IEqualityComparer<System.IO.FileInfo>
        {
            public FileCompare() { }
            public bool Equals(System.IO.FileInfo f1, System.IO.FileInfo f2)
            {
                return (f1.Name == f2.Name &&
                        f1.Length == f2.Length);
            }
            // Return a hash that reflects the comparison criteria. According to the
            // rules for IEqualityComparer<T>, if Equals is true, then the hash codes must  
            // also be equal. Because equality as defined here is a simple value equality, not  
            // reference identity, it is possible that two or more objects will produce the same  
            // hash code.  
            public int GetHashCode(System.IO.FileInfo fi)
            {
                string s = $"{fi.Name}{fi.Length}";
                return s.GetHashCode();
            }
        }
        public static void EnableAdapter(string interfaceName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", @"interface set interface  """ + interfaceName + @""" enable");
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }
        public static void DisableAdapter(string interfaceName)
        {
            ProcessStartInfo psi = new ProcessStartInfo("netsh", @"interface set interface  """ + interfaceName + @""" disable");
            Process p = new Process();
            p.StartInfo = psi;
            p.Start();
        }
        public bool InstallAndStartWindowsServices()
        {
            try
            {
                if (ServiceInstaller.ServiceIsInstalled("NetTmc Services"))
                {
                    if (ServiceInstaller.GetServiceStatus("NetTmc Services") == ServiceState.Stopped)
                    {
                        ServiceInstaller.StartService("NetTmc Services");
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    ServiceInstaller.InstallAndStart("NetTmc Services", "NetTmc Services", System.Windows.Forms.Application.StartupPath + "\\NETTMCService.exe ");
                    if (ServiceInstaller.GetServiceStatus("NetTmc Services") == ServiceState.Stopped)
                    {
                        ServiceInstaller.StartService("NetTmc Services");
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return false;
            }

        }
        public bool RestartServices()
        {
            try
            {
                if (ServiceInstaller.ServiceIsInstalled("NetTmc Services"))
                {
                    if (ServiceInstaller.GetServiceStatus("NetTmc Services") == ServiceState.Stopped)
                    {
                        ServiceInstaller.StartService("NetTmc Services");
                        return true;
                    }
                    else
                    {
                        ServiceInstaller.StopService("NetTmc Services");
                        ServiceInstaller.StartService("NetTmc Services");
                        return true;
                    }
                }
                else
                {
                    ServiceInstaller.InstallAndStart("NetTmc Services", "NetTmc Services", System.Windows.Forms.Application.StartupPath + "\\NETTMCService.exe ");
                    if (ServiceInstaller.GetServiceStatus("NetTmc Services") == ServiceState.Stopped)
                    {
                        ServiceInstaller.StartService("NetTmc Services");
                        return true;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public bool StopWindowsServices()
        {
            try
            {
                if (ServiceInstaller.GetServiceStatus("NetTmc Services") == ServiceState.Running)
                {
                    ServiceInstaller.StopService("NetTmc Services");
                    return true;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return false;
            }

        }





        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public bool OpenUserControl(UserControl uc, string uc_name, string caption, TabControl tabControl, string AccessableName, string roleID)
        {

            if (!GlobalFunction.Authentication.Permission.UserInformation.CheckPermissionOpenForm(roleID, AccessableName))
            {
                return false;
            }
            else
            {

                TabPage TAbAdd = new TabPage();
                TAbAdd.Name = uc_name;
                TAbAdd.Text = caption;
                foreach (TabPage c in tabControl.TabPages)
                {
                    if (c.Name == uc_name)
                    {
                        tabControl.SelectedTab = c;
                        return true;
                    }
                }

                if (tabControl.TabPages.Count >= 0)
                {
                    uc.Dock = DockStyle.Fill;
                    TAbAdd.Controls.Add(uc);
                    tabControl.TabPages.Add(TAbAdd);
                    tabControl.SelectedTab = TAbAdd;
                }
                return true;
            }

        }
        public bool OpenUserControlWithoutPermission(UserControl uc, string uc_name, string caption, TabControl tabControl, string AccessableName, string roleID)
        {
            TabPage TAbAdd = new TabPage();
            TAbAdd.Name = uc_name;
            TAbAdd.Text = caption;

            foreach (TabPage c in tabControl.TabPages)
            {
                if (c.Name == uc_name)
                {
                    tabControl.SelectedTab = c;
                    return true;
                }
            }

            if (tabControl.TabPages.Count >= 0)
            {
                uc.Dock = DockStyle.Fill;
                TAbAdd.Controls.Add(uc);
                tabControl.TabPages.Add(TAbAdd);
                tabControl.SelectedTab = TAbAdd;
            }
            return true;
        }
        private bool CheckPermissionOpenDirectForm(UserControl uc_name)
        {
            bool isDevIP = myIpaddress == "192.168.1.197";


            StringBuilder query = new StringBuilder();
            CRUDOracle crud = new CRUDOracle("VSMES");
            DataTable dt = new DataTable();
            switch (uc_name.Name)
            {
                case "Dashboard":
                    if (myIpaddress == "192.168.1.19" || myIpaddress == "192.168.0.213" || myIpaddress == "192.168.1.253")
                    {
                        myIpaddress = "192.168.31.69";
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    break;
                case "frmTMC9001":
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'SEC' AND N_COMNAME = '" + myIpaddress + "' AND T_DEFAULT = 'frmTMC9001'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "AndonServerPushNotification":
                    return true;
                    break;
                case "P2PNBAll":
                    return true;
                    break;
                case "P2PSTFAdidascs":
                    return true;
                    break;
                case "ChartDesign":
                    return true;
                    break;
                case "DashboardDesigner":
                    return true;
                    break;
                case "TempAndHumpMonitor":
                    return true;
                    break;
                case "P2PAdidas":
                    return true;
                    break;
                case "frmTMC7032":
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7033":
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7033_A7":
                    if (isDevIP) return true;
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7033_A14":

                    if (isDevIP) return true;

                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7036_New":
                    //if (isDevIP) return true;
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7039":
                    return true;
                    break;
                case "frmTMC7034":
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7035":
                    return true;
                    break;
                case "PrintPcard":
                    return true;
                    break;
                case "frmTMC7036":
                    if (isDevIP) return true;
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7031":
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE C_GROUP = 'BTS' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7099":
                    if (myIpaddress == "192.168.31.100") return true;
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE T_DEFAULT = '" + uc_name.Name + "' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                case "frmTMC7098":
                    if (myIpaddress == "192.168.31.100") return true;
                    query.AppendLine("SELECT COUNT(*) FROM MES.TRTB_M_COMMON WHERE T_DEFAULT = 'frmTMC7099' AND N_COMNAME = '" + myIpaddress + "'");
                    dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0][0].ToString() == "1")
                        {
                            return true;
                        }
                        else if (dt.Rows[0][0].ToString() == "0")
                        {
                            return false;
                        }
                    }
                    break;
                default:
                    return false;
            }

            return true;
        }
        public bool OpenUserControl(UserControl uc, string uc_name, string caption, TabControl tabControl)
        {
            try
            {
                //MessageBox.Show("Tới đây rồi. " + uc.Name + " " + myIpaddress);
                TabPage TAbAdd = new TabPage();
                TAbAdd.Name = uc_name;
                TAbAdd.Text = caption;
                if (!CheckPermissionOpenDirectForm(uc))
                {
                    //MessageBox.Show("Check bị failed rồi. " + uc.Name + " " + myIpaddress);
                    return false;
                }
                //MessageBox.Show("Check permission OK rồi " + uc.Name + " " + myIpaddress);
                foreach (TabPage c in tabControl.TabPages)
                {
                    if (c.Name == uc_name)
                    {
                        tabControl.SelectedTab = c;
                        return true;
                    }
                }

                if (tabControl.TabPages.Count >= 0)
                {
                    uc.Dock = DockStyle.Fill;
                    TAbAdd.Controls.Add(uc);
                    tabControl.TabPages.Add(TAbAdd);
                    tabControl.SelectedTab = TAbAdd;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurs. Please check error code below " + ex.Message, "Error ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public void ExportToExcelFile(DataGridView gv, string sheetName)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select path to export excel";

                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    string path = Path.Combine(
                        fbd.SelectedPath,
                        $"{sheetName}_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    );

                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using (var package = new ExcelPackage())
                    {
                        var ws = package.Workbook.Worksheets.Add(sheetName);

                        // Header
                        for (int i = 0; i < gv.Columns.Count; i++)
                        {
                            ws.Cells[1, i + 1].Value = gv.Columns[i].HeaderText;
                        }

                        // Data
                        for (int i = 0; i < gv.Rows.Count; i++)
                        {
                            for (int j = 0; j < gv.Columns.Count; j++)
                            {
                                ws.Cells[i + 2, j + 1].Value = gv.Rows[i].Cells[j].Value;
                            }
                        }

                        ws.Cells.AutoFitColumns();

                        File.WriteAllBytes(path, package.GetAsByteArray());
                    }

                    if (MessageBox.Show("Export complete", "Information",
                        MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }
                }
            }
        }
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
