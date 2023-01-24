﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Models
{
    public class ImportBatch
    {
        public string importFilePath { get; set; }
        public int importSiteId { get; set; }
        public string importType { get; set; }
        public string importLogName { get; set; }
    }

    public class BatchIdImport
    { 
        public int import_batch_id{ get; set; }
    }

    public class ImportBatchStatus
    {
        public int import_batch_id { get; set; }
        public int is_approved { get; set; }
    }

}
