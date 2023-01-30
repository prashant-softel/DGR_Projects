﻿using DGRA_V1.Models;
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
using DGRA_V1.Filters;

namespace DGRA_V1.Controllers
{
    
   //  [Authorize]
    public class HomeController : Controller
    {
        

        private readonly ILogger<HomeController> _logger;

        private IDapperRepository _idapperRepo;
       
        private readonly GraphServiceClient _graphServiceClient;


        public HomeController(ILogger<HomeController> logger,
                         //GraphServiceClient graphServiceClient,
                         IDapperRepository idapperRepo)
        {
            _logger = logger;
            //_graphServiceClient = graphServiceClient;
            _idapperRepo = idapperRepo;
        }

        public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
       
       // private readonly GraphServiceClient _graphServiceClient;
        // TEST A 123456789
      //  public HomeController(ILogger<HomeController> logger, IDapperRepository idapperRepo, GraphServiceClient graphServiceClient)
       // {
        //    _logger = logger;
          //  _idapperRepo = idapperRepo;

       // }
      


      
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

        // [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Index()
        {
            //var user = await _graphServiceClient.Me.Request().GetAsync();
            //ViewData["ApiResult"] = user.DisplayName;
            //if(!string.IsNullOrEmpty( user.DisplayName))
            //{
            //    return RedirectToAction("Dashbord");
            //}
          //  HttpContext.Session.SetString("product", "laptop");

            return View();
        }


        [AuthorizeForScopes(ScopeKeySection = "DownstreamApi:Scopes")]
      
        public async Task<IActionResult> SSOLogin ()
        {

            //var user = await _graphServiceClient.Me.Request().GetAsync();
           // user.DisplayName = "";
           

          // HttpContext.Session.SetString("DisplayName", user.DisplayName);
           
           // String Name = HttpContext.Session.GetString("DisplayName");
           
               /*if (!string.IsNullOrEmpty(Name))
                {
               
                if (user.DisplayName == "Sujit")
                    if (Name == "Sujit")
                    {
                    TempData["name"] = "Sujit Kumar";
                    TempData["role"] = "Admin";
                    TempData["userid"] = "1";
                    HttpContext.Session.SetString("name", "Sujit Kumar");
                    HttpContext.Session.SetString("role", "User");
                    HttpContext.Session.SetString("userid", "1");
                }
                if (Name == "prashant")
                {
                    
                    HttpContext.Session.SetString("name", "Prashant Shetye");
                    HttpContext.Session.SetString("role", "Admin");
                    HttpContext.Session.SetString("userid", "2");
                    TempData["name"] = "Prashant Shetye";
                    TempData["role"] = "User";
                    TempData["userid"] = "2";
                }*/
                return RedirectToAction("Dashbord");
           // }
            //return RedirectToAction("Index");
        }

        public async Task<IActionResult> Login(string username, string pass)
        {
            string status = "";
            string line = "";
            string[] userList = username.Split("@");
            string last = userList[1];
            //if (last.Equals("herofutureenergies.com")) {
               // SSOLogin();
           // }
           // else {
                bool login_status = false;
                LoginModel model = new LoginModel();
                //UserAccess usermodel = new UserAccess();
                System.Collections.Generic.Dictionary<string, object>[] map = new System.Collections.Generic.Dictionary<string, object>[1];
                try
                {
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UserLogin?username=" + username + "&password=" + pass + "";
                    WebRequest request = WebRequest.Create(url);
                    using (WebResponse response = (HttpWebResponse)request.GetResponse()){
                        Stream receiveStream = response.GetResponseStream();
                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8)){
                             line = readStream.ReadToEnd().Trim();
                        if (string.IsNullOrEmpty(line)){
                                
                                login_status = false;
                               
                        }
                        else{
                                login_status = true;
                                model = JsonConvert.DeserializeObject<LoginModel>(line);
                                HttpContext.Session.SetString("DisplayName", model.username);
                                HttpContext.Session.SetString("role", model.user_role);
                                HttpContext.Session.SetString("userid",model.login_id.ToString());
                             
                                int loginid = model.login_id;
                                string role = model.user_role;
                                 var actionResult = await GetUserAccess(loginid, role, true);
                                
                                
                                
                                
                              

                            }

                    }

                }
            }
            catch (Exception ex)
            {
                //TempData["notification"] = "Username and password invalid Please try again !";
                string message = ex.Message;
            }
           
