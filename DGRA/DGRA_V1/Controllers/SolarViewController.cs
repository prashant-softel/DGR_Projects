using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DGRA_V1.Controllers
{
    //[Authorize]
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



    }
}
