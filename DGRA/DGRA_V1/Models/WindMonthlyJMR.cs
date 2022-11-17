
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class WindMonthlyJMR
    {
        public string fy { get; set; }
        public string site { get; set; }
        public string plantSection { get; set; }
        public string jmrDate { get; set; }
        public string jmrMonth { get; set; }
        public string jmrYear { get; set; }
        public string lineLossPercent { get; set; }
        public string rkvhPercent { get; set; }
        public decimal controllerKwhInv { get; set; }
        public decimal scheduledUnitsKwh { get; set; }
        public decimal exportKwh { get; set; }
        public decimal importKwh { get; set; }
        public decimal netExportKwh{ get; set; }
        public decimal exportKvah { get; set; }
        public decimal importKvah { get; set; }
        public decimal exportKvarhLag { get; set; }
        public decimal importKvarhLag { get; set; }
        public decimal exportKvarhLead { get; set; }
        public decimal importKvarhLead { get; set; }
        public decimal lineLoss { get; set; }
    }
}
