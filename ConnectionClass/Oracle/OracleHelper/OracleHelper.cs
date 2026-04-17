using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using global::Oracle.ManagedDataAccess.Client;
using global::Oracle.ManagedDataAccess.Types;
using System.Collections;

namespace ConnectionClass.Oracle.OracleHelper
{
    public enum DBTYPE
    {
        ORACLE,
    };

    public class OracleHelper
    {
        private DBTYPE dbType;
        private ConnectionString ConStr;
        private DataSet dsResult;
        private DataTable dtResult;
        private string strResult;
        public string strConnectionString;
        private OracleClass.QueryString ORAuseQuery;
        private OracleClass.StoredProcedures ORAuseProc;
        public OracleConnection conOracleConnection;
        public OracleTransaction tranOracleTransaction;
        public delegate void OnStartExcute(string query);
        public delegate void OnEndExcute(bool result);
        public delegate void OnExcuteException(Exception ex);
        private OnExcuteException onExcuteExceptionEvent;
        private OnStartExcute onStartExcuteEvent;
        private OnEndExcute onEndExcuteEvent;
        private string AppSettingKey;
        public event OnExcuteException OnExcuteExceptionEvent
        {
            add
            {
                this.onExcuteExceptionEvent += value;
            }
            remove
            {
                this.onExcuteExceptionEvent -= value;
            }
        }
        public event OnStartExcute OnStartExcuteEvent
        {
            add
            {
                this.onStartExcuteEvent += value;
            }
            remove
            {
                this.onStartExcuteEvent -= value;
            }
        }
        public event OnEndExcute OnEndExcuteEvent
        {
            add
            {
                this.onEndExcuteEvent += value;
            }
            remove
            {
                this.onEndExcuteEvent -= value;
            }
        }
        public OracleHelper(DBTYPE _dbtype, string _appsettingkey)
        {
            this.OnEndExcuteEvent += DacProcessing_OnEndExcuteEvent;
            this.OnStartExcuteEvent += DacProcessing_OnStartExcuteEvent;
            this.OnExcuteExceptionEvent += DacProcessing_OnExcuteExceptionEvent;
            this.AppSettingKey = _appsettingkey;
            try
            {
                this.dbType = _dbtype;
                this.ConStr = new ConnectionString(_appsettingkey);
                this.strConnectionString = ConStr.strConString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 사용자 정의 생성자(외부에서 직접적으로 접속문자열을 동적으로 가공할경우, ConnectionString 클래스를 거치지 않고 접속문자열을 바로 부여한다.)
        /// </summary>
        /// <param name="_dbtype">DB타입</param>
        /// <param name="_appsettiongkey">AppSettingKey(null)로 보내준다.</param>
        /// <param name="_strConString">외부에서 부여된 접속문자열</param>
        public OracleHelper(DBTYPE _dbtype, string _appsettingkey, string _strConString)
        {
            this.OnEndExcuteEvent += DacProcessing_OnEndExcuteEvent;
            this.OnStartExcuteEvent += DacProcessing_OnStartExcuteEvent;
            this.OnExcuteExceptionEvent += DacProcessing_OnExcuteExceptionEvent;
            try
            {
                this.dbType = _dbtype;
                if (_appsettingkey != "")       //키값이 있다면, app.config에 등록된 접속 문자열을 사용
                {
                    this.ConStr = new ConnectionString(_appsettingkey);
                    this.strConnectionString = ConStr.strConString;
                }
                else                           //없다면, 지정된 접속문자열을 사용
                {
                    this.strConnectionString = _strConString;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void DacProcessing_OnExcuteExceptionEvent(Exception ex)
        {

        }

        private void DacProcessing_OnStartExcuteEvent(string query)
        {

        }

        private void DacProcessing_OnEndExcuteEvent(bool result)
        {

        }

        #region ### Select Excute Method ###

        #region DataSet
        /// <summary>
        /// 프로시져를 이용한 조회 메소드
        /// [return: DataSet]
        /// </summary>
        /// <param name="_strProcedureName">프로시져명</param>
        /// <param name="_dicparams">파라미터</param>
        /// <returns>DataSet</returns>
        public DataSet DsSelectExcuteWithProcParam(string _strProcedureName, Dictionary<string, object> _dicparams)
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        dsResult = ORAuseProc.GetDataSet(_strProcedureName, _dicparams);
                        break;

                    default:
                        break;

                }

                return dsResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 프로시져를 이용한 조회 메소드
        /// [return: DataSet]
        /// </summary>
        /// <param name="_strProcedureName">프로시져명</param>
        /// <returns></returns>
        public DataSet DsSelectExcuteWithProc(string _strProcedureName)
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        dsResult = ORAuseProc.GetDataSet(_strProcedureName);
                        break;

                    default:
                        break;
                }

                return dsResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 쿼리를 이용한 조회 메소드
        /// [return: DataSet]
        /// </summary>
        /// <param name="_strquery">쿼리</param>
        /// <returns>DataSet</returns>
        public DataSet DsSelectExcuteWithQuery(string _strquery)
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey);
                        dsResult = ORAuseQuery.GetDataSet(_strquery);
                        break;
                    default:
                        break;


                }

                return dsResult;
            }
            catch (Exception ex)
            {
                new DataSet();
                throw ex;
            }
        }
        #endregion

