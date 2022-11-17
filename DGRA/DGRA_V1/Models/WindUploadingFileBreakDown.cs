
namespace DGRA_V1.Models
{
    public class WindUploadingFileBreakDown
    {
        public int uploading_file_breakdown_id { get; set; }
        public string date { get; set; }
        public string site_name { get; set; }
        public int site_id { get; set; }
        public string wtg { get; set; }
        public int wtg_id { get; set; }
        public string bd_type { get; set; }
        public int bd_type_id { get; set; }
        public string stop_from { get; set; }
        public string stop_to { get; set; }
        public string total_stop { get; set; }
        public string error_description { get; set; }
        public string action_taken { get; set; }
    }
}
