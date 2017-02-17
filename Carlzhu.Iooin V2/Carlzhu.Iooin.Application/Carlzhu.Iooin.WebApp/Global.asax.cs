using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Carlzhu.Iooin.Business.Initialization;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        // Utilities.Base.DebugLog.LogHelper err = Utilities.Base.DebugLog.LogFactory.GetLogger(typeof(MvcApplication));


        protected void Application_Start()
        {

            //设置当前数据库类型
            DbHelper.DbType = DatabaseType.SqlServer;
            DbHelper.DataBaseName = "MINICUT_OA";
            //var ws = new WebSocket("ws://127.0.0.1:4141");
            Application["OnLineCount"] = 0;//在应用程序第一次启动时初始化在线人数为0

            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //自定义注册错误
            RegisterGlobalFilters(GlobalFilters.Filters);

        }




        /// <summary>
        /// 离开应用程序启动这件事会发生
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnLineCount"] = (int)Application["OnLineCount"] - 1;
            Application.UnLock();
        }
        /// <summary>
        /// 异常处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = this.Context.Server.GetLastError();

            while (true)
            {
                if (ex.InnerException == null) break;
                ex = ex.InnerException;
            }


            if (ex != null)
            {
                //  err.Error($"[{ManageProvider.Provider.Current().UserId}]-{ex.Message}");

                //登录是否过期
                if (!ManageProvider.Provider.IsOverdue())
                {
                    HttpContext.Current.Response.Redirect("~/Login/Default");
                }
                Dictionary<string, string> modulesError = new Dictionary<string, string>
                {
                    {"发生时间", DateTime.Now.ToString(CultureInfo.InvariantCulture)},
                    {"错误描述", ex.Message.Replace("\r\n", "")},
                    {"错误对象", ex.Source},
                    {"错误页面", "" + HttpContext.Current.Request.Url + ""},
                    {"浏览器IE", HttpContext.Current.Request.UserAgent},
                    {"服务器IP", Carlzhu.Iooin.Util.Net.GetIPAddress()}
                };
                Application["error"] = modulesError;
                //HttpContext.Current.Response.Redirect("~/Error/Index");
            }
        }


        void Application_End(object sender, EventArgs e) { }

        void Session_Start(object sender, EventArgs e) { }



        /// <summary>
        /// 加载错误
        /// </summary>
        /// <param name="filters"></param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CustomExceptionAttribute(), 1);//自定义的验证特性
            filters.Add(new HandleErrorAttribute(), 2);
        }



    }
}