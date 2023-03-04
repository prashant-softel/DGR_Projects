using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class DailyGenSummary
    {
        public int daily_gen_summary_id { get; set; }
        public string state { get; set; }
        public string site { get; set; }
        public DateTime date { get; set; }
        public string wtg { get; set; }
        public double wind_speed { get; set; }
       
        public double kwh { get; set; }
        public double kwh_afterlineloss { get; set; }

        public string feeder { get; set; }
       
        public double ma_contractual { get; set; }
     
        public double ma_actual { get; set; }
       
        public double iga { get; set; }
      
        public double ega { get; set; }
     
        public double plf { get; set; }
        public double plf_afterlineloss { get; set; }
        public float grid_hrs { get; set; }     
        public float lull_hrs { get; set; }      
        public float production_hrs { get; set; }
        public dynamic unschedule_hrs { get; set; }
        public dynamic unschedule_num { get; set; }
        public dynamic schedule_hrs { get; set; }
        public dynamic schedule_num { get; set; }
        public dynamic others { get; set; }
        public dynamic others_num { get; set; }
        public dynamic igbdh { get; set; }
        public dynamic igbdh_num { get; set; }
        public dynamic egbdh { get; set; }
        public dynamic egbdh_num { get; set; }
        public dynamic load_shedding { get; set; }
        public dynamic load_shedding_num { get; set; }

    }
}
