using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.BehandModule;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;

using WebGrease.Css.Extensions;
//using FileGroup = Base.FileGroup;

namespace Carlzhu.Iooin.WebApp.Controllers
{
    public class DefaultController : Controller
    {
        //
        // GET: /Default/


        public ActionResult QueryDraws()
        {
            List<SelectListItem> searchType = DataFactory.Database().FindList<FormType>("AND Os='PDM'").Select(c => new SelectListItem
            {
                Text = c.FormName.Replace("发行申请单", ""),
                Value = c.FormId.ToString(CultureInfo.InvariantCulture)
            }).ToList();

            searchType.Add(new SelectListItem
            {
                Text = "发行文件综合查询",
                Value = "0"
            });

            ViewData["SearchType"] = searchType;
            ViewData["All"] = DataFactory.Database().FindCount<Published>();
            ViewData["See"] = DataFactory.Database().FindCountBySql("select sum(visit) from minicut.dbo.published  ");

            return View();
        }


        [ActionName("114")]
        public ActionResult M114(string keyValue)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                return RedirectToAction("Index");
            }
            ViewBag.KeyValue = keyValue;
            return View("/Views/Default/M114.cshtml");
        }

        public ActionResult Index()
        {
            return View();

        }


        /// <summary>
        /// 加载主页PDF说明
        /// </summary>
        public void Help()
        {
            BaseHelper.ViewPdf(PdfHelper.PdfToStream(Server.MapPath("/Resource/Template/help.pdf"), true, null, null, false, DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "minicutdraw.png"));
        }










        /// <summary>
        /// 公司通讯录
        /// </summary>
        /// <returns></returns>
        public ActionResult Phone()
        {
            return View("~/Views/Default/Phone.cshtml");
        }


        #region 前台搜索功能实现


        /// <summary>
        /// 前台搜索
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Search()
        {
            var s = new RouteValueDictionary();

            Request.Form.AllKeys.ForEach(c => s.Add(c.ToString(CultureInfo.InvariantCulture), Request.Form[c.ToString(CultureInfo.InvariantCulture)]));

            return RedirectToAction("Searching", "Default", s);
        }





        /// <summary>
        /// 实际搜素功能实现
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public ActionResult Searching(string p)
        {

            try
            {

                DateTime startTime = DateTime.Parse(Request["DateStart"]);
                DateTime endTime = DateTime.Parse(Request["DateEnd"]).AddDays(1);
                string[] keys = string.IsNullOrEmpty(Request["Keywords"]) ? new[] { "" } : Request["Keywords"].Split(' ');

                var searchType = Request["SearchType"];

                List<Published> publisheds;
                //文件综合查询
                if (searchType == "0")
                {
                    publisheds = (
                      new BaseServices<Published>().LoadEntities(c =>
                           c.PublishTime > startTime && c.PublishTime < endTime &&
                           !string.IsNullOrEmpty(c.ProductNo) &&
                           c.ProductNo.Contains(Request["Keywords"] ?? ""))).ToList().OrderByDescending(k => k.PublishType).ThenBy(m => m.PublishTime).ToList();
                    return View("~/Views/Default/rs.cshtml", publisheds);
                }



                if (searchType == "50")
                {
                    List<FormDrawingsSopDewell> result = new List<FormDrawingsSopDewell>();
                    var tkey = Request["Keywords"];
                    if (!string.IsNullOrEmpty(tkey) && tkey.StartsWith("@"))
                    {
                        result.AddRange(ContextFactory.ContextHelper.FormDrawingsSopDewells.Where(c => c.Form.CloseTime < endTime && c.Form.CloseTime > startTime && c.IsDel == false && c.Form.FormStatus == 3 && c.Tag == tkey.Substring(1)));
                    }
                    else
                    {
                        keys.ForEach(k =>
                        {
                            result.AddRange(
                                ContextFactory.ContextHelper.FormDrawingsSopDewells.Where(
                                    c => c.Form.CloseTime < endTime && c.Form.CloseTime > startTime && c.IsDel == false && c.Form.FormStatus == 3 && c.DrawPartNo.Contains(k)));
                        });
                    }


                    return View("~/Views/Default/DewellResult.cshtml", result.OrderByDescending(k => k.CustomerNo).ThenByDescending(k => k.Tag).ThenByDescending(k => k.Form.CreateTime));
                }


                var formType = new BaseServices<FormType>().LoadEntities(c => c.FormId == int.Parse(searchType)).First();

                //随便初始化一个F对像
                F<FormDrawingsBom> f = new F<FormDrawingsBom>();
                var type = f.ReflectionByFormType(formType);


                Type cls = typeof(Home);
                object instance = Activator.CreateInstance(cls);//实例这个类的对象

                //查找方法
                MethodInfo method = cls.GetMethod("HomeSearch",
                    BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new[] { typeof(DateTime), typeof(DateTime), typeof(string[]) },//方法所需的参数类型
                    null);

                //使方法为泛型类型方法
                method = method.MakeGenericMethod(type);

                publisheds = (List<Published>)method.Invoke(instance, new object[] { startTime, endTime, keys });


                ////二次反射创建 BehandQuery 实例，并绑定返回 BehandQuery<T>结果，再强是转成Ilist供程序使用
                //IList result = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(typeof(BehandQuery<>).MakeGenericType(type)), method.Invoke(instance, new object[] { startTime, endTime, keys }));




                ////将结果添加至发行记录中
                //publisheds.AddRange(from dynamic item in result select (Published)item.Published);

                //ViewBag.Result = result;
                ViewBag.FormType = formType;





                //参数处理



                //StringBuilder sb = new StringBuilder();
                //foreach (var propertyInfo in type.GetProperties().Where(propertyInfo =>
                //propertyInfo.GetCustomAttributes(typeof(TableShowAttribute)).Any() && propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute)).Any()))
                //{
                //    var v = ((DisplayNameAttribute[])propertyInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false))[0].DisplayName;
                //}


                //foreach (dynamic item in result)
                //{

                //}






                return View("~/Views/Default/query.cshtml", publisheds);

            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
                return RedirectToAction("Index");
            }
        }



        /// <summary>
        /// 前台搜索主体界面
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public ActionResult GetSearchBody(string formNo)
        {
            var apply = new Applying();

            var formType = apply.GetFormByFormNo(formNo).FormType;
            return PartialView($"cent/{formType.Method}Body", apply.GetFormEntityByFormNo(formNo, formType));
        }


        #endregion





        public ActionResult GetLayoutComm()
        {
            return PartialView("/Views/Shared/_Common.cshtml");
        }



        //#endregion


        /// <summary>
        /// 网页聊天程序
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult Chat()
        {
            return View("/Views/Default/Carlzhu.cshtml");
        }





        #region 设备SOP相关功能

        /// <summary>
        /// 已发行的SOP
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult EqSop()
        {
            var model = new BaseServices<FormEquipmentSop>().LoadEntities(c => true).ToList().Where(c => c.Form.FormStatus == 3).ToList();

            return View(model);
        }


        public void OtherOnline(string md5, string method)
        {
            string uppath = BaseHelper.UpPath;

            //打印与在线查看
            BaseHelper.ViewPdf(method == "print" ? PdfHelper.PdfToStream(uppath + md5, true, null, "mjdrawadm", true, "禁止复印 盖章有效", "minicutdraw.png") : PdfHelper.PdfToStream(uppath + md5, true, null, "mjdrawadm", false, "禁止复印 盖章有效", "minicutdraw.png"));
        }


        #endregion



        #region 图片在线预查看


        public void ViewImages(string fileName)
        {
            BaseHelper.ViewImage(fileName);
        }
        public void GetThumbnail(int width, int height, string fileName)
        {
            BaseHelper.GetThumbnail(width, height, fileName);
        }

        public string GetDrawLink(string partNo)
        {
            BaseServices<Published> pServices = new BaseServices<Published>();
            var model = pServices.LoadEntities(c => c.ProductNo == partNo && c.PublishType == 4).LastOrDefault();
            if (model == null) return "javascript:alert('此款产品没有内部图纸')";

            var files = new FilesFileGroupBll().GetFileListByGroupGuid(model.FileGroup);

            return $"/Pdm/PdfViews?publishKey={model.PubishedGuid}&md5={files[0].Md5}";
        }

        #endregion




        public ActionResult Kanban()
        {

            var model = new BaseServices<FormPdAbnor>().LoadEntities(c => !c.IsClose).ToList();
            model = model.Where(c => c.Form.FormStatus == (int)Form.StatusEnum.签核完成).ToList();
            return View(model);
        }


        public
          ActionResult Check_in(string cardno = "")
        {
            var patten = new Regex("\\d{7}");


            if (!patten.IsMatch(cardno)) return View();

            StringBuilder sql = new StringBuilder($"insert into eastriver.dbo.timerecords  (clock_id, emp_id,card_id,sign_time,dcollecttime,pos_sequ )  select '2' as clock_id, emp_no as emp_id,card_no as card_id,getdate() as sign_time,getdate() as dcollecttime,'0' as pos_sequ from rms.dbo.employee where emp_no = '{cardno}' ");
            int e = DataFactory.Database().ExecuteBySql(sql);
            return Content(e > 0 ? "Check in success" : "error");

        }
    }
}
