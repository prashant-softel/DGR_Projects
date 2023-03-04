using DGRAPIs.BS;
using DGRAPIs.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DGRAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailController : ControllerBase
    {
        private readonly MailServiceBS mailService;
        public MailController(MailServiceBS mailService)
        {
            this.mailService = mailService;
        }
        [HttpGet("send")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
               
                List<string> AddTo = new List<string>();
                MailRequest request = new MailRequest();
                AddTo.Add("sujitkumar0304@gmail.com");
                AddTo.Add("prashant@softeltech.in");
                //request.ToEmail = "sujitkumar0304@gmail.com";
                request.Subject = "test";
                request.Body = "hello";
                var data = await mailService.SendEmailAsync(request);
                //var data = await MailService.SendEmailAsync(request);
                // var data = "test";
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
