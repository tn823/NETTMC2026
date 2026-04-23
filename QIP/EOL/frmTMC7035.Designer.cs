namespace QIP.EOL
{
    partial class frmTMC7035
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
            panelControl1 = new Panel();
            simpleButton1 = new Button();
            btnModel = new Button();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // panelControl1
            // 
            panelControl1.Controls.Add(btnModel);
            panelControl1.Controls.Add(simpleButton1);
            panelControl1.Dock = DockStyle.Top;
            panelControl1.Location = new Point(0, 0);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(1475, 57);
            panelControl1.TabIndex = 0;
            // 
            // simpleButton1
            // 
            simpleButton1.BackColor = Color.Red;
            simpleButton1.Dock = DockStyle.Right;
            simpleButton1.Font = new Font("Tahoma", 14.25F, FontStyle.Bold);
            simpleButton1.ForeColor = Color.White;
            simpleButton1.Location = new Point(1319, 0);
            simpleButton1.Name = "simpleButton1";
            simpleButton1.Size = new Size(156, 57);
            simpleButton1.TabIndex = 0;
            simpleButton1.Text = "ANDON";
            simpleButton1.UseVisualStyleBackColor = false;
            // 
            // btnModel
            // 
            btnModel.BackColor = Color.SlateBlue;
            btnModel.Dock = DockStyle.Right;
            btnModel.Font = new Font("Tahoma", 14.25F, FontStyle.Bold);
            btnModel.ForeColor = Color.White;
            btnModel.Location = new Point(1117, 0);
            btnModel.Name = "btnModel";
            btnModel.Size = new Size(202, 57);
            btnModel.TabIndex = 1;
            btnModel.Text = "Chọn MODEL";
            btnModel.UseVisualStyleBackColor = false;
            // 
            // frmTMC7035
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(panelControl1);
            Name = "frmTMC7035";
            Size = new Size(1475, 748);
            panelControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelControl1;
        private Button simpleButton1;
        private Button btnModel;
    }
}
