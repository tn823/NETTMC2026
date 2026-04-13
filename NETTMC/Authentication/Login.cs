using ConnectionClass.Oracle;
using QIP.EOL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace NETTMC.Authentication
{
    public partial class Login : Form
    {

        public GlobalFunction.PublicFunction pubFunc;
        private bool connectionstatus;
        CRUDOracle crud;


        public Login()
        {
            InitializeComponent();
            pubFunc = new GlobalFunction.PublicFunction();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //tableLayoutLogin.Hide();



            //lblMessage.Text = "";
            txtUser.Focus();
            labelControl5.Text = "Try to connect database...";
            progressPanel1.Show();
            //progressPanel2.Hide();
            string Ipadd = "";
            Ipadd = GlobalFunction.PublicFunction.myIpaddress;
            //Ipadd = "192.168.31.32";
            if (Ipadd.Substring(0, 10) == "192.168.1."
              || Ipadd.Substring(0, 10) == "192.168.2."
              || Ipadd.Substring(0, 10) == "192.168.3."
              || Ipadd.Substring(0, 10) == "192.168.4."
              || Ipadd.Substring(0, 10) == "192.168.5."
              || Ipadd.Substring(0, 10) == "192.168.6."
              || Ipadd.Substring(0, 10) == "192.168.7."
              || Ipadd.Substring(0, 11) == "192.168.54."
              || Ipadd.Substring(0, 11) == "192.168.56."
              || Ipadd.Substring(0, 10) == "192.168.0."
              || Ipadd.Substring(0, 11) == "192.168.52."
              || Ipadd.Substring(0, 11) == "192.168.53."
              || Ipadd.Substring(0, 11) == "192.168.12."
              || Ipadd.Substring(0, 11) == "192.168.17."
              || Ipadd.Substring(0, 11) == "192.168.31."
                )
            {
                labelControl3.Text = "Welcome to NetTMC software - Củ Chi";
                ConnectionClass.Oracle.Site.ConnectionSite = "VS";
            }
            else
            {
                labelControl3.Text = "Welcome to NetTMC software - An Giang";
                ConnectionClass.Oracle.Site.ConnectionSite = "AG";
            }
            //if (ConnectionClass.Oracle.Site.ConnectionSite == "AG")
            //{
            //    checkBoxSite.Checked = true;
            //    checkBoxSite.Text = "An Giang";
            //}
            //else
            //{
            //    checkBoxSite.Checked = false;
            //    checkBoxSite.Text = "Củ Chi";
            //}
            //toggleSite.IsOn = false;
            //labelControl3.Text = "Welcome to NetTMC software - An Giang";
            //ConnectionClass.Oracle.Site.ConnectionSite = "AG";
            crud = new CRUDOracle("VSMES");
            CheckConnectionStatus();
        }


        private bool CheckDefaultProgram()
        {
            var a = new GlobalFunction.PublicFunction().ReadFromFileNotMesage("DefaultProgram");
            //Debug.WriteLine("Default Program: " + a);
            if (a == null)
            {
                return false;
            }
            else
            {
                string[] arr = (string[])a;
                //if (arr[0].ToString() == "P2PAdidas")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    XtraUserControl uc = new LEAN.P2P.P2PAdidas();
                //    if (!pubFunc.OpenUserControl(uc, "P2P End Of Line Adidas", "P2P End Of Line Adidas", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "P2PSTFAdidascs")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    XtraUserControl uc = new LEAN.P2P.P2PSTFAdidascs();
                //    if (!pubFunc.OpenUserControl(uc, "P2P End Of Line Adidas", "P2P End Of Line Adidas", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "P2PNBAll")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();
                //    XtraUserControl uc = new LEAN.P2P.P2PNBAll();
                //    if (!pubFunc.OpenUserControl(uc, "Monitoring All Line (NB)", "Monitoring All Line (NB)", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;


                //}
                if (arr[0].ToString() == "TouchDefectSTF")
                {
                    MainForm mf = new MainForm();
                    mf.splitContainer1.Panel1Collapsed = true;
                    mf.ribbonStrip.Visible = false;
                    mf.mainMenuStrip.Visible = false;
                    mf.Show();

                    UserControl uc;
                    if (GlobalFunction.PublicFunction.myIpaddress == "192.168.0.85")
                    {
                        uc = new QIP.EOL.frmTMC7036_New();
                    }
                    else
                    {
                        uc = new QIP.EOL.frmTMC7036();
                    }
                    if (!pubFunc.OpenUserControl(uc, "Defect Stockfit (frmTMC7036)", "frmTMC7036", mf.tabControl))
                    {
                        MessageBox.Show("Sorry 36 ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        uc.Dispose();
                        mf.Hide();
                        this.Focus();
                        return false;
                    }
                    this.Hide();
                    return true;
                }
                if (arr[0].ToString() == "TouchDefect")
                {
                    MainForm mf = new MainForm();
                    mf.splitContainer1.Panel1Collapsed = true;
                    mf.ribbonStrip.Visible = false;
                    mf.mainMenuStrip.Visible = false;

                    mf.Show();
                    UserControl uc;

                    if (GlobalFunction.PublicFunction.myIpaddress == "192.168.31.249" || GlobalFunction.PublicFunction.myIpaddress == "192.168.31.62" || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.145") || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.213"))
                    {
                        uc = new QIP.EOL.frmTMC7033_A14();
                    }
                    else
                    {
                        uc = new QIP.EOL.frmTMC7033();
                    }
                    if (!pubFunc.OpenUserControl(uc, "NB Defect End Of Line (frmTMC7033)", "frmTMC7033", mf.tabControl))
                    {
                        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        uc.Dispose();
                        mf.Hide();
                        this.Focus();
                        return false;
                    }
                    this.Hide();
                    return true;
                }
                
                //if (arr[0].ToString() == "frmTMC7034")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    UserControl uc = new QIP.EOL.frmTMC7034();
                //    if (!pubFunc.OpenUserControl(uc, "NB Defect JSI (frmTMC7034)", "frmTMC7034", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "frmTMC7035")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    UserControl uc = new QIP.EOL.frmTMC7035();
                //    if (!pubFunc.OpenUserControl(uc, "NB Defect CCQP (frmTMC7035)", "frmTMC7035", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "frmTMC7031")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    UserControl uc = new QIP.frmTMC7031();
                //    if (!pubFunc.OpenUserControl(uc, "AD Defect End Of Line (frmTMC7031)", "frmTMC7031", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "frmTMC7039")
                //{
                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();

                //    UserControl uc = new QIP.EOL.frmTMC7039();
                //    if (!pubFunc.OpenUserControl(uc, "Defect Image (frmTMC7039)", "frmTMC7039", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }
                //    this.Hide();
                //    return true;
                //}
                //if (arr[0].ToString() == "frmTMC9001")
                //{

                //    MainForm mf = new MainForm();
                //    //mf.ribbon.Visible = false;
                //    mf.Show();
                //    UserControl uc = new HRMS.Security.frmTMC9001();
                //    if (!pubFunc.OpenUserControl(uc, "Security Gate Pass View", "Security Gate Pass View", mf.tabControl))
                //    {
                //        MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //        uc.Dispose();
                //        mf.Hide();
                //        this.Focus();
                //        return false;
                //    }

                //    this.Hide();
                //    return true;
                //}
                return false;
            }
        }

        private async void CheckConnectionStatus()
        {
            
            connectionstatus = await Task.Run(() =>
            {
                return crud.ConnectionStatus();

            });
            if (CheckDefaultProgram())
            {
                this.Hide();
            }
            if (connectionstatus)
            {
                labelControl5.Text = "Database Connected";
                labelControl5.ForeColor = Color.Green;
            }
            else
            {
                labelControl5.Text = "Database Disonnected";
                labelControl5.ForeColor = Color.Red;
            }
            //progressPanel1.Hide();
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutLogin_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnDefect_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            mf.splitContainer1.Panel1Collapsed = true;
            mf.ribbonStrip.Visible = false;
            mf.mainMenuStrip.Visible = false;

            mf.tabControl.Appearance = TabAppearance.FlatButtons;
            mf.tabControl.ItemSize = new Size(0, 1);
            mf.tabControl.SizeMode = TabSizeMode.Fixed;
            mf.Show();


            UserControl uc;
            if (GlobalFunction.PublicFunction.myIpaddress == ("192.168.1.197") || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.249") || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.62") || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.145") || GlobalFunction.PublicFunction.myIpaddress == ("192.168.31.213"))
            {
                uc = new QIP.EOL.frmTMC7033_A14();
            }
            else
            {
                uc = new QIP.EOL.frmTMC7033();
            }
            if (!pubFunc.OpenUserControl(uc, "NB Defect End Of Line (frmTMC7033)", "frmTMC7033", mf.tabControl))
            {
                MessageBox.Show("Sorry 111 ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                uc.Dispose();
                mf.Hide();
                this.Focus();
                return;
            }
            if (MessageBox.Show("Do you want to set this program default open on this machine ?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                new GlobalFunction.PublicFunction().WriteToFile("TouchDefect", "DefaultProgram");
            }
            this.Hide();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtUser.Text = "";
            txtPassword.Text = "";
            tableLayoutLogin.Hide();
        }

        private void btnDefectAdidas_Click(object sender, EventArgs e)
        {
            MainForm mf = new MainForm();
            // mf.treeView1.Visible = false;
            mf.splitContainer1.Panel1Collapsed = true;
            mf.ribbonStrip.Visible = false;
            mf.mainMenuStrip.Visible = false;
            mf.Show();
            UserControl uc;
            if (GlobalFunction.PublicFunction.myIpaddress == "192.168.0.85" || GlobalFunction.PublicFunction.myIpaddress == "192.168.1.197")
            {
                uc = new QIP.EOL.frmTMC7036_New();
            }
            else
            {
                uc = new QIP.EOL.frmTMC7036();
            }
            if (!pubFunc.OpenUserControl(uc, "Defect Stockfit (frmTMC7036)", "frmTMC7036", mf.tabControl))
            {
                MessageBox.Show("Sorry ! You don’t have permission to open this program", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                uc.Dispose();
                mf.Hide();
                this.Focus();
                return;
            }
            if (MessageBox.Show("Do you want to set this program default open on this machine ?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                new GlobalFunction.PublicFunction().WriteToFile("TouchDefectSTF", "DefaultProgram");
            }
            this.Hide();
        }

        //private void btnDefectAdidas_Click(object sender, EventArgs e)
        //{

        //}
    }
}
