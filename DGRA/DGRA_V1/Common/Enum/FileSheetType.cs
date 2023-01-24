using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DGRA_V1.Common
{
    public class FileSheetType
    {
        
        public const string Uploading_File_Generation = "Uploading_File_Generation";
        public const string Uploading_File_Breakdown = "Uploading_File_Breakdown";
        public const string Uploading_PyranoMeter1Min = "Uploading_PyranoMeter1Min";
        public const string Uploading_PyranoMeter15Min = "Uploading_PyranoMeter15Min";
        public const string Monthly_JMR_Input_and_Output = "Monthly_JMR_Input_and_Output";
        public const string Monthly_LineLoss = "Monthly_LineLoss";
        public const string Monthly_Target_KPI = "Monthly_Target_KPI";
        public const string Daily_Load_Shedding = "Load_Shedding_Uploading_Format";
        public const string Daily_JMR_Input_and_Output = "Daily_JMR_Input_and_Output";
        public const string Daily_Target_KPI = "Daily_Target_KPI";
        public const string Site_Master = "Site_Master";
        public const string Location_Master = "Location_Master";
        public const string Solar_AC_DC_Capacity = "Solar_AC_DC_Capacity";

        public static List<string> sheetList = new List<string>()
        {
            "Uploading_File_Generation", "Uploading_File_Breakdown", "Uploading_PyranoMeter1Min", "Uploading_PyranoMeter15Min", "Monthly_JMR_Input_and_Output", "Monthly_LineLoss", "Monthly_Target_KPI", "Load_Shedding_Uploading_Format", "Daily_JMR_Input_and_Output", "Daily_Target_KPI", "Site_Master", "Location_Master", "Solar_AC_DC_Capacity" 
        };

        public enum FileImportType
        {
            imporFileType_Invalid = 0,
            imporFileType_Automation = 1,
            imporFileType_Monthly_JMR_Input_and_Output,
            imporFileType_Monthly_LineLoss,
            imporFileType_Monthly_Target_KPI,
            imporFileType_Daily_Load_Shedding,
            imporFileType_Daily_JMR_Input_and_Output,
            imporFileType_Daily_Target_KPI,
            imporFileType_Site_Master,
            imporFileType_Location_Master,
            imporFileType_Solar_AC_DC_Capacity
        }

        enum uiPageType
        {
            page_type

        }
    }
}