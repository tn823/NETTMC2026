using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace QIP.EOL.Popup
{
    public partial class SPCClean : Form
    {
        public SPCClean()
        {
            InitializeComponent();
        }

        public DataTable dtSPC { get; set; }

        private void SPCClean_Load(object sender, EventArgs e)
        {
            chartControl1.Series.Clear();
            chartControl1.ChartAreas.Clear();
            chartControl1.Legends.Clear();

            ChartArea chartArea = new ChartArea("MainArea");
            chartArea.AxisX.Title = "Subgroup Number";
            chartArea.AxisY.Title = "Fraction Nonconforming";

            chartArea.AxisX.TitleFont = new Font("Tahoma", 10F, FontStyle.Bold);
            chartArea.AxisY.TitleFont = new Font("Tahoma", 10F, FontStyle.Bold);

            chartArea.AxisX.IntervalAutoMode = IntervalAutoMode.VariableCount;
            chartArea.AxisX.LabelStyle.Angle = -45;
            chartArea.AxisX.MajorGrid.LineColor = Color.LightGray;
            chartArea.AxisY.MajorGrid.LineColor = Color.LightGray;

            chartControl1.ChartAreas.Add(chartArea);

            Legend legend = new Legend("DefaultLegend");
            chartControl1.Legends.Add(legend);

            if (dtSPC == null || dtSPC.Rows.Count == 0)
            {
                return;
            }

            AddLineSeries("p", "D_GATHER_SLOT", "P", Color.Blue);
            AddLineSeries("p-bar", "D_GATHER_SLOT", "PBAR", Color.Black);
            AddLineSeries("UCL", "D_GATHER_SLOT", "UCL", Color.Red);
            AddLineSeries("LCL", "D_GATHER_SLOT", "LCL", Color.Blue);
        }

        private void AddLineSeries(string seriesName, string xColumn, string yColumn, Color color)
        {
            Series series = new Series(seriesName);
            series.ChartType = SeriesChartType.Line;
            series.BorderWidth = 2;
            series.Color = color;
            series.XValueType = ChartValueType.DateTime;
            series.MarkerStyle = MarkerStyle.Circle;
            series.MarkerSize = 7;
            series.ChartArea = "MainArea";
            series.Legend = "DefaultLegend";
            series.IsXValueIndexed = true; // Fix để các điểm không bị gom lại nếu trùng X

            foreach (DataRow row in dtSPC.Rows)
            {
                //string raw = row[xColumn]?.ToString();
                string xValue = row[xColumn]?.ToString() ?? "";

                double yValue = 0;
                // Parse double an toàn không bị ảnh hưởng bởi culture (dấu phẩy hay dấu chấm)
                if (row[yColumn] != DBNull.Value && row[yColumn] != null)
                {
                    if (row[yColumn] is IConvertible)
                    {
                        yValue = Convert.ToDouble(row[yColumn], System.Globalization.CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        double.TryParse(row[yColumn].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out yValue);
                    }
                }
                series.Points.AddXY(xValue, yValue);
                //quyentest
                //DateTime xDate = DateTime.Now;

                //try
                //{
                //    // raw: 16(0730-0830)
                //    int day = int.Parse(raw.Substring(0, 2));
                //    int hour = int.Parse(raw.Substring(3, 2));
                //    int minute = int.Parse(raw.Substring(5, 2));

                //    xDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day, hour, minute, 0);
                //}
                //catch
                //{
                //    continue;
                //}

                //if (double.TryParse(row[yColumn]?.ToString(), out double yValue))
                //{
                //    series.Points.AddXY(xDate, yValue);
                //}
                //quyentest
            }

            chartControl1.Series.Add(series);
            
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
