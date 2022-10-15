using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyBreakdownReport
    {
        public string date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string breakdown { get; set; }
        public string bd_type { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string from_bd { get; set; }
        public string to_bd { get; set; }
        public dynamic total_stop { get; set; }
        public string bd_remarks { get; set; }
        public string action_taken { get; set; }
    }
    public class SolarFileBreakdown
    {
        public int daily_bd_loss_solar_id { get; set; }
        public string date { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string spv { get; set; }
        public string site { get; set; }
        public string breakdown { get; set; }
        public string bd_type { get; set; }
        public string ext_bd { get; set; }
        public string igbd { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string stop_from { get; set; }
        public string stop_to { get; set; }
        public dynamic total_stop { get; set; }
        public double bd_ir { get; set; }
        public double capacity { get; set; }
        public double act_plant_pr { get; set; }
        public double plant_gen_loss { get; set; }
        public string remarks { get; set; }
        public string action { get; set; }
          
    }
}
