using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindPerformanceReports
    {
        public string site { get; set; }
        public string spv { get; set; }
        public double total_mw { get; set; }
        public double tar_kwh { get; set; }
        public double tar_kwh_mu { get; set; }
        public double act_jmr_kwh { get; set; }
        public double act_jmr_kwh_mu { get; set; }
        public double total_tarrif { get; set; }
        public double capacity { get; set; }
        public dynamic date { get; set; }
        
        public double tar_wind { get; set; }
        public double act_Wind { get; set; }
        public double tar_plf { get; set; }
        public double act_plf { get; set; }
        public double tar_ma { get; set; }
        public double act_ma { get; set; }
        public double tar_iga { get; set; }
        public double act_iga { get; set; }
        public double tar_ega { get; set; }
        public double act_ega { get; set; }
        public double revenue { get; set; }
       
    }
    public class WindOpertionalHead
    {
        
        public float spv_count { get; set; }
        public double capacity { get; set; }

    }
}
