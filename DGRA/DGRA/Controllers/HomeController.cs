using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using DGRAPIs.Models;
using System.Net;

namespace DGRA.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        public ActionResult Index1()
        {
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        public ActionResult Dashbord()
        {
            //return RedirectToAction("Dashbord", "Home");
            return View();
            //return RedirectToAction("Upload", "FileUpload");
        }
        public async Task<ActionResult> Login(string username, string pass)
        {
            string status = "";
            bool login_status = false;
            LoginModel model = new LoginModel();
            System.Collections.Generic.Dictionary<string, object>[] map = new System.Collections.Generic.Dictionary<string, object>[1];
            try
            {
                var url = "http://localhost:23835/api/Login/UserLogin?username="+username+"&password="+pass+"";
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
                            model = JsonConvert.DeserializeObject<LoginModel>(line);
                           
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
                Session["UserName"] = model.name;
                Session["UserID"] = model.Username;
                Session["UserRole"] = model.user_role;
                Session["LoginID"] = model.login_id;

            }
            else
            {
                map[0].Add("UserName", "");
                map[0].Add("UserID", "");
                map[0].Add("UserRole","");
                map[0].Add("LoginID", "");
                map[0].Add("status", "0");
            }
            return Json(map, JsonRequestBehavior.AllowGet);
            // return model;
            //return RedirectToAction("Dashbord", "Home");
        }

        // Wind Views
        public ActionResult WindGenView()
        {
           // return RedirectToAction("WindGenView", "Home");
            
            return View();
        }
        public ActionResult WindDailyTargetKPIView()
        {
            //return RedirectToAction("WindDailyTargetKPIView", "ReportViews");
            return View();
        }
        public ActionResult WindMonthlyTargetKPIView()
        {
            //return RedirectToAction("WindMonthlyTargetKPIView", "ReportViews");
            return View();
        }
        public ActionResult WindMonthlyLinelossView()
        {
            //return RedirectToAction("WindMonthlyLinelossView", "ReportViews");
            return View();
        }
        public ActionResult WindJMRView()
        { 
           // return RedirectToAction("WindJMRView", "ReportViews");
            return View();
        }
        public ActionResult WindDailyLoadSheddingView()
        {
           // return RedirectToAction("WindDailyLoadSheddingView", "ReportViews");
            return View();
        }
        public async Task<ActionResult> Logout(string username, string pass)
        {
    
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            //Response.Redirect("somepage.aspx");
            return RedirectToAction("Index", "Home");
           // return View();
        }
    }
 }