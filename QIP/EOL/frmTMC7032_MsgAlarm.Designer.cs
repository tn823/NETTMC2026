namespace QIP.EOL
{
    partial class frmTMC7032_MsgAlarm
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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblMessage = new Label();
            button1 = new Button();
            panel1 = new Panel();
            label2 = new Label();
            textBox1 = new TextBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.BackColor = Color.Red;
            lblMessage.Dock = DockStyle.Fill;
            lblMessage.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold);
            lblMessage.Location = new Point(0, 140);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(1102, 380);
            lblMessage.TabIndex = 0;
            // 
            // button1
            // 
            button1.Dock = DockStyle.Top;
            button1.Font = new Font("Tahoma", 50.25F, FontStyle.Bold);
            button1.Location = new Point(0, 0);
            button1.Name = "button1";
            button1.Size = new Size(1102, 101);
            button1.TabIndex = 1;
            button1.Text = "CLOSE ( Đóng )";
            button1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            panel1.Controls.Add(textBox1);
            panel1.Controls.Add(label2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 101);
            panel1.Name = "panel1";
            panel1.Size = new Size(1102, 39);
            panel1.TabIndex = 2;
            // 
            // label2
            // 
            label2.Dock = DockStyle.Left;
            label2.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold);
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(694, 39);
            label2.TabIndex = 0;
            label2.Text = "Vui Lòng Nhập Số Thẻ Để Mở Máy Lại :";
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold);
            textBox1.Location = new Point(694, 0);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(408, 39);
            textBox1.TabIndex = 1;
            // 
            // frmTMC7032_MsgAlarm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1102, 520);
            Controls.Add(lblMessage);
            Controls.Add(panel1);
            Controls.Add(button1);
            Name = "frmTMC7032_MsgAlarm";
            Text = "frmTMC7032_MsgAlarm";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblMessage;
        private Button button1;
        private Panel panel1;
        private Label label2;
        private TextBox textBox1;
    }
}