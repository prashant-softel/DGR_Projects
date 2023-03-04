using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace DGRA_V1.Controllers
{

    // [Authorize]
    [AllowAnonymous]
    public class WindReportController : Controller
    {
        private IDapperRepository _idapperRepo;
        public WindReportController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
        
        //FInacnial Year APi Hardcoded 
        public async Task<IActionResult> GetFinacialYear()
        {
            //countryname = "India";
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL")+"/api/DGR/GetFinancialYear";
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
        public async Task<IActionResult>GetStateList(string countryname,string sitelist)
        {
            //countryname = "India";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetStateList?country=" + countryname + "&site="+ sitelist;
                //var url = "http://localhost:23835/api/DGR/GetStateList?country=" + countryname + "";
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
        public async Task<IActionResult> GetSPVList(string state, string sitelist)
        {
            string line = "";
            string statedata = "";
            if (state != "undefined" && state !=null)
            {
                statedata = state;
            }
          
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSPVList?state=" + statedata + "&site="+ sitelist;
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
        public async Task<IActionResult> GetSiteList(string state, string spv,string sitelist)
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSiteList?state=" + statedata + "&spvdata=" + spvdata+"&site="+ sitelist;
               // var url = "http://localhost:23835/api/DGR/GetSiteList?state="+ statedata + "&spvdata="+ spvdata;
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
        public async Task<IActionResult> GetWTGList(string siteid)
        {
           
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWTGList?siteid=" + siteid + "";
                //var url = "http://localhost:23835/api/DGR/GetWTGList?siteid=" + siteid + "";
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
        public async Task<IActionResult> GetSiteMaster(string sitelist)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster?site="+ sitelist;

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
        public async Task<IActionResult> GetLocationMaster(string sitelist)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster?site="+ sitelist;

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
        public async Task<IActionResult> GetWindDailyGenerationReportWTGWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string reportType = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenerationReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&reportType="+ reportType + "";
               
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

        public async Task<IActionResult> GetWindDailyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {
            string month = "";
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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

        //Monthly Gen WTG WIse
        public async Task<IActionResult> GetWindMonthlyGenerationReportWTGWise(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyGenerationReport?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&reportType=" + reportType + "";

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

        //Monthly Gen SIteWIse
        public async Task<IActionResult> GetWindMonthlyGenerationReportSiteWise(string fy, string month, string country, string state, string spv, string site, string wtg, string reportType)
        {


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyYearlyGenSummaryReport2?fy=" + fy + "&month=" + month + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "";

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
        public async Task<IActionResult> GetWindYearlyGenerationReportWTGWise(string fromDate,string toDate,  string country, string state, string spv, string site, string wtg, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindYearlyGenSummaryReport1?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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
        public async Task<IActionResult> GetWindYearlyGenerationReportSiteWise(string fromDate, string toDate, string country, string state, string spv, string site, string wtg, string reportType)
        {
            string month = "";


            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindYearlyGenSummaryReport2?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "&month=" + month + "";

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
       
        public async Task<IActionResult> GetWindBreakdownReport(string fromDate, string toDate, string country, string state, string spv, string site, string wtg)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyBreakdownReport?fromDate=" + fromDate + "&toDate=" + toDate + "&country=" + country + "&state=" + state + "&spv=" + spv + "&site=" + site + "&wtg=" + wtg + "";
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
        public async Task<IActionResult> GetWindPRReportSPVWise(string fy, string fromDate, string toDate,string sitelist)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportBySPVWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist;
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
        public async Task<IActionResult> GetWindPRReportSiteWise(string fy, string fromDate, string toDate,string sitelist)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ sitelist;
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetOperationHeadData?site="+ site + "";
                
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                
                //old 
               // var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
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

        public async Task<IActionResult> GetMontlyOperation(string fy, string fromDate, string toDate, string site)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                ///old api 
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise?fy=" + fy + "&fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site;
                //old 
               // var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindPerformanceReportSiteWise_2?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
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
        public async Task<IActionResult> DeleteWindSite(int siteid)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/DeleteWindSite?siteid=" + siteid;
               
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
        public async Task<IActionResult> GetWindMajorBreakdown(string fromDate, string toDate,string siteList)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMajorBreakdown?fromDate=" + fromDate + "&toDate=" + toDate + "&site="+ siteList;
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
