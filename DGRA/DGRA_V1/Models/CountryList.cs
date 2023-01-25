using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class CountryList
    {
     
        public string country { get; set; }
       
        public List<CountryList> list = new List<CountryList>();
    }
}
