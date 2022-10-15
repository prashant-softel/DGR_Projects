using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DGRA.Models;
using Newtonsoft.Json;

namespace DGRA.Controllers
{
    public class SolarUploadController : Controller
    {
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Upload(string da)
        {
            HttpPostedFileBase file = Request.Files["Path"];
            DataSet ds = Exceldatareader(file);
            TempData["ds"] = ds;
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingFilegeneration> addSolarUploadingFilegenerations = new List<SolarUploadingFilegeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFilegeneration addSolarUploadingFilegeneration = new SolarUploadingFilegeneration();
                    addSolarUploadingFilegeneration.date = Convert.ToString(dr["date"]);
                    addSolarUploadingFilegeneration.site = Convert.ToString(dr["site"]);
                    addSolarUploadingFilegeneration.inverter = Convert.ToString(dr["inverter"]);
                    addSolarUploadingFilegeneration.inv_act = Convert.ToDecimal(dr["INV_Act (KWh)"]);
                    addSolarUploadingFilegeneration.plant_act = Convert.ToDecimal(dr["Plant_Act (kWh)"]);
                    addSolarUploadingFilegeneration.pi = Convert.ToString(dr["PI (%)"]);
                    addSolarUploadingFilegenerations.Add(addSolarUploadingFilegeneration);
                }

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("http://localhost:23835/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var content = new StringContent(JsonConvert.SerializeObject(addSolarUploadingFilegenerations), Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/DGR/InsertSolarUploadingFilegeneration", content);
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.status = "successfully inserted";
                    return View();
                }
                else
                {
                    return View();
                }
            }
            return View();
        }

        public DataSet Exceldatareader(HttpPostedFileBase file)
        {
            DataSet ds = new DataSet();
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName);
            if (allowedExtensions.Contains(ext))
            {
                try
                {
                    if (!Directory.Exists(Server.MapPath(@"~" + @"\TempFile")))
                    {
                        DirectoryInfo dinfo = Directory.CreateDirectory(Server.MapPath(@"~" + @"\TempFile"));
                    }
                    else
                    {
                        string[] filePaths = Directory.GetFiles(Server.MapPath(@"~" + @"\TempFile"));
                        if (filePaths.Length > 0)
                        {
                            foreach (String path in filePaths)
                            {
                                System.IO.File.Delete(path);
                            }
                        }
                    }
                    if (ext == ".xlsx")
                    {
                        try
                        {
                            file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xlsx"));
                            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(@"~" + @"\TempFile\docupload.xlsx") + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            OleDbConnection oconn = new System.Data.OleDb.OleDbConnection(connectionString);

                            string sql = "SELECT * FROM [Sheet1$]";
                            OleDbCommand cmd = new OleDbCommand(sql, oconn);

                            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                            da.Fill(ds);



                        }
                        catch (Exception ex)
                        {
                            TempData["notification"] = "Something went wrong" + ex.ToString();

                        }
                    }
                    if (ext == ".xls")
                    {
                        try
                        {
                            file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xls"));

                            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(@"~" + @"\TempFile\docupload.xls") + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            OleDbConnection oconn = new System.Data.OleDb.OleDbConnection(connectionString);
                            oconn.Open();

                            string sql = "SELECT * FROM [Sheet1$]";
                            OleDbCommand cmd = new OleDbCommand(sql, oconn);

                            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                            da.Fill(ds);
                        }
                        catch (Exception ex)
                        {
                            TempData["notification"] = "Something went wrong" + ex.ToString();

                        }

                    }
                }
                catch
                {
                    TempData["notification"] = "Something went wrong";
                }
            }
            else
            {
                TempData["notification"] = "File format not supported";
            }
            return ds;
        }
    }
}