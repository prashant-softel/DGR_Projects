using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyLoadShedding
    {
        public string Site { get; set; }
        public DateTime Date { get; set; }
        public string Start_Time { get; set; }
        public string End_Time { get; set; }
        public string Total_Time { get; set; }
        public string Permissible_Load_MW { get; set; }
        public string Gen_loss_kWh { get; set; }
    }
}
