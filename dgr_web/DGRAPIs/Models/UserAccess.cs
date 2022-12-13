
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class UserAccess
    {
        public int login_id { get; set; }
        public int site_type { get; set; }
        public Int64 page_type { get; set; }
        public int identity { get; set; }
        public int upload_access { get; set; }
        public string Display_name { get; set; }
        public string Action_url { get; set; }
        public string Controller_name { get; set; }
    }
}
