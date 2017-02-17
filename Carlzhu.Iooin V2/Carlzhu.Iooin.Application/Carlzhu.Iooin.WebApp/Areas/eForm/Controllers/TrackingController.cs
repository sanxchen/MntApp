using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.FormModule;


using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Util;
using Webdiyer.WebControls.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    public class TrackingController : FormControllerBase
    {
        //
        // GET: /Tracking/


        readonly Tracking _tracking = new Tracking();






        public  ActionResult Index()
        {
            return View("Index");
        }


        //public ActionResult GetFormInvolvingUser(Guid guid)
        //{
        //    BaseServices<Carlzhu.Iooin.Entity.FORM.f.FormInvolvingUser> formInvolvingUserService = new BaseServices<FormInvolvingUser>();


        //    ViewData["FormInvolvingUser"] = string.Join(",", formInvolvingUserService.LoadEntities(c => c.Guid == guid).ToList().Select(c => string.Format("{0}/{1}/{2}", c.EmpNo, c.BaseEmployee.RealName, c.Employee.DepartmentCode)));
        //    // formInvolvingUserService.LoadEntities(c => c.Guid == guid).ToList();

        //    return View("/Areas/eForm/Views/f/_EmployeesDetails.cshtml");
        //}

        [HttpPost]
        public string GetJsonInvolvingTag(Guid guid)
        {
            BaseServices<FormInvolvingUser> formInvolvingUserService = new BaseServices<FormInvolvingUser>();
            return string.Join(",", formInvolvingUserService.LoadEntities(c => c.Guid == guid).ToList().Select(c => string.Format("{0}:{1}", c.EmpNo, c.BaseEmployee.RealName)));
        }



        public ActionResult Details(string p)
        {
            try
            {

                string[] para = p.Decrypt().Split(',');
                string formno = para[0];
                string type = para[1];

                string empNo = base.EmpNo;
                var formModel = new Applying().GetFormByFormNo(formno);
                if (formModel != null)
                {
                    switch (type)
                    {
                        case "applyed":
                            if (formModel.CreateEmpNo != empNo) Link.ErrorBy(new Exception("此表单不属于您，若有问题请联系管理员"), this.GetType());
                            break;
                        case "signed":
                            //申请人可以看
                            if (formModel.CreateEmpNo == empNo) break;
                            //签核人才可以看
                            if (new Signing().GetSignRecoredsByFormNo(formno).Count(c => c.SignEmpNo == empNo || c.ActualSignEmpNo == empNo) <= 0) Link.ErrorBy(new Exception("您没有签核过此表单。不允许查看"), this.GetType());
                            break;
                    }
                }
                else
                {
                    Link.ErrorBy(new Exception("系统内部错误"), this.GetType());
                }

                ViewBag.RequestType = type == "applyed" ? "<a href='/eForm/Tracking/Applyed' />追寻申请</a>" : "<a href='/eForm/Tracking/Signed' />追寻签核</a>";
                return View("TrackingDetails", formModel);
            }
            catch (Exception)
            {
                return Link.ErrorBy(new Exception("对不起，您无权限查看此表单,如果您是该表单的申请者或签核者,请联系管理员"), this.GetType());
            }

        }

        #region 表单查询



        public ActionResult Applyed(int id = 1)
        {
            ViewData["Page"] = id;

            var startTime = string.IsNullOrEmpty(Request.Form["startTime"]) ? DateTime.Parse("2001/01/01") : DateTime.Parse(Request.Form["startTime"]);
            var endTime = string.IsNullOrEmpty(Request.Form["endTime"]) ? DateTime.Now : DateTime.Parse(Request.Form["startTime"]);

            var modelList = _tracking.GetApplyedFormList(startTime, endTime, base.EmpNo).OrderByDescending(c => c.CreateTime);

            int[] typeid = modelList.Select(c => c.FormId).Distinct().ToArray();

            var formtype =
              ContextFactory.ContextHelper.FormTypes.Where(k => typeid.Contains(k.FormId)).Select(m => new SelectListItem()
              {
                  Text = m.FormName,
                  Value = m.FormId.ToString()
              });
            ViewBag.FormType = formtype;


            if (Request.IsAjaxRequest())
                return PartialView("_Applyed", modelList.ToPagedList(id, 15));
            return View("Applyed", modelList.ToPagedList(id, 15));
        }

        [HttpPost]
        public ActionResult Applyed(string formType, int? id = 1, string formNo = "")
        {
            ViewData["Page"] = id;
            var startTime = string.IsNullOrEmpty(Request.Form["startTime"]) ? DateTime.Parse("2001/01/01") : DateTime.Parse(Request.Form["startTime"]);
            var endTime = string.IsNullOrEmpty(Request.Form["endTime"]) ? DateTime.Now : DateTime.Parse(Request.Form["endTime"]);

            var result = _tracking.GetApplyedFormList(startTime, endTime, base.EmpNo)
                .Where(c =>
                    c.FormNo.Contains(formNo)
                    &&
                    c.FormType.FormId.ToString(CultureInfo.InvariantCulture) ==
                    (string.IsNullOrEmpty(formType)
                        ? c.FormType.FormId.ToString(CultureInfo.InvariantCulture)
                        : formType)
                    )
                .OrderByDescending(c => c.CreateTime).ToList();

            if (string.IsNullOrEmpty(Request.Form["cksigning"]) &&
                string.IsNullOrEmpty(Request.Form["ckfinish"]) &&
                string.IsNullOrEmpty(Request.Form["ckreject"]) &&
                string.IsNullOrEmpty(Request.Form["ckcancel"]) &&
                string.IsNullOrEmpty(Request.Form["cknosend"])
                )
            {
            }
            else
            {
                if (string.IsNullOrEmpty(Request.Form["cksigning"]))
                    result.RemoveAll(c => c.FormStatus == (int)Form.StatusEnum.签核中);
                if (string.IsNullOrEmpty(Request.Form["ckfinish"]))
                    result.RemoveAll(c => c.FormStatus == (int)Form.StatusEnum.签核完成);
                if (string.IsNullOrEmpty(Request.Form["ckreject"]))
                    result.RemoveAll(c => c.FormStatus == (int)Form.StatusEnum.已否决);
                if (string.IsNullOrEmpty(Request.Form["ckcancel"]))
                    result.RemoveAll(c => c.FormStatus == (int)Form.StatusEnum.已撤消);
                if (string.IsNullOrEmpty(Request.Form["cknosend"]))
                    result.RemoveAll(c => c.FormStatus == (int)Form.StatusEnum.未送出);
            }

            var modelList = result.ToPagedList(int.Parse(id.ToString()), 15);

            if (Request.IsAjaxRequest())
                return PartialView("_Applyed", modelList);
            return View("Applyed", modelList);

        }





        public ActionResult Signed(int id = 1)
        {
            ViewData["Page"] = id;
            var startTime = string.IsNullOrEmpty(Request.Form["startTime"]) ? DateTime.Parse("2001/01/01") : DateTime.Parse(Request.Form["startTime"]);
            var endTime = string.IsNullOrEmpty(Request.Form["endTime"]) ? DateTime.Now : DateTime.Parse(Request.Form["startTime"]);

            var modelList = _tracking.GetSignedFormList(startTime, endTime, base.EmpNo).OrderByDescending(c => c.CreateTime);

            int[] typeid = modelList.Select(c => c.Form.FormId).Distinct().ToArray();

            var formtype =
              ContextFactory.ContextHelper.FormTypes.Where(k => typeid.Contains(k.FormId)).Select(m => new SelectListItem()
              {
                  Text = m.FormName,
                  Value = m.FormId.ToString()
              });
            ViewBag.FormType = formtype;


            if (Request.IsAjaxRequest())
                return PartialView("_Signed", modelList.ToPagedList(id, 15));
            return View("Signed", modelList.ToPagedList(id, 15));
        }

        [HttpPost]
        public ActionResult Signed(string formType, int? id = 1, string formNo = "", string createUser = "")
        {
            ViewData["Page"] = id;
            var startTime = string.IsNullOrEmpty(Request.Form["startTime"]) ? DateTime.Parse("2001/01/01") : DateTime.Parse(Request.Form["startTime"]);
            var endTime = string.IsNullOrEmpty(Request.Form["endTime"]) ? DateTime.Now : DateTime.Parse(Request.Form["startTime"]);

            var result = _tracking.GetSignedFormList(startTime, endTime, base.EmpNo)
                .Where(c =>
                    c.FormNo.Contains(formNo)
                    &&
                    c.Form.FormType.FormId.ToString(CultureInfo.InvariantCulture) ==
                    (string.IsNullOrEmpty(formType)
                        ? c.Form.FormType.FormId.ToString(CultureInfo.InvariantCulture)
                        : formType)
                    &&
                    string.Format("{0} {1}", c.Form.BaseEmployee.Account, c.Form.BaseEmployee.RealName).Contains(createUser))
                .OrderByDescending(c => c.SignTime).ToList();

            //结果处理

            if (
                string.IsNullOrEmpty(Request.Form["ckagree"]) &&
                string.IsNullOrEmpty(Request.Form["ckreject"])
                )
            {
            }
            else
            {

                if (string.IsNullOrEmpty(Request.Form["ckagree"]))
                    result.RemoveAll(c => c.SignResult == (int)FormSign.SignResultEnum.Agree);
                if (string.IsNullOrEmpty(Request.Form["ckreject"]))
                    result.RemoveAll(c => c.SignResult == (int)FormSign.SignResultEnum.Reject);

            }


            var modelList = result.ToPagedList(int.Parse(id.ToString()), 15);

            if (Request.IsAjaxRequest())
                return PartialView("_Signed", modelList);
            return View("Signed", modelList);

        }

        #endregion

        /// <summary>
        /// 表单查询后操作
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult ApplyOperators(string p)
        {
            string[] para = p.Decrypt().Split(',');
            string method = para[0];
            string formNo = para[1];

            bool result = false;
            switch (method)
            {
                case "urge":
                    result = _tracking.Urge(formNo, base.EmpNo);
                    break;
                case "cancel":
                    result = _tracking.Cancel(formNo, base.EmpNo);
                    break;
                case "send":
                    result = _tracking.Send(formNo, base.EmpNo);
                    break;
                case "del":
                    result = _tracking.Delete(formNo, base.EmpNo);
                    break;
                case "edit":
                    //检查合法性

                    var formModel = new Applying().GetFormByFormNo(formNo);


                    if (formModel.CreateEmpNo == base.EmpNo || (formModel.FormType.Handler != null && formModel.FormType.Handler.Contains(base.EmpNo)))
                    {
                        string url = string.Empty;

                        if (Request.UrlReferrer != null && Request.UrlReferrer.AbsolutePath != "/Login/Default")
                        {
                            url = Request.UrlReferrer.ToString();
                        }

                        WebHelper.WriteCookie("url", url);
                        return View("EditForm", formModel);
                    }
                    return ResponseToClient();

            }
            return Json(result ? "success" : "fail");
        }
    }
}
