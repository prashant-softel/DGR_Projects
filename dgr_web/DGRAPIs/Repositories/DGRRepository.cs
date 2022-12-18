using DGRAPIs.Helper;
using DGRAPIs.Models;
using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Data;
namespace DGRAPIs.Repositories
{
    
    public class DGRRepository : GenericRepository
    {
        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB;
        
        public const string MA_Actual = "MA_Actual";
        public const string MA_Contractual = "MA_Contractual";
        public const string Internal_Grid = "Internal_Grid";
        public const string External_Grid = "External_Grid";
        private int approve_status = 0;
        private bool m_bSiteMasterLoaded = false;
        private static readonly string[] Months = new[]
        {
            "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
        };

        public DGRRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {
            getDB = sqlDBHelper;
        }

        internal async Task<List<FinancialYear>> GetFinancialYear()
        {
            List<FinancialYear> _FinancialYear = new List<FinancialYear>();
            _FinancialYear.Add(new FinancialYear { financial_year = "2020-21" });
            _FinancialYear.Add(new FinancialYear { financial_year = "2021-22"});
            _FinancialYear.Add(new FinancialYear { financial_year = "2022-23" });
            _FinancialYear.Add(new FinancialYear { financial_year = "2023-24" });
            return _FinancialYear;

        }
        internal async Task<List<WindDashboardData>> GetWindDashboardDataOld(string startDate, string endDate, string FY, string sites)
        {
           
            string filter = " (date >= '" + startDate + "'  and date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }

            }
            string qry = @"select Date,month(date)as month,year(date)as year,Site,(sum(wind_speed)/count(*))as Wind,sum(kwh)as KWH,(select  replace(line_loss,'%','')as line_loss from monthly_uploading_line_losses where fy='" + FY + "' and month=DATE_FORMAT(t1.date, '%b') and site=t1.site order by monthly_uploading_line_losses_id desc limit 1) as line_loss,sum(kwh)-(sum(kwh)*((select  replace(line_loss,'%','')as line_loss from monthly_uploading_line_losses where fy='" + FY + "' and month=DATE_FORMAT(t1.date, '%b') and site=t1.site order by monthly_uploading_line_losses_id desc limit 1)/100))as jmrkwh,(select (kwh*1000000)as tarkwh from daily_target_kpi where site=t1.site and date=t1.date order by daily_target_kpi_id desc limit 1)as tarkwh ,(select (sum(wind_speed)/(select count(*) from daily_target_kpi where site=t2.site and date=t2.date))as tarwind from daily_target_kpi t2  where site=t1.site and date=t1.date)as tarwind from daily_gen_summary t1 where  " + filter + " group by Site,date order by date desc";


            //t1 where t1.approve_status="+approve_status+" and " + filter + " group by Site,date order by date desc";


            //(date>='2021-04-01'  and date<='2022-03-31')
            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }
        
        internal async Task<List<WindDashboardData1>> GetWindDashboardData(string startDate, string endDate, string FY, string sites)
        {
            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }

            //string qry = @"select t1.Date,month(t1.date) as month,year(t1.date) as year,t1.Site, (sum(t1.wind_speed) / count(*)) as Wind, sum(t1.kwh) - (sum(t1.kwh) * replace(t2.line_loss, '%', '') / 100) as jmrkwh, (t3.kwh * 1000000) as tarkwh, avg(t3.wind_speed) as tarwind from daily_gen_summary t1 left join monthly_uploading_line_losses t2 on t2.site_id = t1.site_id and t2.fy ='" + FY + "' left join daily_target_kpi t3 on t3.site_id = t1.site_id and t3.date = t1.date where " + filter + " group by t1.date order by t1.date desc";
           // string qry = @"select t1.Date,month(t1.date) as month,year(t1.date) as year,t1.Site, (sum(t1.wind_speed) / count(*)) as Wind, sum(kwh_afterlineloss) as jmrkwh, (t3.kwh * 1000000) as tarkwh, avg(t3.wind_speed) as tarwind from daily_gen_summary t1 left join monthly_uploading_line_losses t2 on t2.site_id = t1.site_id and t2.fy ='" + FY + "' left join daily_target_kpi t3 on t3.site_id = t1.site_id and t3.date = t1.date where " + filter + " group by t1.date order by t1.date desc";

            string qry = @"select t1.site_id,t1.Date,month(t1.date) as month,year(t1.date) as year,t1.Site, (sum(t1.wind_speed) / count(*)) as Wind, sum(kwh_afterlineloss) as jmrkwh, (t2.kwh) as tarkwh, avg(t2.wind_speed) as tarwind from daily_gen_summary t1 left join daily_target_kpi as t2 on t2.site_id = t1.site_id and t2.date = t1.date and t2.fy = '" + FY + "' where " + filter + " group by t1.date order by t1.date desc";

            List<WindDashboardData1> _WindDashboardData = await Context.GetData<WindDashboardData1>(qry).ConfigureAwait(false);
            return _WindDashboardData;

            /*string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }

            string temptable = "tempwindData";
            string table = "tblwindData";

            string q1 = "SELECT * FROM information_schema.tables WHERE  table_name = '"+ table + "' LIMIT 1; ";

            int outcount = await Context.CheckGetData(q1).ConfigureAwait(false);
            if(outcount>0)
            {
                string filter1 = "(date >= '" + startDate + "'  and date<= '" + endDate + "')";
                if (!string.IsNullOrEmpty(sites) && sites != "All")
                {
                    string[] siteSplit = sites.Split("~");
                    if (siteSplit.Length > 0)
                    {
                        string sitesnames = "";
                        for (int i = 0; i < siteSplit.Length; i++)
                        {
                            if (!string.IsNullOrEmpty(siteSplit[i]))
                            {
                                sitesnames += "'" + siteSplit[i] + "',";
                            }
                        }
                        sitesnames = sitesnames.TrimEnd(',');
                        filter1 += " and site in(" + sitesnames + ")";
                    }

                }


                q1 = "SELECT * FROM " + table + " where " + filter1 + " group by Site,date order by date desc;";
            List<WindDashboardData> _WindDashboardData =  await Context.GetData<WindDashboardData>(q1).ConfigureAwait(false);
                return _WindDashboardData;
            }
            else
            {
                string qry = @"drop TEMPORARY table IF EXISTS " + temptable + " ;  CREATE TEMPORARY TABLE " + temptable + " select  t1.Date,month(t1.date)as month,year(t1.date)as year, t1.Site,(sum(t1.wind_speed)/count(*))as Wind, sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,  (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1 left join monthly_uploading_line_losses t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site=t1.site and t3.date=t1.date where  " + filter + " group by t1.Site,t1.date order by t1.date desc ; CREATE TABLE " + table + " LIKE " + temptable + ";insert into " + table + " select * from " + temptable + ";select * from " + temptable +";";


                //t1.approve_status = "+approve_status+" and " + filter + " group by t1.Site,t1.date order by t1.date desc; CREATE TABLE " + table + " LIKE " + temptable + "; insert into " + table + " select* from " + temptable + "; select* from " + temptable +"; ";
            

        }*/


        }

        internal async Task<List<WindDashboardData>> GetWindDashboardData_old_AdminApproval(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }
              
                string qry = @"select  Date,month,year, Site,(sum(wind)/count(*))as Wind,
 sum(kwh)as KWH,line_loss, sum(jmrkwh) as jmrkwh, tarkwh, avg(tarwind) as tarwind 
 from  tblwinddata t1 where  " + filter + " group by t1.Site,t1.date order by t1.date desc ;";

             
               return await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            
        }

        internal async Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string FY, string sites,string date)
        {

            

            //string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and t1.date='"+date+"' ";
            string filter = " t1.date='" + date + "' ";
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";
                
            }
            /*if (!string.IsNullOrEmpty(sites))
             {
                 string[] siteSplit = sites.Split("~");
                 if (siteSplit.Length > 0)
                 {
                     string sitesnames = "";
                     for (int i = 0; i < siteSplit.Length; i++)
                     {
                         if (!string.IsNullOrEmpty(siteSplit[i]))
                         {
                             sitesnames += "'" + siteSplit[i] + "',";
                         }
                     }
                     sitesnames = sitesnames.TrimEnd(',');
                     filter += " and t1.site in(" + sitesnames + ")";
                 }

             } */

            /*string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
  sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
  (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
  left join monthly_uploading_line_losses t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site=t1.site and t3.date=t1.date where " + filter + " group by t1.Site,t1.date order by t1.date desc";*/
            
            /*string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where " + filter + " group by t1.Site,t1.date order by t1.date desc";*/
            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(kwh_afterlineloss) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where " + filter + " group by t1.Site,t1.date order by t1.date desc";

            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }

        internal async Task<List<WindDashboardData>> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and month(t1.date)=" + month + " ";
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }
            /*if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }*/
            /*string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where  " + filter + " group by t1.Site,month(t1.date) order by t1.date desc";*/

            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(kwh_afterlineloss) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where  " + filter + " group by t1.Site,month(t1.date) order by t1.date desc";
            
            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }
        internal async Task<List<WindDashboardData>> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')  ";
            /*if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }*/
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }
           /* string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where " + filter + " group by t1.Site,month(t1.date),year(t1.date) order by t1.date desc";*/

            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(kwh_afterlineloss) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site_id=t1.site_id and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site_id=t1.site_id and t3.date=t1.date where " + filter + " group by t1.Site,month(t1.date),year(t1.date) order by t1.date desc";

           

            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }

        internal async Task<List<SolarDashboardData>> GetSolarDashboardDataold(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(date >= '" + startDate + "'  and date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }

            }
            string qry = @"select Date,month(date)as month,year(date)as year,Site,(sum(poa)/count(*))as IR,sum(inv_kwh)as inv_kwh,(select  replace(lineloss,'%','')as line_loss
from monthly_line_loss_solar where fy='" + FY + "' and month=DATE_FORMAT(t1.date, '%b') and site=t1.site order by monthly_line_loss_solar_id desc limit 1) as line_loss,sum(inv_kwh)-(sum(inv_kwh)*((select  replace(lineloss,'%','')as line_loss  from monthly_line_loss_solar where fy='" + FY + "' and month=DATE_FORMAT(t1.date, '%b')  and site=t1.site order by monthly_line_loss_solar_id desc limit 1)/100))as jmrkwh, (select (gen_nos*1000000)as tarkwh from daily_target_kpi_solar where site=t1.site and date=t1.date    order by daily_target_kpi_solar_id desc limit 1)as tarkwh , (select (sum(poa)/(select count(*) from daily_target_kpi_solar where site=t1.site and date=t2.date order by daily_target_kpi_solar_id desc limit 1))as tarwind  from daily_target_kpi_solar t2  where site=t1.site and date=t1.date)as tarIR from daily_gen_summary_solar t1 where " + filter + " group by Site,date order by date desc";

            //from daily_target_kpi_solar t2  where site=t1.site and date=t1.date)as tarIR from daily_gen_summary_solar t1 where t1.approve_status="+approve_status+" and " + filter + " group by Site,date order by date desc";

            return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);


        }
        internal async Task<List<SolarDashboardData1>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')";
            /*if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }*/
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }
            string qry = @"select t1.date,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,(t2.gen_nos * 1000000) as tarkwh, avg(t2.poa) as tarIR from daily_gen_summary_solar t1 left join daily_target_kpi_solar t2 on t2.sites = t1.site and t2.date = t1.date where "+ filter + " group by t1.date order by t1.date desc";

            List<SolarDashboardData1> _SolarDashboardData = await Context.GetData<SolarDashboardData1>(qry).ConfigureAwait(false);
            return _SolarDashboardData;
           
            //            string qry = @" select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,
            //replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,
            //(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 
            //left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date  where " + filter + "  group by t1.Site,t1.date order by t1.date desc ";
            /* string table = "tblsolardata";
             string temptable = "tempsolardata";

             string q1 = "SELECT * FROM information_schema.tables WHERE  table_name = '" + table + "' LIMIT 1; ";

             int outcount = await Context.CheckGetData(q1).ConfigureAwait(false);
             if (outcount > 0)
             {
                 string filter1 = "(date >= '" + startDate + "'  and date<= '" + endDate + "')";
                 if (!string.IsNullOrEmpty(sites) && sites != "All")
                 {
                     string[] siteSplit = sites.Split("~");
                     if (siteSplit.Length > 0)
                     {
                         string sitesnames = "";
                         for (int i = 0; i < siteSplit.Length; i++)
                         {
                             if (!string.IsNullOrEmpty(siteSplit[i]))
                             {
                                 sitesnames += "'" + siteSplit[i] + "',";
                             }
                         }
                         sitesnames = sitesnames.TrimEnd(',');
                         filter1 += " and site in(" + sitesnames + ")";
                     }

                 }

                 q1 = "SELECT * FROM " + table + " where " + filter1 + "  group by site, date order by date desc;";

                 List<SolarDashboardData> _SolarDashboardData=await Context.GetData<SolarDashboardData>(q1).ConfigureAwait(false);
                 return _SolarDashboardData;
             }
             else 
             {
                 string qry = @"drop TEMPORARY table IF EXISTS " + temptable + " ; CREATE TEMPORARY TABLE " + temptable + " select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b')  and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date where " + filter + "  group by t1.site, t1.date order by t1.date desc ;CREATE TABLE "+ table + " LIKE "+ temptable + ";insert into "+ table + " select * from "+ temptable + ";select * from " + temptable + ";";

                 //approve_status + " and " + filter + "  group by t1.site, t1.date order by t1.date desc ;CREATE TABLE "+ table + " LIKE "+ temptable + ";insert into "+ table + " select * from "+ temptable + ";select * from " + temptable + ";";




                 return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);
             }*/



        }
        internal async Task<List<SolarDashboardData>> GetSolarDashboardData_old_AdminApproval(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')";
            if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }
 
                string qry = @"select Date,month,Site,sum(inv_kwh) as inv_kwh,avg(IR) as IR,
  line_loss,sum(jmrkwh) as jmrkwh,
  tarkwh, avg(tarIR) as tarIR from tblsolardata t1 where  " + filter + " group by t1.Site,t1.date order by t1.date desc ;";
             

                return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);
            



        }

       
        internal async Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string FY, string sites,string date)
        {

            string filter = " t1.date='"+date+"' ";
            /*if (!string.IsNullOrEmpty(sites) && sites != "All")
            {
                string[] siteSplit = sites.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t1.site in(" + sitesnames + ")";
                }

            }*/
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }

            string qry = @" select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,
replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,
(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 
left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date  where " + filter + "  group by t1.Site,t1.date order by t1.date desc ";

            // t3 on t3.sites=t1.site and t3.date=t1.date  where t1.approve_status=" + approve_status + " and " + filter + "  group by t1.Site,t1.date order by t1.date desc ";


            return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);


        }

        internal async Task<List<SolarDashboardData>> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and month(t1.date)=" + month + " ";
            /* if (!string.IsNullOrEmpty(sites) && sites != "All")
             {
                 string[] siteSplit = sites.Split("~");
                 if (siteSplit.Length > 0)
                 {
                     string sitesnames = "";
                     for (int i = 0; i < siteSplit.Length; i++)
                     {
                         if (!string.IsNullOrEmpty(siteSplit[i]))
                         {
                             sitesnames += "'" + siteSplit[i] + "',";
                         }
                     }
                     sitesnames = sitesnames.TrimEnd(',');
                     filter += " and t1.site in(" + sitesnames + ")";
                 }

             }*/
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }

            string qry = @" select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,
replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,
(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 
left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date  where   " + filter + "  group by t1.Site,month(t1.date) order by t1.date desc ";

            // t3 on t3.sites=t1.site and t3.date=t1.date  where t1.approve_status=" + approve_status + " and " + filter + "  group by t1.Site,month(t1.date) order by t1.date desc ";


            return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);


        }

        internal async Task<List<SolarDashboardData>> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') ";
            /* if (!string.IsNullOrEmpty(sites) && sites != "All")
             {
                 string[] siteSplit = sites.Split("~");
                 if (siteSplit.Length > 0)
                 {
                     string sitesnames = "";
                     for (int i = 0; i < siteSplit.Length; i++)
                     {
                         if (!string.IsNullOrEmpty(siteSplit[i]))
                         {
                             sitesnames += "'" + siteSplit[i] + "',";
                         }
                     }
                     sitesnames = sitesnames.TrimEnd(',');
                     filter += " and t1.site in(" + sitesnames + ")";
                 }

             }*/
            if (!string.IsNullOrEmpty(sites))
            {
                filter += " and t1.site_id in(" + sites + ")";

            }
            string qry = @" select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,
replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,
(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 
left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date  where  " + filter + "  group by t1.Site,month(t1.date),year(t1.date)  order by t1.date desc ";

            //t3 on t3.sites=t1.site and t3.date=t1.date  where t1.approve_status=" + approve_status + " and " + filter + "  group by t1.Site,month(t1.date),year(t1.date)  order by t1.date desc ";


            return await Context.GetData<SolarDashboardData>(qry).ConfigureAwait(false);


        }

        internal async Task<List<WindSiteMaster>> GetWindSiteMaster()
        {

            return await Context.GetData<WindSiteMaster>("Select * from site_master").ConfigureAwait(false);

        }
        internal async Task<List<WindLocationMaster>> GetWindLocationMaster()
        {

            return await Context.GetData<WindLocationMaster>("Select * from location_master").ConfigureAwait(false);

        }
        internal async Task<List<SolarSiteMaster>> GetSolarSiteMaster()
        {

            return await Context.GetData<SolarSiteMaster>("Select * from site_master_solar").ConfigureAwait(false);

        }
        internal async Task<List<SolarLocationMaster>> GetSolarLocationMasterBySite(string site)
        {

            return await Context.GetData<SolarLocationMaster>("Select location_master_solar_id,country,site,eg,ig,icr_inv,icr,inv,smb,string as strings,string_configuration,total_string_current,total_string_voltage,modules_quantity,wp,capacity,module_make,module_model_no,    module_type from location_master_solar where site='" + site + "'").ConfigureAwait(false);

        }
        internal async Task<List<SolarLocationMaster>> GetSolarLocationMaster()
        {

            return await Context.GetData<SolarLocationMaster>("Select  location_master_solar_id,country,site,eg,ig,icr_inv,icr,inv,smb,string as strings,string_configuration,total_string_current,total_string_voltage,modules_quantity,wp,capacity,module_make,module_model_no,    module_type  from location_master_solar").ConfigureAwait(false);

        }
        internal async Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg,string reportType)
        {
            string filter = " (date >= '" + fromDate + "'  and date<= '" + toDate + "')";
            if (!string.IsNullOrEmpty(site) && site != "All")
            {
                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string siteid = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            siteid += "'" + siteSplit[i] + "',";
                        }
                    }
                    siteid = siteid.TrimEnd(',');
                    filter += " and site in(" + siteid + ")";
                }
            }
            if (reportType == "WTG")
            {
                filter += " group by t1.wtg ";
            }
            if(reportType == "Site")
            {
                filter += " group by t1.site ";
            }
            string qry = @"SELECT (date),t2.country,t1.state,t2.spv,t1.site,t2.capacity_mw
