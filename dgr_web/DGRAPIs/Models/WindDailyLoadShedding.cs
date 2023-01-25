using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDailyLoadShedding
    {
        public string date { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string totalTime { get; set; }
        public dynamic permLoad { get; set; }
        public dynamic genShedding { get; set; }
    }
}
