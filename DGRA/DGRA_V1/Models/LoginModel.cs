using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class LoginModel
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
    public class UserAccess
    {
        public int login_id { get; set; }
        public int site_type { get; set; }
        public int page_type { get; set; }
        public int identity { get; set; }
        public int upload_access { get; set; }
        public string display_name { get; set; }
        public string action_url { get; set; }
        public string controller_name { get; set; }

        public List<UserAccess> access_list = new List<UserAccess>();
    }

}
