namespace QIP.EOL.Popup
{
    partial class SelectFail
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.gridView1 = new System.Windows.Forms.DataGridView();
            this.grdDefect = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.grdCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridView1
            // 
            this.gridView1.AllowUserToAddRows = false;
            this.gridView1.AllowUserToDeleteRows = false;
            this.gridView1.AllowUserToResizeRows = false;
            this.gridView1.AutoGenerateColumns = false;
            this.gridView1.BackgroundColor = System.Drawing.Color.White;
            this.gridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.gridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.grdDefect,
            this.grdCount});
            this.gridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridView1.Location = new System.Drawing.Point(0, 0);
            this.gridView1.MultiSelect = false;
            this.gridView1.Name = "gridView1";
            this.gridView1.ReadOnly = true;
            this.gridView1.RowHeadersWidth = 50;
            this.gridView1.RowTemplate.Height = 30;
            this.gridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridView1.Size = new System.Drawing.Size(521, 683);
            this.gridView1.TabIndex = 0;
            // 
            // grdDefect
            // 
            this.grdDefect.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.grdDefect.DataPropertyName = "REASON_VN";
            this.grdDefect.HeaderText = "REASON NAME";
            this.grdDefect.Name = "grdDefect";
            this.grdDefect.ReadOnly = true;
            this.grdDefect.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
            {
                Font = new System.Drawing.Font("VNI-Times", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            this.grdDefect.HeaderCell.Style = new System.Windows.Forms.DataGridViewCellStyle
            {
                Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            // 
            // grdCount
            // 
            this.grdCount.DataPropertyName = "COUNT";
            this.grdCount.HeaderText = "COUNT";
            this.grdCount.Name = "grdCount";
            this.grdCount.ReadOnly = true;
            this.grdCount.Width = 112;
            this.grdCount.DefaultCellStyle = new System.Windows.Forms.DataGridViewCellStyle
            {
                Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight,
                Format = "N0",
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            this.grdCount.HeaderCell.Style = new System.Windows.Forms.DataGridViewCellStyle
            {
                Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))),
                WrapMode = System.Windows.Forms.DataGridViewTriState.True
            };
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 683);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(521, 43);
            this.panel1.TabIndex = 1;
            // 
            // btnOK
            // 
            this.btnOK.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOK.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Location = new System.Drawing.Point(167, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(177, 43);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnClose
            // 
            this.btnClose.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(344, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(177, 43);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // SelectFail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(521, 726);
            this.ControlBox = false;
            this.Controls.Add(this.gridView1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SelectFail";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SelectFail";
            this.Load += new System.EventHandler(this.SelectFail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.DataGridView gridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn grdDefect;
        private System.Windows.Forms.DataGridViewTextBoxColumn grdCount;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnClose;
    }
}