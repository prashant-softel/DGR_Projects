using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindMonthlyJMR
    {
        public int monthly_jmr_id { get; set; }
        public string fy { get; set; }
        public string site { get; set; }
        public int siteId { get; set; }
        public string plantSection { get; set; }
        public string jmrDate { get; set; }
        public string jmrMonth { get; set; }
        public int jmrMonth_no { get; set; }
        public dynamic jmrYear { get; set; }
        public dynamic lineLossPercent { get; set; }
        public dynamic rkvhPercent { get; set; }
        public dynamic controllerKwhInv { get; set; }
        public dynamic scheduledUnitsKwh { get; set; }
        public dynamic exportKwh { get; set; }
        public dynamic importKwh { get; set; }
        public dynamic netExportKwh { get; set; }
        public dynamic exportKvah { get; set; }
        public dynamic importKvah { get; set; }
        public dynamic exportKvarhLag { get; set; }
        public dynamic importKvarhLag { get; set; }
        public dynamic exportKvarhLead { get; set; }
        public dynamic importKvarhLead { get; set; }
        public dynamic lineLoss { get; set; }
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
