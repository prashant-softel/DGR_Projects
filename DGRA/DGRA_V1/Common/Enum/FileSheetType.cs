using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DGRA_V1.Common
{
    public class FileSheetType
    {
        
        public const string Uploading_File_Generation = "Uploading_File_Generation$";
        public const string Uploading_File_Breakdown = "Uploading_File_Breakdown$";
        public const string Uploading_PyranoMeter1Min = "Uploading_PyranoMeter1Min$";
        public const string Uploading_PyranoMeter15Min = "Uploading_PyranoMeter15Min$";
        public const string Monthly_JMR_Input_and_Output = "Monthly_JMR_Input_and_Output$";
        public const string Wind_Monthly_LineLoss = "Wind_Monthly_LineLoss$";
        public const string Wind_Monthly_Target_KPI = "Wind_Monthly_Target_KPI$";
        public const string Daily_Load_Shedding = "Load_Shedding_Uploading_Format$";
        public const string Daily_JMR_Input_and_Output = "Daily_JMR_Input_and_Output$";

        public static readonly string[] All = {
          Uploading_File_Generation,
          Uploading_File_Breakdown,
          Uploading_PyranoMeter1Min,
          Uploading_PyranoMeter15Min,
          Monthly_JMR_Input_and_Output,
          Daily_Load_Shedding,
          Wind_Monthly_LineLoss,
          Wind_Monthly_Target_KPI,
          Daily_JMR_Input_and_Output


        };


        private static readonly List<SelectListItem> items;

        public static SelectList SelectListItems
        {
            get { return new SelectList(items, "Value", "Text"); }
        }

        static FileSheetType()
        {
            items = All.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
        }
    }
}