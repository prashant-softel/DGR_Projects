using DGRAPIs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DGRAPIs.BS.Interface
{
   public interface IDGRUserBS
    {
        public Task<APIResponse> UserRegister(UserManagement userModel);
    }
}
