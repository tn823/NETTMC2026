using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectionClass.Oracle;
namespace GlobalFunction.Authentication
{
    public static class Permission
    {
        public class UserInformation
        {

            private static string userid;
            private static string username;
            private static string userrole;
            private static string roleid;

            public static string UserID
            {
                get
                {
                    return userid;
                }
                
            }
            public static string UserName
            {
                get
                {
                    return username;
                }

            }
            public static string UserRole
            {
                get
                {
                    return userrole;
                }

            }

            public static string RoleID
            {
                get
                {
                    return roleid;
                }

            }

            public static bool CheckPermissionOpenForm(string roleID, string AccessibleName)
            {
                try
                {
                    CRUDOracle crud = new CRUDOracle("VSMES");
                    DataTable dt = crud.dac.DtSelectExcuteWithQuery("SELECT * FROM QIP.TBC_ROLEPGMBTNID WHERE ROLE_ID = '" + roleID + "' AND PGM_ID = '" + AccessibleName + "' ");
                    if (dt.Rows.Count == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;

                    }
                }
                catch
                {
                    return false;
                }
            }
            public static bool BindUser(string id)
            {
                try
                {
                    CRUDOracle crud = new CRUDOracle("VSMES");
                    DataTable dt = new DataTable();
                    dt = crud.dac.DtSelectExcuteWithQuery("Select A.EMP_ID,A.EMP_NM,B.ROLE_ID,C.ROLE_NM " +
                        " from QIP.TBC_USERINFO A , QIP.TBC_USEROLE B, QIP.TBC_ROLEINFO C " +
                        " WHERE A.EMP_ID = B.EMP_ID AND B.ROLE_ID = C.ROLE_ID" +
                        " AND A.EMP_ID = '" + id + "'");
                    if (dt.Rows.Count > 0)
                    {
                        GlobalFunction.Authentication.Permission.UserInformation user = new GlobalFunction.Authentication.Permission.UserInformation();
                        userid = dt.Rows[0]["EMP_ID"].ToString();
                        username = dt.Rows[0]["EMP_NM"].ToString();
                        userrole = dt.Rows[0]["ROLE_NM"].ToString();
                        roleid = dt.Rows[0]["ROLE_ID"].ToString();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    return false ;
                }
            }
        }
        


    }
}
    
