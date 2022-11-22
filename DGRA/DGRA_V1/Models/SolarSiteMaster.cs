namespace DGRA_V1.Models
{
    public class SolarSiteMaster
    {
        public int site_master_solar_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public string spv { get; set; }
        public string state { get; set; }
        public double dc_capacity { get; set; }
        public double ac_capacity { get; set; }
        public double total_tarrif { get; set; }

    }
}
