using DGRA_V1.Repository;
using DGRA_V1.Repository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Common
{
    public static class ConfigSetting
    {
        public static void ApplicationConfig(IServiceCollection services, IConfiguration Configuration)
        {
            services.AddScoped<IDapperRepository, DapperRepository>();
          
          

        }
    }
}
