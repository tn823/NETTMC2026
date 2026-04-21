namespace QIP.EOL
{
    partial class frmTMC7032
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tableLayoutPanel1 = new TableLayoutPanel();
            panel5 = new Panel();
            panel6 = new Panel();
            txtTime = new Label();
            labelControl11 = new Label();
            chkEng = new CheckBox();
            chkVN = new CheckBox();
            tableLayoutPanel1.SuspendLayout();
            panel5.SuspendLayout();
            panel6.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 589F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(panel5, 1, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            tableLayoutPanel1.Size = new Size(1579, 973);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // panel5
            // 
            panel5.Controls.Add(panel6);
            panel5.Dock = DockStyle.Fill;
            panel5.Location = new Point(592, 3);
            panel5.Name = "panel5";
            panel5.Size = new Size(984, 480);
            panel5.TabIndex = 0;
            // 
            // panel6
            // 
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel6.Controls.Add(chkVN);
            panel6.Controls.Add(chkEng);
            panel6.Controls.Add(labelControl11);
            panel6.Controls.Add(txtTime);
            panel6.Dock = DockStyle.Top;
            panel6.Location = new Point(0, 0);
            panel6.Name = "panel6";
            panel6.Size = new Size(984, 32);
            panel6.TabIndex = 0;
            // 
            // txtTime
            // 
            txtTime.BackColor = Color.Black;
            txtTime.Dock = DockStyle.Left;
            txtTime.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtTime.ForeColor = Color.Lavender;
            txtTime.Location = new Point(0, 0);
            txtTime.Margin = new Padding(3, 4, 3, 4);
            txtTime.Name = "txtTime";
            txtTime.Size = new Size(261, 30);
            txtTime.TabIndex = 7;
            txtTime.Text = "2018-11-19 15:46:45";
            txtTime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // labelControl11
            // 
            labelControl11.BackColor = Color.Transparent;
            labelControl11.Dock = DockStyle.Left;
            labelControl11.Font = new Font("Tahoma", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelControl11.ForeColor = Color.Navy;
            labelControl11.Location = new Point(261, 0);
            labelControl11.Margin = new Padding(3, 4, 3, 4);
            labelControl11.Name = "labelControl11";
            labelControl11.Size = new Size(96, 30);
            labelControl11.TabIndex = 25;
            labelControl11.Text = "Language";
            labelControl11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // chkEng
            // 
            chkEng.Dock = DockStyle.Left;
            chkEng.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkEng.Location = new Point(357, 0);
            chkEng.Margin = new Padding(3, 4, 3, 4);
            chkEng.Name = "chkEng";
            chkEng.Size = new Size(49, 30);
            chkEng.TabIndex = 27;
            chkEng.Text = "ENG";
            chkEng.TextAlign = ContentAlignment.MiddleCenter;
            chkEng.UseVisualStyleBackColor = true;
            // 
            // chkVN
            // 
            chkVN.Checked = true;
            chkVN.CheckState = CheckState.Checked;
            chkVN.Dock = DockStyle.Left;
            chkVN.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkVN.Location = new Point(406, 0);
            chkVN.Margin = new Padding(3, 4, 3, 4);
            chkVN.Name = "chkVN";
            chkVN.Size = new Size(45, 30);
            chkVN.TabIndex = 28;
            chkVN.Text = "VN";
            chkVN.TextAlign = ContentAlignment.MiddleCenter;
            chkVN.UseVisualStyleBackColor = true;
            // 
            // frmTMC7032
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            Controls.Add(tableLayoutPanel1);
            Name = "frmTMC7032";
            Size = new Size(1579, 973);
            tableLayoutPanel1.ResumeLayout(false);
            panel5.ResumeLayout(false);
            panel6.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private Panel panel5;
        private Panel panel6;
        private Label txtTime;
        private Label labelControl11;
        private CheckBox chkEng;
        private CheckBox chkVN;
    }
}
