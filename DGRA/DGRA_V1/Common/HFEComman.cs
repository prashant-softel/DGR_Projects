using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;


namespace DGRA_V1.Common
{
    public class HFEComman
    {
        ReadOnlyDictionary<int, string> dict = new ReadOnlyDictionary<int, string>(new Dictionary<int, string> { { 1, "one" }, { 2, "two" } });
    }
    public class ReadOnlyDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _dict;
        public ReadOnlyDictionary(Dictionary<TKey, TValue> dict)
        {
            _dict = dict;
        }

        public TValue this[TKey key] { get { return _dict[key]; } }
    }
    /* public const int TheAnswerToLife = 42;
     public const int TheFakeAnswerToLife = 43;
     public const string Uploading_File_Generation = "Uploading_File_Generation$";
     public static readonly int[] All = {
       TheAnswerToLife,
       TheFakeAnswerToLife
      };

     //internal string name;
     //internal int value;


     private static readonly List<SelectListItem> items;

     public static SelectList SelectListItems
     {
         get { return new SelectList(items, "Value", "Text"); }
     }

     static HFEComman()
     {
         items = All.Select(s => new SelectListItem { Value = s.ToString(), Text = s.ToString() }).ToList();
     }*/
    /* enum ViewsPages_Type
{
eWindGenView = 1,
eWindDailyTargetKpi = 2,
eWindDailyLoadShedding = 3,
eWindMonthlyTargetKpi = 4,
eWindMothlyLineLoss = 5,
eWindMonthlyJMR = 6
}
}
enum Report_Type
{
eWindSiteMaster = 1,
eWindLoactionMaster = 2,
eWindDailyLoadShedding = 3,
eWindGenReport = 4,
eWindBDReport = 5,
eWindPRReport = 6

}

private static readonly List<SelectListItem> items;

public static SelectList SelectListItems
{
get { return new SelectList(items, "Value", "Text"); }
}*/

//}
}