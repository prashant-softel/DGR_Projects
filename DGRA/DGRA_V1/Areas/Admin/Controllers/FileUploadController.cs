﻿
using DGRA_V1.Models;
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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using DGRA_V1.Common;
using System.Net;
using System.Collections;
using DGRA_V1.Repository.Interface;
using OfficeOpenXml;
using System.ComponentModel;
using System.Globalization;

namespace DGRA_V1.Areas.admin.Controllers
{
    [Area("admin")]
    public class FileUploadController : Controller
    {
        ImportBatch objImportBatch = new ImportBatch();
        private IDapperRepository _idapperRepo;
        private IWebHostEnvironment env;
        private static IHttpContextAccessor HttpContextAccessor;
        CultureInfo timeCulture = new CultureInfo("en-IN");
        public FileUploadController(IDapperRepository idapperobj, IWebHostEnvironment obj, IHttpContextAccessor httpObj)
        {
            HttpContextAccessor = httpObj;
            _idapperRepo = idapperobj;
            m_ErrorLog = new ErrorLog(obj);
            env = obj;
        }
        static int batchIdDGRAutomation = 0;
        string siteUserRole;
        int previousSite = 0;
        static string[] importData = new string[2];
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
        List<string> fileSheets = new List<string>();

        //WindUploadingFileValidation m_ValidationObject;
        ErrorLog m_ErrorLog;
        // static string[] importData = new string[2];

        Hashtable equipmentId = new Hashtable();
        Hashtable breakdownType = new Hashtable();//(B)Gets bdTypeID from bdTypeName: BDType table
        Hashtable siteNameId = new Hashtable(); //(C)Gets siteId from siteName
        Hashtable siteName = new Hashtable(); //(D)Gets siteName from siteId

        Hashtable eqSiteId = new Hashtable();//(E)Gets siteId from (wtg)equipmentName
        Hashtable MonthList = new Hashtable() { { "Jan", 1 }, { "Feb", 2 }, { "Mar", 3 }, { "Apr", 4 }, { "May", 5 }, { "Jun", 6 }, { "Jul", 7 }, { "Aug", 8 }, { "Sep", 9 }, { "Oct", 10 }, { "Nov", 11 }, { "Dec", 12 } };
        Hashtable longMonthList = new Hashtable() { { "January", 1 }, { "February", 2 }, { "March", 3 }, { "April", 4 }, { "May", 5 }, { "June", 6 }, { "July", 7 }, { "August", 8 }, { "September", 9 }, { "October", 10 }, { "November", 11 }, { "December", 12 } };

        // private DataSet dataSetMain;

        /*~FileUploadController()
        {
            //Destructor
            //delete m_ValidationObject;
            //delete m_ErrorLog;
        }*/

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


                            // file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xlsx"));
                            // string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(@"~" + @"\TempFile\docupload.xlsx") + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            //////////string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" +@"\TempFile\docupload.xlsx" + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            //////////List<string> fileSheets = new List<string>();
                            //////////oconn = new System.Data.OleDb.OleDbConnection(connectionString);
                            //////////oconn.Open();
                            //////////DataTable dt = null;
                            //////////dt = oconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            //////////oconn.Close();
                            DataTable dt = null;

                            string _filePath = @"C:\TempFile\docupload.xlsx";
                           // string _filePath = @"G:\TempFile\docupload.xlsx";
                            //string _filePath = env.ContentRootPath + @"\TempFile\docupload.xlsx";
                            dataSetMain = GetDataTableFromExcel(_filePath, true, ref fileSheets);
                            if (dataSetMain == null)
                            {
                                m_ErrorLog.SetError(",Unable to extract excel sheet data for importing,");
                            }
                            ErrorLog("datSet Med"+ dataSetMain);
                            if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min"))
                            {
                                masterHashtable_SiteName_To_SiteId();//C
                                masterHashtable_BDNameToBDId();//B
                                masterHashtable_SiteIdToSiteName();
                                if (fileUploadType == "Wind")
                                {
                                    masterHashtable_WtgToWtgId();
                                    masterHashtable_WtgToSiteId();
                                }
                            }
                            else
                            {
                                masterHashtable_SiteName_To_SiteId();
                                masterHashtable_SiteIdToSiteName();
                            }
                            //Status Codes:
                            //200 = Success ; 400 = Failure(BadRequest)

