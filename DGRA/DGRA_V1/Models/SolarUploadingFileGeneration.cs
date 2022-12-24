
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class SolarUploadingFileGeneration
    {
        public int uploading_file_generation_solar_id { get; set; }
        public dynamic date { get; set; }
        public string site { get; set; }
        public int site_id{ get; set; }
        public string inverter { get; set; }
        public dynamic inv_act { get; set; }
        public dynamic plant_act { get; set; }
        public dynamic pi { get; set; }
    }
}
