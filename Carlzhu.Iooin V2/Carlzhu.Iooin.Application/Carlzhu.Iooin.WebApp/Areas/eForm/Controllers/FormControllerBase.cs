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
        /// ����˻������ͻ��˵���ʾ��Ϣ
        /// </summary>
        /// <param name="alertMessage">��ʾ��Ϣ</param>
        /// <param name="redirectUrl">�ض����ַ��Ĭ��Ϊ��ǰҳ��</param>
        /// <returns></returns>
        public static ContentResult ResponseToClient(string alertMessage= "�Բ���Ȩ�޲��㣡����", string redirectUrl = "/")
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