using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarSiteMaster
    {
        public int site_master_solar_id { get; set; }
        public string country { get; set; }
       
            public string site { get; set; }
 
            public string spv { get; set; }
  
            public string state { get; set; }
 
            public double dc_capacity { get; set; }
 
            public double ac_capacity { get; set; }
        public double total_tarrif { get; set; }
  
    }
}
