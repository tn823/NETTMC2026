using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QIP.EOL
{
    public partial class frmTMC7032_MsgAlarm : Form
    {
        public string IPADDRESS
        {
            get;
            set;
        }
        public string MessageText
        {
            get;
            set;
        }
        public bool IsStopLine
        {
            get;
            set;
        }
        public frmTMC7032_MsgAlarm()
        {
            InitializeComponent();
        }
    }
}
