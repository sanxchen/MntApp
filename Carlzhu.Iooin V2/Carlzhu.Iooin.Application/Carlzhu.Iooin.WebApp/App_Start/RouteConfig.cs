using System.Web.Mvc;
using System.Web.Routing;

namespace Carlzhu.Iooin.WebApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("{*glyphicons}", new { favicon = @"(.*/)?glyphicons(/.*)?" });


            //Account
            routes.MapRoute(
                        name: "Account",
                        url: "Account/{action}/{id}",
                        defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
                        );
            ////FORM
            //routes.MapRoute(
            // name: "FORM",
            // url: "FORM/{controller}/{action}/{id}",
            //defaults: new { controller = "Applying", action = "Welcome", id = UrlParameter.Optional }
            // );

            ////Quality
            //routes.MapRoute(
            //             name: "Quality",
            //             url: "Quality/{controller}/{action}/{id}",
            //             defaults: new { controller = "Calibration", action = "Index", id = UrlParameter.Optional }
            //             );




            routes.MapRoute(
                        name: "Default",
                        url: "{controller}/{action}/{id}",
                        defaults: new { controller = "Default", action = "Index", id = UrlParameter.Optional }
                        );
        }
    }
}