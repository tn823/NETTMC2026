using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionClass.Oracle
{
    public class Site
    {
        public static string ConnectionSite
        {
            get;
            set;
        }
        public static string FTPPath
        {
            get
            {
                return FTPServerIP();
            }
        }
        public static string FTPServerIP()
        {
            if(ConnectionSite == "VS")
            {
                return "192.168.1.15";
            }
            else
            {
                return "192.168.100.21";
            }
        }
    }
}
