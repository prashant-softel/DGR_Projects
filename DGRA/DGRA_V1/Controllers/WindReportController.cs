using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DGRA_V1.Controllers
{
    public class WindReportController : Controller
    {

        private IDapperRepository _idapperRepo ;
        public WindReportController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        [ActionName("View-Wind-Daily-Report")]
        public async Task<IActionResult> WindGenView(string fromDate, string ToDate)
        {
            string status = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            fromDate = "2022-08-10";
            ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string line = readStream.ReadToEnd().Trim();
                        dailyGen.list = JsonConvert.DeserializeObject<List<DailyGenSummary>>(line);
                        return View("ViewWindDailyReport", dailyGen);
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "invalid  !";

            }

            // return RedirectToAction("WindGenView", "Home");
            return View(dailyGen);


        }
        public async Task<IActionResult> GetSiteList(string state, string spv)
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSiteList?state=" + statedata + "&spvdata=" + spvdata+ "";
               // var url = "http://localhost:23835/api/DGR/GetSiteList?state=" + statedata + "&spvdata=" + spvdata;
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
