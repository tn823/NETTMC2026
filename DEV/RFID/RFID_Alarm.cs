using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static GlobalFunction.PublicFunction;



namespace DEV.RFID
{
    public partial class RFID_Alarm : UserControl
    {
       

        public RFID_Alarm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            GlobalFunction.PublicFunction pub = new GlobalFunction.PublicFunction();
            //pub.ExportToExcelFile(gridControl2, gridControl2, "Sheet1");  
        }
    }
}
