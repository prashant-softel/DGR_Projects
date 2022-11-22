using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Models;

namespace DGRAPIs.Repositories
{
    public class LoginRepository : GenericRepository
    {
        private int approve_status = 0;
        public LoginRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {

        }
      //Login 
        internal async Task<UserLogin> GetUserLogin(string username, string password)
        {
            string qry = "";
            qry = "SELECT * FROM `login` where `username`='" + username + "' and `password` ='" + password + "' ;";
            // Console.WriteLine(qry);
           var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            return _UserLogin.FirstOrDefault();
           // return _UserLogin;
          //  return await Context.GetData(qry).ConfigureAwait(false);

        }
        internal async Task<int> WindUserRegistration(string fname, string useremail, string role, string created_on)
        {
            string qry = "insert into login (`username`,`useremail`,`user_role`,`created_on`) VALUES('"+ fname + "','" + useremail + "','"+ role + "','"+ created_on + "')";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        
        public async Task<List<UserInfomation>> GetWindUserInformation(int  login_id)
        {
            string filter = "";
            if (login_id != 0)
            {
                filter = " where login_id='"+ login_id + "'";
            }
            string qry = "";
            qry = "SELECT login_id,username,useremail,user_role,created_on,blocked_user FROM `login` "+ filter;
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<UserInfomation> _country = new List<UserInfomation>();
            _country = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _country;

            //  return await Context.GetData(qry).ConfigureAwait(false);

        }

        //  await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }


    }

}
