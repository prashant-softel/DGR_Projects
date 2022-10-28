using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Repositories;
using System.Globalization;
using MySql.Data.MySqlClient;



namespace DGRAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //public class Calculation : ControllerBase
    public class Calculation : GenericRepository
    {

        public const string MA_Actual = "MA_Actual";
        public const string MA_Contractual = "MA_Contractual";
        public const string Internal_Grid = "Internal_Grid";
        public const string External_Grid = "External_Grid";
        public const string server = "localhost";
        public const string dataBase = "hfe";
        public const string userName = "root";
        public const string passWord = null;
        public const string connString = "SERVER=" + server + ";" + "DATABASE=" + dataBase + ";" + "UID=" + userName + ";" + "PWD=" + passWord + ";";



        public Calculation(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {

        }

       // public void DailyKPICalculation_Wind(string fromDate, string toDate, int siteId, string siteType, int FormualaType
       public int DailyKPICalculation_Wind()
             // internal async Task<List<BDTypeList>> DailyKPICalculation_Wind()
       {
            double Final_USMH = 0;
            double Final_SMH = 0;
            double Final_IGBD = 0;
            double Final_EGBD = 0;
            double Final_LoadShedding = 0;
            double Final_LULL = 0;
            double Final_OthersHour = 0;
            int site_id = 0;
            String sWTG_Name = "";
            string qry = "SELECT fd.site_id,fd.bd_type,bd.bd_type_name, SEC_TO_TIME(SUM(TIME_TO_SEC( fd.`total_stop` ) ) ) AS totalTime FROM `uploading_file_breakdown` as fd join bd_type as bd on bd.bd_type_id=fd.bd_type where `date` = '2022-10-18' group by wtg, fd.bd_type";

            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    DateTime result;
                    TimeSpan Get_Time;
                    var bd_type = reader["bd_type"];
                    var bd_type_name = reader["bd_type_name"];
                    var totalTime = reader["totalTime"];
                    site_id = reader["site_id"].ToInt();
                    sWTG_Name = "";//??

                    if (bd_type_name.Equals("USMH"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_USMH = Get_Time.TotalDays;
                    }
                    if (bd_type_name.Equals("SMH"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_SMH = Get_Time.TotalDays;
                    }
                    if (bd_type_name.Equals("IGBD"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_IGBD = Get_Time.TotalDays;
                    }
                    if (bd_type_name.Equals("EGBD"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_EGBD = Get_Time.TotalDays;
                    }
                    if (bd_type_name.Equals("LoadShedding"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_LoadShedding = Get_Time.TotalDays;
                    }
                    if (bd_type_name.Equals("OthersHour"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_OthersHour = Get_Time.TotalDays;
                    }

                }
                //Task<List<FormulaIds>>get_FromulaType(int site_id);
                // List<FormulaIds> iResult = new List<FormulaIds>()
               
                var iResult = CalculateAndUpdateKPIs(site_id, sWTG_Name, Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding);

                // CalculateFormula(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding);
            }

            return 0; // _BDTypeList;
            
        }


        double CalculateAndUpdateKPIs(int site, string sWTG_Name, double Final_USMH = 0, double Final_SMH = 0, double Final_IGBD = 0, double Final_EGBD = 0, double Final_OthersHour = 0, double Final_LoadShedding = 0)
        {
            int MA_Actual_FormulaID = 0;
            int MA_Contractual_FormulaID = 0;
            int IGA_FormulaID = 0;
            int EGA_FormulaID = 0;
            string qry = "SELECT * FROM `wind_site_formulas` where site_id = '" + site + "'";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(qry, conn);

            using (var reader = cmd.ExecuteReader())
            {
                //  map[0] = new System.Collections.Generic.Dictionary<string, object>();
                while (reader.Read())
                {
                    MA_Actual_FormulaID = (int)reader["MA_Actual"];
                    MA_Contractual_FormulaID = (int)reader["MA_Contractual"];
                    IGA_FormulaID = (int)reader["IGA"];
                    EGA_FormulaID = (int)reader["EGA"];
                }
            }


           // string sWTG_Name = ""

           // double dMA_ACT  = GetCalculatedValue(Final_USMH, Final_SMH, 0, 0, 0, MA_Actual_FormulaID);

           // double dMA_CON  = GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, 0, Final_OthersHour, MA_Contractual_FormulaID);
           // double dIGA     = GetCalculatedValue(0, 0, Final_IGBD, 0, 0, Internal_Grid, IGA_FormulaID,"");
           /// double dEGA     = GetCalculatedValue(0, 0, 0, Final_EGBD, 0, External_Grid, EGA_FormulaID);

            //Write update query for this device to update KPIs
            //string qryUpdate = "UPDATE `daily_gen_summary` set ma_actual = " + dMA_ACT + " ma_contractual = " + dMA_CON + " iga = " + dIGA + " ega = " + dEGA + " where site_id = '" + site + "' AND wtg = '" + sWTG_Name + "'";

            //close database connection
            conn.Close();
            return 0;
        }

        double GetCalculatedValue(double U = 0, double S = 0, double IG = 0, double EG = 0, double O = 0, string FormulaType = "")
        {

            // string qry= "SELECT * FROM `Wind_site_formulas` where site_id = 1";
            // IEnumerable<Calcultation> data = db.Database.SqlQuery<Calcultation>(qry);
            double returnValue = 0;

            switch (FormulaType)
            {
                case MA_Actual: //Machine Availability Actual
                    returnValue = (24 - (U + S)) / 24;
                    break;
                case MA_Contractual: //Machine Availability Contractual
                    returnValue = (24 - (U + S + IG)) / 24;
                    break;
                case Internal_Grid://Internal Grid Availability 
                    returnValue = (24 - (IG)) / 24;
                    break;
                case External_Grid: //External Grid Availablity
                    returnValue = (24 - (EG)) / 24;
                    break;
                default:
                    break;
            }
            return returnValue;
        }

        //public  get_FromulaType(int site)
        //public <List<FormulaIds>> private get_FromulaType(int site)
        internal async Task<List<FormulaIds>> get_FromulaType(int site)
        {
            int formula_ID = 0;
           // System.Collections.Generic.Dictionary<string, object>[] map = new System.Collections.Generic.Dictionary<string, object>[1];
            string qry = "SELECT * FROM `wind_site_formulas` where site_id = '"+ site + "'";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            List<FormulaIds> IDs = new List<FormulaIds>();
            using (var reader = cmd.ExecuteReader())
            {

              //  map[0] = new System.Collections.Generic.Dictionary<string, object>();
                while (reader.Read())
                {

                     IDs = new List<FormulaIds>() {
                        new FormulaIds { MA_Actual = (string)reader["MA_Actual"] }, 
                        new FormulaIds{MA_Contractual = (string)reader["MA_Contractual"] },
                        new FormulaIds{IGA =(string)reader["IGA"] } ,
                        new FormulaIds{EGA = (string)reader["EGA"]},
                        };

                    /*map[0].Add("MA_Actual", reader["MA_Actual"]);
                    map[0].Add("MA_Contractual", reader["MA_Contractual"]);
                    map[0].Add("IGA", reader["IGA"]);
                    map[0].Add("EGA", reader["EGA"]);*/
                    
                }
               
                 
            }
           

            return IDs;
            //conn.Close();
            //  return formula_ID;
            //return formula_ID;


        }

       
        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }


        public class FormulaIds
        {
            public string MA_Actual;
            public string MA_Contractual;
            public string IGA;
            public string EGA;
        }
    }
}
