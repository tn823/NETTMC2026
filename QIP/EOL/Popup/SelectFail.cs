using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionClass.Oracle;

namespace QIP.EOL.Popup
{
    public partial class SelectFail : Form
    {
        private CRUDOracle crud;
        public DataTable returnData
        {
            get;
            set;
        }
        public SelectFail()
        {
            InitializeComponent();
            crud = new CRUDOracle("VSMES");
        }

        private void SelectFail_Load(object sender, EventArgs e)
        {
            LoadDataIntoGridView();
            gridView1.CellFormatting += GridView1_CellFormatting;
            gridView1.CellClick += gridView1_CellClick;
        }

        //private void GridView1_RowCellStyle(object sender, DevExpress.XtraGrid.Views.Grid.RowCellStyleEventArgs e)
        //{
        //    if (e.Column.FieldName == "COUNT")
        //    {
        //        if (e.CellValue.ToString() != "0")
        //        {
        //            e.Appearance.BackColor = Color.LightBlue;
        //            e.Appearance.BackColor2 = Color.LightBlue;
        //            e.Appearance.ForeColor = Color.Red;

        //        }
        //    }
        //}

        //private void gridView1_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        //{
        //    int x = 0;
        //    int y = 0;
        //    if (int.TryParse(gridView1.GetRowCellValue(e.RowHandle, "COUNT").ToString(), out y))
        //    {
        //        x = y + 1;
        //        gridView1.SetRowCellValue(e.RowHandle, "COUNT", x);
        //    }
        //    else
        //    {
        //        MessageBox.Show("Giá trị COUNT không đúng!!!");
        //        //Errro thong bao so count ko dung ....
        //    }
        //}

        private void GridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            DataGridViewColumn column = gridView1.Columns[e.ColumnIndex];
            if (column == null)
                return;

            bool isCountColumn =
                column.Name == "grdCount" ||
                string.Equals(column.DataPropertyName, "COUNT", StringComparison.OrdinalIgnoreCase);

            if (!isCountColumn)
                return;

            string cellValue = e.Value?.ToString() ?? "0";
            if (cellValue != "0")
            {
                e.CellStyle.BackColor = Color.LightBlue;
                e.CellStyle.ForeColor = Color.Red;
            }
        }

        private void gridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            object value = gridView1.Rows[e.RowIndex].Cells["grdCount"].Value;

            if (int.TryParse(value?.ToString(), out int y))
            {
                int x = y + 1;
                gridView1.Rows[e.RowIndex].Cells["grdCount"].Value = x;
                gridView1.Refresh();
            }
            else
            {
                MessageBox.Show("Giá trị COUNT không đúng!!!");
            }
        }

        private void LoadDataIntoGridView()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");
            query.AppendLine("        SELECT PART_ID, REASON_ID, REASON_SHORT, REASON_EN,  REASON_VN, 0 COUNT                               ");
            query.AppendLine(" FROM MES.TRTB_M_BTS_REASON3@inf_m_e                                                                  ");
            query.AppendLine("WHERE DEPT_CODE = 'ASS' AND REASON_ID IN (1,2,3,4,5,6,6,7,8,9,10,11,12,13,14,15,16,19,20,22,23,24)                                                  ");
            try
            {
                DataTable dt = crud.dac.DtSelectExcuteWithQuery(query.ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    //gridControl1.DataSource = dt;
                    returnData = dt;
                    gridView1.AutoGenerateColumns = false;
                    gridView1.DataSource = dt;
                }
                else
                {
                    MessageBox.Show("Không có dữ liệu trả về.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        //private void simpleButton2_Click(object sender, EventArgs e)
        //{
        //    this.gridView1.PostEditor();
        //    this.gridView1.UpdateCurrentRow();
        //    if (!this.gridView1.ValidateEditor())
        //    {
        //        return;
        //    }
        //    DataTable dt = (DataTable)this.gridControl1.DataSource;
        //    returnData = dt;
        //    this.Close();
        //}

        //private void simpleButton1_Click(object sender, EventArgs e)
        //{
        //    this.Close();
        //}
        private void btnOK_Click(object sender, EventArgs e)
        {
            gridView1.EndEdit();

            if (gridView1.DataSource is DataTable dt)
            {
                returnData = dt;
            }

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SelectFail_Load_1(object sender, EventArgs e)
        {

        }
    }
}
