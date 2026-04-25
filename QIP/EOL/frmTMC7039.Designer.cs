namespace QIP.EOL
{
    partial class frmTMC7039
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
            components = new System.ComponentModel.Container();
            panel1 = new Panel();
            panel2 = new Panel();
            labelControl1 = new Label();
            btnPlantF = new Button();
            btnPlantB = new Button();
            btnPlantA = new Button();
            labelControl2 = new Label();
            tableLayoutPanel1 = new TableLayoutPanel();
            timer1 = new System.Windows.Forms.Timer(components);
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(labelControl1);
            panel1.Controls.Add(panel2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(979, 62);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.BackColor = Color.DarkOrange;
            panel2.Controls.Add(labelControl2);
            panel2.Controls.Add(btnPlantA);
            panel2.Controls.Add(btnPlantB);
            panel2.Controls.Add(btnPlantF);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 37);
            panel2.Name = "panel2";
            panel2.Size = new Size(979, 25);
            panel2.TabIndex = 0;
            // 
            // labelControl1
            // 
            labelControl1.BackColor = Color.DarkOrange;
            labelControl1.Dock = DockStyle.Fill;
            labelControl1.Font = new Font("Tahoma", 18F, FontStyle.Bold);
            labelControl1.ForeColor = Color.White;
            labelControl1.Location = new Point(0, 0);
            labelControl1.Name = "labelControl1";
            labelControl1.Size = new Size(979, 37);
            labelControl1.TabIndex = 1;
            labelControl1.Text = "NOTICE BOARD FOR DEFECT IMAGES";
            labelControl1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnPlantF
            // 
            btnPlantF.BackColor = Color.LightGoldenrodYellow;
            btnPlantF.Dock = DockStyle.Right;
            btnPlantF.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            btnPlantF.Location = new Point(904, 0);
            btnPlantF.Name = "btnPlantF";
            btnPlantF.Size = new Size(75, 25);
            btnPlantF.TabIndex = 0;
            btnPlantF.Text = "PLANT F";
            btnPlantF.UseVisualStyleBackColor = false;
            // 
            // btnPlantB
            // 
            btnPlantB.BackColor = Color.LightGoldenrodYellow;
            btnPlantB.Dock = DockStyle.Right;
            btnPlantB.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            btnPlantB.Location = new Point(829, 0);
            btnPlantB.Name = "btnPlantB";
            btnPlantB.Size = new Size(75, 25);
            btnPlantB.TabIndex = 1;
            btnPlantB.Text = "PLANT B";
            btnPlantB.UseVisualStyleBackColor = false;
            // 
            // btnPlantA
            // 
            btnPlantA.BackColor = Color.LightGoldenrodYellow;
            btnPlantA.Dock = DockStyle.Right;
            btnPlantA.Font = new Font("Tahoma", 8.25F, FontStyle.Bold);
            btnPlantA.Location = new Point(754, 0);
            btnPlantA.Name = "btnPlantA";
            btnPlantA.Size = new Size(75, 25);
            btnPlantA.TabIndex = 2;
            btnPlantA.Text = "PLANT A";
            btnPlantA.UseVisualStyleBackColor = false;
            // 
            // labelControl2
            // 
            labelControl2.Dock = DockStyle.Right;
            labelControl2.Font = new Font("Tahoma", 9.75F, FontStyle.Bold);
            labelControl2.ForeColor = Color.White;
            labelControl2.Location = new Point(241, 0);
            labelControl2.Name = "labelControl2";
            labelControl2.Size = new Size(513, 25);
            labelControl2.TabIndex = 1;
            labelControl2.Text = "Auto switch another Plant (1mins) or click here ->";
            labelControl2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 62);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            tableLayoutPanel1.Size = new Size(979, 469);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // timer1
            // 
            timer1.Enabled = true;
            timer1.Interval = 60000;
            // 
            // frmTMC7039
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(tableLayoutPanel1);
            Controls.Add(panel1);
            Name = "frmTMC7039";
            Size = new Size(979, 531);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Panel panel2;
        private Label labelControl1;
        private Label labelControl2;
        private Button btnPlantA;
        private Button btnPlantB;
        private Button btnPlantF;
        private TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Timer timer1;
    }
}
