using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DGRAPIs.Models
{
    public class MailRequest
    {
        public List <string> ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<IFormFile> Attachments { get; set; }
        public List<string> CcEmail { get; set; }
        //public IFormFile Attachments { get; set; }
    }
}
