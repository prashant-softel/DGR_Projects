using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class APIResponse
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public List<dynamic> Data { get; set; }

    }
}
 