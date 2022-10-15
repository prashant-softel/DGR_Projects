
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarUploadingFilegeneration
    {
        public int uploading_file_generation_solar_id { get; set; }
        public string date { get; set; }
        public string site { get; set; }
        public string inverter { get; set; }
        public decimal inv_act { get; set; }
        public decimal plant_act { get; set; }
        public string pi { get; set; }

    }
}
