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
            TreeNode treeNode25 = new TreeNode("Attendace Management");
            TreeNode treeNode26 = new TreeNode("EIS PPH data");
            TreeNode treeNode27 = new TreeNode("Attendace", new TreeNode[] { treeNode25, treeNode26 });
            TreeNode treeNode28 = new TreeNode("Canteen Management");
            TreeNode treeNode29 = new TreeNode("Update CardNum");
            TreeNode treeNode30 = new TreeNode("Canteen", new TreeNode[] { treeNode28, treeNode29 });
            TreeNode treeNode31 = new TreeNode("Dashboard");
            TreeNode treeNode32 = new TreeNode("Security Registration");
            TreeNode treeNode33 = new TreeNode("Security Gate Display TV");
            TreeNode treeNode34 = new TreeNode("Security", new TreeNode[] { treeNode32, treeNode33 });
            TreeNode treeNode35 = new TreeNode("MAS");
            TreeNode treeNode36 = new TreeNode("MAS Change Work");
            TreeNode treeNode37 = new TreeNode("Mas Permission");
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
            TreeNode treeNode49 = new TreeNode("Sample QR Code");
            TreeNode treeNode50 = new TreeNode("RFID Management");
            TreeNode treeNode51 = new TreeNode("RFID Update Movement");
            TreeNode treeNode52 = new TreeNode("RFID Track Movement");
            TreeNode treeNode53 = new TreeNode("RFID Gate");
            TreeNode treeNode54 = new TreeNode("RFID Area");
            TreeNode treeNode55 = new TreeNode("RFID Route");
            TreeNode treeNode56 = new TreeNode("RFID Alarm");
            TreeNode treeNode57 = new TreeNode("Visualization");
            TreeNode treeNode58 = new TreeNode("RFID Exception");
            TreeNode treeNode59 = new TreeNode("RFID Inventory");
            TreeNode treeNode60 = new TreeNode("Receive Shoe");
            TreeNode treeNode61 = new TreeNode("System Log");
            TreeNode treeNode62 = new TreeNode("RFID Chip Inventory");
            TreeNode treeNode63 = new TreeNode("RFID Management", new TreeNode[] { treeNode50, treeNode51, treeNode52, treeNode53, treeNode54, treeNode55, treeNode56, treeNode57, treeNode58, treeNode59, treeNode60, treeNode61, treeNode62 });
            TreeNode treeNode64 = new TreeNode("DEV", new TreeNode[] { treeNode49, treeNode63 });
            TreeNode treeNode65 = new TreeNode("Lab Test Result A01");
            TreeNode treeNode66 = new TreeNode("Lab Test ( FGT,CMA )");
            TreeNode treeNode67 = new TreeNode("Bonding Lab Test");
            TreeNode treeNode68 = new TreeNode("Bonding Sole");
            TreeNode treeNode69 = new TreeNode("Material Lab Test");
            TreeNode treeNode70 = new TreeNode("Analyze Mat Lab Test");
            TreeNode treeNode71 = new TreeNode("Analyze MidSole Lab Test");
            TreeNode treeNode72 = new TreeNode("Lab Test Summary");
            TreeNode treeNode73 = new TreeNode("Midsole Lab Test Result");
            TreeNode treeNode74 = new TreeNode("No Sew Lab Test Result");
            TreeNode treeNode75 = new TreeNode("Sole Lab Test Result");
            TreeNode treeNode76 = new TreeNode("Lab Test Result", new TreeNode[] { treeNode65, treeNode66, treeNode67, treeNode68, treeNode69, treeNode70, treeNode71, treeNode72, treeNode73, treeNode74, treeNode75 });
            TreeNode treeNode77 = new TreeNode("Upload FS Orders");
            TreeNode treeNode78 = new TreeNode("Report Lab Test");
            TreeNode treeNode79 = new TreeNode("Report Boding Test");
            TreeNode treeNode80 = new TreeNode("Report", new TreeNode[] { treeNode78, treeNode79 });
            TreeNode treeNode81 = new TreeNode("CREATENEWFILE");
            TreeNode treeNode82 = new TreeNode("Upload File");
            TreeNode treeNode83 = new TreeNode("View File");
            TreeNode treeNode84 = new TreeNode("barBtnDelete");
            TreeNode treeNode85 = new TreeNode("File Management", new TreeNode[] { treeNode81, treeNode82, treeNode83, treeNode84 });
            TreeNode treeNode86 = new TreeNode("LAB", new TreeNode[] { treeNode76, treeNode77, treeNode80, treeNode85 });
            TreeNode treeNode87 = new TreeNode("Material Request Lab Test");
            TreeNode treeNode88 = new TreeNode("Material Lab Test Management");
            TreeNode treeNode89 = new TreeNode("Material Visual Check");
            TreeNode treeNode90 = new TreeNode("QA Material", new TreeNode[] { treeNode87, treeNode88, treeNode89 });
            TreeNode treeNode91 = new TreeNode("Visual Layout");
            TreeNode treeNode92 = new TreeNode("Material Status");
            TreeNode treeNode93 = new TreeNode("Lab Check Material Request");
            TreeNode treeNode94 = new TreeNode("Material", new TreeNode[] { treeNode91, treeNode92, treeNode93 });
            TreeNode treeNode95 = new TreeNode("MAT", new TreeNode[] { treeNode90, treeNode94 });
            TreeNode treeNode96 = new TreeNode("Upload Picture");
            TreeNode treeNode97 = new TreeNode("barButtonItem13");
            TreeNode treeNode98 = new TreeNode("Defect End Of Line", new TreeNode[] { treeNode96, treeNode97 });
            TreeNode treeNode99 = new TreeNode("QA", new TreeNode[] { treeNode98 });
            TreeNode treeNode100 = new TreeNode("Managed IC Part && Defect");
            TreeNode treeNode101 = new TreeNode("Inspection QTY");
            TreeNode treeNode102 = new TreeNode("Inspection Certificate");
            TreeNode treeNode103 = new TreeNode("Inspection Report");
            TreeNode treeNode104 = new TreeNode("Inspection", new TreeNode[] { treeNode101, treeNode102, treeNode103 });
            TreeNode treeNode105 = new TreeNode("SHAS Information");
            TreeNode treeNode106 = new TreeNode("Prod Line RFT");
            TreeNode treeNode107 = new TreeNode("Inspection Room", new TreeNode[] { treeNode100, treeNode104, treeNode105, treeNode106 });
            TreeNode treeNode108 = new TreeNode("MidSole Lab Test");
            TreeNode treeNode109 = new TreeNode("Bonding Mid-Sole Labtest");
            TreeNode treeNode110 = new TreeNode("Sole Lab Test");
            TreeNode treeNode111 = new TreeNode("Sole Lab Test", new TreeNode[] { treeNode108, treeNode109, treeNode110 });
            TreeNode treeNode112 = new TreeNode("OCPT Tracking List Tester");
            TreeNode treeNode113 = new TreeNode("OCPT Test Allocation");
            TreeNode treeNode114 = new TreeNode("Tracking Fit/Wear Test");
            TreeNode treeNode115 = new TreeNode("OCPT", new TreeNode[] { treeNode112, treeNode113, treeNode114 });
            TreeNode treeNode116 = new TreeNode("Report Bonding Test");
            TreeNode treeNode117 = new TreeNode("REPORTBONDING");
            TreeNode treeNode118 = new TreeNode("SI PO Cancel");
            TreeNode treeNode119 = new TreeNode("PSC");
            TreeNode treeNode120 = new TreeNode("Report", new TreeNode[] { treeNode116, treeNode117, treeNode118, treeNode119 });
            TreeNode treeNode121 = new TreeNode("Defect Return");
            TreeNode treeNode122 = new TreeNode("Analyze DR");
            TreeNode treeNode123 = new TreeNode("Analyze DR ( Top 5 )");
            TreeNode treeNode124 = new TreeNode("Claim History");
            TreeNode treeNode125 = new TreeNode("Defecttive", new TreeNode[] { treeNode121, treeNode122, treeNode123, treeNode124 });
            TreeNode treeNode126 = new TreeNode("Dashboard");
            TreeNode treeNode127 = new TreeNode("Temperature And Humidity", new TreeNode[] { treeNode126 });
            TreeNode treeNode128 = new TreeNode("QIP", new TreeNode[] { treeNode107, treeNode111, treeNode115, treeNode120, treeNode125, treeNode127 });
            TreeNode treeNode129 = new TreeNode("RTDM", new TreeNode[] { treeNode64, treeNode86, treeNode95, treeNode99, treeNode128 });
            TreeNode treeNode130 = new TreeNode("TPM Report");
            TreeNode treeNode131 = new TreeNode("Node29");
            TreeNode treeNode132 = new TreeNode("Node30");
            TreeNode treeNode133 = new TreeNode("Node33");
            TreeNode treeNode134 = new TreeNode("Node34");
            TreeNode treeNode135 = new TreeNode("TPM Management", new TreeNode[] { treeNode130, treeNode131, treeNode132, treeNode133, treeNode134 });
            TreeNode treeNode136 = new TreeNode("Andon System");
            TreeNode treeNode137 = new TreeNode("Monitor All TV Display (NB)");
            TreeNode treeNode138 = new TreeNode("LEAN NB", new TreeNode[] { treeNode135, treeNode136, treeNode137 });
            TreeNode treeNode139 = new TreeNode("LEAN AD");
            TreeNode treeNode140 = new TreeNode("LEAN", new TreeNode[] { treeNode138, treeNode139 });
            TreeNode treeNode141 = new TreeNode("STOCKFIT DEFECT");
            TreeNode treeNode142 = new TreeNode("Main Menu", new TreeNode[] { treeNode24, treeNode48, treeNode129, treeNode140, treeNode141 });
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
            treeNode25.Name = "barBtnAttendaceMngt";
            treeNode25.Tag = "ATTENDANCE";
            treeNode25.Text = "Attendace Management";
            treeNode26.Name = "barBtnEIS";
            treeNode26.Tag = "EISPPHDATA";
            treeNode26.Text = "EIS PPH data";
            treeNode27.Name = "Node44";
            treeNode27.Text = "Attendace";
            treeNode28.Name = "barBtnCanteenManagement";
            treeNode28.Tag = "CANTEEN_MNGT";
            treeNode28.Text = "Canteen Management";
            treeNode29.Name = "barBtnUpdateCardNum";
            treeNode29.Text = "Update CardNum";
            treeNode30.Name = "Node45";
            treeNode30.Text = "Canteen";
            treeNode31.Name = "barBtnDashBoard";
            treeNode31.Tag = "ATTENDANCE";
            treeNode31.Text = "Dashboard";
            treeNode32.Name = "barBtnSecRegistration";
            treeNode32.Tag = "SEC_REG";
            treeNode32.Text = "Security Registration";
            treeNode33.Name = "barBtnSecTV";
            treeNode33.Tag = "SEC_TV";
            treeNode33.Text = "Security Gate Display TV";
            treeNode34.Name = "Node47";
            treeNode34.Text = "Security";
            treeNode35.Name = "btnMas";
            treeNode35.Tag = "ATTENDANCE";
            treeNode35.Text = "MAS";
            treeNode36.Name = "btnMasChangeWork";
            treeNode36.Tag = "MASCHANGEWORK";
            treeNode36.Text = "MAS Change Work";
            treeNode37.Name = "btnMasPermission";
            treeNode37.Text = "Mas Permission";
            treeNode38.Name = "Node48";
            treeNode38.Text = "MAS";
            treeNode39.Name = "btnOrgChart";
            treeNode39.Tag = "ATTENDANCE";
            treeNode39.Text = "Org Chart";
            treeNode40.Name = "Node9";
            treeNode40.Text = "HRMS";
            treeNode41.Name = "barBtnExcelUpload";
            treeNode41.Tag = "UPLOADSALARY";
            treeNode41.Text = "Upload Excel";
            treeNode42.Name = "Node42";
            treeNode42.Text = "Upload Salary";
            treeNode43.Name = "barButtonItem14";
            treeNode43.Tag = "UPLOADSALARY";
            treeNode43.Text = "Register && Execute Allowance";
            treeNode44.Name = "btnSalaryCalculation";
            treeNode44.Tag = "UPLOADSALARY";
            treeNode44.Text = "Salary Calculation";
            treeNode45.Name = "btnSalaryReport";
            treeNode45.Tag = "REPORTSALARY";
            treeNode45.Text = "Report End Of Month";
            treeNode46.Name = "Node43";
            treeNode46.Text = "Salary";
            treeNode47.Name = "Node10";
            treeNode47.Text = "SALARY";
            treeNode48.Name = "Node12";
            treeNode48.Text = "HR";
            treeNode49.Name = "barBtnSampleQRCode";
            treeNode49.Tag = "QRCODEBIZAD";
            treeNode49.Text = "Sample QR Code";
            treeNode50.Name = "barBtnRFIDManagement";
            treeNode50.Tag = "RFIDMNGT";
            treeNode50.Text = "RFID Management";
            treeNode51.Name = "barBtnEditMovement";
            treeNode51.Tag = "RFIDMOVEMENT";
            treeNode51.Text = "RFID Update Movement";
            treeNode52.Name = "barButtonItem24";
            treeNode52.Tag = "RFIDMOVEMENT";
            treeNode52.Text = "RFID Track Movement";
            treeNode53.Name = "barBtnRFIDGate";
            treeNode53.Tag = "RFIDGATE";
            treeNode53.Text = "RFID Gate";
            treeNode54.Name = "barButtonItem22";
            treeNode54.Tag = "RFIDAREA";
            treeNode54.Text = "RFID Area";
            treeNode55.Name = "barBtnRIFDRoute";
            treeNode55.Tag = "barBtnRIFDRoute";
            treeNode55.Text = "RFID Route";
            treeNode56.Name = "barButtonItem19";
            treeNode56.Tag = "RFIDALARM";
            treeNode56.Text = "RFID Alarm";
            treeNode57.Name = "RFID Alarm";
            treeNode57.Tag = "RFIDMNGT";
            treeNode57.Text = "Visualization";
            treeNode58.Name = "barBtnRFIDException";
            treeNode58.Tag = "RFIDEXCEPTION";
            treeNode58.Text = "RFID Exception";
            treeNode59.Name = "barBtnInventory";
            treeNode59.Tag = "RFIDINVENTORY";
            treeNode59.Text = "RFID Inventory";
            treeNode60.Name = "barBtnReceiveShoe";
            treeNode60.Tag = "RFIDRECEIVESHOES";
            treeNode60.Text = "Receive Shoe";
            treeNode61.Name = "barBtnSystemLog";
            treeNode61.Tag = "RFIDLOG";
            treeNode61.Text = "System Log";
            treeNode62.Name = "barBtnInventoryCard";
            treeNode62.Text = "RFID Chip Inventory";
            treeNode63.Name = "Node22";
            treeNode63.Text = "RFID Management";
            treeNode64.Name = "Node4";
            treeNode64.Text = "DEV";
            treeNode65.Name = "barBtnLabTest";
            treeNode65.Tag = "LABTESTRESULT";
            treeNode65.Text = "Lab Test Result A01";
            treeNode66.Name = "barBtnLabTestFGTCMA";
            treeNode66.Tag = "LABTESTRESULT";
            treeNode66.Text = "Lab Test ( FGT,CMA )";
            treeNode67.Name = "barBtnBondingLabTest";
            treeNode67.Tag = "BONDINGLABTEST";
            treeNode67.Text = "Bonding Lab Test";
            treeNode68.Name = "barButtonItem18";
            treeNode68.Tag = "BONDINGLABTEST";
            treeNode68.Text = "Bonding Sole";
            treeNode69.Name = "barButtonItem17";
            treeNode69.Tag = "BONDINGLABTEST";
            treeNode69.Text = "Material Lab Test";
            treeNode70.Name = "barBtnAnalyzeMatLabtest";
            treeNode70.Tag = "BONDINGLABTEST";
            treeNode70.Text = "Analyze Mat Lab Test";
            treeNode71.Name = "barBtnAnalyzeMidSoleLabTest";
            treeNode71.Tag = "BONDINGLABTEST";
            treeNode71.Text = "Analyze MidSole Lab Test";
            treeNode72.Name = "barBtnLabTestDashboard";
            treeNode72.Tag = "LABTESTDASHBOARD";
            treeNode72.Text = "Lab Test Summary";
            treeNode73.Name = "barBtnMidSole";
            treeNode73.Tag = "LABTESTRESULT";
            treeNode73.Text = "Midsole Lab Test Result";
            treeNode74.Name = "barBtnNoSewLabTest";
            treeNode74.Tag = "NOSEWLABTEST";
            treeNode74.Text = "No Sew Lab Test Result";
            treeNode75.Name = "barBtnSole";
            treeNode75.Tag = "LABTESTRESULT";
            treeNode75.Text = "Sole Lab Test Result";
            treeNode76.Name = "Node21";
            treeNode76.Text = "Lab Test Result";
            treeNode77.Name = "barBtnUploadFSOrder";
            treeNode77.Tag = "UPLOADFSORDER";
            treeNode77.Text = "Upload FS Orders";
            treeNode78.Name = "barBtnReportLab";
            treeNode78.Tag = "REPORTBONDING";
            treeNode78.Text = "Report Lab Test";
            treeNode79.Name = "Report Lab Test";
            treeNode79.Tag = "REPORTBONDING";
            treeNode79.Text = "Report Boding Test";
            treeNode80.Name = "Node24";
            treeNode80.Text = "Report";
            treeNode81.Name = "barBtnFileCreateNew";
            treeNode81.Tag = "CREATENEWFILE";
            treeNode81.Text = "CREATENEWFILE";
            treeNode82.Name = "barBtnUploadFile";
            treeNode82.Tag = "UPLOADFILE";
            treeNode82.Text = "Upload File";
            treeNode83.Name = "barBtnViewFile";
            treeNode83.Tag = "VIEWFILE";
            treeNode83.Text = "View File";
            treeNode84.Name = "barBtnDelete";
            treeNode84.Tag = "barBtnDelete";
            treeNode84.Text = "barBtnDelete";
            treeNode85.Name = "Node25";
            treeNode85.Text = "File Management";
            treeNode86.Name = "Node2";
            treeNode86.Text = "LAB";
            treeNode87.Name = "barBtnMatRequestLab";
            treeNode87.Tag = "MATREQLABTEST";
            treeNode87.Text = "Material Request Lab Test";
            treeNode88.Name = "barBtnLabTestManagement";
            treeNode88.Tag = "MATREQLABTEST";
            treeNode88.Text = "Material Lab Test Management";
            treeNode89.Name = "barBtnMatVisualCheck";
            treeNode89.Tag = "MATVISUALCHECK";
            treeNode89.Text = "Material Visual Check";
            treeNode90.Name = "Node31";
            treeNode90.Text = "QA Material";
            treeNode91.Name = "barBtnMatVisualLayout";
            treeNode91.Tag = "MATVISUALLAYOUT";
            treeNode91.Text = "Visual Layout";
            treeNode92.Name = "barBtnMatStatus";
            treeNode92.Tag = "MATSTATUSCHECK";
            treeNode92.Text = "Material Status";
            treeNode93.Name = "barBtnLabMatTest";
            treeNode93.Text = "Lab Check Material Request";
            treeNode94.Name = "Node32";
            treeNode94.Text = "Material";
            treeNode95.Name = "Node5";
            treeNode95.Text = "MAT";
            treeNode96.Name = "barBtnUploadImage";
            treeNode96.Tag = "UPLOADSHOE";
            treeNode96.Text = "Upload Picture";
            treeNode97.Name = "barButtonItem13";
            treeNode97.Tag = "UPLOADSHOE";
            treeNode97.Text = "barButtonItem13";
            treeNode98.Name = "Node28";
            treeNode98.Text = "Defect End Of Line";
            treeNode99.Name = "Node6";
            treeNode99.Text = "QA";
            treeNode100.Name = "btnICPartDefect";
            treeNode100.Tag = "ICPART";
            treeNode100.Text = "Managed IC Part && Defect";
            treeNode101.Name = "barButtonItem8";
            treeNode101.Text = "Inspection QTY";
            treeNode102.Name = "barButtonItem9";
            treeNode102.Text = "Inspection Certificate";
            treeNode103.Name = "barButtonItem10";
            treeNode103.Text = "Inspection Report";
            treeNode104.Name = "Node49";
            treeNode104.Text = "Inspection";
            treeNode105.Name = "btnSHASInformation";
            treeNode105.Tag = "SHASINFO";
            treeNode105.Text = "SHAS Information";
            treeNode106.Name = "barButtonItem5";
            treeNode106.Text = "Prod Line RFT";
            treeNode107.Name = "Node36";
            treeNode107.Text = "Inspection Room";
            treeNode108.Name = "barBtnMidSoleSendLabTest";
            treeNode108.Tag = "REPORTBONDING";
            treeNode108.Text = "MidSole Lab Test";
            treeNode109.Name = "barBtnBondingMidSoleLabTest";
            treeNode109.Tag = "MIDSOLEBONDING";
            treeNode109.Text = "Bonding Mid-Sole Labtest";
            treeNode110.Name = "Bonding Mid-Sole Labtest";
            treeNode110.Tag = "SOLELABTEST";
            treeNode110.Text = "Sole Lab Test";
            treeNode111.Name = "Node37";
            treeNode111.Text = "Sole Lab Test";
            treeNode112.Name = "barBtnOCPTTrackingList";
            treeNode112.Tag = "TRACKINGTESTOCPT";
            treeNode112.Text = "OCPT Tracking List Tester";
            treeNode113.Name = "barButtonItem11";
            treeNode113.Text = "OCPT Test Allocation";
            treeNode114.Name = "barBtnFitWearTestResult";
            treeNode114.Tag = "TRACKINGTESTOCPT";
            treeNode114.Text = "Tracking Fit/Wear Test";
            treeNode115.Name = "Node38";
            treeNode115.Text = "OCPT";
            treeNode116.Name = "barQIPReportBondingTest";
            treeNode116.Tag = "REPORTBONDING";
            treeNode116.Text = "Report Bonding Test";
            treeNode117.Name = "barBtnReportDetailLabTest";
            treeNode117.Tag = "REPORTBONDING";
            treeNode117.Text = "REPORTBONDING";
            treeNode118.Name = "barBtnPOCancel";
            treeNode118.Tag = "ANALYZEDEFECTRETURN";
            treeNode118.Text = "SI PO Cancel";
            treeNode119.Name = "barBTNPSC";
            treeNode119.Tag = "PSCC";
            treeNode119.Text = "PSC";
            treeNode120.Name = "Node39";
            treeNode120.Text = "Report";
            treeNode121.Name = "barDefectReturn";
            treeNode121.Tag = "DEFECTRETURN";
            treeNode121.Text = "Defect Return";
            treeNode122.Name = "barBtnAnalyzeDefectReturn";
            treeNode122.Tag = "ANALYZEDEFECTRETURN";
            treeNode122.Text = "Analyze DR";
            treeNode123.Name = "barAnalyzeTop5Defect";
            treeNode123.Tag = "ANALYZEDEFECTRETURN";
            treeNode123.Text = "Analyze DR ( Top 5 )";
            treeNode124.Name = "barBtnClaimHistory";
            treeNode124.Tag = "ANALYZEDEFECTRETURN";
            treeNode124.Text = "Claim History";
            treeNode125.Name = "Node40";
            treeNode125.Text = "Defecttive";
            treeNode126.Name = "barBtnTempAndHum";
            treeNode126.Tag = "ANALYZEDEFECTRETURN";
            treeNode126.Text = "Dashboard";
            treeNode127.Name = "ssadvdv";
            treeNode127.Tag = "";
            treeNode127.Text = "Temperature And Humidity";
            treeNode128.Name = "Node1";
            treeNode128.Text = "QIP";
            treeNode129.Name = "Node13";
            treeNode129.Text = "RTDM";
            treeNode130.Name = "barBtnreport";
            treeNode130.Tag = "TPMREPORT";
            treeNode130.Text = "TPM Report";
            treeNode131.Name = "Node29";
            treeNode131.Text = "Node29";
            treeNode132.Name = "Node30";
            treeNode132.Text = "Node30";
            treeNode133.Name = "Node33";
            treeNode133.Text = "Node33";
            treeNode134.Name = "Node34";
            treeNode134.Text = "Node34";
            treeNode135.Name = "Node0";
            treeNode135.Text = "TPM Management";
            treeNode136.Name = "Node23";
            treeNode136.Text = "Andon System";
            treeNode137.Name = "barButtonItem16";
            treeNode137.Tag = "CONFIGP2P";
            treeNode137.Text = "Monitor All TV Display (NB)";
            treeNode138.Name = "Node7";
            treeNode138.Text = "LEAN NB";
            treeNode139.Name = "Node8";
            treeNode139.Text = "LEAN AD";
            treeNode140.Name = "Node14";
            treeNode140.Text = "LEAN";
            treeNode141.Name = "Node3";
            treeNode141.Text = "STOCKFIT DEFECT";
            treeNode142.Name = "dvsdvsvd";
            treeNode142.Tag = "";
            treeNode142.Text = "Main Menu";
            treeView1.Nodes.AddRange(new TreeNode[] { treeNode142 });
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
        public System.Windows.Forms.MenuStrip mainMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuHome;
        private System.Windows.Forms.ToolStripMenuItem menuInsert;
        private System.Windows.Forms.ToolStripMenuItem menuView;
        private System.Windows.Forms.ToolStripMenuItem menuTools;
        private System.Windows.Forms.ToolStripMenuItem menuHelp;

        public System.Windows.Forms.ToolStrip ribbonStrip;
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
        public TreeView treeView1;
        public SplitContainer splitContainer1;
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