namespace DGRA_V1.Models
{
    public class SolarMonthlyUploadingLineLosses
    {
        public string FY { get; set; }
        public string Sites { get; set; }
        public int Site_Id { get; set; }
        public string Month { get; set; }
        public dynamic LineLoss { get; set; }
    }
}
