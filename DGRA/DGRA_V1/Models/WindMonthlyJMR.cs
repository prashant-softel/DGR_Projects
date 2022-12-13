
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
        public int siteId { get; set; }
        public string plantSection { get; set; }
        public string jmrDate { get; set; }
        public string jmrMonth { get; set; }
        public string jmrYear { get; set; }
        public decimal lineLossPercent { get; set; }
        public string rkvhPercent { get; set; }
        public decimal controllerKwhInv { get; set; }
        public decimal scheduledUnitsKwh { get; set; }
        public decimal exportKwh { get; set; }
        public decimal importKwh { get; set; }
        public decimal netExportKwh { get; set; }
        public decimal exportKvah { get; set; }
        public decimal importKvah { get; set; }
        public decimal exportKvarhLag { get; set; }
        public decimal importKvarhLag { get; set; }
        public decimal exportKvarhLead { get; set; }
        public decimal importKvarhLead { get; set; }
        public decimal lineLoss { get; set; }
        //public int monthly_jmr_id { get; set; }
        //public string FY { get; set; }
        //public string Site { get; set; }
        //public string Plant_Section { get; set; }
        //public dynamic Controller_KWH_INV { get; set; }
        //public dynamic Scheduled_Units_kWh { get; set; }
        //public dynamic Export_kWh { get; set; }
        //public dynamic Import_kWh { get; set; }
        //public dynamic Net_Export_kWh { get; set; }
        //public dynamic Export_kVAh { get; set; }

        //public dynamic Import_kVAh { get; set; }
        //public dynamic Export_kVArh_lag { get; set; }
        //public dynamic Import_kVArh_lag { get; set; }
        //public dynamic Export_kVArh_lead { get; set; }
        //public dynamic Import_kVArh_lead { get; set; }
        //public dynamic JMR_date { get; set; }
        //public dynamic JMR_Month { get; set; }
        //public dynamic JMR_Year { get; set; }
        //public dynamic LineLoss { get; set; }
        //public dynamic Line_Loss_percentage { get; set; }
        //public dynamic RKVH_percentage { get; set; }
    }
    public class WindMonthlyJMR1
    {

        public int monthly_jmr_id { get; set; }
        public string FY { get; set; }
        public string Site { get; set; }
        public string Plant_Section { get; set; }
        public dynamic Controller_KWH_INV { get; set; }
        public dynamic Scheduled_Units_kWh { get; set; }
        public dynamic Export_kWh { get; set; }
        public dynamic Import_kWh { get; set; }
        public dynamic Net_Export_kWh { get; set; }
        public dynamic Export_kVAh { get; set; }

        public dynamic Import_kVAh { get; set; }
        public dynamic Export_kVArh_lag { get; set; }
        public dynamic Import_kVArh_lag { get; set; }
        public dynamic Export_kVArh_lead { get; set; }
        public dynamic Import_kVArh_lead { get; set; }
        public dynamic JMR_date { get; set; }
        public dynamic JMR_Month { get; set; }
        public dynamic JMR_Year { get; set; }
        public dynamic LineLoss { get; set; }
        public dynamic Line_Loss_percentage { get; set; }
        public dynamic RKVH_percentage { get; set; }

    }
}
