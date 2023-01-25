using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyGenSummary
    {
        public int daily_gen_summary_solar_id { get; set; }
        public string state { get; set; }
        public string site { get; set; }
        public string date { get; set; }
        public string location_name { get; set; }
        public int total_strings { get; set; }
        public double ghi { get; set; }
        public double poa { get; set; }
        public double expected_kwh { get; set; }
        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }
        public double inv_pr { get; set; }
        public double plant_pr { get; set; }
        public double ma { get; set; } 
        public double iga { get; set; }

        public double ega { get; set; }
 
            public double inv_plf_ac { get; set; }
 
            public double inv_plf_dc { get; set; }
 
            public double plant_plf_ac { get; set; }
 
            public double plant_plf_dc { get; set; }
 
            public string pi { get; set; }
  
            public double prod_hrs { get; set; }
   
            public double lull_hrs_bd { get; set; }
 
            public double usmh_bs { get; set; }
  
            public double smh_bd { get; set; }
 
            public double oh_bd { get; set; }
 
            public double igbdh_bd { get; set; }
 
            public double egbdh_bd { get; set; }
 
            public double load_shedding_bd { get; set; }
 
            public double total_bd_hrs { get; set; }
 
            public double usmh { get; set; }
 
            public double smh { get; set; }
 
            public double oh { get; set; }
 
            public double igbdh { get; set; }
 
            public double egbdh { get; set; }
 
            public double load_shedding { get; set; }
 
            public double total_losses { get; set; }
 
            public double dc_capacity { get; set; }

        public double ac_capacity { get; set; }
        public string inverter { get; set; }


    }

}
