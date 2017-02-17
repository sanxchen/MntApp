using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp
{
    /// <summary>
    /// 权限验证,加强安全验证防止未授权匿名不合法的请求
    
    ///
    ///
    
    /// </summary>
    public class ManagerPermissionAttribute : AuthorizeAttribute
    {
        private readonly PermissionMode _customMode;
        /// <summary>默认构造</summary>
        /// <param name="Mode">权限认证模式</param>
        public ManagerPermissionAttribute(PermissionMode Mode)
        {
            _customMode = Mode;
        }
        /// <summary>权限认证</summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //登录权限认证
            if (!ManageProvider.Provider.IsOverdue())
            {
                filterContext.Result = new RedirectResult("~/Login/Default");
            }
            //防止被搜索引擎爬虫、网页采集器
            if (!this.PreventCreeper())
            {
                filterContext.Result = new RedirectResult("~/Login/Default");
            }
            //权限拦截是否忽略
            if (_customMode == PermissionMode.Ignore)
            {
                return;
            }
            //执行权限认证
            if (!this.ActionAuthorize(filterContext))
            {
                ContentResult content = new ContentResult
                {
                    Content = "<script type='text/javascript'>top.Loading(false);alert('很抱歉！您的权限不足，访问被拒绝！');</script>"
                };
                filterContext.Result = content;
            }
        }
        /// <summary>
        /// 执行权限认证
        /// </summary>
        /// <returns></returns>
        private bool ActionAuthorize(AuthorizationContext filterContext)
        {
            if (ManageProvider.Provider.Current().IsSystem)
                return true;
            var areaName = filterContext.RouteData.DataTokens["area"] + "/";            //获取当前区域
            var controllerName = filterContext.RouteData.Values["controller"] + "/";    //获取控制器
            var action = filterContext.RouteData.Values["Action"];                      //获取当前Action
            string requestPath = "/" + areaName + controllerName + action;              //拼接构造完整url
            string moduleId = DESEncrypt.Decrypt(WebHelper.GetCookie("ModuleId"));
            bool result = BaseModulePermissionBll.Instance.ActionAuthorize(requestPath.ToLower(), ManageProvider.Provider.Current().ObjectId, moduleId, ManageProvider.Provider.Current().UserId);
            return result;
        }
        /// <summary>
        /// 防止被搜索引擎爬虫、网页采集器
        /// </summary>
        /// <returns></returns>
        private bool PreventCreeper()
        {
            return true;
        }
    }
}