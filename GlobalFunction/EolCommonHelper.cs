using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GlobalFunction
{
    public static class EolCommonHelper
    {
        public static void SaveLineNameToCsv(DataTable dt, string filePath)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("C_COMCODE,SHOW_LINE");

                foreach (DataRow row in dt.Rows)
                {
                    string code = row["C_COMCODE"]?.ToString().Replace(",", "") ?? "";
                    string showLine = row["SHOW_LINE"]?.ToString().Replace(",", "") ?? "";
                    sb.AppendLine($"{code},{showLine}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Debug.WriteLine("SaveLineNameToCsv OK: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SaveLineNameToCsv Error: " + ex.Message);
            }
        }

        public static DataTable ReadLineNameFromCsv(string filePath)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("C_COMCODE");
                dt.Columns.Add("SHOW_LINE");

                if (!File.Exists(filePath))
                    return dt;

                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);

                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] parts = lines[i].Split(',');
                    if (parts.Length >= 2)
                    {
                        DataRow row = dt.NewRow();
                        row["C_COMCODE"] = parts[0].Trim();
                        row["SHOW_LINE"] = parts[1].Trim();
                        dt.Rows.Add(row);
                    }
                }

                Debug.WriteLine($"ReadLineNameFromCsv OK: {dt.Rows.Count} rows");
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReadLineNameFromCsv Error: " + ex.Message);
                return null;
            }
        }

        public static void SaveErrorButtonToCsv(DataTable dt, string filePath)
        {
            try
            {
                var sb = new StringBuilder();
                sb.AppendLine("PART_ID,REASON_ID,REASON_SHORT,REASON_EN,REASON_VN");

                foreach (DataRow row in dt.Rows)
                {
                    string partId = EscapeCsv(row["PART_ID"]?.ToString() ?? "");
                    string reasonId = EscapeCsv(row["REASON_ID"]?.ToString() ?? "");
                    string reasonShort = EscapeCsv(row["REASON_SHORT"]?.ToString() ?? "");
                    string reasonEn = EscapeCsv(row["REASON_EN"]?.ToString() ?? "");
                    string reasonVn = EscapeCsv(row["REASON_VN"]?.ToString() ?? "");
                    sb.AppendLine($"{partId},{reasonId},{reasonShort},{reasonEn},{reasonVn}");
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                Debug.WriteLine("SaveErrorButtonToCsv OK: " + filePath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("SaveErrorButtonToCsv Error: " + ex.Message);
            }
        }

        public static DataTable ReadErrorButtonFromCsv(string filePath)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.Add("PART_ID");
                dt.Columns.Add("REASON_ID");
                dt.Columns.Add("REASON_SHORT");
                dt.Columns.Add("REASON_EN");
                dt.Columns.Add("REASON_VN");

                if (!File.Exists(filePath))
                    return dt;

                string[] lines = File.ReadAllLines(filePath, Encoding.UTF8);
                for (int i = 1; i < lines.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(lines[i])) continue;

                    string[] parts = ParseCsvLine(lines[i]);
                    if (parts.Length >= 5)
                    {
                        DataRow row = dt.NewRow();
                        row["PART_ID"] = parts[0].Trim();
                        row["REASON_ID"] = parts[1].Trim();
                        row["REASON_SHORT"] = parts[2].Trim();
                        row["REASON_EN"] = parts[3].Trim();
                        row["REASON_VN"] = parts[4].Trim();
                        dt.Rows.Add(row);
                    }
                }

                Debug.WriteLine($"ReadErrorButtonFromCsv OK: {dt.Rows.Count} rows");
                return dt;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ReadErrorButtonFromCsv Error: " + ex.Message);
                return null;
            }
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            return value;
        }

        private static string[] ParseCsvLine(string line)
        {
            var result = new List<string>();
            bool inQuotes = false;
            var current = new StringBuilder();

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }

            result.Add(current.ToString());
            return result.ToArray();
        }


        public static string GetVoiceLogDir()
        {
            string relativeLogDir = System.IO.Path.Combine("VoiceRecognition", "test", "log");
            string[] searchRoots =
            {
                AppDomain.CurrentDomain.BaseDirectory,
                Application.StartupPath,
                System.IO.Directory.GetCurrentDirectory()
            };

            foreach (string root in searchRoots.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var dir = new System.IO.DirectoryInfo(root);
                while (dir != null)
                {
                    string voiceDir = System.IO.Path.Combine(dir.FullName, "VoiceRecognition");
                    if (System.IO.Directory.Exists(voiceDir))
                    {
                        return System.IO.Path.Combine(dir.FullName, relativeLogDir);
                    }

                    dir = dir.Parent;
                }
            }

            return System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeLogDir);
        }

    }
}
