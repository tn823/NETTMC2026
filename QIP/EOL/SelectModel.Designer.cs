namespace QIP.EOL
{
    partial class SelectModel
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.gridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnClose = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();

            // 
            // gridView1
            // 
            this.gridView1.AllowUserToAddRows = false;
            this.gridView1.AllowUserToDeleteRows = false;
            this.gridView1.AllowUserToResizeColumns = false;
            this.gridView1.AllowUserToResizeRows = false;
            this.gridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.gridView1.BackgroundColor = System.Drawing.Color.White;
            this.gridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Single;
            this.gridView1.ColumnHeadersVisible = false;
            this.gridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView1.ReadOnly = true;
            this.gridView1.RowHeadersVisible = false;
            this.gridView1.RowTemplate.Height = 50;
            this.gridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridView1_CellClick);

            // tạo 5 cột giống DevExpress
            this.gridView1.Columns.Add("col1", "col1");
            this.gridView1.Columns.Add("col2", "col2");
            this.gridView1.Columns.Add("col3", "col3");
            this.gridView1.Columns.Add("col4", "col4");
            this.gridView1.Columns.Add("col5", "col5");

            // style giống DevExpress
            this.gridView1.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F);
            this.gridView1.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.gridView1.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;

            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Height = 45;

            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Width = 180;
            this.btnClose.Text = "CLOSE";
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 12F);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // 
            // SelectModel
            // 
            this.ClientSize = new System.Drawing.Size(1257, 531);
            this.Controls.Add(this.gridView1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "SelectModel";
            this.Load += new System.EventHandler(this.SelectModel_Load);

            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.DataGridView gridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnClose;
    }
}