        //}
            // return Ok(model);
            //return RedirectToAction("Dashbord", "Home");
            return Content(line, "application/json");
        }



        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        // Added Code on Redirection Remote 
       /* [ActionName("signin-oidc")]
        public IActionResult signinoidc()
        {
            //return View();
            return RedirectToAction("Dashbord");
        }*/

        //[Authorize]
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult Dashbord()
        {
            TempData["notification"] = "";
            return View();
            
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindDailyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindGenView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyLinelossView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindMonthlyJMRView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindDailyLoadSheddingView()
        {
            TempData["notification"] = "";
            return View();
        }

        // Report Routs
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindSiteMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindLocationMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindGenReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindBDReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindPRReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindWeeklyPRReports()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SiteUserMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult UserRegister()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult ImportApproval()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> WindNewUserRegister(string fname,string useremail,string role,string userpass)
        {
            string line = "";
            try
            {
               /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role + "&userpass="+ userpass + "";
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
                TempData["notification"] = ""; 
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> UpdatePassword(int loginid, string updatepass)
        {
            string line = "";
            try
            {
                /// var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                //var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role+ "&created_on="+ created_on + "";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/UpdatePassword?loginid=" + loginid + "&updatepass=" + updatepass +"";
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
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetWindUserInfo(int login_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
               // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url =  _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetWindUserInformation?login_id=" + login_id;
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
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetSolarUserInfo(int login_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetSolarUserInformation?login_id=" + login_id;
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
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        public async Task<IActionResult> GetUserAccess(int login_id,string role ,bool actionType = false)
        {
            UserAccess usermodel = new UserAccess();
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id="+ login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetWindUserAccess?login_id=" + login_id+"&role="+ role;
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        line = readStream.ReadToEnd().Trim();
                        if (actionType == true)
                        {
                            usermodel.access_list = JsonConvert.DeserializeObject<List<UserAccess>>(line);
                            HttpContext.Session.SetString("UserAccess", JsonConvert.SerializeObject( usermodel));
                           // var people = System.Text.Json.JsonSerializer.Deserialize<List<Person>>(line);
                           // HttpContext.Session.SetString("UserAccess", people.ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        //[HttpPost]
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> SubmitAccess(int login_id,string site,string pages,string reports, string site_type)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                // var url = "http://localhost:23835/api/Login/SubmitUserAccess?login_id=" + login_id+"&siteList="+ site +"&pageList="+ pages +"&reportList="+ reports;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/SubmitUserAccess?login_id=" + login_id + "&siteList=" + site + "&pageList=" + pages + "&reportList=" + reports + "&site_type="+site_type;
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
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult WindUserView()
        {
            
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarUserView()
        {

            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public async Task<IActionResult> GetPageList(int login_id, int site_type)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
               // var url = "http://localhost:23835/api/Login/GetPageList?login_id=" + login_id;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/GetPageList?login_id=" + login_id + "&site_type=" + site_type;
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
                TempData["notification"] = "";
            }
            return Content(line, "application/json");

        }
        
        public IActionResult SolarGenView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarDailyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarDailyLoadSheddingView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyTargetKPIView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyLinelossView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarMonthlyJMRView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarAcDcCapacityView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGHIPOA1MinView()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGHIPOA15MinView()
        {
            TempData["notification"] = "";
            return View();
        }
        // Report Routs
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarGenReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarBDReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarSiteMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarLocationMaster()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarPRReport()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public IActionResult SolarWeeklyPRReports()
        {
            TempData["notification"] = "";
            return View();
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult WindUserDetails(string id)
        {
           return RedirectToAction("WindUserView", new { id });
        }
        [TypeFilter(typeof(SessionValidation))]
        public ActionResult SolarUserDetails(string id,int site_type)
        {
            return RedirectToAction("SolarUserView", new { id , site_type});
        }
        public async Task<ActionResult> Logout(string username, string pass)
        {
            TempData["notification"] = "";
            //Response.Redirect("somepage.aspx");
            return RedirectToAction("Index", "Home");
            // return View();
        }
        [HttpGet("SignOut")]
        public IActionResult SignOut([FromRoute] string scheme)
        {
            return RedirectToAction("Index", "Home");
        }
       
    }
}