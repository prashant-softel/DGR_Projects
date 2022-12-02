using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindMonthlyUploadingLineLosses
    {
        public string FY { get; set; }
        public string Sites { get; set; }
        public string Month { get; set; }
        public dynamic LineLoss { get; set; }
              
         
    }
}
