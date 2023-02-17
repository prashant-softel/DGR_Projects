
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindUploadingFileGeneration
    {
        public int uploading_file_generation_id { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id{ get; set; }
        public dynamic wind_speed { get; set; }
        public dynamic grid_hrs { get; set; }
        public dynamic operating_hrs { get; set; }
        public dynamic lull_hrs { get; set; }
        public dynamic kwh { get; set; }

    }
    public class WindUploadingFilegeneration1
    {
        public int uploading_file_generation_id { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public double wind_speed { get; set; }
        public double grid_hrs { get; set; }
        public double operating_hrs { get; set; }
        public double lull_hrs { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double plf_afterlineloss { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double capacity_kw { get; set; }
        public double kwh_afterloss { get; set; }
        public double kwh_afterlineloss { get; set; }
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
    public class WindUploadingFilegeneration2
    {
        public int uploading_file_generation_id { get; set; }
        public string site { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public int site_id { get; set; }
        public float feeder { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public double wind_speed { get; set; }
        public double grid_hrs { get; set; }
        public double operating_hrs { get; set; }
        public double lull_hrs { get; set; }
        public double kwh { get; set; }
        public double kwh_afterlineloss { get; set; }
        public double plf { get; set; }
        public double plf_afterlineloss { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
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
        public double capacity_kw { get; set; }
        public int import_batch_id { get; set; }
        

    }
    public class WindUploadingFilegeneration
    {
        
    }

    public class WindUploadedData
    {
        //public int uploading_file_generation_id { get; set; }
        public int uploading_file_generation_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public int site_id { get; set; }
        public string site_name { get; set; }
        public decimal wind_speed { get; set; }
        public double capacity_mw { get; set; }
        public double kwh { get; set; }
    }
}
