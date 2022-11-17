
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDailyLoadShedding
    {
        public string date { get; set; }
        public string site{ get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public string totalTime { get; set; }
        public string permLoad { get; set; }
        public string genShedding { get; set; }
    }
}
