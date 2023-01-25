namespace DGRA_V1.Models
{
    public class WindSiteMaster
    {
        public int site_master_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public string spv { get; set; }
        public string state { get; set; }
        public string model { get; set; }
        public double capacity_mw { get; set; }
        public double wtg { get; set; }
        public double total_mw { get; set; }
        public double tarrif { get; set; }
        public double gbi { get; set; }
        public double total_tarrif { get; set; }
        public double ll_compensation { get; set; }
    }
}
