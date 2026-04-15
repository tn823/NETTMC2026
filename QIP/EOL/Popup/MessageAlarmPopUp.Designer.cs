namespace QIP.EOL.Popup
{
    partial class MessageAlarmPopUp
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panelControl1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblMessage = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.simpleButton1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);

            this.panelControl1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();

            // panelControl1 → Panel
            this.panelControl1.Controls.Add(this.tableLayoutPanel1);
            this.panelControl1.Controls.Add(this.simpleButton1);
            this.panelControl1.Dock = DockStyle.Fill;

            // button (top)
            this.simpleButton1.Dock = DockStyle.Top;
            this.simpleButton1.Height = 75;
            this.simpleButton1.Font = new System.Drawing.Font("Tahoma", 26.25F, FontStyle.Bold);
            this.simpleButton1.Text = "ĐÓNG ( CLOSE )";
            this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);

            // tableLayoutPanel
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Red;
            this.tableLayoutPanel1.Dock = DockStyle.Fill;
            this.tableLayoutPanel1.ColumnCount = 5;

            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 5F));

            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 28.9F));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 4F));
            this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 66.6F));

            // lblMessage (DevExpress LabelControl → Label)
            this.lblMessage.Dock = DockStyle.Fill;
            this.lblMessage.BackColor = System.Drawing.Color.Red;
            this.lblMessage.ForeColor = System.Drawing.Color.White;
            this.lblMessage.Font = new System.Drawing.Font("Tahoma", 28F, FontStyle.Bold);
            this.lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            this.tableLayoutPanel1.SetColumnSpan(this.lblMessage, 5);

            // pictureBox1
            this.pictureBox1.Dock = DockStyle.Fill;
            this.pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

            // pictureBox2
            this.pictureBox2.Dock = DockStyle.Fill;
            this.pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            // add controls
            this.tableLayoutPanel1.Controls.Add(this.lblMessage, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 3, 2);

            // timer1
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

            // timer2
            this.timer2.Interval = 60000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);

            // form
            this.ClientSize = new System.Drawing.Size(1043, 497);
            this.Controls.Add(this.panelControl1);
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.WindowState = FormWindowState.Maximized;

            this.Load += new System.EventHandler(this.MessageAlarmPopUp_Load);

            this.panelControl1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelControl1;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Button simpleButton1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}