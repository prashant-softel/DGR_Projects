using DGRA_V1.Common;
using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Directory = System.IO.Directory;

namespace DGRA_V1.Controllers
{
    [Area("admin")]
    //  [Authorize]
    public class DailySummaryController : Controller
    {
        //ImportBatch objImportBatch = new ImportBatch();
        private IDapperRepository _idapperRepo;
        private IWebHostEnvironment env;
        private static IHttpContextAccessor HttpContextAccessor;
        CultureInfo timeCulture = CultureInfo.InvariantCulture;
        public DailySummaryController(IDapperRepository idapperobj, IWebHostEnvironment obj, IHttpContextAccessor httpObj)
        {
            HttpContextAccessor = httpObj;
            _idapperRepo = idapperobj;
            m_ErrorLog = new ErrorLog(obj);
            env = obj;
        }
        //static int batchIdDGRAutomation = 0;
        string siteUserRole;
        //int previousSite = 0;
        static string[] importData = new string[2];
        //static bool isGenValidationSuccess = false;
        //static bool isBreakdownValidationSuccess = false;
        //static bool isPyro1ValidationSuccess = false;
        //static bool isPyro15ValidationSuccess = false;
        //string genJson = string.Empty;
        //string breakJson = string.Empty;
        //string pyro1Json = string.Empty;
        //string pyro15Json = string.Empty;
        //string windGenJson = string.Empty;
        //string windBreakJson = string.Empty;

        //ArrayList kpiArgs = new ArrayList();
        List<int> windSiteUserAccess = new List<int>();
        List<string> fileSheets = new List<string>();
        //List<string> inverterList = new List<string>();
        ErrorLog m_ErrorLog;

        //Hashtable equipmentId = new Hashtable();
        //Hashtable breakdownType = new Hashtable();//(B)Gets bdTypeID from bdTypeName: BDType table
        //Hashtable siteNameId = new Hashtable(); //(C)Gets siteId from siteName
        //Hashtable siteName = new Hashtable(); //(D)Gets siteName from siteId

