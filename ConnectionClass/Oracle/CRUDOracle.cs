using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectionClass.Oracle
{
    public class CRUDOracle
    {
        public OracleHelper.OracleHelper dac;
        public OracleHelper.asyncOracleHelper asyncdac;
       
        public bool ConnectionStatus()
        {
            try
            {
                if (dac != null)
                {
                    string test = "";
                    test = dac.StSelectExcuteWithQuery("Select sysdate from dual");
                    if (test == "")
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public string ConnectionStringNetTMC()
        {
            return "";
        }
        public CRUDOracle(string connection)
        {
            if (connection == "VSMES")
            {
                if(ConnectionClass.Oracle.Site.ConnectionSite == "VS")
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "VSMES");
                }
                else
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "AGMES");
                }
                asyncdac = new OracleHelper.asyncOracleHelper();
            }
            else if (connection == "HRMS")
            {
                if (ConnectionClass.Oracle.Site.ConnectionSite == "VS")
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "HRMS");
                }
                else
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "AGHRMS");
                }
                asyncdac = new OracleHelper.asyncOracleHelper();
            }
            else if (connection == "APPS")
            {
                if (ConnectionClass.Oracle.Site.ConnectionSite == "VS")
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "APPS");
                }
                else
                {
                    dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "AGAPPS");
                }
                asyncdac = new OracleHelper.asyncOracleHelper();
            }
            else if (connection == "ITAMS")
            {
                dac = new OracleHelper.OracleHelper(OracleHelper.DBTYPE.ORACLE, "ConnectionString");
                asyncdac = new OracleHelper.asyncOracleHelper();
            }
            else
            { }
        }

    }
}
