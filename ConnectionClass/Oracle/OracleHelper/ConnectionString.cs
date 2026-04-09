using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ConnectionClass.Oracle.OracleHelper
{
    class ConnectionString
    {
        private string strAppSettingKey;
        public string strConString
        {
            set { }
            get
            {
                return ConfigurationManager.AppSettings[strAppSettingKey];
            }
        }

        public ConnectionString(string _appsettingkey)
        {
            try
            {
                this.strAppSettingKey = _appsettingkey;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
