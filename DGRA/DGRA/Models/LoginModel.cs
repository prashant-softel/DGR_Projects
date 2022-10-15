using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class LoginModel
    {
        public int login_id { get; set; }
        public string name { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string user_role { get; set; }

        internal void Add()
        {
            throw new NotImplementedException();
        }
    }
}
