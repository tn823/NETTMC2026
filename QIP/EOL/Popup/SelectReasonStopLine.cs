using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QIP.EOL.Popup
{
    public partial class SelectReasonStopLine : Form
    {
        public DataTable dtReason
        {
            get;
            set;
        }
        public string ReturnSelection
        {
            get;
            set;
        }
        public SelectReasonStopLine()
        {
            InitializeComponent();
        }

        private void BtnClose_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        //private void SelectReasonStopLine_Load(object sender, EventArgs e)
        //{
        //    this.gridControl1.DataSource = dtReason;
        //    foreach (GridColumn gc in gridView1.Columns)
        //    {
        //        gc.AppearanceCell.Font = new Font("VNI-Times", 12);
        //    }
        //    gridView1.Appearance.Row.Font = new Font("VNI-Times", 12);
        //    this.gridView1.BestFitColumns();
        //}
        private void SelectReasonStopLine_Load(object sender, EventArgs e)
        {
            gridView1.DataSource = dtReason;
            gridView1.DefaultCellStyle.Font = new Font("VNI-Times", 12);
            gridView1.ColumnHeadersDefaultCellStyle.Font = new Font("VNI-Times", 12);

            foreach (DataGridViewColumn col in gridView1.Columns)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        //private void gridView1_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        //{
        //    this.ReturnSelection = this.gridView1.GetRowCellValue(e.RowHandle, e.Column).ToString();
        //    this.Close();
        //}
        private void gridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object value = gridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                ReturnSelection = value != null ? value.ToString() : string.Empty;
                this.Close();
            }
        }

        private void gridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            e.CellStyle.Font = new Font("VNI-Times", 12);
        }
    }
}
