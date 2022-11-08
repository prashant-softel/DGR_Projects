using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Helper;
using DGRAPIs.Models;

namespace DGRAPIs.Repositories
{
    public class DGRRepository : GenericRepository
    {
        private int approve_status = 0;
        public DGRRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {

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
        
        internal async Task<List<WindDashboardData>> GetWindDashboardData(string startDate, string endDate, string FY, string sites)
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

                List<WindDashboardData> _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
                return _WindDashboardData;
            }
          

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

        internal async Task<List<WindDashboardData>> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites,string date)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and t1.date='"+date+"' ";
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
            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site=t1.site and t3.date=t1.date where " + filter + " group by t1.Site,t1.date order by t1.date desc";

            //t3 on t3.site=t1.site and t3.date=t1.date where t1.approve_status="+approve_status+" and " + filter + " group by t1.Site,t1.date order by t1.date desc";

            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }

        internal async Task<List<WindDashboardData>> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and month(t1.date)=" + month + " ";
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
            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site=t1.site and t3.date=t1.date where  " + filter + " group by t1.Site,month(t1.date) order by t1.date desc";

            //t3 on t3.site=t1.site and t3.date=t1.date where t1.approve_status="+approve_status+" and " + filter + " group by t1.Site,month(t1.date) order by t1.date desc";

            List<WindDashboardData> _WindDashboardData = new List<WindDashboardData>();
            _WindDashboardData = await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
            return _WindDashboardData;

        }
        internal async Task<List<WindDashboardData>> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "')  ";
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
            string qry = @"  select  t1.Date,month(t1.date)as month,year(t1.date)as year,t1.Site,  (sum(t1.wind_speed)/count(*))as Wind,
 sum(t1.kwh)as KWH, replace(t2.line_loss,'%','')as line_loss, sum(t1.kwh)-(sum(t1.kwh)* replace(t2.line_loss,'%','') /100) as jmrkwh,
 (t3.kwh*1000000)as tarkwh, avg(t3.wind_speed) as tarwind from  daily_gen_summary t1
 left join monthly_uploading_line_losses t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and fy='" + FY + "' left join daily_target_kpi t3 on t3.site=t1.site and t3.date=t1.date where " + filter + " group by t1.Site,month(t1.date),year(t1.date) order by t1.date desc";

            //t3 on t3.site=t1.site and t3.date=t1.date where t1.approve_status="+approve_status+" and " + filter + " group by t1.Site,month(t1.date),year(t1.date) order by t1.date desc";

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
        internal async Task<List<SolarDashboardData>> GetSolarDashboardData(string startDate, string endDate, string FY, string sites)
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

            //            string qry = @" select t1.date,month(t1.date)as month,t1.site,sum(inv_kwh) as inv_kwh,avg(t1.poa) as IR,
            //replace(t2.lineloss,'%','')as line_loss,sum(inv_kwh)-(sum(inv_kwh) * replace(t2.LineLoss,'%','') /100) as jmrkwh,
            //(t3.gen_nos*1000000) as tarkwh, avg(t3.poa) as tarIR from daily_gen_summary_solar t1 
            //left join monthly_line_loss_solar t2 on t2.site=t1.site and t2.month=DATE_FORMAT(t1.date, '%b') and t2.fy='" + FY + "' left join daily_target_kpi_solar t3 on t3.sites=t1.site and t3.date=t1.date  where " + filter + "  group by t1.Site,t1.date order by t1.date desc ";
            string table = "tblsolardata";
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
            }
           


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

       
        internal async Task<List<SolarDashboardData>> GetSolarDashboardDataByLastDay(string startDate, string endDate, string FY, string sites,string date)
        {

            string filter = "(t1.date >= '" + startDate + "'  and t1.date<= '" + endDate + "') and t1.date='"+date+"' ";
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
        internal async Task<List<WindDailyGenReports>> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string filter = "(t1.date >= '" + fromDate + "'  and t1.date<= '" + toDate + "')";

            string qry = @"SELECT year(date)as year,month(date)as month,date,t2.country,t1.state,t2.spv,t1.site,t2.capacity_mw
,t1.wtg,wind_speed,kwh,plf,ma_actual,ma_contractual,iga,ega,grid_hrs,lull_hrs
,unschedule_hrs,schedule_hrs,others,igbdh,egbdh,load_shedding	 FROM daily_gen_summary t1 left join
site_master t2 on t1.site=t2.site 
where   " + filter;

            //where t1.approve_status="+approve_status+" and " + filter;

            return await Context.GetData<WindDailyGenReports>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports1>> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
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
site_master t2 on t1.site=t2.site 
where " + filter + " group by t1.date, t1.state, t2.spv, t1.site ";

            //where  t1.approve_status="+approve_status+" and " + filter + " group by t1.date, t1.state, t2.spv, t1.site ";

            return await Context.GetData<WindDailyGenReports2>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports1>> GetWindMonthlyYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
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
site_master t2 on t1.site=t2.site 
where   " + filter + " group by t1.state, t2.spv, t1.wtg , month(t1.date)";

            //where t1.approve_status="+approve_status+" and " + filter + " group by t1.state, t2.spv, t1.wtg , month(t1.date)";

            return await Context.GetData<WindDailyGenReports1>(qry).ConfigureAwait(false);

        }

        internal async Task<List<WindDailyGenReports2>> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
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
site_master t2 on t1.site=t2.site 
where   " + filter + " group by t1.state, t2.spv, t1.site , month(t1.date)";

            //where  t1.approve_status="+approve_status+" and " + filter + " group by t1.state, t2.spv, t1.site , month(t1.date)";

            return await Context.GetData<WindDailyGenReports2>(qry).ConfigureAwait(false);

        }

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
site_master t2 on t1.site=t2.site 
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
site_master t2 on t1.site=t2.site 
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

            string qry = @"SELECT date,t1.wtg,bd_type,stop_from,stop_to,total_stop,error_description,action_taken,
