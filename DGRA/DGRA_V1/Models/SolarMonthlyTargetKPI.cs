namespace DGRA_V1.Models
{
    public class SolarMonthlyTargetKPI
    {
        public string FY { get; set; }
        public string Month { get; set; }
        public int month_no { get; set; }
        public int year { get; set; }
        public string Sites { get; set; }
        public int Site_Id { get; set; }
        public dynamic GHI { get; set; }
        public dynamic POA { get; set; }
        public dynamic kWh { get; set; }
        public dynamic MA { get; set; }
        public dynamic IGA { get; set; }
        public dynamic EGA { get; set; }
        public dynamic PR { get; set; }
        public dynamic PLF { get; set; }
    }
}
