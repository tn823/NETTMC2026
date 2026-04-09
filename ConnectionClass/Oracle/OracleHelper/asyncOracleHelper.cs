using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace ConnectionClass.Oracle.OracleHelper
{
    public class asyncOracleHelper
    {
        private OracleHelper dac;
        public BackgroundWorker doAsync_BackWork;

        public asyncOracleHelper()
        {
            dac = new OracleHelper(DBTYPE.ORACLE, "ConnectionString");
            this.doAsync_BackWork = new BackgroundWorker();
            this.doAsync_BackWork.DoWork += doAsync_BackWork_DoWork;
            try
            {

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void doAsync_BackWork_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Dictionary<string, object> dictionary = e.Argument as Dictionary<string, object>;

                var method = dictionary.Where(x => x.Key == "method").Select(x => x.Value.ToString()).FirstOrDefault();
                var parameter = dictionary.Where(x => x.Key == "parameter").Select(x => x.Value).FirstOrDefault();
                var callback_method = dictionary.Where(x => x.Key == "callback_method").Select(x => x.Value).FirstOrDefault();
                var classname = dictionary.Where(x => x.Key == "classname").Select(x => x.Value).FirstOrDefault();

                Type thisType = dac.GetType();

                Dictionary<string, object> retunValue = new Dictionary<string, object>();
                List<object> result = new List<object>();
                MethodInfo theMethod = thisType.GetMethod(method.ToString());
                if (parameter != null)
                    result.Add(theMethod.Invoke(dac, (parameter as List<object>).ToArray()));
                else
                    result.Add(theMethod.Invoke(dac, null));

                retunValue.Add("classname", classname.ToString());
                retunValue.Add("callback_method", callback_method.ToString());
                retunValue.Add("result", result);
                e.Result = retunValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

