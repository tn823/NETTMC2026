using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using System.Data;
using System.Collections;

namespace ConnectionClass.Oracle.OracleClass
{
    public class StoredProcedures
    {
        protected OracleConnection conOracleConnection;		// 데이터 커넥션
        protected OracleCommand cmdOracleCommand;			// 데이터 커맨드
        protected OracleDataAdapter daOracleDataAdapter;	// 데이터 어댑터
        protected OracleTransaction tranOracleTransaction;  // 트랜잭션
        protected static string strConnection = "";         // DB 연결 문자열

        /// <summary>
        /// UsingStoredProcedure 생성자
        /// </summary>
        ///<remarks>
        /// OracleConnection 객체 생성
        /// web.config 또는 App.config 파일에서 "ConnectionString"이란 이름으로 DB 연결 문자열 지정
        ///</remarks>
        public StoredProcedures()
        {
            strConnection = ConfigurationManager.AppSettings["ConnectionString"];
        }

        /// <summary>
        /// UsingStoredProcedure 생성자
        /// </summary>
        ///<remarks>
        /// OracleConnection 객체 생성
        /// 사용자입력한 연결문자열 할당 
        ///</remarks>
        public StoredProcedures(string strConnectionString, string AppKeySetting)
        {
            if (AppKeySetting == "VSMES")
            {
                strConnection = "User Id=MES;Password=MES;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.1.32)(PORT=1521))(CONNECT_DATA=(SID=VMES)));";
            }
            else if (AppKeySetting == "HRMS")
            {
                strConnection = "Data Source=VSTIDB;User Id=HRMS;Password=HRMS;Integrated Security=no";
            }
            else if (AppKeySetting == "APPS")
            {
                strConnection = "Data Source=VSTIDB;User Id=APPS;Password=APPS;Integrated Security=no";
            }
            else if (AppKeySetting == "AGMES")
            {
                strConnection = "Data Source=AGMES;User Id=MES;Password=MES;Integrated Security=no";
            }
            else if (AppKeySetting == "AGHRMS")
            {
                strConnection = "Data Source=AGERP;User Id=HRMS;Password=HRMS;Integrated Security=no";
            }
            else if (AppKeySetting == "AGAPPS")
            {
                strConnection = "Data Source=AGERP;User Id=APPS;Password=APPS;Integrated Security=no";
            }
        }

        /// <summary>
        /// 커넥션,커맨드,어댑터 객체 생성
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure 명 </param>
        protected void Initialize(string strstoredprocedure)
        {
            conOracleConnection = new OracleConnection(strConnection);
            cmdOracleCommand = new OracleCommand(strstoredprocedure, conOracleConnection);
            daOracleDataAdapter = new OracleDataAdapter(cmdOracleCommand);
        }

