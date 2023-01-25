namespace DGRA_V1.Models
{
    public class WindLocationMaster
    {
        public int location_master_id { get; set; }
        public int site_master_id { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string wtg { get; set; }
        public double feeder { get; set; }
        public double max_kwh_day { get; set; }

    }
}
