using System.Web.Optimization;

namespace Carlzhu.Iooin.WebApp
{
    public class BundleConfig
    {
        // 有关 Bundling 的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            BundleTable.EnableOptimizations = true;


            bundles.Add(
                 new ScriptBundle("~/carlzhu/defaultScripts/")
                 .Include("~/Scripts/jquery-1.8.2.js")
                 .Include("~/Scripts/jquery-ui-1.8.24.js")
                 
                 .Include("~/Scripts/jquery.unobtrusive-ajax.js")
                 .Include("~/Scripts/jquery.validate.js")
                 .Include("~/Scripts/jquery.validate.unobtrusive.js")
                 .Include("~/Scripts/knockout-2.2.0.debug.js")
                 .Include("~/Scripts/modernizr-2.6.2.js")
                 .Include("~/Scripts/bootstrap.min.js")

                 //.Include("~/Scripts/angular-1.0.1.js")
                 //.Include("~/Scripts/bootstrap-treeview.js")
                 //.Include("~/Scripts/jquery.bootstrap.js")

                 .Include("~/Scripts/jquery.validate.mvc.min.js")
                 
                 .Include("~/Scripts/mvcpager.min.js")
                 .Include("~/Scripts/fullscreen.js")
                 .Include("~/Scripts/messenger.js")
                 .Include("~/Scripts/timerpicker.js")
                 .Include("~/Scripts/jquery.pop.js")
                 .Include("~/Scripts/jquery.tagsinput.js")
       

                 .Include("~/Scripts/jquery.flot.min.js")
                 .Include("~/Scripts/jquery.flot.pie.min.js")
                 .Include("~/Scripts/jquery.flot.resize.min.js")

                 .Include("~/Scripts/jquery.cookie.js")
                 .Include("~/Scripts/jquery.messager.js")
                 .Include("~/Scripts/jquery.alerts.js")
                 .Include("~/Scripts/carlzhu.common.js")
                 .Include("~/Scripts/Websockets.js")
     );




            bundles.Add(new StyleBundle("~/carlzhu/defaultStyles/")
                .Include("~/Content/bootstrap.min.css")
                .Include("~/Content/jquery-ui.min.css")
                .Include("~/Content/messenger.css")
                .Include("~/Content/jquery.tagsinput.css")
                .Include("~/Content/messenger-theme-future.css")
                .Include("~/Content/icon.css")
                .Include("~/Content/glyphicon.css")
                .Include("~/Content/jquery.alerts.css")
                .Include("~/Content/jquery.validate.mvc.css")
                .Include("~/Content/Site.css")
                .Include("~/Content/Layout.css")
                );









        }
    }
}