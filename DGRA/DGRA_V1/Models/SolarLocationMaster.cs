namespace DGRA_V1.Models
{
    public class SolarLocationMaster
    {
        public int location_master_solar_id { get; set; }
        public string country { get; set; }
        public string site { get; set; }
        public int site_id { get; set; }
        public string eg { get; set; }
        public string ig { get; set; }
        public string icr_inv { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string string_configuration { get; set; }
        public double total_string_current { get; set; }
        public double total_string_voltage { get; set; }
        public double modules_quantity { get; set; }
        public double wp { get; set; }
        public double capacity { get; set; }
        public string module_make { get; set; }
        public string module_model_no { get; set; }
        public string module_type { get; set; }
        public int string_inv_central_inv { get; set; }
    }
}
