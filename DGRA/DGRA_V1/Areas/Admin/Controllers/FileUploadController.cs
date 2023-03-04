
using DGRA_V1.Common;
using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using OfficeOpenXml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DGRA_V1.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;

namespace DGRA_V1.Areas.admin.Controllers
{
    [Area("admin")]
    [AllowAnonymous]
    [ServiceFilter(typeof(SessionValidation))]
    [TypeFilter(typeof(SessionValidation))]
    public class FileUploadController : Controller
    {
        ImportBatch objImportBatch = new ImportBatch();
        private IDapperRepository _idapperRepo;
        private IWebHostEnvironment env;
        private static IHttpContextAccessor HttpContextAccessor;
//        CultureInfo timeCulture = CultureInfo.InvariantCulture;
        public FileUploadController(IDapperRepository idapperobj, IWebHostEnvironment iwebhostobj, IHttpContextAccessor httpObj)
        {
            HttpContextAccessor = httpObj;
            _idapperRepo = idapperobj;
            m_ErrorLog = new ErrorLog(iwebhostobj);
            env = iwebhostobj;
        }
        static int batchIdDGRAutomation = 0;
        string siteUserRole;
        int previousSite = 0;
        static string[] importData = new string[2];
        string generationDate = "";
        static bool isGenValidationSuccess = false;
        static bool isBreakdownValidationSuccess = false;
        static bool isPyro1ValidationSuccess = false;
        static bool isPyro15ValidationSuccess = false;
        string genJson = string.Empty;
        string breakJson = string.Empty;
        string pyro1Json = string.Empty;
        string pyro15Json = string.Empty;
        string windGenJson = string.Empty;
        string windBreakJson = string.Empty;
        ArrayList kpiArgs = new ArrayList();
        List<int> windSiteUserAccess = new List<int>();
        List<int> solarSiteUserAccess = new List<int>();
        List<string> fileSheets = new List<string>();
        List<string> inverterList = new List<string>();
        ErrorLog m_ErrorLog;

        Hashtable equipmentId = new Hashtable();
        Hashtable maxkWhMap_wind = new Hashtable();
        Hashtable breakdownType = new Hashtable();//(B)Gets bdTypeID from bdTypeName: BDType table
        Hashtable siteNameId = new Hashtable(); //(C)Gets siteId from siteName
        Hashtable siteName = new Hashtable(); //(D)Gets siteName from siteId


