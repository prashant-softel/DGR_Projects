using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindUploadingFileBreakDown
    {
        public int uploading_file_breakdown_id { get; set; }
        public dynamic date { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public string bd_type { get; set; }
        public int bd_type_id { get; set; }
        public dynamic stop_from { get; set; }
        public dynamic stop_from_num { get; set; }
        public dynamic stop_to { get; set; }
        public dynamic stop_to_num { get; set; }
        public dynamic total_stop { get; set; }
        public dynamic total_stop_num { get; set; }
        public string error_description { get; set; }
        public string action_taken { get; set; }
        public Int64 wtg_cnt { get; set; }

    }
    public class WindUploadingFileBreakDown1
    {
        public int site_id { get; set; }
        public dynamic date { get; set; }
        public string site { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public string bd_type { get; set; }
        public int bd_type_id { get; set; }
        public dynamic stop_from { get; set; }
        public dynamic stop_to { get; set; }
        public dynamic total_stop { get; set; }
        public string error_description { get; set; }
        public string action_taken { get; set; }
        public string bd_type_name { get; set; }
    }
}
