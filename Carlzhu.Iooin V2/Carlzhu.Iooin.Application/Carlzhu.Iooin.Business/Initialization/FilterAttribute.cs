using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Carlzhu.Iooin.Business.SysModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Business.Initialization
{
    /// <summary>
    /// 权限检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class AuthorizationAttribute : ActionFilterAttribute
    {

        // OnActionExecuted 在执行操作方法后由 ASP.NET MVC 框架调用。
        // OnActionExecuting 在执行操作方法之前由 ASP.NET MVC 框架调用。
        // OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        // OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。

        /// <summary>
        /// 在执行操作方法之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string empNo = ManageProvider.Provider.Current().UserId;

            string systemName = filterContext.HttpContext.Request.Url?.Segments[1].Replace('/', ' ').Trim() ?? "undefine";

            if (new SystemAccountBll().UserValidate(
                filterContext.RouteData.Values["controller"].ToString(),
                filterContext.RouteData.Values["action"].ToString(),
                empNo, systemName))
            {
                base.OnActionExecuting(filterContext);
            }
            else
            {
                if (HttpContext.Current.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
                {
                    Link.ErrorBy(new Exception("对不起，此操作需要访问权限..."), this.GetType());
                }
                else
                {
                    filterContext.Result = new ContentResult { Content = "抱歉,你不具有当前操作的权限！" };
                }
            }

        }

        /// <summary>
        /// 在执行操作方法后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {

            base.OnActionExecuted(filterContext);
        }

        /// <summary>
        ///  OnResultExecuted 在执行操作结果后由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            base.OnResultExecuted(filterContext);
        }
        /// <summary>
        /// OnResultExecuting 在执行操作结果之前由 ASP.NET MVC 框架调用。
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            base.OnResultExecuting(filterContext);
        }


    }




    /// <summary>
    /// 登陆检查
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class LoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!string.IsNullOrEmpty(ManageProvider.Provider.Current().UserId))
                base.OnActionExecuting(filterContext);
            else
            {
                if (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new ContentResult { Content = "抱歉,此操作需要登陆后才能进行！" };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(new { controller = "Account", action = "Login", url = HttpContext.Current.Request.Url }));
                }
            }

        }
    }


    /// <summary>
    /// 系统异常处理
    /// </summary>
    public sealed class CustomExceptionAttribute : HandleErrorAttribute, IExceptionFilter   //HandleErrorAttribute
    {


        public new void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);

            //var emp = ManageProvider.Provider.Current().UserId;
            if (filterContext.ExceptionHandled)
            {
                var httpExce = filterContext.Exception as HttpException;
                if (httpExce != null && httpExce.GetHttpCode() != 500)
                //为什么要特别强调500 因为MVC处理HttpException的时候，如果为500 则会自动
                //将其ExceptionHandled设置为true，那么我们就无法捕获异常
                {
                    return;
                }
            }

            filterContext.Controller.ViewBag.UrlRefer = filterContext.HttpContext.Request.UrlReferrer;

            Exception httpException = filterContext.Exception;
            string exceptionMessage = null;
            if (httpException != null)
            {
                //数据库异常特珠处理
                DbEntityValidationException ex = httpException as DbEntityValidationException;
                if (ex != null)
                {
                    var entityError = ex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage);
                    exceptionMessage = string.Concat(ex.Message, "errors are: ", string.Join("; ", entityError));
                }


                if (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    filterContext.Result = new ContentResult { Content = exceptionMessage ?? httpException.Message };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary(
                        new { controller = "Error", action = "Default" })
                    {
                        { "p",
                            $"{exceptionMessage ?? httpException.Message},{httpException.Source},{filterContext.HttpContext.Request.UrlReferrer}"
                                .Encrypt()}
                    });
                }

            }

            // Pass exception details to the target error View.  




            filterContext.ExceptionHandled = true;//设置异常已经处理
        }
    }
}
