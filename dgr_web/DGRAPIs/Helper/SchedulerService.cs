using DGRAPIs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using DGRAPIs.Repositories;

namespace DGRAPIs.Helper
{
    public class SchedulerService : IHostedService, IDisposable
    {
        private int executionCount = 0;

        private System.Threading.Timer _timerNotification;
        public IConfiguration _iconfiguration;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _env;

        public SchedulerService(IServiceScopeFactory serviceScopeFactory, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, IConfiguration iconfiguration)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _env = env;
            _iconfiguration = iconfiguration;
        }

        //public Task StartAsync(CancellationToken stoppingToken)
        //{
        //    _timerNotification = new Timer(RunJob, null, TimeSpan.Zero,
        //      TimeSpan.FromMinutes(3)); /*Set Interval time here*/
        //    API_ErrorLog("Scheduler started at :- " + DateTime.Now);
        //    return Task.CompletedTask;
        //}
        public Task StartAsync(CancellationToken stoppingToken)
        {
            // Calculate the time until the next 7:30 PM
            DateTime now = DateTime.Now;
            DateTime next730PM = now.Date.AddDays(1).AddHours(19).AddMinutes(30);
            if (now > next730PM)
            {
                next730PM = next730PM.AddDays(1);
            }
            TimeSpan initialDelay = next730PM - now;

            // Set up the timer with a 24-hour interval
            _timerNotification = new Timer(RunJob, null, initialDelay, TimeSpan.FromHours(24));

            return Task.CompletedTask;
        }


        private void RunJob(object state)
        {

            using (var scrope = _serviceScopeFactory.CreateScope())
            {
                try
                {
                    //  var store = scrope.ServiceProvider.GetService<IStoreRepo>(); /* You can access any interface or service like this here*/
                    //store.GetAll(); /* You can access any interface or service method like this here*/

                    /*
                     Place your code here which you want to schedule on regular intervals
                     */

                    string msg = "Sechduler run at : " + DateTime.Now;
                    API_ErrorLog(msg);
                    
                    MYSQLDBHelper db = new MYSQLDBHelper("temp");
                    var repo = new DGRRepository(db);
                    //repo.MailSend("Calling this function  repo.EmailSolarReport(fy, '2023-03-01'   at " + DateTime.Now + "", " Test mail sechduler");



                    DateTime datetimenow = DateTime.Now;
                    DateTime oneWeekAgo = datetimenow.Date.AddDays(-7);
                    string fy = "";

                    if (datetimenow.Month > 3)
                    {
                        if (oneWeekAgo.Month < 4)
                            oneWeekAgo = new DateTime(oneWeekAgo.Year, 04, 01);

                        fy = datetimenow.Year.ToString() + "-" + datetimenow.AddYears(1).Year.ToString().Substring(2, 2);
                    }
                    else
                    {
                        fy = datetimenow.AddYears(-1).Year.ToString() + "-" + datetimenow.Year.ToString().Substring(2, 2);
                    }


                    repo.EmailSolarReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");
                    repo.EmailWindReport(fy, datetimenow.ToString("yyyy-MM-dd"), "");

                    repo.PPTCreate(fy, datetimenow.ToString("yyyy-MM-dd"), datetimenow.ToString("yyyy-MM-dd"), "");
                    repo.PPTCreate_Solar(fy, datetimenow.ToString("yyyy-MM-dd"), datetimenow.ToString("yyyy-MM-dd"), "");

                    //repo.EmailSolarReport(fy, "2022-12-31", "");
                    //API_ErrorLog("Scheduler method EmailWindReport calling at :- " + DateTime.Now);
                    //repo.EmailWindReport(fy, "2023-02-27", "");
                    //API_ErrorLog("Scheduler mail sent :- " + DateTime.Now);
                }


                catch (Exception ex)
                {

                }
                Interlocked.Increment(ref executionCount);
            }
        }


        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timerNotification?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timerNotification?.Dispose();
        }

        private void API_ErrorLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Error**:" + Message + "\r\n");
        }
        private void API_InformationLog(string Message)
        {
            //Read variable from appsetting to enable disable log
            System.IO.File.AppendAllText(@"C:\LogFile\api_Log.txt", "**Info**:" + Message + "\r\n");
        }
    }
}