        //Hashtable eqSiteId = new Hashtable();//(E)Gets siteId from (wtg)equipmentName
        //Hashtable MonthList = new Hashtable() { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
        //Hashtable longMonthList = new Hashtable() { { "January", 1 }, { "February", 2 }, { "March", 3 }, { "April", 4 }, { "May", 5 }, { "June", 6 }, { "July", 7 }, { "August", 8 }, { "September", 9 }, { "October", 10 }, { "November", 11 }, { "December", 12 } };

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
                //  string response = await ExceldatareaderAndUpload(Request.Files["Path"]);
                string response = await ExcelDataReaderAndUpload(HttpContext.Request.Form.Files[0], fileUpload);
                TempData["notification"] = response;
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Failed to upload";
                string message = ex.Message;
            }
            return View();
        }

        public async Task<string> ExcelDataReaderAndUpload(IFormFile file, string fileUploadType)
        {
            var usermodel = JsonConvert.DeserializeObject<UserAccess>(@HttpContextAccessor.HttpContext.Session.GetString("UserAccess"));
            for (int i = 0; i < usermodel.access_list.Count; i++)
            {
                if (usermodel.access_list[i].page_type == 3 && usermodel.access_list[i].site_type == 1)
                {
                    windSiteUserAccess.Add(Convert.ToInt32(usermodel.access_list[i].identity));
                }
            }
            //windSiteList = HttpContextAccessor.HttpContext.Session.GetString("UserAccess");
            siteUserRole = HttpContext.Session.GetString("role");
            DateTime today = DateTime.Now;
            // string csvFileName = env.ContentRootPath +@"\LogFile\"+ file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            string csvFileName = file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            importData[0] = fileUploadType;
            importData[1] = csvFileName;
            OleDbConnection oconn = null;
            string status = "";
            int statusCode = 400;
            DataSet dataSetMain = new DataSet();
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName);

            if (allowedExtensions.Contains(ext))
            {
                try
                {
                    if (!Directory.Exists(@"\TempFile"))
                    {
                        DirectoryInfo dinfo = Directory.CreateDirectory(@"\TempFile");
                    }
                    else
                    {
                        string[] filePaths = Directory.GetFiles(@"\TempFile");
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
                            /* using (var stream = new FileStream(env.ContentRootPath + @"\TempFile\docupload.xlsx", FileMode.Create))
                             {
                                 file.CopyTo(stream);
                             }*/
                            using (var stream = new FileStream(@"\TempFile\docupload.xlsx", FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }
                            DataTable dt = null;

                            // string _filePath = @"C:\TempFile\docupload.xlsx";
                            string _filePath = @"E:\TempFile\docupload.xlsx";
                            //string _filePath = env.ContentRootPath + @"\TempFile\docupload.xlsx";
                            dataSetMain = GetDataTableFromExcel(_filePath, true, ref fileSheets);
                            if (dataSetMain == null)
                            {
                                m_ErrorLog.SetError(",Unable to extract excel sheet data for importing,");
                            }
                            ErrorLog("datSet Med" + dataSetMain);
                            //if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min"))
                            //{
                            //    masterHashtable_SiteName_To_SiteId();//C
                            //    masterHashtable_BDNameToBDId();//B
                            //    masterHashtable_SiteIdToSiteName();
                            //    if (fileUploadType == "Wind")
                            //    {
                            //        masterHashtable_WtgToWtgId();
                            //        masterHashtable_WtgToSiteId();
                            //    }
                            //    if (fileUploadType == "Solar")
                            //    {
                            //        masterInverterList();
                            //    }
                            //}
                            //else
                            //{
                            //    if (fileUploadType == "Wind")
                            //    {
                            //        masterHashtable_WtgToWtgId();
                            //        masterHashtable_WtgToSiteId();
                            //    }
                            //    if (fileUploadType == "Solar")
                            //    {
                            //        masterInverterList();
                            //    }
                            //    masterHashtable_SiteName_To_SiteId();
                            //    masterHashtable_SiteIdToSiteName();
                            //}
                            //Status Codes:
                            //200 = Success ; 400 = Failure(BadRequest)

                            if (statusCode == 200)
                            {
                                // await UploadFileToImportedFileFolder(file);
                                
                                //if (!(fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min")))
                                //{
                                //    await importMetaData(fileUploadType, file.FileName);
                                //}

                                //DGR Automation Function Logic
                                if (fileUploadType == "Wind")
                                {
                                    //if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown"))
                                    //{
                                    //    if (isGenValidationSuccess || isBreakdownValidationSuccess)
                                    //    {
                                    //        await importMetaData(fileUploadType, file.FileName);
                                    //        statusCode = await dgrWindImport(batchIdDGRAutomation);
                                    //        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailyWindKPI?fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                    //        //remove after testing
                                    //        //m_ErrorLog.SetInformation("Url" + url);
                                    //        using (var client = new HttpClient())
                                    //        {
                                    //            var response = await client.GetAsync(url);
                                    //            //status = "Respose" + response;
                                    //            if (response.IsSuccessStatusCode)
                                    //            {
                                    //                m_ErrorLog.SetInformation(",Wind KPI Calculations Updated Successfully:");
                                    //                statusCode = (int)response.StatusCode;
                                    //                status = "Successfully Uploaded";

                                    //                // Added Code auto approved if uploaded by admin
                                    //                string userName = HttpContext.Session.GetString("DisplayName");
                                    //                int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                    //                siteUserRole = HttpContext.Session.GetString("role");
                                    //                if (siteUserRole == "Admin")
                                    //                {
                                    //                    var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                    //                    using (var client1 = new HttpClient())
                                    //                    {
                                    //                        var response1 = await client1.GetAsync(url1);
                                    //                        if (response1.IsSuccessStatusCode)
                                    //                        {
                                    //                            //status = "Successfully Data Approved";
                                    //                        }
                                    //                        else
                                    //                        {
                                    //                            //status = "Data Not Approved";
                                    //                        }
                                    //                    }
                                    //                }
                                    //            }
                                    //            else
                                    //            {
                                    //                m_ErrorLog.SetError(",Wind KPI Calculations API Failed:");
                                    //                statusCode = (int)response.StatusCode;
                                    //                status = "Wind KPI Calculation Import API Failed";
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    status = "Successfully Uploaded";
                                    //}
                                }

                                else if (fileUploadType == "Solar")
                                {
                                    //if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min"))
                                    //{
                                    //    ErrorLog("isGenValidationSuccess" + isGenValidationSuccess);
                                    //    ErrorLog("isPyro15ValidationSuccess" + isPyro15ValidationSuccess);
                                    //    ErrorLog("isPyro1ValidationSuccess" + isPyro1ValidationSuccess);
                                    //    ErrorLog("Before Validation");
                                    //    //pending : instead check the  success flags
                                    //    if (isGenValidationSuccess && isBreakdownValidationSuccess && isPyro15ValidationSuccess && isPyro1ValidationSuccess)
                                    //    {
                                    //        ErrorLog("Before Metadata");
                                    //        await importMetaData(fileUploadType, file.FileName);
                                    //        ErrorLog("After Metadata");
                                    //        statusCode = await dgrSolarImport(batchIdDGRAutomation);
                                    //        ErrorLog("Status COde :" + statusCode);
                                    //        //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?fromDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                    //        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?site=" + (string)kpiArgs[2] + "&fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "";
                                    //        using (var client = new HttpClient())
                                    //        {
                                    //            var response = await client.GetAsync(url);
                                    //            if (response.IsSuccessStatusCode)
                                    //            {
                                    //                m_ErrorLog.SetInformation(",SolarKPI Calculations Updated Successfully:");
                                    //                statusCode = (int)response.StatusCode;
                                    //                status = "Successfully Uploaded";

                                    //                // Added Code auto approved if uploaded by admin
                                    //                string userName = HttpContext.Session.GetString("DisplayName");
                                    //                int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                    //                siteUserRole = HttpContext.Session.GetString("role");
                                    //                if (siteUserRole == "Admin")
                                    //                {
                                    //                    var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetSolarApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                    //                    using (var client1 = new HttpClient())
                                    //                    {
                                    //                        var response1 = await client1.GetAsync(url1);
                                    //                        if (response1.IsSuccessStatusCode)
                                    //                        {
                                    //                            //status = "Successfully Data Approved";
                                    //                        }
                                    //                        else
                                    //                        {
                                    //                            //status = "Data Not Approved";
                                    //                        }
                                    //                    }
                                    //                }
                                    //            }
                                    //            else
                                    //            {
                                    //                m_ErrorLog.SetError(",SolarKPI Calculations API Failed:");
                                    //                statusCode = (int)response.StatusCode;
                                    //                status = "Solar KPI Calculation Import API Failed";
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    status = "Successfully Uploaded";
                                    //}
                                }
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(",Import Operation Failed:");
                            }
                        }
                        catch (Exception ex)
                        {
                            status = "Exception Caught Debugging Required";
                            ex.GetType();
                            m_ErrorLog.SetError("," + status + ":" + ex.Message);
                            ErrorLog("Inside" + ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "Something went wrong : Exception Caught Debugging Required";
                    //status = status.Substring(0, (status.IndexOf("Exception") + 9));
                    m_ErrorLog.SetError("," + status);
                    ErrorLog("True\r\n" + ex.Message);
                }
            }
            else
            {
                //excel file format condition
                status = "File format not supported";
                m_ErrorLog.SetError("," + status + ":");
            }

            m_ErrorLog.SaveToCSV(csvFileName);
            //if (statusCode != 200)
            ArrayList messageList = m_ErrorLog.errorLog();
            foreach (var item in messageList)
            {
                status += ((string)item).Replace(",", "") + ",";
            }
            return status;
        }
        private DataSet GetDataTableFromExcel(string filePath, bool hasHeader, ref List<string> _worksheetList)
        {
            string status = "";
            try
            {
                DataSet dataSet = new DataSet();
                DataTable dt = new DataTable();
                FileInfo excelFile = new FileInfo(filePath);
                var excel = new ExcelPackage(excelFile);
                foreach (var worksheet in excel.Workbook.Worksheets)
                {
                    if (FileSheetType.sheetList.Contains(worksheet.Name))
                    {
                        _worksheetList.Add(worksheet.Name);
                    }
                }

                // ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage(excelFile))
                {
                    foreach (var li in _worksheetList)
                    {
                        dt = new DataTable();
                        dt.TableName = li;
                        ExcelWorksheet workSheet = package.Workbook.Worksheets[li];
                        //add column header
                        try
                        {
                            foreach (var header in workSheet.Cells[1, 1, 1, workSheet.Dimension.End.Column])
                            {
                                dt.Columns.Add(header.Text);
                            }
                            for (int rN = 2; rN <= workSheet.Dimension.End.Row; rN++)
                            {
                                ExcelRange row = workSheet.Cells[rN, 1, rN, workSheet.Dimension.End.Column];
                                DataRow newR = dt.NewRow();
                                foreach (var cell in row)
                                {
                                    try
                                    {
                                        newR[cell.Start.Column - 1] = cell.Text;
                                    }
                                    catch (Exception ex)
                                    {
                                        status = "Something went wrong : Exception Caught Debugging Required";
                                        ex.GetType();
                                        //+ ex.ToString();
                                        //status = status.Substring(0, (status.IndexOf("Exception") + 8));
                                        m_ErrorLog.SetError("," + status);
                                    }
                                }
                                dt.Rows.Add(newR);
                            }
                            dataSet.Tables.Add(dt);
                        }
                        catch (Exception ex)
                        {
                            status = "Something went wrong : Exception Caught Debugging Required";
                            m_ErrorLog.SetError("," + status);
                        }
                        // add rows
                    }
                }
                return dataSet;
            }
            catch (Exception ex)
            {
                status = "Something went wrong : Exception Caught Debugging Required";
                m_ErrorLog.SetError("," + status + ",");
                throw new Exception(ex.Message);
            }
        }
        private void ErrorLog(string Message)
        {
            System.IO.File.AppendAllText(@"C:\LogFile\test.txt", Message);
        }
    }
}