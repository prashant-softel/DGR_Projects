using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
using System.Threading.Tasks;


namespace DGRA_V1.Controllers
{

    // [Authorize]
    [AllowAnonymous]
    public class Dashboard : Controller
    {
        private IDapperRepository _idapperRepo;
        public Dashboard(IDapperRepository idapperRepo)
        {
            _idapperRepo = idapperRepo;
        }

        public JsonSerializerOptions _options { get; private set; }
        public async Task<IActionResult> GetFinacialYear()
        {
            //countryname = "India";
            string line = "";
            try
            {
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
        public async Task<IActionResult> GetWindSiteList(string state, string spv, string sitelist)
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSiteList?state=" + statedata + "&spvdata=" + spvdata + "&site=" + sitelist;
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

        public async Task<IActionResult> GetSolarSiteList(string state, string spv, string sitelist)
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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteList?state=" + statedata + "&spvdata=" + spvdata+"&site="+ sitelist;
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
        public async Task<IActionResult> GetTotalMWforDashbord(string w_site, string s_site)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindAndSolarMW?w_site=" + w_site + "&s_site=" + s_site;
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

        //public async Task<IActionResult> GetWindDashboardDataByLastDay(string startDate, string endDate, string FY, string sites, string date)
        public async Task<IActionResult> GetWindDashboardDataByLastDay(string FY, string sites, string date)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDashboardDataByLastDay?FY="+ FY+ "&sites=" + sites + "&date="+ date;
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
        public async Task<IActionResult> GetWindGraphData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDashboardData?startDate=" + startDate + "&endDate=" + endDate+ "&FY="+ FY + "&sites="+ sites+ "&monthly="+ monthly ;
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
        public async Task<IActionResult> GetWindDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDashboardDataByCurrentMonth?startDate=" + startDate + "&endDate=" + endDate+ "&FY="+ FY + "&sites="+ sites+ "&month="+ month;
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
        public async Task<IActionResult> GetWindDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindDashboardDataByYearly?startDate=" + startDate + "&endDate=" + endDate+ "&FY="+ FY + "&sites="+ sites;
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
        public async Task<IActionResult> GetSolarDashboardData(string startDate, string endDate, string FY, string sites,bool monthly)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDashboardData?startDate=" + startDate + "&endDate=" + endDate + "&FY=" + FY + "&sites=" + sites+"&monthly="+ monthly;
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
        public async Task<IActionResult> GetSolarDashboardDataByLastDay(string FY, string sites, string date)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDashboardDataByLastDay?FY=" + FY + "&sites=" + sites+"&date="+date;
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
        public async Task<IActionResult> GetSolarDashboardDataByCurrentMonth(string startDate, string endDate, string FY, string sites, string month)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDashboardDataByCurrentMonth?startDate=" + startDate + "&endDate=" + endDate + "&FY=" + FY + "&sites=" + sites + "&month=" + month;
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
        public async Task<IActionResult> GetSolarDashboardDataByYearly(string startDate, string endDate, string FY, string sites)
        {
            string line = "";
            try
            {
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarDashboardDataByYearly?startDate=" + startDate + "&endDate=" + endDate + "&FY=" + FY + "&sites=" + sites ;
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