using ConnectionClass.Oracle;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETTMC
{
    public partial class MainForm : Form
    {
        //public GlobalFunction.PublicFunction pubFunc;
        private bool connectionstatus;
        CRUDOracle crud;
        public GlobalFunction.PublicFunction pubFunc;

        public MainForm()
        {
            InitializeComponent();
            ConnectionClass.Oracle.Site.ConnectionSite = "VS";
        }
        public void OpenTab<T>(string tabTitle, Action<T> initAction = null)
        where T : UserControl, new()
        {
            // Tìm tab đã tồn tại chưa — so sánh theo kiểu T và title
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Tag is T existingControl)
                {
                    // Đã mở → chỉ activate
                    tabControl.SelectedTab = page;
                    existingControl.Focus();
                    return;
                }
            }

            // Chưa mở → tạo mới
            var control = new T();
            control.Dock = DockStyle.Fill;

            // Gọi init trước khi hiển thị (load data, truyền params...)
            initAction?.Invoke(control);

            var tabPage = new TabPage(tabTitle);
            tabPage.Tag = control;           // lưu reference để tìm lại sau
            tabPage.Controls.Add(control);

            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
        }

        /// <summary>
        /// Overload: mở tab với key string tùy chỉnh.
        /// Dùng khi cùng 1 kiểu UC nhưng mở nhiều instance khác nhau
        /// (ví dụ: chi tiết đơn hàng #001, #002...)
        /// </summary>
        public void OpenTab<T>(string tabTitle, string uniqueKey, Action<T> initAction = null)
            where T : UserControl, new()
        {
            // Tìm theo uniqueKey thay vì kiểu
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Name == uniqueKey)
                {
                    tabControl.SelectedTab = page;
                    return;
                }
            }

            var control = new T();
            control.Dock = DockStyle.Fill;
            initAction?.Invoke(control);

            var tabPage = new TabPage(tabTitle);
            tabPage.Name = uniqueKey;
            tabPage.Tag = control;
            tabPage.Controls.Add(control);

            tabControl.TabPages.Add(tabPage);
            tabControl.SelectedTab = tabPage;
        }

        /// <summary>
        /// Đóng tab theo kiểu UC
        /// </summary>
        public void CloseTab<T>() where T : UserControl
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Tag is T control)
                {
                    control.Dispose();
                    tabControl.TabPages.Remove(page);
                    return;
                }
            }
        }

        /// <summary>
        /// Đóng tab theo uniqueKey
        /// </summary>
        public void CloseTab(string uniqueKey)
        {
            foreach (TabPage page in tabControl.TabPages)
            {
                if (page.Name == uniqueKey)
                {
                    (page.Tag as Control)?.Dispose();
                    tabControl.TabPages.Remove(page);
                    return;
                }
            }
        }

        /// <summary>
        /// Lấy control đang active trong tab hiện tại
        /// </summary>
        public T GetActiveControl<T>() where T : UserControl
        {
            return tabControl.SelectedTab?.Tag as T;
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║  NÚT ĐÓNG TAB (×) — xử lý click vào tiêu đề tab    ║
        // ╚══════════════════════════════════════════════════════╝

        private void mainTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < tabControl.TabPages.Count; i++)
            {
                var tabRect = tabControl.GetTabRect(i);

                // Vùng nút X (góc phải tab, 16x16px)
                var closeBtn = new Rectangle(
                    tabRect.Right - 18,
                    tabRect.Top + (tabRect.Height - 16) / 2,
                    16, 16);

                if (closeBtn.Contains(e.Location))
                {
                    var page = tabControl.TabPages[i];
                    (page.Tag as Control)?.Dispose();
                    tabControl.TabPages.Remove(page);
                    return;
                }
            }
        }

        // Vẽ nút × trên mỗi tab
        private void mainTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            var page = tabControl.TabPages[e.Index];
            var tabRect = tabControl.GetTabRect(e.Index);
            bool isSelected = (tabControl.SelectedIndex == e.Index);

            // Nền tab
            e.Graphics.FillRectangle(
                isSelected
                    ? SystemBrushes.Window
                    : new SolidBrush(Color.FromArgb(240, 240, 240)),
                tabRect);

            // Tiêu đề (dịch trái để nhường chỗ cho ×)
            var titleRect = new Rectangle(
                tabRect.Left + 6,
                tabRect.Top,
                tabRect.Width - 24,
                tabRect.Height);

            TextRenderer.DrawText(
                e.Graphics,
                page.Text,
                e.Font,
                titleRect,
                isSelected ? Color.FromArgb(0, 96, 192) : Color.FromArgb(60, 60, 60),
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left);

            // Nút ×
            var closeRect = new Rectangle(
                tabRect.Right - 18,
                tabRect.Top + (tabRect.Height - 16) / 2,
                16, 16);

            e.Graphics.DrawString(
                "×",
                new Font("Arial", 10, FontStyle.Bold),
                Brushes.Gray,
                closeRect);
        }

        // ╔══════════════════════════════════════════════════════╗
        // ║  VÍ DỤ SỬ DỤNG — gọi từ menu / treeview / button   ║
        // ╚══════════════════════════════════════════════════════╝


        private void MainForm_Load(object sender, EventArgs e)
        {
            //OpenTab<frmTMC7036>("Tổng quan");
            crud = new CRUDOracle("VSMES");
            StartConnectionTimer();
            pubFunc = new GlobalFunction.PublicFunction();
            //if(GlobalFunction.PublicFunction.ConnectionLocation == "VS")
            //{
            //}
            //tabControl.CloseButtonClick += TabControl_CloseButtonClick;
            //crud = new CRUDOracle("VSMES");
            if (GlobalFunction.Authentication.Permission.UserInformation.UserID != null)
            {
                this.barUserID.Text = "User ID : " + GlobalFunction.Authentication.Permission.UserInformation.UserID + " User Name : " + GlobalFunction.Authentication.Permission.UserInformation.UserName + " Role : " + GlobalFunction.Authentication.Permission.UserInformation.UserRole;
            }
            barIPaddress.Text = GlobalFunction.PublicFunction.myIpaddress;

            //Task taskCheckConnect = new Task(() => {
            //    if (crud.ConnectionStatus())
            //    {
            //        barConnectionStatus.Caption = "Connected";
            //        barConnectionStatus.ItemAppearance.Normal.ForeColor = Color.Green;
            //    }
            //    else
            //    {
            //        barConnectionStatus.Caption = "Disconnected";
            //        barConnectionStatus.ItemAppearance.Normal.ForeColor = Color.Red;
            //    }

            //});
            //taskCheckConnect.Start();
            GetPosition();
        }
        //private void TabControl_CloseButtonClick(object sender, EventArgs e)
        //{
        //    ClosePageButtonEventArgs arg = e as ClosePageButtonEventArgs;
        //    (arg.Page as XtraTabPage).PageVisible = false;
        //    (arg.Page as XtraTabPage).Dispose();
        //}
        private string GetPosition()
        {
            DataTable dt = new DataTable();
            dt = crud.dac.DtSelectExcuteWithQuery("SELECT * FROM MES.TRTB_M_P2P_SETTING WHERE I_IP_NO = '" + GlobalFunction.PublicFunction.myIpaddress + "' ");
            if (dt == null)
            {
                return "";
            }
            if (dt.Rows.Count > 0)
            {
                if (dt.Rows[0]["N_POSITION"] == null)
                {
                    return "";
                }
                else
                {
                    Screen[] screens = Screen.AllScreens;
                    if (screens.Count() > 1)
                    {
                        string position = ""; position = dt.Rows[0]["N_POSITION"].ToString();
                        if (position != "")
                        {
                            if (position == "2")
                            {
                                this.WindowState = FormWindowState.Normal;
                                var workingArea = screens[1].WorkingArea;
                                this.Left = workingArea.Left + 10;
                                this.Top = workingArea.Top + 10;
                                this.Width = workingArea.Width + 10;
                                this.Height = workingArea.Height + 10;
                                this.WindowState = FormWindowState.Maximized;
                            }
                            else
                            {
                                this.WindowState = FormWindowState.Normal;
                                var workingArea = screens[0].WorkingArea;
                                this.Left = workingArea.Left + 10;
                                this.Top = workingArea.Top + 10;
                                this.Width = workingArea.Width + 10;
                                this.Height = workingArea.Height + 10;
                                this.WindowState = FormWindowState.Maximized;
                            }
                        }
                    }
                    return dt.Rows[0]["N_POSITION"].ToString();
                }
            }
            else
            {
                return "";
            }
        }
        private void UpdateConnectionIndicator()
        {

            bool ok = crud.ConnectionStatus();

            // Phải dùng Invoke vì timer chạy trên thread khác (nếu dùng async)
            if (statusLabelConnection.GetCurrentParent()?.InvokeRequired == true)
            {
                statusLabelConnection.GetCurrentParent().Invoke(
                    () => ApplyConnectionUI(ok));
            }
            else
            {
                ApplyConnectionUI(ok);
            }
        }
        private void ApplyConnectionUI(bool isConnected)
        {
            if (isConnected)
            {
                statusLabelConnection.Text = "● Đã kết nối";
                statusLabelConnection.ForeColor = Color.Lime;
                statusLabelConnection.ToolTipText = "Kết nối mạng bình thường";
            }
            else
            {
                statusLabelConnection.Text = "● Mất kết nối";
                statusLabelConnection.ForeColor = Color.Red;
                statusLabelConnection.ToolTipText = "Không có kết nối mạng!";
            }
        }
        /// <summary>
        /// Khởi động timer kiểm tra mạng định kỳ (mỗi 10 giây).
        /// Gọi trong MainForm_Load.
        /// </summary>
        private void StartConnectionTimer()
        {
            timerConnectionCheck = new System.Windows.Forms.Timer();
            timerConnectionCheck.Interval = 300000;   // 10 giây — chỉnh tùy ý
            timerConnectionCheck.Tick += (s, e) => Task.Run(UpdateConnectionIndicator);
            timerConnectionCheck.Start();

            // Kiểm tra ngay lần đầu khi mở form
            Task.Run(UpdateConnectionIndicator);
        }
        private void mainTabControl_SelectedIndexChanged(object sender, EventArgs e) 
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            switch (e.Node.Text)
            {
                case "STOCKFIT DEFECT":
                    OpenTab<testUC>("STOCKFIT DEFECT");
                    break;

                    //case "Module 1":
                    //OpenTab<Module1Control>("Module 1");
                    //break;

                    // Mở nhiều instance cùng kiểu — dùng uniqueKey
                    //case var name when name.StartsWith("Đơn hàng"):
                    //    var orderId = e.Node.Tag?.ToString();
                    //    OpenTab<OrderDetailControl>(
                    //        title: $"Đơn hàng #{orderId}",
                    //        uniqueKey: $"order_{orderId}",
                    //        initAction: uc => uc.LoadOrder(orderId));
                    //    break;
            }
        }

        private void barBtnClose_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
            Application.Exit();
        }
    }
}
