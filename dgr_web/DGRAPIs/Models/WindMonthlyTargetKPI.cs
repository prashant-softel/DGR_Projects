using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindMonthlyTargetKPI
    {


        public string fy { get; set; }
        public string month { get; set; }
        public string site { get; set; }
        public dynamic windSpeed { get; set; }
        public dynamic kwh { get; set; }
        public dynamic ma { get; set; }
        public dynamic iga { get; set; }
        public dynamic ega { get; set; }
        public dynamic plf { get; set; }
    }
}
