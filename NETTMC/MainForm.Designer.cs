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
            TreeNode treeNode25 = new TreeNode("Node50");
            TreeNode treeNode26 = new TreeNode("Node51");
            TreeNode treeNode27 = new TreeNode("Attendace", new TreeNode[] { treeNode25, treeNode26 });
            TreeNode treeNode28 = new TreeNode("Node52");
            TreeNode treeNode29 = new TreeNode("Node53");
            TreeNode treeNode30 = new TreeNode("Canteen", new TreeNode[] { treeNode28, treeNode29 });
            TreeNode treeNode31 = new TreeNode("Dashboard");
            TreeNode treeNode32 = new TreeNode("Node54");
            TreeNode treeNode33 = new TreeNode("Node55");
            TreeNode treeNode34 = new TreeNode("Security", new TreeNode[] { treeNode32, treeNode33 });
            TreeNode treeNode35 = new TreeNode("Node56");
            TreeNode treeNode36 = new TreeNode("Node57");
            TreeNode treeNode37 = new TreeNode("Node58");
            TreeNode treeNode38 = new TreeNode("MAS", new TreeNode[] { treeNode35, treeNode36, treeNode37 });
            TreeNode treeNode39 = new TreeNode("Org Chart");
            TreeNode treeNode40 = new TreeNode("HRMS", new TreeNode[] { treeNode27, treeNode30, treeNode31, treeNode34, treeNode38, treeNode39 });
            TreeNode treeNode41 = new TreeNode("Upload Excel");
            TreeNode treeNode42 = new TreeNode("Upload Salary", new TreeNode[] { treeNode41 });
            TreeNode treeNode43 = new TreeNode("Register && Execute Allowance");
            TreeNode treeNode44 = new TreeNode("Salary Calculation");
            TreeNode treeNode45 = new TreeNode("Report End Of Month");
            TreeNode treeNode46 = new TreeNode("Salary", new TreeNode[] { treeNode43, treeNode44, treeNode45 });
            TreeNode treeNode47 = new TreeNode("SALARY", new TreeNode[] { treeNode42, treeNode46 });
            TreeNode treeNode48 = new TreeNode("HR", new TreeNode[] { treeNode40, treeNode47 });
            TreeNode treeNode49 = new TreeNode("DEV");
            TreeNode treeNode50 = new TreeNode("LAB");
            TreeNode treeNode51 = new TreeNode("MAT");
            TreeNode treeNode52 = new TreeNode("QA");
            TreeNode treeNode53 = new TreeNode("QIP");
            TreeNode treeNode54 = new TreeNode("RTDM", new TreeNode[] { treeNode49, treeNode50, treeNode51, treeNode52, treeNode53 });
            TreeNode treeNode55 = new TreeNode("LEAN NB");
            TreeNode treeNode56 = new TreeNode("LEAN AD");
            TreeNode treeNode57 = new TreeNode("LEAN", new TreeNode[] { treeNode55, treeNode56 });
            TreeNode treeNode58 = new TreeNode("STOCKFIT DEFECT");
            TreeNode treeNode59 = new TreeNode("Main Menu", new TreeNode[] { treeNode24, treeNode48, treeNode54, treeNode57, treeNode58 });
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            mainMenuStrip = new MenuStrip();
            menuHome = new ToolStripMenuItem();
            menuInsert = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            menuTools = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            sTOCKToolStripMenuItem = new ToolStripMenuItem();
            kiểmKhoToolStripMenuItem = new ToolStripMenuItem();
            exportCheckStockToolStripMenuItem = new ToolStripMenuItem();
            fPCardReaderMachineToolStripMenuItem = new ToolStripMenuItem();
            userManagementToolStripMenuItem = new ToolStripMenuItem();
            deviceStatusToolStripMenuItem = new ToolStripMenuItem();
            dataManagementToolStripMenuItem = new ToolStripMenuItem();
            iTAssetsToolStripMenuItem = new ToolStripMenuItem();
            iPManagementToolStripMenuItem = new ToolStripMenuItem();
            printLabelToolStripMenuItem = new ToolStripMenuItem();
            checkIPScanDevicesToolStripMenuItem = new ToolStripMenuItem();
            deviceManufacturerToolStripMenuItem = new ToolStripMenuItem();
            modelManagementToolStripMenuItem = new ToolStripMenuItem();
            devicesTypeManagementToolStripMenuItem = new ToolStripMenuItem();
            deviceManagementToolStripMenuItem = new ToolStripMenuItem();
            modifiedPCARDToolStripMenuItem = new ToolStripMenuItem();
            checkPCARDToolStripMenuItem = new ToolStripMenuItem();
            materialBarcodeToolStripMenuItem = new ToolStripMenuItem();
            cartonGWHSToolStripMenuItem = new ToolStripMenuItem();
            programVersionToolStripMenuItem = new ToolStripMenuItem();
            versionManagementToolStripMenuItem = new ToolStripMenuItem();
            userSystemToolStripMenuItem = new ToolStripMenuItem();
            userManagementToolStripMenuItem1 = new ToolStripMenuItem();
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
            kiểmKhoToolStripMenuItem.Size = new Size(172, 22);
            kiểmKhoToolStripMenuItem.Text = "Kiểm Kho";
            // 
            // exportCheckStockToolStripMenuItem
            // 
            exportCheckStockToolStripMenuItem.Name = "exportCheckStockToolStripMenuItem";
            exportCheckStockToolStripMenuItem.Size = new Size(172, 22);
            exportCheckStockToolStripMenuItem.Text = "Export CheckStock";
            // 
            // fPCardReaderMachineToolStripMenuItem
            // 
            fPCardReaderMachineToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { userManagementToolStripMenuItem, deviceStatusToolStripMenuItem, dataManagementToolStripMenuItem });
            fPCardReaderMachineToolStripMenuItem.Name = "fPCardReaderMachineToolStripMenuItem";
            fPCardReaderMachineToolStripMenuItem.Size = new Size(161, 20);
            fPCardReaderMachineToolStripMenuItem.Text = "FP && Card Reader Machine";
            // 
            // userManagementToolStripMenuItem
            // 
            userManagementToolStripMenuItem.Name = "userManagementToolStripMenuItem";
            userManagementToolStripMenuItem.Size = new Size(172, 22);
            userManagementToolStripMenuItem.Text = "User Management";
            // 
            // deviceStatusToolStripMenuItem
            // 
            deviceStatusToolStripMenuItem.Name = "deviceStatusToolStripMenuItem";
            deviceStatusToolStripMenuItem.Size = new Size(172, 22);
            deviceStatusToolStripMenuItem.Text = "Device Status";
            // 
            // dataManagementToolStripMenuItem
            // 
            dataManagementToolStripMenuItem.Name = "dataManagementToolStripMenuItem";
            dataManagementToolStripMenuItem.Size = new Size(172, 22);
            dataManagementToolStripMenuItem.Text = "Data Management";
            // 
            // iTAssetsToolStripMenuItem
            // 
            iTAssetsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { iPManagementToolStripMenuItem, printLabelToolStripMenuItem, checkIPScanDevicesToolStripMenuItem, deviceManufacturerToolStripMenuItem, modelManagementToolStripMenuItem, devicesTypeManagementToolStripMenuItem, deviceManagementToolStripMenuItem });
            iTAssetsToolStripMenuItem.Name = "iTAssetsToolStripMenuItem";
            iTAssetsToolStripMenuItem.Size = new Size(65, 20);
            iTAssetsToolStripMenuItem.Text = "IT Assets";
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
            // modifiedPCARDToolStripMenuItem
            // 
            modifiedPCARDToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { checkPCARDToolStripMenuItem, materialBarcodeToolStripMenuItem, cartonGWHSToolStripMenuItem });
            modifiedPCARDToolStripMenuItem.Name = "modifiedPCARDToolStripMenuItem";
            modifiedPCARDToolStripMenuItem.Size = new Size(113, 20);
            modifiedPCARDToolStripMenuItem.Text = "Modified P-CARD";
            // 
            // checkPCARDToolStripMenuItem
            // 
            checkPCARDToolStripMenuItem.Name = "checkPCARDToolStripMenuItem";
            checkPCARDToolStripMenuItem.Size = new Size(163, 22);
            checkPCARDToolStripMenuItem.Text = "Check P-CARD";
            // 
            // materialBarcodeToolStripMenuItem
            // 
            materialBarcodeToolStripMenuItem.Name = "materialBarcodeToolStripMenuItem";
            materialBarcodeToolStripMenuItem.Size = new Size(163, 22);
            materialBarcodeToolStripMenuItem.Text = "Material Barcode";
            // 
            // cartonGWHSToolStripMenuItem
            // 
            cartonGWHSToolStripMenuItem.Name = "cartonGWHSToolStripMenuItem";
            cartonGWHSToolStripMenuItem.Size = new Size(163, 22);
            cartonGWHSToolStripMenuItem.Text = "Carton GWHS";
            // 
            // programVersionToolStripMenuItem
            // 
            programVersionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { versionManagementToolStripMenuItem });
            programVersionToolStripMenuItem.Name = "programVersionToolStripMenuItem";
            programVersionToolStripMenuItem.Size = new Size(106, 20);
            programVersionToolStripMenuItem.Text = "Program Version";
            // 
            // versionManagementToolStripMenuItem
            // 
            versionManagementToolStripMenuItem.Name = "versionManagementToolStripMenuItem";
            versionManagementToolStripMenuItem.Size = new Size(186, 22);
            versionManagementToolStripMenuItem.Text = "Version Management";
            // 
            // userSystemToolStripMenuItem
            // 
            userSystemToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { userManagementToolStripMenuItem1 });
            userSystemToolStripMenuItem.Name = "userSystemToolStripMenuItem";
            userSystemToolStripMenuItem.Size = new Size(83, 20);
            userSystemToolStripMenuItem.Text = "User System";
            // 
            // userManagementToolStripMenuItem1
            // 
            userManagementToolStripMenuItem1.Name = "userManagementToolStripMenuItem1";
            userManagementToolStripMenuItem1.Size = new Size(171, 22);
            userManagementToolStripMenuItem1.Text = "User Management";
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
            treeNode1.Name = "barBtn_System_CheckStock";
            treeNode1.Tag = "CHECKSTOCK";
            treeNode1.Text = "Kiểm kho";
            treeNode2.Name = "barButtonItem7";
            treeNode2.Tag = "EX_CHECKSTOCK";
            treeNode2.Text = "Export CheckStock";
            treeNode3.Name = "Node15";
            treeNode3.Text = "Stock";
            treeNode4.Name = "barButtonItem1";
            treeNode4.Text = "User Management";
            treeNode5.Name = "barButtonItem2";
            treeNode5.Text = "Device Status";
            treeNode6.Name = "barButtonItem3";
            treeNode6.Text = "Data Management";
            treeNode7.Name = "Node16";
            treeNode7.Text = "FP && Card Reader Machine";
            treeNode8.Name = "barBtnIPManagement";
            treeNode8.Tag = "ASSET";
            treeNode8.Text = "IP Management";
            treeNode9.Name = "barBtnPrintAssetLabel";
            treeNode9.Tag = "ASSET";
            treeNode9.Text = "Print Label";
            treeNode10.Name = "ASSET";
            treeNode10.Tag = "ASSET";
            treeNode10.Text = "Check IP Scan Devices";
            treeNode11.Name = "barBtnManufacturer";
            treeNode11.Tag = "ASSET";
            treeNode11.Text = "Device Manufacturer";
            treeNode12.Name = "barBtnModel";
            treeNode12.Tag = "ASSET";
            treeNode12.Text = "Model Management";
            treeNode13.Name = "barBtnAssetType";
            treeNode13.Tag = "ASSET";
            treeNode13.Text = "Devices Type Management";
            treeNode14.Name = "barBtnAssetDevicesManagement";
            treeNode14.Tag = "ASSET";
            treeNode14.Text = "Device Management";
            treeNode15.Name = "Node17";
            treeNode15.Text = "IT Assets";
            treeNode16.Name = "barBtnCheckPCard";
            treeNode16.Tag = "PCARD";
            treeNode16.Text = "Check P-CARD";
            treeNode17.Name = "barButtonItem4";
            treeNode17.Text = "Material Barcode";
            treeNode18.Name = "barBtnCartonGWHS";
            treeNode18.Tag = "CARTON";
            treeNode18.Text = "Carton GWHS";
            treeNode19.Name = "Node18";
            treeNode19.Text = "Modified P-CARD";
            treeNode20.Name = "barBtnVersionManagement";
            treeNode20.Tag = "VERSION";
            treeNode20.Text = "Version Management";
            treeNode21.Name = "Node19";
            treeNode21.Text = "Program Version";
            treeNode22.Name = "barBtnUserManagement";
            treeNode22.Tag = "USERMNGT";
            treeNode22.Text = "User Mangement";
            treeNode23.Name = "Node20";
            treeNode23.Text = "User System";
            treeNode24.Name = "Node11";
            treeNode24.Text = "SYSTEM";
            treeNode25.Name = "Node50";
            treeNode25.Text = "Node50";
            treeNode26.Name = "Node51";
            treeNode26.Text = "Node51";
            treeNode27.Name = "Node44";
            treeNode27.Text = "Attendace";
            treeNode28.Name = "Node52";
            treeNode28.Text = "Node52";
            treeNode29.Name = "Node53";
            treeNode29.Text = "Node53";
            treeNode30.Name = "Node45";
            treeNode30.Text = "Canteen";
            treeNode31.Name = "Node46";
            treeNode31.Text = "Dashboard";
            treeNode32.Name = "Node54";
            treeNode32.Text = "Node54";
            treeNode33.Name = "Node55";
            treeNode33.Text = "Node55";
            treeNode34.Name = "Node47";
            treeNode34.Text = "Security";
            treeNode35.Name = "Node56";
            treeNode35.Text = "Node56";
            treeNode36.Name = "Node57";
            treeNode36.Text = "Node57";
            treeNode37.Name = "Node58";
            treeNode37.Text = "Node58";
            treeNode38.Name = "Node48";
            treeNode38.Text = "MAS";
            treeNode39.Name = "Node49";
            treeNode39.Text = "Org Chart";
            treeNode40.Name = "Node9";
            treeNode40.Text = "HRMS";
            treeNode41.Name = "Node38";
            treeNode41.Text = "Upload Excel";
            treeNode42.Name = "Node42";
            treeNode42.Text = "Upload Salary";
            treeNode43.Name = "Node39";
            treeNode43.Text = "Register && Execute Allowance";
            treeNode44.Name = "Node40";
            treeNode44.Text = "Salary Calculation";
            treeNode45.Name = "Node41";
            treeNode45.Text = "Report End Of Month";
            treeNode46.Name = "Node43";
            treeNode46.Text = "Salary";
            treeNode47.Name = "Node10";
            treeNode47.Text = "SALARY";
            treeNode48.Name = "Node12";
            treeNode48.Text = "HR";
            treeNode49.Name = "Node4";
            treeNode49.Text = "DEV";
            treeNode50.Name = "Node2";
            treeNode50.Text = "LAB";
            treeNode51.Name = "Node5";
            treeNode51.Text = "MAT";
            treeNode52.Name = "Node6";
            treeNode52.Text = "QA";
            treeNode53.Name = "Node1";
            treeNode53.Text = "QIP";
            treeNode54.Name = "Node13";
            treeNode54.Text = "RTDM";
            treeNode55.Name = "Node7";
            treeNode55.Text = "LEAN NB";
            treeNode56.Name = "Node8";
            treeNode56.Text = "LEAN AD";
            treeNode57.Name = "Node14";
            treeNode57.Text = "LEAN";
            treeNode58.Name = "Node3";
            treeNode58.Text = "STOCKFIT DEFECT";
            treeNode59.Name = "Node0";
            treeNode59.Text = "Main Menu";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode59 });
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
        public TabControl tabControl;
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