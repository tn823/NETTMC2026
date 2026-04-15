namespace QIP.EOL.Popup
{
    partial class SelectReasonStopLine
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClose = new System.Windows.Forms.Button();
            this.gridView1 = new System.Windows.Forms.DataGridView();
            this.gridColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gridColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.LightGray;
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnClose.Font = new System.Drawing.Font("Arial", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(0, 442);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(1471, 50);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "CLOSE - ĐÓNG LẠI";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
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
            this.gridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.gridView1.ColumnHeadersVisible = false;
            this.gridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.gridColumn1,
            this.gridColumn2,
            this.gridColumn3,
            this.gridColumn4});
            this.gridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gridView1.EnableHeadersVisualStyles = false;
            this.gridView1.Location = new System.Drawing.Point(0, 0);
            this.gridView1.MultiSelect = false;
            this.gridView1.Name = "gridView1";
            this.gridView1.ReadOnly = true;
            this.gridView1.RowHeadersVisible = false;
            this.gridView1.RowTemplate.Height = 50;
            this.gridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.gridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridView1.Size = new System.Drawing.Size(1471, 442);
            this.gridView1.TabIndex = 0;
            this.gridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridView1_CellClick);
            this.gridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gridView1_CellFormatting);
            // 
            // gridColumn1
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridColumn1.DefaultCellStyle = dataGridViewCellStyle1;
            this.gridColumn1.FillWeight = 25F;
            this.gridColumn1.HeaderText = "col1";
            this.gridColumn1.Name = "gridColumn1";
            this.gridColumn1.ReadOnly = true;
            this.gridColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridColumn2
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Times New Roman", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridColumn2.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridColumn2.FillWeight = 25F;
            this.gridColumn2.HeaderText = "col2";
            this.gridColumn2.Name = "gridColumn2";
            this.gridColumn2.ReadOnly = true;
            this.gridColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridColumn3
            // 
            this.gridColumn3.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridColumn3.FillWeight = 25F;
            this.gridColumn3.HeaderText = "col3";
            this.gridColumn3.Name = "gridColumn3";
            this.gridColumn3.ReadOnly = true;
            this.gridColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // gridColumn4
            // 
            this.gridColumn4.DefaultCellStyle = dataGridViewCellStyle2;
            this.gridColumn4.FillWeight = 25F;
            this.gridColumn4.HeaderText = "col4";
            this.gridColumn4.Name = "gridColumn4";
            this.gridColumn4.ReadOnly = true;
            this.gridColumn4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // SelectReasonStopLine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gridView1);
            this.Controls.Add(this.btnClose);
            this.Name = "SelectReasonStopLine";
            this.Size = new System.Drawing.Size(1471, 492);
            this.Load += new System.EventHandler(this.SelectReasonStopLine_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView gridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn gridColumn4;
    }
}