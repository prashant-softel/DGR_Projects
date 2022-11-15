using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarUploadingPyranoMeter1Min
    {
        
        public string date_time { get; set; }
        public string ghi_1 { get; set; }
        public string ghi_2 { get; set; }
        public string poa_1 { get; set; }
        public string poa_2 { get; set; }
        public string poa_3 { get; set; }
        public string poa_4 { get; set; }
        public string poa_5 { get; set; }
        public string poa_6 { get; set; }
        public string poa_7 { get; set; }
        public decimal avg_ghi { get; set; }
        public decimal avg_poa { get; set; }
          
    }
}
