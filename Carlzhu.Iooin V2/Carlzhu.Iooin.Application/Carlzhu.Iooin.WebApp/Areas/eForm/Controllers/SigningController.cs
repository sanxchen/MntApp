using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.FormModule;




using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Framework.Data.Repository;
using Webdiyer.WebControls.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    public class SigningController : FormControllerBase
    {
        //
        // GET: /Signing/

        protected readonly Signing Signing = new Signing();
        private static readonly Object Locker = new Object();

        public ActionResult List(int? id = 1)
        {
            int totalCount = 0;
            int pageIndex = id ?? 1;
            var userList = DataFactory.Database().FindListPage<BaseEmployee>("EmpNo", "desc", pageIndex, 10, ref totalCount);
            PagedList<BaseEmployee> mPage = userList.AsQueryable().ToPagedList(1, 10);
            mPage.TotalItemCount = totalCount;
            mPage.CurrentPageIndex = (int)(id ?? 1);
            return View(mPage);
        }



        public ActionResult Index()
        {
            int[] typeid = Signing.GetSignDataList(EmpNo).ToList().Select(c => c.Form.FormId).ToArray();

            var formtype =
                ContextFactory.ContextHelper.FormTypes.Where(k => typeid.Contains(k.FormId)).Select(m => new SelectListItem()
                {
                    Text = m.FormName,
                    Value = m.FormId.ToString()
                });
            ViewBag.FormType = formtype;
            Console.Write(formtype);
            return View("~/Areas/eForm/Views/Signing/index.cshtml", formtype);
        }


        //表单id
        public ActionResult GetSignDataList(int? id = 0)
        {



            var rsultList = id == 0
                ? Signing.GetSignDataList(EmpNo).Take(100)
                : Signing.GetSignDataList(EmpNo).Where(c => c.Form.FormType.FormId == id).Take(100);
            return PartialView("~/Areas/eForm/Views/Signing/SignData.cshtml", rsultList);
        }





        /// <summary>
        /// 忽略当前表单加载下一张
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public ActionResult Ignore(string formNo)
        {
            List<FormSign> formSigns = Signing.GetSignDataList(EmpNo);

            var firstOrDefault = formSigns.FirstOrDefault(c => c.FormNo == formNo);

            if (firstOrDefault != null)
            {
                int id = firstOrDefault.RowId;
                formSigns.RemoveAll(c => c.RowId <= id);
            }
            Thread.Sleep(1000);
            return Json(formSigns.Count > 0 ? ($"{formSigns[0].FormNo},{formSigns[0].RowId}").Encrypt() : "");
        }


        public ActionResult CopyTo(string url, string formNo)
        {
            //Login
         //  Base.GetResponse($"http://{System.Web.HttpContext.Current.Request.Url.Authority}/Account/Login", "POST", "username=carl.zhu&password=carl.zhu");
            ////GetHtml
            //string result = Base.GetResponse(
            //    $"http://{System.Web.HttpContext.Current.Request.Url.Authority}/FORM/Signing/SignPrint?p={url}", "Get", null);

            //result = result.Replace("href=\"/", "href=\"http://192.168.0.12:8888/").Replace("src=\"/", "src=\"http://192.168.0.12:8888/");


            return Json("表单副本已发送至您的邮箱，请注意查收", JsonRequestBehavior.AllowGet);
        }


        public ActionResult SignPrint(string p)
        {
            string[] para = p.Decrypt().Split(',');
            ViewBag.Item = int.Parse(para[1]);
            return View("~/Areas/eForm/Views/Signing/SignPrint.cshtml", new Applying().GetFormByFormNo(para[0]));
        }


        /// <summary>
        /// 更新标记
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        public JsonResult UpdateTags(int item, string tags)
        {
            int tag;
            return Json(new Dictionary<string, string>
            {
                {"result",  Signing.IsUpdateTags(item, tags, out tag)? "success" : "error"},
                {"tag", tag.ToString(CultureInfo.InvariantCulture)}
            });

        }

        public ActionResult UpdateFile(int item, string fileGroup)
        {
            var model = DataFactory.Database().FindEntity<FormSign>(item);

            Guid auditingFileGroup;
            Guid.TryParse(fileGroup, out auditingFileGroup);
            model.AuditingFileGroup = auditingFileGroup;

            int count = DataFactory.Database().Update(model);


            return Content((count > 0) ? "1" : "0");
        }



        /// <summary>
        /// 打开签核详细信息
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult SignDetails(string p)
        {
            try
            {
                string[] para = p.Decrypt().Split(',');
                ViewBag.Item = int.Parse(para[1]);


                ViewBag.Signed = DataFactory.Database().FindList<FormSign>("FormNo", para[0]).Where(c => c.AuditingFileGroup != Guid.Empty).ToList();


                //(formModel.FormType.Handler != null && formModel.FormType.Handler.Contains(base.EmpNo)



                //对副本抄送时帐号的特珠处理
                if ((Signing.GetSignDataList(EmpNo).Count(c =>
                    c.FormNo == para[0] &&
                    c.RowId.ToString(CultureInfo.InvariantCulture) == para[1] &&
                    c.SignResult == (int)FormSign.SignResultEnum.Watting) > 0))
                {
                    var formEntity = new Applying().GetFormByFormNo(para[0]);

                    ViewBag.IsHander = (formEntity.FormType.Handler != null && formEntity.FormType.Handler.Contains(base.EmpNo));

                    return View("~/Areas/eForm/Views/Signing/SignDetails.cshtml", formEntity);
                }

                return Link.ErrorBy(new Exception("这张表单不属于您，或者表单已被签核，请返回重试！！！"), this.GetType());

            }
            catch
            {
                return Link.ErrorBy(new Exception("系统参数解析错误，请不要随意修改系统参数"), this.GetType());

            }
        }

        /// <summary>
        /// 同意表单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string Agree(string p)
        {

            string[] para = p.Decrypt().Split(',');
            lock (Locker)
            {
                return Signing.Agree(para[0], int.Parse(para[1]), EmpNo) ? "success" : "error";
            }
        }

        /// <summary>
        /// 否决表单
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public string Reject(string p)
        {
            string[] para = p.Decrypt().Split(',');
            lock (Locker)
            {
                return Signing.Reject(para[0], int.Parse(para[1]), EmpNo) ? "success" : "error";
            }
        }


        /// <summary>
        /// 正在改这个
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="receiveEmp"></param>
        /// <param name="reason"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public JsonResult MakeOtherEmployee(string formNo, int item, string receiveEmp, string reason, int direction = -5)
        {
            if (receiveEmp == base.EmpNo)
            {
                return Json(new { result = false, msg = "不允许添加自己!!!!" });
            }

            string msg;
            bool result = (direction == -5)
                ? Signing.IsRedirectSign(formNo, item, receiveEmp, reason, EmpNo, out msg)
                : Signing.AddSign(formNo, item, receiveEmp, reason, direction, EmpNo, out msg);

            return Json(new { result, msg });
        }


        /// <summary>
        /// 根据表单号查找表单的签核记录
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public ActionResult GetSignRecoredsByFormNo(string formNo)
        {
            var modelList = Signing.GetSignRecoredsByFormNo(formNo);
            return PartialView("~/Areas/eForm/Views/Signing/CheckRecords.cshtml", modelList);
        }



        /// <summary>
        /// 根据表单号和签核号读取签核意见
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetSignMarkByItemAndFormNo(string formNo, int item)
        {
            return Signing.GetSignMarkByItemAndFormNo(formNo, item);
        }



        /// <summary>
        /// 根据表单号和签核记录号更新签核意见
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public string UpdateSignMarkByItemAndFormNo(string formNo, int item, string mark)
        {
            return Signing.IsUpdateSignMarkByItemAndFormNo(formNo, item, mark) ? "success" : "error";
        }


        [ChildActionOnly]
        public ActionResult Handle(string formNo)
        {
            return PartialView("~/Areas/eForm/Views/Signing/Handle.cshtml", formNo);
        }




        public ActionResult UpdateLeaveType(string p,int otype,int type)
        {
            string[] para = p.Decrypt().Split(',');
            lock (Locker)
            {
                StringBuilder sb = new StringBuilder($"UPDATE  FormWorkLeave SET LeaveType='{type}' WHERE FormNo='{para[0]}' ");

                Signing.IsUpdateSignMarkByItemAndFormNo(para[0], int.Parse(para[1]), $"{Enum.GetName(typeof(FormWorkLeave.LeaveTypEnum),otype)}=>{Enum.GetName(typeof(FormWorkLeave.LeaveTypEnum), type)}");
                return Content(DataFactory.Database().ExecuteBySql(sb) > 0 ? "success" : "error");
                
            }

           
        }
    }
}
