﻿using DGRA_V1.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DGRA_V1.Repository.Interface;

namespace DGRA_V1.Controllers
{

    //[Authorize]
    public class WindViewController : Controller
    {
        private IDapperRepository _idapperRepo;
        public WindViewController(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        // public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }

        
        public async Task<IActionResult> WindGenView(string fromDate, string ToDate)
        {

            string line = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            // fromDate = "2022-08-10";
            //ToDate = "2022-08-30";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyGenSummary?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
               // var url = "http://localhost:23835/api/DGR/GetWindDailyGenSummary?fromDate=" + fromDate + "&ToDate=" + ToDate + "";
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
        public async Task<IActionResult> WindDailyTargetKPI(string fromDate, string toDate)
        {
           
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&toDate=" + toDate + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyTargetKPI?fromDate=" + fromDate + "&toDate=" + toDate + "";
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
        public async Task<IActionResult> GetMonthlyTargetKPI(string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyTargetKPI?fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyTargetKPI?fy=" + year + "&month=" + month + "";
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
        public async Task<IActionResult> GetMonthlyLinelossView(string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyLineLoss?fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyLineLoss?fy=" + year + "&month=" + month + "";
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
        public async Task<IActionResult> GetDailyLoadshedding(int site, string fromDate,string toDate)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDailyloadShedding?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate + "";
                //var url = "http://localhost:23835/api/DGR/GetWindDailyloadShedding?site=" + site + "&fromDate=" + fromDate + "&toDate=" + toDate +"";
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

        public async Task<IActionResult>GetMonthlyJMRView(string year, string month)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindMonthlyJMR?fy=" + year + "&month=" + month + "";
                //var url = "http://localhost:23835/api/DGR/GetWindMonthlyJMR?fy=" + year + "&month=" + month + "";
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

        public async Task<IActionResult> GetImportBatches(string importFromDate, string importToDate, int siteId,int importType, int status)
        {

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetImportBatches?importFromDate=" + importFromDate + "&importToDate=" + importToDate + "&siteId=" + siteId + "&importType=" + importType + "&status=" + status + "";
                //var url = "http://localhost:23835/api/DGR/GetImportBatches?importFromDate=" + importFromDate + "&importToDate=" + importToDate + "&siteId="+ siteId + "&importType="+ importType+ "&status=" + status + "";
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
        
        public async Task<IActionResult> DataApproved(string data, int approvedBy, string approvedByName, int status)
        {
            //var json = JsonConvert.SerializeObject(data);

            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetApprovalFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
                //var url = "http://localhost:23835/api/DGR/SetApprovalFlagForImportBatches?dataId=" + data + "&approvedBy=" + approvedBy + "&approvedByName=" + approvedByName + "&status=" + status + "";
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