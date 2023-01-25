using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class MonthList
    {
        internal string name;
        internal string value;
       
        private List<MonthList> _list = new List<MonthList>()
        {
            new MonthList{ name="1",value="1"},
            new MonthList{ name="2",value="1"},
            new MonthList{ name="13",value="1"},
        };

       
        public List<MonthList> list { get { return _list; } }


    }



   /* public static class Permission1
    {

        private static List<MonthList> _list = new List<MonthList>()
        {
            new MonthList{ name="1",value="1"},
            new MonthList{ name="2",value="1"},
            new MonthList{ name="13",value="1"},
        };


        public static List<MonthList> PermissionList { get { return _list; } }


    }*/

}
