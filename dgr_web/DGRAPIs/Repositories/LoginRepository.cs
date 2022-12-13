using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Models;
using Nancy.Json;

namespace DGRAPIs.Repositories
{
    public class LoginRepository : GenericRepository
    {
        private int approve_status = 0;
        private object json;

        public LoginRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {

        }
        //Login 

        //internal async Task<List<UserLogin>> GetUserLogin(string username, string password)
        internal async Task<UserLogin> GetUserLogin(string username, string password)
        {
            string qry = "";
            /*//qry = "SELECT * FROM `login` where `username`='" + username + "' and `password` ='" + password + "' and `active_user` = 1 ;";
            qry = "SELECT login_id,username,useremail,user_role FROM `login` where `useremail`='" + username + "' and `password` ='" + password + "' and `active_user` = 1 ;";
            List<UserLogin> _UserLogin = new List<UserLogin>();
            _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            if(_UserLogin.Count > 0)
            {
                string qry1 = "update login set last_accessed=NOW() where login_id=" + _UserLogin[0].login_id + ";";
                await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            }
            return _UserLogin;
           */
            qry = "SELECT login_id,username,useremail,user_role FROM `login` where `useremail`='" + username + "' and `password` ='" + password + "' and `active_user` = 1 ;";
            var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            if (_UserLogin.Count > 0)
            {
                string qry1 = "update login set last_accessed=NOW() where login_id=" + _UserLogin[0].login_id + ";";
                await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            }
            return _UserLogin.FirstOrDefault();

        }
        internal async Task<int> WindUserRegistration(string fname, string useremail, string role, string userpass)
        {
            string qry = "insert into login (`username`,`useremail`,`user_role`,`password`) VALUES('" + fname + "','" + useremail + "','"+ role + "','"+userpass+"')";
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
           // qry = "SELECT login_id,username,useremail,user_role,created_on,active_user FROM `login` " + filter;
            qry = "SELECT login_id,username,useremail,user_role,active_user FROM `login` " + filter;
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        public async Task<List<HFEPage>> GetPageList(int login_id)
        {
           /* string filter = "";
            if (login_id != 0)
            {
                filter = " where login_id='" + login_id + "'";
            }*/
            string qry = "";
            qry = "SELECT * FROM `hfe_pages` where Visible=1 ";
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<HFEPage> _pagelist = new List<HFEPage>();
            _pagelist = await Context.GetData<HFEPage>(qry).ConfigureAwait(false);
            return _pagelist;


        }
        public async Task<List<UserAccess>> GetWindUserAccess(int login_id)
        {
            
            string qry = "";
           // qry = "SELECT t1.category_name,t1.cat_id,t2.login_id,t2.identity,t2.upload_access,t3.display_name,t3.Action_url,t3.Controller_name FROM `access_category`as t1 left join `user_access` as t2 on t1.cat_id=t2.category_id left join hfe_pages as t3 on t3.Id=t2.identity where t2.login_id='" + login_id + "'";
            //qry = "SELECT t1.login_id,t1.site_type,if(t3.Page_type IS NOT NULL,t3.Page_type,3) as page_type,t1.identity,t1.upload_access,t3.display_name,t3.Action_url,t3.Controller_name FROM `user_access`as t1 left join hfe_pages as t3 on t3.Id=t1.identity where t1.login_id='" + login_id + "'";

            qry = "SELECT t1.login_id,t1.site_type,if(t3.Page_type IS NOT NULL,t3.Page_type,3) as page_type,t1.identity,t1.upload_access,t3.display_name,t3.Action_url,t3.Controller_name FROM `user_access`as t1 left join hfe_pages as t3 on t3.Id=t1.identity and t1.category_id NOT IN(3) where t1.login_id='" + login_id + "'";
            
            List<UserAccess> _accesslist = new List<UserAccess>();
            _accesslist = await Context.GetData<UserAccess>(qry).ConfigureAwait(false);
            return _accesslist;


        }
        internal async Task<int> SubmitUserAccess(int login_id, string siteList, string pageList, string reportList)
        {
            var SiteList = new JavaScriptSerializer().Deserialize<dynamic>(siteList);
            var PageList = new JavaScriptSerializer().Deserialize<dynamic>(pageList);
            var ReportList = new JavaScriptSerializer().Deserialize<dynamic>(reportList);

            string qry= "insert into `user_access` (`login_id`,`category_id`,`identity`) VALUES";
            string pagevalues = "";
            foreach (var page in PageList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = page.Key;
                var b = page.Value;
                if(page.Value == true)
                {
                    pagevalues += "('" + login_id + "','1','" + page.Key + "'),";
                }
            }
            qry += pagevalues;
            await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            
            string qry1 = "insert into `user_access` (`login_id`,`category_id`,`identity`) VALUES";
            string reportvalues = "";
            foreach (var report in ReportList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = report.Key;
                var b = report.Value;
                if (report.Value == true)
                {
                    reportvalues += "('" + login_id + "','2','" + report.Key + "'),";
                }
            }
            qry1 += reportvalues;
            await Context.ExecuteNonQry<int>(qry1.Substring(0, (qry1.Length - 1)) + ";").ConfigureAwait(false);

            string qry2 = "insert into `user_access` (`login_id`,`category_id`,`identity`,`upload_access`) VALUES";
            string sitevalues = "";
            foreach (var site in SiteList)
            {
                int upload_access = 0;
                if (site.Value == true)
                {
                    upload_access = 1;
                }

                    sitevalues += "('" + login_id + "','3','" + site.Key + "','"+ upload_access + "'),";
               
            }
            qry2 += sitevalues;
           return await Context.ExecuteNonQry<int>(qry2.Substring(0, (qry2.Length - 1)) + ";").ConfigureAwait(false);
           // return 0;

        }

        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }


    }

}
