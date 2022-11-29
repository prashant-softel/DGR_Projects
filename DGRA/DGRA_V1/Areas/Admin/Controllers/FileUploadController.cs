using DGRA_V1.Common;
using DGRA_V1.Models;
using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

namespace DGRA_V1.Areas.admin.Controllers
{
    [Area("admin")]
    public class FileUploadController : Controller
    {
        ImportLog meta = new ImportLog();
        private IDapperRepository _idapperRepo;
        private IWebHostEnvironment env;
        public FileUploadController(IDapperRepository idapperRepo, IWebHostEnvironment obj)
        {
            _idapperRepo = idapperRepo;
            m_ErrorLog = new ErrorLog(meta, obj);
            env = obj;
        }
        static string[] importData = new string[2];
        ArrayList kpiArgs = new ArrayList();
        //WindUploadingFileValidation m_ValidationObject;
        ErrorLog m_ErrorLog;
        // static string[] importData = new string[2];

        Hashtable equipmentId = new Hashtable();
        Hashtable breakdownType = new Hashtable();//(B)Gets bdTypeID from bdTypeName: BDType table
        Hashtable siteNameId = new Hashtable(); //(C)Gets siteId from siteName
        Hashtable siteName = new Hashtable(); //(D)Gets siteName from siteId
        Hashtable eqSiteId = new Hashtable();//(E)Gets siteId from (wtg)equipmentName
        Hashtable MonthList = new Hashtable(){{"Jan",1},{"Feb",2},{"Mar",3},{"Apr",4},{"May",5},{"Jun",6},{"Jul",7},{"Aug",8},{"Sept",9},{"Oct",10},{"Nov",11},{"Dec",12}};

        /*~FileUploadController()
        {
            //Destructor
            //delete m_ValidationObject;
            //delete m_ErrorLog;
        }*/

        [HttpGet]
        public ActionResult Upload()
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
        [HttpPost]
        public async Task<ActionResult> Upload(string fileUpload)
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