        /// <summary>
        /// Test whether the Oracle connection string can open a connection.
        /// Returns true if open/close succeeds, false if any exception occurs.
        /// </summary>
        public bool TestConnection()
        {
            try
            {
                using (var testConn = new OracleConnection(this.strConnectionString))
                {
                    testConn.Open();
                    testConn.Close();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        #region DataTable
        /// <summary>
        /// 프로시져를 이용한 조회 메소드
        /// [return: DataTable]
        /// </summary>
        /// <param name="_strProcedureName">프로시져명</param>
        /// <param name="_dicparams">파라미터</param>
        /// <returns>DataTable</returns>
        public DataTable DtSelectExcuteWithProc(string _strProcedureName, Dictionary<string, object> _dicparams)
        {
            bool result = false;
            onStartExcuteEvent(_strProcedureName);
            DataTable dtResult = new DataTable();
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        dtResult = ORAuseProc.GetDataTable(_strProcedureName, _dicparams);
                        result = true;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return dtResult;
        }
        /// <summary>
        /// 쿼리를 이용한 조회 메소드
        /// [return: DataTable]
        /// </summary>
        /// <param name="_strquery">쿼리</param>
        /// <returns>DataTable</returns>
        public DataTable DtSelectExcuteWithQuery(string _strquery)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);
            DataTable dtResult = new DataTable();
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey);
                        dtResult = ORAuseQuery.GetDataTable(_strquery);
                        result = true;
                        break;

                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                //throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return dtResult;
        }
        #endregion

        #region string
        /// <summary>
        /// 프로시져를 이용한 조회 메소드
        /// [return: string]
        /// </summary>
        /// <param name="_strProcedureName">프로시져명</param>
        /// <param name="_dicparams">파라미터</param>
        /// <returns>string</returns>
        public string StSelectExcuteWithProc(string _strProcedureName, Dictionary<string, object> _dicparams)
        {
            bool result = false;
            onStartExcuteEvent(_strProcedureName);
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        strResult = (string)ORAuseProc.GetScalar(_strProcedureName, _dicparams);
                        result = true;
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return strResult;
        }