        /// <summary>
        /// Dictionary에 담겨있는 파라미터를 SqlCommand에 ADD
        /// </summary>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        private void AddParams(Dictionary<string, object> dicparams)
        {
            foreach (KeyValuePair<string, object> k in dicparams)
            {
                cmdOracleCommand.Parameters.Add(k.Key, k.Value.ToString());
            }
        }
        private void AddOracleParams(Dictionary<string, object> dicparams)
        {
            foreach (KeyValuePair<string, object> k in dicparams)
            {
                cmdOracleCommand.Parameters.Add(k.Value as OracleParameter);
            }
        }
        private void AddOracleParamsT(Dictionary<string, object> dicparams)
        {
            foreach (KeyValuePair<string, object> k in dicparams)
            {
                object[] dicValue = (object[])k.Value;
                ParameterDirection dir = (ParameterDirection)dicValue[0];
                object value = dicValue[1];          // ← value
                OracleDbType dbType = (OracleDbType)dicValue[2];

                var param = cmdOracleCommand.Parameters.Add(k.Key, dbType);
                param.Direction = dir;
                param.Value = value ?? DBNull.Value;      // ← set value, null-safe
            }
        }
        private void AddOracleParamsOutput(Dictionary<string, object> dicparams)
        {
            foreach (KeyValuePair<string, object> k in dicparams)
            {
                cmdOracleCommand.Parameters.Add(k.Value as OracleParameter);
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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 스칼라 값(object) 반환 
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <returns> object </returns>
        public object GetScalar(string strstoredprocedure)
        {
            Initialize(strstoredprocedure);

            cmdOracleCommand.CommandType = CommandType.StoredProcedure;

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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 스칼라 값(object) 반환 
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        /// <returns> object </returns>
        public object GetScalar(string strstoredprocedure, Dictionary<string, object> dicparams)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            AddParams(dicparams);

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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataReader 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <returns> DataReader </returns>
        public OracleDataReader GetDataReader(string strstoredprocedure)
        {

            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;

            try
            {
                conOracleConnection.Open();

                return cmdOracleCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Kill();
            }
        }

        /// <summary>
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataReader 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        /// <returns> DataReader </returns>
        public OracleDataReader GetDataReader(string strstoredprocedure, Dictionary<string, object> dicparams)
        {

            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            AddParams(dicparams);

            try
            {
                conOracleConnection.Open();

                return cmdOracleCommand.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //this.Kill();
            }
        }

        /// <summary>
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataTable 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <returns> DataTable </returns>
        public DataTable GetDataTable(string strstoredprocedure)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            // 반환될 DataTable 초기화
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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataTable 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        /// <returns> DataTable </returns>
        public DataTable GetDataTable(string strstoredprocedure, Dictionary<string, object> dicparams)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            //AddParams(dicparams);
            AddOracleParams(dicparams);

            // 반환될 DataTable 초기화
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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataSet 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <returns> DataSet </returns>
        public DataSet GetDataSet(string strstoredprocedure)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            // 반환될 DataSet 초기화
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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 DataSet 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        /// <returns> DataSet </returns>
        public DataSet GetDataSet(string strstoredprocedure, Dictionary<string, object> dicparams)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            //AddParams(dicparams);
            AddOracleParams(dicparams);

            // 반환될 DataSet 초기화
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
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 데이터를 입력, 수정, 삭제하고 성공여부 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <returns> bool </returns>
        public bool Execute(string strstoredprocedure)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            // ExecuteNonQuery() 이후에 반환될 값(0 or 1)
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

        /// <summary>
        /// StoredProcedure의 이름과 그곳에 사용될 파라미터들을 사용하여 데이터를 입력, 수정, 삭제하고 성공여부 반환
        /// </summary>
        /// <param name="strstoredprocedure"> StoredProcedure명 </param>
        /// <param name="dicparams"> StoredProcedure에서 사용되는 파라미터 Dictionary</param>
        /// <returns> bool </returns>
        public bool Execute(string strstoredprocedure, Dictionary<string, object> dicparams)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            //AddParams(dicparams);
            if (dicparams != null)
            { AddParams(dicparams); }
            // ExecuteNonQuery() 이후에 반환될 값(0 or 1)
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
        public bool ExecuteT(string strstoredprocedure, Dictionary<string, object> dicparams)
        {
            Initialize(strstoredprocedure);
            cmdOracleCommand.CommandType = CommandType.StoredProcedure;
            //AddParams(dicparams);
            if (dicparams != null)
            { AddOracleParamsT(dicparams); }
            // ExecuteNonQuery() 이후에 반환될 값(0 or 1)
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
        public bool ExecuteNotCommit(string strstoredprocedure, Dictionary<string, object> dicparams, ArrayList data, string FactCD, string Routing, string MDS, string IP, string EMPID)
        {
            Initialize(strstoredprocedure);
            bool returnValue = false;
            int intResult;
            for (int i = 0; i < data.Count; i++)
            {
                DataRow row = data[i] as DataRow;
                dicparams.Clear();
                OracleParameter P_FACT_CD = new OracleParameter();
                OracleParameter P_ROUTING_CD = new OracleParameter();
                OracleParameter P_LINE_CD = new OracleParameter();
                OracleParameter P_ORDER_NO = new OracleParameter();
                OracleParameter P_ORDER_SLIP = new OracleParameter();
                OracleParameter P_PO_DOC_DATE = new OracleParameter();
                OracleParameter P_STYLE_NO = new OracleParameter();
                OracleParameter P_VSP_DATE = new OracleParameter();
                OracleParameter P_ORD_TYPE = new OracleParameter();
                OracleParameter P_MDS_NAME = new OracleParameter();
                OracleParameter P_CREATE_ID = new OracleParameter();
                OracleParameter P_CREATE_IP = new OracleParameter();

                P_FACT_CD.OracleDbType = OracleDbType.Varchar2;
                P_FACT_CD.Value = FactCD;
                P_FACT_CD.ParameterName = "P_FACT_CD";
                dicparams.Add("P_FACT_CD", P_FACT_CD);

                P_ROUTING_CD.OracleDbType = OracleDbType.Varchar2;
                P_ROUTING_CD.Value = Routing;
                P_ROUTING_CD.ParameterName = "P_ROUTING_CD";
                dicparams.Add("P_ROUTING_CD", P_ROUTING_CD);

                P_LINE_CD.OracleDbType = OracleDbType.Varchar2;
                P_LINE_CD.Value = row["LINE"].ToString();
                P_LINE_CD.ParameterName = "P_LINE_CD";
                dicparams.Add("P_LINE_CD", P_LINE_CD);

                P_ORDER_NO.OracleDbType = OracleDbType.Decimal;
                P_ORDER_NO.Value = row["ORDER_NO"].ToString();
                P_ORDER_NO.ParameterName = "P_ORDER_NO";
                dicparams.Add("P_ORDER_NO", P_ORDER_NO);

                P_ORDER_SLIP.OracleDbType = OracleDbType.Decimal;
                P_ORDER_SLIP.Value = row["ORDER_SLIP"].ToString();
                P_ORDER_SLIP.ParameterName = "P_ORDER_SLIP";
                dicparams.Add("P_ORDER_SLIP", P_ORDER_SLIP);


                P_PO_DOC_DATE.OracleDbType = OracleDbType.Varchar2;
                P_PO_DOC_DATE.Value = row["PO_DOC_DATE"].ToString();
                P_PO_DOC_DATE.ParameterName = "P_PO_DOC_DATE";
                dicparams.Add("P_PO_DOC_DATE", P_PO_DOC_DATE);

                P_STYLE_NO.OracleDbType = OracleDbType.Varchar2;
                P_STYLE_NO.Value = row["STYLE_NO"].ToString();
                P_STYLE_NO.ParameterName = "P_STYLE_NO";
                dicparams.Add("P_STYLE_NO", P_STYLE_NO);

                P_VSP_DATE.OracleDbType = OracleDbType.Varchar2;
                P_VSP_DATE.Value = Convert.ToDateTime(row["VSP_DATE"].ToString()).ToString("yyyyMMdd");
                P_VSP_DATE.ParameterName = "P_VSP_DATE";
                dicparams.Add("P_VSP_DATE", P_VSP_DATE);

                P_MDS_NAME.OracleDbType = OracleDbType.Varchar2;
                P_MDS_NAME.Value = MDS;
                P_MDS_NAME.ParameterName = "P_MDS_NAME";
                dicparams.Add("P_MDS_NAME", P_MDS_NAME);

                P_CREATE_ID.OracleDbType = OracleDbType.Varchar2;
                P_CREATE_ID.Value = EMPID;
                P_CREATE_ID.ParameterName = "P_CREATE_ID";
                dicparams.Add("P_CREATE_ID", P_CREATE_ID);

                P_CREATE_IP.OracleDbType = OracleDbType.Varchar2;
                P_CREATE_IP.Value = IP;
                P_CREATE_IP.ParameterName = "P_CREATE_IP";
                dicparams.Add("P_CREATE_IP", P_CREATE_IP);

                cmdOracleCommand.CommandType = CommandType.StoredProcedure;
                AddOracleParams(dicparams);
                if (conOracleConnection.State == ConnectionState.Closed)
                    conOracleConnection.Open();

                tranOracleTransaction = conOracleConnection.BeginTransaction();
                cmdOracleCommand.Transaction = tranOracleTransaction;

                try
                {
                    intResult = cmdOracleCommand.ExecuteNonQuery();
                    if (intResult > 0)
                        returnValue = true;
                    else
                        returnValue = false;
                }
                catch (Exception e)
                {
                    returnValue = false;
                    tranOracleTransaction.Rollback();
                    throw e;
                }
            }
            if (returnValue == false)
            {
                tranOracleTransaction.Rollback();
                return returnValue;
            }
            else
            {
                tranOracleTransaction.Commit();
                returnValue = true;
                this.Kill();
                return returnValue;
            }


        }
    }
}
