using System;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.Initialization;
using Carlzhu.Iooin.Business.SysModule;
using Carlzhu.Iooin.Util;

using Controller = System.Web.Mvc.Controller;

namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    [LoginAuthorize]
    public class FormControllerBase:Controller//<TBEmtity> : PublicController<TBEmtity>
    {

        public string EmpNo => ManageProvider.Provider.Current().UserId;

        /// <summary>
        /// 服务端回馈给客户端的提示信息
        /// </summary>
        /// <param name="alertMessage">提示信息</param>
        /// <param name="redirectUrl">重定向地址，默认为当前页面</param>
        /// <returns></returns>
        public static ContentResult ResponseToClient(string alertMessage= "对不起，权限不足！！！", string redirectUrl = "/")
        {
            ContentResult cr = new ContentResult();
            string content = "<script type='text/javascript'>alert('" + alertMessage + "');{0}</script>";
            if ("/".Equals(redirectUrl))
            {
                content = String.Format(content, "history.go(-1);");
            }
            else
            {
                content = String.Format(content, "window.location.href='" + redirectUrl + "'");
            }
            cr.Content = content;
            return cr;
        }
    }
}