using DGRAPIs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Repositories;
using DGRAPIs.Models;

namespace DGRAPIs.BS
{
    public interface iLoginBS
    {
        Task<int> eQry(string qry);
         Task<UserLogin> GetUserLogin(string username, string password);
      
        Task<int> WindUserRegistration(string fname, string useremail, string role, string created_on);
      
        Task<List<UserInfomation>> GetWindUserInformation(int login_id);

    }
    public class LoginBS : iLoginBS
    {
        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
        public LoginBS(DatabaseProvider dbProvider)
        {
            databaseProvider = dbProvider;
        }
      
        public async Task<UserLogin> GetUserLogin(string username, string password)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetUserLogin(username, password);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> WindUserRegistration(string fname, string useremail, string role, string created_on)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.WindUserRegistration(fname, useremail, role, created_on);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
       
        public async Task<List<UserInfomation>> GetWindUserInformation(int login_id)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetWindUserInformation(login_id);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> eQry(string qry)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.eQry(qry);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
