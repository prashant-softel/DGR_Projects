
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class approvalObject
    {
        public int import_batch_id { get; set; }
        public string file_name { get; set; }
        public int import_type { get; set; }
        public int site_id { get; set; }
        public string site_name { get; set; }
        public DateTime import_date { get; set; }
        public dynamic data_date { get; set; }
        public int imported_by { get; set; }
        public string import_by_name { get; set; }
        public DateTime approval_date { get; set; }
        public int approved_by { get; set; }
        public string approved_by_name { get; set; }
        public DateTime rejected_date { get; set; }
        public int rejected_by { get; set; }
        public string rejected_by_name { get; set; }
        public int is_approved { get; set; }
        public string log_filename { get; set; }
    }
}
