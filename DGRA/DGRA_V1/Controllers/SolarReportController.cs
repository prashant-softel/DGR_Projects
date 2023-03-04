using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
namespace DGRA_V1.Controllers
{

    // [Authorize]
    [AllowAnonymous]
    public class SolarReportController : Controller
    {
        private IDapperRepository _idapperRepo;
        public SolarReportController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }

        //FInacnial Year APi Hardcoded 
        public async Task<IActionResult> GetFinancialYear()
        {
            //countryname = "India";
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetFinancialYear";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        // Country List
        public async Task<IActionResult> GetCountryList()
        {
            string country = "";
            CountryList countrylist = new CountryList();
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetCountryList";
                // var url = "http://localhost:23835/api/DGR/GetCountryList";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string line = readStream.ReadToEnd().Trim();
                        countrylist.list = JsonConvert.DeserializeObject<List<CountryList>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            //return Content(country, "application/json");
            return View(countrylist);
        }
        // State List
        public async Task<IActionResult> GetStateList(string countryname,string sitelist)
        {
            //countryname = "India";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetStateListSolar?country=" + countryname + "&site="+ sitelist;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        // SPV List
        public async Task<IActionResult> GetSPVList(string state,string sitelist)
        {
            string line = "";
            // state = "RJ";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSPVSolarList?state=" + state + "&site="+ sitelist;
                // var url = "http://localhost:23835/api/DGR/GetSPVList?state=" + state + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");

        }
        // Site List
        public async Task<IActionResult> GetSiteList(string state, string spv, string sitelist)
        {
            string line = "";
            string spvdata = "";
            string statedata = "";
            if (state == "undefined" || state == null)
            {
                statedata = "";
            }
            else
            {
                statedata = state;
            }
            if (spv == "undefined" || spv == null)
            {
                spvdata = "";
            }
            else
            {
                spvdata = spv;
            }

            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSiteListSolar?state=" + statedata + "&spvdata=" + spvdata + "&site=" + sitelist;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");

        }
        // Site List
        public async Task<IActionResult> GetInvList(string siteid, string state, string spv)
        {
            // siteid = 187;
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetInvList?siteid="+ siteid + "&state=" + state + "&spv=" + spv;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");

        }
         
        public async Task<IActionResult> GetSolarBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string inv)
        {
            /* fromDate = "2022-10-18";
             toDate = "2022-10-20";
             country     = "";
             state       = "";
             spv         = "";
             site        = "";
             wtg         = "";*/
            //WindBreakdownReports breakdown = new WindBreakdownReports();
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inv="+ inv;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarSiteMaster(string sitelist)
        {
          
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster?site="+ sitelist;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetLocationMaster(string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarLocationMasterBySite?site=" + site;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetSolarPRReportSPVWise(string fy, string fromDate, string toDate,string sitelist)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportBySPVWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetSolarPRReportSiteWise(string fy, string fromDate, string toDate,string sitelist)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportBySiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist;
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetWeeklyOperation(string fy, string fromDate, string toDate, string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportBySiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                // var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetMontlyOperation(string fy, string fromDate, string toDate, string site,int cnt)
        {

            string line = "";
            try
            {

                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportBySiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "&cnt="+cnt;
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetYearlyOperation(string fy, string fromDate, string toDate, string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportBySiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" +country+ "&state=" +state+ "&spv=" +spv+ "&site=" +site+ "&wtg=" +wtg+"";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetOperationHeadData(string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarOperationHeadData?site=" + site + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarMajorBreakdown(string fy, string fromDate, string toDate, string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMajorBreakdownData?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarDailyGenerationReportWTGWise(string fromDate, string toDate, string country, string state, string spv, string site, string inv)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyGenerationReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inv=" + inv + "&reportType=" + reportType + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }

        public async Task<IActionResult> GetSolarDailyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string inv)
        {
            string month = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inv=" + inv + "&month=" + month + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarMonthlyGenerationReportInvWise(string fy, string month, string country, string state, string spv, string site, string inverter, string reportType)
        {


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMonthlyGenSummaryReport1?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inverter=" + inverter + "&reportType=" + reportType + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarMonthlyGenerationReportSiteWise(string fy, string month, string country, string state, string spv, string site, string inverter, string reportType)
        {


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMonthlyGenSummaryReport2?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inverter=" + inverter + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarYearlyGenerationReportWTGWise(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarYearlyGenSummaryReport1?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inverter=" + inverter + "&month=" + month + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> GetSolarYearlyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string inverter, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarYearlyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&inverter=" + inverter + "&month=" + month + "";

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> DeleteSolarSite(int siteid)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/DeleteSolarSite?siteid=" + siteid;

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> PPTCreate(string temp)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/PPTCreate?temp=" + temp;

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
        public async Task<IActionResult> MailSend(string fname)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/MailSend?fname=" + fname;

                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        //  breakdown.list = JsonConvert.DeserializeObject<List<WindBreakdownReports>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Data Not Presents !";
            }
            return Content(line, "application/json");
        }
    }
}
