using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ConnectionClass.Oracle;
using System.Diagnostics;

namespace SystemControl
{
    public partial class CheckStock : UserControl
    {

        private DataTable dtStockScan;
        private DataTable dtWrongdate;
        private DataTable dtStockScanGroup;

        CRUDOracle crud;

        public CheckStock()
        {
            InitializeComponent();
        }

        private void CheckStock_Load(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
