using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;
using Carlzhu.Iooin.Util.Offices;

using Webdiyer.WebControls.Mvc;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem.Controllers
{

    [LoginAuthorize]
    public class AbnormalController : Controller
    {
        //
        // GET: /eSystem/Abnormal/


        readonly BaseServices<FormNcr> _formNcrServices = new BaseServices<FormNcr>();

        public ActionResult Index(
            string formno,
            string timestart, string timeend, string partno = "", string customerno = "", string workstation = "", string abnormalImage = "", string abnormalAttribution = "", int? id = 1)
        {
            int totalCount = 0;
            int pageIndex = id ?? 1;
            IQueryable<FormNcr> userList =
                GetFormNcrs(formno, timestart, timeend, out totalCount,
                    partno, customerno, workstation, abnormalImage, abnormalAttribution, id);


            PagedList<FormNcr> mPage = userList.AsQueryable().ToPagedList(pageIndex, 15);
            mPage.TotalItemCount = totalCount;
            mPage.CurrentPageIndex = (int)(id ?? 1);
            return View(mPage);


        }


        public ActionResult AForm(string p)
        {
            var para = p.Decrypt().Split(',');

            try
            {
                string formNo = para[0];
                var form = ContextFactory.ContextHelper.Forms.Find(formNo);

                if (form.FormStatus != (int)Form.StatusEnum.签核完成) return Content("表单状态不对");

                var model = ContextFactory.ContextHelper.FormNcrs.First(c => c.FormNo == formNo);

                return View("Forms", model);
            }
            catch (Exception)
            {
                Link.ErrorBy(new Exception("请不要随便更改加密参数"), this.GetType());
            }

            return Link.ErrorBy(new Exception("请不要随便更改加密参数"), this.GetType());
        }

        public ActionResult SavePart(FormNcrPart part, string abnormalImage, string abnormalAttribution)
        {
            if (!string.IsNullOrEmpty(abnormalImage) || !string.IsNullOrEmpty(abnormalAttribution))
            {
                var upNcrMain = new BaseServices<FormNcr>();
                //保存描述
                var ncrMain = upNcrMain.Single(c => c.FormNo == part.ParentFormNo);
                ncrMain.AbnormalImage = abnormalImage;
                ncrMain.AbnormalAttribution = abnormalAttribution;
                upNcrMain.UpdateEntity(ncrMain);

            }


            //return Content("111");
            string empNo = ManageProvider.Provider.Current().UserId;
            Dictionary<string, object> dictionary = Request.Form.Cast<object>().ToDictionary<object, string, object>(para => para.ToString(), para => Request.Form[para.ToString()]);

            var formId = int.Parse(dictionary["formid"].ToString());
            var employees = new Applying().GetSignListByFormId(dictionary, formId, empNo);
            var form = new Form
            {
                FormNo = new Applying().CreateFormNo(new object()),
                FormId = formId,
                CreateEmpNo = empNo,
                CreateTime = DateTime.Now,
                SignPath = string.Join(",", employees.Select(c => c.EmpNo)) + ",",
                FormStatus = 0,
                IsEmergents = true,
            };



            FormNcrPart partA = new FormNcrPart() { Mark = part.Mark, ReplyType = formId, ParentFormNo = part.ParentFormNo, Reviewer = empNo, ReviewTime = DateTime.Now };
            var result = new F<FormNcrPart>().SaveData(new List<FormNcrPart>() { partA }, form);


            //送签
            if (!result) return Link.ErrorBy(new Exception("数据更新失败，请检查填写是否完整"), this.GetType());
            new Applying().Send(form.FormNo, empNo);


            var daiqian = new BaseServices<FormSign>().LoadEntities(c => c.FormNo == form.FormNo).Last();
            var signed = new Signing().Agree(daiqian.FormNo, daiqian.RowId, daiqian.SignEmpNo);
            Console.Write(signed);

            return Content("success");
        }


        public void AbnormalExport()
        {
            //所有异常
            List<FormNcr> models;


            if (Request.UrlReferrer != null)
            {
                var decoded = UrlToData(Request.UrlReferrer.ToString());
                var paras = decoded.Item2.ToList();
                Dictionary<string, string> urlparas = new Dictionary<string, string>();//= paras.ToDictionary(kvPair => kvPair.Key, kvPair => kvPair.Value);

                foreach (var kv in paras.Where(kv => !urlparas.ContainsKey(kv.Key)))
                {
                    urlparas.Add(kv.Key, kv.Value);
                }



                int total = 0;
                models = GetFormNcrs(urlparas["formno"], urlparas["timestart"], urlparas["timeend"], out total,
                       urlparas["partno"], urlparas["customerno"], urlparas["workstation"], urlparas["AbnormalImage"], urlparas["AbnormalAttribution"], 1, 500).ToList();

              


            }
            else
            {
                models = ContextFactory.ContextHelper.FormNcrs.Where(c => c.Form.FormStatus == (int)Form.StatusEnum.签核完成).ToList();
            }


            DataTable tt = models.ToDataTable(new List<string>()
            {
                "Customer",
                "检验员",
                "文件编号",
                "报告编号/NO",
                "Ver",
                "客户编号",
                "批号",
                "抽样标准",
                "AQL",
                "允收标准",
                "AC",
                "RE",
                "缺陷等级",
                "DefectsGradeMa",
                "DefectsGradeMi",
                "异常发现地点",
                "供方",
                "文件组",
                "AuditEmp",
                "FormNo",
                "RowId",
                "BaseEmployee", "Form", "FormNcrParts", "GroupGuid", "Supplier"
            });
            Console.Write(tt);


            HttpContext.Response.ContentType = "application/x-excel";
            HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode($"异常单{DateTime.Now.ToString("yyyyMMddHHss")}.xls"));
            (new ExcepNpoi().DataTableToStreamExcel(
                tt, null, "异常")).Write(HttpContext.Response.OutputStream);
        }


        static Tuple<string, IEnumerable<KeyValuePair<string, string>>> UrlToData(string url)
        {
            if (url == null)
                throw new ArgumentNullException(nameof(url));
            url = url.Trim();
            try
            {

                var split = url.Split(new[] { '?', '&' }, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 1)
                    return new Tuple<string, IEnumerable<KeyValuePair<string, string>>>(url, null);
                //获取前面的URL地址
                var host = split[0];
                var pairs = split.Skip(1).Select(s =>
                {
                    //没有用String.Split防止某些少见Query String中出现多个=，要把后面的无法处理的=全部显示出来
                    var idx = s.IndexOf('=');
                    return new KeyValuePair<string, string>(Uri.UnescapeDataString(s.Substring(0, idx)), Uri.UnescapeDataString(s.Substring(idx + 1)));
                }).ToList();

                return new Tuple<string, IEnumerable<KeyValuePair<string, string>>>(host, pairs);
            }
            catch (Exception ex)
            {
                throw new FormatException("URL格式错误", ex);
            }
        }

        /// <summary>
        /// 获取查询或导出数据
        /// </summary>
        /// <param name="formno"></param>
        /// <param name="timestart"></param>
        /// <param name="timeend"></param>
        /// <param name="totalCount"></param>
        /// <param name="partno"></param>
        /// <param name="customerno"></param>
        /// <param name="workstation"></param>
        /// <param name="abnormalImage"></param>
        /// <param name="abnormalAttribution"></param>
        /// <param name="id"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IQueryable<FormNcr> GetFormNcrs(string formno,

            string timestart,
            string timeend, out int totalCount,
            string partno = "",
            string customerno = "",
            string workstation = "", string abnormalImage = "", string abnormalAttribution = "", int? id = 1, int pageSize = 15)
        {



            int pageIndex = id ?? 1;
            IQueryable<FormNcr> userList;

            if (!string.IsNullOrEmpty(formno))
            {
                userList = new BaseServices<FormNcr>().LoadPageEntities(
                  pageIndex, pageSize,
                  out totalCount,
                  c => (c.FormNo == formno),
                  false,
                  d => d.CatchTime,m=>m.Form.FormStatus==3);
            }
            else
            {
                userList = new BaseServices<FormNcr>().LoadPageEntities(
                pageIndex, pageSize,
                out totalCount,
                c => (
                (
                true
                && (timestart == null || (c.CatchTime >= DateTime.Parse(timestart) && c.CatchTime <= DateTime.Parse(timeend))) //确认是否有时间存在
                && c.PartNo.Contains(partno)
                && c.CustomerNo.Contains(customerno))
                && (workstation == "" || workstation.Contains(c.AbnormalPointWorkshop.ToString())) //如果没有车间，直接返回true
                && (abnormalImage == "" || c.AbnormalImage.ToCharArray().Intersect(abnormalImage.ToCharArray()).Any()) //如果没有车间，直接返回true
                && (abnormalAttribution == "" || c.AbnormalAttribution.ToCharArray().Intersect(abnormalAttribution.ToCharArray()).Any()) //如果没有车间，直接返回true
                ),
                false,
                d => d.CatchTime,m=>m.Form.FormStatus==3);
            }


            return userList.Where(c => c.Form.FormStatus == (int)Form.StatusEnum.签核完成);

        }

    }

}
