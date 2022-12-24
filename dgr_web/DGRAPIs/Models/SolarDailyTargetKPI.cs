using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyTargetKPI
    {
        public string FY { get; set; }
        public string Date { get; set; }
        public string Sites { get; set; }
        public int site_id { get; set; }
        public dynamic GHI { get; set; }
        public dynamic POA { get; set; }
        public dynamic kWh { get; set; }
        public dynamic MA { get; set; }
        public dynamic IGA { get; set; }
        public dynamic EGA { get; set; }
        public dynamic PR { get; set; }
        public dynamic PLF { get; set; }
        


    }
}
