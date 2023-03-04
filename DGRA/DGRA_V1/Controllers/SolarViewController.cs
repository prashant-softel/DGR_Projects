using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace DGRA_V1.Controllers
{
    //[Authorize]
    [AllowAnonymous]
    public class SolarViewController : Controller
    {
        private IDapperRepository _idapperRepo;
        public SolarViewController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }

        public async Task<IActionResult> SolarGenView(string site, string fromDate, string ToDate)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
           
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyGenSummary1?site=" + site + "&fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }

        public async Task<IActionResult> SolarDailyLoadSheddingView(string site, string fromDate, string toDate)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyloadShedding?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> SolarDailyTargetKPIView(string fromDate, string toDate, string site)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDailyTargetKPI?fromDate=" + fromDate + "&toDate=" + toDate + "&site=" + site + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> SolarMonthlyTargetKPIView(string year, string month, string site)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMonthlyTargetKPI?fy=" + year + "&month=" + month + "&site=" + site + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> SolarMonthlyLinelossView(string year, string month, string site)
        {
            string line = "";
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMonthlyLineLoss?fy=" + year + "&month=" + month + "&site=" + site + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> SolarMonthlyJMRView(string year, string month, string site)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarMonthlyJMR?fy=" + year + "&month=" + month + "&site=" + site + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }

        public async Task<IActionResult> SolarAcDcCapacityView(string site)
        {
            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarACDCCapacity?site=" + site + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        // dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
            // return RedirectToAction("WindGenView", "Home");
            // return View(dailyGen);
        }
        public async Task<IActionResult> SolarGhi_Poa_1Min(string site, string fromDate, string ToDate)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SolarGhi_Poa_1Min?site=" + site + "&fromDate=" + fromDate + "&ToDate=" + ToDate + "";
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
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
           
        }
        public async Task<IActionResult> SolarGhi_Poa_15Min(string site, string fromDate, string ToDate)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SolarGhi_Poa_15Min?site=" + site + "&fromDate=" + fromDate + "&ToDate=" + ToDate + "";
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
                TempData["notification"] = "invalid  !";
            }
            return Content(line, "application/json");
           
        }
        public async Task<IActionResult> GetImportBatches(string importFromDate, string importToDate, string siteId, int importType, int status)
        {
            int user_id = 0;
            if (HttpContext.Session.GetString("role") == "User")
            {
                user_id = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            }
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetImportBatches?importFromDate=" + importFromDate + "&importToDate=" + importToDate + "&siteId=" + siteId + "&importType=" + importType + "&status=" + status + "&userid=" + user_id + "";
                
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
        public async Task<IActionResult> DataApproved(string data, int approvedBy, string approvedByName, int status, int actionType)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetSolarApprovalFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
               
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
        public async Task<IActionResult> DataReject(string data, int rejectedBy, string rejectByName, int status, int actionType)
        {
            //var json = JsonConvert.SerializeObject(data);

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetSolarRejectFlagForImportBatches?dataId=" + data + "&rejectedBy=" + rejectedBy + "&rejectByName=" + rejectByName + "&status=" + status + "";
                //var url = "http://localhost:23835/api/DGR/SetRejectFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
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
        public async Task<IActionResult> GetSolarGenerationImportData(int importId)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarImportGenData?importId=" + importId + "";
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
        public async Task<IActionResult> GetSolarBrekdownImportData(int importId)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarBrekdownImportData?importId=" + importId + "";
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

        public async Task<IActionResult> GetSolarP1ImportData(int importId)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarP1ImportData?importId=" + importId + "";
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
        public async Task<IActionResult> GetSolarP15ImportData(int importId)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarP15ImportData?importId=" + importId + "";
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

    }
}
