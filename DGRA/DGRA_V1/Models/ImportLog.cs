
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Models
{
    public class ImportLog
    {
        public string importFilePath { get; set; }
        public int importSiteId { get; set; }
        public string importType { get; set; }
        public string importLogName { get; set; }
    }
}
