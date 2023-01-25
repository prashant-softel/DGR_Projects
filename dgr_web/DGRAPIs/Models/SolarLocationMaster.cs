using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarLocationMaster
    {
        public int location_master_solar_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string eg { get; set; }
        public string ig { get; set; }
        public string icr_inv { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string string_configuration { get; set; }
        public double total_string_current { get; set; }
        public double total_string_voltage { get; set; }
        public double modules_quantity { get; set; }
        public double wp { get; set; }

        public double capacity { get; set; }
        public string module_make { get; set; }
        public string module_model_no { get; set; }
        public string module_type { get; set; }
        public int string_inv_central_inv { get; set; }
    }
    public class SolarLocationMaster_Calc
    {
        public int location_master_solar_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public string eg { get; set; }
        public string ig { get; set; }
        public string icr_inv { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string string_configuration { get; set; }
        public double total_string_current { get; set; }
        public double total_string_voltage { get; set; }
        public double modules_quantity { get; set; }
        public double wp { get; set; }
        public double capacity { get; set; }
        public string module_make { get; set; }
        public string module_model_no { get; set; }
        public string module_type { get; set; }
        public string string_inv_central_inv { get; set; }

        public float lostPOA { get; set; }
        public float USMH_lostPOA { get; set; }
        public float SMH_lostPOA { get; set; }
        public float IGBD_lostPOA { get; set; }
        public float EGBD_lostPOA { get; set; }
        public float OthersHour_lostPOA { get; set; }
        public float LullHrs_lostPOA { get; set; }
        public float LS_lostPOA { get; set; }


        //Calculation matrix
        public TimeSpan EGBD_1 { get; set; }
        public TimeSpan EGBD_2 { get; set; }
        public TimeSpan EGBD { get; set; }

        public TimeSpan IGBD_1 { get; set; }
        public TimeSpan IGBD_2 { get; set; }
        public TimeSpan IGBD_3 { get; set; }
        public TimeSpan IGBD_4 { get; set; }
        public TimeSpan IGBD_5 { get; set; }
        public TimeSpan IGBD_6 { get; set; }
        public TimeSpan IGBD { get; set; }

        public TimeSpan LS_1 { get; set; }
        public TimeSpan LS_2 { get; set; }
        public TimeSpan LS_3 { get; set; }
        public TimeSpan LS_4 { get; set; }
        public TimeSpan LS { get; set; }

        public TimeSpan USMH_1 { get; set; }
        public TimeSpan USMH_2 { get; set; }
        public TimeSpan USMH_3 { get; set; }
        public TimeSpan USMH_4 { get; set; }
        public TimeSpan USMH_5 { get; set; }
        public TimeSpan USMH { get; set; }

        public TimeSpan SMH_1 { get; set; }
        public TimeSpan SMH_2 { get; set; }
        public TimeSpan SMH_3 { get; set; }
        public TimeSpan SMH_4 { get; set; }
        public TimeSpan SMH_5 { get; set; }
        public TimeSpan SMH { get; set; }

        public TimeSpan OthersHour_1 { get; set; }
        public TimeSpan OthersHour_2 { get; set; }
        public TimeSpan OthersHour_3 { get; set; }
        public TimeSpan OthersHour_4 { get; set; }
        public TimeSpan OthersHour_5 { get; set; }
        public TimeSpan OthersHour { get; set; }

        public TimeSpan LullHrs_1 { get; set; }
        public TimeSpan LullHrs_2 { get; set; }
        public TimeSpan LullHrs { get; set; }       
    }
}
