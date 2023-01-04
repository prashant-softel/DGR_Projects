using System.Collections.Generic;

namespace DGRA_V1.Models
{
    public class CountryList
    {

        public string country { get; set; }
        public List<CountryList> List = new List<CountryList>();
        public List<string> countryList = new List<string>()
        { "India" };
    }
}   
