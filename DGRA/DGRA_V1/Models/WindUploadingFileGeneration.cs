
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class WindUploadingFileGeneration
    {
        public int uploading_file_generation_id { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public dynamic date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public dynamic wind_speed { get; set; }
        public dynamic grid_hrs { get; set; }
        public dynamic operating_hrs { get; set; }
        public dynamic lull_hrs { get; set; }
        public dynamic kwh { get; set; }
    }
}
