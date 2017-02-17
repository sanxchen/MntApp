using System;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.HrmsModule;
using Carlzhu.Iooin.Business.SysModule;
using Carlzhu.Iooin.Entity.FORM;

namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    public class PersonalController : FormControllerBase
    {
        //
        // GET: /Personal/

        public   ActionResult Index()
        {
            return View("~/Areas/eForm/Views/Personal/Index.cshtml");
        }

        [HttpGet]
        public ActionResult Information()
        {
            return PartialView("~/Areas/eForm/Views/Personal/information.cshtml");
        }

        [HttpGet]
        public ActionResult ProxySetting()
        {

            //只显示代理生效中或即将生效的记录，已生效且完成的不在显示
            var model =
             new BaseServices<FormProxy>().LoadEntities(
                    c => c.SourceEmpNo == base.EmpNo && c.EndTime >= DateTime.Now).ToList();
            return PartialView("~/Areas/eForm/Views/Personal/ProxySetting.cshtml", model);
        }

        public ActionResult CreateProxy()
        {
            return PartialView("~/Areas/eForm/Views/Personal/CreateProxy.cshtml", new FormProxy
            {
                SourceEmpNo = base.EmpNo,
                FormId = 1
            });
        }

        [HttpPost]
        public string CreateProxy(FormProxy formProxy)
        {
            formProxy.SourceEmpNo = base.EmpNo;
            return new BaseServices<FormProxy>().IsAddEntity(formProxy) ? "success" : "error";
        }

        [HttpGet]
        public ActionResult MyReplaceProxy()
        {
            return PartialView("~/Areas/eForm/Views/Personal/MyReplaceProxy.cshtml", new BaseServices<FormProxy>().LoadEntities(c => c.EmpNo == base.EmpNo && c.StarTime < DateTime.Now && c.EndTime > DateTime.Now).ToList());
        }
        [HttpGet]
        public ActionResult ChangePassword()
        {
            var model = new EmployeeBll().Single(base.EmpNo);

            return PartialView("~/Areas/eForm/Views/Personal/ChangePassword.cshtml", new Carlzhu.Iooin.Entity.MANAGER.SystemAccount()
            {
                EmpNo = model.EmpNo,
                UserName = model.Account,
            });
        }

        public string ChangePassword(Carlzhu.Iooin.Entity.MANAGER.SystemAccount account)
        {
            return (SystemAccountBll.ChangePwd(account.UserName, account.OldPassword, account.NewPassword)) ? "success" : "error";
        }

    }
}
