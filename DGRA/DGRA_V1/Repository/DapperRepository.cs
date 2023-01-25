using DGRA_V1.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Repository
{
    public class DapperRepository : IDapperRepository
    {
        private readonly IConfiguration _config;
       
        public DapperRepository(IConfiguration config)
        {
            _config = config;
           
        }

       
        public string GetAppSettingValue(string key)
        {
            return _config.GetSection(key).Value;
        }

    }
}
