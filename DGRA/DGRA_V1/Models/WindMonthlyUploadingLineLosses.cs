using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DGRA_V1.Models
{
    public class WindMonthlyUploadingLineLosses
    {
        public int monthly_uploading_line_losses_id { get; set; }
        public string fy { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public double lineLoss { get; set; }
    }
}