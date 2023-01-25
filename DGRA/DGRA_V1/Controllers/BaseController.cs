//using DGRA_V1.Models;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using System;

//namespace DGRA_V1.Controllers
//{
//    public class BaseController : Controller
//    {
//        public int LoginUserID { get; }
//        public string[] UsersRoles { get; }
//        public string LoginEmial { get; }
//        public int LoginOrgID { get; set; }
//        public string UserName { get; }
//        public int RoleID { get; }
//        public string HOST { get; }
//        public string HostName { get; }
//        public string ClientIP { get; }
//        public bool IsLive { get; }

//        private readonly IHttpContextAccessor _httpContextAccessor;

       
//        public BaseController()
//        {
//            //_httpContextAccessor = HttpContext;

//            HttpContext.Session.GetString("role");
//            this.RoleID = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.LoginUserRoleID));
//            this.LoginUserID = Convert.ToInt32(HttpContext.Session.GetString(SessionVariable.LoginUserID));

//            this.LoginEmial = Convert.ToString(HttpContext.Session.GetString(SessionVariable.LoginUserEmail));

//            // this.UsersRoles = (string[])System.Web.HttpContext.Current.Session[SessionVariable.UserRolesID];
//            this.UserName = Convert.ToString(HttpContext.Session.GetString(SessionVariable.LoginUserFullName));
//            this.HOST = System.Configuration.ConfigurationManager.AppSettings["HostName"].ToString();

//            if(LoginUserID<0)
//            {
//                 RedirectToAction("Login");
//            }


//        }

//    }
//}
