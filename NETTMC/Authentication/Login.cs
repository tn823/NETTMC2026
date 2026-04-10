using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NETTMC.Authentication
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void btnMas_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Sorry, this function is developing. Please try again later.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        private void btnSecurity_Click(object sender, EventArgs e)
        {
            //MainForm mf = new MainForm();
            //mf.ribbon.Visible = false;
            //mf.Show();
            //XtraUserControl uc1 = new HRMS.Security.frmTMC7099();
            //XtraUserControl uc2 = new HRMS.Security.frmTMC7098();
            //if (!pubFunc.OpenUserControl(new HRMS.Security.frmTMC7099(), "Security Registration", "Security Registration", mf.tabControl))
            //{
            //    MessageBox.Show("Sorry ! You don’t have permission to open this program frmTMC7099", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    mf.Hide();
            //    uc1.Dispose();
            //    uc2.Dispose();
            //    this.Focus();
            //    return;
            //}
            //if (!pubFunc.OpenUserControl(new HRMS.Security.frmTMC7098(), "Log Book", "Log Book", mf.tabControl))
            //{
            //    MessageBox.Show("Sorry ! You don’t have permission to open this program frmTMC7098", "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            //    mf.Hide();
            //    this.Focus();
            //    return;
            //}
            //this.Hide();
        }
        public GlobalFunction.PublicFunction pubFunc;
        private void btnDefectAdidas_Click(object sender, EventArgs e)
        {
            MainForm mf = Application.OpenForms["MainForm"] as MainForm;

            if (mf != null)
            {
                mf.MainMenuStrip.Visible = false;
               
            }

            UserControl uc;

            if (GlobalFunction.PublicFunction.myIpaddress == "192.168.0.85")
            {
                uc = new QIP.EOL.frmTMC7036_New();
            }
            else
            {
               uc = new QIP.EOL.frmTMC7036_New();
            }

            if (!pubFunc.OpenUserControl(uc, "Defect Stockfit (frmTMC7036)", "frmTMC7036", mf.tabControl))
            {
                MessageBox.Show("Sorry ! You don’t have permission to open this program",
                    "Security", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                uc.Dispose();
                mf.Hide();
                this.Focus();
                return;
            }

            this.Hide();
        }
    }
}
