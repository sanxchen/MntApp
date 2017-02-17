using System.Web.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.eForm
{
    public class eFormAreaRegistration : AreaRegistration
    {
        public override string AreaName => "eForm";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "eForm_default",
                "eForm/{controller}/{action}/{id}",
                new { action = "So", id = UrlParameter.Optional }
            );
        }
    }
}
