using DGRA.Common;
using DGRAPIs.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DGRA.Areas.admin.Controllers
{
    public class FileUploadController : Controller
    {
        static string[] importData = new string[2];
        //WindUploadingFileValidation m_ValidationObject;
        ErrorLog m_ErrorLog;
        ImportLog meta = new ImportLog();

        Hashtable equipmentId = new Hashtable();
        Hashtable siteName = new Hashtable();
        Hashtable siteNameId = new Hashtable();
        Hashtable breakdownType = new Hashtable();

        public FileUploadController()
        {
            m_ErrorLog = new ErrorLog(meta);
        }

        ~FileUploadController()
        {
            //Destructor
            //delete m_ValidationObject;
            //delete m_ErrorLog;
        }

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
                string response = await ExceldatareaderAndUpload(Request.Files["Path"], fileUpload);
                TempData["notification"] = response;
            }
            catch (Exception ex)
            {
                string sMessage = ex.Message;
                TempData["notification"] = "Failed to upload. Reason : " + sMessage;
            }
            return View();
        }
        public async Task<string> ExceldatareaderAndUpload(HttpPostedFileBase file, string fileUpload)
        {
            importData[0] = fileUpload;
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
                        string tempName = file.FileName;
                        importData[1] = Server.MapPath("/" + tempName);
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
                            m_ErrorLog.SetInformation("Status,Message");
                            m_ErrorLog.SetInformation(",File Import Initiated:");
                            file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xlsx"));
                            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(@"~" + @"\TempFile\docupload.xlsx") + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            List<string> fileSheets = new List<string>();
                            oconn = new OleDbConnection(connectionString);
                            oconn.Open();
                            //The following dataTable will collect the names of all the worksheets present in the uploaded excel workbook
                            DataTable dt = null;
                            dt = oconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            // Add the sheet name to the string array.

                            foreach (DataRow row in dt.Rows)
                            {
                                fileSheets.Add(row["TABLE_NAME"].ToString());
                            }

                            //Since JMR imports are also of wind type, they initiate unneeded api calls... Conditioned wind api calls for hashtables to check for fileSheetType along with fileUploadType before initiating call
                            if (fileSheets.Contains("Uploading_File_Generation$") || fileSheets.Contains("Uploading_File_Breakdown$"))
                            {
                                if (fileUpload == "Wind")
                                {
                                    DataTable dTable = new DataTable();
                                    var url = "http://localhost:23835/api/DGR/GetWindLocationMaster";
                                    var result = string.Empty;
                                    WebRequest request = WebRequest.Create(url);

                                    using (var response = (HttpWebResponse)request.GetResponse())
                                    {
                                        Stream receiveStream = response.GetResponseStream();
                                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                                        {
                                            result = readStream.ReadToEnd();
                                        }
                                        dTable = JsonConvert.DeserializeObject<DataTable>(result);
                                    }

                                    foreach (DataRow dr in dTable.Rows)
                                    {
                                        equipmentId.Add((string)dr["wtg"], Convert.ToString(dr["location_master_id"]));
                                        siteName.Add((string)dr["wtg"], (string)dr["site"]);
                                        siteNameId.Add((string)dr["wtg"], Convert.ToString(dr["site_master_id"]));
                                    }

                                    url = "http://localhost:23835/api/DGR/GetBDType";
                                    result = string.Empty;
                                    request = WebRequest.Create(url);

                                    using (var response = (HttpWebResponse)request.GetResponse())
                                    {
                                        Stream receiveStream = response.GetResponseStream();
                                        using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                                        {
                                            result = readStream.ReadToEnd();
                                        }
                                        dTable = JsonConvert.DeserializeObject<DataTable>(result);
                                    }

                                    foreach (DataRow dr in dTable.Rows)
                                    {
                                        breakdownType.Add((string)dr["bd_type_name"], Convert.ToInt64(dr["bd_type_id"]));
                                    }
                                }
                            }

                            foreach (var excelSheet in fileSheets)
                            {
                                DataSet ds = new DataSet();
                                string sql = "";
                                if (excelSheet == FileSheetType.Uploading_File_Generation)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Generation + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_File_Generation :");
                                            status = await InsertSolarFileGeneration(status, ds);
                                        }
                                        else if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind_Uploading_File_Generation Worksheet :");
                                            status = await InsertWindFileGeneration(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Wind Generation Import Successful:");
                                                m_ErrorLog.SaveToCSV(importData[1]);
                                                m_ErrorLog.Clear();
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Generation Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Generation Import:");
                                                m_ErrorLog.SaveToCSV(importData[1]);
                                                m_ErrorLog.Clear();
                                            }
                                        }
                                        else //add new import types here
                                        {
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_File_Breakdown)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Breakdown + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_File_Breakdown :");

                                            status = await InsertSolarFileBreakDown(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind_Uploading_File_Breakdown :");
                                            status = await InsertWindFileBreakDown(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Wind BreakDown Import Successful:");
                                                m_ErrorLog.SaveToCSV(importData[1]);
                                                m_ErrorLog.Clear();
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind BreakDown Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind BreakDown Import:");
                                                m_ErrorLog.SaveToCSV(importData[1]);
                                                m_ErrorLog.Clear();
                                            }

                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter1Min)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_PyranoMeter1Min + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter1Min: ");
                                            status = await InsertSolarPyranoMeter1Min(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter15Min)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Uploading_PyranoMeter15Min + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter15Min :");
                                            status = await InsertSolarPyranoMeter15Min(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_JMR_Input_and_Output)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Monthly_JMR_Input_and_Output + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind: Monthly_JMR_Input_and_Output :");
                                            status = await InsertWindMonthly_JMR_Input_and_Output(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Load_Shedding)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Daily_Load_Shedding + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind : Daily_Load_Shedding :");
                                            status = await InsertWindDaily_Load_Shedding(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                            }
                            if (status == "Successfully uploaded")
                            {
                                m_ErrorLog.SetInformation(",Import Operation Complete :");
                                await importMetaData(importData[0], importData[1]);
                            }

                        }
                        catch (Exception ex)
                        {
                            m_ErrorLog.SetError(",Import Operation Failed :");
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
        private static async Task<string> InsertSolarFileGeneration(string status, DataSet ds)
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
        private async Task<string> InsertWindFileGeneration(string status, DataSet ds)
        {
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            WindUploadingFileValidation ValidationObject = new WindUploadingFileValidation(m_ErrorLog);

            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFilegeneration> addWindUploadingFilegenerations = new List<WindUploadingFilegeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rowNumber++;
                    WindUploadingFilegeneration addWindUploadingFilegeneration = new WindUploadingFilegeneration();
                    addWindUploadingFilegeneration.wtg = Convert.ToString(dr["WTG"]);
                    addWindUploadingFilegeneration.site_name = (string)(siteName[addWindUploadingFilegeneration.wtg]);
                    addWindUploadingFilegeneration.site_id = Convert.ToInt32(siteNameId[addWindUploadingFilegeneration.wtg]);
                    addWindUploadingFilegeneration.date = Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy");
                    addWindUploadingFilegeneration.wtg_id = Convert.ToInt32(equipmentId[addWindUploadingFilegeneration.wtg]);
                    addWindUploadingFilegeneration.wind_speed = Convert.ToDecimal(dr["Wind_Speed"]);
                    addWindUploadingFilegeneration.kwh = Convert.ToDecimal(dr["kWh"]);
                    addWindUploadingFilegeneration.operating_hrs = Convert.ToDecimal(dr["Gen_Hrs"]);
                    addWindUploadingFilegeneration.production_rs = Convert.ToDecimal(dr["Lull_Hrs"]);
                    addWindUploadingFilegeneration.grid_hrs = Convert.ToDecimal(dr["Grid_Hrs"]);

                    errorFlag = ValidationObject.validateGenData(rowNumber, addWindUploadingFilegeneration.date, addWindUploadingFilegeneration.wtg, addWindUploadingFilegeneration.wind_speed, addWindUploadingFilegeneration.kwh, addWindUploadingFilegeneration.operating_hrs, addWindUploadingFilegeneration.production_rs, addWindUploadingFilegeneration.grid_hrs);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addWindUploadingFilegenerations.Add(addWindUploadingFilegeneration);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addWindUploadingFilegenerations);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = "http://localhost:23835/api/DGR/InsertWindUploadingFilegeneration";

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
                else
                {
                    status = "Failed to upload";
                }

                equipmentId.Clear();
                siteName.Clear();
                siteNameId.Clear();
            }

            return status;
        }

        private static async Task<string> InsertSolarFileBreakDown(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingFileBreakDown> addSolarUploadingFileBreakDowns = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFileBreakDown addSolarUploadingFileBreakdown = new SolarUploadingFileBreakDown();
                    addSolarUploadingFileBreakdown.date = Convert.ToString(dr["Date"]);
                    addSolarUploadingFileBreakdown.site = Convert.ToString(dr["Site"]);
                    addSolarUploadingFileBreakdown.ext_int_bd = Convert.ToString(dr["IGBD"]);
                    addSolarUploadingFileBreakdown.icr = Convert.ToString(dr["ICR"]);
                    addSolarUploadingFileBreakdown.inv = Convert.ToString(dr["INV"]);
                    addSolarUploadingFileBreakdown.smb = Convert.ToString(dr["SMB"]);
                    addSolarUploadingFileBreakdown.strings = Convert.ToString(dr["Strings"]);
                    addSolarUploadingFileBreakdown.from_bd = Convert.ToString(dr["From"]);
                    addSolarUploadingFileBreakdown.to_bd = Convert.ToString(dr["To"]);
                    addSolarUploadingFileBreakdown.bd_remarks = Convert.ToString(dr["BDRemarks"]);
                    addSolarUploadingFileBreakdown.bd_type = Convert.ToString(dr["BDType"]);
                    addSolarUploadingFileBreakdown.action_taken = Convert.ToString(dr["ActionTaken"]);
                    addSolarUploadingFileBreakDowns.Add(addSolarUploadingFileBreakdown);
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

        private async Task<string> InsertWindFileBreakDown(string status, DataSet ds)
        {
            WindUploadingFileValidation ValidationObject = new WindUploadingFileValidation(m_ErrorLog);
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileBreakDown> addWindUploadingFileBreakDowns = new List<WindUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindUploadingFileBreakDown addWindUploadingFileBreakDown = new WindUploadingFileBreakDown();
                    addWindUploadingFileBreakDown.date = Convert.ToDateTime(dr["Date"]).ToString("dd-MM-yyyy");

                    addWindUploadingFileBreakDown.wtg = Convert.ToString(dr["WTG"]);
                    addWindUploadingFileBreakDown.site_name = (string)siteName[addWindUploadingFileBreakDown.wtg];
                    addWindUploadingFileBreakDown.site_id = Convert.ToInt32(siteNameId[addWindUploadingFileBreakDown.wtg]);
                    addWindUploadingFileBreakDown.wtg_id = Convert.ToInt32(equipmentId[addWindUploadingFileBreakDown.wtg]);

                    addWindUploadingFileBreakDown.bd_type = Convert.ToString(dr["BD_Type"]);
                    addWindUploadingFileBreakDown.bd_type_id = Convert.ToInt32(breakdownType[addWindUploadingFileBreakDown.bd_type]);

                    addWindUploadingFileBreakDown.stop_from = Convert.ToString(dr["Stop From"]);
                    addWindUploadingFileBreakDown.stop_to = Convert.ToString(dr["Stop To"]);
                    addWindUploadingFileBreakDown.total_stop = ValidationObject.breakDownCalc(addWindUploadingFileBreakDown.stop_from, addWindUploadingFileBreakDown.stop_to);

                    addWindUploadingFileBreakDown.error_description = Convert.ToString(dr["Error description"]);
                    addWindUploadingFileBreakDown.action_taken = Convert.ToString(dr["Action Taken"]);

                    errorFlag = ValidationObject.validateBreakDownData(rowNumber, addWindUploadingFileBreakDown.bd_type, addWindUploadingFileBreakDown.stop_from, addWindUploadingFileBreakDown.stop_to);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addWindUploadingFileBreakDown.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("hh:mm:ss");
                    addWindUploadingFileBreakDown.stop_to = Convert.ToDateTime(dr["Stop To"]).ToString("hh:mm:ss");
                    addWindUploadingFileBreakDowns.Add(addWindUploadingFileBreakDown);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addWindUploadingFileBreakDowns);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = "http://localhost:23835/api/DGR/InsertWindUploadingFileBreakDown";
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
                else
                {
                    status = "Failed to upload";
                }
                equipmentId.Clear();
                siteName.Clear();
                siteNameId.Clear();
            }
            return status;
        }

        private static async Task<string> InsertSolarPyranoMeter1Min(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter1Min> addSet = new List<SolarUploadingPyranoMeter1Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingPyranoMeter1Min adUnit = new SolarUploadingPyranoMeter1Min();
                    adUnit.date_time = Convert.ToString(dr["Time stamp"]);
                    adUnit.ghi_1 = Convert.ToString(dr["GHI-1"]);
                    adUnit.ghi_2 = Convert.ToString(dr["GHI-2"]);
                    adUnit.poa_1 = Convert.ToString(dr["POA-1"]);
                    adUnit.poa_2 = Convert.ToString(dr["POA-2"]);
                    adUnit.poa_3 = Convert.ToString(dr["POA-3"]);
                    adUnit.poa_4 = Convert.ToString(dr["POA-4"]);
                    adUnit.poa_5 = Convert.ToString(dr["POA-5"]);
                    adUnit.poa_6 = Convert.ToString(dr["POA-6"]);
                    adUnit.poa_7 = Convert.ToString(dr["POA-7"]);
                    adUnit.avg_ghi = Convert.ToDecimal(dr["Average GHI (w/m²)"]);
                    adUnit.avg_poa = Convert.ToDecimal(dr["Average POA (w/m²)"]);
                    addSet.Add(adUnit);
                }

                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "http://localhost:23835/api/DGR/InsertSolarUploadingPyranoMeter1Min";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        //importMetaData(importData[0], importData[1],"Developer");
                    }
                    else
                    {
                        status = "Failed to upload";
                    }
                }
            }

            return status;
        }

        private static async Task<string> InsertSolarPyranoMeter15Min(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter15Min> addSet = new List<SolarUploadingPyranoMeter15Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingPyranoMeter15Min addUnit = new SolarUploadingPyranoMeter15Min();
                    addUnit.date_time = Convert.ToString(dr["Time stamp"]);
                    addUnit.ghi_1 = Convert.ToDecimal(dr["GHI-1"]);
                    addUnit.ghi_2 = Convert.ToDecimal(dr["GHI-2"]);
                    addUnit.poa_1 = Convert.ToDecimal(dr["POA-1"]);
                    addUnit.poa_2 = Convert.ToDecimal(dr["POA-2"]);
                    addUnit.poa_3 = Convert.ToDecimal(dr["POA-3"]);
                    addUnit.poa_4 = Convert.ToDecimal(dr["POA-4"]);
                    addUnit.poa_5 = Convert.ToDecimal(dr["POA-5"]);
                    addUnit.poa_6 = Convert.ToDecimal(dr["POA-6"]);
                    addUnit.poa_7 = Convert.ToDecimal(dr["POA-7"]);
                    addUnit.avg_ghi = Convert.ToDecimal(dr["Average GHI (w/m²)"]);
                    addUnit.avg_poa = Convert.ToDecimal(dr["Average POA (w/m²)"]);
                    addUnit.amb_temp = Convert.ToDecimal(dr["Ambient Temp"]);
                    addUnit.mod_temp = Convert.ToDecimal(dr["Module Temp"]);
                    addSet.Add(addUnit);
                }

                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "http://localhost:23835/api/DGR/InsertSolarUploadingPyranoMeter15Min";
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

        private async Task<string> InsertWindMonthly_JMR_Input_and_Output(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyJMR> addSet = new List<WindMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyJMR addUnit = new WindMonthlyJMR();
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.plantSection = Convert.ToString(dr["Plant Section"]);

                    if ((dr["JMR date"]) is DBNull) { addUnit.jmrDate = "0000-00-00"; }
                    else { addUnit.jmrDate = Convert.ToDateTime(dr["JMR date"]).ToString("yyyy-MM-dd"); }

                    addUnit.jmrMonth = Convert.ToString(dr["JMR Month"]);
                    addUnit.jmrYear = Convert.ToString(dr["JMR Year"]);
                    addUnit.lineLossPercent = Convert.ToString(dr["Line Loss%"]);
                    addUnit.rkvhPercent = Convert.ToString(dr["RKVH%"]);

                    if (dr["Controller KWH/INV KWH"] is DBNull) { addUnit.controllerKwhInv = 0; }
                    else { addUnit.controllerKwhInv = Convert.ToDecimal(dr["Controller KWH/INV KWH"]); }

                    if (dr["Scheduled Units  (kWh)"] is DBNull) { addUnit.scheduledUnitsKwh = 0; }
                    else { addUnit.scheduledUnitsKwh = Convert.ToDecimal(dr["Scheduled Units  (kWh)"]); }

                    if (dr["Export (kWh)"] is DBNull) { addUnit.exportKwh = 0; }
                    else { addUnit.exportKwh = Convert.ToDecimal(dr["Export (kWh)"]); }

                    if (dr["Import (kWh)"] is DBNull) { addUnit.importKwh = 0; }
                    else { addUnit.importKwh = Convert.ToDecimal(dr["Import (kWh)"]); }

                    if (dr["Net Export (kWh)"] is DBNull) { addUnit.netExportKwh = 0; }
                    else { addUnit.netExportKwh = Convert.ToDecimal(dr["Net Export (kWh)"]); }

                    if (dr["Export (kVAh)"] is DBNull) { addUnit.exportKvah = 0; }
                    else { addUnit.exportKvah = Convert.ToDecimal(dr["Export (kVAh)"]); }

                    if (dr["Import (kVAh)"] is DBNull) { addUnit.importKvah = 0; }
                    else { addUnit.importKvah = Convert.ToDecimal(dr["Import (kVAh)"]); }

                    if (dr["Export (kVArh lag)"] is DBNull) { addUnit.exportKvarhLag = 0; }
                    else { addUnit.exportKvarhLag = Convert.ToDecimal(dr["Export (kVArh lag)"]); }

                    if (dr["Import (kVArh lag)"] is DBNull) { addUnit.importKvarhLag = 0; }
                    else { addUnit.importKvarhLag = Convert.ToDecimal(dr["Import (kVArh lag)"]); }

                    if (dr["Export (kVArh lead)"] is DBNull) { addUnit.exportKvarhLead = 0; }
                    else { addUnit.exportKvarhLead = Convert.ToDecimal(dr["Export (kVArh lead)"]); }

                    if (dr["Import (kVArh lead)"] is DBNull) { addUnit.importKvarhLead = 0; }
                    else { addUnit.importKvarhLead = Convert.ToDecimal(dr["Import (kVArh lead)"]); }

                    if (dr["LineLoss"] is DBNull) { addUnit.lineLoss = 0; }
                    else { addUnit.lineLoss = Convert.ToDecimal(dr["LineLoss"]); }
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "http://localhost:23835/api/DGR/InsertWindJMR";
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
        private async Task<string> InsertWindDaily_Load_Shedding(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<WindDailyLoadShedding> addSet = new List<WindDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyLoadShedding addUnit = new WindDailyLoadShedding();
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.startTime = Convert.ToDateTime(dr["Start Time"]).ToString("hh-mm-ss");
                    addUnit.endTime = Convert.ToDateTime(dr["End Time"]).ToString("hh-mm-ss");
                    addUnit.totalTime = Convert.ToDateTime(dr["Total Time"]).ToString("HH-mm-ss");
                    addUnit.permLoad = Convert.ToString(dr[" Permissible Load (MW)"]);
                    addUnit.genShedding = Convert.ToString(dr["Generation loss in KWH due to Load shedding"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = "http://localhost:23835/api/DGR/InsertWindDailyLoadShedding";
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
        private async Task<string> importMetaData(string importType, string fileName)
        {
            meta.importFilePath = fileName;
            if (importType == "Solar")
            {
                meta.importType = "1";
            }
            else if (importType == "Wind")
            {
                meta.importType = "2";
            }
            var json = JsonConvert.SerializeObject(meta);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = "http://localhost:23835/api/DGR/importMetaData";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, data);
            }
            string status = "Logged activity successfully";
            return status;
        }
    }
}