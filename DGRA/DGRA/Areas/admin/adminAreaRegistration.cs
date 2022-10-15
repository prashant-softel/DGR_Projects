using System.Web.Mvc;

namespace DGRA.Areas.admin
{
    public class adminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(null, "fileupload/upload", new { action = "Upload", controller = "FileUpload" });
           // context.MapRoute(null, "reportviews/windgenview", new { action = "WindGenView", controller = "ReportViews" });
        }
    }
}