,t1.wtg,wind_speed,kwh,plf,ma_actual,ma_contractual,iga,ega,grid_hrs,lull_hrs
,unschedule_hrs,schedule_hrs,others,igbdh,egbdh,load_shedding FROM daily_gen_summary t1 left join
site_master t2 on t1.site_id = t2.site_master_id
where   " + filter;
            List<WindDailyGenReports> _windDailyGenReports = new List<WindDailyGenReports>();
            _windDailyGenReports = await Context.GetData<WindDailyGenReports>(qry).ConfigureAwait(false);
            return _windDailyGenReports;
            /* string filter = "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";

             string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site,t2.capacity_mw
 ,t1.wtg,wind_speed,kwh,plf,ma_actual,ma_contractual,iga,ega,grid_hrs,lull_hrs
 ,unschedule_hrs,schedule_hrs,others,igbdh,egbdh,load_shedding	 FROM daily_gen_summary t1 left join
 site_master t2 on t1.site=t2.site 
 where   " + filter;

             //where t1.approve_status="+approve_status+" and " + filter;

             return await Context.GetData<WindDailyGenReports>(qry).ConfigureAwait(false);*/

        }

        internal async Task<List<WindDailyGenReports1>> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {

           
            /// Not Use
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
            //  (t1.date >= '2021-12-01'  and t1.date <= '2021-12-02')
            //and t2.country in ('India')
            //and t1.state in('TS')
            //and t2.spv in ('CWP Ananthpur')
            //and t1.site in ('Zaheerabad')
            //and t1.wtg in('ZHB04')


            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site
            ,t1.wtg,wind_speed as wind_speed,kwh as kwh,plf as plf,ma_actual as ma_actual,ma_contractual as ma_contractual,iga as iga,ega as ega,production_hrs as grid_hrs,lull_hrs as lull_hrs
            ,unschedule_hrs as unschedule_hrs,schedule_hrs as schedule_hrs,others as others,igbdh as igbdh,egbdh as egbdh,load_shedding as load_shedding	 FROM daily_gen_summary t1 left join
            site_master t2 on t1.site=t2.site 
            where    " + filter + " group by t1.date, t1.state, t2.spv, t1.site, t1.wtg";

            // where  t1.approve_status="+approve_status+" and " + filter + " group by t1.date, t1.state, t2.spv, t1.site, t1.wtg";




            //string qry = "select * from view_getwinddailygensummaryreport1 where "+filter+ " group by t1.date, t1.state, t2.spv, t1.site, t1.wtg ";
            return await Context.GetData<WindDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports2>> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            //sitewisewinddaily
            
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
           

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site,
t2.total_mw
,(sum(wind_speed)/count(*)) as wind_speed,
sum(kwh) as kwh,
(sum(plf)/count(*))as plf,
(sum(ma_actual)/count(*))as ma_actual,
(sum(ma_contractual)/count(*))as ma_contractual,
(sum(iga)/count(*))as iga,
(sum(ega)/count(*))as ega,
sum(production_hrs)as grid_hrs,
sum(lull_hrs)as lull_hrs
,sum(unschedule_hrs)as unschedule_hrs,
sum(schedule_hrs)as schedule_hrs,
sum(others) as others,
sum(igbdh)as igbdh,
sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site_id=t2.site_master_id 
where " + filter + " group by t1.date, t1.state, t2.spv, t1.site ";

            //where  t1.approve_status="+approve_status+" and " + filter + " group by t1.date, t1.state, t2.spv, t1.site ";

            return await Context.GetData<WindDailyGenReports2>(qry).ConfigureAwait(false);

        }
        //  GetWindMonthlyYearlyGenSummaryReport1 Function name Renamed
        internal async Task<List<WindDailyGenReports1>> GetWindMonthlyGenerationReport(string fromDate, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {
            
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                //filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                filter += "MONTH(t1.date)=MONTH('"+fromDate+"')";
               // MONTH(`date`) = MONTH('2022-04-01')
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
          
            string qry = @"SELECT year(date)as year,DATE_FORMAT(date,'%M') as month,date,t2.country,t1.state,t2.spv,t1.site,
t1.wtg
,(sum(wind_speed)/count(*))as wind_speed,
sum(kwh) as kwh,
(sum(plf)/count(*))as plf,
(sum(ma_actual)/count(*))as ma_actual,
(sum(ma_contractual)/count(*))as ma_contractual,
(sum(iga)/count(*))as iga,
(sum(ega)/count(*))as ega,
sum(production_hrs)as grid_hrs,
sum(lull_hrs)as lull_hrs
,sum(unschedule_hrs)as unschedule_hrs,
sum(schedule_hrs)as schedule_hrs,
sum(others) as others,
sum(igbdh)as igbdh,
sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site=t2.site 
where   " + filter + " group by t1.state, t2.spv, t1.wtg , month(t1.date)";

            //where t1.approve_status="+approve_status+" and " + filter + " group by t1.state, t2.spv, t1.wtg , month(t1.date)";

            return await Context.GetData<WindDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string month, string country, string state, string spv, string site, string wtg)
        {
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                //filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                filter += "MONTH(t1.date)=MONTH('" + fromDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
            //  (t1.date >= '2021-12-01'  and t1.date <= '2021-12-02')
            //and t2.country in ('India')
            //and t1.state in('TS')
            //and t2.spv in ('CWP Ananthpur')
            //and t1.site in ('Zaheerabad')
            //and t1.wtg in('ZHB04')


            string qry = @"SELECT year(date)as year,DATE_FORMAT(date,'%M') as month,date,t2.country,t1.state,t2.spv,t1.site,
t2.total_mw
,(sum(wind_speed)/count(*))as wind_speed,
sum(kwh) as kwh,
(sum(plf)/count(*))as plf,
(sum(ma_actual)/count(*))as ma_actual,
(sum(ma_contractual)/count(*))as ma_contractual,
(sum(iga)/count(*))as iga,
(sum(ega)/count(*))as ega,
sum(production_hrs)as grid_hrs,
sum(lull_hrs)as lull_hrs
,sum(unschedule_hrs)as unschedule_hrs,
sum(schedule_hrs)as schedule_hrs,
sum(others) as others,
sum(igbdh)as igbdh,
sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site=t2.site 
where   " + filter + " group by t1.state, t2.spv, t1.site , month(t1.date)";

            //where  t1.approve_status="+approve_status+" and " + filter + " group by t1.state, t2.spv, t1.site , month(t1.date)";

            return await Context.GetData<WindDailyGenReports2>(qry).ConfigureAwait(false);

        }
        // WTG WIse
        internal async Task<List<WindDailyGenReports1>> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
           

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site,
t1.wtg
,(sum(wind_speed)/count(*))as wind_speed,
sum(kwh) as kwh,
(sum(plf)/count(*))as plf,
(sum(ma_actual)/count(*))as ma_actual,
(sum(ma_contractual)/count(*))as ma_contractual,
(sum(iga)/count(*))as iga,
(sum(ega)/count(*))as ega,
sum(production_hrs)as grid_hrs,
sum(lull_hrs)as lull_hrs
,sum(unschedule_hrs)as unschedule_hrs,
sum(schedule_hrs)as schedule_hrs,
sum(others) as others,
sum(igbdh)as igbdh,
sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site_id=t2.site_master_id 
where   " + filter + " group by t1.state, t2.spv, t1.wtg ";

            //where  t1.approve_status=" + approve_status + " and " + filter + " group by t1.state, t2.spv, t1.wtg ";

            return await Context.GetData<WindDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports2>> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spmonth = month.Split("~");
                filter += "month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }
            //  (t1.date >= '2021-12-01'  and t1.date <= '2021-12-02')
            //and t2.country in ('India')
            //and t1.state in('TS')
            //and t2.spv in ('CWP Ananthpur')
            //and t1.site in ('Zaheerabad')
            //and t1.wtg in('ZHB04')


            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site,
t2.total_mw
,(sum(wind_speed)/count(*))as wind_speed,
sum(kwh) as kwh,
(sum(plf)/count(*))as plf,
(sum(ma_actual)/count(*))as ma_actual,
(sum(ma_contractual)/count(*))as ma_contractual,
(sum(iga)/count(*))as iga,
(sum(ega)/count(*))as ega,
sum(production_hrs)as grid_hrs,
sum(lull_hrs)as lull_hrs
,sum(unschedule_hrs)as unschedule_hrs,
sum(schedule_hrs)as schedule_hrs,
sum(others) as others,
sum(igbdh)as igbdh,
sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site_id=t2.site_master_id 
where    " + filter + " group by t1.state, t2.spv, t1.site  ";

            //where  t1.approve_status=" + approve_status + " and " + filter + " group by t1.state, t2.spv, t1.site  ";

            return await Context.GetData<WindDailyGenReports2>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(date >= '" + fromDate + "'  and date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t3.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t3.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t2.spv in (" + spv + ")";
                string[] spspv = spv.Split("~");
                filter += "t3.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.site in (" + site + ")";
                string[] spsite = site.Split("~");
                filter += "t2.site_master_id in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(wtg) && wtg != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.wtg in (" + wtg + ")";
                string[] spwtg = wtg.Split("~");
                filter += "t1.wtg in (";
                string wtgs = "";
                for (int i = 0; i < spwtg.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spwtg[i].ToString()))
                    {
                        wtgs += "'" + spwtg[i].ToString() + "',";
                    }
                }
                filter += wtgs.TrimEnd(',') + ")";
            }

            string qry = @"SELECT date,t1.wtg,bd_type,stop_from,stop_to,total_stop,error_description,action_taken,t3.country,t3.state,t3.spv, t2.site,t4.bd_type_name FROM uploading_file_breakdown t1 left join location_master t2 on t2.wtg=t1.wtg left join site_master t3 on t3.site_master_id=t2.site_master_id left join bd_type as t4 on t4.bd_type_id=t1.bd_type ";

            //t3.site=t2.site  where t1.approve_status="+approve_status+"";

            if (!string.IsNullOrEmpty(filter))
            {
                qry += " where  " + filter;
            }
            return await Context.GetData<WindDailyBreakdownReport>(qry).ConfigureAwait(false);

        }
        internal async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary()
        {
            string qry = @"SELECT *,
(SELECT dc_capacity FROM solar_ac_dc_capacity where inverter=t1.location_name)as dc_capacity,
(SELECT ac_capacity FROM solar_ac_dc_capacity where inverter=t1.location_name) as ac_capacity
 FROM daily_gen_summary_solar t1 ";

            //FROM daily_gen_summary_solar t1 where t1.approve_status=" + approve_status + "";

            return await Context.GetData<SolarDailyGenSummary>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports>> GetWindWtgFromdailyGenSummary(string state, string site)
        {
            string filter = "";
            int chkfilter = 0;

            if (!string.IsNullOrEmpty(state) && state!="All" && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                chkfilter = 1;

                string[] spstate = state.Split("~");
                filter += "state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site!="All" && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                chkfilter = 1;

                string[] spsite = site.Split("~");
                filter += "site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if(!string.IsNullOrEmpty(filter))
            {
                filter = " where " + filter;

            }
            string qry = @"select distinct wtg from daily_gen_summary " + filter;
            return await Context.GetData<WindDailyGenReports>(qry).ConfigureAwait(false);

        }
        internal async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_2(string fromDate, string toDate, string site)
        {



            /*string strPerformanceQuery = "select AVG(t1.wind_speed) as act_wind,AVG(t1.plf) as act_plf,AVG(t1.ma_actual) as act_ma,AVG(t1.iga) as act_iga,AVG(t1.ega) as act_ega,SUM(t1.kwh_afterloss) as act_jmr_kwh,SUM(t1.kwh_afterloss/1000000) as act_jmr_kwh_mu,SUM(t1.kwh) as total_mw,avg(t2.wind_speed) as tar_wind,AVG(t2.plf) as tar_plf,AVG(t2.ma) as tar_ma,AVG(t2.iga) as tar_iga,AVG(t2.ega) as tar_ega,SUM(t2.kwh*1000000) as tar_kwh,SUM(t2.kwh) as tar_kwh_mu from uploading_file_generation as t1 join daily_target_kpi as t2 on t2.date = t1.date where t1.date >= '" + fromDate + "'  and t1.date <= '" + toDate + "' and t1.site_id IN("+ site+")";*/

            string strPerformanceQuery = "select AVG(t1.wind_speed) as act_wind,AVG(t1.plf) as act_plf,AVG(t1.ma_actual) as act_ma,AVG(t1.iga) as act_iga,AVG(t1.ega) as act_ega,SUM(t1.kwh_afterlineloss) as act_jmr_kwh,SUM(t1.kwh_afterlineloss / 1000000) as act_jmr_kwh_mu,SUM(t1.kwh) as total_mw,avg(t2.wind_speed) as tar_wind,AVG(t2.plf) as tar_plf,AVG(t2.ma) as tar_ma,AVG(t2.iga) as tar_iga,AVG(t2.ega) as tar_ega,SUM(t2.kwh * 1000000) as tar_kwh,SUM(t2.kwh) as tar_kwh_mu from daily_gen_summary as t1 join daily_target_kpi as t2 on t2.date = t1.date where t1.date >= '" + fromDate + "'  and t1.date <= '" + toDate + "' and t1.site_id IN(" + site + ")";

            return await Context.GetData<WindPerformanceReports>(strPerformanceQuery).ConfigureAwait(false);
        }
        /*internal async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_3(string fromDate, string toDate, string site)
        {



            string strPerformanceQuery = "select AVG(t1.wind_speed) as act_wind,AVG(t1.plf) as act_plf,AVG(t1.ma_actual) as act_ma,AVG(t1.iga) as act_iga,AVG(t1.ega) as act_ega,SUM(t1.kwh_afterlineloss) as act_jmr_kwh,SUM(t1.kwh_afterlineloss / 1000000) as act_jmr_kwh_mu,SUM(t1.kwh) as total_mw,avg(t2.wind_speed) as tar_wind,AVG(t2.plf) as tar_plf,AVG(t2.ma) as tar_ma,AVG(t2.iga) as tar_iga,AVG(t2.ega) as tar_ega,SUM(t2.kwh * 1000000) as tar_kwh,SUM(t2.kwh) as tar_kwh_mu from daily_gen_summary as t1 join daily_target_kpi as t2 on t2.date = t1.date where t1.date >= '" + fromDate + "'  and t1.date <= '" + toDate + "' and t1.site_id IN(" + site + ")";

            return await Context.GetData<WindPerformanceReports>(strPerformanceQuery).ConfigureAwait(false);
        }
        internal async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise_4(string fromDate, string toDate, string site)
        {



           
            string strPerformanceQuery = "select AVG(t1.wind_speed) as act_wind,AVG(t1.plf) as act_plf,AVG(t1.ma_actual) as act_ma,AVG(t1.iga) as act_iga,AVG(t1.ega) as act_ega,SUM(t1.kwh_afterlineloss) as act_jmr_kwh,SUM(t1.kwh_afterlineloss / 1000000) as act_jmr_kwh_mu,SUM(t1.kwh) as total_mw,avg(t2.wind_speed) as tar_wind,AVG(t2.plf) as tar_plf,AVG(t2.ma) as tar_ma,AVG(t2.iga) as tar_iga,AVG(t2.ega) as tar_ega,SUM(t2.kwh * 1000000) as tar_kwh,SUM(t2.kwh) as tar_kwh_mu from daily_gen_summary as t1 join daily_target_kpi as t2 on t2.date = t1.date where t1.date >= '" + fromDate + "'  and t1.date <= '" + toDate + "' and t1.site_id IN(" + site + ")";

            return await Context.GetData<WindPerformanceReports>(strPerformanceQuery).ConfigureAwait(false);
        }*/
        internal async Task<List<WindPerformanceReports>> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate)
        {


            string qry = @" select site,
(select total_mw from site_master where site=t1.site)as total_mw, 
(select (sum(kwh)*1000000)as tarkwh from daily_target_kpi
 where site=t1.site and (date >= '" + fromDate + "'  and date<= '" + todate + "') )as tar_kwh ,(SELECT sum(kwh)as tarkwh FROM daily_target_kpi where fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "') and site=t1.site)as tar_kwh_mu,  (sum(kwh)-(sum(kwh)*((select  replace(line_loss,'%','')as line_loss  from monthly_uploading_line_losses where fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b') and site=t1.site order by monthly_uploading_line_losses_id desc limit 1)/100))) as act_jmr_kwh,  (sum(kwh)-(sum(kwh)*((select  replace(line_loss,'%','')as line_loss  from monthly_uploading_line_losses where fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b')  and site=t1.site order by monthly_uploading_line_losses_id desc limit 1)/100)))/1000000 as act_jmr_kwh_mu, (SELECT total_tarrif FROM site_master where site=t1.site)as total_tarrif,  (select (sum(wind_speed)/count(*)) as tarwind from daily_target_kpi t2  where  site=t1.site and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_wind, (sum(wind_speed)/count(*))as act_Wind, (SELECT (sum(plf)/count(*)) FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_plf, (sum(plf)/count(*))as act_plf,  (SELECT (sum(ma)/count(*)) FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_ma, (sum(ma_actual)/count(*)) as act_ma,  (SELECT (sum(iga)/count(*)) FROM daily_target_kpi where site=t1.site  and fy= '" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_iga, (sum(iga)/count(*)) as act_iga,   (SELECT (sum(ega)/count(*)) FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_ega, (sum(ega)/count(*)) as act_ega from daily_gen_summary t1 where  (date >= '" + fromDate + "'  and date<= '" + todate + "') group by site";

            //daily_gen_summary t1 where t1.approve_status=" + approve_status + " and (date >= '" + fromDate + "'  and date<= '" + todate + "') group by site";

            return await Context.GetData<WindPerformanceReports>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindPerformanceReports>> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {


            string qry = @" select  t1.site,t2.spv,
(select total_mw from site_master where site=t1.site)as total_mw, 
(select (sum(kwh)*1000000)as tarkwh from daily_target_kpi
 where site=t1.site and (date >= '" + fromDate + "'  and date<= '" + todate + "') )as tar_kwh ,(SELECT sum(kwh)as tarkwh FROM daily_target_kpi where fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "') and site=t1.site)as tar_kwh_mu,  (sum(kwh)-(sum(kwh)*((select  replace(line_loss,'%','')as line_loss  from monthly_uploading_line_losses where fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b') and site=t1.site order by monthly_uploading_line_losses_id desc limit 1)/100))) as act_jmr_kwh,  (sum(kwh)-(sum(kwh)*((select  replace(line_loss,'%','')as line_loss  from monthly_uploading_line_losses where fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b')  and site=t1.site order by monthly_uploading_line_losses_id desc limit 1)/100)))/1000000 as act_jmr_kwh_mu, (SELECT total_tarrif FROM site_master where site=t1.site)as total_tarrif,  (select (sum(wind_speed)/count(*))as tarwind from daily_target_kpi t2  where  site=t1.site and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_wind, (sum(wind_speed)/count(*))as act_Wind, (SELECT (sum(plf)/count(*)) FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_plf, (sum(plf)/count(*))as act_plf,  (SELECT (sum(ma)/count(*))  FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_ma, (sum(ma_actual)/count(*)) as act_ma,  (SELECT (sum(iga)/count(*)) FROM daily_target_kpi where site=t1.site  and fy= '" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_iga, (sum(iga)/count(*)) as act_iga,   (SELECT (sum(ega)/count(*)) FROM daily_target_kpi where site=t1.site  and fy='" + fy + "' and (date >= '" + fromDate + "'  and date<= '" + todate + "'))as tar_ega, (sum(ega)/count(*)) as act_ega from daily_gen_summary t1  left join site_master t2 on t1.site=t2.site where   (date >= '" + fromDate + "'  and date<= '" + todate + "') group by spv";

          

            return await Context.GetData<WindPerformanceReports>(qry).ConfigureAwait(false);

        }

        public async Task<List<SolarSiteMaster>> GetSolarSiteData(string state, string spv, string site)
        {
            string filter = "";
            filter += " where site_master_solar_id IN(" + site + ")";
            if (!string.IsNullOrEmpty(state) && string.IsNullOrEmpty(spv))
            {
                filter += " where state='" + state + "'";
            }
            if (!string.IsNullOrEmpty(spv) && !string.IsNullOrEmpty(state))
            {
                filter += " where state='" + state + "' and spv='" + spv + "'";
            }

            string query = "SELECT * FROM `site_master_solar`" + filter;
            List<SolarSiteMaster> _sitelist = new List<SolarSiteMaster>();
            _sitelist = await Context.GetData<SolarSiteMaster>(query).ConfigureAwait(false);
            return _sitelist;

        }
        internal async Task<List<SolarDailyBreakdownReport>> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site)
        {
            string filter = "";
            int chkfilter = 0;
            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += "(date >= '" + fromDate + "'  and date<= '" + toDate + "')";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                string[] spcountry = country.Split("~");
                filter += "t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";
                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                // filter += "t1.state in (" + state + ")";
                string[] spstate = state.Split("~");
                filter += "t2.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spspv = spv.Split("~");
                filter += "t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

                chkfilter = 1;
            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }

                string[] spsite = site.Split("~");
                filter += "t2.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";
                chkfilter = 1;
            }


            string qry = @"SELECT date,t2.country,t2.state,t2.spv,t2.site,
'' as breakdown,bd_type,icr,inv,smb,strings,stop_from as from_bd,stop_to as to_bd,total_stop,
remarks as bd_remarks,action as action_taken
 FROM daily_bd_loss_solar t1 left join site_master_solar t2 on t2.site=t1.site ";

            //FROM daily_bd_loss_solar t1 left join site_master_solar t2 on t2.site=t1.site where t1.approve_status="+ approve_status;

            if (!string.IsNullOrEmpty(filter))
            {
                qry += " where " + filter;
            }
            return await Context.GetData<SolarDailyBreakdownReport>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertDailyTargetKPI(List<WindDailyTargetKPI> set)
        {
            //pending : add activity log

            //check for existing records with date and site reference to delete existing records before inserting fresh data
            string delqry = "delete from daily_target_kpi where";
            string qry = "insert into daily_target_kpi (fy, date, site, site_id, wind_speed, kwh, ma, iga, ega, plf) values ";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.FY + "','" + unit.Date + "','" + unit.Site + "','" + unit.site_id + "','" + unit.WindSpeed + "','" + unit.kWh + "','" + unit.MA + "','" + unit.IGA + "','" + unit.EGA + "','" + unit.PLF + "'),";

                delqry += " site_id = " + unit.site_id + " and date = " + unit.Date + " and fy = '" + unit.FY + "' or";
            }
            qry += values;
            await Context.ExecuteNonQry<int>(delqry.Substring(0, (delqry.Length - 2)) + ";").ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);

            //string delqry = "";
            //for (int i = 0; i < windDailyTargetKPI.Count; i++)
            //{

            //    string dates = Convert.ToDateTime(unit.Date).ToString("yyyy-MM-dd");

            //    delqry += "delete from daily_target_kpi where fy='" + windDailyTargetKPI[i].FY + "' and date='" + dates + "' and site='" + windDailyTargetKPI[i].Site + "' ;";
            //}
            //await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            //string qry = "";
            //for (int i = 0; i < windDailyTargetKPI.Count; i++)
            //{

            //    string dates = Convert.ToDateTime(windDailyTargetKPI[i].Date).ToString("yyyy-MM-dd");
            //    string ma = Convert.ToString(windDailyTargetKPI[i].MA);
            //    string iga = Convert.ToString(windDailyTargetKPI[i].IGA);
            //    string ega = Convert.ToString(windDailyTargetKPI[i].EGA);
            //    string plf = Convert.ToString(windDailyTargetKPI[i].PLF);

            //    qry += "insert into daily_target_kpi (fy,date,site,wind_speed,kwh,ma,iga,ega,plf) values ('" + windDailyTargetKPI[i].FY + "','" + dates + "','" + windDailyTargetKPI[i].Site + "','" + windDailyTargetKPI[i].WindSpeed + "','" + windDailyTargetKPI[i].kWh + "','" + ma.TrimEnd('%') + "','" + iga.TrimEnd('%') + "','" + ega.TrimEnd('%') + "','" + plf.TrimEnd('%') + "');";
            //}
            //return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> set)
        {
            //pending : add log activity
            //pending : delete existing data and insert fresh data 
            string delqry = "delete from monthly_target_kpi where";
            string qry = " insert into monthly_target_kpi (fy, month, month_no, year, site,site_id, wind_speed, kwh, ma, iga, ega, plf) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.fy + "','" + unit.month + "','" + unit.month_no + "','" + unit.year + "','" + unit.site + "','" + unit.site_id + "','" + unit.windSpeed + "','" + unit.kwh + "','" + unit.ma + "','" + unit.iga + "','" + unit.ega + "','" + unit.plf + "'),";
                delqry += " (site_id = " + unit.site_id + " and year = " + unit.year + " and month = '" + unit.month + "') or";
            }
            qry += values;
            string mysqry = delqry.Substring(0, (delqry.Length - 2)) + ";";
            await Context.ExecuteNonQry<int>(mysqry).ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }


        internal async Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> set)
        {
            //pending : add log activity
            //pending : if record exists update it then call the CalculateAndUpdatePLFandKWHAfterLineLoss 
            //fetching table data
            string fetchQry = "select monthly_uploading_line_losses_id, fy, site, site_id, month, month_no as month_number, year,line_loss as lineLoss from monthly_uploading_line_losses ;";
            List<WindMonthlyUploadingLineLosses> tableData = new List<WindMonthlyUploadingLineLosses>();
            tableData = await Context.GetData<WindMonthlyUploadingLineLosses>(fetchQry).ConfigureAwait(false);
            WindMonthlyUploadingLineLosses existingRecord = new WindMonthlyUploadingLineLosses();

            string updateQry = "";
            string qry = " insert into monthly_uploading_line_losses (fy, site, site_id, month, month_no, year, line_loss) values";
            string values = "";
            foreach (var unit in set)
            {
                //checking if excel sheet row already exists as a record in db table and storing matching entries in an object
               // int update = 1;
                //try {
                    existingRecord = tableData.Find(tSite => tSite.site_id.Equals(unit.site_id) && tSite.year.Equals(unit.year) && tSite.month_no.Equals(unit.month_no));
               /* }
                catch (Exception)
                {
                    update = 0;
                }*/
                
                //if (string.IsNullOrEmpty(existingRecord.site))
                /*if(update == 0)
                {
                   // values += "('" + unit.fy + "','" + unit.site + "','" + unit.site_id + "','" + unit.month + "','" + unit.month_number + "','" + unit.year + "','" + unit.lineLoss + "'),";
                }
                else
                {*/
                    //if match is found
                    try
                    {
                        updateQry += "update monthly_uploading_line_losses set line_loss = " + unit.lineLoss + " where monthly_uploading_line_losses_id = " + existingRecord.monthly_uploading_line_losses_id + ";";
                        await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        values += "('" + unit.fy + "','" + unit.site + "','" + unit.site_id + "','" + unit.month + "','" + unit.month_no + "','" + unit.year + "','" + unit.lineLoss + "'),";
                    }
                
            }

            if (values != "") {
                qry += values;
                return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            return -1;

        }


        internal async Task<int> InsertWindJMR(List<WindMonthlyJMR> set)
        {
            //pending : add activity log
            //delete existing records with reference to site_id, month and year
            string delqry = "delete from monthly_jmr where ";

            //inserting new client data into wind:monthly_jmr
            string qry = " insert into monthly_jmr (FY, Site, site_id, Plant_Section, Controller_KWH_INV, Scheduled_Units_kWh, Export_kWh, Import_kWh, Net_Export_kWh, Export_kVAh, Import_kVAh, Export_kVArh_lag, Import_kVArh_lag, Export_kVArh_lead, Import_kVArh_lead, JMR_date, JMR_Month, JMR_Year, LineLoss, Line_Loss_percentage, RKVH_percentage) values";
            string values = "";
            foreach (var unit in set)
            {
                //values are now recording site_id
                values += "('" + unit.fy + "','" + unit.site + "','" + unit.siteId + "','" + unit.plantSection + "','" + unit.controllerKwhInv + "','" + unit.scheduledUnitsKwh + "','" + unit.exportKwh + "','" + unit.importKwh + "','" + unit.netExportKwh + "','" + unit.exportKvah + "','" + unit.importKvah + "','" + unit.exportKvarhLag + "','" + unit.importKvarhLag + "','" + unit.exportKvarhLead + "', '" + unit.importKvarhLead + "', '" + unit.jmrDate + "', '" + unit.jmrMonth + "', '" + unit.jmrYear + "', '" + unit.lineLoss + "', '" + unit.lineLossPercent + "', '" + unit.rkvhPercent + "'),";

                delqry += " JMR_date = " + unit.jmrDate + " and JMR_Month = '" + unit.jmrMonth + "' and JMR_Year = " + unit.jmrYear + " and site_id = " + unit.siteId + " or";
            }
            qry += values;
            await Context.ExecuteNonQry<int>(delqry.Substring(0, (delqry.Length - 2)) + ";").ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> set)
        {
            //pending : add activity log
            //deletes existing records with reference to site_id and date
            string delqry = "delete from daily_load_shedding where";
            //await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            //inserting new client data into wind:daily_load_shedding  
            string qry = " insert into daily_load_shedding (site_id, Site, Date, Start_Time, End_Time, Total_Time, Permissible_Load_MW, Gen_loss_kWh) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.site_id + "','" + unit.site + "','" + unit.date + "','" + unit.startTime + "','" + unit.endTime + "','" + unit.totalTime + "','" + unit.permLoad + "','" + unit.genShedding + "'),";
                delqry += " site_id = " + unit.site_id + " and Date = " + unit.date + " or";
            }
            qry += values;
            await Context.ExecuteNonQry<int>(delqry.Substring(0, (delqry.Length - 2)) + ";").ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        /*internal async Task<int> InsertDailyJMR(List<WindDailyJMR> set)
        {
            //pending : add activity log
            string qry = " insert into daily_jmr (date, site, site_id, jmr_kwh) values";
            string values = "";
            string delqry = "delete from daily_jmr where";
            foreach (var unit in set)
            {
                values += "('" + unit.date + "','" + unit.site + "','" + unit.site_id + "','" + unit.jmr_kwh + "'),";
                //where clause for deleting table data which matches with client data based on matching dates and site_ids
                delqry += " date = " + unit.date + " and site_id = " + unit.site_id + " or";
            }
            qry += values;
            await Context.ExecuteNonQry<int>(delqry.Substring(0, (delqry.Length - 2)) + ";").ConfigureAwait(false);
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }*/
        internal async Task<int> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> solarDailyTargetKPI)
        {
            string delqry = "";
            
            for (int i = 0; i < solarDailyTargetKPI.Count; i++)
            { 
                string dates = Convert.ToDateTime(solarDailyTargetKPI[i].Date).ToString("yyyy-MM-dd");
                

                delqry += "delete from daily_target_kpi_solar where fy='" + solarDailyTargetKPI[i].FY + "' and date='" + dates + "' and sites='" + solarDailyTargetKPI[i].Sites + "' ;";
                
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            int recordcount = 0;
            for (int i = 0; i < solarDailyTargetKPI.Count; i++)
            {
                recordcount++;
                string dates = Convert.ToDateTime(solarDailyTargetKPI[i].Date).ToString("yyyy-MM-dd");
                string ma = Convert.ToString(solarDailyTargetKPI[i].MA);
                string iga = Convert.ToString(solarDailyTargetKPI[i].IGA);
                string ega = Convert.ToString(solarDailyTargetKPI[i].EGA);
                string pr = Convert.ToString(solarDailyTargetKPI[i].PR);
                string plf = Convert.ToString(solarDailyTargetKPI[i].PLF);

                qry += "insert into daily_target_kpi_solar (fy,date,sites,ghi,poa,gen_nos,ma,iga,ega,pr,plf) values ('" + solarDailyTargetKPI[i].FY + "','" + dates + "','" + solarDailyTargetKPI[i].Sites + "','" + solarDailyTargetKPI[i].GHI + "','" + solarDailyTargetKPI[i].POA + "','" + solarDailyTargetKPI[i].kWh + "','" + ma.TrimEnd('%') + "','" + iga.TrimEnd('%') + "','" + ega.TrimEnd('%') + "','" + pr.TrimEnd('%') + "','" + plf.TrimEnd('%') + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> set)
        {

            string qry = " insert into monthly_target_kpi_solar (fy, month, sites, site_id, ghi, poa, gen_nos, ma, iga, ega, pr, plf) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.FY + "','" + unit.Month + "','" + unit.Sites + "','" + unit.Site_Id + "','" + unit.GHI + "','" + unit.POA + "','" + unit.kWh + "','" + unit.MA + "','" + unit.IGA + "','" + unit.EGA + "','" + unit.PR + "','" + unit.PLF + "'),";
            }
            qry += values;

            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> set)
        {
            string qry = " insert into monthly_line_loss_solar (fy, site, site_id, month, lineloss) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.FY + "','" + unit.Sites + "','" + unit.Site_Id + "','" + unit.Month + "','" + unit.LineLoss + "'),";
            }
            qry += values;
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarJMR(List<SolarMonthlyJMR> set)
        {
            string qry = " insert into monthly_jmr_solar (FY, Site, site_id, Plant_Section, Controller_KWH_INV, Scheduled_Units_kWh, Export_kWh, Import_kWh, Net_Export_kWh, Export_kVAh, Import_kVAh, Export_kVArh_lag, Import_kVArh_lag, Export_kVArh_lead, Import_kVArh_lead, JMR_date, JMR_Month, JMR_Year, LineLoss, Line_Loss_percentage, RKVH_percentage) values";
            string values = "";
            foreach (var unit in set)
            {
                values += "('" + unit.FY + "','" + unit.Site + "','" + unit.site_id + "','" + unit.Plant_Section + "','" + unit.Controller_KWH_INV + "','" + unit.Scheduled_Units_kWh + "','" + unit.Export_kWh + "','" + unit.Import_kWh + "','" + unit.Net_Export_kWh + "','" + unit.Export_kVAh + "','" + unit.Import_kVAh + "','" + unit.Export_kVArh_lag + "','" + unit.Import_kVArh_lag + "','" + unit.Export_kVArh_lead + "','" + unit.Import_kVArh_lead + "','" + unit.JMR_date + "','" + unit.JMR_Month + "','" + unit.JMR_Year + "','" + unit.LineLoss + "','" + unit.Line_Loss_percentage + "','" + unit.RKVH_percentage + "'),";
            }
            qry += values;

            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> set)
        {
            string qry = " insert into daily_load_shedding_solar (Site, Site_ID, Date, Start_Time, End_Time, Total_Time, Permissible_Load_MW, Gen_loss_kWh) values";
            string values = "";
            foreach (var unit in set)
            {
                values += "('" + unit.Site + "','" + unit.Site_Id + "','" + unit.Date + "','" + unit.Start_Time + "','" + unit.End_Time + "','" + unit.Total_Time + "','" + unit.Permissible_Load_MW + "','" + unit.Gen_loss_kWh + "'),";
            }
            qry += values;

            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> set)
        {
            string qry = " insert into solar_ac_dc_capacity (site, site_id, inverter, dc_capacity, ac_capacity) values";
            string values = "";
            foreach (var unit in set)
            {
                values += "('" + unit.site + "','" + unit.site_id + "','" + unit.inverter + "','" + unit.dc_capacity + "','" + unit.ac_capacity + "'),";
            }
            qry += values;

            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarDailyBDloss(List<SolarDailyBDloss> solarDailyBDloss)
        {

            string qry = "";
            int recordcount = 0;
            for (int i = 0; i < solarDailyBDloss.Count; i++)
            {
                recordcount++;
                string dates = Convert.ToDateTime(solarDailyBDloss[i].Date).ToString("yyyy-MM-dd");
                string Tar_Plant_PR = Convert.ToString(solarDailyBDloss[i].Tar_Plant_PR);
                

                qry += "insert into daily_bd_loss_solar (date,site,bd_type,ext_bd,igbd,icr,inv,smb,strings,stop_from,stop_to,total_stop,bd_ir,capacity,act_plant_pr,plant_gen_loss,remarks,action) values ('" + dates + "','" + solarDailyBDloss[i].Site + "','" + solarDailyBDloss[i].BD_Type + "','" + solarDailyBDloss[i].Ext_BD + "','" + solarDailyBDloss[i].IGBD + "','" + solarDailyBDloss[i].ICR + "','" + solarDailyBDloss[i].INV + "','" + solarDailyBDloss[i].SMB + "','" + solarDailyBDloss[i].Strings + "','" + solarDailyBDloss[i].From + "','" + solarDailyBDloss[i].To + "','" + solarDailyBDloss[i].Total_Stop + "','" + solarDailyBDloss[i].BD_IR_POA + "','" + solarDailyBDloss[i].Capacity_kwp + "','" + Tar_Plant_PR.TrimEnd('%') + "','" + solarDailyBDloss[i].Plant_Gen_Loss + "','" + solarDailyBDloss[i].Remarks + "','" + solarDailyBDloss[i].Action_Taken + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> importMetaData(ImportBatch meta, string userName, int userId)
        {
            string query = "";
            meta.importFilePath = meta.importFilePath.Replace("\\", "\\\\");
            int import_by = 2;
            string import_by_name = "Demo User";

            //query = "insert into import_log (file_name, import_type, log_filename) values ('" + meta.importFilePath + "','" + meta.importType + "','" + meta.importLogName + "');";
            query = "insert into import_batches (file_name, import_type, log_filename,site_id,import_date,imported_by,import_by_name) values ('" + meta.importFilePath + "','" + meta.importType + "','" + meta.importLogName + "','" + meta.importSiteId + "',NOW(),'" + userId + "','" + userName + "');";
            return await Context.ExecuteNonQry<int>(query).ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> set, int batchId)
        {
            string delqry = "delete from uploading_pyranometer_1_min_solar where date_time = '" + set[0].date_time + "' and site_id='"+set[0].site_id+"';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);
            string qry = " insert into uploading_pyranometer_1_min_solar(site_id, date_time, ghi_1, ghi_2, poa_1, poa_2, poa_3, poa_4, poa_5, poa_6, poa_7, avg_ghi, avg_poa, amb_temp, mod_temp, import_batch_id) values";
            string values = "";
            foreach (var unit in set)
            {
                values += "('" + unit.site_id + "','" + unit.date_time + "','" + unit.ghi_1 + "','" + unit.ghi_2 + "','" + unit.poa_1 + "','" + unit.poa_2 + "','" + unit.poa_3 + "','" + unit.poa_4 + "','" + unit.poa_5 + "','" + unit.poa_6 + "','" + unit.poa_7 + "','" + unit.avg_ghi + "','" + unit.avg_poa + "','" + unit.amb_temp + "','" + unit.mod_temp + "','" + batchId + "'),";
            }
            qry += values;
            return await Context.ExecuteNonQry<int>(delqry + qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> set, int batchId)
        {
            string delqry = "delete from uploading_pyranometer_15_min_solar where date_time = '" + set[0].date_time + "' and site_id='" + set[0].site_id + "';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);
            string qry = " insert into uploading_pyranometer_15_min_solar(site_id, date_time, ghi_1, ghi_2, poa_1, poa_2, poa_3, poa_4, poa_5, poa_6, poa_7, avg_ghi, avg_poa, amb_temp, mod_temp, import_batch_id) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.site_id + "','" + unit.date_time + "','" + unit.ghi_1 + "','" + unit.ghi_2 + "','" + unit.poa_1 + "','" + unit.poa_2 + "','" + unit.poa_3 + "','" + unit.poa_4 + "','" + unit.poa_5 + "','" + unit.poa_6 + "','" + unit.poa_7 + "','" + unit.avg_ghi + "','" + unit.avg_poa + "','" + unit.amb_temp + "','" + unit.mod_temp + "','" + batchId + "'),";
            }
            qry += values;
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarUploadingFileGeneration(List<SolarUploadingFileGeneration> set, int batchId)
        {
            string delqry = "delete from uploading_file_generation_solar  where date = '" + set[0].date + "' and site_id='" + set[0].site_id + "';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = " insert into uploading_file_generation_solar (date, site, site_id, inverter, inv_act, plant_act, pi, import_batch_id) values";
            string values = "";
            foreach (var unit in set)
            {
                values += "('" + unit.date + "','" + unit.site + "','" + unit.site_id + "','" + unit.inverter + "','" + unit.inv_act + "','" + unit.plant_act + "','" + unit.pi + "','" + batchId + "'),";
            }
            qry += values;
            
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<int> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> set, int batchId)
        {//Updated
            string delqry = "delete from uploading_file_breakdown_solar where date = '" + set[0].date + "' and site_id='" + set[0].site_id + "';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);
            string qry = " insert into uploading_file_breakdown_solar (date, site, site_id, ext_int_bd, igbd, icr, inv, smb, strings, from_bd, to_bd, total_stop, bd_remarks, bd_type, bd_type_id, action_taken, import_batch_id) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.date + "','" + unit.site + "','" + unit.site_id + "','" + unit.ext_int_bd + "','" + unit.igbd + "','" + unit.icr + "','" + unit.inv + "','" + unit.smb + "','" + unit.strings + "','" + unit.from_bd + "','" + unit.to_bd + "','" + unit.total_bd + "','" + unit.bd_remarks + "','" + unit.bd_type + "','" + unit.bd_type_id + "','" + unit.action_taken + "','" + batchId + "'),";
            }
            qry += values;
            
            return await Context.ExecuteNonQry<int>(delqry + qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);

            //bool response = false;
            //string qry = "";

            //foreach (var solarUploadingFileBreakDown in listSolarUploadingFileBreakDown)
            //{
            //    qry += "insert into uploading_file_generation_solar (date,site,ext_int_bd,icr,inv,smb,strings,from_bd,to_bd,bd_remarks,bd_type,action_taken) values ('" + solarUploadingFileBreakDown.date + "','" + solarUploadingFileBreakDown.site + "','" + solarUploadingFileBreakDown.ext_int_bd + "','" + solarUploadingFileBreakDown.icr + "','" + solarUploadingFileBreakDown.inv + "','" + solarUploadingFileBreakDown.smb + "','" + solarUploadingFileBreakDown.strings + "','" + solarUploadingFileBreakDown.from_bd + "','" + solarUploadingFileBreakDown.to_bd + "','" + solarUploadingFileBreakDown.bd_remarks + "','" + solarUploadingFileBreakDown.bd_type + "','" + solarUploadingFileBreakDown.action_taken + "');";

            //    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
            //    response = true;
            //}
            //return response;

        }

        internal async Task<int> InsertWindUploadingFileGeneration(List<WindUploadingFileGeneration> set, int batchId)
        {
            string delqry = "delete from uploading_file_generation where date = '" + set[0].date + "' and site_id='" + set[0].site_id + "';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);
            string qry = " insert into uploading_file_generation (site_name, site_id, date, wtg, wtg_id, wind_speed, grid_hrs, operating_hrs, lull_hrs, kwh, import_batch_id) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.site_name + "','" + unit.site_id + "','" + unit.date + "','" + unit.wtg + "','" + unit.wtg_id + "','" + unit.wind_speed + "','" + unit.grid_hrs + "','" + unit.operating_hrs + "','" + unit.lull_hrs + "','" + unit.kwh + "','" + batchId + "'),";
            }
            qry += values;
            
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }
        internal async Task<BatchIdImport> GetBatchId(string logFileName)
        {

            try
            {
                string qry = "select import_batch_id from import_batches where log_filename ='" + logFileName + "'";
                DataTable dt = await Context.FetchData(qry).ConfigureAwait(false);
                BatchIdImport obj = new BatchIdImport();
                obj.import_batch_id = (int)dt.Rows[0]["import_batch_id"];
                return obj;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        internal async Task<int> InsertWindUploadingFileBreakDown(List<WindUploadingFileBreakDown> set, int batchId)
        {
            string delqry = "delete from uploading_file_breakdown where date = '" + set[0].date + "' and site_id='" + set[0].site_id + "';";
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);
            string qry = " insert into uploading_file_breakdown(date, site_name, site_id, wtg, wtg_id, bd_type, bd_type_id, stop_from, stop_to, total_stop, error_description, action_taken, import_batch_id) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.date + "','" + unit.site_name + "','" + unit.site_id + "','" + unit.wtg + "','" + unit.wtg_id + "','" + unit.bd_type + "','" + unit.bd_type_id + "','" + unit.stop_from + "','" + unit.stop_to + "','" + unit.total_stop + "','" + unit.error_description + "', '" + unit.action_taken + "', '" + batchId + "'),";
            }
            qry += values;

            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }

        internal async Task<List<SolarDailyGenReports>> GetSolarInverterFromdailyGenSummary(string state, string site)
        {
            string filter = "";
            int chkfilter = 0;

            if (!string.IsNullOrEmpty(state) && state!="All" && state != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                chkfilter = 1;

                string[] spstate = state.Split("~");
                filter += "state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site !="All" && site != "All~")
            {
                if (chkfilter == 1) { filter += " and "; }
                chkfilter = 1;

                string[] spsite = site.Split("~");
                filter += "site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if(!string.IsNullOrEmpty(filter))
            {
                filter = " where " + filter;
            }
            string qry = @"select distinct location_name as Inverter from daily_gen_summary_solar " + filter;
            return await Context.GetData<SolarDailyGenReports>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports1>> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,
t2.spv,t1.site,location_name as Inverter, sum(t3.dc_capacity)as dc_capacity,
sum(t3.ac_capacity)as ac_capacity,
(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*))as inv_pr,(sum(plant_pr)/count(*)) as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site  left join solar_ac_dc_capacity t3 on  t3.site=t1.site 
where   t2.state=t1.state  and t3.inverter=t1.location_name  " + filter + " group by t1.site,date,location_name ";

            //where t1.approve_status=" + approve_status + " and t2.state=t1.state  and t3.inverter=t1.location_name  " + filter + " group by t1.site,date,location_name ";

            return await Context.GetData<SolarDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports2>> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }


            string qry = @"SELECT year(date)as year,month(date)as month,date,
t2.country,t1.state,t2.spv,t1.site,(t2.dc_capacity)as dc_capacity,
(t2.ac_capacity)as ac_capacity,(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*))as inv_pr,(sum(plant_pr)/count(*))as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,'' as tracker_losses,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site
where   t2.state=t1.state  " + filter + " group by date,t1.site ";

            //where t1.approve_status=" + approve_status + " and  t2.state=t1.state  " + filter + " group by date,t1.site ";

            return await Context.GetData<SolarDailyGenReports2>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports1>> GetSolarMonthlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,
t2.spv,t1.site,location_name as Inverter, (t3.dc_capacity)as dc_capacity,
(t3.ac_capacity)as ac_capacity,
(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*)) as inv_pr,(sum(plant_pr)/count(*)) as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site   left join solar_ac_dc_capacity t3 on  t3.site=t1.site 
where   t2.state=t1.state  and t3.inverter=t1.location_name " + filter + " group by t1.site,location_name ,month(date)";

            //where t1.approve_status=" + approve_status + " and  t2.state=t1.state  and t3.inverter=t1.location_name " + filter + " group by t1.site,location_name ,month(date)";

            return await Context.GetData<SolarDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports2>> GetSolarMonthlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }


            string qry = @"SELECT year(date)as year,month(date)as month,date,
t2.country,t1.state,t2.spv,t1.site,(t2.dc_capacity)as dc_capacity,
(t2.ac_capacity)as ac_capacity,(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*)) as inv_pr,(sum(plant_pr)/count(*)) as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,'' as tracker_losses,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site
where   t2.state=t1.state  " + filter + " group by t1.site ,month(date)";

            //where t1.approve_status=" + approve_status + " and t2.state=t1.state  " + filter + " group by t1.site ,month(date)";

            return await Context.GetData<SolarDailyGenReports2>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports1>> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,
