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
        //Task<List<UserLogin>> GetUserLogin(string username, string password);
        Task<UserLogin> GetUserLogin(string username, string password,bool isSSO);
        Task<int> UpdateLoginStatus(int UserID);
        Task<int> DirectLogOut(int UserID);
        Task<int> WindUserRegistration(string fname, string useremail, string role, string userpass);
        Task<int> UpdatePassword(int loginid, string updatepass);
        Task<int> DeactivateUser(int loginid);
        Task<int> ActivateUser(int loginid);
        Task<int> DeleteUser(int loginid);
        Task<List<UserInfomation>> GetWindUserInformation(int login_id);
        Task<List<UserInfomation>> GetSolarUserInformation(int login_id);
        Task<List<HFEPage>> GetPageList(int login_id, int site_type);
        Task<List<UserAccess>> GetWindUserAccess(int login_id,string role);

        Task<int> SubmitUserAccess(int login_id, string siteList, string pageList, string reportList, string site_type, int importapproval);
        //GetUserLoginId
        Task<List<UserInfomation>> GetUserLoginId(string username, string useremail);

        //SubmitCloneUserAccess
        Task<int> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access);


    }
    public class LoginBS : iLoginBS
    {
        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
        public LoginBS(DatabaseProvider dbProvider)
        {
            databaseProvider = dbProvider;
        }

        public async Task<int> UpdateLoginStatus(int UserID)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.UpdateLoginStatus(UserID);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> DirectLogOut(int UserID)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.DirectLogOut(UserID);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<int> UpdatePassword(int loginid, string updatepass)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.UpdatePassword(loginid, updatepass);

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<int> DeactivateUser(int loginid)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.DeactivateUser(loginid);

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        //ActivateUser
        public async Task<int> ActivateUser(int loginid)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.ActivateUser(loginid);

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        //DeleteUser
        public async Task<int> DeleteUser(int loginid)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.DeleteUser(loginid);

                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
        //public async Task<List<UserLogin>> GetUserLogin(string username, string password)
        public async Task<UserLogin> GetUserLogin(string username, string password, bool isSSO)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetUserLogin(username, password, isSSO);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> SubmitUserAccess(int login_id, string siteList, string pageList, string reportList, string site_type,int importapproval)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.SubmitUserAccess(login_id, siteList, pageList, reportList, site_type, importapproval);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //GetUserLoginId
        public async Task<List<UserInfomation>> GetUserLoginId(string username, string useremail)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetUserLoginId(username, useremail);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //SubmitCloneUserAccess
        public async Task<int> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.SubmitCloneUserAccess(login_id, site_type, page_type, identity, upload_access);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<int> WindUserRegistration(string fname, string useremail, string role, string userpass)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.WindUserRegistration(fname, useremail, role, userpass);

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
        public async Task<List<UserInfomation>> GetSolarUserInformation(int login_id)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetSolarUserInformation(login_id);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<HFEPage>> GetPageList(int login_id,int site_type)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetPageList(login_id, site_type);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<List<UserAccess>> GetWindUserAccess(int login_id,string role)
        {
            try
            {
                using (var repos = new LoginRepository(getDB))
                {
                    return await repos.GetWindUserAccess(login_id,role);

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
