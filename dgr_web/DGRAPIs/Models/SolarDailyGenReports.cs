using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarDailyGenReports
    {
        public int year { get; set; }
        public string month { get; set; }
        public string date { get; set; }
        public string country { get; set; }
        public string state { get; set; }

        public string spv { get; set; }

        public string site { get; set; }
        public string Inverter { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double ghi { get; set; }
        public double poa { get; set; }

        public double expected_kwh { get; set; }

        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }

        public double inv_pr { get; set; }
        public double plant_pr { get; set; }
        public double inv_plf { get; set; }
        public double plant_plf { get; set; }


        public double ma_actual { get; set; }

        public double ma_contractual { get; set; }

        public double iga { get; set; }
        public double ega { get; set; }
        public double prod_hrs { get; set; }
        public double usmh { get; set; }
        public double smh { get; set; }
        public double oh { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }
        public double lull_hrs { get; set; }
        public string tracker_losses { get; set; }
        public double total_losses { get; set; }


        public double lull_hrs_bd { get; set; }
        public double usmh_bs { get; set; }
        public double smh_bd { get; set; }
        public double oh_bd { get; set; }
        public double igbdh_bd { get; set; }
        public double egbdh_bd { get; set; }
        public double load_shedding_bd { get; set; }
        public double total_bd_hrs { get; set; }
    }
    public class SolarDailyGenReports1
    {
        public int year { get; set; }
        public string month { get; set; }
        public string date { get; set; }
        public string country { get; set; }
        public string state { get; set; }

        public string spv { get; set; }

        public string site { get; set; }
        public string Inverter { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double ghi { get; set; }
        public double poa { get; set; }

        public double expected_kwh { get; set; }

        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }

        public double inv_pr { get; set; }
        public double plant_pr { get; set; }
        public double inv_plf { get; set; }
        public double plant_plf { get; set; }


        public double ma_actual { get; set; }

        public double ma_contractual { get; set; }

        public double iga { get; set; }
        public double ega { get; set; }
        public double gen_hrs { get; set; }
        public double usmh { get; set; }
        public double smh { get; set; }
        public double oh { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }
        public double lull_hrs { get; set; }
        public string tracker_losses { get; set; }
        public double total_losses { get; set; }
        public double total_bd_hrs { get; set; }
        public double lull_hrs_bd { get; set; }
        public double usmh_bs { get; set; }
        public double smh_bd { get; set; }
        public double oh_bd { get; set; }
        public double igbdh_bd { get; set; }
        public double egbdh_bd { get; set; }
        public double load_shedding_bd { get; set; }
       // public double total_bd_hrs { get; set; }
    }
    public class SolarDailyGenReports2
    {
        public long year { get; set; }
        public string month { get; set; }
        public string date { get; set; }
        public string country { get; set; }
        public string state { get; set; }

        public string spv { get; set; }

        public string site { get; set; }
        public string Inverter { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double ghi { get; set; }
        public double poa { get; set; }

        public double expected_kwh { get; set; }

        public double inv_kwh { get; set; }
        public double plant_kwh { get; set; }

        public double inv_pr { get; set; }
        public double plant_pr { get; set; }
        public double inv_plf { get; set; }
        public double plant_plf { get; set; }


        public double ma_actual { get; set; }

        public double ma_contractual { get; set; }

        public double iga { get; set; }
        public double ega { get; set; }
        public double prod_hrs { get; set; }
        public double usmh { get; set; }
        public double smh { get; set; }
        public double oh { get; set; }
        public double igbdh { get; set; }
        public double egbdh { get; set; }
        public double load_shedding { get; set; }
        public double lull_hrs { get; set; }
        public string tracker_losses { get; set; }
        public double total_losses { get; set; }


        public double lull_hrs_bd { get; set; }
        public double usmh_bs { get; set; }
        public double smh_bd { get; set; }
        public double oh_bd { get; set; }
        public double igbdh_bd { get; set; }
        public double egbdh_bd { get; set; }
        public double load_shedding_bd { get; set; }
        public double total_bd_hrs { get; set; }


    }
}
