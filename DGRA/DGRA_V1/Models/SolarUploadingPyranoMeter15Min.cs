using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class SolarUploadingPyranoMeter15Min
    {
        public dynamic date_time { get; set; }
        public int site_id { get; set; }
        public dynamic ghi_1 { get; set; }
        public dynamic ghi_2 { get; set; }
        public dynamic poa_1 { get; set; }
        public dynamic poa_2 { get; set; }
        public dynamic poa_3 { get; set; }
        public dynamic poa_4 { get; set; }
        public dynamic poa_5 { get; set; }
        public dynamic poa_6 { get; set; }
        public dynamic poa_7 { get; set; }
        public dynamic avg_ghi { get; set; }
        public dynamic avg_poa { get; set; }
        public dynamic amb_temp { get; set; }
        public dynamic mod_temp { get; set; }
    }
}
