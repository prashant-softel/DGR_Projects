using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class UserLogin
    {
        public int login_id { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string password { get; set; }
        public string user_role { get; set; }
        public DateTime created_on { get; set; }
        public string auth_session { get; set; }
        public DateTime last_accessed { get; set; }
        public int active_user { get; set; }
        public bool islogin { get; set; }
    }
    public class UserInfomation
    {
        public int login_id { get; set; }
        public string username { get; set; }
        public string useremail { get; set; }
        public string user_role { get; set; }
       // public DateTime created_on { get; set; }
        public int active_user { get; set; }
      
    }
    public class HFEPage
    {
        public int Id { get; set; }
        public string Display_name { get; set; }
        public string Action_url { get; set; }
        public string Controller_name { get; set; }
        public int Page_type { get; set; }
        public int Order_no { get; set; }
        public int Visible { get; set; }

    }
    public class HFEPage1
    {
       
        public int login_id { get; set; }
        public int page_type { get; set; }
        public int identity { get; set; }
        public int upload_access { get; set; }
        public string Display_name { get; set; }
        public string Action_url { get; set; }
        public string Controller_name { get; set; }
        
      
     

    }
}
