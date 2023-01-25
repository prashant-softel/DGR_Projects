using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace DGRA_V1.Common
{
    public class FileUploadType
    {
        public const string Solar = "Solar";
        public const string Wind = "Wind";

       
        public static readonly string[] All = {
            Solar,
            Wind
        };


        private static readonly List<SelectListItem> items;

        public static SelectList SelectListItems
        {
            get { return new SelectList(items, "Value", "Text"); }
        }

        static FileUploadType()
        {
            items = All.Select(s => new SelectListItem { Value = s, Text = s }).ToList();
        }
    }
}