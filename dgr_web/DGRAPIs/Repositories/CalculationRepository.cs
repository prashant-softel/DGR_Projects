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
       public int DailyKPICalculation_Wind(string sDate, int site_id)
             // internal async Task<List<BDTypeList>> DailyKPICalculation_Wind()
       {
            //string sDate = "2022-10-18";
            //int siteId = 1;

            double Final_USMH = 0;
            double Final_SMH = 0;
            double Final_IGBD = 0;
            double Final_EGBD = 0;
            double Final_LoadShedding = 0;
            double Final_LULL = 0;
            double Final_OthersHour = 0;
            //int site_id = 0;
            //string sWTG_Name = "";

            string MA_Actual_FormulaID = "";
            string MA_Contractual_FormulaID = "";
            string IGA_FormulaID = "";
            string EGA_FormulaID = "";
            string qry = "SELECT * FROM `wind_site_formulas` where site_id = '" + site_id + "'";
            MySqlConnection conn = new MySqlConnection(connString);
            conn.Open();
            MySqlCommand cmd = new MySqlCommand(qry, conn);
            string sCurrentWTG = "";
            string sLastWTG = "";

            int iCount = 0;
            using (var reader = cmd.ExecuteReader())
            {
                //  map[0] = new System.Collections.Generic.Dictionary<string, object>();
                while (reader.Read())
                {
                    MA_Actual_FormulaID = (string)reader["MA_Actual"];
                    MA_Contractual_FormulaID = (string)reader["MA_Contractual"];
                    IGA_FormulaID = (string)reader["IGA"];
                    EGA_FormulaID = (string)reader["EGA"];
                    break;
                }
            }

            
            qry = "SELECT fd.site_id,fd.bd_type,fd.wtg,bd.bd_type_name, SEC_TO_TIME(SUM(TIME_TO_SEC( fd.`total_stop` ) ) ) AS totalTime FROM `uploading_file_breakdown` as fd join bd_type as bd on bd.bd_type_id=fd.bd_type where site_id = " + site_id + " AND`date` = '" + sDate +"' group by fd.wtg, fd.bd_type";
            MySqlCommand cmd2 = new MySqlCommand(qry, conn);
            string sLog = "";
            using (var reader = cmd2.ExecuteReader())
            {
                while (reader.Read())
                {
                    iCount++;
                    DateTime result;
                    TimeSpan Get_Time;
                    site_id             = reader["site_id"].ToInt();
                    sCurrentWTG         = (string)reader["wtg"];
                    var bd_type         = reader["bd_type"];
                    var bd_type_name    = reader["bd_type_name"];
                    var totalTime       = reader["totalTime"];

                    if(iCount == 1)
                    {
                        sLastWTG    = sCurrentWTG;
                    }
                    if (sCurrentWTG != sLastWTG)
                    {
                        //Update WTG KPIs
                        CalculateAndUpdateKPIs(site_id, sLastWTG, Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Actual_FormulaID, MA_Contractual_FormulaID, IGA_FormulaID, EGA_FormulaID, conn);
                        sLastWTG = sCurrentWTG;
                        Final_USMH = 0;
                        Final_SMH = 0;
                        Final_IGBD = 0;
                        Final_EGBD = 0;
                        Final_LoadShedding = 0;
                        Final_LULL = 0;
                        Final_OthersHour = 0;
                    }
                    if (bd_type_name.Equals("USMH"))            //Pending : optimise it use bd_type id
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_USMH = Get_Time.TotalDays;
                    }
                    else if (bd_type_name.Equals("SMH"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_SMH = Get_Time.TotalDays;
                    }
                    else if (bd_type_name.Equals("IGBD"))
                    {
                        result = Convert.ToDateTime(totalTime.ToString());
                        Get_Time = result.TimeOfDay;
                        Get_Time = Get_Time * 24;
                        Final_IGBD = Get_Time.TotalDays;
                    }
                    else if (bd_type_name.Equals("EGBD"))
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

                var iResult = CalculateAndUpdateKPIs(site_id, sCurrentWTG, Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Actual_FormulaID, MA_Contractual_FormulaID, IGA_FormulaID, EGA_FormulaID, conn);

                //Task<List<FormulaIds>>get_FromulaType(int site_id);
                // List<FormulaIds> iResult = new List<FormulaIds>()

            }
            conn.Close();
            return 0; // _BDTypeList;
            
        }


        double CalculateAndUpdateKPIs(int site_id, string sWTG_Name, double Final_USMH, double Final_SMH, double Final_IGBD, double Final_EGBD, double Final_OthersHour, double Final_LoadShedding, string MA_Actual_FormulaID, string MA_Contractual_FormulaID, string IGA_FormulaID, string EGA_FormulaID, MySqlConnection conn)
        {
            //Log the result
            string sLog = "Updating WTG <" + sWTG_Name + "> KPI paramters.";

            double dMA_ACT  = GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Actual_FormulaID);
            double dMA_CON  = GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Contractual_FormulaID);
            double dIGA     = GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, IGA_FormulaID);
            double dEGA     = GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, EGA_FormulaID);

            //Write update query for this device to update KPIs
            //string qryUpdate = "UPDATE `daily_gen_summary` set ma_actual = " + dMA_ACT + ", ma_contractual = " + dMA_CON + ", iga = " + dIGA + ", ega = " + dEGA + " where site_id = '" + site_id + "' AND wtg = '" + sWTG_Name + "'";
            string qryUpdate = "UPDATE `daily_gen_summary` set ma_actual = " + dMA_ACT + ", ma_contractual = " + dMA_CON + ", iga = " + dIGA + ", ega = " + dEGA + " where wtg = '" + sWTG_Name + "'";
            MySqlCommand cmd2 = new MySqlCommand(qryUpdate, conn);

            //close database connection
            //conn.Close();
            return 0;
        }

        double GetCalculatedValue(double U, double S, double IG, double EG, double OthersHour, double LoadShedding, string Formula)
        {
            double returnValue = 0;
            switch (Formula)
            {
                case "24-(USMH+SMH))/24": /*MA_Actual_FormulaID*/ //Machine Availability Actual
                    returnValue = (24 - (U + S)) / 24;
                    break;
                case "(24-(USMH+SMH+IG))/24": /*MA_Contractual_FormulaID*/ //Machine Availability Contractual
                    returnValue = (24 - (U + S + IG)) / 24;
                    break;
                case "(24-(IG))/24" ://Internal Grid Availability 
                    returnValue = (24 - (IG)) / 24;
                    break;
                case "(24-(EG))/24": /*External_Grid_FormulaID*///External Grid Availablity
                    returnValue = (24 - (EG)) / 24;
                    break;
                default:
                    break;
            }
            return returnValue * 100;
        }

        //public  get_FromulaType(int site)
        //public <List<FormulaIds>> private get_FromulaType(int site)
        internal async Task<List<FormulaIds>> get_FromulaType(int site)
        {
            //int formula_ID = 0;
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
