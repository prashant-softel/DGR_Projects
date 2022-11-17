using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DGRA.Common
{
    public class FileSheetType
    {
        public const string Uploading_File_Generation = "Uploading_File_Generation$";
        public const string Uploading_File_Breakdown = "Uploading_File_Breakdown$";
        public const string Uploading_PyranoMeter1Min = "Uploading_PyranoMeter1Min$";
        public const string Uploading_PyranoMeter15Min = "Uploading_PyranoMeter15Min$";
        public const string Monthly_JMR_Input_and_Output = "Monthly_JMR_Input_and_Output$";
        public const string Daily_Load_Shedding = "Load_Shedding_Uploading_Format$";


        public static readonly string[] All = {
          Uploading_File_Generation,
          Uploading_File_Breakdown,
          Uploading_PyranoMeter1Min,
          Uploading_PyranoMeter15Min,
          Monthly_JMR_Input_and_Output,
          Daily_Load_Shedding
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