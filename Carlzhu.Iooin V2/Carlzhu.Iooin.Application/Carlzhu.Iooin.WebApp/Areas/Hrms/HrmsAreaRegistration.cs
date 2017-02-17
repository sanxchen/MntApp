using System.Web.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.Hrms
{
    public class HrmsAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Hrms";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Hrms_default",
                "Hrms/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
