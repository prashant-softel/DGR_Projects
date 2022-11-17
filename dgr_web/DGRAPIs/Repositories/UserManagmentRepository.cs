using DGRAPIs.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DGRAPIs.Models;
namespace DGRAPIs.Repositories
{
    public class UserManagmentRepository : GenericRepository
    {
        private int approve_status = 0;
        public UserManagmentRepository(MYSQLDBHelper sqlDBHelper) : base(sqlDBHelper)
        {
            //query = "insert into import_batches (file_name, import_type, log_filename,site_id,import_date,imported_by,import_by_name) values ('" + meta.importFilePath + "','" + meta.importType + "','" + meta.importLogName + "','" + site_id + "',NOW(),'" + import_by + "','" + import_by_name + "');";
            //return await Context.ExecuteNonQry<int>(query).ConfigureAwait(false);




        }
        internal async Task<APIResponse> UserRegister(UserManagement management)
        {
            string query = "";
           
            int site_id = 190;
            int import_by = 2;
            string import_by_name = "Demo User";
                
                query = "insert into import_batches (Title, import_type, log_filename,site_id,import_date,imported_by,import_by_name) values ('" + management.Title + "','" + management.Name + "','" + management.user_mail_id + "','" + site_id + "',NOW(),'" + import_by + "','" + import_by_name + "');";
                return await Context.ExecuteAPIQry<APIResponse>(query).ConfigureAwait(false);
            

            //query = "insert into import_log (file_name, import_type, log_filename) values ('" + meta.importFilePath + "','" + meta.importType + "','" + meta.importLogName + "');";
           
        }
    }
}
