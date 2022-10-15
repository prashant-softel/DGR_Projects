﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class SolarUploadingFileBreakDown
    {
        public int uploading_file_breakdown_solar_id { get; set; }
        public string date { get; set; }
        public string site { get; set; }
        public string ext_int_bd { get; set; }
        public string icr { get; set; }
        public string inv { get; set; }
        public string smb { get; set; }
        public string strings { get; set; }
        public string from_bd { get; set; }
        public string to_bd { get; set; }
        public string bd_remarks { get; set; }
        public string bd_type { get; set; }
        public string action_taken { get; set; }
    }
}
