using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
namespace ConnectionClass.Oracle.OracleClass
{
    public class QueryString
    {

        protected OracleConnection conOracleConnection;
        protected OracleCommand cmdOracleCommand;
        protected OracleDataAdapter daOracleDataAdapter;
        protected OracleTransaction tranOracleTransaction;
        protected static string strConnection;

        /// <summary>
        /// UsingQueryString 생성자
        /// </summary>
        ///<remarks>
        /// OracleConnection 객체 생성
        /// web.config 또는 App.config 파일에서 "ConnectionString"이란 이름으로 DB 연결 문자열 지정
        ///</remarks>
        public QueryString(string strConnection)
        {
            QueryString.strConnection = ConfigurationManager.AppSettings["ConnectionString"];
        }
        /// <summary>
        /// UsingQueryString 생성자
        /// </summary>
        ///<remarks>
        /// OracleConnection 객체 생성z
        /// 사용자입력한 연결문자열 할당 
        ///</remarks>
        public QueryString(string strConnectionString, string AppKeySetting)
        {
            if (AppKeySetting == "VSMES")
            {
                QueryString.strConnection = "User Id=MES;Password=MES;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.32)(PORT=1521))(CONNECT_DATA=(SID=VMES)));";
            }
            else if (AppKeySetting == "HRMS")
            {
                QueryString.strConnection = "Data Source=VSTIDB;User Id=HRMS;Password=HRMS;Integrated Security=no";
            }
            else if (AppKeySetting == "APPS")
            {
                QueryString.strConnection = "Data Source=VSTIDB;User Id=APPS;Password=APPS;Integrated Security=no";
            }
            else if (AppKeySetting == "AGMES")
            {
                QueryString.strConnection = "Data Source=AGMES;User Id=MES;Password=MES;Integrated Security=no";
            }
            else if (AppKeySetting == "AGHRMS")
            {
                QueryString.strConnection = "Data Source=AGERP;User Id=HRMS;Password=HRMS;Integrated Security=no";
            }
            else if (AppKeySetting == "AGAPPS")
            {
                QueryString.strConnection = "Data Source=AGERP;User Id=APPS;Password=APPS;Integrated Security=no";
            }
            else if (AppKeySetting == "ITAMS")
            {
                //QueryString.strConnection = ConfigurationManager.AppSettings["ConnectionString"];
                QueryString.strConnection = "User Id=MES;Password=mes;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.32)(PORT=1521))(CONNECT_DATA=(SID=vmes)));";
            
            }
        }
        /// <summary>
        /// 커넥션,커맨드,어댑터 객체 생성
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        protected void Initialize(string strquerystring)
        {
            try
            {
                conOracleConnection = new OracleConnection(strConnection);
                cmdOracleCommand = new OracleCommand(strquerystring, conOracleConnection);
                daOracleDataAdapter = new OracleDataAdapter(cmdOracleCommand);
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 데이터관련 객체 리소스 해제
        /// </summary>
        protected void Kill()
        {
            try
            {
                if (this.conOracleConnection.State.ToString() == "Open")
                {
                    this.conOracleConnection.Close();
                }
                if (this.cmdOracleCommand != null)
                {
                    this.cmdOracleCommand.Dispose();
                }
                if (this.daOracleDataAdapter != null)
                {
                    this.daOracleDataAdapter.Dispose();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        /// <summary>
        /// QueryString을 사용하여 스칼라 값(object) 반환 
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        /// <returns> object </returns>
        public object GetScalar(string strquerystring)
        {
            Initialize(strquerystring);

            cmdOracleCommand.CommandType = CommandType.Text;

            object objReturn;

            try
            {
                conOracleConnection.Open();
                objReturn = cmdOracleCommand.ExecuteScalar();
                return objReturn;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Kill();

            }
        }

        /// <summary>
        /// QueryString을 사용하여 DataReader 반환
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        /// <returns> DataReader </returns>
        public OracleDataReader GetDataReader(string strquerystring)
        {
            Initialize(strquerystring);

            cmdOracleCommand.CommandType = CommandType.Text;

            try
            {
                conOracleConnection.Open();

                return cmdOracleCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// QueryString을 사용하여 DataTable 반환
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        /// <returns> DataTable </returns>
        public DataTable GetDataTable(string strquerystring)
        {
            Initialize(strquerystring);
            cmdOracleCommand.CommandType = CommandType.Text;
            DataTable dtReturn = new DataTable();

            try
            {
                daOracleDataAdapter.Fill(dtReturn);
                return dtReturn;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Kill();
            }
        }
        /// <summary>
        /// QueryString을 사용하여 DataSet 반환
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        /// <returns> DataSet </returns>
        public DataSet GetDataSet(string strquerystring)
        {
            Initialize(strquerystring);

            cmdOracleCommand.CommandType = CommandType.Text;

            DataSet dsReturn = new DataSet();

            try
            {
                daOracleDataAdapter.Fill(dsReturn);
                return dsReturn;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.Kill();
            }
        }
        /// <summary>
        /// QueryString을 Insert Update Image
        /// Value In QuerryString Must Be Like "Update TB_Image Set ImageColumn = :Blob where ID = '" & txtID & "'"
        /// </summary>
        /// <param name="strquerystring","image"> QueryString </param>
        /// <returns> bool  </returns>
        public bool IUBlobImage(string strquerystring, byte[] image)
        {
            try
            {
                int intResult;
                conOracleConnection = new OracleConnection(strConnection);
                cmdOracleCommand = new OracleCommand(strquerystring, conOracleConnection);
                cmdOracleCommand.Transaction = tranOracleTransaction;
                conOracleConnection.Open();
                OracleParameter blobpara = new OracleParameter();
                blobpara.OracleDbType = OracleDbType.Blob;
                blobpara.ParameterName = "Blob";
                blobpara.Value = image;
                cmdOracleCommand = new OracleCommand(strquerystring, conOracleConnection);
                cmdOracleCommand.Parameters.Add(blobpara);
                intResult = cmdOracleCommand.ExecuteNonQuery();
                if (intResult > 0)
                    return true;
                else
                    return false;

            }
            catch (Exception e)
            {
                tranOracleTransaction.Rollback();
                throw e;
            }
            finally
            {
                this.Kill();
            }

        }
        /// <summary>
        /// QueryString을 사용하여 데이터를 입력, 수정, 삭제하고 성공여부 반환
        /// </summary>
        /// <param name="strquerystring"> QueryString </param>
        /// <returns> bool  </returns>
        public bool Execute(string strquerystring)
        {
            Initialize(strquerystring);
            cmdOracleCommand.CommandType = CommandType.Text;
            int intResult;
            // Transaction 처리
            conOracleConnection.Open();
            tranOracleTransaction = conOracleConnection.BeginTransaction();
            cmdOracleCommand.Transaction = tranOracleTransaction;

            try
            {
                intResult = cmdOracleCommand.ExecuteNonQuery();
                tranOracleTransaction.Commit();

                if (intResult > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                tranOracleTransaction.Rollback();
                throw e;
            }
            finally
            {
                this.Kill();
            }
        }
        public bool ExecuteWithParamenter(string strquerystring, Dictionary<string, object> dic)
        {
            Initialize(strquerystring);
            cmdOracleCommand.CommandType = CommandType.Text;
            int intResult;
            // Transaction 처리
            conOracleConnection.Open();
            tranOracleTransaction = conOracleConnection.BeginTransaction();
            cmdOracleCommand.Transaction = tranOracleTransaction;

            foreach (var kvp in dic)
            {
                cmdOracleCommand.Parameters.Add(kvp.Key.ToString(), kvp.Value);
            }
            try
            {
                intResult = cmdOracleCommand.ExecuteNonQuery();
                tranOracleTransaction.Commit();

                if (intResult > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                tranOracleTransaction.Rollback();
                throw e;
            }
            finally
            {
                this.Kill();
            }
        }
    }
}
