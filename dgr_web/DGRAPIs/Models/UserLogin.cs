using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class UserLogin
    {
        public int login_id { get; set; }
        public string name { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string user_role { get; set; }
        // public string dgr { get; set; }
        // public int status { get; set; }
    }
}
