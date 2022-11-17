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
           // qry = "SELECT * FROM `login` where `username`='sujitkumar@gmail.com' and `password` ='sujit123' ;";
            // Console.WriteLine(qry);
            var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            return _UserLogin.FirstOrDefault();
           // return _UserLogin;
          //  return await Context.GetData(qry).ConfigureAwait(false);

        }
       
        //  await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }


    }

}
