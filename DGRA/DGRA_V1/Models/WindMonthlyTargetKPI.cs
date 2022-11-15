using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
   
        public class WindMonthlyTargetKPI
        {
            public string fy { get; set; }
            public string month { get; set; }
            public string site { get; set; }
            public decimal windSpeed { get; set; }
            public decimal kwh { get; set; }
            public string ma { get; set; }
            public string iga { get; set; }
            public string ega { get; set; }
            public string plf { get; set; }
        }

    }
