using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class SolarUploadingPyranoMeter15Min
    {
        public string date_time { get; set; }
        public decimal ghi_1 { get; set; }
        public decimal ghi_2 { get; set; }
        public decimal poa_1 { get; set; }
        public decimal poa_2 { get; set; }
        public decimal poa_3 { get; set; }
        public decimal poa_4 { get; set; }
        public decimal poa_5 { get; set; }
        public decimal poa_6 { get; set; }
        public decimal poa_7 { get; set; }
        public decimal avg_ghi { get; set; }
        public decimal avg_poa { get; set; }
        public decimal amb_temp { get; set; }
        public decimal mod_temp { get; set; }
    }
}