        /// <summary>
        /// 쿼리를 이용한 조회 메소드
        /// [return: string]
        /// </summary>
        /// <param name="_strquery">쿼리</param>
        /// <returns>string</returns>
        public string StSelectExcuteWithQuery(string _strquery)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey);
                        strResult = ORAuseQuery.GetScalar(_strquery).ToString();
                        result = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return strResult;
        }
        public object ObjSelectExcuteWithQuery(string _strquery)
        {
            bool result = false;
            object objResult = null;
            onStartExcuteEvent(_strquery);
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey);
                        objResult = ORAuseQuery.GetScalar(_strquery);
                        result = true;
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return objResult;
        }
        #endregion

        #endregion

        #region ### Insert And Update Excute Method ###
        /// <summary>
        /// 프로시져를 이용한 Insert 및 Update 메소드
        /// </summary>
        /// <param name="_strProcedureName">프로시져명</param>
        /// <param name="_dicparams">파라미터</param>
        /// <return></return>
        public void IUExcuteWithProc(string _strProcedureName, Dictionary<string, object> _dicparams)
        {
            bool result = false;
            onStartExcuteEvent(_strProcedureName);
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        ORAuseProc.Execute(_strProcedureName, _dicparams);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
        }
        public void IUExcuteWithProcReturn(string _strProcedureName, Dictionary<string, object> _dicparams)
        {
            bool result = false;
            onStartExcuteEvent(_strProcedureName);
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        ORAuseProc.ExecuteT(_strProcedureName, _dicparams);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
        }
        /// <summary>
        /// 쿼리를 이용한 Insert 및 Update 메소드
        /// </summary>
        /// <param name="_strquery">쿼리</param>
        /// <return></return>
        public void IUExcuteWithQuery(string _strquery)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey); // fix: thêm AppSettingKey như các hàm SELECT
                        ORAuseQuery.Execute(_strquery);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Substring(0, 9) != "ORA-12543")
                {
                    if (ex.Message.ToString().Substring(0, 9) != "ORA-03113")
                    {
                        throw ex;
                    }
                }
            }
            finally
            {
                onEndExcuteEvent(result);
            }
        }
        public void IUExcuteWithQueryWithParamenter(string _strquery, Dictionary<string, object> dic)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey); // fix: thêm AppSettingKey như các hàm SELECT
                        ORAuseQuery.ExecuteWithParamenter(_strquery, dic);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Substring(0, 9) != "ORA-12543")
                {
                    if (ex.Message.ToString().Substring(0, 9) != "ORA-03113")
                    {
                        throw ex;
                    }
                }
            }
            finally
            {
                onEndExcuteEvent(result);
            }
        }
        public bool IUBlobImageWithQuery(string _strquery, byte[] _image)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);

            bool returnValue = false;
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        //IUBlobImageWithQuery = new SH_Helper.Data.Oracle.UsingQueryString(strConnectionString);
                        returnValue = ORAuseQuery.IUBlobImage(_strquery, _image);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return returnValue;
        }

        public bool IUExcuteWithQueryReturn(string _strquery)
        {
            bool result = false;
            onStartExcuteEvent(_strquery);
            bool returnValue = false;
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseQuery = new OracleClass.QueryString(strConnectionString, AppSettingKey);
                        returnValue = ORAuseQuery.Execute(_strquery);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                if (ex.Message.ToString().Substring(0, 9) != "ORA-12543")
                {
                    if (ex.Message.ToString().Substring(0, 9) != "ORA-03113")
                    {
                        if (ex.Message.ToString().Substring(0, 9) == "ORA-00001")
                        {
                            throw ex;
                        }
                    }
                }
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return returnValue;
        }
        public bool IUExcuteWithProcReturn(string _strProcedureName, Dictionary<string, object> _dicparams, ArrayList data, string FactCD, string Routing, string MDS, string IP, string EMPID)
        {
            bool result = false;
            onStartExcuteEvent(_strProcedureName);
            bool returnValue = false;
            try
            {
                //DB 타입
                switch (this.dbType)
                {

                    case DBTYPE.ORACLE:
                        ORAuseProc = new OracleClass.StoredProcedures(strConnectionString, AppSettingKey);
                        returnValue = ORAuseProc.ExecuteNotCommit(_strProcedureName, _dicparams, data, FactCD, Routing, MDS, IP, EMPID);
                        result = true;
                        break;

                    default:
                        break;

                }
            }
            catch (Exception ex)
            {
                return returnValue;
                throw ex;
            }
            finally
            {
                onEndExcuteEvent(result);
            }
            return returnValue;
        }
        #endregion

        #region ### Passivity Transaction Proce ###

        #region Transaction Beginning
        /// <summary>
        /// 수동 트랜잭션 생성 메소드
        /// </summary>
        public void TranBeginning()
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        conOracleConnection = new OracleConnection(strConnectionString);
                        conOracleConnection.Open();
                        tranOracleTransaction = conOracleConnection.BeginTransaction();
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Transaction Commit
        /// <summary>
        /// 수동트랜잭션 커밋 메소드
        /// </summary>
        public void TranCommit()
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        tranOracleTransaction.Commit();
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Transaction Rollback
        /// <summary>
        /// 수동트랜잭션 롤백 메소드
        /// </summary>
        public void TranRollback()
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        tranOracleTransaction.Rollback();
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Transaction Dispose
        /// <summary>
        /// 수동트랜잭션 킬 메소드
        /// </summary>
        public void TranKill()
        {
            try
            {
                //DB 타입
                switch (this.dbType)
                {
                    case DBTYPE.ORACLE:
                        conOracleConnection.Close();
                        tranOracleTransaction.Dispose();
                        break;

                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #endregion
    }

}