t2.spv,t1.site,location_name as Inverter, (t3.dc_capacity)as dc_capacity,
(t3.ac_capacity)as ac_capacity,
(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*)) as inv_pr,(sum(plant_pr)/count(*)) as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site  left join solar_ac_dc_capacity t3 on  t3.site=t1.site 
where   t2.state=t1.state  and t3.inverter=t1.location_name " + filter + " group by t1.site,location_name ";

            //where t1.approve_status=" + approve_status + " and t2.state=t1.state  and t3.inverter=t1.location_name " + filter + " group by t1.site,location_name ";

            return await Context.GetData<SolarDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyGenReports2>> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            string filter = "";

            if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
            {
                filter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "')";

            }
            if (!string.IsNullOrEmpty(country) && country != "All~")
            {

                string[] spcountry = country.Split("~");
                filter += " and t2.country in (";
                string countrys = "";
                for (int i = 0; i < spcountry.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spcountry[i].ToString()))
                    {
                        countrys += "'" + spcountry[i].ToString() + "',";
                    }
                }
                filter += countrys.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(state) && state != "All~")
            {

                string[] spstate = state.Split("~");
                filter += " and t1.state in (";
                string states = "";
                for (int i = 0; i < spstate.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spstate[i].ToString()))
                    {
                        states += "'" + spstate[i].ToString() + "',";
                    }
                }
                filter += states.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All~")
            {

                string[] spspv = spv.Split("~");
                filter += " and t2.spv in (";
                string spvs = "";
                for (int i = 0; i < spspv.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spspv[i].ToString()))
                    {
                        spvs += "'" + spspv[i].ToString() + "',";
                    }
                }
                filter += spvs.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(site) && site != "All~")
            {

                string[] spsite = site.Split("~");
                filter += " and t1.site in (";
                string sites = "";
                for (int i = 0; i < spsite.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spsite[i].ToString()))
                    {
                        sites += "'" + spsite[i].ToString() + "',";
                    }
                }
                filter += sites.TrimEnd(',') + ")";

            }
            if (!string.IsNullOrEmpty(inverter) && inverter != "All~")
            {
                inverter = inverter.Replace('=', '/');
                string[] spinverter = inverter.Split("~");
                filter += " and t1.location_name in (";
                string inverters = "";
                for (int i = 0; i < spinverter.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spinverter[i].ToString()))
                    {
                        inverters += "'" + spinverter[i].ToString() + "',";
                    }
                }
                filter += inverters.TrimEnd(',') + ")";
            }
            if (!string.IsNullOrEmpty(month) && month != "All~")
            {

                string[] spmonth = month.Split("~");
                filter += " and month(date) in (";
                string months = "";
                for (int i = 0; i < spmonth.Length; i++)
                {
                    if (!string.IsNullOrEmpty(spmonth[i].ToString()))
                    {
                        months += "" + spmonth[i].ToString() + ",";
                    }
                }
                filter += months.TrimEnd(',') + ")";
            }


            string qry = @"SELECT year(date)as year,month(date)as month,date,
