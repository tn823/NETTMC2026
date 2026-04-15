using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QIP.EOL
{
    public partial class SelectModel : Form
    {
        public DataTable dtModel
        {
            get;
            set;
        }
        public string ReturnSelection
        {
            get;
            set;
        }
        public SelectModel()
        {
            InitializeComponent();
        }

        //private void SelectModel_Load(object sender, EventArgs e)
        //{
        //    this.gridControl1.DataSource = dtModel;
        //    this.gridView1.BestFitColumns();
        //}
        private void SelectModel_Load(object sender, EventArgs e)
        {
            gridView1.DataSource = dtModel;
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
                ReturnSelection = value != null ? value.ToString() : "";
                this.Close();
            }
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.ReturnSelection = "";
            this.Close();
        }
    }
}
