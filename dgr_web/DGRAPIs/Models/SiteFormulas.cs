using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SiteFormulas
    {
        public int id { get; set; }
        public int site_id { get; set; }
        public string site_type { get; set; }
        public string MA_Actual { get; set; }
        public string MA_Contractual { get; set; }
        public string IGA { get; set; }
        public string EGA { get; set; }
        public double capacity_mw { get; set; }

    }
}