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


       
    }
  
}
