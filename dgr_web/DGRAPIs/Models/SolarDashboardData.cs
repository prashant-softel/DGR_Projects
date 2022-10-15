using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDashboardData
    {
        public string Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string Site { get; set; }
        public double IR { get; set; }

        public double inv_kwh { get; set; }
        public string line_loss { get; set; }
        public double jmrkwh { get; set; }
        public double tarkwh { get; set; }
        public double tarIR { get; set; }

    }
}
