
using DGRAPIs.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DGRA.Common;
namespace DGRA.Areas.admin.Controllers
{
    public class FileUploadController : Controller
    {
        [HttpGet]
        public ActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Upload(string fileUpload)
        {
            try
            {
                string response = await ExceldatareaderAndUpload(Request.Files["Path"]);
                TempData["notification"] = response;
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Failed to upload";
            }
            return View();
        }

        public async Task<string> ExceldatareaderAndUpload(HttpPostedFileBase file)
        {
            OleDbConnection oconn = null;
            string status = "";
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
                             oconn = new System.Data.OleDb.OleDbConnection(connectionString);

                            foreach (var excelSheet in FileSheetType.All)
                            {
                                DataSet ds = new DataSet();
                                string sql = "";
                                if (excelSheet == FileSheetType.Uploading_File_Generation)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Generation + "$]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    if (ds.Tables.Count > 0)
                                    {
                                        status = await InsertFileGeneration(status, ds);
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_File_Breakdown)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Breakdown + "$]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    if (ds.Tables.Count > 0)
                                    {
                                        //status = await InsertFileBreakDown(status, ds);
                                    }
                                }
                            }
                              
                        }
                        catch (Exception ex)
                        {
                            status = "Something went wrong" + ex.ToString();
                         
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "Something went wrong" + ex.ToString();
               
                }
            }
            else
            {
                status = "File format not supported";
            }
            return status;
        }

        private static async Task<string> InsertFileGeneration(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingFilegeneration> addSolarUploadingFilegenerations = new List<SolarUploadingFilegeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFilegeneration addSolarUploadingFilegeneration = new SolarUploadingFilegeneration();
                    addSolarUploadingFilegeneration.date = Convert.ToString(dr["DATE"]);
                    addSolarUploadingFilegeneration.site = Convert.ToString(dr["Site"]);
                    addSolarUploadingFilegeneration.inverter = Convert.ToString(dr["INVERTER"]);
                    addSolarUploadingFilegeneration.inv_act = Convert.ToDecimal(dr["INV_Act(KWh)"]);
                    addSolarUploadingFilegeneration.plant_act = Convert.ToDecimal(dr["Plant_Act(kWh)"]);
                    addSolarUploadingFilegeneration.pi = Convert.ToString(dr["PI(%)"]);
                    addSolarUploadingFilegenerations.Add(addSolarUploadingFilegeneration);
                }

                var json = JsonConvert.SerializeObject(addSolarUploadingFilegenerations);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "http://localhost:23835/api/DGR/InsertSolarUploadingFilegeneration";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                    }
                    else
                    {
                        status = "Failed to upload";
                    }
                }
            }

            return status;
        }
        private static async Task<string> InsertFileBreakDown(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingFileBreakDown> addSolarUploadingFileBreakDowns = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFileBreakDown addSolarUploadingFilegeneration = new SolarUploadingFileBreakDown();
                    addSolarUploadingFilegeneration.date = Convert.ToString(dr["Date"]);
                    addSolarUploadingFilegeneration.site = Convert.ToString(dr["Site"]);
                    addSolarUploadingFilegeneration.ext_int_bd = Convert.ToString(dr["IGBD"]);
                    addSolarUploadingFilegeneration.icr = Convert.ToString(dr["ICR"]);
                    addSolarUploadingFilegeneration.inv = Convert.ToString(dr["INV"]);
                    addSolarUploadingFilegeneration.smb = Convert.ToString(dr["SMB"]);
                    addSolarUploadingFilegeneration.strings = Convert.ToString(dr["Strings"]);
                    addSolarUploadingFilegeneration.from_bd = Convert.ToString(dr["From"]);
                    addSolarUploadingFilegeneration.to_bd = Convert.ToString(dr["To"]);
                    addSolarUploadingFilegeneration.bd_remarks = Convert.ToString(dr["BDRemarks"]);
                    addSolarUploadingFilegeneration.bd_type = Convert.ToString(dr["BDType"]);
                    addSolarUploadingFilegeneration.action_taken = Convert.ToString(dr["ActionTaken"]);
                    addSolarUploadingFileBreakDowns.Add(addSolarUploadingFilegeneration);
                }

                var json = JsonConvert.SerializeObject(addSolarUploadingFileBreakDowns);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = "http://localhost:23835/api/DGR/InsertSolarUploadingFileBreakDown";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                    }
                    else
                    {
                        status = "Failed to upload";
                    }
                }
            }

            return status;
        }
    }
}