using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DGRA_V1.Controllers
{

    // [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IDapperRepository _idapperRepo;

        private readonly GraphServiceClient _graphServiceClient;


        public HomeController(ILogger<HomeController> logger,
                         GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
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

        public async Task<IActionResult> SSOLogin()
        {
            var user = await _graphServiceClient.Me.Request().GetAsync();
            ViewBag.Username = "";
            ViewBag.Role = "";
            ViewBag.userID = "";
            TempData["name"] = "";
            TempData["role"] = "";
            TempData["userid"] = "";

            HttpContext.Session.SetString("DisplayName", user.DisplayName);

            ViewData["ApiResult"] = user.DisplayName;
            if (!string.IsNullOrEmpty(user.DisplayName))
            {

                if (user.DisplayName == "Sujit")
                {
                    TempData["name"] = "Sujit Kumar";
                    TempData["role"] = "User";
                    TempData["userid"] = "1";
                    // ViewBag.Username = "Sujit Kumar";
                    // ViewBag.Role = "User";
                    //ViewBag.userID = "1";
                    HttpContext.Session.SetString("name", "Sujit Kumar");
                    HttpContext.Session.SetString("role", "User");
                    HttpContext.Session.SetString("userid", "1");
                }
                if (user.DisplayName == "prashant")
                {
                    // ViewBag.Username = "Prashant Shetye";
                    //ViewBag.Role = "Admin";
                    //ViewBag.userID = "2";
                    HttpContext.Session.SetString("name", "Prashant Shetye");
                    HttpContext.Session.SetString("role", "Admin");
                    HttpContext.Session.SetString("userid", "2");
                    TempData["name"] = "Prashant Shetye";
                    TempData["role"] = "Admin";
                    TempData["userid"] = "2";
                }
                return RedirectToAction("Dashbord");
            }
            return RedirectToAction("Index");
        }




        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Index1()
        {
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        [Authorize]
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
                var url = "http://localhost:5000/api/Login/UserLogin?username=" + username + "&password=" + pass + "";
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

        public IActionResult WindDailyTargetKPIView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindGenView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindMonthlyTargetKPIView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindMonthlyLinelossView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindMonthlyJMRView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindDailyLoadSheddingView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }

        // Report Routs
        public IActionResult WindSiteMaster()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WIndLocationMaster()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindGenReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindBDReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindPRReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindSiteUserMaster()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult WindUserRegister()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {
                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }

        public IActionResult ImportApproval()
        {

            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }

            return View();
        }
        public async Task<IActionResult> WindNewUserRegister(string fname, string useremail, string role, string created_on)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                var url = "http://localhost:23835/api/Login/WindUserRegistration?fname=" + fname + "&useremail=" + useremail + "&role=" + role + "&created_on=" + created_on + "";
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
        public async Task<IActionResult> GetWindUserInfo(int login_id)
        {
            string line = "";
            try
            {
                //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/Login/WindUserRegistration?fname=" + fname + "&useremail="+ useremail + "&site="+ site + "&role="+ role + "&pages="+ pages + "&reports="+ reports + "&read="+ read + "&write="+ write + "";
                var url = "http://localhost:23835/api/Login/GetWindUserInformation?login_id=" + login_id;
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
        public IActionResult WindUserView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarGenView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarDailyTargetKPIView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarDailyLoadSheddingView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarMonthlyTargetKPIView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarMonthlyLinelossView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarMonthlyJMRView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarAcDcCapacityView()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        // Report Routs
        public IActionResult SolarGenReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarBDReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarSiteMaster()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarLocationMaster()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public IActionResult SolarPRReport()
        {
            String Name = HttpContext.Session.GetString("DisplayName");

            if (Name == "Sujit")
            {
                TempData["name"] = "Sujit Kumar";
                TempData["role"] = "User";
                TempData["userid"] = "1";

            }
            if (Name == "prashant")
            {

                TempData["name"] = "Prashant Shetye";
                TempData["role"] = "Admin";
                TempData["userid"] = "2";
            }
            return View();
        }
        public ActionResult WindUserDetails(string id)
        {
            return RedirectToAction("WindUserView", new { id });
        }
        public async Task<ActionResult> Logout(string username, string pass)
        {


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