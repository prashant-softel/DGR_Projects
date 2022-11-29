using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DGRA_V1.Models
{
    public class WindMonthlyUploadingLineLosses
    {
        public string fy { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string month { get; set; }
        public int month_number { get; set; }
        public int year { get; set; }
        public string lineLoss { get; set; }
    }
}
