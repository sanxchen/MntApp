using System;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.FormModule;

using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.InteractiveAdapter;


namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
  
    public class ApplyingController : FormControllerBase
    {
        //ccc
        // GET: /Applying/
        readonly Applying _applying = new Applying();

       
        public ActionResult Welcome()
        {
            //~/Areas/eForm/Views
            return View("~/Areas/eForm/Views/Applying/Welcome.cshtml");
        }

      
        public ActionResult Index()
        {
            return View("~/Areas/eForm/Views/Applying/index.cshtml", DataFactory.Database().FindList<FormClass>().Where(c => c.Remark == "item").ToList());
        }

        public ActionResult GetFormTypeByHot()
        {
            var model = _applying.GetFormTypesByDisplay(c => c.IsDisplay && c.IsHot).ToList();

            return PartialView("~/Areas/eForm/Views/Applying/_FormTypes.cshtml", model);
        }


        public ActionResult GetFormTypeByDepartmentCode(string code)
        {
            var model = _applying.GetFormTypesByDisplay(c => c.IsDisplay && c.Class == code);

            return PartialView("~/Areas/eForm/Views/Applying/_FormTypes.cshtml", model);
        }

        public ActionResult Apply(string p)
        {
            try
            {
                var model = _applying.GetFormTypesByDisplay(c => c.FormId == int.Parse(p.Decrypt())).First();
                if (model == null) Link.ErrorBy(new Exception("系统参数解析错误，请返回重试"), this.GetType());
                return View("~/Areas/eForm/Views/Applying/Apply.cshtml", model);
            }
            catch (Exception)
            {
                return Link.ErrorBy(new Exception("请不要修改系统参数"), this.GetType());
            }

        }


        [HttpPost]
        public string Send(string formNo)
        {
            return _applying.Send(formNo, base.EmpNo) ? "success" : "error";
        }



    }
}
