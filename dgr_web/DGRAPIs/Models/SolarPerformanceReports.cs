using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarPerformanceReports
    {
        public string site { get; set; }
        public string spv { get; set; }
        public double capacity { get; set; }
        public double dc_capacity { get; set; }
        public double total_tarrif { get; set; }
        public double tar_kwh { get; set; }
        public double expected_kwh { get; set; }
        public double act_kwh { get; set; }
        public string lineloss { get; set; }
        public double tar_ghi { get; set; }
        public double act_ghi { get; set; }
        public double tar_poa { get; set; }
        public double act_poa { get; set; }
        public double tar_plf { get; set; }
        public double act_plf { get; set; }
        public double tar_pr { get; set; }
        public double act_pr { get; set; }
        public double tar_ma { get; set; }
        public double act_ma { get; set; }
        public double tar_iga { get; set; }
        public double act_iga { get; set; }
        public double tar_ega { get; set; }
        public double act_ega { get; set; }

    }
    public class SolarPerformanceReports1
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public string spv { get; set; }
        public double capacity { get; set; }
        public double dc_capacity { get; set; }
        public double total_tarrif { get; set; }
        public double tar_kwh { get; set; }
        public double expected_kwh { get; set; }
        public double act_kwh { get; set; }
        public double lineloss { get; set; }
        public double tar_ghi { get; set; }
        public double act_ghi { get; set; }
        public double tar_poa { get; set; }
        public double act_poa { get; set; }
        public double tar_plf { get; set; }
        public double act_plf { get; set; }
        public double tar_pr { get; set; }
        public double act_pr { get; set; }
        public double tar_ma { get; set; }
        public double act_ma { get; set; }
        public double tar_iga { get; set; }
        public double act_iga { get; set; }
        public double tar_ega { get; set; }
        public double act_ega { get; set; }
       

    }
    public class SolarPerformanceReports2
    {
        public string site { get; set; }
        public string spv { get; set; }
        public double capacity { get; set; }
        public double dc_capacity { get; set; }
        public double total_tarrif { get; set; }
        public double tar_kwh { get; set; }
        public double tar_kwh_mu { get; set; }
        public double expected_kwh { get; set; }
        public double act_kwh { get; set; }
        public double act_kwh_mu { get; set; }
        public string lineloss { get; set; }
        public double tar_ghi { get; set; }
        public double act_ghi { get; set; }
        public double tar_poa { get; set; }
        public double act_poa { get; set; }
        public double tar_plf { get; set; }
        public double act_plf { get; set; }
        public double tar_pr { get; set; }
        public double act_pr { get; set; }
        public double tar_ma { get; set; }
        public double act_ma { get; set; }
        public double tar_iga { get; set; }
        public double act_iga { get; set; }
        public double tar_ega { get; set; }
        public double act_ega { get; set; }
        public double total_mw { get; set; }
        public double revenue { get; set; }
    }
    public class SolarOpertionalHead
    {

        public float spv_count { get; set; }
        public double capacity { get; set; }

    }
    public class SolarOpertionalHead1
    {

        public double wind_total_mw { get; set; }
        public double solar_total_ac_mw { get; set; }

    }
}
