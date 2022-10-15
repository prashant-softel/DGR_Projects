using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindLocationMaster
    {
        public int location_master_id { get; set; }
        public int site_master_id { get; set; }
        public string site { get; set; }
        public string wtg { get; set; }
        public double feeder { get; set; }
        public double max_kwh_day { get; set; }
         
    }
}
