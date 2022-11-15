
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class WindUploadingFilegeneration
    {
        public int uploading_file_generation_id { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public decimal wind_speed { get; set; }
        public decimal grid_hrs { get; set; }
        public decimal operating_hrs { get; set; }
        public decimal production_rs { get; set; }
        public decimal kwh { get; set; }
    }
}
