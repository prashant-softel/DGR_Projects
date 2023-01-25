using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDailyTargetKPI
    {
        public string FY { get; set; }
        public string Date { get; set; }
        public string Site { get; set; }
        public int site_id { get; set; }
        public dynamic WindSpeed { get; set; }
        public dynamic kWh { get; set; }
        public dynamic MA { get; set; }
        public dynamic IGA { get; set; }
        public dynamic EGA { get; set; }
        public dynamic PLF { get; set; }
  
    }
}
