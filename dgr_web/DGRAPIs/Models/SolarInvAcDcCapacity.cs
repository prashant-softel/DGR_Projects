using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarInvAcDcCapacity
    {
        public int capacity_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string inverter { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
          
    }
}
