namespace NetTMCVersion
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            progBar = new ProgressBar();
            label1 = new Label();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // progBar
            // 
            progBar.Dock = DockStyle.Top;
            progBar.Location = new Point(0, 0);
            progBar.Name = "progBar";
            progBar.Size = new Size(502, 44);
            progBar.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(0, 63);
            label1.Name = "label1";
            label1.Size = new Size(0, 15);
            label1.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(0, 47);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(0, 15);
            lblStatus.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(502, 87);
            Controls.Add(lblStatus);
            Controls.Add(label1);
            Controls.Add(progBar);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "NetTMCVersion ( Checking newest version and download...)";
            Load += Form1_Load;
            Shown += Update_Shown;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ProgressBar progBar;
        private Label label1;
        private Label lblStatus;
    }
}
