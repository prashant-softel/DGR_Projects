using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarMonthlyUploadingLineLosses
    {
        public int monthly_line_loss_solar_id { get; set; }
        public string FY { get; set; }
        public string Sites { get; set; }
        public int Site_Id { get; set; }
        public string Month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public dynamic LineLoss { get; set; }
    }
}