        public async Task<string> ExcelDataReaderAndUpload(IFormFile file, string fileUpload)
        {
            var saveFile = file;
            OleDbConnection oconn = null;
            importData[0] = fileUpload;
            string status = "";
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
                    // HttpContext.Current.Server.MapPath(@"~" + @"\TempFile");
                    //if (!Directory.Exists(Server.MapPath(@"~" + @"\TempFile")))
                    //{
                    //    DirectoryInfo dinfo = Directory.CreateDirectory(Server.MapPath(@"~" + @"\TempFile"));
                    //}
                    else
                    {
                        string tempName = file.FileName;
                        importData[1] = tempName;//Server.MapPath("/" + tempName);

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
                            // file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xlsx"));

                            using (var stream = new FileStream(@"\TempFile\docupload.xlsx", FileMode.Create))
                            {
                                file.CopyTo(stream);
                            }

                            // file.SaveAs(Server.MapPath(@"~" + @"\TempFile\docupload.xlsx"));
                            // string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + Server.MapPath(@"~" + @"\TempFile\docupload.xlsx") + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + @"\TempFile\docupload.xlsx" + ";" + "Extended Properties=" + "\"" + "Excel 12.0;HDR=YES;" + "\"";
                            List<string> fileSheets = new List<string>();
                            oconn = new System.Data.OleDb.OleDbConnection(connectionString);
                            oconn.Open();
                            DataTable dt = null;
                            dt = oconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            oconn.Close();

                            m_ErrorLog.SetInformation("Status,Message");
                            m_ErrorLog.SetInformation(",File Import Initiated:");

                            foreach (DataRow row in dt.Rows)
                            {
                                fileSheets.Add(row["TABLE_NAME"].ToString());
                            }
                            if (fileSheets.Contains("Uploading_File_Generation$") || fileSheets.Contains("Uploading_File_Breakdown$"))
                            {
                                masterHashtable_SiteName_To_SiteId();
                                if (fileUpload == "Wind")
                                {
                                    masterHashtable_WtgToWtgId();
                                    masterHashtable_SiteIdToSiteName(); 
                                    masterHashtable_BDNameToBDId();
                                    masterHashtable_WtgToSiteId();
                                }
                            }

                            if (fileSheets.Contains("Location_Master$") || fileSheets.Contains("Site_Master$") || fileSheets.Contains("Monthly_JMR_Input_and_Output$") || fileSheets.Contains("Monthly_LineLoss$") || fileSheets.Contains("Monthly_Target_KPI$") || fileSheets.Contains("Daily_Target_KPI$"))
                            {
                                masterHashtable_SiteName_To_SiteId();
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
                                            m_ErrorLog.SetInformation(",Reviewing Solar_Uploading_File_Generation WorkSheet:");
                                            status = await InsertSolarFileGeneration(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar Generation Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Generation Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Generation Import:");
                                            }
                                        }
                                        else if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind_Uploading_File_Generation WorkSheet :");
                                            status = await InsertWindFileGeneration(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Wind Generation Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Generation Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Generation Import:");
                                            }
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
                                            m_ErrorLog.SetInformation(",Reviewing Solar Uploading_File_Breakdown WorkSheet:");
                                            status = await InsertSolarFileBreakDown(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Solar Breakdown Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Breakdown Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Breakdown Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Uploading_File_Breakdown WorkSheet:");
                                            status = await InsertWindFileBreakDown(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors,Wind BreakDown Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind BreakDown Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind BreakDown Import:");
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
                                            if (status == "Successfully uploaded")
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
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Monthly_JMR_Input_and_Output + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_JMR_Input_and_Output WorkSheet:");
                                            status = await InsertWindMonthlyJMR(status, ds);
                                            if (status == "Successfully uploaded")
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
                                            status = await InsertSolarMonthlyJMR(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Monthly_LineLoss + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_LineLoss WorkSheet:");
                                            status = await InsertWindMonthlyLineLoss(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Monthly LineLoss Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Monthly LineLoss Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Wind Monthly LineLoss Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Monthly_LineLoss WorkSheet:");
                                            status = await InsertSolarMonthlyLineLoss(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Monthly_Target_KPI + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Monthly_Target_KPI WorkSheet:");
                                            status = await InsertWindMonthlyTargetKPI(status, ds);
                                            if (status == "Successfully uploaded")
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
                                            status = await InsertSolarMonthlyTargetKPI(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Daily_Load_Shedding + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Load_Shedding WorkSheet:");
                                            status = await InsertWindDailyLoadShedding(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Daily_Load_Shedding Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Daily_Load_Shedding Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Daily_Load_Shedding Import:");
                                            }
                                        }
                                        else
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Daily_Load_Shedding WorkSheet:");
                                            status = await InsertSolarDailyLoadShedding(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Solar Daily_Load_Shedding Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Solar Daily_Load_Shedding Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Solar Daily_Load_Shedding Import:");
                                            }
                                        }
                                    }
                                }
                                else if (excelSheet == FileSheetType.Daily_JMR_Input_and_Output)
                                {
                                    sql = "SELECT * FROM [" + FileSheetType.Daily_JMR_Input_and_Output + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_JMR_Input_and_Output WorkSheet:");
                                            status = await InsertWindDailyJMR(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Daily_Target_KPI + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Daily_Target_KPI WorkSheet:");
                                            status = await InsertWindDailyTargetKPI(status, ds);
                                            if (status == "Successfully uploaded")
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
                                            status = await InsertSolarDailyTargetKPI(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Site_Master + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Site_Master WorkSheet:");
                                            status = await InsertWindSiteMaster(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Site_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Site_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Site_Master Import:");
                                            }
                                        }
                                        else if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Site_Master WorkSheet:");
                                            status = await InsertSolarSiteMaster(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Location_Master + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Wind Location_Master WorkSheet:");
                                            status = await InsertWindLocationMaster(status, ds);
                                            if (status == "Successfully uploaded")
                                            {
                                                m_ErrorLog.SetInformation("No Errors, Wind Location_Master Import Successful:");
                                            }
                                            else
                                            {
                                                m_ErrorLog.SetError(",Wind Location_Master Import Failed:");
                                                m_ErrorLog.SetInformation(",End of Wind Location_Master Import:");
                                            }
                                        }
                                        else if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar Location_Master WorkSheet:");
                                            status = await InsertSolarLocationMaster(status, ds);
                                            if (status == "Successfully uploaded")
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
                                    sql = "SELECT * FROM [" + FileSheetType.Solar_AC_DC_Capacity + "]";
                                    OleDbCommand cmd = new OleDbCommand(sql, oconn);
                                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                                    da.Fill(ds);
                                    oconn.Close();

                                    if (ds.Tables.Count > 0)
                                    {
                                        if (fileUpload == "Wind")
                                        {
                                            status = "Wrong file upload type selected";
                                        }
                                        else if (fileUpload == "Solar")
                                        {
                                            m_ErrorLog.SetInformation(",Reviewing Solar AC_DC_Capacity WorkSheet:");
                                            status = await InsertSolarAcDcCapacity(status, ds);
                                            if (status == "Successfully uploaded")
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
                            }
                            if (status == "Successfully uploaded")
                            {
                                m_ErrorLog.SetInformation(",Import Operation Complete :");
                                m_ErrorLog.SaveToCSV(importData[1]);
                                await UploadFile(saveFile);
                                await importMetaData(importData[0], importData[1]);
                                if (fileSheets.Contains("Uploading_File_Generation$") || fileSheets.Contains("Uploading_File_Breakdown$"))
                                {
                                    if (fileUpload == "Wind")
                                    {
                                        var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/CalculateDailyWindKPI?fromDate=" + Convert.ToDateTime(kpiArgs[0]).ToString("yyyy-MM-dd") + "&toDate=" + Convert.ToDateTime(kpiArgs[1]).ToString("yyyy-MM-dd") + "&site=" + (string)kpiArgs[2] + "&logFileName=" + meta.importLogName + "";
                                        using (var client = new HttpClient())
                                        {
                                            await client.GetAsync(url);
                                        }


                                    }

                                }
                            }
                            else
                            {
                                m_ErrorLog.SetInformation(",Import Operation Failed:");
                                m_ErrorLog.SaveToCSV(importData[1]);
                            }
                        }
                        catch (Exception ex)
                        {
                            status = "Something went wrong" + ex.ToString();
                            m_ErrorLog.SetError(",Import Operation Failed :");
                        }
                    }
                }
                catch (Exception ex)
                {
                    status = "Something went wrong " + ex.ToString();
                }
            }
            else
            {
                status = "File format not supported";
            }
            return status;
        }
        //Remove static
        private async Task<string> InsertSolarFileGeneration(string status, DataSet ds)
        {
            //Changed model datatype to reflect database
            //Updated API function
            //records site id
            //percentage value import fixed
            //importing successfully?
            //validating successfully?

            bool errorFlag = false;
            int rowNumber = 0;
            int errorCount = 0;
            if (ds.Tables.Count > 0)
            {
                SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation(m_ErrorLog);
                List<SolarUploadingFileGeneration> addSet = new List<SolarUploadingFileGeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rowNumber++;
                    SolarUploadingFileGeneration addUnit = new SolarUploadingFileGeneration();
                    addUnit.date = Convert.ToDateTime(dr["Date"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                    meta.importSiteId = addUnit.site_id;//C
                    addUnit.inverter = Convert.ToString(dr["Inverter"]);
                    addUnit.inv_act = Convert.ToDecimal(dr["Inv_Act(KWh)"]);
                    addUnit.plant_act = Convert.ToDecimal(dr["Plant_Act(kWh)"]);
                    addUnit.pi = Convert.ToDecimal(dr["PI(%)"]);
                    decimal percentage = addUnit.pi * 100;
                    if (!(percentage > 100)) { addUnit.pi = percentage; }
                    errorFlag = validationObject.validateGenerationData(rowNumber, addUnit.date, addUnit.inverter, addUnit.inv_act, addUnit.plant_act);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileGeneration";
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

            }
            return status;
        }

        private async Task<string> InsertWindFileGeneration(string status, DataSet ds)
        {//siteID recorded
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            DateTime fromDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
            DateTime toDate = Convert.ToDateTime(ds.Tables[0].Rows[0]["Date"]);
            DateTime nextDate;
            string site = "";
            WindUploadingFileValidation validationObject = new WindUploadingFileValidation(m_ErrorLog);

            if (ds.Tables.Count > 0)
            {
                List<WindUploadingFileGeneration> addSet = new List<WindUploadingFileGeneration>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    rowNumber++;
                    WindUploadingFileGeneration addUnit = new WindUploadingFileGeneration();
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                    addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);//E
                    addUnit.site_name = (string)(siteName[addUnit.site_id]);//D
                    meta.importSiteId = addUnit.site_id;
                    nextDate = Convert.ToDateTime(dr["Date"]);
                    fromDate = ((nextDate < fromDate) ? (nextDate) : (fromDate));
                    toDate = (nextDate > toDate) ? (nextDate) : (toDate);
                    site = Convert.ToString(addUnit.site_id);
                    addUnit.wind_speed = Convert.ToDecimal(dr["Wind_Speed"]);
                    addUnit.kwh = Convert.ToDecimal(dr["kWh"]);
                    addUnit.operating_hrs = Convert.ToDecimal(dr["Gen_Hrs"]);
                    addUnit.production_rs = Convert.ToDecimal(dr["Lull_Hrs"]);
                    addUnit.grid_hrs = Convert.ToDecimal(dr["Grid_Hrs"]);

                    errorFlag = validationObject.validateGenData(rowNumber, addUnit.date, addUnit.wtg, addUnit.wind_speed, addUnit.kwh, addUnit.operating_hrs, addUnit.production_rs, addUnit.grid_hrs);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileGeneration";
                    using (var client = new HttpClient())
                    {
                        var response = await client.PostAsync(url, data);
                        if (response.IsSuccessStatusCode)
                        {
                            status = "Successfully uploaded";
                            kpiArgs.Add(fromDate);
                            kpiArgs.Add(toDate);
                            kpiArgs.Add(site);
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

            }

            return status;
        }

        private async Task<string> InsertSolarFileBreakDown(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //importing successfully?
            //validating successfully?
            long rowNumber = 0;
            bool errorFlag = false;
            int errorCount = 0;
            if (ds.Tables.Count > 0)
            {
                SolarUploadingFileValidation validationObject = new SolarUploadingFileValidation();
                List<SolarUploadingFileBreakDown> addSet = new List<SolarUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingFileBreakDown addUnit = new SolarUploadingFileBreakDown();
                    rowNumber++;
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                    meta.importSiteId = addUnit.site_id;//C
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
                    addUnit.action_taken = Convert.ToString(dr["ActionTaken"]);
                    errorFlag = validationObject.validateBreakDownData(rowNumber, addUnit.from_bd, addUnit.to_bd, addUnit.igbd);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingFileBreakDown";
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
                List<WindUploadingFileBreakDown> addSet = new List<WindUploadingFileBreakDown>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindUploadingFileBreakDown addUnit = new WindUploadingFileBreakDown();
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.wtg_id = Convert.ToInt32(equipmentId[addUnit.wtg]);//A
                    addUnit.site_id = Convert.ToInt32(eqSiteId[addUnit.wtg]);//E
                    addUnit.site_name = (string)siteName[addUnit.site_id];//D
                    meta.importSiteId = addUnit.site_id;
                    addUnit.bd_type = Convert.ToString(dr["BD_Type"]);
                    addUnit.bd_type_id = Convert.ToInt32(breakdownType[addUnit.bd_type]);//B
                    addUnit.stop_from = Convert.ToString(dr["Stop From"]);
                    addUnit.stop_to = Convert.ToString(dr["Stop To"]);
                    addUnit.total_stop = ValidationObject.breakDownCalc(addUnit.stop_from, addUnit.stop_to);
                    addUnit.error_description = Convert.ToString(dr["Error description"]);
                    addUnit.action_taken = Convert.ToString(dr["Action Taken"]);
                    errorFlag = ValidationObject.validateBreakDownData(rowNumber, addUnit.bd_type, addUnit.stop_from, addUnit.stop_to);
                    if (errorFlag)
                    {
                        errorCount++;
                        continue;
                    }
                    addUnit.stop_from = Convert.ToDateTime(dr["Stop From"]).ToString("hh:mm:ss");
                    addUnit.stop_to = Convert.ToDateTime(dr["Stop To"]).ToString("hh:mm:ss");
                    addSet.Add(addUnit);
                }
                if (!(errorCount > 0))
                {
                    var json = JsonConvert.SerializeObject(addSet);
                    var data = new StringContent(json, Encoding.UTF8, "application/json");
                    var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindUploadingFileBreakDown";
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
            }
            return status;
        }
        private async Task<string> InsertSolarPyranoMeter1Min(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //importing successfully?
            //validating successfully?
            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter1Min> addSet = new List<SolarUploadingPyranoMeter1Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingPyranoMeter1Min addUnit = new SolarUploadingPyranoMeter1Min();
                    addUnit.date_time = Convert.ToDateTime(dr["Time stamp"]);
                    string site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[site]);//C
                    meta.importSiteId = addUnit.site_id;
                    addUnit.ghi_1 = Convert.ToDecimal((dr["GHI-1"] is DBNull) ? 0 : dr["GHI-1"]);
                    addUnit.ghi_2 = Convert.ToDecimal((dr["GHI-2"] is DBNull) ? 0 : dr["GHI-2"]);
                    addUnit.poa_1 = Convert.ToDecimal((dr["POA-1"] is DBNull) ? 0 : dr["POA-1"]);
                    addUnit.poa_2 = Convert.ToDecimal((dr["POA-2"] is DBNull) ? 0 : dr["POA-2"]);
                    addUnit.poa_3 = Convert.ToDecimal((dr["POA-3"] is DBNull) ? 0 : dr["POA-3"]);
                    addUnit.poa_4 = Convert.ToDecimal((dr["POA-4"] is DBNull) ? 0 : dr["POA-4"]);
                    addUnit.poa_5 = Convert.ToDecimal((dr["POA-5"] is DBNull) ? 0 : dr["POA-5"]);
                    addUnit.poa_6 = Convert.ToDecimal((dr["POA-6"] is DBNull) ? 0 : dr["POA-6"]);
                    addUnit.poa_7 = Convert.ToDecimal((dr["POA-7"] is DBNull) ? 0 : dr["POA-7"]);
                    addUnit.avg_ghi = Convert.ToDecimal((dr["Average GHI (w/m²)"] is DBNull) ? 0 : dr["Average GHI (w/m²)"]);
                    addUnit.avg_poa = Convert.ToDecimal((dr["Average POA (w/m²)"] is DBNull) ? 0 : dr["Average POA (w/m²)"]);
                    addUnit.amb_temp = Convert.ToDecimal((dr["Ambient Temp"] is DBNull) ? 0 : dr["Ambient Temp"]);
                    addUnit.mod_temp = Convert.ToDecimal((dr["Module Temp"] is DBNull) ? 0 : dr["Module Temp"]);
                    addSet.Add(addUnit);
                }

                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter1Min";
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

        private async Task<string> InsertSolarPyranoMeter15Min(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id

            if (ds.Tables.Count > 0)
            {
                List<SolarUploadingPyranoMeter15Min> addSet = new List<SolarUploadingPyranoMeter15Min>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarUploadingPyranoMeter15Min addUnit = new SolarUploadingPyranoMeter15Min();
                    addUnit.date_time = Convert.ToDateTime(dr["Time stamp"]);
                    string site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[site]);//C
                    meta.importSiteId = addUnit.site_id;
                    addUnit.ghi_1 = Convert.ToDecimal((dr["GHI-1"] is DBNull) ? 0 : dr["GHI-1"]);
                    addUnit.ghi_2 = Convert.ToDecimal((dr["GHI-2"] is DBNull) ? 0 : dr["GHI-2"]);
                    addUnit.poa_1 = Convert.ToDecimal((dr["POA-1"] is DBNull) ? 0 : dr["POA-1"]);
                    addUnit.poa_2 = Convert.ToDecimal((dr["POA-2"] is DBNull) ? 0 : dr["POA-2"]);
                    addUnit.poa_3 = Convert.ToDecimal((dr["POA-3"] is DBNull) ? 0 : dr["POA-3"]);
                    addUnit.poa_4 = Convert.ToDecimal((dr["POA-4"] is DBNull) ? 0 : dr["POA-4"]);
                    addUnit.poa_5 = Convert.ToDecimal((dr["POA-5"] is DBNull) ? 0 : dr["POA-5"]);
                    addUnit.poa_6 = Convert.ToDecimal((dr["POA-6"] is DBNull) ? 0 : dr["POA-6"]);
                    addUnit.poa_7 = Convert.ToDecimal((dr["POA-7"] is DBNull) ? 0 : dr["POA-7"]);
                    addUnit.avg_ghi = Convert.ToDecimal((dr["Average GHI (w/m²)"] is DBNull) ? 0 : dr["Average GHI (w/m²)"]);
                    addUnit.avg_poa = Convert.ToDecimal((dr["Average POA (w/m²)"] is DBNull) ? 0 : dr["Average POA (w/m²)"]);
                    addUnit.amb_temp = Convert.ToDecimal((dr["Ambient Temp"] is DBNull) ? 0 : dr["Ambient Temp"]);
                    addUnit.mod_temp = Convert.ToDecimal((dr["Module Temp"] is DBNull) ? 0 : dr["Module Temp"]);
                    addSet.Add(addUnit);
                }

                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertSolarUploadingPyranoMeter15Min";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarUploadingPyranoMeter15Min";
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
        private async Task<string> InsertSolarMonthlyJMR(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            if (ds.Tables.Count > 0)
            {
                List<SolarMonthlyJMR> addSet = new List<SolarMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyJMR addUnit = new SolarMonthlyJMR();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                    //meta.importSiteId = addUnit.site_id;//C
                    addUnit.Plant_Section = Convert.ToString(dr["Plant Section"]);

                    addUnit.Controller_KWH_INV = Convert.ToDecimal((dr["Controller KWH/INV KWH"] is DBNull) ? 0 : dr["Controller KWH/INV KWH"]);
                    addUnit.Scheduled_Units_kWh = Convert.ToDecimal((dr["Scheduled Units (kWh)"] is DBNull) ? 0 : dr["Scheduled Units (kWh)"]);
                    addUnit.Export_kWh = Convert.ToDecimal((dr["Export (kWh)"] is DBNull) ? 0 : dr["Export (kWh)"]);
                    addUnit.Import_kWh = Convert.ToDecimal((dr["Import (kWh)"] is DBNull) ? 0 : dr["Import (kWh)"]);
                    addUnit.Net_Export_kWh = Convert.ToDecimal((dr["Net Export (kWh)"] is DBNull) ? 0 : dr["Net Export (kWh)"]);
                    addUnit.Export_kVAh = Convert.ToDecimal((dr["Export (kVAh)"] is DBNull) ? 0 : dr["Export (kVAh)"]);
                    addUnit.Import_kVAh = Convert.ToDecimal((dr["Import (kVAh)"] is DBNull) ? 0 : dr["Import (kVAh)"]);
                    addUnit.Export_kVArh_lag = Convert.ToDecimal((dr["Export (kVArh lag)"] is DBNull) ? 0 : dr["Export (kVArh lag)"]);
                    addUnit.Import_kVArh_lag = Convert.ToDecimal((dr["Import (kVArh lag)"] is DBNull) ? 0 : dr["Import (kVArh lag)"]);
                    addUnit.Export_kVArh_lead = Convert.ToDecimal((dr["Export (kVArh lead)"] is DBNull) ? 0 : dr["Export (kVArh lead)"]);
                    addUnit.Import_kVArh_lead = Convert.ToDecimal((dr["Import (kVArh lead)"] is DBNull) ? 0 : dr["Import (kVArh lead)"]);
                    addUnit.JMR_date = Convert.ToDateTime((dr["JMR date"] is DBNull) ? "0000-00-00 00:00:00" : dr["JMR date"]).ToString("yyyy-MM-dd");
                    addUnit.JMR_Month = Convert.ToString(dr["JMR Month"]);
                    addUnit.JMR_Year = Convert.ToString(dr["JMR Year"]);
                    addUnit.LineLoss = Convert.ToDecimal((dr["LineLoss"] is DBNull) ? 0 : dr["LineLoss"]);

                    addUnit.Line_Loss_percentage = Convert.ToDecimal((dr["Line Loss%"] is DBNull) ? 0 : dr["Line Loss%"]);
                    decimal percentage = addUnit.Line_Loss_percentage * 100;
                    if (!(percentage > 100)) { addUnit.Line_Loss_percentage = percentage; }

                    addUnit.RKVH_percentage = Convert.ToDecimal((dr["RKVH%"] is DBNull) ? 0 : dr["RKVH%"]);
                    percentage = addUnit.RKVH_percentage * 100;
                    if (!(percentage > 100)) { addUnit.RKVH_percentage = percentage; }

                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarJMR";
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

        private async Task<string> InsertWindMonthlyJMR(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyJMR> addSet = new List<WindMonthlyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyJMR addUnit = new WindMonthlyJMR();
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.siteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    meta.importSiteId = addUnit.siteId;

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
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindJMR";
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
        private async Task<string> InsertSolarMonthlyLineLoss(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            if (ds.Tables.Count > 0)
            {
                bool errorFlag = false;
                List<SolarMonthlyUploadingLineLosses> addSet = new List<SolarMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyUploadingLineLosses addUnit = new SolarMonthlyUploadingLineLosses();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);
                    meta.importSiteId = addUnit.Site_Id;//C

                    addUnit.Month = Convert.ToString(dr["Month"]);

                    decimal percentage = 0;
                    addUnit.LineLoss = Convert.ToDecimal((dr["Line Loss (%)"] is DBNull) ? 0 : dr["Line Loss (%)"]);
                    percentage = addUnit.LineLoss * 100;
                    if (!(percentage > 100)) { addUnit.LineLoss = percentage; }

                    addSet.Add(addUnit);
                }
                if (!(errorFlag))
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
            }
            return status;
        }
        private async Task<string> InsertWindMonthlyLineLoss(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyUploadingLineLosses> addSet = new List<WindMonthlyUploadingLineLosses>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyUploadingLineLosses addUnit = new WindMonthlyUploadingLineLosses();
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    meta.importSiteId = addUnit.site_id;//C
                    addUnit.month = Convert.ToString(dr["Month"]);
                    addUnit.month_number = Convert.ToInt32(MonthList[addUnit.month]);
                    int hiphen = addUnit.fy.IndexOf("-");
                    hiphen += 1;
                    int finalYear = Convert.ToInt32("20"+addUnit.fy.Substring(hiphen));
                    addUnit.year = (addUnit.month_number>3)?finalYear-=1:finalYear; 
                    addUnit.lineLoss = Convert.ToString(dr["Line Loss"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyUploadingLineLosses";
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

        private async Task<string> InsertSolarMonthlyTargetKPI(string status, DataSet ds)
        {
            //Changed model datatypes reflecting database
            //Updated API function
            //records site id
            //fixed percentages
            if (ds.Tables.Count > 0)
            {
                List<SolarMonthlyTargetKPI> addSet = new List<SolarMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarMonthlyTargetKPI addUnit = new SolarMonthlyTargetKPI();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Month = Convert.ToString(dr["Month"]);
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Sites]);//C
                    meta.importSiteId = addUnit.Site_Id;//C
                    addUnit.GHI = Convert.ToDecimal(dr[3]);
                    addUnit.POA = Convert.ToDecimal(dr[4]);
                    addUnit.kWh = Convert.ToDecimal(dr[5]);
                    addUnit.MA = Convert.ToDecimal(dr["MA (%)"]);
                    addUnit.IGA = Convert.ToDecimal(dr["IGA (%)"]);
                    addUnit.EGA = Convert.ToDecimal(dr["EGA (%)"]);
                    addUnit.PR = Convert.ToDecimal(dr["PR (%)"]);
                    addUnit.PLF = Convert.ToDecimal(dr["PLF (%)"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertSolarMonthlyTargetKPI";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarMonthlyTargetKPI";
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
        private async Task<string> InsertWindMonthlyTargetKPI(string status, DataSet ds)
        {//siteId recorded
            if (ds.Tables.Count > 0)
            {
                List<WindMonthlyTargetKPI> addSet = new List<WindMonthlyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindMonthlyTargetKPI addUnit = new WindMonthlyTargetKPI();
                    addUnit.fy = Convert.ToString(dr["FY"]);
                    addUnit.month = Convert.ToString(dr["Month"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //meta.importSiteId = addUnit.site_id;//C

                    addUnit.windSpeed = Convert.ToDecimal(dr["WindSpeed"]);
                    addUnit.kwh = Convert.ToDecimal(dr["kWh"]);
                    addUnit.ma = Convert.ToString(dr["MA"]);
                    addUnit.iga = Convert.ToString(dr["IGA"]);
                    addUnit.ega = Convert.ToString(dr["EGA"]);
                    addUnit.plf = Convert.ToString(dr["PLF"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertMonthlyTargetKPI";
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
        private async Task<string> InsertSolarDailyLoadShedding(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyLoadShedding> addSet = new List<SolarDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarDailyLoadShedding addUnit = new SolarDailyLoadShedding();
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    addUnit.Site_Id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                    meta.importSiteId = addUnit.Site_Id;//C
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Start_Time = Convert.ToDateTime(dr["Start Time"]).ToString("HH:mm:ss");
                    addUnit.End_Time = Convert.ToDateTime(dr["End Time"]).ToString("HH:mm:ss");
                    addUnit.Total_Time = Convert.ToDateTime(dr["Total Time"]).ToString("HH:mm:ss");
                    addUnit.Permissible_Load_MW = Convert.ToDecimal(dr[" Permissible Load (MW)"]);
                    addUnit.Gen_loss_kWh = Convert.ToDecimal(dr["Generation loss in KWH due to Load shedding"]);
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
                    }
                    else
                    {
                        status = "Failed to upload";
                    }
                }
            }
            return status;
        }
        private async Task<string> InsertWindDailyLoadShedding(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindDailyLoadShedding> addSet = new List<WindDailyLoadShedding>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyLoadShedding addUnit = new WindDailyLoadShedding();
                    addUnit.site = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //meta.importSiteId = addUnit.site_id;//C
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
                //var url = "http://localhost:23835/api/DGR/InsertWindDailyLoadShedding";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindDailyLoadShedding";
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


        private async Task<string> InsertWindDailyJMR(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindDailyJMR> addSet = new List<WindDailyJMR>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyJMR addUnit = new WindDailyJMR();
                    addUnit.site = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //meta.importSiteId = addUnit.site_id;
                    addUnit.date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.jmr_kwh = Convert.ToString(dr[" Permissible Load (MW)"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertDailyJMR";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyJMR";
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
        private async Task<string> InsertSolarDailyTargetKPI(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<SolarDailyTargetKPI> addSet = new List<SolarDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarDailyTargetKPI addUnit = new SolarDailyTargetKPI();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Sites = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Sites]);//C
                    //meta.importSiteId = addUnit.site_id;//C
                    addUnit.GHI = Convert.ToDecimal(dr[3]);
                    addUnit.POA = Convert.ToDecimal(dr[4]);
                    addUnit.kWh = Convert.ToDecimal(dr[5]);
                    addUnit.MA = Convert.ToDecimal(dr["MA (%)"]);
                    addUnit.IGA = Convert.ToDecimal(dr["IGA (%)"]);
                    addUnit.EGA = Convert.ToDecimal(dr["EGA (%)"]);
                    addUnit.PR = Convert.ToDecimal(dr["PR (%)"]);
                    addUnit.PLF = Convert.ToDecimal(dr["PLF (%)"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertSolarDailyTargetKPI";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarDailyTargetKPI";
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
        private async Task<string> InsertWindDailyTargetKPI(string status, DataSet ds)
        {
            if (ds.Tables.Count > 0)
            {
                List<WindDailyTargetKPI> addSet = new List<WindDailyTargetKPI>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindDailyTargetKPI addUnit = new WindDailyTargetKPI();
                    addUnit.FY = Convert.ToString(dr["FY"]);
                    addUnit.Date = Convert.ToDateTime(dr["Date"]).ToString("yyyy-MM-dd");
                    addUnit.Site = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.Site]);//C
                    //meta.importSiteId = addUnit.site_id;//C
                    addUnit.WindSpeed = Convert.ToDecimal(dr["WindSpeed"]);
                    addUnit.kWh = Convert.ToDecimal(dr["kWh"]);
                    addUnit.MA = Convert.ToString(dr["MA"]);
                    addUnit.IGA = Convert.ToString(dr["IGA"]);
                    addUnit.EGA = Convert.ToString(dr["EGA"]);
                    addUnit.PLF = Convert.ToString(dr["PLF"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertDailyTargetKPI";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertDailyTargetKPI";
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
        private async Task<string> InsertSolarSiteMaster(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<SolarSiteMaster> addSet = new List<SolarSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarSiteMaster addUnit = new SolarSiteMaster();
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    meta.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    addUnit.spv = Convert.ToString(dr["SPV"]);
                    addUnit.state = Convert.ToString(dr["State"]);
                    addUnit.dc_capacity = Convert.ToDouble(dr["DC Capacity (MWp)"]);
                    addUnit.ac_capacity = Convert.ToDouble(dr["AC Capacity (MW)"]);
                    addUnit.total_tarrif = Convert.ToDouble(dr["Total Tariff"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertSolarSiteMaster";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarSiteMaster";
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
        private async Task<string> InsertWindSiteMaster(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindSiteMaster> addSet = new List<WindSiteMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindSiteMaster addUnit = new WindSiteMaster();
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    meta.importSiteId = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    addUnit.spv = Convert.ToString(dr["SPV"]);
                    addUnit.state = Convert.ToString(dr["State"]);
                    addUnit.model = Convert.ToString(dr["Model"]);
                    addUnit.capacity_mw = Convert.ToDouble(dr["Capacity(MW)"]);
                    addUnit.wtg = Convert.ToDouble(dr["WTG"]);
                    addUnit.total_mw = Convert.ToDouble(dr["Total_MW"]);
                    addUnit.tarrif = Convert.ToDouble(dr["Tariff"]);
                    addUnit.gbi = Convert.ToDouble(dr["GBI"]);
                    addUnit.total_tarrif = Convert.ToDouble(dr["Total_Tariff"]);
                    addUnit.ll_compensation = Convert.ToDouble(dr["LL_Compensation"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertWindSiteMaster";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindSiteMaster";
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
        private async Task<string> InsertSolarLocationMaster(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<SolarLocationMaster> addSet = new List<SolarLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarLocationMaster addUnit = new SolarLocationMaster();
                    addUnit.country = Convert.ToString(dr["Country"]);
                    addUnit.site = Convert.ToString(dr["Site"]);
                    //addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    //meta.importSiteId = addUnit.site_id;//C
                    addUnit.eg = Convert.ToString(dr["EG"]);
                    addUnit.ig = Convert.ToString(dr["IG"]);
                    addUnit.icr_inv = Convert.ToString(dr["ICR/INV"]);
                    addUnit.icr = Convert.ToString(dr["ICR"]);
                    addUnit.inv = Convert.ToString(dr["INV"]);
                    addUnit.smb = Convert.ToString(dr["SMB"]);
                    addUnit.strings = Convert.ToString(dr["String"]);
                    addUnit.string_configuration = Convert.ToString(dr["String Configuration"]);
                    addUnit.total_string_current = Convert.ToDouble(dr["Total String Current (amp)"]);
                    addUnit.total_string_voltage = Convert.ToDouble(dr["Total String voltage"]);
                    addUnit.modules_quantity = Convert.ToDouble(dr[12]);
                    addUnit.wp = Convert.ToDouble(dr["Wp"]);
                    addUnit.capacity = Convert.ToDouble(dr["Capacity (KWp)"]);
                    addUnit.module_make = Convert.ToString(dr["Module Make"]);
                    addUnit.module_model_no = Convert.ToString(dr[16]);
                    addUnit.module_type = Convert.ToString(dr["Module Type"]);
                    addUnit.string_inv_central_inv = Convert.ToString(dr["String Inv / Central Inv"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertSolarLocationMaster";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertSolarLocationMaster";
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
        private async Task<string> InsertWindLocationMaster(string status, DataSet ds)
        {//siteID recorded
            if (ds.Tables.Count > 0)
            {
                List<WindLocationMaster> addSet = new List<WindLocationMaster>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    WindLocationMaster addUnit = new WindLocationMaster();
                    addUnit.site_master_id = Convert.ToInt32(siteNameId[addUnit.site]);//C
                    meta.importSiteId = addUnit.site_master_id;
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.wtg = Convert.ToString(dr["WTG"]);
                    addUnit.feeder = Convert.ToDouble(dr["Feeder"]);
                    addUnit.max_kwh_day = Convert.ToDouble(dr["Max. kWh/Day"]);
                    addSet.Add(addUnit);
                }
                var json = JsonConvert.SerializeObject(addSet);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                //var url = "http://localhost:23835/api/DGR/InsertWindLocationMaster";
                var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/InsertWindLocationMaster";
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
        private async Task<string> InsertSolarAcDcCapacity(string status, DataSet ds)
        {
            //Changed model datatype to reflect database
            //Updated API function
            //records site id
            //percentage value import fixed
            //importing successfully?
            //validating successfully?
            if (ds.Tables.Count > 0)
            {
                List<SolarInvAcDcCapacity> addSet = new List<SolarInvAcDcCapacity>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SolarInvAcDcCapacity addUnit = new SolarInvAcDcCapacity();
                    addUnit.site = Convert.ToString(dr["Site"]);
                    addUnit.site_id = Convert.ToInt32(siteNameId[addUnit.site]);
                    addUnit.inverter = Convert.ToString(dr["Inverter"]);
                    meta.importSiteId = addUnit.site_id;
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
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/importMetaData";
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(url, data);
            }
            string status = "Logged activity successfully";
            return status;
        }
        public void masterHashtable_WtgToWtgId()
        {
            //fills a hashtable with key = wtg and value = location_master_id from table : Wind Location Master
            //gets equipmentID from equipmentName in Wind Location Master
            DataTable dTable = new DataTable();
            //var url = "http://localhost:23835/api/DGR/GetWindLocationMaster";
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
                equipmentId.Add((string)dr["wtg"], Convert.ToInt32(dr["location_master_id"]));
            }
        }
        public void masterHashtable_BDNameToBDId()
        {
            //fills a hashtable with key = bdTypeName and value = bdTypeId from table : BDType 
            //gets breakdownId from breakdownName in BDType

            DataTable dTable = new DataTable();
            //var url = "http://localhost:23835/api/DGR/GetBDType";
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
                breakdownType.Add((string)dr["bd_type_name"], Convert.ToInt32(dr["bd_type_id"]));
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
                //url = "http://localhost:23835/api/DGR/GetSolarSiteMaster";
                url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetSolarSiteMaster";
            }
            else if (importData[0] == "Wind")
            {
                //url = "http://localhost:23835/api/DGR/GetWindSiteMaster";
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
                    siteNameId.Add((string)dr["site"], Convert.ToInt32(dr["site_master_solar_id"]));
                }
            }
            else if (importData[0] == "Wind")
            {
                foreach (DataRow dr in dTable.Rows)
                {
                    siteNameId.Add((string)dr["site"], Convert.ToInt32(dr["site_master_id"]));
                }
            }
        }

        public void masterHashtable_SiteIdToSiteName()
        {
            //fills a hashtable with as key = siteId and value = siteNameId from table : Wind Site Master
            //gets siteName from siteId in Wind Site Master

            siteName.Clear();
            DataTable dTable = new DataTable();
            //var url = "http://localhost:23835/api/DGR/GetWindSiteMaster";
            var url = _idapperRepo.GetAppSettingValue("API_URL") + "/api/DGR/GetWindSiteMaster";
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
                siteName.Add(Convert.ToInt32(dr["site_master_id"]), Convert.ToString(dr["site"]));
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
                eqSiteId.Add((string)dr["wtg"], Convert.ToInt32(dr["site_master_id"]));
            }
        }
        private async Task<bool> UploadFile(IFormFile ufile)
        {
            if (ufile != null && ufile.Length > 0)
            {
                var fileName = Path.GetFileName(ufile.FileName);
                var filePath = env.ContentRootPath + @"\ImportedFile\" + fileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ufile.CopyToAsync(fileStream);
                }
                return true;
            }
            return false;
        }
    }
}