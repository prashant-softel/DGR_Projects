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
        internal async Task<UserLogin> GetUserLogin(string username, string password, bool isSSO)
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
            if (isSSO)
            {
                qry = "SELECT login_id,username,useremail,user_role,islogin as islogin FROM `login` where `useremail`='" + username + "'  and `active_user` = 1 ;";
               
            }
            else {
                qry = "SELECT login_id,username,useremail,user_role,islogin as islogin FROM `login` where `useremail`='" + username + "' and `password` = md5('" + password + "') and `active_user` = 1 ;";
            }
            var _UserLogin = await Context.GetData<UserLogin>(qry).ConfigureAwait(false);
            if (_UserLogin.Count > 0)
            {
                string qry1 = "update login set last_accessed=NOW(),islogin=1 where login_id=" + _UserLogin[0].login_id + ";";
                await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            }
            return _UserLogin.FirstOrDefault();

        }
        internal async Task<int> UpdateLoginStatus(int UserID)
        {
            string qry1 = "Update login set islogin=0 where last_accessed<  date_add(now(),interval -10 minute); update login set last_accessed=NOW(),islogin=1 where login_id=" + UserID + ";";
            return await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);

        }
        internal async Task<int> DirectLogOut(int UserID)
        {
            string qry1 = "Update login set islogin=0 where  login_id=" + UserID + ";";
            return await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);

        }
        internal async Task<int> WindUserRegistration(string fname, string useremail, string role, string userpass)
        {
            string qry1 = "SELECT useremail FROM login WHERE useremail = '"+useremail+"'";
            List<UserLogin> email = new List<UserLogin>();
            email = await Context.GetData<UserLogin>(qry1).ConfigureAwait(false);
            if (email.Capacity > 0)
            {
                return -1;
            }
            else
            {
                string qry = "insert into login (`username`,`useremail`,`user_role`,`password`) VALUES('" + fname + "','" + useremail + "','" + role + "', MD5('" + userpass + "'))";
                //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
                return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
            }
            
        }

        internal async Task<int> UpdatePassword(int loginid, string updatepass)
        {
            string qry = "update login set password=MD5('" + updatepass + "') where login_id=" + loginid + "";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
           // string a = qry;
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> DeactivateUser(int loginid)
        {
            //UPDATE login SET active_user = 0 WHERE login_id = 3;
            string qry = "update login set active_user= 0 where login_id=" + loginid + "";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            // string a = qry;
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }

        //ActivateUser
        internal async Task<int> ActivateUser(int loginid)
        {
            //UPDATE login SET active_user = 1 WHERE login_id = 3;
            string qry = "update login set active_user= 1 where login_id=" + loginid + "";
            //return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            // string a = qry;
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }
        //DeleteUser
        internal async Task<int> DeleteUser(int loginid)
        {
            //DELETE FROM `login` WHERE login_id = 3 ;
            string qry = "delete from login where login_id=" + loginid + "";
            string qry1 = "delete from user_access where login_id=" + loginid + "";
            await Context.ExecuteNonQry<int>(qry1).ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
        }

        //GetUserLoginId
        internal async Task<List<UserInfomation>> GetUserLoginId(string username, string useremail)
        {
            string qry = "select login_id, username,useremail,user_role,active_user from login where username='" + username + "' AND useremail= '" + useremail + "';";
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        //Clone User access
        internal async Task<int> SubmitCloneUserAccess(int login_id, int site_type, int page_type, int identity, int upload_access)
        {
            string qry = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES (" +login_id +"," + site_type + "," + page_type + "," + identity + "," + upload_access + ");";
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
        public async Task<List<UserInfomation>> GetSolarUserInformation(int login_id)
        {
            string filter = "";
            if (login_id != 0)
            {
                filter = " where login_id=" + login_id;
            }
            string qry = "";
            // qry = "SELECT login_id,username,useremail,user_role,created_on,active_user FROM `login` " + filter;
            qry = "SELECT login_id, username,useremail,user_role,active_user  FROM `login` " + filter;
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<UserInfomation> _userinfo = new List<UserInfomation>();
            _userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            return _userinfo;
        }
        public async Task<List<HFEPage>> GetPageList(int login_id, int site_type)
        {
            /* string filter = "";
             if (login_id != 0)
             {
                 filter = " where login_id='" + login_id + "'";
             }*/
            string qry = "";

           //qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2";
            if(site_type == 2)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=2 or site_type = 0";
            }
            if (site_type == 1)
            {
                qry = "SELECT * FROM `hfe_pages` where Visible=1 and site_type=1 or site_type = 0";
            }
            // Console.WriteLine(qry);
            // var _Userinfo = await Context.GetData<UserInfomation>(qry).ConfigureAwait(false);
            // return _Userinfo.FirstOrDefault();
            List<HFEPage> _pagelist = new List<HFEPage>();
            _pagelist = await Context.GetData<HFEPage>(qry).ConfigureAwait(false);
            return _pagelist;


        }
        public async Task<List<UserAccess>> GetWindUserAccess(int login_id,string role)
        {
            
            string qry = "";
            if (role == "Admin")
            {
                qry = "SELECT Site_Type as site_type,Page_type as page_type,display_name,Action_url,Controller_name FROM `hfe_pages` where Visible=1 order by Order_no";
            }
            else
            {
                qry = "SELECT t1.login_id,t1.site_type,if(t3.Page_type IS NOT NULL,t3.Page_type,3) as page_type,t1.identity,t1.upload_access,t3.display_name,t3.Action_url,t3.Controller_name FROM `user_access`as t1 left join hfe_pages as t3 on t3.Id=t1.identity and t1.category_id NOT IN(3) where t1.login_id='" + login_id + "'";
            }
            List<UserAccess> _accesslist = new List<UserAccess>();
            _accesslist = await Context.GetData<UserAccess>(qry).ConfigureAwait(false);
            return _accesslist;


        }
        internal async Task<int> SubmitUserAccess(int login_id, string siteList, string pageList, string reportList, string site_type,int importapproval)
        {
            var SiteList = new JavaScriptSerializer().Deserialize<dynamic>(siteList);
            var PageList = new JavaScriptSerializer().Deserialize<dynamic>(pageList);
            var ReportList = new JavaScriptSerializer().Deserialize<dynamic>(reportList);
            int flag = 0;
            if (string.IsNullOrEmpty(site_type)) site_type = "1";
            string delAccess ="DELETE FROM `user_access` WHERE login_id  = '" + login_id + "' AND `site_type` = '"+ site_type + "'";
            await Context.ExecuteNonQry<int>(delAccess).ConfigureAwait(false);
           
            
            string qry= "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
            string pagevalues = "";
            foreach (var page in PageList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = page.Key;
                var b = page.Value;
                if(page.Value == true)
                {
                    string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and site_type = " + site_type + " and " +
                        "category_id = 1 and identity = " + page.Key;
                    List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false); 
                    if (_accesslist.Capacity > 0) continue;
                    //string qry = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
                    flag = 1;
                    pagevalues += "('" + login_id + "',"+site_type+",'1','" + page.Key + "'),";
                }
            }
            if (flag == 1)
            {

                qry += pagevalues;
                await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            string qry1 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
            string reportvalues = "";
            int flag2 = 0; 
            foreach (var report in ReportList)
            {
                //Console.WriteLine("dictionary key is {0} and value is {1}", dictionary.Key, dictionary.Value);
                var a = report.Key;
                var b = report.Value;
                if (report.Value == true)
                {
                    string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and site_type = " + site_type + " and " +
                        "category_id = 2 and identity = " + report.Key;
                    List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false);
                    if (_accesslist.Capacity > 0) continue;
                    //string qry1 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`) VALUES";
                    flag2 = 1;
                    reportvalues += "('" + login_id + "',"+site_type+",'2','" + report.Key + "'),";
                }
            }
            if (flag2 == 1)
            {

                qry1 += reportvalues;
                await Context.ExecuteNonQry<int>(qry1.Substring(0, (qry1.Length - 1)) + ";").ConfigureAwait(false);
            }

            string qry2 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES";
            string sitevalues = "";
            bool flag3 = false;
            foreach (var site in SiteList)
            {

                int upload_access = 0;
                if (site.Value == true)
                {
                    upload_access = 1;
                }
                if (site.Key == "") continue;
                string checkqry = "select login_id from `user_access` where login_id = " + login_id + " and " +
                        "category_id = 3 and identity = " + site.Key ;
                List<UserAccess> _accesslist = await Context.GetData<UserAccess>(checkqry).ConfigureAwait(false);
                if (_accesslist.Capacity > 0) 
                {
                    string qry3 = "update`user_access` set `upload_access` = " + upload_access + " where login_id = " + login_id + " and indentity =" +
                        site.Key;
                }
                else
                {
                    flag3 = true;
                    sitevalues += "('" + login_id + "'," + site_type + ",3,'" + site.Key + "','" + upload_access + "'),";
                }
            }
            if(site_type != "1" || site_type != "2")
            {
                string delAccess1 = "DELETE FROM `user_access` WHERE login_id  = '" + login_id + "' AND `site_type` = '0' AND 	category_id = 4";
                await Context.ExecuteNonQry<int>(delAccess1).ConfigureAwait(false);
            }
            if(importapproval > 0)
            {
                string qry4 = "insert into `user_access` (`login_id`, `site_type`, `category_id`,`identity`,`upload_access`) VALUES  ('" + login_id + "', 0, 4, '" + importapproval + "', '0')";
                await Context.ExecuteNonQry<int>(qry4).ConfigureAwait(false);
            }
            
            if (flag3 == true)
            {
                qry2 += sitevalues;
                return await Context.ExecuteNonQry<int>(qry2.Substring(0, (qry2.Length - 1)) + ";").ConfigureAwait(false);
            }
            return 0;

        }


        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }


    }

}
