using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using DGRAPIs.Repositories;
using DGRAPIs.Helper;
using DGRAPIs.Models;
using System.Collections.Generic;

namespace ServiceWorkerCronJobDemo.Services
{
    public class MyCronJob1 : CronJobService
    {
        private readonly ILogger<MyCronJob1> _logger;

        public MyCronJob1(IScheduleConfig<MyCronJob1> config, ILogger<MyCronJob1> logger)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _logger = logger;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("CronJob 1 starts.");
            MYSQLDBHelper db = new MYSQLDBHelper("temp");
            var repo = new DGRRepository(db);

            //FY LOGIC
            DateTime datetimenow = DateTime.Now;
            DateTime oneWeekAgo = datetimenow.Date.AddDays(-7);
            string fy = "";

            if (datetimenow.Month > 3)
            {
                if (oneWeekAgo.Month < 4)
                    oneWeekAgo = new DateTime(oneWeekAgo.Year,04, 01);

                fy = datetimenow.Year.ToString() + "-" + datetimenow.AddYears(1).Year.ToString().Substring(2, 2);
            }
            else
            {
                fy = datetimenow.AddYears(-1).Year.ToString() + "-" + datetimenow.Year.ToString().Substring(2, 2);
            }


            //repo.PPTCreate("2022-23", "3/5/2022", "6/8/2022");
            repo.PPTCreate(fy, oneWeekAgo.ToString("dd/MM/yyyy"), datetimenow.ToString("dd/MM/yyyy"),"Wind");
            repo.PPTCreate(fy, oneWeekAgo.ToString("dd/MM/yyyy"), datetimenow.ToString("dd/MM/yyyy"), "Solar");
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("{now} CronJob 1 is working.", DateTime.Now.ToString("T"));
            //_logger.LogInformation("CronJob 1 starts.");
            MYSQLDBHelper db = new MYSQLDBHelper("temp");
            var repo = new DGRRepository(db);

            //FY LOGIC
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


            //repo.PPTCreate("2022-23", "3/5/2022", "6/8/2022");
            repo.PPTCreate(fy, oneWeekAgo.ToString("dd/MM/yyyy"), datetimenow.ToString("dd/MM/yyyy"), "Wind");
            repo.PPTCreate(fy, oneWeekAgo.ToString("dd/MM/yyyy"), datetimenow.ToString("dd/MM/yyyy"), "Solar");
            return Task.CompletedTask;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation("CronJob 1 is stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}