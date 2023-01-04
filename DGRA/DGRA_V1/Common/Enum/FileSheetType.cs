using System.Collections.Generic;


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

        //private static readonly List<SelectListItem> items;

        //public static SelectList SelectListItems
        //{
        //    get { return new SelectList(items, "Value", "Text"); }
        //}

        //public static readonly string[] All = {
        //  Uploading_File_Generation,
        //  Uploading_File_Breakdown,
        //  Uploading_PyranoMeter1Min,
        //  Uploading_PyranoMeter15Min,
        //  Monthly_JMR_Input_and_Output,
        //  Daily_Load_Shedding,
        //  Monthly_LineLoss,
        //  Monthly_Target_KPI,
        //  Daily_JMR_Input_and_Output
        //};
        //static FileSheetType()
        //{
        //    items = All.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
        //}
    }
}