t2.country,t1.state,t2.spv,t1.site,(t2.dc_capacity)as dc_capacity,
(t2.ac_capacity)as ac_capacity,(sum(ghi)/count(*))as ghi,(sum(poa)/count(*))as poa,sum(expected_kwh)as expected_kwh,
sum(inv_kwh)as inv_kwh,sum(plant_kwh)as plant_kwh,(sum(inv_pr)/count(*)) as inv_pr,(sum(plant_pr)/count(*)) as plant_pr,
inv_plf_ac as inv_plf,plant_plf_ac as plant_plf,
ma as ma_actual,ma as ma_contractual,
(sum(iga)/count(*))as iga,(sum(ega)/count(*))as ega,sum(prod_hrs) as gen_hrs,sum(usmh)as usmh,sum(smh)as smh,
sum(oh)as oh,sum(igbdh)as igbdh,sum(egbdh)as egbdh,
sum(load_shedding)as load_shedding,'' as tracker_losses,sum(total_losses)as total_losses
 FROM daily_gen_summary_solar t1 left join site_master_solar t2 on  t2.site=t1.site
where  t2.state=t1.state  " + filter + " group by t1.site ";

            //where t1.approve_status=" + approve_status + " and t2.state=t1.state  " + filter + " group by t1.site ";

            return await Context.GetData<SolarDailyGenReports2>(qry).ConfigureAwait(false);

        }


        internal async Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate)
        {

            string datefilter = " (date >= '" + fromDate + "'  and date<= '" + todate + "') ";

            string qry = @"SELECT site,
(SELECT ac_capacity FROM site_master_solar where site=t1.site and state=t1.state)as capacity,
(SELECT dc_capacity FROM site_master_solar where site=t1.site and state=t1.state)as dc_capacity,(SELECT total_tarrif FROM site_master_solar where site=t1.site and state=t1.state)as total_tarrif,
(SELECT  sum(gen_nos) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy='" + fy + "') as tar_kwh,(sum(expected_kwh)/1000000)as expected_kwh,(sum(inv_kwh)/1000000)as act_kwh,(SELECT lineloss FROM monthly_line_loss_solar where site=t1.site and fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b')  order by monthly_line_loss_solar_id desc limit 1)as lineloss,(SELECT  sum(ghi)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_ghi,sum(ghi)/count(*) as act_ghi,(SELECT  sum(poa)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_poa,sum(poa)/count(*) as act_poa,(SELECT  sum(plf)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_plf,sum(plant_plf_ac)/count(*) as act_plf,(SELECT  sum(pr)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_pr,sum(plant_pr)/count(*) as act_pr,(SELECT  sum(ma)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_ma,sum(ma)/count(*) as act_ma,(SELECT  sum(iga)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_iga,sum(iga)/count(*) as act_iga,(SELECT  sum(ega)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + "  and fy= '" + fy + "') as tar_ega,sum(ega)/count(*) as act_ega FROM daily_gen_summary_solar t1 where  " + datefilter + " group by site";

            //and fy= '" + fy + "') as tar_ega,sum(ega)/count(*) as act_ega FROM daily_gen_summary_solar t1 where t1.approve_status=" + approve_status + " and " + datefilter + " group by site";

            return await Context.GetData<SolarPerformanceReports>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarPerformanceReports>> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {

            string datefilter = " (date >= '" + fromDate + "'  and date<= '" + todate + "') ";

            string qry = @"SELECT t1.site,spv,
(SELECT ac_capacity FROM site_master_solar where site=t1.site and state=t1.state)as capacity,
(SELECT dc_capacity FROM site_master_solar where site=t1.site and state=t1.state)as dc_capacity,(SELECT total_tarrif FROM site_master_solar where site=t1.site and state=t1.state)as total_tarrif,
(SELECT  sum(gen_nos) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy='" + fy + "') as tar_kwh,(sum(inv_kwh)/1000000)as act_kwh,(SELECT lineloss FROM monthly_line_loss_solar where site=t1.site and fy='" + fy + "' and month=DATE_FORMAT(t1.date, '%b') order by monthly_line_loss_solar_id desc limit 1)as lineloss,(SELECT  sum(ghi)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_ghi,sum(ghi)/count(*) as act_ghi,(SELECT  sum(poa)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_poa,sum(poa)/count(*) as act_poa,(SELECT  sum(plf)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_plf,sum(plant_plf_ac)/count(*) as act_plf,(SELECT  sum(pr)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_pr,sum(plant_pr)/count(*) as act_pr,(SELECT  sum(ma)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_ma,sum(ma)/count(*) as act_ma,(SELECT  sum(iga)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + " and fy= '" + fy + "') as tar_iga,sum(iga)/count(*) as act_iga,(SELECT  sum(ega)/count(*) FROM daily_target_kpi_solar where sites=t1.site and " + datefilter + "  and fy= '" + fy + "') as tar_ega,sum(ega)/count(*) as act_ega FROM daily_gen_summary_solar t1 left join site_master_solar t2 on t1.site =t2.site  where   " + datefilter + " group by spv";

            //count(*) as act_ega FROM daily_gen_summary_solar t1 left join site_master_solar t2 on t1.site =t2.site  where t1.approve_status=" + approve_status + " and " + datefilter + " group by spv";

            return await Context.GetData<SolarPerformanceReports>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyBDloss>> GetSolarDailyBDLossData(string fromDate, string todate)
        {

            string datefilter = " (date >= '" + fromDate + "'  and date<= '" + todate + "') ";

            string qry = @"SELECT   date as Date,site as Site,bd_type as BD_Type,
    ext_bd as Ext_BD,igbd as IGBD,icr as ICR,inv as INV,
    smb as SMB,strings as Strings,stop_from as 'From',stop_to as 'To',
    total_stop as Total_Stop,bd_ir as BD_IR_POA,capacity as Capacity_kwp,
    act_plant_pr as Tar_Plant_PR,plant_gen_loss as Plant_Gen_Loss,
    remarks as Remarks,action as Action_Taken 
FROM daily_bd_loss_solar where   " + datefilter;

            //FROM daily_bd_loss_solar where approve_status="+approve_status+" and " + datefilter;


            return await Context.GetData<SolarDailyBDloss>(qry).ConfigureAwait(false);

        }



      

        ////#region views


        internal async Task<List<DailyGenSummary>> GetWindDailyGenSummary(string site, string fromDate, string ToDate)
        {
            if (String.IsNullOrEmpty(site)) return new List<DailyGenSummary>();
            string filter = " where site_id in (" + site + ") and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            //string filter = " where approve_status=" + approve_status + " and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            return await Context.GetData<DailyGenSummary>("Select * from daily_gen_summary " + filter).ConfigureAwait(false);
        }

        internal async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary1(string site, string fromDate, string ToDate)
        {
            if (String.IsNullOrEmpty(site)) return new List<SolarDailyGenSummary>();
            string filter = " where site_id in (" + site + ") and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            //string filter = " where approve_status=" + approve_status + " and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            return await Context.GetData<SolarDailyGenSummary>("Select * from daily_gen_summary_solar " + filter).ConfigureAwait(false);
        }
        internal async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary(string fromDate, string ToDate)
        {

            string filter = " where   date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            //  string filter = " where approve_status=" + approve_status + " and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            return await Context.GetData<SolarDailyGenSummary>("Select * from daily_gen_summary_solar " + filter).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string site, string fromDate, string todate)
        {
            if (String.IsNullOrEmpty(site)) return new List<WindDailyTargetKPI>();
            string filter = " where site_id in (" + site + ") and (date >= '" + fromDate + "'  and date<= '" + todate + "') ";
            string qry = @"SELECT fy,date,site,wind_speed as WindSpeed,kwh,ma,iga,ega,plf FROM daily_target_kpi" + filter;
            return await Context.GetData<WindDailyTargetKPI>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate, string site)
        {
            string filter = " site_id in (" + site + ") and (date >= '" + fromDate + "'  and date<= '" + todate + "') ";
            string qry = @"SELECT fy, date, sites, ghi, poa, gen_nos as kWh,ma,iga,ega,pr,plf FROM daily_target_kpi_solar where " + filter;
            return await Context.GetData<SolarDailyTargetKPI>(qry).ConfigureAwait(false);

        }
        internal async Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string site, string fy, string month)
        {
            if (String.IsNullOrEmpty(site)) return new List<WindMonthlyTargetKPI>();
            string filter = " where site_id in (" + site + ") and fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and month in(" + monthnames + ")";
                }

            }

            string qry = @"SELECT fy,month,site,wind_speed as WindSpeed,kwh,ma,iga,ega,plf FROM monthly_target_kpi" + filter;

            return await Context.GetData<WindMonthlyTargetKPI>(qry).ConfigureAwait(false);

        }
		internal async Task<int> InsertWindSiteMaster(List<WindSiteMaster> set)
        {
            //pending : add activity log
            //prepared update query because existing queries cannot be deleted and orphan existing site master ids
            //grabs db site_master table data into local object list
            string fetchQry = "select * from site_master";
            List<WindSiteMaster> tableData = await Context.GetData<WindSiteMaster>(fetchQry).ConfigureAwait(false);
            int val = 0;
            //stores an existing record from the database which matches with a record in the client dataset
            WindSiteMaster existingRecord = new WindSiteMaster();
            string updateQry = "";
            string qry = "insert into site_master(country, site, spv, state, model, capacity_mw, wtg, total_mw, tarrif, total_tarrif, gbi, ll_compensation) values";
            string insertValues = "";
            foreach (var unit in set)
            {
                //checks if db table contains site record that matches a record in client dataset
                existingRecord = tableData.Find(tableRecord => tableRecord.site.Equals(unit.site));
                if (existingRecord==null)
                {
                    insertValues += "('" + unit.country + "','" + unit.site + "','" + unit.spv + "','" + unit.state + "','" + unit.model + "','" + unit.capacity_mw + "','" + unit.wtg + "','" + unit.total_mw + "','" + unit.tarrif + "','" + unit.total_tarrif + "','" + unit.gbi + "','" + unit.ll_compensation + "'),";
                }
                else
                {
                    //if match is found
                    updateQry += "update site_master set capacity_mw = " + unit.capacity_mw + ", wtg = " + unit.wtg + ", total_mw = " + unit.total_mw + ", tarrif = " + unit.tarrif + " , total_tarrif = " + unit.total_tarrif + " , gbi = " + unit.gbi + ", ll_compensation = " + unit.ll_compensation + " where site_master_id = " + existingRecord.site_master_id + ";";
                }
            }
            qry += insertValues;
            if (!(string.IsNullOrEmpty(insertValues)))
            {
                val = await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            if (!(string.IsNullOrEmpty(updateQry)))
            {
                val = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
            }
            else
            {
                val = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
                val = await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            return val;
        }
        internal async Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month, string site)
        {
            string filter = " fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and month in(" + monthnames + ") and site_id in ("+site+");";
                }

            }
            string qry = @"SELECT  fy, month, sites, ghi, poa, gen_nos as kWh, ma, iga, ega, pr, plf FROM monthly_target_kpi_solar where " + filter;
            return await Context.GetData<SolarMonthlyTargetKPI>(qry).ConfigureAwait(false);
        }
        internal async Task<int> InsertWindLocationMaster(List<WindLocationMaster> set)
        {
            //pending : add activity log
            //added logic where if site and wtg exists then update existing records
            //grabs db location_master table data into local object list
            string fetchQry = "select * from location_master";
            List<WindLocationMaster> tableData = await Context.GetData<WindLocationMaster>(fetchQry).ConfigureAwait(false);
            int val = 0;
            //stores an existing record from the database which matches with a record in the client dataset
            WindLocationMaster existingRecord = new WindLocationMaster();
            string updateQry = "";
            // string qry = "insert into location_master(location_master_id, site_master_id, site, wtg, feeder, max_kwh_day) values";
            string qry = "insert into location_master(site_master_id, site, wtg, feeder, max_kwh_day) values";
            string insertValues = "";
            foreach (var unit in set)
            {
                //checks if db table contains site record that matches a record in client dataset
                existingRecord = tableData.Find(tableRecord => tableRecord.site.Equals(unit.site) && tableRecord.wtg.Equals(unit.wtg));
                try
                {
                    updateQry += "update location_master set feeder = " + unit.feeder + " , max_kwh_day =  " + unit.max_kwh_day + "  where site_master_id = " + existingRecord.site_master_id + ";";
                    val = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
                    /*if (string.IsNullOrEmpty(existingRecord.site))
                    {
                        insertValues += "('" + unit.site_master_id + "','" + unit.site + "','" + unit.wtg + "','" + unit.feeder + "','" + unit.max_kwh_day + "'),";
                    }*/
                }
                catch(Exception)
                {
                    //if match is found
                    insertValues += "('" + unit.site_master_id + "','" + unit.site + "','" + unit.wtg + "','" + unit.feeder + "','" + unit.max_kwh_day + "'),";

                }
            }
            qry += insertValues;
            if (!(string.IsNullOrEmpty(insertValues)))
            {
                val = await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            }
            /*if (!(string.IsNullOrEmpty(updateQry)))
            {
                val = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
            }*/
            //else
           // {
               // val = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
              //  val = await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
            //}
            return val;
        }
        internal async Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string site, string fy, string month)
        {

            string filter = " site_id in ("+ site +") and fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and month in(" + monthnames + ")";
                }

            }
            string qry = @"SELECT  fy,month,site,line_loss as LineLoss FROM monthly_uploading_line_losses where " + filter;
            return await Context.GetData<WindMonthlyUploadingLineLosses>(qry).ConfigureAwait(false);
        }

        internal async Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month, string site)
        {
            string filter = "site_id in ("+site+") and fy='" + fy + "' ";
            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and month in(" + monthnames + ")";
                }
            }
            string qry = @"SELECT  fy, month, site as Sites, LineLoss FROM monthly_line_loss_solar where " + filter;
            return await Context.GetData<SolarMonthlyUploadingLineLosses>(qry).ConfigureAwait(false);
        }

        internal async Task<List<WindMonthlyJMR1>> GetWindMonthlyJMR(string site, string fy, string month)
        {

            if (String.IsNullOrEmpty(site)) return new List<WindMonthlyJMR1>();
            string filter = " site_id in ( "+site+" ) and fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and jmr_month in(" + monthnames + ")";
                }

            }


            string qry = @"SELECT  fy,site,Plant_Section,Controller_KWH_INV,Scheduled_Units_kWh,Export_kWh,Import_kWh,Net_Export_kWh,Export_kVAh,Import_kVAh,Export_kVArh_lag,Import_kVArh_lag,Export_kVArh_lead,Import_kVArh_lead,JMR_date,JMR_Month,JMR_Year,LineLoss,Line_Loss_percentage,RKVH_percentage FROM monthly_jmr where " + filter;

            return await Context.GetData<WindMonthlyJMR1>(qry).ConfigureAwait(false);
        }
        internal async Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month, string site)
        {

            
            string filter = " site_id in (" + site + ") and fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and jmr_month in(" + monthnames + ")";
                }

            }

            string qry = @"SELECT  fy, site, Plant_Section, Controller_KWH_INV, Scheduled_Units_kWh, Export_kWh, Import_kWh, Net_Export_kWh, Export_kVAh, Import_kVAh, Export_kVArh_lag, Import_kVArh_lag, Export_kVArh_lead, Import_kVArh_lead, JMR_date, JMR_Month, JMR_Year, LineLoss, Line_Loss_percentage, RKVH_percentage FROM monthly_jmr_solar where " + filter;

            return await Context.GetData<SolarMonthlyJMR>(qry).ConfigureAwait(false);
        }
        internal async Task<List<SolarInvAcDcCapacity>> GetSolarACDCCapacity(string site)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(site) && site != "All" && site != "All~")
            {
                
                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " where site_id in(" + site + ")";
                }

            }

            //string datefilter = " where site='" + site + "' ";
            //if(site=="All")
            //  { datefilter = ""; }
            string qry = @"SELECT  site,inverter,dc_capacity,ac_capacity FROM solar_ac_dc_capacity " + filter;

            return await Context.GetData<SolarInvAcDcCapacity>(qry).ConfigureAwait(false);
        }
        internal async Task<List<UserManagement>> GetUserManagement(string userMail,string date)
        {

            string datefilter = " user_mail_id='" + userMail + "' and date='"+date+"'";

            string qry = @"SELECT user_mail_id,date,dgr,status  FROM user_management where " + datefilter;

            return await Context.GetData<UserManagement>(qry).ConfigureAwait(false);
        }
        internal async Task<List<BDType>> GetBDType()
        {
            string qry = @"SELECT * FROM bd_type ";
            return await Context.GetData<BDType>(qry).ConfigureAwait(false);
        }
        internal async Task<List<WindViewDailyLoadShedding>> GetWindDailyloadShedding(string site,string fromDate,string toDate)
        {
            if(site=="") return new List<WindViewDailyLoadShedding>();
            string datefilter = " where site_id in (" + site + ") and (date >= '" + fromDate + "'  and date<= '" + toDate + "') ";
            string qry = @"SELECT * FROM daily_load_shedding " + datefilter;

            return await Context.GetData<WindViewDailyLoadShedding>(qry).ConfigureAwait(false);
        }
        internal async Task<List<SolarDailyLoadShedding>> GetSolarDailyloadShedding(string site, string fromDate, string toDate)
        {

            string datefilter = " where site_id in ("+site+")";
            if (site == "All")
            {
                datefilter = " where date >= '" + fromDate + "'  and date<= '" + toDate + "' ";
            }
            else
            {
                datefilter += " and date >= '" + fromDate + "' and date<= '" + toDate + "' ";
            }
            string qry = @"SELECT * FROM daily_load_shedding_solar" + datefilter;

            return await Context.GetData<SolarDailyLoadShedding>(qry).ConfigureAwait(false);
        }
		internal async Task<int> InsertSolarLocationMaster(List<SolarLocationMaster> set)
        {
            string qry = " insert into location_master_solar(country, site, site_id, eg, ig, icr_inv, icr, inv, smb, string, string_configuration, total_string_current, total_string_voltage, modules_quantity, wp, capacity, module_make, module_model_no, module_type, string_inv_central_inv) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.country + "','" + unit.site + "','" + unit.site_id + "','" + unit.eg + "','" + unit.ig + "','" + unit.icr_inv + "','" + unit.icr + "','" + unit.inv + "','" + unit.smb + "','" + unit.strings + "','" + unit.string_configuration + "','" + unit.total_string_current + "','" + unit.total_string_voltage + "','" + unit.modules_quantity + "','" + unit.wp + "','" + unit.capacity + "','" + unit.module_make + "','" + unit.module_model_no + "','" + unit.module_type + "','" + unit.string_inv_central_inv + "'),";
            }
            qry += values;
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }
        internal async Task<List<DailyGenSummary>> GetWindDailyGenSummaryPending(string date, string site)
        {
            string filter = " where approve_status!=1 ";
            if (!string.IsNullOrEmpty(date) && date!="All")
            {
                filter += " and date='" + date + "'";
            }
            if (!string.IsNullOrEmpty(site) && site != "All")
            {
               // filter += " and site='" + site + "'";

                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }

            }
             
            //            string filter = " where date >= '" + fromDate + "' and date <= '" + ToDate + "' ";

            string qry = "Select * from daily_gen_summary " + filter;


            return await Context.GetData<DailyGenSummary>(qry).ConfigureAwait(false);

        }
		internal async Task<int> InsertSolarSiteMaster(List<SolarSiteMaster> set)
        {
            string qry = " insert into site_master_solar(country, site, spv, state, dc_capacity, ac_capacity, total_tarrif) values";
            string values = "";

            foreach (var unit in set)
            {
                values += "('" + unit.country + "','" + unit.site + "','" + unit.spv + "','" + unit.state + "','" + unit.dc_capacity + "','" + unit.ac_capacity + "','" + unit.total_tarrif + "'),";
            }
            qry += values;
            return await Context.ExecuteNonQry<int>(qry.Substring(0, (qry.Length - 1)) + ";").ConfigureAwait(false);
        }
        internal async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummaryPending(string date, string site)
        {

            //string filter = " where date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            string filter = " where approve_status!=1";
            if (!string.IsNullOrEmpty(date) && date != "All")
            {
                filter += " and date='" + date + "'";
            }
            if (!string.IsNullOrEmpty(site) && site != "All")
            {
                // filter += " and site='" + site + "'";

                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }

            }

            return await Context.GetData<SolarDailyGenSummary>("Select * from daily_gen_summary_solar " + filter).ConfigureAwait(false);

        }
        internal async Task<List<WindDailyBreakdownReport>> GetWindDailyBreakdownPending(string date, string site)
        {
            string filter = " where approve_status!=1 ";
            if (!string.IsNullOrEmpty(date) && date != "All")
            {
                filter += " and t1.date='" + date + "'";
            }
            if (!string.IsNullOrEmpty(site) && site != "All")
            {
                // filter += " and site='" + site + "'";

                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and t2.site in(" + sitesnames + ")";
                }

            }
            string qry = @"SELECT t1.*,t2.site FROM uploading_file_breakdown t1 left join location_master t2 on t1.wtg=t2.wtg " + filter;
            
            return await Context.GetData<WindDailyBreakdownReport>(qry).ConfigureAwait(false);

        }

        internal async Task<List<SolarFileBreakdown>> GetSolarDailyBreakdownPending(string date, string site)
        {


            string filter = " where approve_status!=1";

            if (!string.IsNullOrEmpty(date) && date != "All")
            {
                filter += " and date='" + date + "'";
            }
            if (!string.IsNullOrEmpty(site) && site != "All")
            {
                // filter += " and site='" + site + "'";

                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }

            }

            string qry = @"SELECT *  FROM daily_bd_loss_solar "+filter;
            
            return await Context.GetData<SolarFileBreakdown>(qry).ConfigureAwait(false);

        }

       
        internal async Task<int> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            string temptable = "tempwindData";
            string qry = "";
            string ids = "";
            if (approve_status == 1)
            {


                for (int i = 0; i < dailyGenSummary.Count; i++)
                {
                    ids += dailyGenSummary[i].daily_gen_summary_id + ",";
                }
                ids = ids.TrimEnd(',');

                //       string dataQry = @"select  t1.Date,month(t1.date) as month,year(t1.date) as year, t1.Site, t1.wind_speed as Wind,
                //t1.kwh as KWH,     replace(t2.line_loss, '%', '') as line_loss,     (t3.kwh * 1000000) as tarkwh     from daily_gen_summary t1 left
                //join monthly_uploading_line_losses t2 on t2.site = t1.site      and t2.month = DATE_FORMAT(t1.date, '%b') and fy = '2022-23'
                //left join daily_target_kpi t3 on t3.site = t1.site and t3.date = t1.date where      t1.approve_status = " + approve_status + "  and t1.daily_gen_summary_id in(" + ids + ")   group by t1.Site,t1.date order by t1.date desc; ";

                string dataQry = @"select  t1.Date,month(t1.date)as month,year(t1.date)as year, t1.Site,
    (t1.wind_speed)as Wind,    (t1.kwh)as KWH, 
    (select replace(line_loss,'%','') from monthly_uploading_line_losses where site=t1.site and 
    month=DATE_FORMAT(t1.date, '%b')   and fy='2022-23' order by monthly_uploading_line_losses_id desc limit 1)as line_loss,
    
    (t1.kwh)-((t1.kwh)* (select replace(line_loss,'%','') from monthly_uploading_line_losses where site=t1.site and 
    month=DATE_FORMAT(t1.date, '%b')   and fy='2022-23'  order by monthly_uploading_line_losses_id desc limit 1) /100) as jmrkwh,
    
    (select kwh*1000000 from daily_target_kpi where site=t1.site and date=t1.date order by daily_gen_summary_id desc limit 1)as tarkwh, 
    (select wind_speed from daily_target_kpi where site=t1.site and date=t1.date order by daily_gen_summary_id desc limit 1)
    as tarwind from  daily_gen_summary t1 where t1.daily_gen_summary_id in(" + ids + ") ";

                string qrynew = @"drop TEMPORARY table IF EXISTS " + temptable + " ;  CREATE TEMPORARY TABLE " + temptable + " " + dataQry + " ;insert into tblwinddata (Date,Month,year,Site,Wind,KWH,line_loss,jmrkwh,tarkwh,tarwind) select Date,Month,year,Site,Wind,KWH,line_loss,jmrkwh,tarkwh,tarwind from " + temptable + ";";

                await Context.ExecuteNonQry<int>(qrynew).ConfigureAwait(false);

            }
            for (int i = 0; i < dailyGenSummary.Count; i++)
            { 

                qry += "update daily_gen_summary set approve_status=1 where daily_gen_summary_id=" + dailyGenSummary[i].daily_gen_summary_id+";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {

            string qry = "";
            for (int i = 0; i < dailyGenSummary.Count; i++)
            {

                qry += "delete from daily_gen_summary  where daily_gen_summary_id=" + dailyGenSummary[i].daily_gen_summary_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {

            string qry = "";
            for (int i = 0; i < windDailyBreakdownReport.Count; i++)
            {

                qry += "update uploading_file_breakdown set approve_status=1 where uploading_file_breakdown_id=" + windDailyBreakdownReport[i].uploading_file_breakdown_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {

            string qry = "";
            for (int i = 0; i < windDailyBreakdownReport.Count; i++)
            {

                qry += "delete from uploading_file_breakdown where uploading_file_breakdown_id=" + windDailyBreakdownReport[i].uploading_file_breakdown_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            string temptable = "tempsolarData";
            string qry = "";
            string ids = "";
            if (approve_status == 1)
            {


                for (int i = 0; i < solarDailyGenSummary.Count; i++)
                {
                    ids += solarDailyGenSummary[i].daily_gen_summary_solar_id + ",";
                }
                ids = ids.TrimEnd(',');


                string dataQry = @"select t1.date,month(t1.date)as month,t1.site,
(inv_kwh) as inv_kwh,
(t1.poa) as IR,
(select replace(lineloss,'%','') from monthly_line_loss_solar where site=t1.site and month=DATE_FORMAT(t1.date, '%b')
 and fy='2022-23' order by monthly_line_loss_solar_id desc limit 1 )as line_loss,
(inv_kwh)-((inv_kwh) * (select replace(lineloss,'%','') from monthly_line_loss_solar where
 site=t1.site and month=DATE_FORMAT(t1.date, '%b')
 and fy='2022-23' order by monthly_line_loss_solar_id desc limit 1 ) /100) as jmrkwh,
(select gen_nos*1000000 from daily_target_kpi_solar where sites=t1.site and date=t1.date 
order by daily_target_kpi_solar_id desc limit 1) as tarkwh, 
(select poa from daily_target_kpi_solar where sites=t1.site and date=t1.date order by 
daily_target_kpi_solar_id desc limit 1) as tarIR from daily_gen_summary_solar t1 where t1.daily_gen_summary_solar_id in(" + ids + ") ";

                string qrynew = @"drop TEMPORARY table IF EXISTS " + temptable + " ;  CREATE TEMPORARY TABLE " + temptable + " " + dataQry + " ;insert into tblsolardata (Date,Month,Site,inv_kwh,IR,line_loss,jmrkwh,tarkwh,tarIR) select Date,Month,Site,inv_kwh,IR,line_loss,jmrkwh,tarkwh,tarIR from " + temptable + ";";

                await Context.ExecuteNonQry<int>(qrynew).ConfigureAwait(false);
            }
            for (int i = 0; i < solarDailyGenSummary.Count; i++)
            {

                qry += "update daily_gen_summary_solar set approve_status=1 where daily_gen_summary_solar_id=" + solarDailyGenSummary[i].daily_gen_summary_solar_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {

            string qry = "";
            for (int i = 0; i < solarDailyGenSummary.Count; i++)
            {

                qry += "delete from daily_gen_summary_solar  where daily_gen_summary_solar_id=" + solarDailyGenSummary[i].daily_gen_summary_solar_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {

            string qry = "";
            for (int i = 0; i < solarFileBreakdown.Count; i++)
            {

                qry += "update daily_bd_loss_solar set approve_status=1 where daily_bd_loss_solar_id=" + solarFileBreakdown[i].daily_bd_loss_solar_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {

            string qry = "";
            for (int i = 0; i < solarFileBreakdown.Count; i++)
            {

                qry += "delete from daily_bd_loss_solar where daily_bd_loss_solar_id=" + solarFileBreakdown[i].daily_bd_loss_solar_id + ";";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        //LOgin 
      /*  internal async Task<List<UserLogin>> GetLogin(string username, string password)
        {
            string qry = "";
            qry = "SELECT * FROM `login` where `username`='" + username + "' and `password` ='" + password + "' ;";
           // Console.WriteLine(qry);
            return await Context.GetData<UserLogin>(qry).ConfigureAwait(false);

        }*/
        ////#endregion

        //  await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        public async Task<List<approvalObject>> GetImportBatches(string importFromDate, string importToDate, string siteId, int importType,int status,int userid)
        {

            string filter = "";
            if (!string.IsNullOrEmpty(siteId) && siteId != "All")
            {
               filter += " and ib.site_id IN(" + siteId+")";
            }
            if(status != -1)
            {
                filter += " and ib.is_approved  =" + status;
            }
            if(userid !=0)
            {
                filter += " and ib.imported_by  =" + userid;
            }
            filter += " group by t3.import_batch_id";
            //string query = "select * from import_batches where import_date>=" + importFromDate + " and import_date>=" + importToDate + " and import_type='2' and is_approved  ='" + status +"' " +filter+"";
            string query = "select ib.*,sm.site as site_name from import_batches as ib join site_master as sm on sm.site_master_id=ib.site_id join uploading_file_generation as t3 on t3.import_batch_id=ib.import_batch_id where DATE(ib.import_date)>='" + importFromDate + "' and DATE(ib.import_date) <='" + importToDate + "'and `file_name` like '%DGR_Automation%' and ib.import_type=" + importType + "" + filter + "";
            List<approvalObject> _approvalObject = new List<approvalObject>();
            _approvalObject = await Context.GetData<approvalObject>(query).ConfigureAwait(false);
            return _approvalObject;

        }
       
         internal async Task<int> SetApprovalFlagForImportBatches(string dataId, int approvedBy, string approvedByName, int status)
        {
            
            string qry = "select t1.*,t2.site,t2.country,t2.state,t3.feeder from uploading_file_generation as t1 left join site_master as t2 on t2.site_master_id=t1.site_id left join location_master as t3 on t3.site_master_id=t1.site_id where import_batch_id IN(" + dataId+")";

            List<WindUploadingFilegeneration2> _importedData = new List<WindUploadingFilegeneration2>();
            _importedData = await Context.GetData<WindUploadingFilegeneration2>(qry).ConfigureAwait(false);

            string qry1 = " insert into daily_gen_summary(state, site,site_id, date, wtg, wind_speed, kwh, kwh_afterlineloss, feeder, ma_contractual, ma_actual, iga, ega, plf,plf_afterlineloss,capacity_kw, grid_hrs, lull_hrs, production_hrs, unschedule_hrs, schedule_hrs, others, igbdh, egbdh, load_shedding, approve_status) values";
            string values = "";
            
            foreach (var unit in _importedData)
            {
               
                values += "('"+unit.state+"','" + unit.site + "','" + unit.site_id + "','" + unit.date + "','" + unit.wtg + "','" + unit.wind_speed + "','" + unit.kwh + "','" + unit.kwh_afterlineloss + "','" + unit.feeder+"','" + unit.ma_contractual + "','" + unit.ma_actual + "','" + unit.iga + "','" + unit.ega + "','" + unit.plf + "','" + unit.plf_afterlineloss + "','" + unit.capacity_kw + "','" + unit.grid_hrs + "','"+unit.lull_hrs+"','" + unit.operating_hrs + "','" + unit.unschedule_hrs + "','" + unit.schedule_hrs + "','" + unit.others + "','" + unit.igbdh + "','" + unit.egbdh + "','" + unit.load_shedding + "','1'),";
            }

            qry1 += values;
            int res= await Context.ExecuteNonQry<int>(qry1.Substring(0, (qry1.Length - 1)) + ";").ConfigureAwait(false);
            if(res > 0)
            {
                string query = "UPDATE `import_batches` SET `approval_date` = NOW(),`approved_by`= " + approvedBy + ",`is_approved`=" + status + ",`approved_by_name`='" + approvedByName + "' WHERE `import_batch_id` IN(" + dataId + ")";
                return await Context.ExecuteNonQry<int>(query).ConfigureAwait(false);
            }
            else
            {
                return 0;
            }

        }

        internal async Task<int> SetRejectFlagForImportBatches(string dataId, int rejectedBy, string rejectByName, int status)
        {

           /* string qry = "select t1.*,t2.site,t2.country,t2.state,t3.feeder from uploading_file_generation as t1 left join site_master as t2 on t2.site_master_id=t1.site_id left join location_master as t3 on t3.site_master_id=t1.site_id where import_batch_id IN(" + dataId + ")";

            List<WindUploadingFilegeneration2> _importedData = new List<WindUploadingFilegeneration2>();
            _importedData = await Context.GetData<WindUploadingFilegeneration2>(qry).ConfigureAwait(false);

            string qry1 = " insert into daily_gen_summary(state, site,site_id, date, wtg, wind_speed, kwh, feeder, ma_contractual, ma_actual, iga, ega, plf, grid_hrs, lull_hrs, production_hrs, unschedule_hrs, schedule_hrs, others, igbdh, egbdh, load_shedding, approve_status) values";
            string values = "";

            foreach (var unit in _importedData)
            {

                values += "('" + unit.state + "','" + unit.site + "','" + unit.site_id + "','" + unit.date + "','" + unit.wtg + "','" + unit.wind_speed + "','" + unit.kwh + "','" + unit.feeder + "','" + unit.ma_contractual + "','" + unit.ma_actual + "','" + unit.iga + "','" + unit.ega + "','" + unit.plf + "','" + unit.grid_hrs + "','" + unit.operating_hrs + "','" + unit.production_rs + "','" + unit.unschedule_hrs + "','" + unit.schedule_hrs + "','" + unit.others + "','" + unit.igbdh + "','" + unit.egbdh + "','" + unit.load_shedding + "','1'),";
            }

            qry1 += values;
            int res = await Context.ExecuteNonQry<int>(qry1.Substring(0, (qry1.Length - 1)) + ";").ConfigureAwait(false);
            if (res > 0)
            {*/
                string query = "UPDATE `import_batches` SET `rejected_date` = NOW(),`rejected_by`= " + rejectedBy + ",`is_approved`=" + status + ",`rejected_by_name`='" + rejectByName + "' WHERE `import_batch_id` IN(" + dataId + ")";
                return await Context.ExecuteNonQry<int>(query).ConfigureAwait(false);
            /*}
            else
            {
                return 0;
            }*/

        }
        public async Task<List<CountryList>> GetCountryData()
        { 

            string query = "SELECT country FROM `site_master` group by country";
            List<CountryList> _country = new List<CountryList>();
            _country = await Context.GetData<CountryList>(query).ConfigureAwait(false);
            return _country;

        }
        public async Task<List<StateList>> GetStateData(string country)
        {

            string query = "SELECT state FROM `site_master` where country='"+country + "' group by state";
            List<StateList> _state = new List<StateList>();
            _state = await Context.GetData<StateList>(query).ConfigureAwait(false);
            return _state;

        }
        public async Task<List<SPVList>> GetSPVData(string state)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(state) && state != "All")
            {
                filter += " where state IN(" + state + ")";
            }
            string query = "SELECT spv FROM `site_master` "+ filter + " group by spv";
            List<SPVList> _spvlist = new List<SPVList>();
            _spvlist = await Context.GetData<SPVList>(query).ConfigureAwait(false);
            return _spvlist;

        }
        public async Task<List<WindSiteMaster>> GetSiteData(string state, string spv,string site)
        {
            string filter = "";
            if(!string.IsNullOrEmpty(site) || !string.IsNullOrEmpty(state) || !string.IsNullOrEmpty(spv))
            {
                filter += " where ";
            }
            if (!string.IsNullOrEmpty(site) && string.IsNullOrEmpty(site))
            {
                filter += " site_master_id IN(" + site + ")";
            }

            if (!string.IsNullOrEmpty(state) && state != "All")
            {
              
                string[] siteSplit = state.Split(",");
                if (siteSplit.Length > 0)
                {
                    string statenames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            statenames += "'" + siteSplit[i] + "',";
                        }
                    }
                    statenames = statenames.TrimEnd(',');
                    //filter += " and site in(" + sitesnames + ")";

                    filter += " state IN(" + statenames + ")";
                }

            }
            if (!string.IsNullOrEmpty(spv) && spv != "All")
            {

                string[] spvSplit = spv.Split(",");
                if (spvSplit.Length > 0)
                {
                    string spvnames = "";
                    for (int i = 0; i < spvSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(spvSplit[i]))
                        {
                            spvnames += "'" + spvSplit[i] + "',";
                        }
                    }
                    spvnames = spvnames.TrimEnd(',');
                    //filter += " and site in(" + sitesnames + ")";

                    filter += " and spv IN(" + spvnames + ")";
                    //filter += " where state='" + state + "' and spv='" + spv + "'";
                }

            }

            // if (!string.IsNullOrEmpty(state) && string.IsNullOrEmpty(spv))
            //{
            //  filter += " where state='" + state + "'";
            //}
            ///if (!string.IsNullOrEmpty(spv) && !string.IsNullOrEmpty(state))
            //{
             //   filter += " where state='" + state + "' and spv='" + spv+"'";
            //}
            

            string query = "SELECT * FROM `site_master`" + filter;
            List<WindSiteMaster> _sitelist = new List<WindSiteMaster>();
            _sitelist = await Context.GetData<WindSiteMaster>(query).ConfigureAwait(false);
            return _sitelist;

        }
        public async Task<List<WindLocationMaster>> GetWTGData(string siteid)
        {

            string query = "SELECT * FROM `location_master` where site_master_id IN("+ siteid + ")";
            List<WindLocationMaster> _locattionmasterDate = new List<WindLocationMaster>();
            _locattionmasterDate = await Context.GetData<WindLocationMaster>(query).ConfigureAwait(false);
            return _locattionmasterDate;

        }
        
         public async Task<List<WindUploadingFilegeneration1>> GetImportGenData(int importId)
        {
            // string query = "SELECT t1.*,t2.site as site_name FROM `uploading_file_generation` as t1 join site_master as t2 on t2.site_master_id=t1.site_id  where import_batch_id =" + importId + "";
            string query = "SELECT t1.uploading_file_generation_id, t1.site_id, t1.date, t1.wtg, t1.wtg_id, t1.wind_speed, t1.grid_hrs, t1.operating_hrs, t1.lull_hrs, t1.kwh, t1.ma_contractual, t1.ma_actual, t1.iga, t1.ega, t1.plf,  t1.unschedule_hrs,  t1.schedule_hrs,  t1.others,  t1.igbdh,  t1.egbdh,  t1.load_shedding, t2.site as site_name FROM `uploading_file_generation` as t1 join site_master as t2 on t2.site_master_id=t1.site_id where import_batch_id =" + importId + "";
            List<WindUploadingFilegeneration1> _importGenData = new List<WindUploadingFilegeneration1>();
            _importGenData = await Context.GetData<WindUploadingFilegeneration1>(query).ConfigureAwait(false);
            return _importGenData;
           
        }
        public async Task<List<WindUploadingFileBreakDown1>> GetBrekdownImportData(int importId)
        {
            // string query = "SELECT t1.*,t2.site as site_name FROM `uploading_file_generation` as t1 join site_master as t2 on t2.site_master_id=t1.site_id  where import_batch_id =" + importId + "";
            //string query = "SELECT * FROM `uploading_file_breakdown` where import_batch_id =" + importId + "";
            string query = " SELECT t1.site_id,t1.date,t1.wtg,t1.stop_from,t1.stop_to,t1.total_stop,t1.error_description,t2.site,t3.bd_type_name FROM `uploading_file_breakdown` as t1 left join site_master as t2 on t2.site_master_id = t1.site_id left join bd_type as t3 on t3.bd_type_id = t1.bd_type_id where import_batch_id = " + importId + "";
            List<WindUploadingFileBreakDown1> _importBreakdownData = new List<WindUploadingFileBreakDown1>();
            _importBreakdownData = await Context.GetData<WindUploadingFileBreakDown1>(query).ConfigureAwait(false);
            return _importBreakdownData;

        }


        //#region KPI Calculations
        /// <summary>
        /// This function calculates the KPI of the site on a given date
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        /// 
        internal async Task<bool> CalculateDailyWindKPI(string fromDate, string toDate, string site)
        {
            string filter = "";
            bool response = false;

            TimeSpan Final_USMH_Time = new TimeSpan();
            TimeSpan Final_SMH_Time = new TimeSpan();
            TimeSpan Final_IGBD_Time = new TimeSpan();
            TimeSpan Final_EGBD_Time = new TimeSpan();
            TimeSpan Final_LoadShedding_Time = new TimeSpan();
            TimeSpan Final_LULL_Time = new TimeSpan();
            TimeSpan Final_OthersHour_Time = new TimeSpan();

            string MA_Actual_Formula = "";
            string MA_Contractual_Formula = "";
            string IGA_Formula = "";
            string EGA_Formula = "";
            string sCurrentWTG = "";
            string sLastWTG = "";
            double capacity_mw = 0;
            try
            {
                if (string.IsNullOrEmpty(site) || site == "All")
                {
                    throw new Exception("Invalid site " + site);
                    //return response;
                }

                int site_id = int.Parse(site);

                if (site_id <= 0)
                {
                    throw new Exception("Invalid site " + site);
                    //return response;
                }

                //string qrySiteFormulas = "SELECT * FROM `wind_site_formulas` where site_id = '" + site_id + "'";
                string qrySiteFormulas = "SELECT t1.*,t2.capacity_mw FROM `wind_site_formulas` as t1 left join `site_master` as t2 on t2.site_master_id = t1.site_id where t1.site_id = '" + site_id + "'";
                List<SiteFormulas> _SiteFormulas = await Context.GetData<SiteFormulas>(qrySiteFormulas).ConfigureAwait(false);
                foreach (SiteFormulas SiteFormula in _SiteFormulas)
                {
                    MA_Actual_Formula = SiteFormula.MA_Actual; //(string)reader["MA_Actual"];
                    MA_Contractual_Formula = SiteFormula.MA_Contractual; // (string)reader["MA_Contractual"];
                    IGA_Formula = SiteFormula.IGA; // (string)reader["IGA"];
                    EGA_Formula = SiteFormula.EGA; // (string)reader["EGA"];
                                                   //break;
                    capacity_mw = SiteFormula.capacity_mw;
                }

                //string qryFileBreakdown = "SELECT fd.site_id,fd.bd_type,fd.wtg,bd.bd_type_name, SEC_TO_TIME(SUM(TIME_TO_SEC( fd.`total_stop` ) ) ) AS totalTime FROM `uploading_file_breakdown` as fd join bd_type as bd on bd.bd_type_id=fd.bd_type where site_id = " + site_id + " AND`date` = '" + fromDate + "' group by fd.wtg, fd.bd_type";
                string qry = @"SELECT date,t1.site_id,t1.wtg,t1.bd_type_id,t1.bd_type,SEC_TO_TIME(SUM(TIME_TO_SEC(total_stop)))  AS total_stop FROM uploading_file_breakdown t1 left join location_master t2 on t2.wtg=t1.wtg left join site_master t3 on t3.site_master_id=t2.site_master_id left join bd_type as t4 on t4.bd_type_id=t1.bd_type ";
                int iBreakdownCount = 0;
                filter = "";
                int chkfilter = 0;
                filter = "" + site_id;
                if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
                {
                    //filter += "(date >= '" + fromDate + "'  and date<= '" + toDate + "')";
                    filter += " AND date = '" + fromDate + "'";
                    chkfilter = 1;
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    qry += " where  site_id = " + filter;
                }
                //qry += "  AND t1.wtg = 'BD-25'";
                qry += "  group by t1.wtg, t1.bd_type";
                List<WindFileBreakdown> _WindFileBreakdown = await Context.GetData<WindFileBreakdown>(qry).ConfigureAwait(false);
                foreach (WindFileBreakdown sBreakdown in _WindFileBreakdown)
                {
                    iBreakdownCount++;
                    DateTime result;
                    TimeSpan Get_Time;
                    int site_id2 = sBreakdown.site_id;
                    sCurrentWTG = sBreakdown.wtg; // (string)reader["wtg"];
                    int bd_type_id = sBreakdown.bd_type_id;// reader["bd_type"];
                    string bd_type_name = sBreakdown.bd_type; // reader["bd_type_name"];
                    var totalTime = sBreakdown.total_stop;// reader["totalTime"];

                    if (iBreakdownCount == 1)
                    {
                        sLastWTG = sCurrentWTG;
                    }
                    if (sCurrentWTG != sLastWTG)
                    {
                        //Update WTG KPIs
                        CalculateAndUpdateKPIs(site_id, fromDate, sLastWTG, Final_USMH_Time, Final_SMH_Time, Final_IGBD_Time, Final_EGBD_Time, Final_OthersHour_Time, Final_LoadShedding_Time, MA_Actual_Formula, MA_Contractual_Formula, IGA_Formula, EGA_Formula);
                       // CalculateAndUpdatePLFandKWHAfterLineLoss(site_id, fromDate, sLastWTG);
                        Final_USMH_Time = TimeSpan.Zero;
                        Final_SMH_Time = TimeSpan.Zero;
                        Final_IGBD_Time = TimeSpan.Zero;
                        Final_EGBD_Time = TimeSpan.Zero;
                        Final_LoadShedding_Time = TimeSpan.Zero;
                        Final_LULL_Time = TimeSpan.Zero;
                        Final_OthersHour_Time = TimeSpan.Zero;

                        sLastWTG = sCurrentWTG;
                    }
                    switch (bd_type_id)
                    {
                        case 1:                 //if (bd_type_name.Equals("USMH"))            //Pending : optimise it use bd_type id
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_USMH_Time = result.TimeOfDay;
                            break;

                        case 2:                 //else if (bd_type_name.Equals("SMH"))              
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_SMH_Time = result.TimeOfDay;
                            break;

                        case 3:                 //else if (bd_type_name.Equals("IGBD"))                
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_IGBD_Time = result.TimeOfDay;
                            break;

                        case 4:                 //else if (bd_type_name.Equals("EGBD"))                
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_EGBD_Time = result.TimeOfDay;
                            break;

                        case 5:                 //if (bd_type_name.Equals("Load Shedding"))                
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_LoadShedding_Time = result.TimeOfDay;
                            break;

                        case 6:                 //if (bd_type_name.Equals("Others Hour"))                
                            result = Convert.ToDateTime(totalTime.ToString());
                            Final_OthersHour_Time = result.TimeOfDay;
                            break;

                        default:
                            //Pending : error reporting
                            throw new Exception("Unsupported BD_TYPE " + bd_type_id + " For WTG " + sCurrentWTG + " for date " + fromDate);
                            break;

                    }
                }

                //Pending : validation of Total time to be 24
                CalculateAndUpdateKPIs(site_id, fromDate, sCurrentWTG, Final_USMH_Time, Final_SMH_Time, Final_IGBD_Time, Final_EGBD_Time, Final_OthersHour_Time, Final_LoadShedding_Time, MA_Actual_Formula, MA_Contractual_Formula, IGA_Formula, EGA_Formula);
                 CalculateAndUpdatePLFandKWHAfterLineLoss(site_id, fromDate, toDate, capacity_mw);
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                response = false;
                //pending : log error
                throw new Exception(strEx);
            }

            return response;
        }
        /*
         * Import -> Upload_Generation table and upload_breakdown and kpis are calculted in upload_gen table
         * Approval - > data from upload_gen table gets copied to daily summary table
         * 
         * */
        /*
         * When line loss is updated
         * kpis are calculted in gen_summary table
         * */
        
        public async Task<bool> CalculateAndUpdatePLFandKWHAfterLineLoss(int site_id, string fromDate, string toDate,double capacity_mw)
        {
            //add column called kwh_afterlineloss and plf_afterlineloss in dailygensummary and uploadgentable
            double lineLoss = await GetLineLoss(site_id, fromDate);
            bool bIsGenSummary = false;
            /*string qry = "SELECT * from daily_gen_summary where site_id = " + site_id + " and date>='"+fromDate+"' and date<='"+toDate+"'";
            try
            {
                List<DailyGenSummary> checkIfApproved = await Context.GetData<DailyGenSummary>(qry).ConfigureAwait(false);
                if (checkIfApproved.Count > 0)
                    bIsGenSummary = true;
            }
            catch(Exception ex)
            {
                //Pending Error 
            }*/
           
           
            lineLoss =1-(lineLoss/100);
            return await CalculateAndUpdatePLFandKWHAfterLineLoss2(site_id, fromDate, toDate, lineLoss, bIsGenSummary, capacity_mw);
        }
        public async Task<bool> CalculateAndUpdatePLFandKWHAfterLineLoss2(int site_id, string fromDate, string toDate, double lineloss, bool bIsGenSummary,double capacity_mw)
        {
            //Pending : Add information to log file
            string sLog = "PLF and KWH updated for site id =" + site_id + " fromDate=" + fromDate + " toDate=" + toDate;
            string tableName;
            if (bIsGenSummary)
            {
                tableName = "daily_gen_summary";  //Approved data
            }
            else
            {
                tableName = "uploading_file_generation"; //unapproved data
            }

            //Add capacity_kw in dailygen summary
          //return  await updateTable(site_id, tableName, lineloss, capacity_mw, fromDate, toDate);

            //string myQuery = "Update " + tableName +
              //  " set kwh_afterlineloss = kwh * "+lineloss+", " +
               // " plf = (kwh/"+capacity_mw+"*1000*24), " +
                //"plf_afterlineloss =  ((kwh * "+lineloss+ ")/"+capacity_mw+"*1000*24)" +
                //" where date>='" + fromDate + "' and date<='" + toDate + "' and site_id=" + site_id;
           
            string myQuery = "Update " + tableName +
                " set kwh_afterlineloss = kwh * " + lineloss + ", " +
                " plf = (kwh/(" + capacity_mw + "*1000*24) * 100), " +
                "plf_afterlineloss =  ((kwh * " + lineloss + ")/(" + capacity_mw + "*1000*24) * 100)" +
                " where date>='" + fromDate + "' and date<='" + toDate + "' and site_id=" + site_id;
            //int result = await Context.ExecuteNonQry<int>(myQuery.Substring(0, (myQuery.Length - 1)) + ";").ConfigureAwait(false);
            int result = await getDB.ExecuteNonQry<int>(myQuery).ConfigureAwait(false);
            if (result > 0)
                return true;
            return false;
        }
        //Refrence 
       /* internal async Task<int> updateTable(int site_id, string tableName, double lineloss, double capacity_mw, string fromDate, string toDate)
        {
            string myQuery = "Update " + tableName +
               " set kwh_afterlineloss = kwh * " + lineloss + ", " +
               " plf = (kwh/" + capacity_mw + "*1000*24), " +
               "plf_afterlineloss =  ((kwh * " + lineloss + ")/" + capacity_mw + "*1000*24)" +
               " where date>='" + fromDate + "' and date<='" + toDate + "' and site_id=" + site_id;
            return await getDB.ExecuteNonQry<int>(myQuery).ConfigureAwait(false);

        }*/
        /*private async Task<bool> CalculateAndUpdatePLFandKWHAfterLineLoss(int site_id, string fromDate, string sWTG_Name)
        {
            //site_id, string fromDate, string sWTG_Name
            double dLineLoss = 0.65;
            string fy = GetFY(fromDate);
            string site = GetSiteFromSiteID(site_id);
            string month = GetMonth(fromDate);

            bool response = false;
            try
            {
                int chkfilter = 0;
                string filter = "";
                int iGenerationCount = 0;

                string qryLineLoass = @"SELECT  fy, month,site,line_loss as LineLoss FROM monthly_uploading_line_losses where site = '" + site + "' and fy = '" + fy + "' and month = '" + month + "'";

                List<WindMonthlyUploadingLineLosses> _WindMonthlyUploadingLineLosses = await Context.GetData<WindMonthlyUploadingLineLosses>(qryLineLoass).ConfigureAwait(false);

                foreach (WindMonthlyUploadingLineLosses WindMonthlyLineLosses in _WindMonthlyUploadingLineLosses)
                {
                    dLineLoss = double.Parse(WindMonthlyLineLosses.lineLoss);
                    break;

                }

                string sLog = "Updating WTG <" + sWTG_Name + ">  KWH and PLF paramters.";
                //Pending : Log the result
                string qry = @"SELECT uploading_file_generation_id, date,t1.wtg,t2.site_master_id as site_id,t2.site as site_name,t1.kwh,t3.capacity_mw FROM uploading_file_generation t1 left join location_master t2 on t2.wtg=t1.wtg left join site_master t3 on t3.site_master_id=t2.site_master_id";
                //,t1.wind_speed
                if (!string.IsNullOrEmpty(fromDate) && fromDate != "All")
                {
                    //filter += "(date >= '" + fromDate + "'  and date<= '" + toDate + "')";
                    filter += "(date = '" + fromDate + "')";
                    chkfilter = 1;
                }

                if (!string.IsNullOrEmpty(filter))
                {
                    qry += " where  " + filter;
                }
                qry += " order by t1.wtg";

                double dCalculatedPLF = 0; ;
                double dWTG_Capacity = 0;
                double dDaily_kwh = 0;
                double dKWH_AfterLineLoss = 0;
                int iCount = 0;
                int id = 0;
                string updateQry = "UPDATE uploading_file_generation s JOIN( ";

                List<WindUploadedData> _WindUploadedData = await Context.GetData<WindUploadedData>(qry).ConfigureAwait(false);
                foreach (WindUploadedData WindGeneration in _WindUploadedData)
                {
                    iCount++;
                    id = WindGeneration.uploading_file_generation_id;
                    dDaily_kwh = (double)WindGeneration.kwh;
                    dWTG_Capacity = (double)WindGeneration.capacity_mw;
                    dCalculatedPLF = Math.Round(dDaily_kwh / (dWTG_Capacity * 1000 * 24) * 100, 6);
                    dKWH_AfterLineLoss = Math.Round(dDaily_kwh - Math.Round(dDaily_kwh * dLineLoss / 100));
                    if (iCount == 1)
                    {
                        updateQry += "SELECT " + id + " as id, " + dKWH_AfterLineLoss + " as new_kwh, " + dWTG_Capacity + " as wtg_capacity, " + dCalculatedPLF + " as new_plf";
                    }
                    else
                    {
                        updateQry += " UNION ALL ";
                        updateQry += "SELECT " + id + ", " + dKWH_AfterLineLoss + ", " + dWTG_Capacity + ", " + dCalculatedPLF + "";
                    }
                }

                updateQry += ") vals ON s.uploading_file_generation_id = vals.id";
                updateQry += " SET kwh_afterloss = new_kwh, capacity_kw = wtg_capacity, plf = new_plf;";

                if (iCount > 0)
                {
                    int result = await Context.ExecuteNonQry<int>(updateQry).ConfigureAwait(false);
                    if (result > 0)
                        response = true;
                }
            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                //pending : log error
                throw;

            }
            return response;
        }*/
        string GetFY(string fromDate)
        {
            string sFY;
            DateTime dt = Convert.ToDateTime(fromDate);
            int year = dt.Year;
            int month = dt.Month;
            if (month >= 4)
            {
                int nextyear = year % 2000 + 1;
                sFY = year + "-" + nextyear;
            }
            else
            {

                int prevyear = year - 1;
                year = year % 2000;
                sFY = prevyear + "-" + year;
            }
            return sFY;
        }

        string GetMonth(string fromDate)
        {
            DateTime dt = Convert.ToDateTime(fromDate);
            int month = dt.Month;
            string sMonth = Months[month - 1];
            return sMonth;
        }
        string GetSiteFromSiteID(int site_id)
        {
            if (!m_bSiteMasterLoaded)
            {
                //make query and create hashtable

                m_bSiteMasterLoaded = true;

            }
            //get from hashtable member variable
            return "Badnawar";
        }
        private async Task<double> GetLineLoss(int site_id, string fromDate)
        {
            double dLineLoss = 0.65;

            /*
            string fy = GetFY(fromDate);
            string site = GetSiteFromSiteID(site_id);
            string month = GetMonth(fromDate);

            string filter = " fy='" + fy + "' ";

            if (!string.IsNullOrEmpty(site) && site != "All")
            {
                // filter += " and site='" + site + "'";

                string[] siteSplit = site.Split("~");
                if (siteSplit.Length > 0)
                {
                    string sitesnames = "";
                    for (int i = 0; i < siteSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(siteSplit[i]))
                        {
                            sitesnames += "'" + siteSplit[i] + "',";
                        }
                    }
                    sitesnames = sitesnames.TrimEnd(',');
                    filter += " and site in(" + sitesnames + ")";
                }
            }

            if (!string.IsNullOrEmpty(month) && month != "All")
            {
                string[] monthSplit = month.Split("~");
                if (monthSplit.Length > 0)
                {
                    string monthnames = "";
                    for (int i = 0; i < monthSplit.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(monthSplit[i]))
                        {
                            monthnames += "'" + monthSplit[i] + "',";
                        }
                    }
                    monthnames = monthnames.TrimEnd(',');
                    filter += " and month in(" + monthnames + ")";
                }

            }
            string qry = @"SELECT  fy, month,site as Sites,line_loss as LineLoss FROM monthly_uploading_line_losses where " + filter;
            
             */
            DateTime dt = Convert.ToDateTime(fromDate);
            int year = dt.Year;
            int month = dt.Month;

            string qry = @"SELECT  fy, month_no, site, line_loss as lineLoss FROM monthly_uploading_line_losses where site_id = " + site_id + " and year = " + year + " and month_no = " + month;

            List<WindMonthlyUploadingLineLosses> _WindMonthlyUploadingLineLosses = await Context.GetData<WindMonthlyUploadingLineLosses>(qry).ConfigureAwait(false);

            foreach (WindMonthlyUploadingLineLosses WindMonthlyLineLosses in _WindMonthlyUploadingLineLosses)
            {
                dLineLoss = WindMonthlyLineLosses.lineLoss;//double.Parse(WindMonthlyLineLosses.lineLoss);
                break;
            }
            return dLineLoss;
        }
        /// <summary>
        /// This function calculates the KPI of the given WTG and update the KPI to uploading_file_generation table. 
        /// Approval function then copy this data to daily_generaition_summary table
        /// </summary>
        /// <param name="site_id"></param>
        /// <param name="fromDate"></param>
        /// <param name="sWTG_Name"></param>
        /// <param name="Final_USMH"></param>
        /// <param name="Final_SMH"></param>
        /// <param name="Final_IGBD"></param>
        /// <param name="Final_EGBD"></param>
        /// <param name="Final_OthersHour"></param>
        /// <param name="Final_LoadShedding"></param>
        /// <param name="MA_Actual_FormulaID"></param>
        /// <param name="MA_Contractual_FormulaID"></param>
        /// <param name="IGA_FormulaID"></param>
        /// <param name="EGA_FormulaID"></param>
        /// <returns></returns>
        private async Task<bool> CalculateAndUpdateKPIs(int site_id, string fromDate, string sWTG_Name, TimeSpan Final_USMH_Time, TimeSpan Final_SMH_Time, TimeSpan Final_IGBD_Time, TimeSpan Final_EGBD_Time, TimeSpan Final_OthersHour_Time, TimeSpan Final_LoadShedding_Time, string MA_Actual_Formula, string MA_Contractual_Formula, string IGA_Formula, string EGA_Formula)
        {
            bool response = false;
            double Final_USMH = 0;
            double Final_SMH = 0;
            double Final_IGBD = 0;
            double Final_EGBD = 0;
            double Final_LoadShedding = 0;
            double Final_LULL = 0;
            double Final_OthersHour = 0;
            TimeSpan Get_Time;
            try
            {
                string sLog = "Updating WTG <" + sWTG_Name + "> KPI paramters.";
                //Pending : Log the result

                Get_Time = Final_USMH_Time * 24;
                Final_USMH = Get_Time.TotalDays;
                Get_Time = Final_SMH_Time * 24;
                Final_SMH = Get_Time.TotalDays;
                Get_Time = Final_IGBD_Time * 24;
                Final_IGBD = Get_Time.TotalDays;
                Get_Time = Final_EGBD_Time * 24;
                Final_EGBD = Get_Time.TotalDays;
                Get_Time = Final_LoadShedding_Time * 24;
                Final_LoadShedding = Get_Time.TotalDays;
                Get_Time = Final_OthersHour_Time * 24;
                Final_OthersHour = Get_Time.TotalDays;
                
                

                double dMA_ACT = Math.Round(GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Actual_Formula), 6);
                double dMA_CON = Math.Round(GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, MA_Contractual_Formula), 6);
                double dIGA = Math.Round(GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, IGA_Formula), 6);
                double dEGA = Math.Round(GetCalculatedValue(Final_USMH, Final_SMH, Final_IGBD, Final_EGBD, Final_OthersHour, Final_LoadShedding, EGA_Formula), 6);

              
                string qryUpdate = "UPDATE `uploading_file_generation` set ma_actual = " + dMA_ACT + ", ma_contractual = " + dMA_CON + ", iga = " + dIGA + ", ega = " + dEGA;
                qryUpdate += ", unschedule_hrs = '" + Final_USMH_Time + "', schedule_hrs = '" + Final_SMH_Time + "', igbdh = '" + Final_IGBD_Time + "', egbdh = '" + Final_EGBD_Time + "', others = '" + Final_OthersHour_Time + "', load_shedding = '" + Final_LoadShedding_Time + "'";
                qryUpdate += " where wtg = '" + sWTG_Name + "' and date = '" + fromDate + "'";

                int result = await Context.ExecuteNonQry<int>(qryUpdate).ConfigureAwait(false);
                if (result > 0)
                    response = true;

            }
            catch (Exception ex)
            {
                string strEx = ex.ToString();
                throw;

            }
            finally
            {
                int i = 0;
            }
            return response;
        }

        /// <summary>
        /// This function caluldates the value as per the formula type 
        /// </summary>
        /// <param name="U"></param>
        /// <param name="S"></param>
        /// <param name="IG"></param>
        /// <param name="EG"></param>
        /// <param name="OthersHour"></param>
        /// <param name="LoadShedding"></param>
        /// <param name="Formula"></param>
        /// <returns></returns>
        private double GetCalculatedValue(double U, double S, double IG, double EG, double OthersHour, double LoadShedding, string Formula)
        {
            double returnValue = 0;
            //Pending : Iteration 2 => to add a formula parser to evaludate formuals as defined by user
            switch (Formula)
            {
                case "24-(USMH+SMH))/24": // MA_Actual_FormulaID / //Machine Availability Actual
                    returnValue = (24 - (U + S)) / 24;
                    break;
                case "(24-(USMH+SMH+IG))/24": // MA_Contractual_FormulaID / //Machine Availability Contractual
                    returnValue = (24 - (U + S + IG)) / 24;
                    break;
                case "(24-(IG))/24"://Internal Grid Availability 
                    returnValue = (24 - (IG)) / 24;
                    break;
                case "(24-(EG))/24": // External_Grid_FormulaID///External Grid Availablity
                    returnValue = (24 - (EG)) / 24;
                    break;
                case "(24-(OTHER+EG+USMH+SMH+IG))/(24-(EG+OTHER))": // /MA_Contractual_FormulaID for 190,191,197,198,199/ //Machine Availability Contractual
                    returnValue = (24 - (OthersHour + EG + U + S + IG)) / (24 - (EG + OthersHour));
                    break;
                case "(24-(OTHER+EG+USMH+SMH+IG))/(24-(EG+OTHER+IG))":
                    returnValue = (24 - (OthersHour + EG + U + S + IG)) / (24 - (EG + OthersHour + IG));
                    break;
                case "(24-(EG+LS))/24":
                    returnValue = (24 - (EG + LoadShedding)) / 24;
                    break;
                default:
                    //Pending : error reporting
                    //throw;
                    break;
            }
            return returnValue * 100;
        }

        ////#endregion //KPI Calculations
    }
//}

    internal class ViewerStatsFormat
    {
    }
}
