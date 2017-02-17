using System.Web.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.CodeMaticModule
{
    public class CodeMaticModuleAreaRegistration : AreaRegistration
    {
        public override string AreaName => "CodeMaticModule";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                this.AreaName + "_Default",
                this.AreaName + "/{controller}/{action}/{id}",
                new { area = this.AreaName, controller = "Home", action = "Index", id = UrlParameter.Optional },
                new string[] { "Carlzhu.Iooin.WebApp.Areas." + this.AreaName + ".Controllers" }
            );
        }
    }
}
