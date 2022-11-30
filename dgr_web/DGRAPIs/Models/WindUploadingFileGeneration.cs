
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
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public int site_id { get; set; }
        public decimal wind_speed { get; set; }
        public decimal grid_hrs { get; set; }
        public decimal operating_hrs { get; set; }
        public decimal production_rs { get; set; }
        public decimal kwh { get; set; }
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
        public double production_rs { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double unschedule_hrs { get; set; }
        public double schedule_hrs { get; set; }
        public double others { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }

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
        public double production_rs { get; set; }
        public double kwh { get; set; }
        public double plf { get; set; }
        public double ma_actual { get; set; }
        public double ma_contractual { get; set; }
        public double iga { get; set; }
        public double ega { get; set; }
        public double unschedule_hrs { get; set; }
        public double schedule_hrs { get; set; }
        public double others { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }

    }
    public class WindUploadingFilegeneration
    {
        
    }

    public class WindUploadedData
    {
        //        public int uploading_file_generation_id { get; set; }
        public int uploading_file_generation_id { get; set; }
        public string date { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public int site_id { get; set; }
        public string site_name { get; set; }
        public decimal wind_speed { get; set; }
        public double Capacity_mw { get; set; }
        public double kwh { get; set; }
    }
}