t3.country,t3.state,t3.spv, t2.site FROM uploading_file_breakdown t1 left join location_master t2 on t2.wtg=t1.wtg left join site_master t3 on t3.site=t2.site   ";

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
                filter =" where " + filter
;            }
            string qry = @"select distinct wtg from daily_gen_summary " + filter;
            return await Context.GetData<WindDailyGenReports>(qry).ConfigureAwait(false);

        }

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

            //daily_gen_summary t1  left join site_master t2 on t1.site=t2.site where t1.approve_status=" + approve_status + " and (date >= '" + fromDate + "'  and date<= '" + todate + "') group by spv";

            return await Context.GetData<WindPerformanceReports>(qry).ConfigureAwait(false);

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
      
        internal async Task<int> InsertDailyTargetKPI(List<WindDailyTargetKPI> windDailyTargetKPI)
        {

            string delqry = "";
            for (int i = 0; i < windDailyTargetKPI.Count; i++)
            {

                string dates = Convert.ToDateTime(windDailyTargetKPI[i].Date).ToString("yyyy-MM-dd");
                 
                delqry += "delete from daily_target_kpi where fy='" + windDailyTargetKPI[i].FY + "' and date='" + dates + "' and site='" + windDailyTargetKPI[i].Site + "' ;";
            }
              await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windDailyTargetKPI.Count; i++)
            {
                 
                string dates = Convert.ToDateTime(windDailyTargetKPI[i].Date).ToString("yyyy-MM-dd");
                string ma = Convert.ToString(windDailyTargetKPI[i].MA);
                string iga = Convert.ToString(windDailyTargetKPI[i].IGA);
                string ega = Convert.ToString(windDailyTargetKPI[i].EGA);
                string plf = Convert.ToString(windDailyTargetKPI[i].PLF);

                qry += "insert into daily_target_kpi (fy,date,site,wind_speed,kwh,ma,iga,ega,plf) values ('" + windDailyTargetKPI[i].FY + "','" + dates + "','" + windDailyTargetKPI[i].Site + "','" + windDailyTargetKPI[i].WindSpeed + "','" + windDailyTargetKPI[i].kWh + "','" + ma.TrimEnd('%') + "','" + iga.TrimEnd('%')+ "','" + ega.TrimEnd('%') + "','" + plf.TrimEnd('%') + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> windMonthlyTargetKPI)
        {
            string delqry = "";
            for (int i = 0; i < windMonthlyTargetKPI.Count; i++)
            {
                delqry += "delete from monthly_target_kpi where fy='" + windMonthlyTargetKPI[i].FY + "' and month='" + windMonthlyTargetKPI[i].Month + "' and site='" + windMonthlyTargetKPI[i].Site + "' ;";
            }
              await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windMonthlyTargetKPI.Count; i++)
            {
                string MA = Convert.ToString(windMonthlyTargetKPI[i].MA);
                string IGA = Convert.ToString(windMonthlyTargetKPI[i].IGA);
                string EGA = Convert.ToString(windMonthlyTargetKPI[i].EGA);
                string PLF = Convert.ToString(windMonthlyTargetKPI[i].PLF);

                qry += "insert into monthly_target_kpi (fy,month,site,wind_speed,kwh,ma,iga,ega,plf) values ('" + windMonthlyTargetKPI[i].FY + "','" + windMonthlyTargetKPI[i].Month + "','" + windMonthlyTargetKPI[i].Site + "','" + windMonthlyTargetKPI[i].WindSpeed + "','" + windMonthlyTargetKPI[i].kWh + "','" + MA.TrimEnd('%') + "','" + IGA.TrimEnd('%') + "','" + EGA.TrimEnd('%') + "','" + PLF.TrimEnd('%') + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> windMonthlyUploadingLineLosses)
        {
            string delqry = "";
            for (int i = 0; i < windMonthlyUploadingLineLosses.Count; i++)
            {
                delqry += "delete from monthly_uploading_line_losses where fy='" + windMonthlyUploadingLineLosses[i].FY + "' and site='" + windMonthlyUploadingLineLosses[i].Sites + "' and month='" + windMonthlyUploadingLineLosses[i].Month + "' ;";
            }
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windMonthlyUploadingLineLosses.Count; i++)
            {
                qry += "insert into monthly_uploading_line_losses (fy,site,month,line_loss) values ('" + windMonthlyUploadingLineLosses[i].FY + "','" + windMonthlyUploadingLineLosses[i].Sites + "','" + windMonthlyUploadingLineLosses[i].Month + "','" + windMonthlyUploadingLineLosses[i].LineLoss + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
      
        internal async Task<int> InsertWindJMR(List<WindMonthlyJMR> windMonthlyJMR)
        {
            string delqry = "";
            for (int i = 0; i < windMonthlyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(windMonthlyJMR[i].JMR_date).ToString("yyyy-MM-dd");

                delqry += "delete from monthly_jmr where FY='" + windMonthlyJMR[i].FY + "' and Site='" + windMonthlyJMR[i].Site + "' and JMR_date='" + dates + "' and JMR_Month='" + windMonthlyJMR[i].JMR_Month + "' and JMR_Year='" + windMonthlyJMR[i].JMR_Year + "'; ";
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windMonthlyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(windMonthlyJMR[i].JMR_date).ToString("yyyy-MM-dd");

                qry += "insert into monthly_jmr (FY,Site,Plant_Section,Controller_KWH_INV,Scheduled_Units_kWh,Export_kWh,Import_kWh,Net_Export_kWh,Export_kVAh,Import_kVAh,Export_kVArh_lag,Import_kVArh_lag,Export_kVArh_lead,Import_kVArh_lead,JMR_date,JMR_Month,JMR_Year,LineLoss,Line_Loss_percentage,RKVH_percentage) values ('" + windMonthlyJMR[i].FY + "','" + windMonthlyJMR[i].Site + "','" + windMonthlyJMR[i].Plant_Section + "','" + windMonthlyJMR[i].Controller_KWH_INV + "','" + windMonthlyJMR[i].Scheduled_Units_kWh + "','" + windMonthlyJMR[i].Export_kWh + "','" + windMonthlyJMR[i].Import_kWh + "','" + windMonthlyJMR[i].Net_Export_kWh + "','" + windMonthlyJMR[i].Export_kVAh + "','" + windMonthlyJMR[i].Import_kVAh + "','" + windMonthlyJMR[i].Export_kVArh_lag + "','" + windMonthlyJMR[i].Import_kVArh_lag + "','" + windMonthlyJMR[i].Export_kVArh_lead + "','" + windMonthlyJMR[i].Import_kVArh_lead + "','" + dates + "','" + windMonthlyJMR[i].JMR_Month + "','" + windMonthlyJMR[i].JMR_Year + "','" + windMonthlyJMR[i].LineLoss + "','" + windMonthlyJMR[i].Line_Loss_percentage + "','" + windMonthlyJMR[i].RKVH_percentage + "'); ";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> windDailyLoadShedding)
        {
            string delqry = "";
            for (int i = 0; i < windDailyLoadShedding.Count; i++)
            {
                string dates = Convert.ToDateTime(windDailyLoadShedding[i].Date).ToString("yyyy-MM-dd");

                delqry += "delete from  daily_load_shedding where Site='" + windDailyLoadShedding[i].Site + "' and Date='" + dates + "' ;";
            }
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windDailyLoadShedding.Count; i++)
            {
                string dates = Convert.ToDateTime(windDailyLoadShedding[i].Date).ToString("yyyy-MM-dd");

                qry += "insert into daily_load_shedding (Site,Date,Start_Time,End_Time,Total_Time,Permissible_Load_MW,Gen_loss_kWh) values ('" + windDailyLoadShedding[i].Site + "','" + dates + "','" + windDailyLoadShedding[i].Start_Time + "','" + windDailyLoadShedding[i].End_Time + "','" + windDailyLoadShedding[i].Total_Time + "','" + windDailyLoadShedding[i].Permissible_Load_MW + "','" + windDailyLoadShedding[i].Gen_loss_kWh + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertDailyJMR(List<WindDailyJMR> windDailyJMR)
        {
            string delqry = "";
            for (int i = 0; i < windDailyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(windDailyJMR[i].Date).ToString("yyyy-MM-dd");
                delqry += "delete from daily_jmr where date='" + dates + "' and site='" + windDailyJMR[i].Site + "' ;";
            }
              await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < windDailyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(windDailyJMR[i].Date).ToString("yyyy-MM-dd");
                qry += "insert into daily_jmr (date,site,jmr_kwh) values ('" + dates + "','" + windDailyJMR[i].Site + "','" + windDailyJMR[i].JMR_KWH + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
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

                qry += "insert into daily_target_kpi_solar (fy,date,sites,ghi,poa,gen_nos,ma,iga,ega,pr,plf) values ('" + solarDailyTargetKPI[i].FY + "','" + dates + "','" + solarDailyTargetKPI[i].Sites + "','" + solarDailyTargetKPI[i].GHI + "','" + solarDailyTargetKPI[i].POA + "','" + solarDailyTargetKPI[i].GenNosMU + "','" + ma.TrimEnd('%') + "','" + iga.TrimEnd('%') + "','" + ega.TrimEnd('%') + "','" + pr.TrimEnd('%') + "','" + plf.TrimEnd('%') + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<int> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> solarMonthlyTargetKPI)
        {
            string delqry = "";
            
            for (int i = 0; i < solarMonthlyTargetKPI.Count; i++)
            {
                 

                delqry += "delete from monthly_target_kpi_solar where fy='" + solarMonthlyTargetKPI[i].FY + "'  and month='" + solarMonthlyTargetKPI[i].Month + "' and sites='" + solarMonthlyTargetKPI[i].Sites + "' ;";
                 
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            int recordcount = 0;
            for (int i = 0; i < solarMonthlyTargetKPI.Count; i++)
            {
                recordcount++;
                string ma = Convert.ToString(solarMonthlyTargetKPI[i].MA);
                string iga = Convert.ToString(solarMonthlyTargetKPI[i].IGA);
                string ega = Convert.ToString(solarMonthlyTargetKPI[i].EGA);
                string pr = Convert.ToString(solarMonthlyTargetKPI[i].PR);
                string plf = Convert.ToString(solarMonthlyTargetKPI[i].PLF);

                qry += "insert into monthly_target_kpi_solar (fy,month,sites,ghi,poa,gen_nos,ma,iga,ega,pr,plf) values ('" + solarMonthlyTargetKPI[i].FY + "','" + solarMonthlyTargetKPI[i].Month + "','" + solarMonthlyTargetKPI[i].Sites + "','" + solarMonthlyTargetKPI[i].GHI + "','" + solarMonthlyTargetKPI[i].POA + "','" + solarMonthlyTargetKPI[i].GenNosMU + "','" + ma.TrimEnd('%') + "','" + iga.TrimEnd('%') + "','" + ega.TrimEnd('%') + "','" + pr.TrimEnd('%') + "','" + plf.TrimEnd('%') + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> solarMonthlyUploadingLineLosses)
        {
            string delqry = "";
            for (int i = 0; i < solarMonthlyUploadingLineLosses.Count; i++)
            {
                delqry += "delete from monthly_line_loss_solar where fy='" + solarMonthlyUploadingLineLosses[i].FY + "' and site='" + solarMonthlyUploadingLineLosses[i].Sites + "' and month='" + solarMonthlyUploadingLineLosses[i].Month + "' ;";
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < solarMonthlyUploadingLineLosses.Count; i++)
            {
                qry += "insert into monthly_line_loss_solar (fy,site,month,lineloss) values ('" + solarMonthlyUploadingLineLosses[i].FY + "','" + solarMonthlyUploadingLineLosses[i].Sites + "','" + solarMonthlyUploadingLineLosses[i].Month + "','" + solarMonthlyUploadingLineLosses[i].LineLoss + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarJMR(List<SolarMonthlyJMR> solarMonthlyJMR)
        {
            string delqry = "";
            for (int i = 0; i < solarMonthlyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(solarMonthlyJMR[i].JMR_date).ToString("yyyy-MM-dd");

                delqry += "delete from monthly_jmr_solar where FY='" + solarMonthlyJMR[i].FY + "' and Site='" + solarMonthlyJMR[i].Site + "' and JMR_date='" + dates + "' and JMR_Month='" + solarMonthlyJMR[i].JMR_Month + "' and JMR_Year='" + solarMonthlyJMR[i].JMR_Year + "' ; ";
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < solarMonthlyJMR.Count; i++)
            {
                string dates = Convert.ToDateTime(solarMonthlyJMR[i].JMR_date).ToString("yyyy-MM-dd");

                qry += "insert into monthly_jmr_solar (FY,Site,Plant_Section,Controller_KWH_INV,Scheduled_Units_kWh,Export_kWh,Import_kWh,Net_Export_kWh,Export_kVAh,Import_kVAh,Export_kVArh_lag,Import_kVArh_lag,Export_kVArh_lead,Import_kVArh_lead,JMR_date,JMR_Month,JMR_Year,LineLoss,Line_Loss_percentage,RKVH_percentage) values ('" + solarMonthlyJMR[i].FY + "','" + solarMonthlyJMR[i].Site + "','" + solarMonthlyJMR[i].Plant_Section + "','" + solarMonthlyJMR[i].Controller_KWH_INV + "','" + solarMonthlyJMR[i].Scheduled_Units_kWh + "','" + solarMonthlyJMR[i].Export_kWh + "','" + solarMonthlyJMR[i].Import_kWh + "','" + solarMonthlyJMR[i].Net_Export_kWh + "','" + solarMonthlyJMR[i].Export_kVAh + "','" + solarMonthlyJMR[i].Import_kVAh + "','" + solarMonthlyJMR[i].Export_kVArh_lag + "','" + solarMonthlyJMR[i].Import_kVArh_lag + "','" + solarMonthlyJMR[i].Export_kVArh_lead + "','" + solarMonthlyJMR[i].Import_kVArh_lead + "','" + dates + "','" + solarMonthlyJMR[i].JMR_Month + "','" + solarMonthlyJMR[i].JMR_Year + "','" + solarMonthlyJMR[i].LineLoss + "','" + solarMonthlyJMR[i].Line_Loss_percentage + "','" + solarMonthlyJMR[i].RKVH_percentage + "'); ";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> solarDailyLoadShedding)
        {
            string delqry = "";
            for (int i = 0; i < solarDailyLoadShedding.Count; i++)
            {
                string dates = Convert.ToDateTime(solarDailyLoadShedding[i].Date).ToString("yyyy-MM-dd");

                delqry += "delete from daily_load_shedding_solar where Site='"+ solarDailyLoadShedding[i].Site + "' and Date='"+ dates + "' ;";
            }
             await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);


            string qry = "";
            for (int i = 0; i < solarDailyLoadShedding.Count; i++)
            {
                string dates = Convert.ToDateTime(solarDailyLoadShedding[i].Date).ToString("yyyy-MM-dd");

                qry += "insert into daily_load_shedding_solar (Site,Date,Start_Time,End_Time,Total_Time,Permissible_Load_MW,Gen_loss_kWh) values ('" + solarDailyLoadShedding[i].Site + "','" + dates + "','" + solarDailyLoadShedding[i].Start_Time + "','" + solarDailyLoadShedding[i].End_Time + "','" + solarDailyLoadShedding[i].Total_Time + "','" + solarDailyLoadShedding[i].Permissible_Load_MW + "','" + solarDailyLoadShedding[i].Gen_loss_kWh + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> solarInvAcDcCapacity)
        {
            string delqry = "";
            for (int i = 0; i < solarInvAcDcCapacity.Count; i++)
            {


                delqry += "delete from solar_ac_dc_capacity where site='"+ solarInvAcDcCapacity[i].site + "' and inverter='"+ solarInvAcDcCapacity[i].inverter + "' ; ";
            }
            await Context.ExecuteNonQry<int>(delqry).ConfigureAwait(false);

            string qry = "";
            for (int i = 0; i < solarInvAcDcCapacity.Count; i++)
            {


                qry += "insert into solar_ac_dc_capacity (site,inverter,dc_capacity,ac_capacity) values ('" + solarInvAcDcCapacity[i].site + "','" + solarInvAcDcCapacity[i].inverter + "','" + solarInvAcDcCapacity[i].dc_capacity + "','" + solarInvAcDcCapacity[i].ac_capacity + "');";
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

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

        internal async Task<int> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> solarUploadingPyranoMeter1Min)
        {

            string qry = "";
            int recordcount = 0;
            for (int i = 0; i < solarUploadingPyranoMeter1Min.Count; i++)
            {
                recordcount++;
                string Time_stamp = Convert.ToDateTime(solarUploadingPyranoMeter1Min[i].Time_stamp).ToString("yyyy-MM-dd HH:mm:ss");



                string GHI_1 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].GHI_1) ? "0" : solarUploadingPyranoMeter1Min[i].GHI_1;
                string GHI_2 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].GHI_2) ? "0" : solarUploadingPyranoMeter1Min[i].GHI_2;
                string POA_1 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_1) ? "0" : solarUploadingPyranoMeter1Min[i].POA_1;
                string POA_2 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_2) ? "0" : solarUploadingPyranoMeter1Min[i].POA_2;
                string POA_3 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_3) ? "0" : solarUploadingPyranoMeter1Min[i].POA_3;
                string POA_4 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_4) ? "0" : solarUploadingPyranoMeter1Min[i].POA_4;
                string POA_5 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_5) ? "0" : solarUploadingPyranoMeter1Min[i].POA_5;
                string POA_6 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_6) ? "0" : solarUploadingPyranoMeter1Min[i].POA_6;
                string POA_7 = string.IsNullOrEmpty(solarUploadingPyranoMeter1Min[i].POA_7) ? "0" : solarUploadingPyranoMeter1Min[i].POA_7;

                qry += "insert into uploading_pyranometer_one_min (Time_stamp,ghi_1,ghi_2,poa_1,poa_2,poa_3,poa_4,poa_5,poa_6,poa_7,average_ghi,average_poa) values ('" + Time_stamp + "','" + GHI_1 + "','" + GHI_2 + "','" + POA_1 + "','" + POA_2 + "','" + POA_3 + "','" + POA_4 + "','" + POA_5 + "','" + POA_6 + "','" + POA_7 + "','" + solarUploadingPyranoMeter1Min[i].Average_GHI + "','" + solarUploadingPyranoMeter1Min[i].Average_POA + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        internal async Task<int> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> solarUploadingPyranoMeter15Min)
        {

            string qry = "";
            int recordcount = 0;
            for (int i = 0; i < solarUploadingPyranoMeter15Min.Count; i++)
            {
                recordcount++;
                string Time_stamp = Convert.ToDateTime(solarUploadingPyranoMeter15Min[i].Time_stamp).ToString("yyyy-MM-dd HH:mm:ss");



                string GHI_1 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].GHI_1) ? "0" : solarUploadingPyranoMeter15Min[i].GHI_1;
                string GHI_2 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].GHI_2) ? "0" : solarUploadingPyranoMeter15Min[i].GHI_2;
                string POA_1 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_1) ? "0" : solarUploadingPyranoMeter15Min[i].POA_1;
                string POA_2 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_2) ? "0" : solarUploadingPyranoMeter15Min[i].POA_2;
                string POA_3 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_3) ? "0" : solarUploadingPyranoMeter15Min[i].POA_3;
                string POA_4 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_4) ? "0" : solarUploadingPyranoMeter15Min[i].POA_4;
                string POA_5 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_5) ? "0" : solarUploadingPyranoMeter15Min[i].POA_5;
                string POA_6 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_6) ? "0" : solarUploadingPyranoMeter15Min[i].POA_6;
                string POA_7 = string.IsNullOrEmpty(solarUploadingPyranoMeter15Min[i].POA_7) ? "0" : solarUploadingPyranoMeter15Min[i].POA_7;

                qry += "insert into uploading_pyranometer_fifteen_min (Time_stamp,ghi_1,ghi_2,poa_1,poa_2,poa_3,poa_4,poa_5,poa_6,poa_7,average_ghi,average_poa,ambient_temp,module_temp) values ('" + Time_stamp + "','" + GHI_1 + "','" + GHI_2 + "','" + POA_1 + "','" + POA_2 + "','" + POA_3 + "','" + POA_4 + "','" + POA_5 + "','" + POA_6 + "','" + POA_7 + "','" + solarUploadingPyranoMeter15Min[i].Average_GHI + "','" + solarUploadingPyranoMeter15Min[i].Average_POA + "','" + solarUploadingPyranoMeter15Min[i].Ambient_Temp + "','" + solarUploadingPyranoMeter15Min[i].Module_Temp + "');";
                if (recordcount == 100)
                {

                    await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                    qry = "";
                    recordcount = 0;
                }
            }
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }
        internal async Task<bool> InsertSolarUploadingFilegeneration(List<SolarUploadingFilegeneration> listSolarUploadingFilegeneration)
        {
            bool response = false;
            string qry = "";
       
            foreach (var solarUploadingFilegeneration in listSolarUploadingFilegeneration)
            {
                qry += "insert into uploading_file_generation_solar (date,site,inverter,inv_act,plant_act,pi) values ('" + solarUploadingFilegeneration.date + "','" + solarUploadingFilegeneration.site + "','" + solarUploadingFilegeneration.inverter + "','" + solarUploadingFilegeneration.inv_act + "','" + solarUploadingFilegeneration.plant_act + "','" + solarUploadingFilegeneration.pi + "');";

                await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                response = true;
            }
            return  response;

        }
        internal async Task<bool> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> listSolarUploadingFileBreakDown)
        {
            bool response = false;
            string qry = "";

            foreach (var solarUploadingFileBreakDown in listSolarUploadingFileBreakDown)
            {
                qry += "insert into uploading_file_generation_solar (date,site,ext_int_bd,icr,inv,smb,strings,from_bd,to_bd,bd_remarks,bd_type,action_taken) values ('" + solarUploadingFileBreakDown.date + "','" + solarUploadingFileBreakDown.site + "','" + solarUploadingFileBreakDown.ext_int_bd + "','" + solarUploadingFileBreakDown.icr + "','" + solarUploadingFileBreakDown.inv + "','" + solarUploadingFileBreakDown.smb + "','" + solarUploadingFileBreakDown.strings + "','" + solarUploadingFileBreakDown.from_bd + "','" + solarUploadingFileBreakDown.to_bd + "','" + solarUploadingFileBreakDown.bd_remarks + "','" + solarUploadingFileBreakDown.bd_type + "','" + solarUploadingFileBreakDown.action_taken + "')";

                await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);
                response = true;
            }
            return response;

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



      

        #region views


        internal async Task<List<DailyGenSummary>> GetWindDailyGenSummary(string fromDate, string ToDate)
        {

            string filter = " where   date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            //string filter = " where approve_status=" + approve_status + " and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            return await Context.GetData<DailyGenSummary>("Select * from daily_gen_summary " + filter).ConfigureAwait(false);

        }
        internal async Task<List<SolarDailyGenSummary>> GetSolarDailyGenSummary(string fromDate, string ToDate)
        {

            string filter = " where   date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            //  string filter = " where approve_status=" + approve_status + " and date >= '" + fromDate + "' and date <= '" + ToDate + "' ";
            return await Context.GetData<SolarDailyGenSummary>("Select * from daily_gen_summary_solar " + filter).ConfigureAwait(false);

        }
        internal async Task<List<WindDailyTargetKPI>> GetWindDailyTargetKPI(string fromDate, string todate)
        {

            string datefilter = " (date >= '" + fromDate + "'  and date<= '" + todate + "') ";

            string qry = @"SELECT fy,date,site,wind_speed as WindSpeed,kwh,ma,iga,ega,plf FROM daily_target_kpi where " + datefilter;

            return await Context.GetData<WindDailyTargetKPI>(qry).ConfigureAwait(false);

        }
        internal async Task<List<SolarDailyTargetKPI>> GetSolarDailyTargetKPI(string fromDate, string todate)
        {

            string datefilter = " (date >= '" + fromDate + "'  and date<= '" + todate + "') ";

            string qry = @"SELECT fy,date,Sites,ghi,poa,gen_nos as GenNosMU,ma,iga,ega,pr,plf FROM daily_target_kpi_solar where " + datefilter;

            return await Context.GetData<SolarDailyTargetKPI>(qry).ConfigureAwait(false);

        }
        internal async Task<List<WindMonthlyTargetKPI>> GetWindMonthlyTargetKPI(string fy, string month)
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
                    filter += " and month in(" + monthnames + ")";
                }

            }

            string qry = @"SELECT fy,month,site,wind_speed as WindSpeed,kwh,ma,iga,ega,plf FROM monthly_target_kpi where " + filter;

            return await Context.GetData<WindMonthlyTargetKPI>(qry).ConfigureAwait(false);

        }
        internal async Task<List<SolarMonthlyTargetKPI>> GetSolarMonthlyTargetKPI(string fy, string month)
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
                    filter += " and month in(" + monthnames + ")";
                }

            }


            string qry = @"SELECT  fy,month,Sites,ghi,poa,gen_nos as GenNosMU,ma,iga,ega,pr,plf FROM monthly_target_kpi_solar where " + filter;

            return await Context.GetData<SolarMonthlyTargetKPI>(qry).ConfigureAwait(false);

        }
        internal async Task<List<WindMonthlyUploadingLineLosses>> GetWindMonthlyLineLoss(string fy, string month)
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
                    filter += " and month in(" + monthnames + ")";
                }

            }


            string qry = @"SELECT  fy,month,site as Sites,line_loss as LineLoss FROM monthly_uploading_line_losses where " + filter;

            return await Context.GetData<WindMonthlyUploadingLineLosses>(qry).ConfigureAwait(false);

            

        }
        internal async Task<List<SolarMonthlyUploadingLineLosses>> GetSolarMonthlyLineLoss(string fy, string month)
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
                    filter += " and month in(" + monthnames + ")";
                }

            }


            string qry = @"SELECT  fy,month,site as Sites,LineLoss FROM monthly_line_loss_solar where " + filter;

            return await Context.GetData<SolarMonthlyUploadingLineLosses>(qry).ConfigureAwait(false);



        }

        internal async Task<List<WindMonthlyJMR>> GetWindMonthlyJMR(string fy, string month)
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
                    filter += " and jmr_month in(" + monthnames + ")";
                }

            }


            string qry = @"SELECT  fy,site,Plant_Section,Controller_KWH_INV,Scheduled_Units_kWh,Export_kWh,Import_kWh,Net_Export_kWh,Export_kVAh,Import_kVAh,Export_kVArh_lag,Import_kVArh_lag,Export_kVArh_lead,Import_kVArh_lead,JMR_date,JMR_Month,JMR_Year,LineLoss,Line_Loss_percentage,RKVH_percentage FROM monthly_jmr where " + filter;

            return await Context.GetData<WindMonthlyJMR>(qry).ConfigureAwait(false);
        }
        internal async Task<List<SolarMonthlyJMR>> GetSolarMonthlyJMR(string fy, string month)
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
                    filter += " and jmr_month in(" + monthnames + ")";
                }

            }

            string qry = @"SELECT  fy,site,Plant_Section,Controller_KWH_INV,Scheduled_Units_kWh,Export_kWh,Import_kWh,Net_Export_kWh,Export_kVAh,Import_kVAh,Export_kVArh_lag,Import_kVArh_lag,Export_kVArh_lead,Import_kVArh_lead,JMR_date,JMR_Month,JMR_Year,LineLoss,Line_Loss_percentage,RKVH_percentage FROM monthly_jmr_solar where " + filter;

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
                    filter += " where site in(" + sitesnames + ")";
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
        internal async Task<List<SolarBDType>> GetSolarBDType()
        {
            string qry = @"SELECT * FROM bd_type " ;

            return await Context.GetData<SolarBDType>(qry).ConfigureAwait(false);
        }
        internal async Task<List<WindDailyLoadShedding>> GetWindDailyloadShedding(string site,string fromDate,string toDate)
        {

            string datefilter = " where site='" + site + "' ";
            if (site == "All")
            {
                datefilter = " where (date >= '" + fromDate + "'  and date<= '" + toDate + "') ";
            }
            else 
            {
                datefilter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "') ";
            }
            string qry = @"SELECT * FROM daily_load_shedding " + datefilter;

            return await Context.GetData<WindDailyLoadShedding>(qry).ConfigureAwait(false);
        }
        internal async Task<List<SolarDailyLoadShedding>> GetSolarDailyloadShedding(string site, string fromDate, string toDate)
        {

            string datefilter = " where site='" + site + "' ";
            if (site == "All")
            {
                datefilter = " where (date >= '" + fromDate + "'  and date<= '" + toDate + "') ";
            }
            else
            {
                datefilter += " and (date >= '" + fromDate + "'  and date<= '" + toDate + "') ";
            }
            string qry = @"SELECT * FROM daily_load_shedding_solar " + datefilter;

            return await Context.GetData<SolarDailyLoadShedding>(qry).ConfigureAwait(false);
        }

        internal async Task<List<DailyGenSummary>> GetWindDailyGenSummaryPending(string date,string site)
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
        #endregion

        //  await Context.GetData<WindDashboardData>(qry).ConfigureAwait(false);
        internal async Task<int> eQry(string qry)
        {
            return await Context.ExecuteNonQry<int>(qry).ConfigureAwait(false);

        }

        public async Task<List<approvalObject>> GetBatches (string importFromDate, string importToDate, int siteId, string status)
        {
            //Get_Batches is a part of the approval module which takes the foll fields from the UI: Date range, Site-Id and Status-type
            
            //Following establishes a database connection:
           // string connString = "SERVER = localhost ; DATABASE = hfe ; UID = root ; PASSWORD = ;";
         
            string query = "select * from import_batches where import_date>=" + importFromDate + " and import_date>=" + importToDate + " and site_id ="+siteId+" and status ="+status+";";
            List<approvalObject> _approvalObject = new List<approvalObject>();
            _approvalObject = await Context.GetData<approvalObject>(query).ConfigureAwait(false);
            return _approvalObject;

            //            MySqlConnection conn = new MySqlConnection(connString);
            //            conn.Open();
            //            MySqlCommand cmd = new MySqlCommand(query, conn);

            //            //Here the resulting table data retrieved from the inserted query gets read and fed into a newly created list object using the following while loop                          
            //            List<approvalObject> tableSet = new List<approvalObject>();
            //            using (var reader = cmd.ExecuteReader())
            //            {
            //                approvalObject tableRows = new approvalObject();
            //                while (reader.Read())
            //                {
            //                    tableRows.column1= (int)reader["import_log_id"];
            //                    tableRows.column2 = (string)(reader["file_name"]);
            //                    tableRows.column3 = (string)(reader["import_type"]);
            //                    tableRows.column4= (int)(reader["site_id"]);
            //                    tableRows.column5 = (DateTime)(reader["import_date"]);
            //                    tableRows.column6 = (int)(reader["imported_by"]);
            //                    tableRows.column7 = (DateTime)(reader["approval_date"]);
            //                    tableRows.column8 = (int)(reader["approved_by"]);
            //                    tableRows.column9 = (string)(reader["is_approved"]);
            //                    tableRows.column10 = (string)(reader["log_filename"]);
            //                    tableSet.Add(tableRows);
            //                }
            //            }

            //            //Here the list object holding the table data gets converted to a JSON string to be returned to the calling function for Get_atches
            //            string json = JsonConvert.SerializeObject(tableSet);
            ////            return public async Task<List< approvalObject >> ;
        }
        public async Task<List<WindSiteMaster>> GetSiteData(string state, string spv)
        {
            string filter = "";
            if (!string.IsNullOrEmpty(state) && string.IsNullOrEmpty(spv))
            {
                filter += " where state='" + state + "'";
            }
            if (!string.IsNullOrEmpty(spv) && !string.IsNullOrEmpty(state))
            {
                filter += " where state='" + state + "' and spv='" + spv + "'";
            }


            string query = "SELECT * FROM `site_master`" + filter;
            List<WindSiteMaster> _sitelist = new List<WindSiteMaster>();
            _sitelist = await Context.GetData<WindSiteMaster>(query).ConfigureAwait(false);
            return _sitelist;

        }

    }

}