                            foreach (var excelSheet in fileSheets)
                            {
                                DataSet ds = new DataSet();
                                string sql = "";
                                if (excelSheet == FileSheetType.Uploading_File_Generation)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Generation + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
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
                                    //sql = "SELECT * FROM [" + FileSheetType.Uploading_File_Breakdown + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Uploading_File_Breakdown WorkSheet:");
                                            statusCode = await InsertSolarFileBreakDown(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar Breakdown Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Breakdown Import API Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Breakdown Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Uploading_File_Breakdown WorkSheet:");
                                            statusCode = await InsertWindFileBreakDown(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Wind BreakDown Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetInformation(",End of Wind BreakDown Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter1Min)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Uploading_PyranoMeter1Min + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter1Min: ");
                                            statusCode = await InsertSolarPyranoMeter1Min(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar PyranoMeter1Min Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar PyranoMeter1Min Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar PyranoMeter1Min Import:");
                                            }
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Uploading_PyranoMeter15Min)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Uploading_PyranoMeter15Min + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_PyranoMeter15Min :");
                                            statusCode = await InsertSolarPyranoMeter15Min(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar PyranoMeter15Min Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar PyranoMeter15Min Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar PyranoMeter1Min Import:");
                                            }
                                        }
                                        else
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_JMR_Input_and_Output)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Monthly_JMR_Input_and_Output + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertWindMonthlyJMR(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Monthly JMR Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Monthly JMR Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Monthly JMR Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_JMR_Input_and_Output WorkSheet:");
                                            statusCode = await InsertSolarMonthlyJMR(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar Monthly JMR Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Monthly JMR Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Monthly JMR Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_LineLoss)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Monthly_LineLoss + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertWindMonthlyLineLoss(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Monthly LineLoss Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Monthly LineLoss Import API Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Wind Monthly LineLoss Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_LineLoss WorkSheet:");
                                            statusCode = await InsertSolarMonthlyLineLoss(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Monthly_LineLoss Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Monthly_LineLoss Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Monthly_LineLoss Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Monthly_Target_KPI)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Monthly_Target_KPI + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindMonthlyTargetKPI(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Monthly_Target_KPI Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Monthly_Target_KPI Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Monthly_Target_KPI Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarMonthlyTargetKPI(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Monthly_Target_KPI Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Monthly_Target_KPI Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Monthly_Target_KPI Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Load_Shedding)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Daily_Load_Shedding + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertWindDailyLoadShedding(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Daily_Load_Shedding Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",API Wind Daily_Load_Shedding Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Daily_Load_Shedding Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Daily_Load_Shedding WorkSheet:");
                                            statusCode = await InsertSolarDailyLoadShedding(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Daily_Load_Shedding Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",API Solar Daily_Load_Shedding Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Daily_Load_Shedding Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_JMR_Input_and_Output)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Daily_JMR_Input_and_Output + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_JMR_Input_and_Output WorkSheet:");
                                            //statusCode = await InsertWindDailyJMR(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Daily_JMR_Input_and_Output Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Daily_JMR_Input_and_Output Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Daily_JMR_Input_and_Output Import:");
                                            }
                                        }
                                        else
                                        {
                                            //As per instructions : Daily JMR function is not supposed to exist but is allowed for wind imports

                                            //status = await InsertSolarDailyJMR(status, ds);
                                            //if (status == "Successfully uploaded")
                                            //{
                                            //    m_ErrorLog.SetInformation("No Errors, Solar Daily_JMR_Input_and_Output Import Successful:");
                                            //}
                                            //else
                                            //{
                                            //    m_ErrorLog.SetError(",Solar Daily_JMR_Input_and_Output Import Failed:");
                                            //    m_ErrorLog.SetInformation(",End of Solar Daily_JMR_Input_and_Output Import:");
                                            //}
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_Target_KPI)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Daily_Target_KPI + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertWindDailyTargetKPI(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Daily_Target_KPI Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Daily_Target_KPI Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Daily_Target_KPI Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Daily_Target_KPI WorkSheet:");
                                            statusCode = await InsertSolarDailyTargetKPI(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Daily_Target_KPI Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Daily_Target_KPI Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Daily_Target_KPI Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Site_Master)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Site_Master + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Site_Master WorkSheet:");
                                            statusCode = await InsertWindSiteMaster(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Site_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Site_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Site_Master Import:");
                                            }
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Site_Master WorkSheet:");
                                            statusCode = await InsertSolarSiteMaster(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Site_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Site_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Site_Master Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Location_Master)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Location_Master + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Location_Master WorkSheet:");
                                            statusCode = await InsertWindLocationMaster(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Location_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Location_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Location_Master Import:");
                                            }
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Location_Master WorkSheet:");
                                            statusCode = await InsertSolarLocationMaster(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Location_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Location_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Location_Master Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Solar_AC_DC_Capacity)
                                {
                                    //sql = "SELECT * FROM [" + FileSheetType.Solar_AC_DC_Capacity + "]";
                                    //OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    //OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    //da.Fill(ds);
                                    //oconn.Close();
                                    ds.Tables.Add(dataSetMain.Tables[excelSheet].Copy());
                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUploadType == "Wind")
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                        else if (fileUploadType == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar AC_DC_Capacity WorkSheet:");
                                            statusCode = await InsertSolarAcDcCapacity(status, ds);
                                            if (statusCode == 200)
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar AC_DC_Capacity Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar AC_DC_Capacity  Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar AC_DC_Capacity Import:");
                                            }
                                        }
                                    }
                                }
                            } // end of foreach (var excelSheet in fileSheets)
                            if (statusCode == 200)
                            {
                                m_ErrorLog.SetInformation(",Import Operation Complete :");
                                // await UploadFileToImportedFileFolder(file);
                                if (!(fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown") || fileSheets.Contains("Uploading_PyranoMeter1Min") || fileSheets.Contains("Uploading_PyranoMeter15Min")))
                                {
                                    await importMetaData(fileUploadType, file.FileName);
                                }

                                //DGR Automation Function Logic
                                if (fileUploadType == "Wind")
                                {
                                    if (fileSheets.Contains("Uploading_File_Generation") || fileSheets.Contains("Uploading_File_Breakdown"))
                                    {
                                        if (isGenValidationSuccess || isBreakdownValidationSuccess)
                                        {
                                            await importMetaData(fileUploadType, file.FileName);
                                            statusCode = await dgrWindImport(batchIdDGRAutomation);
                                            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailyWindKPI?fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                            //remove after testing
                                            m_ErrorLog.SetInformation("Url" + url);
                                            using (var client = new HttpClient())
                                            {
                                                var response = await client.GetAsync(url);
                                                //status = "Respose" + response;
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    m_ErrorLog.SetInformation(",Wind KPI Calculations Updated Successfully:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Successfully Uploaded";
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(",Wind KPI Calculations API Failed:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Calculation Import Faild";
                                                }
                                            }

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
                                        ErrorLog("isGenValidationSuccess"+ isGenValidationSuccess);
                                        ErrorLog("isPyro15ValidationSuccess"+ isPyro15ValidationSuccess);
                                        ErrorLog("isPyro1ValidationSuccess" + isPyro1ValidationSuccess);
                                        ErrorLog("Before Validation");
                                        //pending : instead check the  success flags
                                        if (isGenValidationSuccess && isGenValidationSuccess && isPyro15ValidationSuccess && isPyro1ValidationSuccess)
                                        {
                                            ErrorLog("Before Metadata");
                                            await importMetaData(fileUploadType, file.FileName);
                                            ErrorLog("After Metadata");
                                            statusCode = await dgrSolarImport(batchIdDGRAutomation);
                                            ErrorLog("Status COde :"+ statusCode);
                                            //var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?fromDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "";
                                            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailySolarKPI?site="+(string)kpiArgs[2] +"&fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "";
                                            using (var client = new HttpClient())
                                            {
                                                var response = await client.GetAsync(url);
                                                if (response.IsSuccessStatusCode)
                                                {
                                                    m_ErrorLog.SetInformation(",SolarKPI Calculations Updated Successfully:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Successfully Uploaded";
                                                }
                                                else
                                                {
                                                    m_ErrorLog.SetError(",SolarKPI  Calculations API Failed:");
                                                    statusCode = (int)response.StatusCode;
                                                    status = "Failed to Upload";
                                                }
                                            }
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
                            status = "Something went wrong : Exception Caught Debugging Required";
                            ex.GetType();
                            m_ErrorLog.SetError("," + status + ":");
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
            if (statusCode != 200)
            {
                ArrayList messageList = m_ErrorLog.errorLog();
                foreach (var item in messageList)
                {
                    status += ((string)item).Replace(",", "") + ",";
                }
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
                    _worksheetList.Add(worksheet.Name);
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
                m_ErrorLog.SetError("," + status);
                throw new Exception(ex.Message);
            }
        }

        //Remove static
        //Beginning of all DGR Import functions for both Wind and Solar Upload types

        private async Task<int> InsertSolarFileGeneration(string status, DataSet ds)
        {

            List<bool> errorFlag = new List<bool>();
            int rowNumber = 0;
            int errorCount = 0;
            int responseCode = 400;
            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;
            try
            {
                //kpi helper variables(to be worked on:-)
                //DateTime.TryParseExact(ds.Tables[0].Rows[0]["Date"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt1);
               // DateTime fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
               // DateTime toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"], timeCulture);
                DateTime nextDate = DateTime.MinValue;
                string site = "";
                //---------------------
                if (ds.Tables.Count > 0)
                {
                    SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                    List<SolarUploadingFileGeneration> addSet = new List<SolarUploadingFileGeneration>();
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        rowNumber++;
                        SolarUploadingFileGeneration addUnit = new SolarUploadingFileGeneration();
                        //addUnit.date = Convert.ToDateTime(dr["Date"], timeCulture).ToString("yyyy-MM-dd");
                        addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                        //Date Validation  Added
                        errorFlag.Add(addUnit.date == DateTime.MinValue.ToString("dd-MM-yyyy") ? true : false);
                        addUnit.site = Convert.ToString(dr["Site"]);
                        addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                        //Site Validation Added
                        errorFlag.Add(siteValidation(addUnit.site, addUnit.site_id, rowNumber));
                        objImportBatch.importSiteId = addUnit.site_id;//C
                        //nextDate = Convert.ToDateTime(dr["Date"], timeCulture);
                        nextDate = Convert.ToDateTime(dr["Date"]);
                        //fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                        //toDate = (nextDate > toDate) ? (nextDate) : (toDate);
                        if (nextDate < minDate)
                        {
                            minDate = nextDate;
                        }
                        if (nextDate > maxDate)
                        {
                            maxDate = nextDate;
                        }
                        site = Convert.ToString(addUnit.site_id);
                        addUnit.inverter = Convert.ToString(dr["Inverter"]);
                        addUnit.inv_act = Convert.ToDouble(dr["Inv_Act(KWh)"]);
                        addUnit.plant_act = Convert.ToDouble(dr["Plant_Act(kWh)"]);
                        addUnit.pi = stringToPercentage(rowNumber, Convert.ToString(dr["PI(%)"]), "PI(%)");
                        errorFlag.Add(validationObject.validateGenerationData(rowNumber, addUnit.inverter, addUnit.inv_act, addUnit.plant_act));
                        foreach (bool item in errorFlag)
                        {
                            if (item)
                            {
                                errorCount++;
                                break;
                            }
                        }
                        addSet.Add(addUnit);
                    }
                    if (!(errorCount > 0))
                    {
                        //set the  validationgeneration sucess flag
                        isGenValidationSuccess = true;
                        responseCode = 200;
                        //kpiArgs.Add(fromDate);
                       // kpiArgs.Add(toDate);
                        kpiArgs.Add(minDate);
                        kpiArgs.Add(maxDate);
                        kpiArgs.Add(site);
                        genJson = JsonConvert.SerializeObject(addSet);
                    }
                    else
                    {
                        // add to error log that validation of generation failed
                        status = "Solar Generation Validation Failed";
                        m_ErrorLog.SetError("," + status + ":");
                       
                    }
                }
            }
            catch (Exception e)
            {
                e.GetType();
                ErrorLog("Inside" + e.Message);
            }
            return responseCode;
        }

        private async Task<int> InsertWindFileGeneration(string status, DataSet ds)
        {
            //siteID recorded
            //siteID validated

            //validation variables
            DateTime dateValidate = DateTime.MinValue;
            long rowNumber = 0;
            int errorCount = 0;
            bool errorFlag = false;
            //DateTime dt1;
            //DateTime dt2;
            //kpi helper variables
            //string firstDate = (ds.Tables[0].Rows[0]["Date"].ToString()); 
            //DateTime.TryParseExact(ds.Tables[0].Rows[0]["Date"].ToString(), "dd-MM-yyyy", CultureInfo.InvariantCulture,DateTimeStyles.None,out dt1);

            DateTime minDate = DateTime.MaxValue;
            DateTime maxDate = DateTime.MinValue;

            //DateTime fromDate = dt1;// Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
            //DateTime toDate = dt1;// Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);



            DateTime nextDate= DateTime.MinValue;
            string site = "";
            //function return variable
            int responseCode = 400;

            WindUploadingFileValidation validationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);

            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileGeneration> addSet = new List<WindUploadingFileGeneration>();

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rowNumber++;
                    WindUploadingFileGeneration addUnit = new WindUploadingFileGeneration();
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    dateValidate = Convert.ToDateTime(addUnit.date);
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                    addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);
                    addUnit.site_name = (string)(siteName[addUnit.site_id]);//D
                    objImportBatch.importSiteId = addUnit.site_id;
                    nextDate = Convert.ToDateTime(dr["Date"]);
                   //fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                    //toDate = (nextDate > toDate) ? (nextDate) : (toDate);
                    if(nextDate< minDate)
                    {
                        minDate = nextDate;
                    }
                    if(nextDate > maxDate)
                    {
                        maxDate = nextDate;
                    }
                    site = Convert.ToString(addUnit.site_id);
                    addUnit.wind_speed = Convert.ToDouble(dr["Wind_Speed"]);
                    addUnit.kwh = Convert.ToDouble(dr["kWh"]);
                    addUnit.operating_hrs = Convert.ToDouble(dr["Gen_Hrs"]);
                    addUnit.lull_hrs = Convert.ToDouble(dr["Lull_Hrs"]);
                    addUnit.grid_hrs = Convert.ToDouble(dr["Grid_Hrs"]);
                    errorFlag = uniformWindSiteValidation(rowNumber, addUnit.site_id, addUnit.wtg);
                    errorFlag = (siteUserRole == "Admin") ? false : importDateValidation(dateValidate);
                    errorFlag = validationObject.validateGenData(rowNumber, addUnit.date, addUnit.wtg, addUnit.wind_speed, addUnit.kwh, addUnit.operating_hrs, addUnit.lull_hrs, addUnit.grid_hrs);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {

                    isGenValidationSuccess = true;
                    responseCode = 200;
                    genJson = JsonConvert.SerializeObject(addSet);
                    //kpiArgs.Add(fromDate);
                    kpiArgs.Add(minDate);
                    //kpiArgs.Add(toDate);
                    kpiArgs.Add(maxDate);
                    kpiArgs.Add(site);
                    m_ErrorLog.SetInformation(",Wind Generation File Validation Successful");
                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "";
                    m_ErrorLog.SetError(",Wind Generation File Validation Failed");
                }
            }
            return responseCode;
        }

        private async Task<int> InsertSolarFileBreakDown(string status, DataSet ds)
        {
            ErrorLog("Inside Brekdown <br>\r\n");
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation(m_ErrorLog, _idapperRepo);
                List<SolarUploadingFileBreakDown> addSet = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFileBreakDown addUnit = new SolarUploadingFileBreakDown();
                    rowNumber++;
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.site = Convert.ToString(dr["Site"]);
                    if (siteNameId.ContainsKey(addUnit.site))
                    { addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]); }//C
                    else { errorFlag = true; }
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.ext_int_bd = Convert.ToString(dr["Ext_BD"]);
                    addUnit.igbd = Convert.ToString(dr["IGBD"]);
                    addUnit.icr = Convert.ToString(dr["ICR"]);
                    addUnit.inv = Convert.ToString(dr["INV"]);
                    addUnit.smb = Convert.ToString(dr["SMB"]);
                    addUnit.strings = Convert.ToString(dr["Strings"]);
                    addUnit.from_bd = Convert.ToDateTime(dr["From"]);
                    addUnit.to_bd = Convert.ToDateTime(dr["To"]);
                    addUnit.total_bd = validationObject.breakDownCalc(addUnit.from_bd, addUnit.to_bd);
                    addUnit.bd_remarks = Convert.ToString(dr["BDRemarks"]);
                    addUnit.bd_type = Convert.ToString(dr["BDType"]);
                    addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                    addUnit.action_taken = Convert.ToString(dr["ActionTaken"]);
                    errorFlag = validationObject.validateBreakDownData(rowNumber, addUnit.from_bd, addUnit.to_bd, addUnit.igbd);
                    addUnit.from_bd = Convert.ToDateTime(addUnit.from_bd).ToString("HH:mm:ss");
                    addUnit.to_bd = Convert.ToDateTime(addUnit.to_bd).ToString("HH:mm:ss");
                    if (errorFlag)
                    {
                        //ErrorLog("Inside Brekdown <br>\r\n"+ rowNumber);
                        errorCount++;
                        errorFlag = false;
                        continue;

                    }
                    ErrorLog("Inside Brekdown <br>\r\n");
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "Solar Breakdown Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindFileBreakDown(string status, DataSet ds)
        {
            WindUploadingFileValidation ValidationObject = new WindUploadingFileValidation(m_ErrorLog, _idapperRepo);
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileBreakDown> addSet = new List<WindUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindUploadingFileBreakDown addUnit = new WindUploadingFileBreakDown();
                    rowNumber++;
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                    addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);//E
                    addUnit.site_name = (string)siteName[addUnit.site_id];//D
                    objImportBatch.importSiteId = addUnit.site_id;
                    addUnit.bd_type = Convert.ToString(dr["BD_Type"]);
                    addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                    errorFlag = await bdTypeValidation(addUnit.bd_type, rowNumber);
                    addUnit.stop_from = Convert.ToString(dr["Stop From"]);
                    addUnit.stop_to = Convert.ToString(dr["Stop To"]);
                    addUnit.total_stop = ValidationObject.breakDownCalc(addUnit.stop_from, addUnit.stop_to);
                    addUnit.error_description = Convert.ToString(dr["Error description"]);
                    addUnit.action_taken = Convert.ToString(dr["Action Taken"]);
                    errorFlag = ValidationObject.validateBreakDownData(rowNumber, addUnit.bd_type, addUnit.wtg, addUnit.stop_from, addUnit.stop_to);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("hh:mm:ss");
                    addUnit.stop_to = Convert.ToDateTime(dr["Stop To"]).ToString("hh:mm:ss");
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    isBreakdownValidationSuccess = true;
                    responseCode = 200;
                    breakJson = JsonConvert.SerializeObject(addSet);
                    m_ErrorLog.SetError(",Wind Breakdown File Validation Successful");

                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "Wind Breakdown Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                    m_ErrorLog.SetError(",Wind Breakdown File Validation Failed");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarPyranoMeter1Min(string status, DataSet ds)
        {
            ErrorLog("Inside 1 MIn <br>\r\n");
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //importing successfully?
            //validating successfully?
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
           
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter1Min> addSet = new List<SolarUploadingPyranoMeter1Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                   // DateTime dt1;
                    rowNumber++;
                    SolarUploadingPyranoMeter1Min addUnit = new SolarUploadingPyranoMeter1Min();
                    addUnit.date_time = Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                 
                   // ErrorLog("Inside 1 MIn <br>\r\n"+ addUnit.date_time);
                    string site = Convert.ToString(dr["Site"]);
                    if (siteNameId.ContainsKey(site))
                    { addUnit.site_id = Convert.ToInt32(siteNameId[site]); }//C
                    else { errorFlag = true; }
                    objImportBatch.importSiteId = addUnit.site_id;
                    addUnit.ghi_1 = Convert.ToDouble((dr["GHI-1"] is DBNull) || string.IsNullOrEmpty((string)dr["GHI-1"]) ? 0 : dr["GHI-1"]);
                    addUnit.ghi_2 = Convert.ToDouble((dr["GHI-2"] is DBNull) || string.IsNullOrEmpty((string)dr["GHI-2"]) ? 0 : dr["GHI-2"]);
                    addUnit.poa_1 = Convert.ToDouble((dr["POA-1"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-1"]) ? 0 : dr["POA-1"]);
                    addUnit.poa_2 = Convert.ToDouble((dr["POA-2"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-2"]) ? 0 : dr["POA-2"]);
                    addUnit.poa_3 = Convert.ToDouble((dr["POA-3"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-3"]) ? 0 : dr["POA-3"]);
                    addUnit.poa_4 = Convert.ToDouble((dr["POA-4"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-4"]) ? 0 : dr["POA-4"]);
                    addUnit.poa_5 = Convert.ToDouble((dr["POA-5"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-5"]) ? 0 : dr["POA-5"]);
                    addUnit.poa_6 = Convert.ToDouble((dr["POA-6"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-6"]) ? 0 : dr["POA-6"]);
                    addUnit.poa_7 = Convert.ToDouble((dr["POA-7"] is DBNull) || string.IsNullOrEmpty((string)dr["POA-7"]) ? 0 : dr["POA-7"]);
                    addUnit.avg_ghi = Convert.ToDouble((dr["Average GHI (w/m²)"] is DBNull) || string.IsNullOrEmpty((string)dr["Average GHI (w/m²)"]) ? 0 : dr["Average GHI (w/m²)"]);
                    addUnit.avg_poa = Convert.ToDouble((dr["Average POA (w/m²)"] is DBNull) || string.IsNullOrEmpty((string)dr["Average POA (w/m²)"]) ? 0 : dr["Average POA (w/m²)"]);
                    addUnit.amb_temp = Convert.ToDouble((dr["Ambient Temp"] is DBNull) || string.IsNullOrEmpty((string)dr["Ambient Temp"]) ? 0 : dr["Ambient Temp"]);
                    addUnit.mod_temp = Convert.ToDouble((dr["Module Temp"] is DBNull) || string.IsNullOrEmpty((string)dr["Module Temp"]) ? 0 : dr["Module Temp"]);
                    if (errorFlag)
                    {
                        //ErrorLog("Inside 1 MIn <br>\r\n" + rowNumber);
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                   // ErrorLog("Inside 1 MIn <br>\r\n" );
                    addSet.Add(addUnit);
                }
                //pending check error
                //set IsPyro1 flag
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    isPyro1ValidationSuccess = true;
                    responseCode = 200;
                    pyro1Json = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "Solar PyranoMeter1Min Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }

        private async Task<int> InsertSolarPyranoMeter15Min(string status, DataSet ds)
        {
            ErrorLog("Inside 15 MIn <br>\r\n");
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter15Min> addSet = new List<SolarUploadingPyranoMeter15Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                   //DateTime dt1;
                    rowNumber++;
                    SolarUploadingPyranoMeter15Min addUnit = new SolarUploadingPyranoMeter15Min();
                    addUnit.date_time = Convert.ToDateTime(dr["Time stamp"]).ToString("yyyy-MM-dd HH:mm:ss");
                    
                   // ErrorLog("Inside 15 MIn <br>\r\n" + addUnit.date_time);
                    string site = Convert.ToString(dr["Site"]);
                    if (siteNameId.ContainsKey(site))
                    { addUnit.site_id = Convert.ToInt32(siteNameId[site]); }//C
                    else { errorFlag = true; }
                    objImportBatch.importSiteId = addUnit.site_id;
                    addUnit.ghi_1 = Convert.ToDouble((dr["GHI-1"] is DBNull || string.IsNullOrEmpty((string)dr["GHI-1"])) ? 0 : dr["GHI-1"]);
                    addUnit.ghi_2 = Convert.ToDouble((dr["GHI-2"] is DBNull || string.IsNullOrEmpty((string)dr["GHI-2"])) ? 0 : dr["GHI-2"]);
                    addUnit.poa_1 = Convert.ToDouble((dr["POA-1"] is DBNull || string.IsNullOrEmpty((string)dr["POA-1"])) ? 0 : dr["POA-1"]);
                    addUnit.poa_2 = Convert.ToDouble((dr["POA-2"] is DBNull || string.IsNullOrEmpty((string)dr["POA-2"])) ? 0 : dr["POA-2"]);
                    addUnit.poa_3 = Convert.ToDouble((dr["POA-3"] is DBNull || string.IsNullOrEmpty((string)dr["POA-3"])) ? 0 : dr["POA-3"]);
                    addUnit.poa_4 = Convert.ToDouble((dr["POA-4"] is DBNull || string.IsNullOrEmpty((string)dr["POA-4"])) ? 0 : dr["POA-4"]);
                    addUnit.poa_5 = Convert.ToDouble((dr["POA-5"] is DBNull || string.IsNullOrEmpty((string)dr["POA-5"])) ? 0 : dr["POA-5"]);
                    addUnit.poa_6 = Convert.ToDouble((dr["POA-6"] is DBNull || string.IsNullOrEmpty((string)dr["POA-6"])) ? 0 : dr["POA-6"]);
                    addUnit.poa_7 = Convert.ToDouble((dr["POA-7"] is DBNull || string.IsNullOrEmpty((string)dr["POA-7"])) ? 0 : dr["POA-7"]);
                    addUnit.avg_ghi = Convert.ToDouble((dr["Average GHI (w/m²)"] is DBNull || string.IsNullOrEmpty((string)dr["Average GHI (w/m²)"])) ? 0 : dr["Average GHI (w/m²)"]);
                    addUnit.avg_poa = Convert.ToDouble((dr["Average POA (w/m²)"] is DBNull || string.IsNullOrEmpty((string)dr["Average POA (w/m²)"])) ? 0 : dr["Average POA (w/m²)"]);
                    addUnit.amb_temp = Convert.ToDouble((dr["Ambient Temp"] is DBNull || string.IsNullOrEmpty((string)dr["Ambient Temp"])) ? 0 : dr["Ambient Temp"]);
                    addUnit.mod_temp = Convert.ToDouble((dr["Module Temp"] is DBNull || string.IsNullOrEmpty((string)dr["Module Temp"])) ? 0 : dr["Module Temp"]);
                    if (errorFlag)
                    {
                       // ErrorLog("Inside 15 MIn <br>\r\n" + rowNumber);
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    //ErrorLog("Inside 15 MIn <br>\r\n");
                    addSet.Add(addUnit);
                }
                //pending check error
                //set IsPyro1 flag
                if (!(errorCount > 0))
                {
                    //set the  validationgeneration sucess flag
                    isPyro15ValidationSuccess = true;
                    responseCode = 200;
                    pyro15Json = JsonConvert.SerializeObject(addSet);
                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "Solar PyranoMeter15Min Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyJMR(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            long rowNumber = 0;
            int errorCount = 0;
            int responseCode = 400;
            bool errorFlag = false;
            if (ds.Tables.Count > 0)
            {
                List<SolarMonthlyJMR> addSet = new List<SolarMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyJMR addUnit = new SolarMonthlyJMR();
                    rowNumber++;
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                                                                                //objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.Plant_Section = Convert.ToString(dr["Plant Section"]);

                    addUnit.Controller_KWH_INV = Convert.ToDouble((dr["Controller KWH/INV KWH"] is DBNull) ? 0 : dr["Controller KWH/INV KWH"]);
                    addUnit.Scheduled_Units_kWh = Convert.ToDouble(dr["Scheduled Units (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units (kWh)"]) ? 0 : dr["Scheduled Units (kWh)"]);
                    addUnit.Export_kWh = Convert.ToDouble((dr["Export (kWh)"] is DBNull) ? 0 : dr["Export (kWh)"]);
                    addUnit.Import_kWh = Convert.ToDouble((dr["Import (kWh)"] is DBNull) ? 0 : dr["Import (kWh)"]);
                    addUnit.Net_Export_kWh = Convert.ToDouble((dr["Net Export (kWh)"] is DBNull) ? 0 : dr["Net Export (kWh)"]);
                    addUnit.Export_kVAh = Convert.ToDouble((dr["Export (kVAh)"] is DBNull) ? 0 : dr["Export (kVAh)"]);
                    addUnit.Import_kVAh = Convert.ToDouble((dr["Import (kVAh)"] is DBNull) ? 0 : dr["Import (kVAh)"]);
                    addUnit.Export_kVArh_lag = Convert.ToDouble((dr["Export (kVArh lag)"] is DBNull) ? 0 : dr["Export (kVArh lag)"]);
                    addUnit.Import_kVArh_lag = Convert.ToDouble((dr["Import (kVArh lag)"] is DBNull) ? 0 : dr["Import (kVArh lag)"]);
                    addUnit.Export_kVArh_lead = Convert.ToDouble((dr["Export (kVArh lead)"] is DBNull) ? 0 : dr["Export (kVArh lead)"]);
                    addUnit.Import_kVArh_lead = Convert.ToDouble((dr["Import (kVArh lead)"] is DBNull) ? 0 : dr["Import (kVArh lead)"]);
                    addUnit.JMR_date = Convert.ToDateTime((dr["JMR date"] is DBNull) ? "0000-00-00 00:00:00" : dr["JMR date"]).ToString("yyyy-MM-dd");
                    addUnit.JMR_Month = Convert.ToString(dr["JMR Month"]);
                    errorFlag = longMonthList.Contains(addUnit.JMR_Month) ? false : true;
                    addUnit.JMR_Month_no = Convert.ToInt32(longMonthList[addUnit.JMR_Month]);
                    addUnit.JMR_Year = Convert.ToInt32(dr["JMR Year"]);
                    addUnit.LineLoss = Convert.ToDouble((dr["LineLoss"] is DBNull) ? 0 : dr["LineLoss"]);
                    addUnit.Line_Loss_percentage = stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                    errorFlag = (addUnit.Line_Loss_percentage > 100 || addUnit.Line_Loss_percentage < 0) ? true : false;
                    addUnit.RKVH_percentage = stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                    errorFlag = (addUnit.RKVH_percentage > 100 || addUnit.RKVH_percentage < 0) ? true : false;
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;

                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;

                        }
                    }
                }
            }
            return responseCode;
        }
        //End of all DGR Import functions for both Wind and Solar Upload types
        private async Task<int> InsertWindMonthlyJMR(string status, DataSet ds)
        {
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyJMR> addSet = new List<WindMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rowNumber++;
                    WindMonthlyJMR addUnit = new WindMonthlyJMR();
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    //added site_id recording
                    addUnit.siteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //added site_id validation
                    errorFlag = siteValidation(addUnit.site, addUnit.siteId, rowNumber);
                    //recording site_id for import_batches record
                    objImportBatch.importSiteId = addUnit.siteId;
                    addUnit.plantSection = Convert.ToString(dr["Plant Section"]);
                    addUnit.jmrDate = Convert.ToDateTime((dr["JMR date"] is DBNull) ? "0000-00-00" : dr["JMR date"]).ToString("yyyy-MM-dd");
                    addUnit.jmrMonth = Convert.ToString(dr["JMR Month"]);
                    errorFlag = longMonthList.Contains(addUnit.jmrMonth) ? false : true;
                    addUnit.jmrMonth_no = Convert.ToInt32(longMonthList[addUnit.jmrMonth]);
                    addUnit.jmrYear = Convert.ToInt32(dr["JMR Year"]);
                    //string lineLoss = Convert.ToString(dr["Line Loss%"]);
                    //addUnit.lineLossPercent = Convert.ToDouble(lineLoss.TrimEnd('%')) / 100;

                    addUnit.lineLossPercent = stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                    errorFlag = (addUnit.lineLossPercent > 100 || addUnit.lineLossPercent < 0) ? true : false;
                    addUnit.rkvhPercent = stringToPercentage(rowNumber, Convert.ToString(dr["RKVH%"]), "RKVH%");
                    errorFlag = (addUnit.rkvhPercent > 100 || addUnit.rkvhPercent < 0) ? true : false;

                    addUnit.controllerKwhInv = Convert.ToDouble(dr["Controller KWH/INV KWH"] is DBNull ? 0 : dr["Controller KWH/INV KWH"]);
                    addUnit.scheduledUnitsKwh = Convert.ToDouble(dr["Scheduled Units  (kWh)"] is DBNull || string.IsNullOrEmpty((string)dr["Scheduled Units  (kWh)"]) ? 0 : dr["Scheduled Units  (kWh)"]);
                    addUnit.exportKwh = Convert.ToDouble(dr["Export (kWh)"] is DBNull ? 0 : dr["Export (kWh)"]);
                    addUnit.importKwh = Convert.ToDouble(dr["Import (kWh)"] is DBNull ? 0 : dr["Import (kWh)"]);
                    addUnit.netExportKwh = Convert.ToDouble(dr["Net Export (kWh)"] is DBNull ? 0 : dr["Net Export (kWh)"]);
                    addUnit.exportKvah = Convert.ToDouble(dr["Export (kVAh)"] is DBNull ? 0 : dr["Export (kVAh)"]);
                    addUnit.importKvah = Convert.ToDouble(dr["Import (kVAh)"] is DBNull ? 0 : dr["Import (kVAh)"]);
                    addUnit.exportKvarhLag = Convert.ToDouble(dr["Export (kVArh lag)"] is DBNull ? 0 : dr["Export (kVArh lag)"]);
                    addUnit.importKvarhLag = Convert.ToDouble(dr["Import (kVArh lag)"] is DBNull ? 0 : dr["Import (kVArh lag)"]);
                    addUnit.exportKvarhLead = Convert.ToDouble(dr["Export (kVArh lead)"] is DBNull ? 0 : dr["Export (kVArh lead)"]);
                    addUnit.importKvarhLead = Convert.ToDouble(dr["Import (kVArh lead)"] is DBNull ? 0 : dr["Import (kVArh lead)"]);
                    addUnit.lineLoss = Convert.ToDouble(dr["LineLoss"] is DBNull ? 0 : dr["LineLoss"]);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    //api call used for importing wind:monthly jmr client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindJMR";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of generation failed
                    status = "Wind Monthly JMR Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarMonthlyLineLoss(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            bool errorFlag = false;
            int errorCount = 0;
            long rowNumber = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarMonthlyUploadingLineLosses> addSet = new List<SolarMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyUploadingLineLosses addUnit = new SolarMonthlyUploadingLineLosses();
                    rowNumber++;
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);
                    objImportBatch.importSiteId = addUnit.Site_Id;//C
                    addUnit.Month = Convert.ToString(dr["Month"]);
                    errorFlag = MonthList.Contains(addUnit.Month) ? false : true;
                    addUnit.month_no = Convert.ToInt32(MonthList[addUnit.Month]);
                    int year = Convert.ToInt32(addUnit.FY.Substring(0, 4));
                    addUnit.year = (addUnit.month_no > 3) ? year : year +=1;
                    addUnit.LineLoss = stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss (%)"]), "Line Loss (%)");
                    errorFlag = (addUnit.LineLoss > 100 || addUnit.LineLoss < 0) ? true : false;
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    status = "Solar Monthly Lineloss Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyLineLoss(string status, DataSet ds)
        {
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyUploadingLineLosses> addSet = new List<WindMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyUploadingLineLosses addUnit = new WindMonthlyUploadingLineLosses();
                    rowNumber++;
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.site = Convert.ToString(dr["Sites"]);
                    //added site_id recording
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //added site_id validation
                    errorFlag = siteValidation(addUnit.site, addUnit.site_id, rowNumber);
                    //added site_id recording for import_batches record
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.month = Convert.ToString(dr["Month"]);
                    addUnit.month_no = Convert.ToInt32(MonthList[addUnit.month]);
                    int finalYear = Convert.ToInt32(addUnit.fy.Substring(0, 4));
                    addUnit.year = (addUnit.month_no > 3) ? finalYear : finalYear = +1;
                    //addUnit.lineLoss = Convert.ToString(dr["Line Loss"]);
                    //string lineloss = Convert.ToString(dr["Line Loss"]);
                    addUnit.lineLoss = stringToPercentage(rowNumber, Convert.ToString(dr["Line Loss%"]), "Line Loss%");
                    errorFlag = (addUnit.lineLoss > 100 || addUnit.lineLoss < 0) ? true : false;
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                //validation success
                if (!(errorCount > 0))
                {
                    //api call used for importing wind:monthly linelosses client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyUploadingLineLosses";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    status = "Wind Monthly Line Losses Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }

        private async Task<int> InsertSolarMonthlyTargetKPI(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            long rowNumber = 0;
            int errorCount = 0;
            bool errorFlag = false;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarMonthlyTargetKPI> addSet = new List<SolarMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyTargetKPI addUnit = new SolarMonthlyTargetKPI();
                    rowNumber++;
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Month = Convert.ToString(dr["Month"]);
                    errorFlag = MonthList.Contains(addUnit.Month) ? false : true;
                    addUnit.month_no = Convert.ToInt32(MonthList[addUnit.Month]);
                    int year = Convert.ToInt32(addUnit.FY.Substring(0, 4));
                    addUnit.year = (addUnit.month_no > 3) ? year : year += 1;
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);//C
                    objImportBatch.importSiteId = addUnit.Site_Id;//C
                    addUnit.GHI = Convert.ToDouble(dr[3]);
                    addUnit.POA = Convert.ToDouble(dr[4]);
                    addUnit.kWh = Convert.ToDouble(dr[5]);
                    addUnit.MA = Convert.ToDouble(dr["MA (%)"]);
                    addUnit.IGA = Convert.ToDouble(dr["IGA (%)"]);
                    addUnit.EGA = Convert.ToDouble(dr["EGA (%)"]);
                    addUnit.PR = Convert.ToDouble(dr["PR (%)"]);
                    addUnit.PLF = Convert.ToDouble(dr["PLF (%)"]);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    status = "Solar Monthly Target KPI Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindMonthlyTargetKPI(string status, DataSet ds)
        {
            long rowNumber = 0;
            int errorCount = 0;
            bool errorFlag = false;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyTargetKPI> addSet = new List<WindMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyTargetKPI addUnit = new WindMonthlyTargetKPI();
                    rowNumber++;
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.month = Convert.ToString(dr["Month"]);
                    addUnit.month_no = Convert.ToInt32(MonthList[addUnit.month]);
                    int year = Convert.ToInt32(addUnit.fy.Substring(0, 4));
                    addUnit.year = (addUnit.month_no < 4 ? year += 1 : year);
                    addUnit.site = Convert.ToString(dr["Sites"]);
                    //added site_id recording
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //added site_id validation
                    errorFlag = siteValidation(addUnit.site, addUnit.site_id, rowNumber);
                    //added site_id recording for import_batches record
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.windSpeed = Convert.ToDecimal(dr["WindSpeed"]);
                    addUnit.kwh = Convert.ToDecimal(dr["kWh"]);
                    addUnit.ma = stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                    errorFlag = (addUnit.ma > 100 || addUnit.ma < 0) ? true : false;
                    addUnit.iga = stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                    errorFlag = (addUnit.iga > 100 || addUnit.iga < 0) ? true : false;
                    addUnit.ega = stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                    errorFlag = (addUnit.ega > 100 || addUnit.ega < 0) ? true : false;
                    addUnit.plf = stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                    errorFlag = (addUnit.ega > 100 || addUnit.plf < 0) ? true : false;
                    errorFlag = MonthList.Contains(addUnit.month) ? false : true;
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    //api call used for importing wind:monthly target kpi client data to the database
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
                else
                {
                    // add to error log that validation of file failed
                    status = "Wind Monthly Target KPI Validation Failed";
                    m_ErrorLog.SetError("," + status + ":");
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarDailyLoadShedding(string status, DataSet ds)
        {
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyLoadShedding> addSet = new List<SolarDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarDailyLoadShedding addUnit = new SolarDailyLoadShedding();
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                    objImportBatch.importSiteId = addUnit.Site_Id;//C
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Start_Time = Convert.ToDateTime(dr["Start Time"]).ToString("HH:mm:ss");
                    addUnit.End_Time = Convert.ToDateTime(dr["End Time"]).ToString("HH:mm:ss");
                    addUnit.Total_Time = Convert.ToDateTime(dr["Total Time"]).ToString("HH:mm:ss");
                    addUnit.Permissible_Load_MW = Convert.ToDouble(dr[" Permissible Load (MW)"]);
                    addUnit.Gen_loss_kWh = Convert.ToDouble(dr["Generation loss in KWH due to Load shedding"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyLoadShedding";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;
                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindDailyLoadShedding(string status, DataSet ds)
        {//siteID recorded
            long rowNumber = 0;
            int errorCount = 0;
            bool errorFlag = false;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindDailyLoadShedding> addSet = new List<WindDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyLoadShedding addUnit = new WindDailyLoadShedding();
                    rowNumber++;
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    errorFlag = siteValidation(addUnit.site, addUnit.site_id, rowNumber);
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.startTime = Convert.ToDateTime(dr["Start Time"]).ToString("hh:mm:ss");
                    addUnit.endTime = Convert.ToDateTime(dr["End Time"]).ToString("hh:mm:ss");
                    addUnit.totalTime = Convert.ToDateTime(dr["Total Time"]).ToString("hh:mm:ss");
                    //addUnit.startTime = Convert.ToString(dr["Start Time"]);
                    //addUnit.endTime = Convert.ToString(dr["End Time"]);
                    //addUnit.totalTime = Convert.ToString(dr["Total Time"]);
                    addUnit.permLoad = Convert.ToDouble(dr[" Permissible Load (MW)"]);
                    addUnit.genShedding = Convert.ToDouble(dr["Generation loss in KWH due to Load shedding"]);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);

                    //addUnit.stop_from = Convert.ToString(dr["Stop From"]);
                    // addUnit.stop_to = Convert.ToString(dr["Stop To"]);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindDailyLoadShedding";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            return responseCode;
        }

        //private async Task<int> InsertWindDailyJMR(string status, DataSet ds)
        //{//siteID recorded
        //    int responseCode = 400;
        //    if (ds.Tables.Count > 0)
        //    {
        //        List<WindDailyJMR> addSet = new List<WindDailyJMR>();
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            WindDailyJMR addUnit = new WindDailyJMR();
        //            addUnit.site = Convert.ToString(dr["Site"]);
        //            //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
        //            //objImportBatch.importSiteId = addUnit.site_id;
        //            addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
        //            addUnit.jmr_kwh = Convert.ToString(dr[" Permissible Load (MW)"]);
        //            addSet.Add(addUnit);
        //        }
        //        var json = JsonConvert.SerializeObject(addSet);
        //        var data = new StringContent(json, Encoding.UTF8, "application/json");
        //        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyJMR";
        //        using (var client = new HttpClient())
        //        {
        //            var response = await client.PostAsync(url, data);
        //            if (response.IsSuccessStatusCode)
        //            {
        //                status = "Successfully uploaded";
        //            }
        //            else
        //            {
        //                status = "Failed to upload";
        //            }
        //        }
        //    }
        //    return responseCode;
        //}

        private async Task<int> InsertSolarDailyTargetKPI(string status, DataSet ds)
        {
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyTargetKPI> addSet = new List<SolarDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarDailyTargetKPI addUnit = new SolarDailyTargetKPI();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Sites]);//C
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.GHI = Convert.ToDouble(dr[3]);
                    addUnit.POA = Convert.ToDouble(dr[4]);
                    addUnit.kWh = Convert.ToDouble(dr[5]);
                    addUnit.MA = Convert.ToDouble(dr["MA (%)"]);
                    addUnit.IGA = Convert.ToDouble(dr["IGA (%)"]);
                    addUnit.EGA = Convert.ToDouble(dr["EGA (%)"]);
                    addUnit.PR = Convert.ToDouble(dr["PR (%)"]);
                    addUnit.PLF = Convert.ToDouble(dr["PLF (%)"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyTargetKPI";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;
                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindDailyTargetKPI(string status, DataSet ds)
        {
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindDailyTargetKPI> addSet = new List<WindDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyTargetKPI addUnit = new WindDailyTargetKPI();
                    rowNumber++;
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.WindSpeed = Convert.ToDouble(dr["WindSpeed"]);
                    addUnit.kWh = Convert.ToDouble(dr["kWh"]);
                    addUnit.MA = stringToPercentage(rowNumber, Convert.ToString(dr["MA%"]), "MA%");
                    errorFlag = (addUnit.MA > 100 || addUnit.MA < 0) ? true : false;
                    addUnit.IGA = stringToPercentage(rowNumber, Convert.ToString(dr["IGA%"]), "IGA%");
                    errorFlag = (addUnit.IGA > 100 || addUnit.IGA < 0) ? true : false;
                    addUnit.EGA = stringToPercentage(rowNumber, Convert.ToString(dr["EGA%"]), "EGA%");
                    errorFlag = (addUnit.EGA > 100 || addUnit.EGA < 0) ? true : false;
                    addUnit.PLF = stringToPercentage(rowNumber, Convert.ToString(dr["PLF%"]), "PLF%");
                    errorFlag = (addUnit.PLF > 100 || addUnit.PLF < 0) ? true : false;
                    errorFlag = siteValidation(addUnit.Site, addUnit.site_id, rowNumber);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyTargetKPI";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarSiteMaster(string status, DataSet ds)
        {//siteID recorded
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarSiteMaster> addSet = new List<SolarSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarSiteMaster addUnit = new SolarSiteMaster();
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    addUnit.spv = Convert.ToString(dr["SPV"]);
                    addUnit.state = Convert.ToString(dr["State"]);
                    addUnit.dc_capacity = Convert.ToDouble(dr["DC Capacity (MWp)"]);
                    addUnit.ac_capacity = Convert.ToDouble(dr["AC Capacity (MW)"]);
                    addUnit.total_tarrif = Convert.ToDouble(dr["Total Tariff"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarSiteMaster";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;
                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindSiteMaster(string status, DataSet ds)
        {//siteID recorded
            long rowNumber = 0;
            int responseCode = 400;
            string llcompensation = "";
            if (ds.Tables.Count > 0)
            {
                List<WindSiteMaster> addSet = new List<WindSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindSiteMaster addUnit = new WindSiteMaster();
                    rowNumber++;
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    objImportBatch.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    addUnit.spv = Convert.ToString(dr["SPV"]);
                    addUnit.state = Convert.ToString(dr["State"]);
                    addUnit.model = Convert.ToString(dr["Model"]);
                    addUnit.capacity_mw = Convert.ToDouble(dr["Capacity(MW)"]);
                    addUnit.wtg = Convert.ToDouble(dr["WTG"]);
                    addUnit.total_mw = Convert.ToDouble(dr["Total_MW"]);
                    addUnit.tarrif = Convert.ToDouble(dr["Tariff"]);
                    addUnit.gbi = Convert.ToDouble(dr["GBI"]);
                    addUnit.total_tarrif = Convert.ToDouble(dr["Total_Tariff"]);
                    //llcompensation = Convert.ToString(dr["LL_Compensation"]);
                    //llcompensation = llcompensation.TrimEnd('%');
                    addUnit.ll_compensation = stringToPercentage(rowNumber, Convert.ToString(dr["LL_Compensation%"]), "LL_Compensation%");
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindSiteMaster";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;
                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarLocationMaster(string status, DataSet ds)
        {//siteID recorded
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarLocationMaster> addSet = new List<SolarLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarLocationMaster addUnit = new SolarLocationMaster();
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    objImportBatch.importSiteId = addUnit.site_id;//C
                    addUnit.eg = Convert.ToString(dr["EG"]);
                    addUnit.ig = Convert.ToString(dr["IG"]);
                    addUnit.icr_inv = Convert.ToString(dr["ICR/INV"]);
                    addUnit.icr = Convert.ToString(dr["ICR"]);
                    addUnit.inv = Convert.ToString(dr["INV"]);
                    addUnit.smb = Convert.ToString(dr["SMB"] is DBNull || string.IsNullOrEmpty((string)dr["SMB"]) ? "Nil" : dr["SMB"]);
                    addUnit.strings = Convert.ToString(dr["String"]);
                    addUnit.string_configuration = Convert.ToString(dr["String Configuration"] is DBNull || string.IsNullOrEmpty((string)dr["String Configuration"]) ? "Nil" : dr["String Configuration"]);
                    addUnit.total_string_current = Convert.ToDouble(dr["Total String Current (amp)"]);
                    addUnit.total_string_voltage = Convert.ToDouble(dr["Total String voltage"]);
                    addUnit.modules_quantity = Convert.ToDouble(dr[12]);
                    addUnit.wp = Convert.ToDouble(dr["Wp"]);
                    addUnit.capacity = Convert.ToDouble(dr["Capacity (KWp)"]);
                    addUnit.module_make = Convert.ToString(dr["Module Make"]);
                    addUnit.module_model_no = Convert.ToString(dr[16]);
                    addUnit.module_type = Convert.ToString(dr["Module Type"]);
                    addUnit.string_inv_central_inv = (Convert.ToString(dr["String Inv / Central Inv"]) == "Central Inverter") ? 2 : 1;
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarLocationMaster";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;

                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;

                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertWindLocationMaster(string status, DataSet ds)
        {//siteID recorded
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<WindLocationMaster> addSet = new List<WindLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindLocationMaster addUnit = new WindLocationMaster();
                    rowNumber++;
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_master_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    errorFlag = siteValidation(addUnit.site, addUnit.site_master_id, rowNumber);
                    objImportBatch.importSiteId = addUnit.site_master_id;
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.feeder = Convert.ToDouble(dr["Feeder"]);
                    addUnit.max_kwh_day = Convert.ToDouble(dr["Max. kWh/Day"]);
                    if (errorFlag)
                    {
                        errorCount++;
                        errorFlag = false;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindLocationMaster";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            return responseCode = (int)response.StatusCode;
                        }
                        else
                        {
                            status = "Failed to upload";
                            return responseCode = (int)response.StatusCode;
                        }
                    }
                }
            }
            return responseCode;
        }
        private async Task<int> InsertSolarAcDcCapacity(string status, DataSet ds)
        {
            int responseCode = 400;
            if (ds.Tables.Count > 0)
            {
                List<SolarInvAcDcCapacity> addSet = new List<SolarInvAcDcCapacity>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarInvAcDcCapacity addUnit = new SolarInvAcDcCapacity();
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                    addUnit.inverter = Convert.ToString(dr["Inverter"]);
                    objImportBatch.importSiteId = addUnit.site_id;
                    addUnit.dc_capacity = Convert.ToDecimal(dr["DC Capacity(kWp)"]);
                    addUnit.ac_capacity = Convert.ToDecimal(dr["AC Capacity (kW)"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarInvAcDcCapacity";
                using (var client = new HttpClient())
                {
                    var response = await client.PostAsync(url, data);
                    if (response.IsSuccessStatusCode)
                    {
                        status = "Successfully uploaded";
                        return responseCode = (int)response.StatusCode;


                    }
                    else
                    {
                        status = "Failed to upload";
                        return responseCode = (int)response.StatusCode;

                    }
                }
            }
            return responseCode;
        }

        private async Task importMetaData(string importType, string fileName)
        {
            int responseCode = 400;
            string status = "";
            objImportBatch.importFilePath = fileName;
            objImportBatch.importLogName = importData[1];
            string userName = HttpContext.Session.GetString("DisplayName");
            int userId = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            if (importType == "Solar")
            {
                objImportBatch.importType = "1";
            }
            else if (importType == "Wind")
            {
                objImportBatch.importType = "2";
            }
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
                }
            }
        }

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
                int convert = (int)Convert.ToInt64(dr["location_master_id"]);//D
                equipmentId.Add((string)dr["wtg"], convert);
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

            foreach (DataRow dr in dTable.Rows)
            {
                int convert = (int)Convert.ToInt64(dr["bd_type_id"]);//D
                breakdownType.Add((string)dr["bd_type_name"], convert);
            }
        }
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
                    int convert = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D
                    siteNameId.Add((string)dr["site"], convert);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int convert = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteNameId.Add((string)dr["site"], convert);
                }
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
                    int convert = (int)Convert.ToInt64(dr["site_master_solar_id"]);//D

                    siteName.Add(convert, (string)dr["site"]);
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    int convert = (int)Convert.ToInt64(dr["site_master_id"]);//D
                    siteName.Add(convert, (string)dr["site"]);
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

            foreach (DataRow dr in dTable.Rows)
            {
                int convert = (int)Convert.ToInt64(dr["site_master_id"]);//D
                eqSiteId.Add((string)dr["wtg"], convert);
            }
        }
        private async Task<bool> UploadFileToImportedFileFolder(IFormFile ufile)
        {
            if (ufile != null && ufile.Length > 0)
            {
                var fileName = Path.GetFileName(ufile.FileName);
                var filePath = env.ContentRootPath + @"C:\ImportedFile\" + fileName;
				 //var filePath = env.ContentRootPath + @"\ImportedFile\" + fileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ufile.CopyToAsync(fileStream);
                }
                return true;
            }
            return false;
        }

        public async Task<int> dgrSolarImport(int batchId)
        {
            ErrorLog("Inside DGT IMport <br>\r\n");
            int responseCodeGen = 0;
            int responseCodeBreak = 0;
            int responseCodePyro1 = 0;
            int responseCodePyro15 = 0;
            var dataGeneration = new StringContent(genJson, Encoding.UTF8, "application/json");
            var urlGeneration = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileGeneration?batchId=" + batchId + "";
            ErrorLog("Gen Url<br>\r\n"+ urlGeneration);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlGeneration, dataGeneration);
                
                if (response.IsSuccessStatusCode)
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Gen Import API Successful");
                    ErrorLog("True\r\n");
                }
                else
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Gen Import API Failed");
                    ErrorLog("False\r\n");
                }
            }

            var dataBreakdown = new StringContent(breakJson, Encoding.UTF8, "application/json");
            var urlBreakdown = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileBreakDown?batchId=" + batchId + "";
            ErrorLog("BreakDOwn\r\n"+ urlBreakdown);
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(urlBreakdown, dataBreakdown);
                if (response.IsSuccessStatusCode)
                {
                    ErrorLog("True\r\n");
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Break Import API Successful");
                }
                else
                {
                    ErrorLog("False\r\n");
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Break Import API Failed");
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
                    m_ErrorLog.SetError(",Solar Pyro-1 Import API Successful");
                }
                else
                {
                    ErrorLog("False\r\n");
                    responseCodePyro1 = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Pyro-1 Import API Failed");
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
                    m_ErrorLog.SetError(",Solar Pyro-15 Import API Successful");
                }
                else
                {
                    ErrorLog("False\r\n");
                    responseCodePyro15 = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Solar Pyro-15 Import API Failed");
                }
            }
            if (responseCodeGen != 200 || responseCodeBreak != 200 || responseCodePyro1 != 200 || responseCodePyro15 != 200)
            {
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
                    m_ErrorLog.SetError(",Wind Generation Import API Successful");
                }
                else
                {
                    responseCodeGen = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind Generation Import API Failed");
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
                    m_ErrorLog.SetError(",Wind BreakDown Import API Successful:");
                }
                else
                {
                    responseCodeBreak = (int)response.StatusCode;
                    m_ErrorLog.SetError(",Wind BreakDown Import API Failed:");
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

        public bool siteValidation(string site, int siteId, long rowNumber)
        {
            bool response = false;
            if (!(siteName.ContainsKey(siteId)))
            {
                response = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid Site - <" + site + "> was not found in master records");
            }
            return response;
        }

        public async Task<bool> bdTypeValidation(string bdtype, long rowNumber)
        {
            bool response = false;
            if (!(breakdownType.ContainsKey(bdtype)))
            {
                response = true;
                m_ErrorLog.SetError(",File Row <" + rowNumber + "> Invalid breakdown type - <" + bdtype + "> was not found in master records");
            }
            return response;
        }

        public bool importDateValidation(DateTime importDate)
        {
            bool invalidDate = false;
            DateTime today = DateTime.Now;
            TimeSpan dayDiff = today - importDate;
            int dayOfWeek = (int)today.DayOfWeek;
            //for DayOfWeek function 
            //if file-date is of previous day and today is from Tuesday-Friday
            if (dayDiff.Days == 1 && dayOfWeek > 1 && dayOfWeek < 6)
            {
                // file date is correct
                invalidDate = false;
            }
            //if today is Monday and file-date is of day-before-yesterday or yesterday
            else if (dayOfWeek == 1 && dayDiff.Days > 0 && dayDiff.Days <= 2)
            {
                invalidDate = false;
            }
            else
            {
                //Pending : log to error log file
                invalidDate = true;
            }
            return invalidDate;
        }
        //public bool uniformSolarSiteValidation(long rowNo, int siteId)
        //{
        //    bool invalidSiteUniformity = false;
        //}
        public bool uniformWindSiteValidation(long rowNo, int siteId, string wtg)
        {
            bool invalidSiteUniformity = false;
            if (rowNo == 1)
            {
                previousSite = siteId;
                if (fileSheets.Contains("Uploading_File_Generation") && importData[0] == "Wind")
                {
                    if (siteId == 0)
                    {
                        m_ErrorLog.SetError(",File Row <" + rowNo + "> Invalid Site Id: " + siteId + " due to invalid wtg: " + wtg);
                    }
                }
                else
                {
                    if (windSiteUserAccess.Contains(siteId))
                    {
                        //add error log : user has access to site
                        m_ErrorLog.SetInformation(", User has access to site : " + siteName[siteId]);
                    }
                    else
                    {
                        //add error log : user does not have access to site
                        m_ErrorLog.SetError(", User does not have access to site : " + siteName[siteId]);
                        invalidSiteUniformity = true;
                    }
                }
                //valdiate if the user has access to this site
                //Collection of site for upload access
                //Check if this siteid is in the above collection of sites accessible to the user
                //Get Site name should be in csv file
            }
            if (previousSite != siteId)
            {
                //pending log error in csv
                m_ErrorLog.SetError(",File Row <" + rowNo + "> Site entry is not the same as in other rows");
                invalidSiteUniformity = true;
            }
            return invalidSiteUniformity;
        }
        public double stringToPercentage(long rowNo, string excelValue, string columnName)
        {
            double percentage = 0;
            if (!(string.IsNullOrEmpty(excelValue)))
            {
                excelValue = excelValue.Replace("%", "");
                excelValue = excelValue.Replace(" ", "");
                percentage = Convert.ToDouble(excelValue);
                if (percentage > 100)
                {
                    m_ErrorLog.SetError(",File Row <" + rowNo + "> Percentage value in column <" + columnName + "> exceeds 100% value");
                }
                else if (percentage < 0)
                {
                    m_ErrorLog.SetError(",File Row <" + rowNo + "> Percentage value in column <" + columnName + "> is negative");
                }
            }
            else
            {
                return 0;
            }

            return percentage;
        }
    }
}