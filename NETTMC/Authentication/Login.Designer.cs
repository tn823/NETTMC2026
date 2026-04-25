namespace NETTMC.Authentication
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            labelControl3 = new Label();
            label2 = new Label();
            panelControl2 = new FlowLayoutPanel();
            btnMas = new Button();
            btnSecurity = new Button();
            barBtnGatePassCheck = new Button();
            barBtnForITOnly = new Button();
            btnDefect = new Button();
            btnDefectAdidas = new Button();
            btnJSI = new Button();
            btnCCQP = new Button();
            btnP2PAllLineNB = new Button();
            btnP2P = new Button();
            btnCanteen = new Button();
            btnDefectImage = new Button();
            tableLayoutLogin = new TableLayoutPanel();
            txtUser = new TextBox();
            txtPassword = new TextBox();
            panelControl1 = new Panel();
            label1 = new Label();
            btnCancel = new Button();
            btnLogin = new Button();
            checkBoxSite = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            labelControl5 = new Label();
            button1 = new Button();
            progressPanel1 = new ProgressBar();
            simpleButton3 = new Button();
            btnSystem = new Button();
            simpleButton5 = new Button();
            panelControl2.SuspendLayout();
            tableLayoutLogin.SuspendLayout();
            panelControl1.SuspendLayout();
            SuspendLayout();
            // 
            // labelControl3
            // 
            labelControl3.Dock = DockStyle.Top;
            labelControl3.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            labelControl3.Location = new Point(0, 0);
            labelControl3.Name = "labelControl3";
            labelControl3.Size = new Size(710, 48);
            labelControl3.TabIndex = 0;
            labelControl3.Text = "Welcome to NetTMC software.";
            labelControl3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 7.8F, FontStyle.Italic);
            label2.Location = new Point(227, 58);
            label2.Name = "label2";
            label2.Size = new Size(241, 13);
            label2.TabIndex = 1;
            label2.Text = "Please select function that you want to access in";
            label2.TextAlign = ContentAlignment.TopCenter;
            // 
            // panelControl2
            // 
            panelControl2.BorderStyle = BorderStyle.FixedSingle;
            panelControl2.Controls.Add(btnMas);
            panelControl2.Controls.Add(btnSecurity);
            panelControl2.Controls.Add(barBtnGatePassCheck);
            panelControl2.Controls.Add(barBtnForITOnly);
            panelControl2.Controls.Add(btnDefect);
            panelControl2.Controls.Add(btnDefectAdidas);
            panelControl2.Controls.Add(btnJSI);
            panelControl2.Controls.Add(btnCCQP);
            panelControl2.Controls.Add(btnP2PAllLineNB);
            panelControl2.Controls.Add(btnP2P);
            panelControl2.Controls.Add(btnCanteen);
            panelControl2.Controls.Add(btnDefectImage);
            panelControl2.Location = new Point(46, 87);
            panelControl2.Name = "panelControl2";
            panelControl2.Size = new Size(620, 287);
            panelControl2.TabIndex = 2;
            // 
            // btnMas
            // 
            btnMas.Font = new Font("Tahoma", 8.25F);
            btnMas.Image = Properties.Resources.mas;
            btnMas.Location = new Point(3, 3);
            btnMas.Name = "btnMas";
            btnMas.Size = new Size(146, 82);
            btnMas.TabIndex = 0;
            btnMas.Text = "MAS";
            btnMas.TextImageRelation = TextImageRelation.ImageAboveText;
            btnMas.UseVisualStyleBackColor = true;
            btnMas.Click += button1_Click;
            // 
            // btnSecurity
            // 
            btnSecurity.Font = new Font("Tahoma", 8.25F);
            btnSecurity.Image = Properties.Resources.cuuho;
            btnSecurity.Location = new Point(155, 3);
            btnSecurity.Name = "btnSecurity";
            btnSecurity.Size = new Size(146, 82);
            btnSecurity.TabIndex = 1;
            btnSecurity.Text = "Security Registration Đăng ký bảo vệ";
            btnSecurity.TextImageRelation = TextImageRelation.ImageAboveText;
            btnSecurity.UseVisualStyleBackColor = true;
            // 
            // barBtnGatePassCheck
            // 
            barBtnGatePassCheck.Font = new Font("Tahoma", 8.25F);
            barBtnGatePassCheck.Image = Properties.Resources.gatepass;
            barBtnGatePassCheck.Location = new Point(307, 3);
            barBtnGatePassCheck.Name = "barBtnGatePassCheck";
            barBtnGatePassCheck.Size = new Size(146, 82);
            barBtnGatePassCheck.TabIndex = 2;
            barBtnGatePassCheck.Text = "GatePass Check";
            barBtnGatePassCheck.TextImageRelation = TextImageRelation.ImageAboveText;
            barBtnGatePassCheck.UseVisualStyleBackColor = true;
            // 
            // barBtnForITOnly
            // 
            barBtnForITOnly.Font = new Font("Tahoma", 8.25F);
            barBtnForITOnly.Image = Properties.Resources.vetinh;
            barBtnForITOnly.Location = new Point(459, 3);
            barBtnForITOnly.Name = "barBtnForITOnly";
            barBtnForITOnly.Size = new Size(146, 82);
            barBtnForITOnly.TabIndex = 3;
            barBtnForITOnly.Text = "Automated For IT";
            barBtnForITOnly.TextImageRelation = TextImageRelation.ImageAboveText;
            barBtnForITOnly.UseVisualStyleBackColor = true;
            // 
            // btnDefect
            // 
            btnDefect.Font = new Font("Tahoma", 8.25F);
            btnDefect.Image = Properties.Resources.hand_18406955;
            btnDefect.Location = new Point(3, 91);
            btnDefect.Name = "btnDefect";
            btnDefect.Size = new Size(146, 82);
            btnDefect.TabIndex = 4;
            btnDefect.Text = "DEFECT Chấm lỗi cuối chuyền";
            btnDefect.TextImageRelation = TextImageRelation.ImageAboveText;
            btnDefect.UseVisualStyleBackColor = true;
            btnDefect.Click += btnDefect_Click;
            // 
            // btnDefectAdidas
            // 
            btnDefectAdidas.Font = new Font("Tahoma", 8.25F);
            btnDefectAdidas.Image = Properties.Resources.hand_18406955;
            btnDefectAdidas.Location = new Point(155, 91);
            btnDefectAdidas.Name = "btnDefectAdidas";
            btnDefectAdidas.Size = new Size(146, 82);
            btnDefectAdidas.TabIndex = 5;
            btnDefectAdidas.Text = "DEFECT Stockfit";
            btnDefectAdidas.TextImageRelation = TextImageRelation.ImageAboveText;
            btnDefectAdidas.UseVisualStyleBackColor = true;
            btnDefectAdidas.Click += btnDefectAdidas_Click;
            // 
            // btnJSI
            // 
            btnJSI.Font = new Font("Tahoma", 8.25F);
            btnJSI.Image = Properties.Resources.hand_18406955;
            btnJSI.Location = new Point(307, 91);
            btnJSI.Name = "btnJSI";
            btnJSI.Size = new Size(146, 82);
            btnJSI.TabIndex = 6;
            btnJSI.Text = "DEFECT JSI ( NB only )";
            btnJSI.TextImageRelation = TextImageRelation.ImageAboveText;
            btnJSI.UseVisualStyleBackColor = true;
            // 
            // btnCCQP
            // 
            btnCCQP.Font = new Font("Tahoma", 8.25F);
            btnCCQP.Image = Properties.Resources.hand_18406955;
            btnCCQP.Location = new Point(459, 91);
            btnCCQP.Name = "btnCCQP";
            btnCCQP.Size = new Size(146, 82);
            btnCCQP.TabIndex = 7;
            btnCCQP.Text = "CCQP";
            btnCCQP.TextImageRelation = TextImageRelation.ImageAboveText;
            btnCCQP.UseVisualStyleBackColor = true;
            // 
            // btnP2PAllLineNB
            // 
            btnP2PAllLineNB.Font = new Font("Tahoma", 8.25F);
            btnP2PAllLineNB.Image = Properties.Resources.P2P;
            btnP2PAllLineNB.Location = new Point(3, 179);
            btnP2PAllLineNB.Name = "btnP2PAllLineNB";
            btnP2PAllLineNB.Size = new Size(146, 82);
            btnP2PAllLineNB.TabIndex = 8;
            btnP2PAllLineNB.Text = "P2P All Line NB";
            btnP2PAllLineNB.TextImageRelation = TextImageRelation.ImageAboveText;
            btnP2PAllLineNB.UseVisualStyleBackColor = true;
            // 
            // btnP2P
            // 
            btnP2P.Font = new Font("Tahoma", 8.25F);
            btnP2P.Image = Properties.Resources.tivi_cuoichuyen;
            btnP2P.Location = new Point(155, 179);
            btnP2P.Name = "btnP2P";
            btnP2P.Size = new Size(146, 82);
            btnP2P.TabIndex = 9;
            btnP2P.Text = "P2P Tivi Cuối chuyền";
            btnP2P.TextImageRelation = TextImageRelation.ImageAboveText;
            btnP2P.UseVisualStyleBackColor = true;
            // 
            // btnCanteen
            // 
            btnCanteen.Font = new Font("Tahoma", 8.25F);
            btnCanteen.Image = Properties.Resources.monitor;
            btnCanteen.Location = new Point(307, 179);
            btnCanteen.Name = "btnCanteen";
            btnCanteen.Size = new Size(146, 82);
            btnCanteen.TabIndex = 10;
            btnCanteen.Text = "Monitor Temp && Hum";
            btnCanteen.TextImageRelation = TextImageRelation.ImageAboveText;
            btnCanteen.UseVisualStyleBackColor = true;
            // 
            // btnDefectImage
            // 
            btnDefectImage.Font = new Font("Tahoma", 8.25F);
            btnDefectImage.Image = Properties.Resources.image;
            btnDefectImage.Location = new Point(459, 179);
            btnDefectImage.Name = "btnDefectImage";
            btnDefectImage.Size = new Size(146, 82);
            btnDefectImage.TabIndex = 11;
            btnDefectImage.Text = "Defect Images";
            btnDefectImage.TextImageRelation = TextImageRelation.ImageAboveText;
            btnDefectImage.UseVisualStyleBackColor = true;
            // 
            // tableLayoutLogin
            // 
            tableLayoutLogin.Anchor = AnchorStyles.Right;
            tableLayoutLogin.ColumnCount = 2;
            tableLayoutLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 31.7415733F));
            tableLayoutLogin.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 68.25843F));
            tableLayoutLogin.Controls.Add(txtUser, 1, 0);
            tableLayoutLogin.Controls.Add(txtPassword, 1, 1);
            tableLayoutLogin.Controls.Add(panelControl1, 0, 2);
            tableLayoutLogin.Controls.Add(label4, 0, 1);
            tableLayoutLogin.Controls.Add(label3, 0, 0);
            tableLayoutLogin.Location = new Point(46, 392);
            tableLayoutLogin.Margin = new Padding(4, 2, 4, 2);
            tableLayoutLogin.Name = "tableLayoutLogin";
            tableLayoutLogin.RowCount = 3;
            tableLayoutLogin.RowStyles.Add(new RowStyle(SizeType.Percent, 44.8717957F));
            tableLayoutLogin.RowStyles.Add(new RowStyle(SizeType.Percent, 55.1282043F));
            tableLayoutLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 51F));
            tableLayoutLogin.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutLogin.Size = new Size(356, 130);
            tableLayoutLogin.TabIndex = 0;
            tableLayoutLogin.Paint += tableLayoutLogin_Paint;
            // 
            // txtUser
            // 
            txtUser.Location = new Point(116, 3);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(187, 20);
            txtUser.TabIndex = 2;
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(116, 38);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '*';
            txtPassword.Size = new Size(190, 20);
            txtPassword.TabIndex = 3;
            // 
            // panelControl1
            // 
            tableLayoutLogin.SetColumnSpan(panelControl1, 2);
            panelControl1.Controls.Add(label1);
            panelControl1.Controls.Add(btnCancel);
            panelControl1.Controls.Add(btnLogin);
            panelControl1.Controls.Add(checkBoxSite);
            panelControl1.Location = new Point(3, 81);
            panelControl1.Name = "panelControl1";
            panelControl1.Size = new Size(350, 46);
            panelControl1.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(7, 12);
            label1.Name = "label1";
            label1.Size = new Size(29, 13);
            label1.TabIndex = 4;
            label1.Text = "Site:";
            // 
            // btnCancel
            // 
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(238, 7);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(94, 29);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnLogin
            // 
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Location = new Point(134, 7);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(89, 29);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            // 
            // checkBoxSite
            // 
            checkBoxSite.AutoSize = true;
            checkBoxSite.Location = new Point(44, 12);
            checkBoxSite.Name = "checkBoxSite";
            checkBoxSite.Size = new Size(69, 17);
            checkBoxSite.TabIndex = 1;
            checkBoxSite.Text = "An Giang";
            checkBoxSite.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 0);
            label3.Name = "label3";
            label3.Size = new Size(47, 13);
            label3.TabIndex = 0;
            label3.Text = "User ID:";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(3, 35);
            label4.Name = "label4";
            label4.Size = new Size(57, 13);
            label4.TabIndex = 1;
            label4.Text = "Password:";
            // 
            // labelControl5
            // 
            labelControl5.AutoSize = true;
            labelControl5.Font = new Font("Tahoma", 7.8F, FontStyle.Italic);
            labelControl5.ForeColor = Color.Navy;
            labelControl5.Location = new Point(542, 58);
            labelControl5.Name = "labelControl5";
            labelControl5.Size = new Size(137, 13);
            labelControl5.TabIndex = 3;
            labelControl5.Text = "Try to connect database...";
            // 
            // button1
            // 
            button1.Location = new Point(435, 431);
            button1.Name = "button1";
            button1.Size = new Size(75, 68);
            button1.TabIndex = 4;
            button1.Text = "Test Insert Pass, Fail";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // progressPanel1
            // 
            progressPanel1.BackColor = Color.FromArgb(0, 192, 0);
            progressPanel1.Location = new Point(555, 42);
            progressPanel1.MarqueeAnimationSpeed = 30;
            progressPanel1.Name = "progressPanel1";
            progressPanel1.Size = new Size(100, 13);
            progressPanel1.Style = ProgressBarStyle.Marquee;
            progressPanel1.TabIndex = 5;
            // 
            // simpleButton3
            // 
            simpleButton3.Location = new Point(555, 363);
            simpleButton3.Name = "simpleButton3";
            simpleButton3.Size = new Size(111, 27);
            simpleButton3.TabIndex = 12;
            simpleButton3.Text = "Dashboard Design";
            simpleButton3.UseVisualStyleBackColor = true;
            simpleButton3.Visible = false;
            // 
            // btnSystem
            // 
            btnSystem.BackColor = SystemColors.ScrollBar;
            btnSystem.Font = new Font("Tahoma", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnSystem.ForeColor = Color.White;
            btnSystem.Image = (Image)resources.GetObject("btnSystem.Image");
            btnSystem.Location = new Point(526, 396);
            btnSystem.Name = "btnSystem";
            btnSystem.Size = new Size(125, 92);
            btnSystem.TabIndex = 13;
            btnSystem.Text = "For Office";
            btnSystem.TextAlign = ContentAlignment.BottomCenter;
            btnSystem.UseVisualStyleBackColor = false;
            btnSystem.Click += btnSystem_Click;
            // 
            // simpleButton5
            // 
            simpleButton5.Font = new Font("Tahoma", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            simpleButton5.Location = new Point(526, 489);
            simpleButton5.Name = "simpleButton5";
            simpleButton5.Size = new Size(125, 37);
            simpleButton5.TabIndex = 14;
            simpleButton5.Text = "In Tem PCard";
            simpleButton5.UseVisualStyleBackColor = true;
            simpleButton5.Click += simpleButton5_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(6F, 12F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonFace;
            BackgroundImageLayout = ImageLayout.None;
            ClientSize = new Size(710, 533);
            Controls.Add(simpleButton5);
            Controls.Add(btnSystem);
            Controls.Add(simpleButton3);
            Controls.Add(progressPanel1);
            Controls.Add(button1);
            Controls.Add(labelControl5);
            Controls.Add(tableLayoutLogin);
            Controls.Add(panelControl2);
            Controls.Add(label2);
            Controls.Add(labelControl3);
            Font = new Font("Tahoma", 7.8F);
            ForeColor = Color.FromArgb(72, 70, 68);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Login";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Login - NetTMC";
            Load += Login_Load;
            panelControl2.ResumeLayout(false);
            tableLayoutLogin.ResumeLayout(false);
            tableLayoutLogin.PerformLayout();
            panelControl1.ResumeLayout(false);
            panelControl1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label labelControl3;
        private Label label2;
        private FlowLayoutPanel panelControl2;
        private TableLayoutPanel tableLayoutLogin;
        private Label label3;
        private Label label4;
        private TextBox txtUser;
        private TextBox txtPassword;
        private Panel panelControl1;
        private Button btnCancel;
        private Button btnLogin;
        private CheckBox checkBoxSite;
        private Button btnMas;
        private Button btnSecurity;
        private Button barBtnGatePassCheck;
        private Button barBtnForITOnly;
        private Button btnDefect;
        private Button btnDefectAdidas;
        private Button btnJSI;
        private Button btnCCQP;
        private Button btnP2PAllLineNB;
        private Button btnP2P;
        private Button btnCanteen;
        private Button btnDefectImage;
        private Label label1;
        private Label labelControl5;
        private Button button1;
        private ProgressBar progressPanel1;
        private Button simpleButton3;
        private Button btnSystem;
        private Button simpleButton5;
    }
}