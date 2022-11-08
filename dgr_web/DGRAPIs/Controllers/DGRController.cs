using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DGRController : ControllerBase
    {
        private readonly IDGRBS _dgrBs;
        public DGRController(IDGRBS dgr)
        {
            _dgrBs = dgr;
        }
        [Route("GetWindDailyGenSummary")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummary(string fromDate, string ToDate)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummary(fromDate, ToDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindSiteMaster")]
        [HttpGet]
        public async Task<IActionResult> GetWindSiteMaster()
        {
            try
            {
                var data = await _dgrBs.GetWindSiteMaster();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindLocationMaster")]
        [HttpGet]
        public async Task<IActionResult> GetWindLocationMaster()
        {
            try
            {
                var data = await _dgrBs.GetWindLocationMaster();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarSiteMaster")]
        [HttpGet]
        public async Task<IActionResult> GetSolarSiteMaster()
        {
            try
            {
                var data = await _dgrBs.GetSolarSiteMaster();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarLocationMaster")]
        [HttpGet]
        public async Task<IActionResult> GetSolarLocationMaster()
        {
            try
            {
                var data = await _dgrBs.GetSolarLocationMaster();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarLocationMasterBySite/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarLocationMasterBySite(string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarLocationMasterBySite(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        // [Route("GetWindDashboardData")]
        [Route("GetWindDashboardData/{startDate}/{endDate}/{FY}/{sites}")]
        //  [HttpGet("{filter}/{FY}")]
        // [HttpGet("startDate={startDate}&endDate={endDate}&FY={FY}")]
        //[Route("GetVolunteersByEmpId/{emdId}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardData(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardData(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        //[Route("GetWindDashboardDataCache/{startDate}/{endDate}/{FY}/{sites}")]
        //[HttpGet]
        //public async IActionResult GetWindDashboardDataCache(string startDate, string endDate, string FY, string sites)
        //{
        //    if (!_cache.TryGetValue(CacheKeys.Employees, out List<Employee> employees))
        //    {
        //        employees = GetEmployeesDeatilsFromDB(); // Get the data from database
        //        var cacheEntryOptions = new MemoryCacheEntryOptions
        //        {
        //            AbsoluteExpiration = DateTime.Now.AddMinutes(5),
        //            SlidingExpiration = TimeSpan.FromMinutes(2),
        //            Size = 1024,
        //        };
        //        _cache.Set(CacheKeys.Employees, employees, cacheEntryOptions);
        //    }
        //    return Ok(employees);
        //}


        [Route("GetWindDashboardDataByLastDay/{startDate}/{endDate}/{FY}/{sites}/{date}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites,string date)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByLastDay(startDate, endDate, FY, sites, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindDashboardDataByCurrentMonth/{startDate}/{endDate}/{FY}/{sites}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindDashboardDataByYearly/{startDate}/{endDate}/{FY}/{sites}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetWindDashboardDataByYearly(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarDashboardData/{startDate}/{endDate}/{FY}/{sites}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardData(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardData(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDashboardDataByLastDay/{startDate}/{endDate}/{FY}/{sites}/{date}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByLastDay(startDate, endDate, FY, sites, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarDashboardDataByCurrentMonth/{startDate}/{endDate}/{FY}/{sites}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByCurrentMonth(startDate, endDate, FY, sites, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarDashboardDataByYearly/{startDate}/{endDate}/{FY}/{sites}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            try
            {
                var data = await _dgrBs.GetSolarDashboardDataByYearly(startDate, endDate, FY, sites);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyGenerationReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenerationReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenerationReport(fromDate, toDate, country, state, spv, site, wtg);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyBreakdownReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyBreakdownReport(fromDate, toDate, country, state, spv, site, wtg);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarDailyGenSummary")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummary()
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummary();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetWindMonthlyYearlyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindMonthlyYearlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetWindYearlyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindYearlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{wtg}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, wtg, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetWindWtgFromdailyGenSummary/{state}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindWtgFromdailyGenSummary(string state, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindWtgFromdailyGenSummary(state, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindPerformanceReportSiteWise/{fy}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceReportSiteWise(string fy, string fromDate, string todate)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceReportSiteWise(fy, fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindPerformanceReportBySPVWise/{fy}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetWindPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {
            try
            {
                var data = await _dgrBs.GetWindPerformanceReportBySPVWise(fy, fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarDailyBreakdownReport/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyBreakdownReport(fromDate, toDate, country, state, spv, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertDailyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertDailyTargetKPI(List<WindDailyTargetKPI> windDailyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertDailyTargetKPI(windDailyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertMonthlyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertMonthlyTargetKPI(List<WindMonthlyTargetKPI> windMonthlyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertMonthlyTargetKPI(windMonthlyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertMonthlyUploadingLineLosses")]
        [HttpPost]
        public async Task<IActionResult> InsertMonthlyUploadingLineLosses(List<WindMonthlyUploadingLineLosses> windMonthlyUploadingLineLosses)
        {
            try
            {
                var data = await _dgrBs.InsertMonthlyUploadingLineLosses(windMonthlyUploadingLineLosses);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("InsertWindJMR")]
        [HttpPost]
        public async Task<IActionResult> InsertWindJMR(List<WindMonthlyJMR> windMonthlyJMR)
        {
            try
            {
                var data = await _dgrBs.InsertWindJMR(windMonthlyJMR);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("InsertWindDailyLoadShedding")]
        [HttpPost]
        public async Task<IActionResult> InsertWindDailyLoadShedding(List<WindDailyLoadShedding> windDailyLoadShedding)
        {
            try
            {
                var data = await _dgrBs.InsertWindDailyLoadShedding(windDailyLoadShedding);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("InsertDailyJMR")]
        [HttpPost]
        public async Task<IActionResult> InsertDailyJMR(List<WindDailyJMR> windDailyJMR)
        {
            try
            {
                var data = await _dgrBs.InsertDailyJMR(windDailyJMR);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarDailyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyTargetKPI(List<SolarDailyTargetKPI> solarDailyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyTargetKPI(solarDailyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarMonthlyTargetKPI")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarMonthlyTargetKPI(List<SolarMonthlyTargetKPI> solarMonthlyTargetKPI)
        {
            try
            {
                var data = await _dgrBs.InsertSolarMonthlyTargetKPI(solarMonthlyTargetKPI);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarMonthlyUploadingLineLosses")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarMonthlyUploadingLineLosses(List<SolarMonthlyUploadingLineLosses> solarMonthlyUploadingLineLosses)
        {
            try
            {
                var data = await _dgrBs.InsertSolarMonthlyUploadingLineLosses(solarMonthlyUploadingLineLosses);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarJMR")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarJMR(List<SolarMonthlyJMR> solarMonthlyJMR)
        {
            try
            {
                var data = await _dgrBs.InsertSolarJMR(solarMonthlyJMR);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarDailyLoadShedding")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyLoadShedding(List<SolarDailyLoadShedding> solarDailyLoadShedding)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyLoadShedding(solarDailyLoadShedding);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("InsertSolarInvAcDcCapacity")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarInvAcDcCapacity(List<SolarInvAcDcCapacity> solarInvAcDcCapacity)
        {
            try
            {
                var data = await _dgrBs.InsertSolarInvAcDcCapacity(solarInvAcDcCapacity);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("InsertSolarDailyBDloss")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarDailyBDloss(List<SolarDailyBDloss> solarDailyBDloss)
        {
            try
            {
                var data = await _dgrBs.InsertSolarDailyBDloss(solarDailyBDloss);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingPyranoMeter1Min")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingPyranoMeter1Min(List<SolarUploadingPyranoMeter1Min> solarUploadingPyranoMeter1Min)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingPyranoMeter1Min(solarUploadingPyranoMeter1Min);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingPyranoMeter15Min")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingPyranoMeter15Min(List<SolarUploadingPyranoMeter15Min> solarUploadingPyranoMeter15Min)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingPyranoMeter15Min(solarUploadingPyranoMeter15Min);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("InsertSolarUploadingFilegeneration")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingFilegeneration(List<SolarUploadingFilegeneration> addSolarUploadingFilegenerations)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingFilegeneration(addSolarUploadingFilegenerations);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
       
        [Route("InsertSolarUploadingFileBreakDown")]
        [HttpPost]
        public async Task<IActionResult> InsertSolarUploadingFileBreakDown(List<SolarUploadingFileBreakDown> addSolarUploadingFileBreakDowns)
        {
            try
            {
                var data = await _dgrBs.InsertSolarUploadingFileBreakDown(addSolarUploadingFileBreakDowns);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        
        [Route("GetSolarInverterFromdailyGenSummary/{state}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarInverterFromdailyGenSummary(string state, string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarInverterFromdailyGenSummary(state, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarMonthlyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarMonthlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetSolarYearlyGenSummaryReport1/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarYearlyGenSummaryReport1(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarYearlyGenSummaryReport1(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarYearlyGenSummaryReport2/{fromDate}/{toDate}/{country}/{state}/{spv}/{site}/{inverter}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarYearlyGenSummaryReport2(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarYearlyGenSummaryReport2(fromDate, toDate, country, state, spv, site, inverter, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }




        [Route("GetSolarPerformanceReportBySiteWise/{fy}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceReportBySiteWise(string fy, string fromDate, string todate)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceReportBySiteWise(fy, fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarPerformanceReportBySPVWise/{fy}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarPerformanceReportBySPVWise(string fy, string fromDate, string todate)
        {
            try
            {
                var data = await _dgrBs.GetSolarPerformanceReportBySPVWise(fy, fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        #region get views

        [Route("GetSolarDailyGenSummaryData/{fromDate}/{ToDate}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryData(string fromDate, string ToDate)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSolarDailyGenSummary(fromDate, ToDate);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetWindDailyTargetKPI")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyTargetKPI(string fromDate, string todate)
        {

            try
            {
                var data = await _dgrBs.GetWindDailyTargetKPI(fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

        }
        [Route("GetSolarDailyTargetKPI/{fromDate}/{todate}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyTargetKPI(string fromDate, string todate)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyTargetKPI(fromDate, todate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindMonthlyTargetKPI/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyTargetKPI(string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyTargetKPI(fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyTargetKPI/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyTargetKPI(string fy, string month)
        {
         try
                {
                    var data = await _dgrBs.GetSolarMonthlyTargetKPI(fy, month);
                    return Ok(data);

             }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
            }
        }
        [Route("GetWindMonthlyLineLoss/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyLineLoss(string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyLineLoss(fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyLineLoss/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyLineLoss(string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyLineLoss(fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetWindMonthlyJMR/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetWindMonthlyJMR(string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetWindMonthlyJMR(fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarMonthlyJMR/{fy}/{month}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarMonthlyJMR(string fy, string month)
        {
            try
            {
                var data = await _dgrBs.GetSolarMonthlyJMR(fy, month);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarACDCCapacity/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarACDCCapacity(string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarACDCCapacity(site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetUserManagement/{userMail}/{date}")]
        [HttpGet]
        public async Task<IActionResult> GetUserManagement(string userMail, string date)
        {
            try
            {
                var data = await _dgrBs.GetUserManagement(userMail, date);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("GetSolarBDType")]
        [HttpGet]
        public async Task<IActionResult> GetSolarBDType()
        {
            try
            {
                var data = await _dgrBs.GetSolarBDType();
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyloadShedding/{site}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyloadShedding(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyloadShedding/{site}/{fromDate}/{toDate}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyloadShedding(string site, string fromDate, string toDate)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyloadShedding(site, fromDate, toDate);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetWindDailyGenSummaryPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyGenSummaryPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyGenSummaryPending(date,site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyGenSummaryPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyGenSummaryPending(string date,string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyGenSummaryPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        
        [Route("GetWindDailyBreakdownPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetWindDailyBreakdownPending(string date, string site)
        {
            try
            {
                var data = await _dgrBs.GetWindDailyBreakdownPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("GetSolarDailyBreakdownPending/{date}/{site}")]
        [HttpGet]
        public async Task<IActionResult> GetSolarDailyBreakdownPending(string date,string site)
        {
            try
            {
                var data = await _dgrBs.GetSolarDailyBreakdownPending(date, site);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateWindDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.UpdateWindDailyGenSummaryApproveStatus(dailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateWindDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                var data = await _dgrBs.UpdateWindDailyBreakdownApproveStatus(windDailyBreakdownReport);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteWindDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteWindDailyGenSummaryApproveStatus(List<DailyGenSummary> dailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.DeleteWindDailyGenSummaryApproveStatus(dailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [Route("DeleteWindDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteWindDailyBreakdownApproveStatus(List<WindDailyBreakdownReport> windDailyBreakdownReport)
        {
            try
            {
                var data = await _dgrBs.DeleteWindDailyBreakdownApproveStatus(windDailyBreakdownReport);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("UpdateSolarDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.UpdateSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("DeleteSolarDailyGenSummaryApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteSolarDailyGenSummaryApproveStatus(List<SolarDailyGenSummary> solarDailyGenSummary)
        {
            try
            {
                var data = await _dgrBs.DeleteSolarDailyGenSummaryApproveStatus(solarDailyGenSummary);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("UpdateSolarDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> UpdateSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                var data = await _dgrBs.UpdateSolarDailyBreakdownApproveStatus(solarFileBreakdown);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
        [Route("DeleteSolarDailyBreakdownApproveStatus")]
        [HttpPost]
        public async Task<IActionResult> DeleteSolarDailyBreakdownApproveStatus(List<SolarFileBreakdown> solarFileBreakdown)
        {
            try
            {
                var data = await _dgrBs.DeleteSolarDailyBreakdownApproveStatus(solarFileBreakdown);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        [Route("GetBatches")]
        [HttpGet]
        public async Task<IActionResult> GetBatches(string importFromDate, string importToDate, int siteId, string status)
        {
            {
                try
                {
                    var data = await _dgrBs.GetBatches(importFromDate, importToDate, siteId, status);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("GetSiteList")]
        [HttpGet]
        public async Task<IActionResult> GetSiteList(string state, string spvdata)
        {
            {
                try
                {
                    var data = await _dgrBs.GetSiteList(state, spvdata);
                    return Ok(data);

                }
                catch (Exception ex)
                {

                    return BadRequest(ex.Message);
                }
            }
        }
        [Route("eQry/{qry}")]
        [HttpGet]
        public async Task<IActionResult> eQry(string qry)
        {
            try
            {
                var data = await _dgrBs.eQry(qry);
                return Ok(data);

            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }


        #endregion
    }
}
