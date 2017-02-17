using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Business.Initialization;
using Carlzhu.Iooin.Business.QualityModule;
using Carlzhu.Iooin.Business.TpaModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.InteractiveAdapter;

using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;

using WebGrease.Css.Extensions;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem.Controllers
{
    [LoginAuthorize]
    public class PdmController : Controller
    {
        //
        // GET: /eSystem/Pdm/

        public ActionResult QueryDrawings()
        {
            return View();
        }



        public void Pubdewell()
        {
            var datalist = new Signing().GetSignDataList("1504109").Take(1000);


            var query = new Quality();
            foreach (var formSign in datalist)
            {
                query.Published(formSign.FormNo, formSign.RowId, formSign.SignEmpNo, DateTime.Now, false);
            }


        }


        //
        // GET: /Pdm/
        //readonly BLL.Quality _pdm = new BLL.Quality();
        readonly Quality _pdm = new Quality();
        public ActionResult Index()
        {
            return View("Publishing", _pdm.GetPublishingList(ManageProvider.Provider.Current().UserId));
        }

        public ActionResult Publishing(string p)
        {
            try
            {
                var para = p.Decrypt().Split(',');
                return Json(_pdm.Published(para[0], int.Parse(para[1]), ManageProvider.Provider.Current().UserId, null, para[2] == "1") ? "发行成功" : "发行失败");
            }
            catch (Exception)
            {
                return Link.ErrorBy(new Exception("请不要修改系统参数"), this.GetType());
            }

        }

        #region 搜索结果操作

        private static void GetPdf(Guid publishKey, string md5, string method)
        {

            Uri url = System.Web.HttpContext.Current.Request.UrlReferrer;

            var file = new FilesBll().SignFiles(md5);

            if (url != null && file != null && url.AbsolutePath.Contains("Pdm/PdfViews"))
            {
                var publish = DataFactory.Database().FindEntity<Published>(publishKey);

                string targetPath = Path.GetFullPath(BaseHelper.PublishedPath);

                if (method == "Showing")
                {
                    BaseHelper.ViewPdf(publish.IsDel ? PdfHelper.PdfToStream(targetPath + md5, true, "mjdrawadm", "mjdrawadm", false, "旧版本保留", "minicutdraw.png") : PdfHelper.PdfToStream(targetPath + md5, true, publish.IsPass ? "minicut*" : null, "mjdrawadm", false, "禁止复印 盖章有效", "minicutdraw.png"));
                }
                else
                {
                    BaseHelper.ViewPdf(publish.IsDel ? PdfHelper.PdfToStream(targetPath + md5, true, null, "mjdrawadm", true, "旧版本保留", "minicutdraw.png") : PdfHelper.PdfToStream(targetPath + md5, true, null, "mjdrawadm", true, "禁止复印 盖章有效", "minicutdraw.png"));
                }
            }
            else
            {
                Link.ErrorBy(new Exception("文件链接错误，请走正常链接..."), typeof(PdmController));
            }

        }







        public void Showing(Guid publishKey, string md5)
        {
            GetPdf(publishKey, md5, "Showing");
        }

        [Authorization]
        public void Print(Guid publishKey, string md5)
        {
            GetPdf(publishKey, md5, "Print");
        }

        /// <summary>
        /// 发行文件预览页
        /// </summary>
        /// <param name="publishKey"></param>
        /// <param name="md5"></param>
        /// <returns></returns>
        public ActionResult PdfViews(Guid publishKey, string md5)
        {
            //update visit
            Task.Run(() => new Quality().PdfView(publishKey, md5));

            var publish = new BaseServices<Published>().LoadEntities(c => c.PubishedGuid == publishKey).FirstOrDefault();
            if (publish != null)
            {
                var formType = new Applying().GetFormByFormNo(publish.FormNo).FormType;
                DrawingsBase bBase = new Applying().GetFormEntityByFormNo(publish.FormNo, formType);



                //  var file = BaseServices<Files>().LoadEntities(c => c.Md5 == md5).FirstOrDefault();

                var file = new FilesBll().SignFiles(md5);

                if (file == null || file.FileType != ".pdf")
                {
                    return Link.ErrorBy(new Exception("暂不支持此文件在线查看"), GetType());
                }


                var oldRecord = Task.Run(() => new BaseServices<Published>().LoadEntities(
                    c => c.Identity == publish.Identity && c.PubishedGuid != publish.PubishedGuid).ToList());

                var contiguous = Task.Run(() => new BaseServices<Published>().LoadEntities(
                    c => c.PublishVer == publish.PublishVer && c.ProductNo == publish.ProductNo && c.PubishedGuid != publish.PubishedGuid).ToList());


                ViewBag.DrawType = formType.FormName;
                ViewBag.PublishKey = publishKey;
                ViewBag.Md5 = md5;
                ViewBag.File = file.FileName;
                ViewBag.Base = bBase;
                ViewBag.OldRecord = oldRecord.Result;
                ViewBag.Contiguous = contiguous.Result;
            }

            return View("/Areas/eSystem/Views/Pdm/PdfViews.cshtml", publish);
        }



        #endregion

        #region 旧文件导入
        /// <summary>
        /// 前台页面
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorization]
        [HttpGet]
        public ActionResult OldImport()
        {
            return View("oldimport");
        }

        [HttpPost]
        public ActionResult OldImport(Guid fileGroup, string sheet)
        {
            List<FilesFileGroup> files = new FilesFileGroupBll().GetFileListByGroupGuid(fileGroup);

            if (files.Count == 1)
            {
                var phyPath = Path.GetFullPath(BaseHelper.UpPath) + files[0].Md5;

                DataTable dt = ExcepNpoi.ExcelToDataTable(phyPath, sheet, 0);

                var ds = new DataSet();

                ds.Tables.Add(dt);

                int count = _pdm.OldImport(ds);

                return Json(count > 0 ? "上传成功(" + count + "/" + dt.Rows.Count + ")！！！！" : "系统错误");

            }


            return Json("文件个数错误", JsonRequestBehavior.AllowGet);
        }


        public ActionResult DrawReplace(string formNo)
        {
            return Json(_pdm.DrawReplace(formNo, ManageProvider.Provider.Current().UserId) ? "替换成功" : "替换失败");
        }


        #endregion



        #region 报表查看

        public ActionResult Report()
        {
            ViewBag.DropCustomer = new TpaCustomerBll().GetCustomerDropList(null);
            return View("Report");
        }

        public ActionResult GetYearReport(string year = "2015")
        {
            var yearData = new BaseServices<Published>().LoadEntities(c => c.PublishTime.Year.ToString() == year).ToList().GroupBy(c => c.FormType).ToList();

            string[] colors = { "#272727", "#4D0000", "#820041", "#5E005E", "#3A006F", "#000093", "#003D79", "#005757", "#01814A", "#007500", "#548C00", "#5B5B00", "#796400", "#9F5000", "#842B00" };

            var sbYear = new StringBuilder("[");

            int color = 0;
            foreach (var typedata in yearData)
            {
                ++color;
                sbYear.Append("{ data:[");
                int sum = 0;
                for (int month = 1; month <= 12; month++)
                {
                    int count = typedata.Count(c => c.PublishTime.Month.Equals(month));
                    sbYear.Append($"[{month},{count}],");
                    sum = sum + count;
                }
                sbYear.Append("],label:'" + typedata.Key.FormName + "(<a href=\"/Quality/Pdm/ExportReport?y=" + year + "&t=" + typedata.Key.FormId + "\" >" + sum + "</a>)',color:'" + colors[color] + "' },");
            }

            sbYear.Append("]");
            return Json(sbYear.ToString(), JsonRequestBehavior.AllowGet);

        }

        public ActionResult GetMonthReport(string month = "1")
        {

            var monthData = new BaseServices<Published>().LoadEntities(
                     c => c.PublishTime.Year.Equals(DateTime.Now.Year) && c.PublishTime.Month.ToString().Equals(month)).ToList().GroupBy(c => c.FormType).ToList();

            var sbMonth = new StringBuilder("[");

            foreach (var typedata in monthData)
            {
                sbMonth.Append("{label:'" + typedata.Key.FormName + "(<a href=\"/Quality/Pdm/ExportReport?m=" + month + "&t=" + typedata.Key.FormId + "\" >" + typedata.Count() + "</a>)',data:" + typedata.Count() + "},");
            }
            sbMonth.Append("]");


            return Json(sbMonth.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCustomerReport(string customerNo)
        {
            var yearData = new BaseServices<Published>().LoadEntities(c => c.CustomerNo == customerNo && c.PublishTime.Year.Equals(DateTime.Now.Year)).ToList();
            var sbCustomer = new StringBuilder("[");
            for (int i = 1; i <= 12; i++)
            {
                sbCustomer.Append($"[{i},{yearData.Count(c => c.PublishTime.Month == i)}],");
            }

            sbCustomer.Append("]");

            return Json(sbCustomer.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(string y, string m, int t)
        {
            List<Published> data = null;


            FormType formType = new BaseServices<FormType>().LoadEntities(c => c.FormId == t).First();

            if (!string.IsNullOrEmpty(y))
                data = new BaseServices<Published>().LoadEntities(c => c.PublishTime.Year.ToString() == y).ToList();

            if (!string.IsNullOrEmpty(m))
                data = new BaseServices<Published>().LoadEntities(c => c.PublishTime.Year.Equals(DateTime.Now.Year) && c.PublishTime.Month.ToString() == m).ToList();

            if (data != null)
            {
                data = data.Where(d => d.PublishType == t).ToList();

                //将发行记录改为发行实体
                var formnoBuilder = new StringBuilder("(");
                data.Where(d => d.PublishType == t).ForEach(k => formnoBuilder.Append("'" + k.FormNo + "',"));

                string whereString = formnoBuilder.ToString();


                DataTable dt = DataFactory.Database().FindTableBySql($"SELECT * FROM Form{formType.Method} " + "WHERE FormNo in " + whereString.Substring(0, whereString.Length - 1) + ")");


                //下载
                HttpContext.Response.ContentType = "application/x-excel";
                string fileName = HttpUtility.UrlEncode($"{formType.FormName}.xls");
                HttpContext.Response.AddHeader("Content-Disposition", "attachment;filename=" + fileName);




                //var ds = new DataSet();
                //ds.Tables.Add(dt);
                var workbook = new ExcepNpoi().DataTableToStreamExcel(dt, "Data");
                workbook.Write(HttpContext.Response.OutputStream);
                return null;
            }
            return Link.ErrorBy(new Exception("内部异常，请联系管理员"), this.GetType());

        }


        #endregion
    }

}

