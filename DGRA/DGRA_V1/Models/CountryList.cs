using System.Collections.Generic;

namespace DGRA_V1.Models
{
    public class CountryList
    {

        public string country { get; set; }

        public List<CountryList> list = new List<CountryList>();
    }
}
