namespace DGRA_V1.Models
{
    public class SolarInvAcDcCapacity
    {
        public string site { get; set; }
        public int site_id { get; set; }
        public string inverter { get; set; }
        public dynamic dc_capacity { get; set; }
        public dynamic ac_capacity { get; set; }
    }
}
