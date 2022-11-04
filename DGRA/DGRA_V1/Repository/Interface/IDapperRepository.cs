using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DGRA_V1.Repository.Interface
{
   public interface IDapperRepository
    {
       
        string GetAppSettingValue(string AppSetting_Key);
    }
}
