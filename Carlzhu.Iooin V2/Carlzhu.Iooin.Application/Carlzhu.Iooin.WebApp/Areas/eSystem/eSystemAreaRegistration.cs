using System.Web.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem
{
    public class eSystemAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "eSystem";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "eSystem_default",
                "eSystem/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
