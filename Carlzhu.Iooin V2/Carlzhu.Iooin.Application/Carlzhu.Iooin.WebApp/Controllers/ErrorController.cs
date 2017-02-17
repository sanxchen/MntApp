using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.InteractiveAdapter;


namespace Carlzhu.Iooin.WebApp.Controllers
{
    public class ErrorController : Controller
    {
        /// <summary>
        /// 错误页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string message)
        {
            Dictionary<string, string> modulesError = (Dictionary<string, string>)HttpContext.Application["error"];
            ViewData["Message"] = modulesError;
            return View();
        }
        /// <summary>
        /// 错误页面404
        /// </summary>
        /// <returns></returns>
        public ActionResult Error404()
        {
            return View();
        }
        /// <summary>
        /// 建议升级浏览器软件
        /// </summary>
        /// <returns></returns>
        public ActionResult Browser()
        {
            return View();
        }



        public ActionResult Default(string p)
        {
            string err = null, source = null, url = null;
            try
            {
                var errMsg = p.Decrypt().Split(',');
                err = errMsg[0];
                source = errMsg[1];
                url = errMsg[2];

            }
            catch (Exception)
            {
                err = "";
                source = "";
                url = "";
            }
            finally
            {
                ViewData["Err"] = err;
                ViewData["Source"] = source;
                ViewData["Url"] = url;

            }
            return View();
        }
    }
}