        Hashtable eqSiteId = new Hashtable();//(E)Gets siteId from (wtg)equipmentName
        Hashtable MonthList = new Hashtable() { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
        Hashtable longMonthList = new Hashtable() { { "January", 1 }, { "February", 2 }, { "March", 3 }, { "April", 4 }, { "May", 5 }, { "June", 6 }, { "July", 7 }, { "August", 8 }, { "September", 9 }, { "October", 10 }, { "November", 11 }, { "December", 12 } };


        /*~FileUploadController()
        {
            //Destructor
            //delete m_ValidationObject;
            //delete m_ErrorLog;
        }*/

        [HttpGet]
        public ActionResult Upload()
        {
            TempData["notification"] = "";
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> Upload(string FileUpload)
        {
            try
            {
                //  string response = await ExceldatareaderAndUpload(Request.Files["Path"]);
                string response = await ExcelDataReaderAndUpload(HttpContext.Request.Form.Files[0], FileUpload);
                TempData["notification"] = response;
            }
            catch (Exception ex)
            {
                TempData["notification"] = "Failed to upload";
                string message = ex.Message;
                m_ErrorLog.SetError(",Failed to upload {fileUploadType},");
                ErrorLog("Failed to upload <" + FileUpload + ">  File <" + HttpContext.Request.Form.Files[0] + "> Reason : " + ex.Message);

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
                if (usermodel.access_list[i].page_type == 3 && usermodel.access_list[i].site_type == 2)
                {
                    solarSiteUserAccess.Add(Convert.ToInt32(usermodel.access_list[i].identity));
                }
            }
            //windSiteList = HttpContextAccessor.HttpContext.Session.GetString("UserAccess");
            siteUserRole = HttpContext.Session.GetString("role");
            DateTime today = DateTime.Now;
            // string csvFileName = env.ContentRootPath +@"\LogFile\"+ file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            string csvFileName = file.FileName + "_" + today.ToString("dd-MM-yyyy") + "_" + today.ToString("hh-mm-ss") + ".csv";
            importData[0] = fileUploadType;
            importData[1] = csvFileName;
            string status = "";
            int statusCode = 400;   //Bad request
            DataSet dataSetMain = new DataSet();
            var allowedExtensions = new[] { ".xlsx", ".xls" };
            var ext = Path.GetExtension(file.FileName);

            if (allowedExtensions.Contains(ext))
            {
                try
                {
                    /* if (!Directory.Exists(env.ContentRootPath + @"\TempFile"))
                     {
                         DirectoryInfo dinfo = Directory.CreateDirectory(env.ContentRootPath + @"\TempFile");
                     }
                     else
                     {
                         string[] filePaths = Directory.GetFiles(env.ContentRootPath + @"\TempFile");
                         if (filePaths.Length > 0)
                         {
                             foreach (String path in filePaths)
                             {
                                 System.IO.File.Delete(path);
                             }
                         }
                     }*/
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
                    if (ext == ".xlsx") //Pending what about xls file.
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

                            string _filePath = @"C:\TempFile\docupload.xlsx";
                            //string _filePath = @"G:\TempFile\docupload.xlsx";
                            //string _filePath = env.ContentRootPath + @"\TempFile\docupload.xlsx";
                            dataSetMain = GetDataTableFromExcel(_filePath, true, ref fileSheets);
                            if (dataSetMain == null)
                            {
                                m_ErrorLog.SetError(",Unable to extract excel sheet data for importing,");
                            }
                            ErrorLog("datSetMain null " + dataSetMain);


                            //masterHashtable_SiteName_To_SiteId();//C
                            masterHashtable_SiteIdToSiteName();
                            if (fileUploadType == "Wind")
                            {
                                masterHashtable_WtgToWtgId();
                                masterHashtable_WtgToSiteId();
                            }
                            if (fileUploadType == "Solar")
                            {
                                masterInverterList();
                            }

                            if (fileSheets.Contains("Uploading_File_Breakdown"))
                            {
                                masterHashtable_BDNameToBDId();//B
                            }

                            //Status Codes:
                            //200 = Success ; 400 = Failure(BadRequest)
                            FileSheetType.FileImportType fileImportType = FileSheetType.FileImportType.imporFileType_Invalid;
                            foreach (var excelSheet in fileSheets)
                            {
                               
                                DataSet ds = new DataSet();
                                if (excelSheet == FileSheetType.Uploading_File_Generation)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_File_Generation WorkSheet:");
                                            statusCode = await InsertSolarFileGeneration(status, ds);

                                        }
                                        else if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind_Uploading_File_Generation WorkSheet");
                                            statusCode = await InsertWindFileGeneration(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_File_Breakdown)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Uploading_File_Breakdown WorkSheet:");
                                            statusCode = await InsertSolarFileBreakDown(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Uploading_File_Breakdown WorkSheet:");
                                            statusCode = await InsertWindFileBreakDown(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter1Min)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter1Min: ");
                                            statusCode = await InsertSolarPyranoMeter1Min(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for pyranometer import";
                                            m_ErrorLog.SetError("," + status);
                                            ErrorLog(status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter15Min)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Automation;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter15Min :");
                                            statusCode = await InsertSolarPyranoMeter15Min(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for pyranometer import";
                                            m_ErrorLog.SetError("," + status);
                                            ErrorLog(status);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_JMR_Input_and_Output)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_JMR_Input_and_Output;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertWindMonthlyJMR(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertSolarMonthlyJMR(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_LineLoss)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_LineLoss;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertWindMonthlyLineLoss(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertSolarMonthlyLineLoss(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_Target_KPI)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Monthly_Target_KPI;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindMonthlyTargetKPI(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarMonthlyTargetKPI(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Load_Shedding)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Daily_Load_Shedding;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertWindDailyLoadShedding(status, ds);

                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertSolarDailyLoadShedding(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Target_KPI)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Daily_Target_KPI;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindDailyTargetKPI(status, ds);
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarDailyTargetKPI(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Site_Master)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Site_Master;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Site_Master WorkSheet:");
                                            statusCode = await InsertWindSiteMaster(status, ds);
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Site_Master WorkSheet:");
                                            statusCode = await InsertSolarSiteMaster(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Location_Master)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Location_Master;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Location_Master WorkSheet:");
                                            statusCode = await InsertWindLocationMaster(status, ds);
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Location_Master WorkSheet:");
                                            statusCode = await InsertSolarLocationMaster(status, ds);
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_AC_DC_Capacity)
                                {
                                    fileImportType = FileSheetType.FileImportType.imporFileType_Solar_AC_DC_Capacity;
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar AC_DC_Capacity WorkSheet:");
                                            statusCode = await InsertSolarAcDcCapacity(status, ds);
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected for acdc import";
                                        }
                                    }
                                }
                                else
                                {
                                    status = "Unsupported Tab name <" + excelSheet + ">. Pl do not change tab name.";
                                    m_ErrorLog.SetError(status);
                                    ErrorLog(status);
                                    //responseCode = 400;
                                }
                                //clear all rows in ds
                                ds.Clear();
                                //statusCode = 200;
                            } // end of foreach (var excelSheet in fileSheets)
                            if (statusCode == 200)
                            {
                                // await UploadFileToImportedFileFolder(file);
                                if (!(fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min")))
                                {
                                    await importMetaData(fileUploadType, file.FileName, fileImportType);
                                }

                                //DGR Automation Function Logic
                                if (fileUploadType == "Wind")
                                {
                                    if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown"))
                                    {
                                        if (isGenValidationSuccess && isBreakdownValidationSuccess)
                                        {
                                            await importMetaData(fileUploadType, file.FileName, fileImportType);
                                            statusCode = await dgrWindImport(batchIdDGRAutomation);
                                            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailyWindKPI?fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                            //remove after testing
                                            // m_ErrorLog.SetInformation("Url" + url);
                                            using (var client = new HttpClient())
                                            {
                                                var response = await client.GetAsync(url);
                                                //status = "Respose" + response;
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    m_ErrorLog.SetInformation(",Wind KPI Calculations Updated Successfully:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Successfully Uploaded";

                                                    // Added Code auto approved if uploaded by admin
                                                    string userName = HttpContext.Session.GetString("DisplayName");
                                                    int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                                    siteUserRole = HttpContext.Session.GetString("role");
                                                    if (siteUserRole == "Admin")
                                                    {
                                                        
                                                        var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                                        using (var client1 = new HttpClient())
                                                        {
                                                            await Task.Delay(10000);
                                                            var response1 = await client1.GetAsync(url1);
                                                            if (response1.IsSuccessStatusCode)
                                                            {
                                                                //status = "Successfully Data Approved";
                                                            }
                                                            else
                                                            {
                                                                //status = "Data Not Approved";
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    
                                                    m_ErrorLog.SetError(",Wind KPI Calculations API Failed:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Wind KPI Calculation Import API Failed";

                                                    //for solar 0, wind 1;
                                                    int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 1);
                                                    if(deleteStatus == 1)
                                                    {
                                                        m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                    }
                                                    else
                                                    {
                                                        m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetError(",Data not imported.");
                                        }
                                    }
                                    else
                                    {
                                        status = "Successfully Uploaded";
                                    }
                                }
                                else if (fileUploadType == "Solar")
                                {
                                    if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min"))
                                    {
                                        ErrorLog("isGenValidationSuccess" + isGenValidationSuccess);
                                        ErrorLog("isPyro15ValidationSuccess" + isPyro15ValidationSuccess);
                                        ErrorLog("isPyro1ValidationSuccess" + isPyro1ValidationSuccess);
                                        ErrorLog("Before Validation");
                                        //pending : instead check the  success flags
                                        if (isGenValidationSuccess && isBreakdownValidationSuccess && isPyro15ValidationSuccess && isPyro1ValidationSuccess)
                                        {
                                            await importMetaData(fileUploadType, file.FileName, fileImportType);
                                            statusCode = await dgrSolarImport(batchIdDGRAutomation);
                                            //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?fromDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?site=" + (string)kpiArgs[2] + "&fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "";
                                            using (var client = new HttpClient())
                                            {
                                                var response = await client.GetAsync(url);
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    m_ErrorLog.SetInformation(",SolarKPI Calculations Updated Successfully:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Successfully Uploaded";

                                                    // Added Code auto approved if uploaded by admin
                                                    string userName = HttpContext.Session.GetString("DisplayName");
                                                    int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
                                                    siteUserRole = HttpContext.Session.GetString("role");
                                                    if (siteUserRole == "Admin")
                                                    {
                                                        var url1 = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/SetSolarApprovalFlagForImportBatches?dataId=" + batchIdDGRAutomation + "&approvedBy=" + userId + "&approvedByName=" + userName + "&status=1";
                                                        using (var client1 = new HttpClient())
                                                        {
                                                            await Task.Delay(10000);
                                                            var response1 = await client1.GetAsync(url1);
                                                            if (response1.IsSuccessStatusCode)
                                                            {
                                                                //status = "Successfully Data Approved";
                                                            }
                                                            else
                                                            {
                                                                //status = "Data Not Approved";
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(",SolarKPI Calculations API Failed:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Solar KPI Calculation Import API Failed";

                                                    //for solar 0, wind 1;
                                                    int deleteStatus = await DeleteRecordsAfterFailure(importData[1], 0);
                                                    if (deleteStatus == 1)
                                                    {
                                                        m_ErrorLog.SetInformation(", Records deleted successfully after incomplete upload");
                                                    }
                                                    else
                                                    {
                                                        m_ErrorLog.SetInformation(", Records deletion failed due to incomplete upload");
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetError(",Data not imported.");
                                        }
                                    }
                                    else
                                    {
                                        status = "Successfully Uploaded";
                                    }
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
                    ErrorLog(status + "True\r\n" + ex.Message);
                }
            }
            else
            {
                //excel file format condition
                status = "File extension not supported. Upload type <" + fileUploadType + ">  Filename < " + csvFileName + ">"  ;
                m_ErrorLog.SetError("," + status + ":");
                ErrorLog(status);
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
                                       // m_ErrorLog.SetError("," + status);
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

        //Remove static
        //Beginning of all DGR Import functions for both Wind and Solar Upload types

        private async Task<int> GetBatchId(string importData)
        {
            var urlGetId = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchId?logFileName=" + importData + "";
            var result = string.Empty;
            WebRequest request = WebRequest.Create(urlGetId);
            using (var responses = (HttpWebResponse)request.GetResponse())
            {
                Stream receiveStream = responses.GetResponseStream();
                using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                {
                    result = readStream.ReadToEnd();
                }
                BatchIdImport obj = new BatchIdImport();
                obj = JsonConvert.DeserializeObject<BatchIdImport>(result);
                batchIdDGRAutomation = obj.import_batch_id;
                if (batchIdDGRAutomation == 0)
                {
                    return 0;
                }
                else
                {
                    return batchIdDGRAutomation;
                }
            }
        }
        private async Task<int> DeleteRecordsAfterFailure(string importData, int siteType)
        {
            //for solar 0, wind 1;
                int batchId = await GetBatchId(importData);

                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/DeleteRecordsAfterFailure?batchId=" + batchId + "&siteType=" + siteType + "";
                var result = "";
                WebRequest request = WebRequest.Create(url);
                using (var responses = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = responses.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                    if (result == "1")
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                }
        }

        private async Task<int> InsertSolarFileGeneration(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string kpiSite = "";
            try
            {
                //fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
                //toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
                fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
            }
            catch (Exception e)
            {
                //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy");
                fromDate = DateTime.MaxValue;
                toDate = DateTime.MinValue;
            }
            //---------------------
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                SolarUploadingFileValidation solarValidation = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarUploadingFileGeneration> addSet = new List<SolarUploadingFileGeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarUploadingFileGeneration addUnit = new SolarUploadingFileGeneration();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if(isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);
                        
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        
                        addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            generationDate = addUnit.date;
                        }
                        if(rowNumber > 2)
                        {
                            if (generationDate != addUnit.date)
                            {
                                m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                                errorCount++;
                                skipRow = true;
                                continue;
                            }
                        }


                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        if (siteUserRole != "Admin")
                        {
                            errorFlag.Add(uniformWindSiteValidation(rowNumber, addUnit.site_id, ""));
                        }
                           
                        objImportBatch.importSiteId = addUnit.site_id;//C
                        kpiSite = Convert.ToString(addUnit.site_id);
                        if (rowNumber == 2)
                        {
                            objImportBatch.automationDataDate = addUnit.date;
                            dateValidate = Convert.ToDateTime((string)dr["Date"]);
                            errorFlag.Add(importDateValidation(2, addUnit.site_id, dateValidate));
                            
                        }
                        //nextDate = Convert.ToDateTime(dr["Date"], timeCulture);
                        nextDate = Convert.ToDateTime(dr["Date"]);
                        fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                        toDate = (nextDate > toDate) ? (nextDate) : (toDate);

                        addUnit.inverter = string.IsNullOrEmpty((string)dr["Inverter"]) ? "Nil" : Convert.ToString(dr["Inverter"]);
                        errorFlag.Add(solarInverterValidation(addUnit.inverter, "Inverter", rowNumber));

                        // addUnit.inv_act = string.IsNullOrEmpty((string)dr["Inv_Act(KWh)"]) ? 0 : Convert.ToDouble(dr["Inv_Act(KWh)"]);
                        // addUnit.plant_act = string.IsNullOrEmpty((string)dr["Plant_Act(kWh)"]) ? 0 : Convert.ToDouble(dr["Plant_Act(kWh)"]);

                        double importValue = 0.00;
                        int logErrorFlag = 0; //log as information
//                        addUnit.inv_act = string.IsNullOrEmpty((string)dr["Inv_Act(KWh)"]) ? 0 : Convert.ToDouble(dr["Inv_Act(KWh)"]);
                        addUnit.inv_act = validateNumeric(((string)dr["Inv_Act(KWh)"]), "Inv_Act(KWh)", rowNumber, dr["Inv_Act(KWh)"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(negativeNullValidation(addUnit.inv_act, "Inv_Act(KWh)", rowNumber));
                        //addUnit.plant_act = string.IsNullOrEmpty((string)dr["Plant_Act(kWh)"]) ? 0 : Convert.ToDouble(dr["Plant_Act(kWh)"]);
                        addUnit.plant_act = validateNumeric(((string)dr["Plant_Act(KWh)"]), "Plant_Act(KWh)", rowNumber, dr["Plant_Act(KWh)"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(negativeNullValidation(addUnit.plant_act, "Inv_Act(KWh)", rowNumber));

                        addUnit.pi = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PI(%)"]), "PI(%)");
                        errorFlag.Add(solarValidation.validateGenerationData(rowNumber, addUnit.inverter, addUnit.inv_act, addUnit.plant_act));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": function: InsertSolarFileGeneration,");
                        ErrorLog(",Exception Occurred In Function: InsertSolarFileGeneration: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    //kpiArgs.Add(minDate);
                    //kpiArgs.Add(maxDate);
                    m_ErrorLog.SetInformation(",Solar Generation Validation Successful");
                    isGenValidationSuccess = true;
                    responseCode = 200;
                    kpiArgs.Add(fromDate);
                    kpiArgs.Add(toDate);
                    kpiArgs.Add(kpiSite);
                    genJson = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Solar Generation Validation Failed");
                    isGenValidationSuccess = false;
                }
            }
            return responseCode;
        }

        private async Task<int> InsertWindFileGeneration(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            DateTime dateValidate = DateTime.MinValue;
            DateTime fromDate;
            DateTime toDate;
            DateTime nextDate = DateTime.MinValue;
            string site = "";
            try
            {
                fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
                toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);

            }
            catch (Exception e)
            {
                //m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format MM-dd-yyyy");
                m_ErrorLog.SetError(",File Row <2> column <Date> Invalid Date Format. Use format dd-MM-yyyy");
                fromDate = DateTime.MaxValue;
                toDate = DateTime.MinValue;
            }
            //---------------------------------
            WindUploadingFileValidation validationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);
            if (ds.Tables.Count > 0)
            {
               var numberofrows = ds.Tables[0].Rows.Count;

                for (int i = 0; i < numberofrows; i++)
                {
                    //for each row, get the 3rd column
                    var cell = ds.Tables[0].Rows[i][6];
                }

                generationDate = "";
                double max_kWh = 0;
                List<WindUploadingFileGeneration> addSet = new List<WindUploadingFileGeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindUploadingFileGeneration addUnit = new WindUploadingFileGeneration();
                        bool skipRow = false;
                        rowNumber++;
                        ErrorLog("Before date conversion\r\n");

                        //addUnit.date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString((string)dr["Date"]);
                        // errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        //addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if(addUnit.wtg == "" && addUnit.wind_speed == "" && addUnit.kwh == "" && addUnit.grid_hrs == "" && addUnit.lull_hrs == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" +"Is blank.");
                            ErrorLog(",File row <" + rowNumber + ">" +"Is blank.");
                            skipRow = true;
                            continue;
                        }

                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : Convert.ToString((string)dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if(rowNumber == 2)
                        {
                            generationDate = addUnit.date; 
                        }
                        if(rowNumber > 2)
                        {
                            if (generationDate != addUnit.date)
                            {
                                m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                                errorCount++;
                                skipRow = true;
                                continue;
                            }
                        }

                        addUnit.wtg = dr["WTG"] is DBNull || string.IsNullOrEmpty((string)dr["WTG"]) ? "Nil" : Convert.ToString(dr["WTG"]);
                        addUnit.wtg_id = equipmentId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(equipmentId[addUnit.wtg]) : 0;
                        errorFlag.Add(wtgValidation(addUnit.wtg, addUnit.wtg_id, rowNumber));
                        if (rowNumber == 2)
                        {
                            if (maxkWhMap_wind.ContainsKey(addUnit.wtg_id))
                            {
                                max_kWh = Convert.ToDouble(maxkWhMap_wind[addUnit.wtg_id]);
                            }
                            else
                            {
                                errorFlag.Add(true);
                                m_ErrorLog.SetError(",WTG <" + addUnit.wtg + "> WTG_id<" + addUnit.wtg_id + "> does not have max kWh set in location master");
                            }
                        }

                        addUnit.site_id = eqSiteId.ContainsKey(addUnit.wtg) ? Convert.ToInt32(eqSiteId[addUnit.wtg]) : 0;
                        addUnit.site_name = siteName.ContainsKey(addUnit.site_id) ? (string)(siteName[addUnit.site_id]) : "Nil";
                        site = Convert.ToString(addUnit.site_id);
                        if (siteUserRole != "Admin")
                        {
                            errorFlag.Add(uniformWindSiteValidation(rowNumber, addUnit.site_id, addUnit.wtg));
                        }
                            
                        objImportBatch.importSiteId = addUnit.site_id;
                        //dateValidate = Convert.ToDateTime((string)dr["Date"]).ToString("yyyy-MM-dd");
                        if (rowNumber == 2)
                        {
                            objImportBatch.automationDataDate = addUnit.date;
                            dateValidate = Convert.ToDateTime(dr["Date"]);
                            errorFlag.Add(importDateValidation(1, addUnit.site_id, dateValidate));
                        }

                        int logErrorFlag = 0;   //log as information
                        double importValue = 0;

                        //addUnit.wind_speed = string.IsNullOrEmpty((string)dr["Wind_Speed"]) ? 0 : Convert.ToDouble(dr["Wind_Speed"]);
                        addUnit.wind_speed = validateNumeric(((string)dr["Wind_Speed"]), "Wind_Speed", rowNumber, dr["Wind_Speed"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(numericNullValidation(addUnit.wind_speed, "Wind_Speed", rowNumber));
                       
                        //addUnit.operating_hrs = dr["Gen_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Gen_Hrs"]) ? 0 : Convert.ToDouble(dr["Gen_Hrs"]);
                        addUnit.operating_hrs = validateNumeric(((string)dr["Gen_Hrs"]), "Gen_Hrs", rowNumber, dr["Gen_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        //errorFlag.Add(numericNullValidation(addUnit.operating_hrs, "Gen_Hrs", rowNumber));

                        //addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        addUnit.kwh = validateNumeric(((string)dr["kWh"]), "kWh", rowNumber, dr["kWh"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        errorFlag.Add(kwhValidation(addUnit.kwh, addUnit.operating_hrs, "kWh", rowNumber, max_kWh));

                       // addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                       // errorFlag.Add(numericNullValidation(addUnit.kwh, "kWh", rowNumber));

                       // addUnit.operating_hrs = dr["Gen_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Gen_Hrs"]) ? 0 : ////Convert.ToDouble(dr["Gen_Hrs"]);

                        //errorFlag.Add(numericNullValidation(addUnit.operating_hrs, "Gen_Hrs", rowNumber));
                        //last column is Blank
                        if (addUnit.grid_hrs == "" || addUnit.grid_hrs == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Grid_hrs> :  Grid hours column is blank <" + addUnit.grid_hrs + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                        //addUnit.lull_hrs = dr["Lull_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Lull_Hrs"]) ? 0 : Convert.ToDouble(dr["Lull_Hrs"]);
                        addUnit.lull_hrs = validateNumeric(((string)dr["Lull_Hrs"]), "Lull_Hrs", rowNumber, dr["Lull_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        //addUnit.grid_hrs = dr["Grid_Hrs"] is DBNull || string.IsNullOrEmpty((string)dr["Grid_Hrs"]) ? 0 : Convert.ToDouble(dr["Grid_Hrs"]);
                        addUnit.grid_hrs = validateNumeric(((string)dr["Grid_Hrs"]), "Grid_Hrs", rowNumber, dr["Grid_Hrs"] is DBNull, logErrorFlag, out importValue) ? 0 : importValue;
                        nextDate = Convert.ToDateTime(dr["Date"]);
                        fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                        toDate = (nextDate > toDate) ? (nextDate) : (toDate);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        errorCount++;
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindFileGeneration,");
                        ErrorLog(",Exception Occurred In Function: InsertWindFileGeneration: " + e.Message + "");
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Generation File Validation Successful,");
                    isGenValidationSuccess = true;
                    responseCode = 200;
                    genJson = JsonConvert.SerializeObject(addSet);
                    kpiArgs.Add(fromDate);
                    kpiArgs.Add(toDate);
                    kpiArgs.Add(site);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Generation File Validation Failed");
                    isGenValidationSuccess = false;
                }
            }

            return responseCode;
        }

        private async Task<int> InsertSolarFileBreakDown(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarUploadingFileBreakDown> addSet = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarUploadingFileBreakDown addUnit = new SolarUploadingFileBreakDown();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date = isdateEmpty ? "Nil" : (string)dr["Date"];
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? "Nil" : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        if (generationDate != addUnit.date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        //last row is Blank
                        if(addUnit.date == "" && addUnit.bd_type == "" && addUnit.action_taken == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" + "Is blank.");
                            ErrorLog(",Exception Occurred In : SolarUploadingFileBreakdown: ");
                            skipRow = true;
                            continue;
                        }

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;
                        //can be nil:
                        addUnit.ext_int_bd = string.IsNullOrEmpty((string)dr["Ext_BD"]) ? "Nil" : Convert.ToString(dr["Ext_BD"]);
                        //can be nil:
                        addUnit.igbd = string.IsNullOrEmpty((string)dr["IGBD"]) ? "Nil" : Convert.ToString(dr["IGBD"]);
                        //can be nil:
                        addUnit.icr = string.IsNullOrEmpty((string)dr["ICR"]) ? "Nil" : Convert.ToString(dr["ICR"]);
                        //can be nil:
                        addUnit.inv = string.IsNullOrEmpty((string)dr["INV"]) ? "Nil" : Convert.ToString(dr["INV"]);
                        //can be nil:
                        addUnit.smb = string.IsNullOrEmpty((string)dr["SMB"]) ? "Nil" : Convert.ToString(dr["SMB"]);
                        //can be nil:
                        addUnit.strings = dr["Strings"] is DBNull || string.IsNullOrEmpty((string)dr["Strings"]) ? "Nil" : Convert.ToString(dr["Strings"]);

                        //from_bd and to_bd conversion for validation
                        addUnit.from_bd = dr["From"] is DBNull || string.IsNullOrEmpty((string)dr["From"]) ? "Nil" : Convert.ToDateTime(dr["From"]).ToString("HH:mm:ss");
                        errorFlag.Add(timeValidation(addUnit.from_bd, "From", rowNumber));

                        addUnit.to_bd = dr["To"] is DBNull || string.IsNullOrEmpty((string)dr["To"]) ? "Nil" : Convert.ToDateTime(dr["To"]).ToString("HH:mm:ss");
                        errorFlag.Add(timeValidation(addUnit.to_bd, "To", rowNumber));

                        addUnit.total_bd = validationObject.breakDownCalc(addUnit.from_bd, addUnit.to_bd, rowNumber);
                        addUnit.bd_remarks = dr["BDRemarks"] is DBNull || string.IsNullOrEmpty((string)dr["BDRemarks"]) ? "Nil" : Convert.ToString(dr["BDRemarks"]);
                        addUnit.bd_type = dr["BDType"] is DBNull || string.IsNullOrEmpty((string)dr["BDType"]) ? "Nil" : Convert.ToString(dr["BDType"]);
                        addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                        errorFlag.Add(bdTypeValidation(addUnit.bd_type, rowNumber));

                        string sActionTaken = dr["ActionTaken"] is DBNull || string.IsNullOrEmpty((string)dr["ActionTaken"]) ? "Nil" : Convert.ToString(dr["ActionTaken"]);
                        sActionTaken = validateAndCleanSpChar(rowNumber, "Action_Taken", sActionTaken);
                        addUnit.action_taken = sActionTaken;
                        errorFlag.Add(validationObject.validateBreakDownData(rowNumber, addUnit.from_bd, addUnit.to_bd, addUnit.igbd));
                        if (addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        //last column is Blank
                        if (addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File row <" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarFileBreakDown");
                        ErrorLog(",Exception Occurred In Function: InsertSolarFileBreakDown: " + e.Message);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Breakdown Validation Successful:");
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Solar Breakdown Validation Failed");
                    isBreakdownValidationSuccess = false;
                }
            }

            return responseCode;
        }
        private async Task<int> InsertWindFileBreakDown(string status, DataSet ds)
        {
            WindUploadingFileValidation ValidationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileBreakDown> addSet = new List<WindUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindUploadingFileBreakDown addUnit = new WindUploadingFileBreakDown();
                        rowNumber++;
                        bool skipRow = false;
                        bool isdateEmpty = dr["Date"] is DBNull || string.IsNullOrEmpty((string)dr["Date"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date =  isdateEmpty ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[0] ? "Nil" : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        //last if last row is Blank then skip? delete the row
                        if (addUnit.wtg == "" && addUnit.bd_type == "" && addUnit.stop_from == "" && addUnit.stop_to == "" && addUnit.error_description == "" && addUnit.action_taken == "")
                        {
                            m_ErrorLog.SetError(",File row <" + rowNumber + ">" + "Is blank.");
                            ErrorLog(",Exception Occurred In : WindUploadingFileBreakdown: ");
                            skipRow = true;
                            continue;
                        }
                        if (addUnit.date == "" || addUnit.date == "Nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : Date field is empty <" + addUnit.date + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        if(generationDate !=addUnit.date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Breakdown Date <" + addUnit.date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        addUnit.wtg = Convert.ToString(dr["WTG"]);
                        addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                        addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);//E
                        addUnit.site_name = (string)siteName[addUnit.site_id];//D
                        objImportBatch.importSiteId = addUnit.site_id;
                        addUnit.bd_type = Convert.ToString(dr["BD_Type"]);
                        addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
/*                        if (addUnit.bd_type_id == 0)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <BD_Type> : Invalid BD_Type <" + addUnit.bd_type + ">");
                            errorFlag.Add(true);                                
                        }*/
                        errorFlag.Add(bdTypeValidation(addUnit.bd_type, rowNumber));
                        addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("HH:mm:ss");
                        addUnit.stop_to = Convert.ToDateTime(dr["Stop To"]).ToString("HH:mm:ss");
                        addUnit.total_stop = ValidationObject.breakDownCalc(addUnit.stop_from, addUnit.stop_to);
                        addUnit.error_description = Convert.ToString(dr["Error description"]);
                        string sActionTaken = dr["Action Taken"] is DBNull || string.IsNullOrEmpty((string)dr["Action Taken"]) ? "Nil" : Convert.ToString(dr["Action Taken"]);
                        sActionTaken = validateAndCleanSpChar(rowNumber, "Action Taken", sActionTaken);
                        addUnit.action_taken = sActionTaken;
                        errorFlag.Add(ValidationObject.validateBreakDownData(rowNumber, addUnit.bd_type, addUnit.wtg, addUnit.stop_from, addUnit.stop_to));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                        //last column is Blank
                        if(addUnit.action_taken == "" || addUnit.action_taken == "nil")
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Action Taken> :  Action Taken column is blank <" + addUnit.action_taken + ">");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindFileBreakDown,");
                        ErrorLog(",Exception Occurred In Function: InsertWindFileBreakDown: " + e.Message + ",");
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Breakdown File Validation Successful,");
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);

                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Breakdown File Validation Failed,");
                    isBreakdownValidationSuccess = false;
                }
            }

            return responseCode;
        }

        private async Task<int> InsertSolarPyranoMeter1Min(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            try
            {
                if (ds.Tables.Count > 0)
                {
                    List<SolarUploadingPyranoMeter1Min> addSet = new List<SolarUploadingPyranoMeter1Min>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SolarUploadingPyranoMeter1Min addUnit = new SolarUploadingPyranoMeter1Min();
                        rowNumber++;
                        bool skipRow = false;
                        //addUnit.date_time = string.IsNullOrEmpty((string)dr["Time stamp"]) ? "Nil" : Convert.ToDateTime(dr["Time stamp"], timeCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        bool isdateEmpty = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date_time = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                        errorFlag.Add(stringNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        errorFlag.Add(dateNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        string temp = addUnit.date_time;
                        string temp_date = temp.Substring(0, 10);
                        if (generationDate != temp_date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Pyranometer 1 Minute <" + temp_date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        string site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[site]);
                        errorFlag.Add(siteValidation(site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        int logErrorFlag = 1;
                        double importValue = 0.00;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-1"]), "GHI-1", rowNumber, dr["GHI-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-2"]), "GHI-2", rowNumber, dr["GHI-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-1"]), "POA-1", rowNumber, dr["POA-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-2", rowNumber, dr["POA-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-3", rowNumber, dr["POA-3"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_3 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-4"]), "POA-4", rowNumber, dr["POA-4"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_4 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-5"]), "POA-5", rowNumber, dr["POA-5"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_5 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-6"]), "POA-6", rowNumber, dr["POA-6"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_6 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-7"]), "POA-7", rowNumber, dr["POA-7"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_7 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average GHI (w/m²)"]), "Average GHI (w/m²)", rowNumber, dr["Average GHI (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_ghi = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average POA (w/m²)"]), "Average POA (w/m²)", rowNumber, dr["Average POA (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_poa = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Ambient Temp"]), "Ambient Temp", rowNumber, dr["Ambient Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.amb_temp = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Module Temp"]), "Module Temp", rowNumber, dr["Module Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.mod_temp = importValue;
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    //pending check error
                    //set IsPyro1 flag
                    if (!(errorCount > 0))
                    {
                        m_ErrorLog.SetInformation(",Solar PyranoMeter-1Min Validation Successful");
                        //set the  validationgeneration sucess flag
                        isPyro1ValidationSuccess = true;
                        responseCode = 200;
                        pyro1Json = JsonConvert.SerializeObject(addSet);
                    }
                    else
                    {
                        // add to error log that validation of generation failed
                        m_ErrorLog.SetError(",Solar PyranoMeter-1Min Validation Failed");
                        isPyro1ValidationSuccess = false;
                    }
                }

            }
            catch (Exception e)
            {
                //developer errorlog
                m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarPyranoMeter1Min");
                ErrorLog(",Exception Occurred In Function: InsertSolarPyranoMeter1Min: " + e.Message);
            }
            return responseCode;
        }

        private async Task<int> InsertSolarPyranoMeter15Min(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            try
            {
                if (ds.Tables.Count > 0)
                {
                    List<SolarUploadingPyranoMeter15Min> addSet = new List<SolarUploadingPyranoMeter15Min>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        SolarUploadingPyranoMeter15Min addUnit = new SolarUploadingPyranoMeter15Min();
                        rowNumber++;
                        bool skipRow = false;
                        // addUnit.date_time = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]) ? "Nil" : Convert.ToDateTime(dr["Time stamp"], timeCulture).ToString("yyyy-MM-dd HH:mm:ss");
                        bool isdateEmpty = dr["Time stamp"] is DBNull || string.IsNullOrEmpty((string)dr["Time stamp"]);
                        if (isdateEmpty)
                        {
                            m_ErrorLog.SetInformation(", Date value is empty. The row would be skiped.");
                            continue;
                        }
                        addUnit.date_time = isdateEmpty ? "Nil" : Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                        errorFlag.Add(stringNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        errorFlag.Add(dateNullValidation(addUnit.date_time, "Time stamp", rowNumber));
                        string temp = addUnit.date_time;
                        string temp_date = temp.Substring(0, 10);
                        if (generationDate != temp_date)
                        {
                            m_ErrorLog.SetError(",Row <" + rowNumber + "> column <Date> : File Generation <" + generationDate + "> and Pyranometer 15 Minute <" + temp_date + "> missmatched");
                            errorCount++;
                            skipRow = true;
                            continue;
                        }

                        string site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[site]);
                        errorFlag.Add(siteValidation(site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        int logErrorFlag = 1;
                        double importValue = 0.00;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-1"]), "GHI-1", rowNumber, dr["GHI-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["GHI-2"]), "GHI-2", rowNumber, dr["GHI-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.ghi_2 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-1"]), "POA-1", rowNumber, dr["POA-1"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_1 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-2", rowNumber, dr["POA-2"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_2 = importValue; 
                        errorFlag.Add(validateNumeric(((string)dr["POA-3"]), "POA-3", rowNumber, dr["POA-3"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_3 = importValue; 
                        errorFlag.Add(validateNumeric(((string)dr["POA-4"]), "POA-4", rowNumber, dr["POA-4"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_4 = importValue; 
                        errorFlag.Add(validateNumeric(((string)dr["POA-5"]), "POA-5", rowNumber, dr["POA-5"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_5 = importValue; 
                        errorFlag.Add(validateNumeric(((string)dr["POA-6"]), "POA-6", rowNumber, dr["POA-6"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_6 = importValue; 
                        errorFlag.Add(validateNumeric(((string)dr["POA-7"]), "POA-7", rowNumber, dr["POA-7"] is DBNull, logErrorFlag, out importValue));
                        addUnit.poa_7 = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average GHI (w/m²)"]), "Average GHI (w/m²)", rowNumber, dr["Average GHI (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_ghi = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Average POA (w/m²)"]), "Average POA (w/m²)", rowNumber, dr["Average POA (w/m²)"] is DBNull, logErrorFlag, out importValue));
                        addUnit.avg_poa = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Ambient Temp"]), "Ambient Temp", rowNumber, dr["Ambient Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.amb_temp = importValue;
                        errorFlag.Add(validateNumeric(((string)dr["Module Temp"]), "Module Temp", rowNumber, dr["Module Temp"] is DBNull, logErrorFlag, out importValue));
                        addUnit.mod_temp = importValue;
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    //pending check error
                    //set IsPyro1 flag
                    if (!(errorCount > 0))
                    {
                        //set the  validationgeneration sucess flag
                        m_ErrorLog.SetInformation(",Solar PyranoMeter15Min Validation Successful");
                        isPyro15ValidationSuccess = true;
                        responseCode = 200;
                        pyro15Json = JsonConvert.SerializeObject(addSet);
                    }
                    else
                    {
                        // add to error log that validation of generation failed
                        status = "";
                        m_ErrorLog.SetError(",Solar PyranoMeter15Min Validation Failed");
                        isPyro15ValidationSuccess = false;
                    }
                }
            }
            catch (Exception e)
            {
                m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarPyranoMeter15Min");
                ErrorLog(",Exception Occurred In Function: InsertSolarPyranoMeter15Min: " + e.Message);
            }
            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyJMR(string status, DataSet ds)
        {
            long rowNumber = 0;
            int errorCount = 0;
            int responseCode = 400;
            //bool errorInRow = false;
            //bool bValidationFailed = false;
            List<bool> errorFlag = new List<bool>();
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);

                List<SolarMonthlyJMR> addSet = new List<SolarMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        bool skipRow = false;
                        SolarMonthlyJMR addUnit = new SolarMonthlyJMR();
                        rowNumber++;
                        addUnit.Site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : (string)dr["Site"];
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.Site) ? Convert.ToInt32(siteNameId[addUnit.Site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.site_id, rowNumber));

                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : (string)(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        //addUnit.JMR_date = (dr["JMR date"] is DBNull) ? "Nil" : Convert.ToDateTime(dr["JMR date"], timeCulture).ToString("yyyy-MM-dd");
                        addUnit.JMR_date = (dr["JMR date"] is DBNull) ? "Nil" : Convert.ToDateTime(dr["JMR date"]).ToString("yyyy-MM-dd");
                        errorFlag.Add(dateNullValidation(addUnit.JMR_date, "JMR date", rowNumber));

                        addUnit.JMR_Month = dr["JMR Month"] is DBNull || string.IsNullOrEmpty((string)dr["JMR Month"]) ? "Nil" : Convert.ToString(dr["JMR Month"]);
                        addUnit.JMR_Month_no = longMonthList.ContainsKey((string)dr["JMR Month"]) ? Convert.ToInt32(longMonthList[addUnit.JMR_Month]) : 0;
                        addUnit.JMR_Year = dr["JMR Year"] is DBNull || string.IsNullOrEmpty((string)dr["JMR Year"]) ? 0 : Convert.ToInt32(dr["JMR Year"]);
                        errorFlag.Add(monthValidation(addUnit.JMR_Month, addUnit.JMR_Month_no, rowNumber));
                        errorFlag.Add(yearValidation(addUnit.JMR_Year, rowNumber));

                        addUnit.Plant_Section = dr["Plant Section"] is DBNull || string.IsNullOrEmpty((string)dr["Plant Section"]) ? "Nil" : Convert.ToString(dr["Plant Section"]);
                        //errorFlag.Add(stringNullValidation(addUnit.Plant_Section, "Plant Section", rowNumber));

                        addUnit.Controller_KWH_INV = Convert.ToDouble((dr["Controller KWH/INV KWH"] is DBNull) ? 0 : dr["Controller KWH/INV KWH"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Controller_KWH_INV, "Controller KWH/INV KWH", rowNumber));

                        addUnit.Scheduled_Units_kWh = Convert.ToDouble(dr["Scheduled Units (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units (kWh)"]) ? 0 : dr["Scheduled Units (kWh)"]);
                       // errorFlag.Add(negativeNullValidation(addUnit.Scheduled_Units_kWh, "Scheduled Units (kWh)", rowNumber));

                        addUnit.Export_kWh = Convert.ToDouble((dr["Export (kWh)"] is DBNull) ? 0 : dr["Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Export_kWh, "Export (kWh)", rowNumber));

                        addUnit.Import_kWh = Convert.ToDouble((dr["Import (kWh)"] is DBNull) ? 0 : dr["Import (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Import_kWh, "Import (kWh)", rowNumber));

                        addUnit.Net_Export_kWh = Convert.ToDouble((dr["Net Export (kWh)"] is DBNull) ? 0 : dr["Net Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Net_Export_kWh, "Net Export (kWh)", rowNumber));

                        addUnit.Net_Billable_kWh = Convert.ToDouble((dr["Net Billable (kWh)"] is DBNull) ? 0 : dr["Net Billable (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Net_Billable_kWh, "Net Billable (kWh)", rowNumber));

                        addUnit.Export_kVAh = Convert.ToDouble((dr["Export (kVAh)"] is DBNull) ? 0 : dr["Export (kVAh)"]);
                        errorFlag.Add(numericNullValidation(addUnit.Export_kVAh, "Export (kVAh)", rowNumber));

                        addUnit.Import_kVAh = Convert.ToDouble((dr["Import (kVAh)"] is DBNull) ? 0 : dr["Import (kVAh)"]);
                        errorFlag.Add(numericNullValidation(addUnit.Import_kVAh, "Import (kVAh)", rowNumber));

                        addUnit.Export_kVArh_lag = Convert.ToDouble((dr["Export (kVArh lag)"] is DBNull) ? 0 : dr["Export (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Export_kVArh_lag, "Export (kVArh lag)", rowNumber));

                        addUnit.Import_kVArh_lag = Convert.ToDouble((dr["Import (kVArh lag)"] is DBNull) ? 0 : dr["Import (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Import_kVArh_lag, "Import (kVArh lag)", rowNumber));

                        addUnit.Export_kVArh_lead = Convert.ToDouble((dr["Export (kVArh lead)"] is DBNull) ? 0 : dr["Export (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Export_kVArh_lead, "Export (kVArh lead)", rowNumber));

                        addUnit.Import_kVArh_lead = Convert.ToDouble((dr["Import (kVArh lead)"] is DBNull) ? 0 : dr["Import (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.Import_kVArh_lead, "Import (kVArh lead)", rowNumber));

                        addUnit.LineLoss = Convert.ToDouble((dr["LineLoss"] is DBNull) ? 0 : dr["LineLoss"]);
                        errorFlag.Add(numericNullValidation(addUnit.LineLoss, "LineLoss", rowNumber));

                        addUnit.Line_Loss_percentage = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        errorFlag.Add((addUnit.Line_Loss_percentage > 100 || addUnit.Line_Loss_percentage < 0) ? true : false);

                        addUnit.RKVH_percentage = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        errorFlag.Add((addUnit.RKVH_percentage > 100 || addUnit.RKVH_percentage < 0) ? true : false);

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_JMR(addUnit, addSet, rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyJMR");
                        ErrorLog(",Exception Occurred In Function: InsertSolarMonthlyJMR: " + e.Message);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly JMR Validation Successful");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly JMR Import API Successful");
                            return responseCode = (int)response.StatusCode;

                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly JMR Import API Failed");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly JMR Validation Failed");
                }
            }
            return responseCode;
        }
        //End of all DGR Import functions for both Wind and Solar Upload types
        private async Task<int> InsertWindMonthlyJMR(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindMonthlyJMR> addSet = new List<WindMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindMonthlyJMR addUnit = new WindMonthlyJMR();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.siteId = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.siteId, rowNumber));
                        objImportBatch.importSiteId = addUnit.siteId;

                        addUnit.plantSection = string.IsNullOrEmpty((string)dr["Plant Section"]) ? "Nil" : Convert.ToString(dr["Plant Section"]);
                        //errorFlag.Add(stringNullValidation(addUnit.plantSection, "Plant Section", rowNumber));

                        addUnit.jmrDate = string.IsNullOrEmpty((string)dr["JMR date"]) ? "Nil" : Convert.ToString(dr["JMR date"]);
                        errorFlag.Add(dateNullValidation(addUnit.jmrDate, "JMR date", rowNumber));
                        addUnit.jmrDate = errorFlag[0] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["JMR date"]).ToString("yyyy-MM-dd");

                        addUnit.jmrMonth = string.IsNullOrEmpty((string)dr["JMR Month"]) ? "Nil" : Convert.ToString(dr["JMR Month"]);
                        addUnit.jmrMonth_no = longMonthList.Contains(addUnit.jmrMonth) ? Convert.ToInt32(longMonthList[addUnit.jmrMonth]) : 0;
                        errorFlag.Add(monthValidation(addUnit.jmrMonth, addUnit.jmrMonth_no, rowNumber));

                        addUnit.jmrYear = string.IsNullOrEmpty((string)dr["JMR Year"]) ? 0 : Convert.ToInt32(dr["JMR Year"]);
                        errorFlag.Add(yearValidation(addUnit.jmrYear, rowNumber));

                        addUnit.lineLossPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        errorFlag.Add((addUnit.lineLossPercent > 100 || addUnit.lineLossPercent < 0) ? true : false);

                        addUnit.rkvhPercent = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                        errorFlag.Add((addUnit.rkvhPercent > 100 || addUnit.rkvhPercent < 0) ? true : false);

                        addUnit.controllerKwhInv = Convert.ToDouble(dr["Controller KWH/INV KWH"] is DBNull || string.IsNullOrEmpty((string)dr["Controller KWH/INV KWH"]) ? 0 : dr["Controller KWH/INV KWH"]);
                        errorFlag.Add(negativeNullValidation(addUnit.controllerKwhInv, "Controller KWH/INV KWH", rowNumber));

                        addUnit.scheduledUnitsKwh = Convert.ToDouble(dr["Scheduled Units  (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units  (kWh)"]) ? 0 : dr["Scheduled Units  (kWh)"]);
                        //errorFlag.Add(negativeNullValidation(addUnit.scheduledUnitsKwh, "Scheduled Units  (kWh)", rowNumber));

                        addUnit.exportKwh = Convert.ToDouble(dr["Export (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kWh)"]) ? 0 : dr["Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.exportKwh, "Export (kWh)", rowNumber));

                        addUnit.importKwh = Convert.ToDouble(dr["Import (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kWh)"]) ? 0 : dr["Import (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.importKwh, "Import (kWh)", rowNumber));

                        addUnit.netExportKwh = Convert.ToDouble(dr["Net Export (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Net Export (kWh)"]) ? 0 : dr["Net Export (kWh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.netExportKwh, "Net Export (kWh)", rowNumber));

                        addUnit.exportKvah = Convert.ToDouble(dr["Export (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Net Export (kWh)"]) ? 0 : dr["Export (kVAh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.exportKvah, "Export (kVAh)", rowNumber));

                        addUnit.importKvah = Convert.ToDouble(dr["Import (kVAh)"] is DBNull || string.IsNullOrEmpty((string)dr["Net Export (kWh)"]) ? 0 : dr["Import (kVAh)"]);
                        errorFlag.Add(negativeNullValidation(addUnit.importKvah, "Import (kVAh)", rowNumber));

                        addUnit.exportKvarhLag = Convert.ToDouble(dr["Export (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lag)"]) ? 0 : dr["Export (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.exportKvarhLag, "Export (kVArh lag)", rowNumber));

                        addUnit.importKvarhLag = Convert.ToDouble(dr["Import (kVArh lag)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lag)"]) ? 0 : dr["Import (kVArh lag)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.importKvarhLag, "Import (kVArh lag)", rowNumber));

                        addUnit.exportKvarhLead = Convert.ToDouble(dr["Export (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Export (kVArh lead)"]) ? 0 : dr["Export (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.exportKvarhLead, "Export (kVArh lead)", rowNumber));

                        addUnit.importKvarhLead = Convert.ToDouble(dr["Import (kVArh lead)"] is DBNull || string.IsNullOrEmpty((string)dr["Import (kVArh lead)"]) ? 0 : dr["Import (kVArh lead)"]);
                        //errorFlag.Add(numericNullValidation(addUnit.importKvarhLead, "Import (kVArh lead)", rowNumber));

                        addUnit.lineLoss = Convert.ToDouble(dr["LineLoss"] is DBNull || string.IsNullOrEmpty((string)dr["LineLoss"]) ? 0 : dr["LineLoss"]);
                        errorFlag.Add(numericNullValidation(addUnit.lineLoss, "LineLoss", rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File row <" + rowNumber + "> exception type <" + e.GetType() + ">: Function: InsertWindMonthlyJMR");
                        ErrorLog(",Exception <" + e.GetType() + "> Occurred In Function: InsertWindMonthlyJMR: " + e.Message);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly JMR Validation Successful");
                    //api call used for importing wind:monthly jmr client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly JMR Import API Successful:");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly JMR Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of generation failed
                    m_ErrorLog.SetError(",Wind Monthly JMR Validation Failed");
                }
            }

            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyLineLoss(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            long rowNumber = 1;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarMonthlyUploadingLineLosses> addSet = new List<SolarMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarMonthlyUploadingLineLosses addUnit = new SolarMonthlyUploadingLineLosses();
                        bool skipRow = false;
                        rowNumber++;

                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.Month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.Month) ? Convert.ToInt32(MonthList[addUnit.Month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.Month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.FY.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? year : year += 1;
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        addUnit.LineLoss = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss (%)"]), "Line Loss (%)");
                        if (addUnit.LineLoss > 100)
                        {
                            m_ErrorLog.SetError("," + ": Line loss can not be more than 100,");
                            errorFlag.Add(true);
                        }

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_LineLoss(addUnit, addSet, rowNumber));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyLineLoss,");
                        ErrorLog(",Exception Occurred In Function: InsertSolarMonthlyLineLoss: " + e.Message);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly Lineloss Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly Lineloss Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly Lineloss Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly Lineloss Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Solar Monthly Lineloss File Empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyLineLoss(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindMonthlyUploadingLineLosses> addSet = new List<WindMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindMonthlyUploadingLineLosses addUnit = new WindMonthlyUploadingLineLosses();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["FY"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.month = string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.month) ? Convert.ToInt32(MonthList[addUnit.month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));

                        int finalYear = errorFlag[0] == false ? Convert.ToInt32(addUnit.fy.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? finalYear : finalYear = +1;

                        addUnit.lineLoss = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                        //errorFlag.Add((addUnit.lineLoss > 100 || addUnit.lineLoss < 0) ? true : false);
                        if (addUnit.lineLoss > 100)
                        {
                            m_ErrorLog.SetError("," + ": Line loss can not be more than 100,");
                            errorFlag.Add(true);
                        }
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindMonthlyLineLoss,");
                        ErrorLog(",Exception Occurred In Function: InsertWindMonthlyLineLoss: " + e.Message);
                        errorCount++;
                    }
                }
                //validation success
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly Line Loss Validation Successful,");
                    //api call used for importing wind:monthly linelosses client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly Line Loss Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly Line Loss Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    m_ErrorLog.SetError(",Wind Monthly Line Loss Validation Failed,");
                }
            }
            else
            {
                // add to error log that validation of file failed
                m_ErrorLog.SetError(",Wind Monthly Line Loss File empty,");
            }
            return responseCode;
        }

        private async Task<int> InsertSolarMonthlyTargetKPI(string status, DataSet ds)
        {
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarMonthlyTargetKPI> addSet = new List<SolarMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarMonthlyTargetKPI addUnit = new SolarMonthlyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        //addUnit.month_no = Convert.ToInt32(MonthList[addUnit.Month]);
                        addUnit.Month = dr["Month"] is DBNull || string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = MonthList.ContainsKey(addUnit.Month) ? Convert.ToInt32(MonthList[addUnit.Month]) : 0;
                        errorFlag.Add(monthValidation(addUnit.Month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.FY.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no > 3) ? year : year += 1;
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        //addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);
                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.GHI = Convert.ToDouble((dr[3] is DBNull) || string.IsNullOrEmpty((string)dr[3]) ? 0 : dr[3]);
                        errorFlag.Add(numericNullValidation(addUnit.GHI, "GHI", rowNumber));

                        addUnit.POA = Convert.ToDouble((dr[4] is DBNull) || string.IsNullOrEmpty((string)dr[4]) ? 0 : dr[4]);
                        errorFlag.Add(numericNullValidation(addUnit.POA, "POA", rowNumber));

                        addUnit.kWh = Convert.ToDouble((dr[5] is DBNull) || string.IsNullOrEmpty((string)dr[5]) ? 0 : dr[5]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kWh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA (%)"]), "MA (%)");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);

                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA (%)"]), "IGA (%)");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);

                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA (%)"]), "EGA (%)");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);

                        addUnit.PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PR (%)"]), "PR (%)");
                        errorFlag.Add((addUnit.PR > 100 || addUnit.PR < 0) ? true : false);

                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF (%)"]), "PLF (%)");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);

                        errorFlag.Add(uniqueRecordCheckSolarPerMonthYear_KPI(addUnit, addSet, rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarMonthlyTargetKPI,");
                        ErrorLog(",Exception Occurred In Function: InsertSolarMonthlyTargetKPI: " + e.Message + ",");
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Monthly Target KPI Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Monthly Target KPI Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Monthly Target KPI Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Monthly Target KPI Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Solar Monthly Target KPI File empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyTargetKPI(string status, DataSet ds)
        {
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyTargetKPI> addSet = new List<WindMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                        WindMonthlyTargetKPI addUnit = new WindMonthlyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.fy = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.fy, rowNumber));

                        addUnit.month = string.IsNullOrEmpty((string)dr["Month"]) ? "Nil" : Convert.ToString(dr["Month"]);
                        addUnit.month_no = string.IsNullOrEmpty((string)dr["Month"]) ? 0 : Convert.ToInt32(MonthList[addUnit.month]);
                        errorFlag.Add(monthValidation(addUnit.month, addUnit.month_no, rowNumber));

                        int year = errorFlag[0] == false ? Convert.ToInt32(addUnit.fy.Substring(0, 4)) : 0;
                        addUnit.year = (addUnit.month_no < 4 ? year += 1 : year);
                        errorFlag.Add(yearValidation(addUnit.year, rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Sites"]) ? "Nil" : Convert.ToString(dr["Sites"]);
                        addUnit.site_id = string.IsNullOrEmpty(addUnit.site) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);//C
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.windSpeed = string.IsNullOrEmpty((string)dr["WindSpeed"]) ? 0 : Convert.ToDouble(dr["WindSpeed"]);
                        errorFlag.Add(numericNullValidation(addUnit.windSpeed, "WindSpeed", rowNumber));

                        addUnit.kwh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        errorFlag.Add(negativeNullValidation(addUnit.kwh, "kwh", rowNumber));

                        addUnit.ma = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                        errorFlag.Add((addUnit.ma > 100 || addUnit.ma < 0) ? true : false);
                        addUnit.iga = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                        errorFlag.Add((addUnit.iga > 100 || addUnit.iga < 0) ? true : false);
                        addUnit.ega = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                        errorFlag.Add((addUnit.ega > 100 || addUnit.ega < 0) ? true : false);
                        addUnit.plf = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                        errorFlag.Add((addUnit.ega > 100 || addUnit.plf < 0) ? true : false);
                        errorFlag.Add(MonthList.Contains(addUnit.month) ? false : true);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindMonthlyTargetKPI,");
                        ErrorLog(",Exception Occurred In Function: InsertWindMonthlyTargetKPI: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Monthly Target KPI Validation Successful,");
                    //api call used for importing wind:monthly target kpi client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Monthly Target KPI Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Monthly Target KPI Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    m_ErrorLog.SetError(",Wind Monthly Target KPI Validation Failed,");
                }
            }
            else
            {
                m_ErrorLog.SetError(",Wind Monthly Target KPI File empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertSolarDailyLoadShedding(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyLoadShedding> addSet = new List<SolarDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarDailyLoadShedding addUnit = new SolarDailyLoadShedding();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.Site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.Site_Id = siteNameId.ContainsKey(addUnit.Site) ? Convert.ToInt32(siteNameId[addUnit.Site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.Site_Id, rowNumber));
                        objImportBatch.importSiteId = addUnit.Site_Id;//C

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");


                        addUnit.Start_Time = (string.IsNullOrEmpty((string)dr["Start Time"])) ? "Nil" : Convert.ToString(dr["Start Time"]);
                        errorFlag.Add(timeValidation(addUnit.Start_Time, "Start Time", rowNumber));

                        addUnit.End_Time = string.IsNullOrEmpty((string)dr["End Time"]) ? "Nil" : Convert.ToString(dr["End Time"]);
                        errorFlag.Add(timeValidation(addUnit.End_Time, "End Time", rowNumber));

                        addUnit.Total_Time = (string.IsNullOrEmpty((string)dr["Total Time"])) ? "Nil" : Convert.ToString(dr["Total Time"]);
                        errorFlag.Add(timeValidation(addUnit.Total_Time, "Total Time", rowNumber));

                        addUnit.Permissible_Load_MW = Convert.ToDouble(string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]) ? 0 : dr[" Permissible Load (MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.Permissible_Load_MW, " Permissible Load (MW)", rowNumber));

                        addUnit.Gen_loss_kWh = Convert.ToDouble(string.IsNullOrEmpty((string)dr["Generation loss in KWH due to Load shedding"]) ? 0 : dr["Generation loss in KWH due to Load shedding"]);
                        errorFlag.Add(negativeNullValidation(addUnit.Gen_loss_kWh, "Generation loss in KWH due to Load shedding", rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarDailyLoadShedding,");
                        ErrorLog(",Exception Occurred In Function: InsertSolarDailyLoadShedding: " + e.Message);
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Daily Load Shedding Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyLoadShedding";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Daily Load Shedding Import API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Daily Load Shedding Import API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Daily Load Shedding Validation Failed,");
                }

            }
            else
            {
                m_ErrorLog.SetError(",Solar Daily Load Shedding File empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertWindDailyLoadShedding(string status, DataSet ds)
        {
            long rowNumber = 1;
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindDailyLoadShedding> addSet = new List<WindDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindDailyLoadShedding addUnit = new WindDailyLoadShedding();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = string.IsNullOrEmpty(addUnit.site) ? 0 : Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;//C

                        addUnit.date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.date, "Date", rowNumber));
                        addUnit.date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");


                        addUnit.startTime = string.IsNullOrEmpty((string)dr["Start Time"]) ? "Nil" : Convert.ToString(dr["Start Time"]);
                        errorFlag.Add(timeValidation(addUnit.startTime, "Start Time", rowNumber));

                        addUnit.endTime = string.IsNullOrEmpty((string)dr["End Time"]) ? "Nil" : Convert.ToString(dr["End Time"]);
                        errorFlag.Add(timeValidation(addUnit.startTime, "End Time", rowNumber));

                        addUnit.totalTime = string.IsNullOrEmpty((string)dr["Total Time"]) ? "Nil" : Convert.ToString(dr["Total Time"]);
                        errorFlag.Add(timeValidation(addUnit.totalTime, "Total Time", rowNumber));

                        addUnit.permLoad = string.IsNullOrEmpty((string)dr[" Permissible Load (MW)"]) ? 0 : Convert.ToDouble(dr[" Permissible Load (MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.permLoad, " Permissible Load (MW)", rowNumber));

                        addUnit.genShedding = string.IsNullOrEmpty((string)dr["Generation loss in KWH due to Load shedding"]) ? 0 : Convert.ToDouble(dr["Generation loss in KWH due to Load shedding"]);
                        errorFlag.Add(negativeNullValidation(addUnit.genShedding, "Generation loss in KWH due to Load shedding", rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindDailyLoadShedding,");
                        ErrorLog(",Exception Occurred In Function: InsertWindDailyLoadShedding: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Daily Load Shedding Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindDailyLoadShedding";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Daily Load Shedding API Successful,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Daily Load Shedding API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Daily Load Shedding Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Daily Load Shedding File is empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertSolarDailyTargetKPI(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                List<SolarDailyTargetKPI> addSet = new List<SolarDailyTargetKPI>();
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        rowNumber++;
                        SolarDailyTargetKPI addUnit = new SolarDailyTargetKPI();
                        bool skipRow = false;
                        addUnit.FY = dr["FY"] is DBNull || string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        addUnit.Sites = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.Sites) ? Convert.ToInt32(siteNameId[addUnit.Sites]) : 0;
                        errorFlag.Add(siteValidation(addUnit.Sites, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.GHI = Convert.ToDouble((dr[3] is DBNull) ? 0 : dr[3]);
                        errorFlag.Add(numericNullValidation(addUnit.GHI, " GHI", rowNumber));

                        addUnit.POA = Convert.ToDouble((dr[4] is DBNull) ? 0 : dr[4]);
                        errorFlag.Add(numericNullValidation(addUnit.POA, " POA", rowNumber));

                        addUnit.kWh = Convert.ToDouble((dr[5] is DBNull) ? 0 : dr[5]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kwh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA (%)"]), "MA (%)");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);
                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA (%)"]), "IGA (%)");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);
                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA (%)"]), "EGA (%)");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);
                        addUnit.PR = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PR (%)"]), "PR (%)");
                        errorFlag.Add((addUnit.PR > 100 || addUnit.PR < 0) ? true : false);
                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF (%)"]), "PLF (%)");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarDailyTargetKPI");
                        ErrorLog(",Exception Occurred In Function: InsertSolarDailyTargetKPI: " + e.Message);
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar Daily Target KPI Validation SuccessFul");

                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Daily Target KPI API Successful");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Daily Target KPI API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Daily Target KPI Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Solar Daily Target KPI File is empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertWindDailyTargetKPI(string status, DataSet ds)
        {
            long rowNumber = 1;
            List<bool> errorFlag = new List<bool>();
            int errorCount = 0;
            int responseCode = 400;

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindDailyTargetKPI> addSet = new List<WindDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindDailyTargetKPI addUnit = new WindDailyTargetKPI();
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.FY = string.IsNullOrEmpty((string)dr["FY"]) ? "Nil" : Convert.ToString(dr["FY"]);
                        errorFlag.Add(financialYearValidation(addUnit.FY, rowNumber));

                        addUnit.Date = string.IsNullOrEmpty((string)dr["Date"]) ? "Nil" : Convert.ToString(dr["Date"]);
                        errorFlag.Add(dateNullValidation(addUnit.Date, "Date", rowNumber));
                        addUnit.Date = errorFlag[1] ? DateTime.MinValue.ToString("yyyy-MM-dd") : Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");

                        addUnit.Site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = string.IsNullOrEmpty((string)dr["Site"]) ? 0 : Convert.ToInt32(siteNameId[addUnit.Site]);
                        errorFlag.Add(siteValidation(addUnit.Site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.WindSpeed = string.IsNullOrEmpty((string)dr["WindSpeed"]) ? 0 : Convert.ToDouble(dr["WindSpeed"]);
                        errorFlag.Add(numericNullValidation(addUnit.WindSpeed, "WindSpeed", rowNumber));

                        addUnit.kWh = string.IsNullOrEmpty((string)dr["kWh"]) ? 0 : Convert.ToDouble(dr["kWh"]);
                        errorFlag.Add(negativeNullValidation(addUnit.kWh, "kWh", rowNumber));

                        addUnit.MA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                        errorFlag.Add((addUnit.MA > 100 || addUnit.MA < 0) ? true : false);
                        addUnit.IGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                        errorFlag.Add((addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false);
                        addUnit.EGA = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                        errorFlag.Add((addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false);
                        addUnit.PLF = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                        errorFlag.Add((addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindDailyTargetKPI,");
                        ErrorLog(",Exception Occurred In Function: InsertWindDailyTargetKPI: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Daily Target KPI Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Daily Target KPI API Success,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Daily Target KPI API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Daily Target KPI Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Daily Target KPI File is empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertSolarSiteMaster(string status, DataSet ds)
        {
            int responseCode = 400;
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 0;
            int errorCount = 0;
            if (ds.Tables.Count > 0)
            {
                List<SolarSiteMaster> addSet = new List<SolarSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarSiteMaster addUnit = new SolarSiteMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = Convert.ToString(dr["Country"]);
                        errorFlag.Add(stringNullValidation(addUnit.country, "Country", rowNumber));

                        addUnit.site = Convert.ToString(dr["Site"]);
                        errorFlag.Add(stringNullValidation(addUnit.site, "Site", rowNumber));
                        objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C

                        addUnit.spv = Convert.ToString(dr["SPV"]);
                        errorFlag.Add(stringNullValidation(addUnit.spv, "SPV", rowNumber));

                        addUnit.state = Convert.ToString(dr["State"]);
                        errorFlag.Add(stringNullValidation(addUnit.state, "State", rowNumber));

                        addUnit.dc_capacity = Convert.ToDouble(dr["DC Capacity (MWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "DC Capacity (MWp)", rowNumber));

                        addUnit.ac_capacity = Convert.ToDouble(dr["AC Capacity (MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.ac_capacity, "AC Capacity (MW)", rowNumber));

                        addUnit.total_tarrif = Convert.ToDouble(dr["Total Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_tarrif, "Total Tariff", rowNumber));
                        uniqueRecordCheckSolarSiteMaster(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarSiteMaster");
                        ErrorLog(",Exception Occurred In Function: InsertSolarSiteMaster: " + e.Message);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Solar Site Master Validation SuccessFul");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarSiteMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Site Master API SuccessFul");
                            responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Site Master API Failure: responseCode <" + (int)response.StatusCode + ">");
                            responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Site Master Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Solar Site Master File is empty,");
            }
            return responseCode;
        }
        private async Task<int> InsertWindSiteMaster(string status, DataSet ds)
        {
            int errorCount = 0;
            long rowNumber = 1;
            int responseCode = 400;
            List<bool> errorFlag = new List<bool>();

            if (ds.Tables.Count > 0)
            {
                CommonFileValidation commonValidation = new CommonFileValidation(m_ErrorLog, _idapperRepo);
                List<WindSiteMaster> addSet = new List<WindSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindSiteMaster addUnit = new WindSiteMaster();
                        errorFlag.Clear();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = string.IsNullOrEmpty((string)dr["Country"]) ? "Nil" : Convert.ToString(dr["Country"]);
                        errorFlag.Add(countryValidation(addUnit.country, "Country", rowNumber));

                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        errorFlag.Add(stringNullValidation(addUnit.site, "Site", rowNumber));
                        objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C

                        addUnit.spv = string.IsNullOrEmpty((string)dr["SPV"]) ? "Nil" : Convert.ToString(dr["SPV"]);
                        errorFlag.Add(stringNullValidation(addUnit.spv, "SPV", rowNumber));

                        addUnit.state = string.IsNullOrEmpty((string)dr["State"]) ? "Nil" : Convert.ToString(dr["State"]);
                        errorFlag.Add(stringNullValidation(addUnit.state, "State", rowNumber));

                        addUnit.model = string.IsNullOrEmpty((string)dr["Model"]) ? "Nil" : Convert.ToString(dr["Model"]);
                        errorFlag.Add(stringNullValidation(addUnit.model, "Model", rowNumber));

                        addUnit.capacity_mw = string.IsNullOrEmpty((string)dr["Capacity(MW)"]) ? 0 : Convert.ToDouble(dr["Capacity(MW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.capacity_mw, "Capacity(MW)", rowNumber));

                        addUnit.wtg = dr["WTG"] is DBNull || string.IsNullOrEmpty((string)dr["WTG"]) ? 0 : Convert.ToDouble(dr["WTG"]);
                        errorFlag.Add(numericNullValidation(addUnit.wtg, "WTG", rowNumber));

                        addUnit.total_mw = string.IsNullOrEmpty((string)dr["Total_MW"]) ? 0 : Convert.ToDouble(dr["Total_MW"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_mw, "Total_MW", rowNumber));

                        addUnit.tarrif = string.IsNullOrEmpty((string)dr["Tariff"]) ? 0 : Convert.ToDouble(dr["Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.tarrif, "Tariff", rowNumber));

                        addUnit.gbi = string.IsNullOrEmpty((string)dr["GBI"]) ? 0 : Convert.ToDouble(dr["GBI"]);
                        // errorFlag.Add(numericNullValidation(addUnit.gbi, "GBI", rowNumber));
                        //addUnit.gbi = Convert.ToDouble((string)dr["GBI"]);
                        // errorFlag.Add(numericNullValidation(addUnit.gbi, "GBI", rowNumber));

                        addUnit.total_tarrif = string.IsNullOrEmpty((string)dr["Total_Tariff"]) ? 0 : Convert.ToDouble(dr["Total_Tariff"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_tarrif, "Total_Tariff", rowNumber));

                        addUnit.ll_compensation = commonValidation.stringToPercentage(rowNumber, Convert.ToString(dr["LL_Compensation%"]), "LL_Compensation%");
                        uniqueRecordCheckWindSiteMaster(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindSiteMaster,");
                        ErrorLog(",Exception Occurred In Function: InsertWindSiteMaster: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Wind Site Master Validation Successful,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindSiteMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Site Master API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Site Master API Failure: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Site Master Validation Failed,");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Site Master File is empty,");
            }
            return responseCode;
        }

        private async Task<int> InsertSolarLocationMaster(string status, DataSet ds)
        {//siteID recorded
            int errorCount = 0;
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarLocationMaster> addSet = new List<SolarLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        SolarLocationMaster addUnit = new SolarLocationMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.country = dr["Country"] is DBNull || string.IsNullOrEmpty((string)dr["Country"]) ? "Nil" : Convert.ToString(dr["Country"]);
                        errorFlag.Add(countryValidation(addUnit.country, "Country", rowNumber));
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.eg = Convert.ToString(dr["EG"]);
                        errorFlag.Add(stringNullValidation(addUnit.eg, "EG", rowNumber));

                        addUnit.ig = Convert.ToString(dr["IG"]);
                        errorFlag.Add(stringNullValidation(addUnit.ig, "IG", rowNumber));

                        addUnit.icr_inv = Convert.ToString(dr["ICR/INV"]);
                        errorFlag.Add(stringNullValidation(addUnit.icr_inv, "ICR/INV", rowNumber));

                        addUnit.icr = Convert.ToString(dr["ICR"]);
                        errorFlag.Add(stringNullValidation(addUnit.icr, "ICR", rowNumber));

                        addUnit.inv = Convert.ToString(dr["INV"]);
                        errorFlag.Add(stringNullValidation(addUnit.inv, "INV", rowNumber));
                        //SMB can remain Nil
                        addUnit.smb = Convert.ToString(dr["SMB"] is DBNull || string.IsNullOrEmpty((string)dr["SMB"]) ? "Nil" : dr["SMB"]);

                        addUnit.strings = Convert.ToString(dr["String"]);
                        errorFlag.Add(stringNullValidation(addUnit.strings, "String", rowNumber));

                        addUnit.string_configuration = Convert.ToString(dr["String Configuration"] is DBNull || string.IsNullOrEmpty((string)dr["String Configuration"]) ? "Nil" : dr["String Configuration"]);
                        errorFlag.Add(stringNullValidation(addUnit.string_configuration, "String", rowNumber));

                        addUnit.total_string_current = Convert.ToDouble(dr["Total String Current (amp)"] is DBNull || string.IsNullOrEmpty((string)dr["Total String Current (amp)"]) ? 0 : dr["Total String Current (amp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_string_current, "Total String Current (amp)", rowNumber));

                        addUnit.total_string_voltage = Convert.ToDouble(dr["Total String voltage"] is DBNull || string.IsNullOrEmpty((string)dr["Total String voltage"]) ? 0 : dr["Total String voltage"]);
                        errorFlag.Add(numericNullValidation(addUnit.total_string_voltage, "Total String voltage", rowNumber));

                        addUnit.modules_quantity = Convert.ToDouble(dr[12] is DBNull || string.IsNullOrEmpty((string)dr[12]) ? 0 : dr[12]);
                        errorFlag.Add(numericNullValidation(addUnit.modules_quantity, "Modules Qty", rowNumber));

                        addUnit.wp = Convert.ToDouble(dr["Wp"] is DBNull || string.IsNullOrEmpty((string)dr["Wp"]) ? 0 : (dr["Wp"]));
                        errorFlag.Add(numericNullValidation(addUnit.wp, "Wp", rowNumber));

                        addUnit.capacity = Convert.ToDouble(dr["Capacity (KWp)"] is DBNull || string.IsNullOrEmpty((string)dr["Capacity (KWp)"]) ? 0 : dr["Capacity (KWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.modules_quantity, "Modules", rowNumber));

                        addUnit.module_make = Convert.ToString(dr["Module Make"]);
                        errorFlag.Add(stringNullValidation(addUnit.module_make, "Module Make", rowNumber));

                        addUnit.module_model_no = Convert.ToString(dr[16]);
                        errorFlag.Add(stringNullValidation(addUnit.module_model_no, "Module Model No.", rowNumber));

                        addUnit.module_type = Convert.ToString(dr["Module Type"]);
                        errorFlag.Add(stringNullValidation(addUnit.module_type, "Module Type", rowNumber));

                        addUnit.string_inv_central_inv = (Convert.ToString(dr["String Inv / Central Inv"]) == "Central Inverter") ? 2 : 1;
                        errorFlag.Add(stringNullValidation((string)dr["String Inv / Central Inv"], "String Inv / Central Inv", rowNumber));

                        errorFlag.Add(uniqueRecordCheckSolarLocationMaster(addUnit, addSet, rowNumber));

                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertSolarLocationMaster");
                        ErrorLog(",Exception Occurred In Function: InsertSolarLocationMaster: " + e.Message);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Solar Location Master Validation SuccessFul,");

                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarLocationMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar Location Master API SuccessFul");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar Location Master API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar Location Master Validation Failed");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindLocationMaster(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            bool errorInRow = false;
            bool bValidationFailed = false;
            if (ds.Tables.Count > 0)
            {
                List<WindLocationMaster> addSet = new List<WindLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {
                        WindLocationMaster addUnit = new WindLocationMaster();
                        rowNumber++;
                        bool skipRow = false;
                        addUnit.site = string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : (string)dr["Site"];
                        addUnit.site_master_id = siteNameId.ContainsKey(addUnit.site) ? Convert.ToInt32(siteNameId[addUnit.site]) : 0;
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_master_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_master_id;

                        addUnit.wtg = string.IsNullOrEmpty((string)dr["WTG"]) ? "Nil" : (string)dr["WTG"];
                        errorFlag.Add(stringNullValidation(addUnit.wtg, "WTG", rowNumber));

                        addUnit.feeder = string.IsNullOrEmpty((string)dr["Feeder"]) ? 0 : Convert.ToDouble(dr["Feeder"]);
                        errorFlag.Add(numericNullValidation(addUnit.feeder, "Feeder", rowNumber));

                        addUnit.max_kwh_day = string.IsNullOrEmpty((string)dr["Max. kWh/Day"]) ? 0 : Convert.ToDouble(dr["Max. kWh/Day"]);
                        errorFlag.Add(negativeNullValidation(addUnit.max_kwh_day, "Max. kWh/Day", rowNumber));

                        //uniqueRecordCheckWtgWise();
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError("," + e.GetType() + ": Function: InsertWindLocationMaster");
                        ErrorLog(",Exception Occurred In Function: InsertWindLocationMaster: " + e.Message);
                        errorCount++;
                    }
                }
                if (errorCount == 0)
                {
                    m_ErrorLog.SetInformation(",Wind Location Master Validation SuccessFul");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindLocationMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Wind Location Master API SuccessFul");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Wind Location Master API Failed: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Wind Location Master Validation Failed");
                }
            }
            else
            {
                //No data in the file
                m_ErrorLog.SetError(",Wind Location Master File is emplty,");
            }
            return responseCode;
        }

        private async Task<int> InsertSolarAcDcCapacity(string status, DataSet ds)
        {
            List<bool> errorFlag = new List<bool>();
            long rowNumber = 1;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarInvAcDcCapacity> addSet = new List<SolarInvAcDcCapacity>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarInvAcDcCapacity addUnit = new SolarInvAcDcCapacity();
                    try
                    {
                        bool skipRow = false;
                        rowNumber++;
                        addUnit.site = dr["Site"] is DBNull || string.IsNullOrEmpty((string)dr["Site"]) ? "Nil" : Convert.ToString(dr["Site"]);
                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;

                        addUnit.inverter = Convert.ToString(dr["Inverter"]);
                        errorFlag.Add(solarInverterValidation((string)dr["Inverter"], "Inverter", rowNumber));

                        addUnit.dc_capacity = dr["DC Capacity(kWp)"] is DBNull || string.IsNullOrEmpty((string)dr["DC Capacity(kWp)"]) ? 0 : Convert.ToDouble(dr["DC Capacity(kWp)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "DC Capacity(kWp)", rowNumber));

                        addUnit.ac_capacity = dr["AC Capacity (kW)"] is DBNull || string.IsNullOrEmpty((string)dr["AC Capacity (kW)"]) ? 0 : Convert.ToDouble(dr["AC Capacity (kW)"]);
                        errorFlag.Add(numericNullValidation(addUnit.dc_capacity, "AC Capacity (kW)", rowNumber));
                        uniqueRecordCheckAcDcCapacity(addUnit, addSet, rowNumber);
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                skipRow = true;
                                break;
                            }
                        }
                        errorFlag.Clear();
                        if (!(skipRow))
                        {
                            addSet.Add(addUnit);
                        }
                    }
                    catch (Exception e)
                    {
                        //developer errorlog
                        m_ErrorLog.SetError(",File Row<" + rowNumber + ">" + e.GetType() + ": Function: InsertSolarAcDcCapacity,");
                        ErrorLog(",Exception Occurred In Function: InsertSolarAcDcCapacity: " + e.Message + ",");
                        errorCount++;
                    }
                }
                if (!(errorCount > 0))
                {
                    m_ErrorLog.SetInformation(",Solar ACDC Validation SuccessFul,");
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarInvAcDcCapacity";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            m_ErrorLog.SetInformation(",Solar ACDC API SuccessFul,");
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Solar ACDC API Failure,: responseCode <" + (int)response.StatusCode + ">");
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    m_ErrorLog.SetError(",Solar ACDC Validation Failed,");
                }
            }
            return responseCode;
        }

        private async Task importMetaData(string importType, string fileName, FileSheetType.FileImportType fileImportType)
        {
            int responseCode = 400;
            string status = "";
            objImportBatch.importFilePath = fileName;
            objImportBatch.importLogName = importData[1];
            string userName = HttpContext.Session.GetString("DisplayName");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            if (importType == "Solar")
            {
                objImportBatch.importType = "2";
            }
            else if (importType == "Wind")
            {
                objImportBatch.importType = "1";
            }
            objImportBatch.importFileType = (int)fileImportType;
            var json = JsonConvert.SerializeObject(objImportBatch);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/importMetaData?userName=" + userName + "&userId=" + userId;
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, data);
                if (response.IsSuccessStatusCode)
                {
                    status = "Batch Id Created Successfully";
                    m_ErrorLog.SetInformation("," + status + ":");
                    responseCode = (int)response.StatusCode;
                }
                else
                {
                    status = "Batch Id Creation API Failed";
                    m_ErrorLog.SetInformation("," + status + ":");
                    responseCode = (int)response.StatusCode;
                }
            }

            if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min"))
            {
                var urlGetId = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchId?logFileName=" + importData[1] + "";
                var result = string.Empty;
                WebRequest request = WebRequest.Create(urlGetId);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                        result = readStream.ReadToEnd();
                    }
                    BatchIdImport obj = new BatchIdImport();
                    obj = JsonConvert.DeserializeObject<BatchIdImport>(result);
                    batchIdDGRAutomation = obj.import_batch_id;
                    if (batchIdDGRAutomation == 0)
                    {
                        m_ErrorLog.SetError(",BatchId not returned successfully,");
                    }
                    else
                    {
                        m_ErrorLog.SetInformation(",BatchId <" + batchIdDGRAutomation + "> created successfully,");
                    }
                }
            }
        }
        //HashTable List
        public void masterHashtable_WtgToWtgId()
        {
            //fills a hashtable with key = wtg and value = location_master_id from table : Wind Location Master
            //gets equipmentID from equipmentName in Wind Location Master
            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster";
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
                int wtgId = (int)Convert.ToInt64(dr["location_master_id"]);//D
                equipmentId.Add((string)dr["wtg"], wtgId);
                double max_kWh = Convert.ToDouble(dr["max_kwh_day"]);
                maxkWhMap_wind.Add(wtgId, max_kWh);
            }
        }
        public void masterHashtable_BDNameToBDId()
        {
            //fills a hashtable with key = bdTypeName and value = bdTypeId from table : BDType 
            //gets breakdownId from breakdownName in BDType

            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBDType";
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
            breakdownType.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                int bd_type_id = (int)Convert.ToInt64(dr["bd_type_id"]);//D
                breakdownType.Add((string)dr["bd_type_name"], bd_type_id);
            }
        }
        /*
         * this function was causing duplicate API calls to get sitemaster date. Merged into masterHashtable_SiteIdToSiteName
        public void masterHashtable_SiteName_To_SiteId()
        {
            //fills a hashtable with as key = siteName and value = siteId from table : Wind Site Master
            //gets siteID from siteName in Wind Site Master

            siteNameId.Clear();
            DataTable dTable = new DataTable();
            var url = "";
            if (importData[0] == "Solar")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster";
            }
            else if (importData[0] == "Wind")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster";
            }
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

            if (importData[0] == "Solar")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
        }
        */
        public void masterInverterList()
        {
            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarLocationMaster";
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
            inverterList.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                inverterList.Add((string)dr["icr_inv"]);
            }
        }
        public void masterHashtable_SiteIdToSiteName()
        {
            //fills a hashtable with as key = siteId and value = siteNameId from table : Wind Site Master
            //gets siteName from siteId in Wind Site Master

            siteName.Clear();
            DataTable dTable = new DataTable();
            var url = "";
            if (importData[0] == "Solar")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster";
            }
            else if (importData[0] == "Wind")
            {
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster";
            }
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
            if (importData[0] == "Solar")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D
                    siteName.Add(siteMasterId, (string)dr["site"]);
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteName.Add(siteMasterId, (string)dr["site"]);
                    siteNameId.Add((string)dr["site"], siteMasterId);
                }
            }
        }
        public void masterHashtable_WtgToSiteId()
        {
            //fills a hashtable with as key = siteId and value = siteNameId from table : Wind Site Master
            //gets siteId from wtg(equipment) in Wind Location Master

            DataTable dTable = new DataTable();
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindLocationMaster";
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

            eqSiteId.Clear();
            foreach (DataRow dr in dTable.Rows)
            {
                int siteMasterId = (int)Convert.ToInt64(dr["site_master_id"]);//D
                eqSiteId.Add((string)dr["wtg"], siteMasterId);
            }
        }
        private async Task<bool> UploadFileToImportedFileFolder(IFormFile ufile)
        {
            bool retValue = false;
            if (ufile != null && ufile.Length > 0)
            {
                var fileName = Path.GetFileName(ufile.FileName);
                var filePath = env.ContentRootPath + @"C:\ImportedFile\" + fileName;
                //var filePath = env.ContentRootPath + @"\ImportedFile\" + fileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ufile.CopyToAsync(fileStream);
                }
                retValue = true;
            }
            return retValue;
        }

        public async Task<int> dgrSolarImport(int batchId)
        {
            ErrorLog("Inside dgrSolarImport<br>\r\n");
            int responseCodeGen = 0;
            int responseCodeBreak = 0;
            int responseCodePyro1 = 0;
            int responseCodePyro15 = 0;
            var dataGeneration = new StringContent(genJson, Encoding.UTF8, "application/json");
            var urlGeneration = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileGeneration?batchId=" + batchId + "";
            ErrorLog("Gen Url<br>\r\n" + urlGeneration);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlGeneration, dataGeneration);

                if (response.IsSuccessStatusCode)
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Solar Gen Import API Successful");
                    ErrorLog("InsertSolarUploadingFileGeneration True\r\n");
                }
                else
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Gen Import API Failed");
                    ErrorLog("InsertSolarUploadingFileGeneration False. Error code <" + responseCodeGen + ">\r\n");
                }
            }

            var dataBreakdown = new StringContent(breakJson, Encoding.UTF8, "application/json");
            var urlBreakdown = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileBreakDown?batchId=" + batchId + "";
            ErrorLog("BreakDown\r\n" + urlBreakdown);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlBreakdown, dataBreakdown);
                if (response.IsSuccessStatusCode)
                {
                    ErrorLog("InsertSolarUploadingFileBreakDown True\r\n");
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Solar Break Import API Successful");
                }
                else
                {
                    ErrorLog("InsertSolarUploadingFileBreakDown False\r\n");
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Break Import API Failed. Error code <"+ responseCodeBreak + ">");
                }
            }

            var dataPyro1Min = new StringContent(pyro1Json, Encoding.UTF8, "application/json");
            var urlPyro1Min = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter1Min?batchId=" + batchId + "";
            ErrorLog("1Min\r\n" + urlPyro1Min);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlPyro1Min, dataPyro1Min);
                if (response.IsSuccessStatusCode)
                {
                    ErrorLog("True\r\n");
                    responseCodePyro1 = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Solar Pyro-1 Import API Successful");
                }
                else
                {
                    ErrorLog("False\r\n");
                    responseCodePyro1 = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Pyro-1 Import API Failed. Error code <" + responseCodePyro1 + ">");
                }
            }

            var dataPyro15Min = new StringContent(pyro15Json, Encoding.UTF8, "application/json");
            var urlPyro15Min = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter15Min?batchId=" + batchId + "";
            ErrorLog("15Min\r\n" + urlPyro15Min);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlPyro15Min, dataPyro15Min);
                if (response.IsSuccessStatusCode)
                {
                    ErrorLog("true\r\n");
                    responseCodePyro15 = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Solar Pyro-15 Import API Successful");
                }
                else
                {
                    ErrorLog("False\r\n");
                    responseCodePyro15 = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Pyro-15 Import API Failed. Error code <" + responseCodePyro15 + ">");
                }
            }
            if (responseCodeGen != 200 || responseCodeBreak != 200 || responseCodePyro1 != 200 || responseCodePyro15 != 200)
            {
                //pending : Need to add code to cleanup import that is failed.
                return 400;
            }
            else
            {
                return 200;
            }
        }
        public async Task<int> dgrWindImport(int batchId)
        {
            int responseCodeGen = 0;
            int responseCodeBreak = 0;

            var dataGeneration = new StringContent(genJson, Encoding.UTF8, "application/json");
            var urlGeneration = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileGeneration?batchId=" + batchId + "";
            //ErrorLog(urlGeneration);
            // ErrorLog(breakJson);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlGeneration, dataGeneration);
                if (response.IsSuccessStatusCode)
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Wind Generation Import API Successful");
                }
                else
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind Generation Import API Failed. Error code <" + responseCodeGen + ">\r\n");
                }
            }
            var dataBreakdown = new StringContent(breakJson, Encoding.UTF8, "application/json");
            var urlBreakdown = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileBreakDown?batchId=" + batchId + "";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlBreakdown, dataBreakdown);
                if (response.IsSuccessStatusCode)
                {
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetInformation(",Wind BreakDown Import API Successful:");
                }
                else
                {
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind BreakDown Import API Failed. Error code <" + responseCodeBreak + ">\r\n");
                }
            }
            if (responseCodeGen != 200 || responseCodeBreak != 200)
            {
                return 400;
            }
            else
            {
                return 200;
            }
        }

        private void ErrorLog(string Message)
        {
            System.IO.File.AppendAllText(@"C:\LogFile\test.txt", Message);
        }
        //Validation Functions

        public bool uniqueRecordCheckWtgWise(string value, string columnName, long rowNo, Hashtable equipmentList)
        {
            bool retValue = false;
            if (equipmentList.ContainsKey(value))
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> column <" + columnName + ">: WTG <" + value + "> already exists. Data will be updated, ");
            }
            return retValue;
        }

        public bool uniqueRecordCheckSolarLocationMaster(SolarLocationMaster currentRecord, List<SolarLocationMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarLocationMaster existingRecord = new SolarLocationMaster();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tableRecord => tableRecord.site_id.Equals(currentRecord.site_id) && tableRecord.icr.Equals(currentRecord.icr) && tableRecord.inv.Equals(currentRecord.inv) && tableRecord.smb.Equals(currentRecord.smb) && tableRecord.strings.Equals(currentRecord.strings));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> Duplicate record error: There already exists a row in excel sheet with following values: Site<" + currentRecord.site + ">; ICR<" + currentRecord.icr + ">; INV<" + currentRecord.inv + ">; SMB<" + currentRecord.smb + ">; Strings<" + currentRecord.strings + ">,");
                retVal = true;
            }
            return retVal;
        }

        public bool uniqueRecordCheckSolarPerMonthYear_JMR(SolarMonthlyJMR thisRecord, List<SolarMonthlyJMR> tableData, long rowNo)
        {
            bool retValue = false;
            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyJMR existingRecord = tableData.Find(tableRecord => tableRecord.site_id.Equals(thisRecord.site_id) && tableRecord.JMR_Year.Equals(thisRecord.JMR_Year) && tableRecord.JMR_Month_no.Equals(thisRecord.JMR_Month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Site + ">, year<" + thisRecord.JMR_Year + ">, month<" + thisRecord.JMR_Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
        }

        public bool uniqueRecordCheckSolarPerMonthYear_LineLoss(SolarMonthlyUploadingLineLosses thisRecord, List<SolarMonthlyUploadingLineLosses> tableData, long rowNo)
        {
            bool retValue = false;
            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyUploadingLineLosses existingRecord = tableData.Find(tableRecord => tableRecord.Site_Id.Equals(thisRecord.Site_Id) && tableRecord.year.Equals(thisRecord.year) && tableRecord.month_no.Equals(thisRecord.month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Sites + ">, year<" + thisRecord.year + ">, month<" + thisRecord.Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
            //SetInformation instead of error : 
        }

        public bool uniqueRecordCheckSolarPerMonthYear_KPI(SolarMonthlyTargetKPI thisRecord, List<SolarMonthlyTargetKPI> tableData, long rowNo)
        {
            bool retValue = false;

            //checks if recordSet contains record that matches current record being checked
            SolarMonthlyTargetKPI existingRecord = tableData.Find(tableRecord => tableRecord.Site_Id.Equals(thisRecord.Site_Id) && tableRecord.year.Equals(thisRecord.year) && tableRecord.month_no.Equals(thisRecord.month_no));
            if (existingRecord != null)
            {
                //JMR record already exists
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for site<" + thisRecord.Sites + ">, year<" + thisRecord.year + ">, month<" + thisRecord.Month + "> already exists in a previous excel data row. Data would be updated");
            }
            return retValue;
        }
        public bool uniqueRecordCheckAcDcCapacity(SolarInvAcDcCapacity currentRecord, List<SolarInvAcDcCapacity> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarInvAcDcCapacity existingRecord = new SolarInvAcDcCapacity();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tSite => (tSite.site_id == currentRecord.site_id && tSite.inverter == currentRecord.inverter));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + ">; Inverter <" + currentRecord.inverter + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }

        public bool uniqueRecordCheckSolarSiteMaster(SolarSiteMaster currentRecord, List<SolarSiteMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            SolarSiteMaster existingRecord = new SolarSiteMaster();
            //checks if recordSet contains record that that matches current record being checked
            existingRecord = recordSet.Find(tableRecord => tableRecord.site.Equals(currentRecord.site));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }
        public bool uniqueRecordCheckWindSiteMaster(WindSiteMaster currentRecord, List<WindSiteMaster> recordSet, long rowNo)
        {
            bool retVal = false;
            WindSiteMaster existingRecord = new WindSiteMaster();
            existingRecord = recordSet.Find(tableRecord => tableRecord.site.Equals(currentRecord.site));
            if (existingRecord != null)
            {
                m_ErrorLog.SetInformation(",File row <" + rowNo + "> data for Site<" + currentRecord.site + "> already exists in a previous excel data row. Data would be updated,");
                retVal = true;
            }
            return retVal;
        }


        public bool uniformDateValidation(string date, int siteId, dynamic recordSet)
        {
            bool retVal = false;
            //doubt as to how date and site should be checked
            return retVal;
        }
        public bool uniformWindSiteValidation(long rowNo, int siteId, string wtg)
        {
            bool invalidSiteUniformity = false;

            if (rowNo == 2)
            {
                previousSite = siteId;
                if (fileSheets.Contains("Uploading_File_Generation") && importData[0] == "Wind")
                {
                    if (siteId == 0)
                    {
                        m_ErrorLog.SetError(",File Row <" + rowNo + "> Invalid Site Id: " + siteId + " due to invalid wtg: " + wtg);
                    }
                    //else if (windSiteUserAccess.IndexOf(siteId)<-1)
                    else if (windSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        //m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                        invalidSiteUniformity = true;
                    }
                    
                }
                else if(fileSheets.Contains("Uploading_File_Generation") && importData[0] == "Solar")
                {
                    if (siteId == 0)
                    {
                        m_ErrorLog.SetError(",File Row <" + rowNo + "> Invalid Site Id: " + siteId + " due to invalid wtg: " + wtg);
                    }
                    //else if (solarSiteUserAccess.IndexOf(siteId) < -1)
                    else if (solarSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                        invalidSiteUniformity = true;
                    }
                }
                else
                {
                   /*if (windSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId] + ",");
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId] + ",");
                        invalidSiteUniformity = true;
                    }*/
                }
                //valdiate if the user has access to this site
                //Collection of site for upload access
                //Check if this siteid is in the above collection of sites accessible to the user
                //Get Site name should be in csv file
            }
            if (previousSite != siteId)
            {
                //pending log error in csv
                m_ErrorLog.SetError(",File Row <" + rowNo + "> Site entry is not the same as in other rows,");
                invalidSiteUniformity = true;
            }
            return invalidSiteUniformity;
        }
        public bool importDateValidation(int importType, int siteID, DateTime importDate)
        {
            bool retValue = false;
            DateTime dtToday = DateTime.Now;
            DateTime dtImportDate = Convert.ToDateTime(importDate);
            TimeSpan dayDiff = dtToday - dtImportDate;
            int dayOfWeek = (int)dtToday.DayOfWeek;

            
            //for DayOfWeek function 
            //if it's not true that file-date is of previous day and today is from Tuesday-Friday
            //&& dayOfWeek > 1 && dayOfWeek < 6
            if (!(dayDiff.Days >= 0 && dayDiff.Days <= 5))
            {
                if(siteUserRole == "Admin")
                {
                    m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the admin user can import it.");
                }
                else
                {
                    // file date is incorrect
                    m_ErrorLog.SetInformation(",The import date <" + importDate + ">  is more than 5 days older but the site user cannot import it.");
                    retValue = true;
                }

            }
            if(retValue == false)
            {
                //if date is within 5 days
                //Check if the data is already import and/or Approved
                
                int IBStatus = 0;
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetBatchStatus?site_id=" + siteID + "&import_type=" + importType + "&import_date=" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd");
                
                string result ="";
               // string line = "";
                WebRequest request = WebRequest.Create(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    Stream receiveStream = response.GetResponseStream();
                    using (StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8))
                    {
                       // result = readStream.ReadToEnd();
                        result = readStream.ReadToEnd().Trim();
                    }
                    //ImportBatchStatus obj = new ImportBatchStatus();
                    //obj = JsonConvert.DeserializeObject<ImportBatchStatus>(result);
                    //obj = JsonConvert.DeserializeObject<ImportBatchStatus>(result);
                    
                    IBStatus = Convert.ToInt32(result);
                    if (IBStatus == 1)
                    {
                        if (siteUserRole == "Admin")
                        {
                            m_ErrorLog.SetInformation(",Data for <" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd") + "> exist in database and is already approved but the admin user can reimport it.");
                        }
                        else
                        {
                            m_ErrorLog.SetError(",Data for <" + Convert.ToDateTime(importDate).ToString("yyyy-MM-dd") + "> exist in database and is already approved. The site user cannot reimport it.");
                            retValue = true;
                        }
                    }
                }
            }
            return retValue;
        }
        public bool wtgValidation(string wtg, int wtgId, long rowNumber)
        {
            bool retVal = false;
            if (stringNullValidation(wtg, "WTG", rowNumber))
            {
                retVal = true;
            }
            else if (!(equipmentId.ContainsKey(wtg)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid WTG <" + wtg + "> is not found in master records");
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Site due to invalid <" + wtg + "> not found in master records");
            }
            return retVal;
        }
        public bool siteValidation(string site, int siteId, long rowNumber)
        {
            bool retVal = false;
            if (stringNullValidation(site, "Site", rowNumber))
            {
                retVal = true;
            }
            else if (!(siteName.ContainsKey(siteId)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Site <" + site + "> is not found in master records");
            }
            return retVal;
        }
        public bool bdTypeValidation(string bdtype, long rowNumber)
        {
            bool retVal = false;
            if (!(breakdownType.ContainsKey(bdtype)))
            {
                retVal = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid breakdown type - <" + bdtype + "> is not found in master records");
            }
            return retVal;
        }
        public bool dateNullValidation(string value, string columnName, long rowNo)
        {
            //ErrorLog("Inside dateNullValidation");
            //ErrorLog("Date  Value :" + value + "T\r\n");
            bool retVal = false;
            try
            {
                //ErrorLog("Inside try block");
                // string dateValue = Convert.ToDateTime(value, timeCulture).ToString("dd-MM-yyyy");
                //value = Convert.ToDateTime(value, timeCulture).ToString("dd-MM-yyyy");
                string dateValue = Convert.ToDateTime(value).ToString("dd-MM-yyyy");
                //ErrorLog("Inside try block 1");
                value = Convert.ToDateTime(value).ToString("dd-MM-yyyy");
                //ErrorLog("Inside try block 2");
                //ErrorLog("Date  Value :" + dateValue + "Value :" + value + "T\r\n");
                //ErrorLog("Date  Tome Min :"  +DateTime.MinValue.ToString("dd-MM-yyyy"));
                if (value == "Nil")
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Cell value cannot be empty 1,");
                    retVal = true;
                }
                else if (value != dateValue)
                {
                    //m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format: MM-dd-yyyy,");
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format: yyyy-mm-dd,");
                    retVal = true;
                }
                else if (value == DateTime.MinValue.ToString("dd-MM-yyyy"))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: Incorrect date format <" + value + ">. While feeding data use following format:  yyyy-mm-dd,");
                    retVal = true;
                }
            }
            catch (Exception e)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Incorrect date conversion <" + value + ">. While feeding data use following format:  yyyy-mm-dd catch " + e.Message + ", ");
                retVal = true;
            }
            return retVal;
        }
        /* public bool numericNullValidation(double value, string columnName, long rowNo)
         {
             bool retVal = false;
             if (value == 0)
             {
                 m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be empty or zero,");
                 retVal = true;
             }
             return retVal;
         }*/
        public bool numericNullValidation(double value, string columnName, long rowNo)
        {
            bool retVal = false;
            if (value == 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be null or zero,");
                retVal = true;
            }
            if (value < 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be negative,");
                retVal = true;
            }
            return retVal;

        }

        public bool stringNullValidation(string value, string columnName, long rowNo)
        {
            bool retVal = false;
            if (string.IsNullOrEmpty(value) || value == "Nil")
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + ">: value   <" + value + "> cannot be empty,");
                retVal = true;
            }
            return retVal;
        }
        public bool financialYearValidation(string year, long rowNo)
        {
            bool retVal = false;
            try
            {
                if (year == "Nil" || string.IsNullOrEmpty(year))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Please add financial year in foll format:'2022-23',");
                    retVal = true;
                }
                int yearLimit1 = Convert.ToInt32(year.Substring(0, 4));
                int yearLimit2 = Convert.ToInt32("20" + year.Substring(year.IndexOf("-") + 1));
                if (!((yearLimit1 < yearLimit2) && (yearLimit1 <= 2040 && yearLimit1 >= 2020) && (yearLimit2 <= 2040 && yearLimit2 >= 2020)))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Invalid financial year <" + year + "> range must be between year 2020 and 2040,");
                    retVal = true;
                }
                if (!(yearLimit2 - yearLimit1 == 1))
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> Invalid financial year <" + year + ">,");
                    retVal = true;

                }
            }
            catch (Exception)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> Please add financial year in foll format:'2022-23',");
                retVal = true;
            }
            return retVal;
        }
        public bool timeValidation(string timeValue, string columnName, long rowNo)
        {
            bool retVal = false;
            try
            {
                //conversion error should throw format error
                // string standardTime = Convert.ToDateTime(timeValue, timeCulture).ToString("HH:mm:ss");
                string standardTime = Convert.ToDateTime(timeValue).ToString("HH:mm:ss");
                //if cell empty then throw empty error
                ErrorLog("Standatd  Time :" + standardTime + "TimeValue :" + timeValue + "T\r\n");
                if (timeValue == "Nil")
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + "> Cell value empty. Add valid time value in HH:mm:ss 24 hour format,");
                    retVal = true;
                }
                else if (timeValue != standardTime)
                {
                    m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + "> Add valid time value in HH:mm:ss 24 hour format,");
                    retVal = true;
                }
            }
            catch (Exception)
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> column <" + columnName + "> Invalid Time Format. Add valid time value in HH:mm:ss format,");
                retVal = true;
            }
            return retVal;
        }
        public bool monthValidation(string month, int monthNo, long rowNo)
        {
            bool retVal = false;
            if (!(monthNo >= 1 && monthNo <= 12))
            {
                retVal = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> Incorrect Month<" + month + "> Add a valid full name of a month,");
            }
            return retVal;
        }
        public bool yearValidation(int year, long rowNo)
        {
            bool retValue = false;
            if (!(year >= 2020 && year <= 2040))
            {
                m_ErrorLog.SetError(",File row<" + rowNo + "> Incorrect Year<" + year + "> Add a valid year between 2020 and 2040,");
                retValue = true;
            }
            return retValue;
        }
        public bool solarInverterValidation(string inverterValue, string columnName, long rowNo)
        {
            bool retValue = false;
            if (string.IsNullOrEmpty(inverterValue) || !(inverterList.Contains(inverterValue)))
            {
                retValue = true;
                m_ErrorLog.SetError(",File row<" + rowNo + "> column<" + columnName + ">: Invalid Inverter value <" + inverterValue + "> not found in master records,");
            }
            return retValue;
        }
        public bool countryValidation(string countryValue, string columnName, long rowNo)
        {
            bool retVal = false;
            if (!(countryValue == "India") || countryValue == "Nil")
            {
                m_ErrorLog.SetError(",File Row<" + rowNo + "> column <" + columnName + ">: Invalid Country name<" + countryValue + ">,");
                retVal = true;
            }
            return retVal;
        }
		
        bool validateNumeric(string val, string columnName, long rowNo, bool dbNullError, int logErrorFlag, out double importValue)
        {
            bool retValue = false;
            importValue = 0;

//            if (val is DBNull)
            if (dbNullError)
            {
                //dont log error but set value to 0
                //retValue = true;
                //if (logErrorFlag == 0)
                //{
                //    m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be DBNull,");
                //}
                //else if (logErrorFlag == 1)
                //{
                //    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be DBNull,");
                //}
            }
            if (string.IsNullOrEmpty(val))
            {
                //dont log error but set value to 0
                //retValue = true;
                //if (logErrorFlag == 0)
                //{
                //    m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be null or empty,");
                //}
                //else if (logErrorFlag == 1)
                //{
                //    m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value cannot be null or empty,");

                //}
            }
            else
            {
                if (!Regex.IsMatch(val, @"^-?([0-9]*|\d*\.\d{1}?\d*)$"))
                {
                    //log error for non numeric values
                    retValue = true;
                    if (logErrorFlag == 0)
                    {
                        m_ErrorLog.SetWarning(",Row <" + rowNo + "> column <" + columnName + "> : value  <" + val + "> cannot be non numeric,");
                    }
                    else if (logErrorFlag == 1)
                    {
                        m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value  <" + val + "> cannot be non numeric,");
                    }
                }
                else
                {
                    //all good, convert value to double
                    importValue = Convert.ToDouble(val);
                }
            }
            return retValue;
        }

        bool validateNumeric(string val, out double importValue)
        {
            bool retValue = false;
            importValue = 0;

            if (val is DBNull)
            {
                retValue = true;
            }
            if (string.IsNullOrEmpty(val))
            {
                retValue = true;
            }
            else
            {
                if (!Regex.IsMatch(val, @"^([0-9]*|\d*\.\d{1}?\d*)$"))
                {
                    retValue = true;
                }
                else
                {
                    importValue = Convert.ToDouble(val);
                }
            }
            return retValue;
        }

        public bool kwhValidation(double kwhValue, double prodHrsValue, string columnName, long rowNo, double kwhMax)
        {
            bool retVal = false;
            if (kwhValue == 0 && prodHrsValue != 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be zero, because production hours  <" + prodHrsValue + "> is not zero,");
                retVal = true;
            }
            string value = Convert.ToString(kwhValue);
            if (kwhValue < 0 )
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be negative or null or 0,");
                retVal = true;
            }

            //get max kWh from location master and validate.
            if (kwhValue > kwhMax)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : kWh value cannot be more than <" + kwhMax + "> as set in location master,");
                retVal = true;
            }

            return retVal;
        }
        public bool negativeNullValidation(double value, string columnName, long rowNo)
        {
            bool retVal = false;
            string checker = Convert.ToString(value);
            if (value < 0)
            {
                m_ErrorLog.SetError(",Row <" + rowNo + "> column <" + columnName + "> : value <" + value + "> cannot be null or negative,");
                retVal = true;
            }
            return retVal;

        }

        public string Filter(long rowNumber, string colName, string str, List<char> charsToRemove)
        {
            foreach (char c in charsToRemove)
            {
                str = str.Replace(c.ToString(), string.Empty);
//                m_ErrorLog.SetInformation(",Row <" + rowNumber + "> column <" + colName + "> : value <" + c.ToString() + "> removed from string,");
            }

            return str;
        }
        public string validateAndCleanSpChar(long rowNumber, string colName, string sActionTaken)
        {
            string retValue = "";
            List<char> charsToRemove = new List<char>() { '\'', '@' };
            retValue = Filter(rowNumber, colName, sActionTaken, charsToRemove);
            return retValue;
        }



    }
}