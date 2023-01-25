using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class DailyGenSummary
    {
        public int daily_gen_summary_id { get; set; }
        public string state { get; set; }
        public string site { get; set; }
        public DateTime date { get; set; }
        public string wtg { get; set; }
        public float wind_speed { get; set; }

        public float kwh { get; set; }

        public string feeder { get; set; }

        public float ma_contractual { get; set; }

        public float ma_actual { get; set; }

        public float iga { get; set; }

        public float ega { get; set; }

        public float plf { get; set; }

        public float grid_hrs { get; set; }

        public float lull_hrs { get; set; }

        public float production_hrs { get; set; }

        public float unschedule_hrs { get; set; }

        public float schedule_hrs { get; set; }

        public float others { get; set; }

        public float igbdh { get; set; }

        public float egbdh { get; set; }

        public string load_shedding { get; set; }
        public List<DailyGenSummary> list = new List<DailyGenSummary>();
    }
}
