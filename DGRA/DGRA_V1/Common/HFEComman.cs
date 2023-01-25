using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;


namespace DGRA_V1.Common
{
    public  class HFEComman
    {
        public  string name { set; get; }
        public int value { set; get; }
    }
    public static class PageList
    {
        public static List<HFEComman> _list { get; } = new List<HFEComman>()
        {
            new HFEComman{ name ="Generation View Summery",value=1},
            new HFEComman{ name ="Daily Target KPI",value=2},
            new HFEComman{ name ="Daily Loadshedding",value=3},
            new HFEComman{ name ="Mothly Target KPI",value=4},
            new HFEComman{ name ="Monthly Lineloss",value=5},
            new HFEComman{ name ="Monthly JMR",value=6},
           // new HFEComman{ name="2",value="1"},
           // new HFEComman{ name="13",value="1"},
        };
        public static List<HFEComman> PermissionList { get { return _list; } }

    }
}