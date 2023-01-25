using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Controllers;
using DGRAPIs.Repositories;
using DGRAPIs.Helper;
using DGRAPIs.Models;

namespace DGRAPIs.Controllers
{
    //private MYSQLDBHelper getDB => databaseProvider.SqlInstance();

    internal class databaseProvider
    {
        internal static MYSQLDBHelper SqlInstance()
        {
            throw new NotImplementedException();
        }
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly MYSQLDBHelper getDB;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            Calculation objCal = new Calculation(getDB);
           // using (var cal = new Calculation(getDB))
           // {
               // var data = objCal.DailyKPICalculation_Wind();
            //}

            
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
            
           //delete objCal;
        }


       
    }
}
