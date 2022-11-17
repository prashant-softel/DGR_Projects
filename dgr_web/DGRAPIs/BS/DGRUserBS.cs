using DGRAPIs.BS.Interface;
using DGRAPIs.Helper;
using DGRAPIs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Repositories;
namespace DGRAPIs.BS
{
    public class DGRUserBS : IDGRUserBS
    {

        private readonly DatabaseProvider databaseProvider;
        private MYSQLDBHelper getDB => databaseProvider.SqlInstance();
        public DGRUserBS(DatabaseProvider dbProvider)
        {
            databaseProvider = dbProvider;
        }
        public async Task<APIResponse> UserRegister(UserManagement userModel)
        {
            try
            {
                using (var repos = new UserManagmentRepository(getDB))
                {
                    return await repos.UserRegister(userModel);

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
