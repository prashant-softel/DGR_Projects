using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class WindBreakdownReports
    {
        public int uploading_file_breakdown_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public string bd_type { get; set; }
        public string stop_from { get; set; }
        public string stop_to { get; set; }

        public string total_stop { get; set; }

        public string error_description { get; set; }

        public string action_taken { get; set; }

        public string country { get; set; }

        public string state { get; set; }

        public string spv { get; set; }

        public string site { get; set; }

       
        public List<WindBreakdownReports> list = new List<WindBreakdownReports>();
    }
}
