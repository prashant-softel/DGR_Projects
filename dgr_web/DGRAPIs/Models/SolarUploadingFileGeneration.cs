
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarUploadingFileGeneration
    {
        public int uploading_file_generation_solar_id { get; set; }
        public DateTime date { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string inverter { get; set; }
        public decimal inv_act { get; set; }
        public decimal plant_act { get; set; }
        public decimal pi { get; set; }
    }
}
