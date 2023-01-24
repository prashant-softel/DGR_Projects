using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class WindDashboardData
    {
        
        public DateTime Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string Site { get; set; }
        public double Wind{ get; set; }

        public double KWH { get; set; }
        public double line_loss { get; set; }
        public double jmrkwh { get; set; }
        public double tarkwh { get; set; }
        public double tarwind { get; set; }
        public string tar_date { get; set; }
    }
    public class WindDashboardData1
    {

        public DateTime Date { get; set; }
        public int month { get; set; }
        public int year { get; set; }
        public string Site { get; set; }
        public double Wind { get; set; }

       // public double KWH { get; set; }
       // public string line_loss { get; set; }
        public double jmrkwh { get; set; }
        public double tarkwh { get; set; }
        public double tarwind { get; set; }
    }
}
