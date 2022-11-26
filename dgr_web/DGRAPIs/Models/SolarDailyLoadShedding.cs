using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyLoadShedding
    {
        public string Site { get; set; }
        public int Site_Id { get; set; }
        public dynamic Date { get; set; }
        public dynamic Start_Time { get; set; }
        public dynamic End_Time { get; set; }
        public dynamic Total_Time { get; set; }
        public decimal Permissible_Load_MW { get; set; }
        public decimal Gen_loss_kWh { get; set; }
    }
}
