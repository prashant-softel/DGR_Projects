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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IDapperRepository _idapperRepo;



        public object GetWindDailyGenSummary { get; private set; }
        public JsonSerializerOptions _options { get; private set; }
  // TEST A 123456789
        public HomeController(ILogger<HomeController> logger, IDapperRepository idapperRepo)
        {
            _logger = logger;
            _idapperRepo = idapperRepo;
   

        private readonly GraphServiceClient _graphServiceClient;


        public HomeController(ILogger<HomeController> logger,
                         GraphServiceClient graphServiceClient)
        {
            _logger = logger;
            _graphServiceClient = graphServiceClient;
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
            var user = await _graphServiceClient.Me.Request().GetAsync();


            HttpContext.Session.SetString("DisplayName", user.DisplayName);

            ViewData["ApiResult"] = user.DisplayName;
            if (!string.IsNullOrEmpty(user.DisplayName))
            {
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
            return View();
        }
        public IActionResult WindGenView()
        {
            return View();
        }
        public IActionResult WindMonthlyTargetKPIView()
        {
            return View();
        }
        public IActionResult WindMonthlyLinelossView()
        {
            return View();
        }
        public IActionResult WindMonthlyJMRView()
        {
            return View();
        }
        public IActionResult WindDailyLoadSheddingView()
        {
            return View();
        }

        // Report Routs
        public IActionResult WindSiteMaster()
        {
            return View();
        }
        public IActionResult WIndLocationMaster()
        {
            return View();
        }
        public IActionResult WindGenReport()
        {
            return View();
        }
        public IActionResult WindBDReport()
        {
            return View();
        }
        public IActionResult WindPRReport()
        {
            return View();
        }

        public IActionResult ImportApproval()
        {
            return View();
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