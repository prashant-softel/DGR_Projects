using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IDapperRepository _idapperRepo;
      


        public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
  // TEST A 
        public HomeController(ILogger<HomeController> logger, IDapperRepository idapperRepo)
        {
            _logger = logger;
            _idapperRepo = idapperRepo;
        }

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        public IActionResult Index()
        {
            //return View();
            return RedirectToAction("Dashbord", "Home");
            //return RedirectToAction("Upload", "FileUpload");
        }
        public IActionResult Index1()
        {
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        public IActionResult Dashbord()
        {
            //return RedirectToAction("Dashbord", "Home");
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        public async Task<IActionResult> Login(string username, string pass)
        {
            string status = "";
            username = "sujitkumar@gmail.com";
            pass = "sujit123";
            bool login_status = false;
            LoginModel model = new LoginModel();
            System.Collections.Generic.Dictionary<string, object>[] map = new System.Collections.Generic.Dictionary<string, object>[1];
            try
            {
                var url = "http://localhost:23835/api/Login/UserLogin?username=" + username + "&password=" + pass + "";
                WebRequest request = WebRequest.Create(url);

                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string line = readStream.ReadToEnd().Trim();
                        if (string.IsNullOrEmpty(line))
                        {
                            // status = "Username and password invalid Please try again !";
                            login_status = false;

                        }
                        else
                        {
                            login_status = true;
                           // model = JsonConvert.DeserializeObject<LoginModel>(line);

                        }

                    }

                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Username and password invalid Please try again !";
            }
            // return View();
            map[0] = new System.Collections.Generic.Dictionary<string, object>();
            if (login_status == true)
            {

                map[0].Add("UserName", model.name);
                map[0].Add("UserID", model.Username);
                map[0].Add("UserRole", model.user_role);
                map[0].Add("LoginID", model.login_id);
                map[0].Add("status", "1");
               

            }
            else
            {
                map[0].Add("UserName", "");
                map[0].Add("UserID", "");
                map[0].Add("UserRole", "");
                map[0].Add("LoginID", "");
                map[0].Add("status", "0");
            }
         
             return Ok(model);
            //return RedirectToAction("Dashbord", "Home");
        }

        // Wind Views
        public async  Task<IActionResult> WindGenView( string fromDate ,string ToDate)
        {
            string status = "";
            DailyGenSummary dailyGen = new DailyGenSummary();
            fromDate = "2022-08-10";
            ToDate = "2022-08-30";
            try
            {
                var url= "http://localhost:23835/api/DGR/GetWindDailyGenSummary?fromDate=" + fromDate + "&ToDate=" + ToDate+"";
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        string line = readStream.ReadToEnd().Trim();
                       

                            dailyGen.list = JsonConvert.DeserializeObject<List< DailyGenSummary>>(line);

                        
                        return View(dailyGen);

                        

                      

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
        public IActionResult WindDailyTargetKPIView()
        {
            //return RedirectToAction("WindDailyTargetKPIView", "ReportViews");
            return View();
        }
        public IActionResult WindMonthlyTargetKPIView()
        {
            //return RedirectToAction("WindMonthlyTargetKPIView", "ReportViews");
            return View();
        }
        public IActionResult WindMonthlyLinelossView()
        {
            //return RedirectToAction("WindMonthlyLinelossView", "ReportViews");
            return View();
        }
        public IActionResult WindJMRView()
        {
            // return RedirectToAction("WindJMRView", "ReportViews");
            return View();
        }
        public IActionResult WindDailyLoadSheddingView()
        {
            // return RedirectToAction("WindDailyLoadSheddingView", "ReportViews");
            return View();
        }
        public IActionResult WindApproval()
        {
            // return RedirectToAction("WindDailyLoadSheddingView", "ReportViews");
            return View();
        }
        public async Task<ActionResult> Logout(string username, string pass)
        {

          
            //Response.Redirect("somepage.aspx");
            return RedirectToAction("Index", "Home");
            // return View();
        }


    }
}
