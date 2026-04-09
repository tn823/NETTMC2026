namespace NETTMC
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TreeNode treeNode1 = new TreeNode("Kiểm kho");
            TreeNode treeNode2 = new TreeNode("Export CheckStock");
            TreeNode treeNode3 = new TreeNode("Stock", new TreeNode[] { treeNode1, treeNode2 });
            TreeNode treeNode4 = new TreeNode("User Management");
            TreeNode treeNode5 = new TreeNode("Device Status");
            TreeNode treeNode6 = new TreeNode("Data Management");
            TreeNode treeNode7 = new TreeNode("FP && Card Reader Machine", new TreeNode[] { treeNode4, treeNode5, treeNode6 });
            TreeNode treeNode8 = new TreeNode("IP Management");
            TreeNode treeNode9 = new TreeNode("Print Label");
            TreeNode treeNode10 = new TreeNode("Check IP Scan Devices");
            TreeNode treeNode11 = new TreeNode("Device Manufacturer");
            TreeNode treeNode12 = new TreeNode("Model Management");
            TreeNode treeNode13 = new TreeNode("Devices Type Management");
            TreeNode treeNode14 = new TreeNode("Device Management");
            TreeNode treeNode15 = new TreeNode("IT Assets", new TreeNode[] { treeNode8, treeNode9, treeNode10, treeNode11, treeNode12, treeNode13, treeNode14 });
            TreeNode treeNode16 = new TreeNode("Check P-CARD");
            TreeNode treeNode17 = new TreeNode("Material Barcode");
            TreeNode treeNode18 = new TreeNode("Carton GWHS");
            TreeNode treeNode19 = new TreeNode("Modified P-CARD", new TreeNode[] { treeNode16, treeNode17, treeNode18 });
            TreeNode treeNode20 = new TreeNode("Version Management");
            TreeNode treeNode21 = new TreeNode("Program Version", new TreeNode[] { treeNode20 });
            TreeNode treeNode22 = new TreeNode("User Mangement");
            TreeNode treeNode23 = new TreeNode("User System", new TreeNode[] { treeNode22 });
            TreeNode treeNode24 = new TreeNode("SYSTEM", new TreeNode[] { treeNode3, treeNode7, treeNode15, treeNode19, treeNode21, treeNode23 });
            TreeNode treeNode25 = new TreeNode("HRMS");
            TreeNode treeNode26 = new TreeNode("SALARY");
            TreeNode treeNode27 = new TreeNode("HR", new TreeNode[] { treeNode25, treeNode26 });
            TreeNode treeNode28 = new TreeNode("DEV");
            TreeNode treeNode29 = new TreeNode("LAB");
            TreeNode treeNode30 = new TreeNode("MAT");
            TreeNode treeNode31 = new TreeNode("QA");
            TreeNode treeNode32 = new TreeNode("QIP");
            TreeNode treeNode33 = new TreeNode("RTDM", new TreeNode[] { treeNode28, treeNode29, treeNode30, treeNode31, treeNode32 });
            TreeNode treeNode34 = new TreeNode("LEAN NB");
            TreeNode treeNode35 = new TreeNode("LEAN AD");
            TreeNode treeNode36 = new TreeNode("LEAN", new TreeNode[] { treeNode34, treeNode35 });
            TreeNode treeNode37 = new TreeNode("STOCKFIT DEFECT");
            TreeNode treeNode38 = new TreeNode("Main Menu", new TreeNode[] { treeNode24, treeNode27, treeNode33, treeNode36, treeNode37 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainMenuStrip = new MenuStrip();
            menuHome = new ToolStripMenuItem();
            menuInsert = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            menuTools = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            ribbonStrip = new ToolStrip();
            btnNew = new ToolStripButton();
            btnOpen = new ToolStripButton();
            btnSave = new ToolStripButton();
            sepFile = new ToolStripSeparator();
            btnCut = new ToolStripButton();
            btnCopy = new ToolStripButton();
            btnPaste = new ToolStripButton();
            sepClipboard = new ToolStripSeparator();
            btnPrint = new ToolStripButton();
            btnFind = new ToolStripButton();
            btnSettings = new ToolStripButton();
            statusStrip = new StatusStrip();
            statusLabelReady = new ToolStripStatusLabel();
            statusLabelCount = new ToolStripStatusLabel();
            statusLabelVersion = new ToolStripStatusLabel();
            statusLabelConnection = new ToolStripStatusLabel();
            barIPaddress = new ToolStripStatusLabel();
            barUserID = new ToolStripStatusLabel();
            barBtnClose = new ToolStripStatusLabel();
            treeView1 = new TreeView();
            splitContainer1 = new SplitContainer();
            tabControl = new TabControl();
            timerConnectionCheck = new System.Windows.Forms.Timer(components);
            sTOCKToolStripMenuItem = new ToolStripMenuItem();
            kiểmKhoToolStripMenuItem = new ToolStripMenuItem();
            exportCheckStockToolStripMenuItem = new ToolStripMenuItem();
            fPCardReaderMachineToolStripMenuItem = new ToolStripMenuItem();
            iTAssetsToolStripMenuItem = new ToolStripMenuItem();
            modifiedPCARDToolStripMenuItem = new ToolStripMenuItem();
            programVersionToolStripMenuItem = new ToolStripMenuItem();
            userSystemToolStripMenuItem = new ToolStripMenuItem();
            userManagementToolStripMenuItem = new ToolStripMenuItem();
            deviceStatusToolStripMenuItem = new ToolStripMenuItem();
            dataManagementToolStripMenuItem = new ToolStripMenuItem();
            iPManagementToolStripMenuItem = new ToolStripMenuItem();
            printLabelToolStripMenuItem = new ToolStripMenuItem();
            checkIPScanDevicesToolStripMenuItem = new ToolStripMenuItem();
            deviceManufacturerToolStripMenuItem = new ToolStripMenuItem();
            modelManagementToolStripMenuItem = new ToolStripMenuItem();
            devicesTypeManagementToolStripMenuItem = new ToolStripMenuItem();
            deviceManagementToolStripMenuItem = new ToolStripMenuItem();
            checkPCARDToolStripMenuItem = new ToolStripMenuItem();
            materialBarcodeToolStripMenuItem = new ToolStripMenuItem();
            cartonGWHSToolStripMenuItem = new ToolStripMenuItem();
            versionManagementToolStripMenuItem = new ToolStripMenuItem();
            userManagementToolStripMenuItem1 = new ToolStripMenuItem();
            mainMenuStrip.SuspendLayout();
            ribbonStrip.SuspendLayout();
            statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenuStrip
            // 
            mainMenuStrip.Items.AddRange(new ToolStripItem[] { menuHome, menuInsert, menuView, menuTools, menuHelp, sTOCKToolStripMenuItem, fPCardReaderMachineToolStripMenuItem, iTAssetsToolStripMenuItem, modifiedPCARDToolStripMenuItem, programVersionToolStripMenuItem, userSystemToolStripMenuItem });
            mainMenuStrip.Location = new Point(0, 0);
            mainMenuStrip.Name = "mainMenuStrip";
            mainMenuStrip.Size = new Size(1257, 24);
            mainMenuStrip.TabIndex = 2;
            // 
            // menuHome
            // 
            menuHome.Name = "menuHome";
            menuHome.Size = new Size(92, 20);
            menuHome.Text = "Trang chủ (&H)";
            // 
            // menuInsert
            // 
            menuInsert.Name = "menuInsert";
            menuInsert.Size = new Size(61, 20);
            menuInsert.Text = "Chèn (&I)";
            // 
            // menuView
            // 
            menuView.Name = "menuView";
            menuView.Size = new Size(61, 20);
            menuView.Text = "Xem (&V)";
            // 
            // menuTools
            // 
            menuTools.Name = "menuTools";
            menuTools.Size = new Size(82, 20);
            menuTools.Text = "Công cụ (&T)";
            // 
            // menuHelp
            // 
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(81, 20);
            menuHelp.Text = "Trợ giúp (&P)";
            // 
            // ribbonStrip
            // 
            ribbonStrip.ImageScalingSize = new Size(20, 20);
            ribbonStrip.Items.AddRange(new ToolStripItem[] { btnNew, btnOpen, btnSave, sepFile, btnCut, btnCopy, btnPaste, sepClipboard, btnPrint, btnFind, btnSettings });
            ribbonStrip.Location = new Point(0, 24);
            ribbonStrip.Name = "ribbonStrip";
            ribbonStrip.Size = new Size(1257, 25);
            ribbonStrip.TabIndex = 1;
            // 
            // btnNew
            // 
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(23, 22);
            // 
            // btnOpen
            // 
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(23, 22);
            // 
            // btnSave
            // 
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(23, 22);
            // 
            // sepFile
            // 
            sepFile.Name = "sepFile";
            sepFile.Size = new Size(6, 25);
            // 
            // btnCut
            // 
            btnCut.Name = "btnCut";
            btnCut.Size = new Size(23, 22);
            // 
            // btnCopy
            // 
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(23, 22);
            // 
            // btnPaste
            // 
            btnPaste.Name = "btnPaste";
            btnPaste.Size = new Size(23, 22);
            // 
            // sepClipboard
            // 
            sepClipboard.Name = "sepClipboard";
            sepClipboard.Size = new Size(6, 25);
            // 
            // btnPrint
            // 
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(23, 22);
            // 
            // btnFind
            // 
            btnFind.Name = "btnFind";
            btnFind.Size = new Size(23, 22);
            // 
            // btnSettings
            // 
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new Size(23, 22);
            // 
            // statusStrip
            // 
            statusStrip.BackColor = Color.FromArgb(25, 118, 210);
            statusStrip.ForeColor = Color.White;
            statusStrip.Items.AddRange(new ToolStripItem[] { statusLabelReady, statusLabelCount, statusLabelVersion, statusLabelConnection, barIPaddress, barUserID, barBtnClose });
            statusStrip.Location = new Point(0, 716);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1257, 24);
            statusStrip.TabIndex = 3;
            // 
            // statusLabelReady
            // 
            statusLabelReady.BorderSides = ToolStripStatusLabelBorderSides.Right;
            statusLabelReady.Name = "statusLabelReady";
            statusLabelReady.Size = new Size(58, 19);
            statusLabelReady.Text = "Sẵn sàng";
            // 
            // statusLabelCount
            // 
            statusLabelCount.BorderSides = ToolStripStatusLabelBorderSides.Right;
            statusLabelCount.Name = "statusLabelCount";
            statusLabelCount.Size = new Size(44, 19);
            statusLabelCount.Text = "0 mục";
            // 
            // statusLabelVersion
            // 
            statusLabelVersion.Name = "statusLabelVersion";
            statusLabelVersion.Size = new Size(829, 19);
            statusLabelVersion.Spring = true;
            statusLabelVersion.Text = "v1.0.0";
            statusLabelVersion.TextAlign = ContentAlignment.MiddleRight;
            // 
            // statusLabelConnection
            // 
            statusLabelConnection.ForeColor = Color.FromArgb(255, 255, 128);
            statusLabelConnection.Name = "statusLabelConnection";
            statusLabelConnection.Size = new Size(110, 19);
            statusLabelConnection.Text = "\"● Đang kiểm tra...\"";
            // 
            // barIPaddress
            // 
            barIPaddress.Name = "barIPaddress";
            barIPaddress.Size = new Size(62, 19);
            barIPaddress.Text = "192.168.x.x";
            // 
            // barUserID
            // 
            barUserID.Name = "barUserID";
            barUserID.Size = new Size(87, 19);
            barUserID.Text = "User ID : admin";
            // 
            // barBtnClose
            // 
            barBtnClose.Image = Properties.Resources.Close;
            barBtnClose.Name = "barBtnClose";
            barBtnClose.Size = new Size(52, 19);
            barBtnClose.Text = "Close";
            barBtnClose.Click += barBtnClose_Click;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeNode1.Name = "Node21";
            treeNode1.Text = "Kiểm kho";
            treeNode2.Name = "Node22";
            treeNode2.Text = "Export CheckStock";
            treeNode3.Name = "Node15";
            treeNode3.Text = "Stock";
            treeNode4.Name = "Node23";
            treeNode4.Text = "User Management";
            treeNode5.Name = "Node24";
            treeNode5.Text = "Device Status";
            treeNode6.Name = "Node25";
            treeNode6.Text = "Data Management";
            treeNode7.Name = "Node16";
            treeNode7.Text = "FP && Card Reader Machine";
            treeNode8.Name = "Node26";
            treeNode8.Text = "IP Management";
            treeNode9.Name = "Node27";
            treeNode9.Text = "Print Label";
            treeNode10.Name = "Node28";
            treeNode10.Text = "Check IP Scan Devices";
            treeNode11.Name = "Node29";
            treeNode11.Text = "Device Manufacturer";
            treeNode12.Name = "Node30";
            treeNode12.Text = "Model Management";
            treeNode13.Name = "Node31";
            treeNode13.Text = "Devices Type Management";
            treeNode14.Name = "Node32";
            treeNode14.Text = "Device Management";
            treeNode15.Name = "Node17";
            treeNode15.Text = "IT Assets";
            treeNode16.Name = "Node33";
            treeNode16.Text = "Check P-CARD";
            treeNode17.Name = "Node34";
            treeNode17.Text = "Material Barcode";
            treeNode18.Name = "Node35";
            treeNode18.Text = "Carton GWHS";
            treeNode19.Name = "Node18";
            treeNode19.Text = "Modified P-CARD";
            treeNode20.Name = "Node36";
            treeNode20.Text = "Version Management";
            treeNode21.Name = "Node19";
            treeNode21.Text = "Program Version";
            treeNode22.Name = "Node37";
            treeNode22.Text = "User Mangement";
            treeNode23.Name = "Node20";
            treeNode23.Text = "User System";
            treeNode24.Name = "Node11";
            treeNode24.Text = "SYSTEM";
            treeNode25.Name = "Node9";
            treeNode25.Text = "HRMS";
            treeNode26.Name = "Node10";
            treeNode26.Text = "SALARY";
            treeNode27.Name = "Node12";
            treeNode27.Text = "HR";
            treeNode28.Name = "Node4";
            treeNode28.Text = "DEV";
            treeNode29.Name = "Node2";
            treeNode29.Text = "LAB";
            treeNode30.Name = "Node5";
            treeNode30.Text = "MAT";
            treeNode31.Name = "Node6";
            treeNode31.Text = "QA";
            treeNode32.Name = "Node1";
            treeNode32.Text = "QIP";
            treeNode33.Name = "Node13";
            treeNode33.Text = "RTDM";
            treeNode34.Name = "Node7";
            treeNode34.Text = "LEAN NB";
            treeNode35.Name = "Node8";
            treeNode35.Text = "LEAN AD";
            treeNode36.Name = "Node14";
            treeNode36.Text = "LEAN";
            treeNode37.Name = "Node3";
            treeNode37.Text = "STOCKFIT DEFECT";
            treeNode38.Name = "Node0";
            treeNode38.Text = "Main Menu";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode38 });
            treeView1.Size = new Size(183, 667);
            treeView1.TabIndex = 4;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(treeView1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(tabControl);
            splitContainer1.Size = new Size(1257, 667);
            splitContainer1.SplitterDistance = 183;
            splitContainer1.TabIndex = 5;
            // 
            // tabControl
            // 
            tabControl.Dock = DockStyle.Fill;
            tabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabControl.Location = new Point(0, 0);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(1070, 667);
            tabControl.TabIndex = 0;
            tabControl.DrawItem += mainTabControl_DrawItem;
            tabControl.SelectedIndexChanged += mainTabControl_SelectedIndexChanged;
            tabControl.MouseDown += mainTabControl_MouseDown;
            // 
            // sTOCKToolStripMenuItem
            // 
            sTOCKToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { kiểmKhoToolStripMenuItem, exportCheckStockToolStripMenuItem });
            sTOCKToolStripMenuItem.Name = "sTOCKToolStripMenuItem";
            sTOCKToolStripMenuItem.Size = new Size(55, 20);
            sTOCKToolStripMenuItem.Text = "STOCK";
            // 
            // kiểmKhoToolStripMenuItem
            // 
            kiểmKhoToolStripMenuItem.Name = "kiểmKhoToolStripMenuItem";
            kiểmKhoToolStripMenuItem.Size = new Size(180, 22);
            kiểmKhoToolStripMenuItem.Text = "Kiểm Kho";
            // 
            // exportCheckStockToolStripMenuItem
            // 
            exportCheckStockToolStripMenuItem.Name = "exportCheckStockToolStripMenuItem";
            exportCheckStockToolStripMenuItem.Size = new Size(180, 22);
            exportCheckStockToolStripMenuItem.Text = "Export CheckStock";
            // 
            // fPCardReaderMachineToolStripMenuItem
            // 
            fPCardReaderMachineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { userManagementToolStripMenuItem, deviceStatusToolStripMenuItem, dataManagementToolStripMenuItem });
            fPCardReaderMachineToolStripMenuItem.Name = "fPCardReaderMachineToolStripMenuItem";
            fPCardReaderMachineToolStripMenuItem.Size = new Size(161, 20);
            fPCardReaderMachineToolStripMenuItem.Text = "FP && Card Reader Machine";
            // 
            // iTAssetsToolStripMenuItem
            // 
            iTAssetsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { iPManagementToolStripMenuItem, printLabelToolStripMenuItem, checkIPScanDevicesToolStripMenuItem, deviceManufacturerToolStripMenuItem, modelManagementToolStripMenuItem, devicesTypeManagementToolStripMenuItem, deviceManagementToolStripMenuItem });
            iTAssetsToolStripMenuItem.Name = "iTAssetsToolStripMenuItem";
            iTAssetsToolStripMenuItem.Size = new Size(65, 20);
            iTAssetsToolStripMenuItem.Text = "IT Assets";
            // 
            // modifiedPCARDToolStripMenuItem
            // 
            modifiedPCARDToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkPCARDToolStripMenuItem, materialBarcodeToolStripMenuItem, cartonGWHSToolStripMenuItem });
            modifiedPCARDToolStripMenuItem.Name = "modifiedPCARDToolStripMenuItem";
            modifiedPCARDToolStripMenuItem.Size = new Size(113, 20);
            modifiedPCARDToolStripMenuItem.Text = "Modified P-CARD";
            // 
            // programVersionToolStripMenuItem
            // 
            programVersionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { versionManagementToolStripMenuItem });
            programVersionToolStripMenuItem.Name = "programVersionToolStripMenuItem";
            programVersionToolStripMenuItem.Size = new Size(106, 20);
            programVersionToolStripMenuItem.Text = "Program Version";
            // 
            // userSystemToolStripMenuItem
            // 
            userSystemToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { userManagementToolStripMenuItem1 });
            userSystemToolStripMenuItem.Name = "userSystemToolStripMenuItem";
            userSystemToolStripMenuItem.Size = new Size(83, 20);
            userSystemToolStripMenuItem.Text = "User System";
            // 
            // userManagementToolStripMenuItem
            // 
            userManagementToolStripMenuItem.Name = "userManagementToolStripMenuItem";
            userManagementToolStripMenuItem.Size = new Size(180, 22);
            userManagementToolStripMenuItem.Text = "User Management";
            // 
            // deviceStatusToolStripMenuItem
            // 
            deviceStatusToolStripMenuItem.Name = "deviceStatusToolStripMenuItem";
            deviceStatusToolStripMenuItem.Size = new Size(180, 22);
            deviceStatusToolStripMenuItem.Text = "Device Status";
            // 
            // dataManagementToolStripMenuItem
            // 
            dataManagementToolStripMenuItem.Name = "dataManagementToolStripMenuItem";
            dataManagementToolStripMenuItem.Size = new Size(180, 22);
            dataManagementToolStripMenuItem.Text = "Data Management";
            // 
            // iPManagementToolStripMenuItem
            // 
            iPManagementToolStripMenuItem.Name = "iPManagementToolStripMenuItem";
            iPManagementToolStripMenuItem.Size = new Size(216, 22);
            iPManagementToolStripMenuItem.Text = "IP Management";
            // 
            // printLabelToolStripMenuItem
            // 
            printLabelToolStripMenuItem.Name = "printLabelToolStripMenuItem";
            printLabelToolStripMenuItem.Size = new Size(216, 22);
            printLabelToolStripMenuItem.Text = "Print Label";
            // 
            // checkIPScanDevicesToolStripMenuItem
            // 
            checkIPScanDevicesToolStripMenuItem.Name = "checkIPScanDevicesToolStripMenuItem";
            checkIPScanDevicesToolStripMenuItem.Size = new Size(216, 22);
            checkIPScanDevicesToolStripMenuItem.Text = "Check IP Scan Devices";
            // 
            // deviceManufacturerToolStripMenuItem
            // 
            deviceManufacturerToolStripMenuItem.Name = "deviceManufacturerToolStripMenuItem";
            deviceManufacturerToolStripMenuItem.Size = new Size(216, 22);
            deviceManufacturerToolStripMenuItem.Text = "Device Manufacturer";
            // 
            // modelManagementToolStripMenuItem
            // 
            modelManagementToolStripMenuItem.Name = "modelManagementToolStripMenuItem";
            modelManagementToolStripMenuItem.Size = new Size(216, 22);
            modelManagementToolStripMenuItem.Text = "Model Management";
            // 
            // devicesTypeManagementToolStripMenuItem
            // 
            devicesTypeManagementToolStripMenuItem.Name = "devicesTypeManagementToolStripMenuItem";
            devicesTypeManagementToolStripMenuItem.Size = new Size(216, 22);
            devicesTypeManagementToolStripMenuItem.Text = "Devices Type Management";
            // 
            // deviceManagementToolStripMenuItem
            // 
            deviceManagementToolStripMenuItem.Name = "deviceManagementToolStripMenuItem";
            deviceManagementToolStripMenuItem.Size = new Size(216, 22);
            deviceManagementToolStripMenuItem.Text = "Device Management";
            // 
            // checkPCARDToolStripMenuItem
            // 
            checkPCARDToolStripMenuItem.Name = "checkPCARDToolStripMenuItem";
            checkPCARDToolStripMenuItem.Size = new Size(180, 22);
            checkPCARDToolStripMenuItem.Text = "Check P-CARD";
            // 
            // materialBarcodeToolStripMenuItem
            // 
            materialBarcodeToolStripMenuItem.Name = "materialBarcodeToolStripMenuItem";
            materialBarcodeToolStripMenuItem.Size = new Size(180, 22);
            materialBarcodeToolStripMenuItem.Text = "Material Barcode";
            // 
            // cartonGWHSToolStripMenuItem
            // 
            cartonGWHSToolStripMenuItem.Name = "cartonGWHSToolStripMenuItem";
            cartonGWHSToolStripMenuItem.Size = new Size(180, 22);
            cartonGWHSToolStripMenuItem.Text = "Carton GWHS";
            // 
            // versionManagementToolStripMenuItem
            // 
            versionManagementToolStripMenuItem.Name = "versionManagementToolStripMenuItem";
            versionManagementToolStripMenuItem.Size = new Size(186, 22);
            versionManagementToolStripMenuItem.Text = "Version Management";
            // 
            // userManagementToolStripMenuItem1
            // 
            userManagementToolStripMenuItem1.Name = "userManagementToolStripMenuItem1";
            userManagementToolStripMenuItem1.Size = new Size(180, 22);
            userManagementToolStripMenuItem1.Text = "User Management";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1257, 740);
            Controls.Add(splitContainer1);
            Controls.Add(ribbonStrip);
            Controls.Add(mainMenuStrip);
            Controls.Add(statusStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = mainMenuStrip;
            MinimumSize = new Size(800, 550);
            Name = "MainForm";
            Text = "NETTMC";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            mainMenuStrip.ResumeLayout(false);
            mainMenuStrip.PerformLayout();
            ribbonStrip.ResumeLayout(false);
            ribbonStrip.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        // Helper để set thuộc tính ToolStripButton nhanh
        private static void SetupToolStripButton(
            System.Windows.Forms.ToolStripButton btn,
            string name, string text, string tooltip)
        {
            btn.Name = name;
            btn.Text = text;
            btn.ToolTipText = tooltip;
            btn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            // Gán Image từ ImageList hoặc resource:
            // btn.Image = Properties.Resources.icon_new;
        }

        #endregion

        // ── Field declarations ────────────────────────────────────────
        private System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuHome;
        private System.Windows.Forms.ToolStripMenuItem menuInsert;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;

        private System.Windows.Forms.ToolStrip ribbonStrip;
        private System.Windows.Forms.ToolStripButton btnNew;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator sepFile;
        private System.Windows.Forms.ToolStripButton btnCut;
        private System.Windows.Forms.ToolStripButton btnCopy;
        private System.Windows.Forms.ToolStripButton btnPaste;
        private System.Windows.Forms.ToolStripSeparator sepClipboard;
        private System.Windows.Forms.ToolStripButton btnPrint;
        private System.Windows.Forms.ToolStripButton btnFind;
        private System.Windows.Forms.ToolStripButton btnSettings;

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelReady;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelCount;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelVersion;
        private TreeView treeView1;
        private SplitContainer splitContainer1;
        private TabControl tabControl;
        private ToolStripStatusLabel statusLabelConnection;
        private System.Windows.Forms.Timer timerConnectionCheck;
        private ToolStripStatusLabel barIPaddress;
        private ToolStripStatusLabel barUserID;
        private ToolStripStatusLabel barBtnClose;
        private ToolStripMenuItem sTOCKToolStripMenuItem;
        private ToolStripMenuItem kiểmKhoToolStripMenuItem;
        private ToolStripMenuItem exportCheckStockToolStripMenuItem;
        private ToolStripMenuItem fPCardReaderMachineToolStripMenuItem;
        private ToolStripMenuItem userManagementToolStripMenuItem;
        private ToolStripMenuItem deviceStatusToolStripMenuItem;
        private ToolStripMenuItem dataManagementToolStripMenuItem;
        private ToolStripMenuItem iTAssetsToolStripMenuItem;
        private ToolStripMenuItem iPManagementToolStripMenuItem;
        private ToolStripMenuItem printLabelToolStripMenuItem;
        private ToolStripMenuItem checkIPScanDevicesToolStripMenuItem;
        private ToolStripMenuItem deviceManufacturerToolStripMenuItem;
        private ToolStripMenuItem modelManagementToolStripMenuItem;
        private ToolStripMenuItem devicesTypeManagementToolStripMenuItem;
        private ToolStripMenuItem modifiedPCARDToolStripMenuItem;
        private ToolStripMenuItem programVersionToolStripMenuItem;
        private ToolStripMenuItem userSystemToolStripMenuItem;
        private ToolStripMenuItem deviceManagementToolStripMenuItem;
        private ToolStripMenuItem checkPCARDToolStripMenuItem;
        private ToolStripMenuItem materialBarcodeToolStripMenuItem;
        private ToolStripMenuItem cartonGWHSToolStripMenuItem;
        private ToolStripMenuItem versionManagementToolStripMenuItem;
        private ToolStripMenuItem userManagementToolStripMenuItem1;
    }
}