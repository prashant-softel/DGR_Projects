using DGRAPIs.BS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IDGRBS _dgrBs;
        public UserController(IDGRBS dgr)
        {
            _dgrBs = dgr;
        }
    }
}
