using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Business.TpaModule;


using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.FORM.f.draw;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using WebGrease.Css.Extensions;

namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    public class FController : FormControllerBase
    {

        //
        // GET: /F/

        private string FormNo { get; set; }
        private List<BaseEmployee> _employees;

        #region 申请表单

        /// <summary>
        /// 生成表单页面,主要作用是生成备注页
        /// </summary>S
        /// <param name="formId"></param>
        /// <param name="formNo"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult FormComm(int formId = 0, string formNo = "", string method = "Add")
        {

            //ViewData["Method"] = method;

            F model = new FormTest();

            if (formId > 0)
            {
                model.Form = new Form { FormType = new BaseServices<FormType>().LoadEntities(c => c.FormId == formId).First() };
                model.Form.FormType.FormId = formId;
            }
            else
            {
                if (!string.IsNullOrEmpty(formNo))
                {
                    var formType = new Applying().GetFormByFormNo(formNo).FormType;
                    var formbody = new Applying().GetFormEntityByFormNo(formNo);
                    model = formType.IsModel ? formbody : ((IEnumerable<F>)formbody).First();
                }
            }


            return PartialView($"~/Areas/eForm/Views/FormComm/FormComm{method}.cshtml", model);
        }





        /// <summary>
        /// 表单申请页面
        /// </summary>
        /// <param name="formId"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult ApplyForm(int formId)
        {
            var formType = new BaseServices<FormType>().LoadEntities(c => c.FormId == formId).First();

            ViewBag.DropCustomer = Task.Run(() => new TpaCustomerBll().GetCustomerDropList(null)).Result;
            ViewBag.DropSupplier = Task.Run(() => new TpaSupplierBll().GetSupplierDropList()).Result;
            ViewBag.FormType = formType;

            //随便初始化一个F对像
            F<FormDrawingsBom> f = new F<FormDrawingsBom>();
            var type = f.ReflectionByFormType(formType);

            ////var newfile = System.IO.File.ReadAllText(Server.MapPath($"~/Areas/eForm/Views/{formType.FormPage.Substring(4)}.cshtml"));
            ////创建一个临时cshtml文件
            //string file = $"~/Areas/eForm/Views/formtemp/{formType.FormName}B.cshtml";
            //if (!System.IO.File.Exists(file))
            //{
            //    System.IO.File.WriteAllText(Server.MapPath(file), formType.EditHtml, Encoding.UTF8);
            //}
            //return PartialView(file, formType.IsModel ? Activator.CreateInstance(type) : Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));

            return View($"~/Areas/eForm/Views/{formType.FormPage.Substring(4)}.cshtml",
                         formType.IsModel
                             ? Activator.CreateInstance(type)
                             : Activator.CreateInstance(typeof(List<>).MakeGenericType(type)));

        }


        public ActionResult InitEditBody()
        {
            string s = string.Empty;

            foreach (var formType in new BaseServices<FormType>().LoadEntities(c => true).ToList())
            {

                try
                {
                    formType.EditHtml = System.IO.File.ReadAllText(Server.MapPath($"~/Areas/eForm/Views/{formType.FormPage.Substring(4)}.cshtml"));
                    formType.DetailsHtml = System.IO.File.ReadAllText(Server.MapPath($"~/Areas/eForm/Views/{formType.FormPage.Substring(5)}Details.cshtml"));

                    if (new BaseServices<FormType>().UpdateEntity(formType))
                    {
                        s += formType.FormId + ",";
                    }

                }
                catch (Exception)
                {
                    continue;
                }


            }



            return Content(s);
        }





        /// <summary>
        /// 生成表单实体
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="employees">返回签核人信息至前台</param>
        /// <returns></returns>
        [NonAction]
        private Form CreateForm(Dictionary<string, object> dictionary, out List<BaseEmployee> employees)
        {

            int formId = int.Parse(dictionary["formid"].ToString());
            employees = new Applying().GetSignListByFormId(dictionary, formId, EmpNo);

            var form = new Form
            {
                FormNo = new Applying().CreateFormNo(new object()),
                FormId = formId,
                CreateEmpNo = EmpNo,
                CreateTime = DateTime.Now,
                SignPath = string.Join(",", employees.Select(c => c.EmpNo)) + ",",
                FormStatus = (int)Form.StatusEnum.未送出,
                IsEmergents = dictionary.ContainsKey("isemergents")
                //Explain = dictionary["Form.Explain"].ToString()

            };
            FormNo = form.FormNo;
            return form;
        }

        #endregion

        #region 修改表单


        /// <summary>
        /// 主要用来修改表单时查找表单备注
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public string GetFormMark(string formNo)
        {
            return ((F)(new Applying().GetFormEntityByFormNo(formNo))).Mark;
        }



        /// <summary>
        /// 表单修改页
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult EditForm(string formNo)
        {
            //取得表单序号，查看表单存放的实体类
            var formType = new Applying().GetFormByFormNo(formNo).FormType;


            ViewBag.DropCustomer = Task.Run(() => new TpaCustomerBll().GetCustomerDropList(null)).Result;
            ViewBag.DropSupplier = Task.Run(() => new TpaSupplierBll().GetSupplierDropList()).Result;
            ViewBag.FormType = formType;



            ////创建一个临时cshtml文件
            //string file = $"~/Areas/eForm/Views/formtemp/{formType.FormName}A.cshtml";
            //if (!System.IO.File.Exists(file))
            //{
            //    System.IO.File.WriteAllText(Server.MapPath(file), formType.EditHtml, Encoding.UTF8);
            //}


            //return PartialView(file,new Applying().GetFormEntityByFormNo(formNo, formType));


            return PartialView($"~/Areas/eForm/Views/{formType.FormPage.Substring(4)}.cshtml",
                new Applying().GetFormEntityByFormNo(formNo, formType));

        }









        /// <summary>
        /// 修改表单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditForm()
        {
            Dictionary<string, object> dictionary = Request.Form.Cast<object>().ToDictionary<object, string, object>(para => para.ToString(), para => Request.Form[para.ToString()]);

            FormType formType = new BaseServices<FormType>().LoadEntities(c => c.FormId == int.Parse(Request.Params["formid"])).FirstOrDefault();
            if (formType != null)
            {
                return GetRequetMethod(dictionary, formType, formType.IsModel ? "Edit" : "ListEdit");
            }

            return Link.ErrorBy(new Exception("系统参数错误，请返回重试"), GetType());
        }

        /// <summary>
        /// 提交修改结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        [NonAction]
        private ActionResult Edit<T>(Dictionary<string, object> dictionary) where T : F, new()
        {
            return new F<T>().EditForm(ReflectionModelByForm<T>(dictionary, ""), dictionary["FormNo"].ToString()) ? EditSuccess() : EditError();
        }

        [NonAction]
        private ActionResult ListEdit<T>(Dictionary<string, object> dictionary) where T : F, new()
        {
            var listEntity = new List<T>();
            for (int i = 1; i <= 10; i++)
            {
                if (!dictionary.ContainsKey("IsValid" + i)) continue;
                listEntity.Add(ReflectionModelByForm<T>(dictionary, i.ToString(CultureInfo.InvariantCulture))); ;
            }
            return new F<T>().EditForm(listEntity, dictionary["FormNo"].ToString()) ? EditSuccess() : EditError();
        }

        [NonAction]
        private ActionResult EditSuccess()
        {
            var conso = WebHelper.GetCookie("url");

            if (!string.IsNullOrEmpty(conso))
            {
                return Redirect(conso);
            }
            return RedirectToAction("Applyed", "Tracking");

        }

        [NonAction]
        private static ActionResult EditError()
        {
            return Link.ErrorBy(new Exception("修改失败，请返回重试"), typeof(FController));
        }

        #endregion

        #region 保存表单

        /// <summary>
        /// 提交表单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitForm()
        {

            Dictionary<string, object> dictionary = Request.Form.Cast<object>().ToDictionary<object, string, object>(para => para.ToString(), para => Request.Form[para.ToString()]);

            FormType formType = new BaseServices<FormType>().LoadEntities(c => c.FormId == int.Parse(Request.Params["formid"])).FirstOrDefault();

            return formType != null ? GetRequetMethod(dictionary, formType, formType.IsModel ? "Submit" : "ListSubmit") : Link.ErrorBy(new Exception("系统参数错误，请返回重试"), GetType());
        }

        [NonAction]
        private ActionResult Submit<T>(Dictionary<string, object> dictionary) where T : F, new()
        {
            var listEntity = new List<T> { ReflectionModelByForm<T>(dictionary, "") };
            return new F<T>().SaveData(listEntity, CreateForm(dictionary, out _employees)) ? SaveSuccess() : SaveError();
        }

        [NonAction]
        private ActionResult ListSubmit<T>(Dictionary<string, object> dictionary) where T : F, new()
        {
            var listEntity = new List<T>();
            //反射出所有字段
            for (int i = 1; i <= 10; i++)
            {
                if (!dictionary.ContainsKey("IsValid" + i)) continue;
                listEntity.Add(ReflectionModelByForm<T>(dictionary, i.ToString(CultureInfo.InvariantCulture)));
            }


            return new F<T>().SaveData(listEntity, CreateForm(dictionary, out _employees)) ? SaveSuccess() : SaveError();
        }


        /// <summary>
        /// 保存成功
        /// </summary>
        /// <returns></returns>
        private ActionResult SaveSuccess()
        {
            ViewBag.FormNo = FormNo;
            return PartialView("~/Areas/eForm/Views/Applying/Save.cshtml", _employees);
        }

        /// <summary>
        /// 保存失败
        /// </summary>
        /// <returns></returns>
        private static ActionResult SaveError()
        {
            return Link.ErrorBy(new Exception("表单保存失败，请返回重试"), typeof(FController));
        }


        #endregion


        #region 查看表单

        /// <summary>
        /// 表单详细信息页面
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public ActionResult TrackingForm(string formNo)
        {
            //取得表单序号，查看表单存放的实体类
            var formType = new Applying().GetFormByFormNo(formNo).FormType;
            var model = new Applying().GetFormEntityByFormNo(formNo, formType);



            //string file = $"~/Areas/eForm/Views/formtemp/{formType.FormName}D.cshtml";
            //if (!System.IO.File.Exists(file))
            //{
            //    System.IO.File.WriteAllText(Server.MapPath(file), formType.DetailsHtml, Encoding.UTF8);
            //}

       
            return PartialView($"~/Areas/eForm/Views/{formType.FormPage.Substring(4)}Details.cshtml", model);
        }

        #endregion


        [NonAction]
        private static ActionResult GetRequetMethod(Dictionary<string, object> dictionary, FormType formType, string requestMethod)
        {

            //随便初始化一个F子对像
            F<FormDrawingsBom> f = new F<FormDrawingsBom>();
            var type = f.ReflectionByFormType(formType);


            Type cls = typeof(FController);
            object instance = Activator.CreateInstance(cls); //实例这个类的对象
            MethodInfo method = cls.GetMethod(requestMethod,
                BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Public |
                BindingFlags.Instance,
                null, new[] { dictionary.GetType() }, //方法所需的参数类型
                null);

            method = method.MakeGenericMethod(type);
            return (ActionResult)method.Invoke(instance, new object[] { dictionary });


        }







        [NonAction]
        private static T ReflectionModelByForm<T>(IDictionary<string, object> dictionary, string i) where T : F, new()
        {
            var entity = new T();

            //添加表头信息
            foreach (var propertyInfo in (typeof(T)).GetProperties().Where(o => o.GetCustomAttributes(typeof(FormHeaderAttribute), false).Any()))
            {
                propertyInfo.SetValue(entity, TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertFrom(dictionary[propertyInfo.Name]), null);

            }

            //将添表体信息
            foreach (var propertyInfo in (typeof(T)).GetProperties().Where(propertyInfo => dictionary.ContainsKey(string.Format(propertyInfo.Name + i))))
            {
                var fullName = (propertyInfo.PropertyType).FullName;
                //去除父属性
                if (fullName.StartsWith("System.") || fullName.EndsWith("Enum"))
                {
                    try
                    {
                        propertyInfo.SetValue(entity, TypeDescriptor.GetConverter(propertyInfo.PropertyType).ConvertFrom(dictionary[propertyInfo.Name + i]), null);
                    }
                    catch (Exception exception)
                    {
                        propertyInfo.SetValue(entity, dictionary[propertyInfo.Name + i].ToString().Contains("true"));
                        Console.WriteLine(exception);
                    }

                }
            }

            return entity;
        }



        public bool UpdateInvolvingTag(string employees, Guid guid)
        {

            List<string> empList = new List<string>();

            employees.Split(',').ForEach(k => empList.Add(k.Split(':')[0]));


            return new Applying().UpdateInvolvingUser(empList, guid, EmpNo);

        }


        #region 公用页面内容以及上传控件加载

        /// <summary>
        /// 文件上传页面加载
        /// </summary>
        /// <param name="limits"></param>
        /// <param name="filetype"></param>
        /// <param name="reqFileExp"></param>
        /// <returns></returns>
        public ActionResult UpFilePage(string limits, string filetype, string reqFileExp)
        {
            if (string.IsNullOrEmpty(filetype)) return Content("上传控件加载中...");

            ViewBag.Limits = limits;
            ViewBag.FileType = String.Join(",", filetype);
            ViewBag.ReqFileExp = String.Join(",", reqFileExp);
            return PartialView("~/Areas/eForm/Views/Shared/_Upfile.cshtml");
        }

        /// <summary>
        /// 文件上传
        /// </summary>
        [HttpPost]
        public void UpFile()
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();

            string msg = "上传成功！";//返回信息

            string fileList = null;
            var groupId = Guid.NewGuid();

            List<Files> fileses = new List<Files>();
            try
            {
                int iTotal = Request.Files.Count;
                for (int i = 0; i < iTotal; i++)
                {
                    HttpPostedFileBase file = HttpContext.Request.Files[i];
                    if (file != null && (file.ContentLength > 0 || !string.IsNullOrEmpty(file.FileName)))
                    {
                        Stream stream = file.InputStream;
                        string md5 = DirFileHelper.GetMd5StringByStream(stream);

                        var files = database.FindEntity<Files>(md5);
                        if (string.IsNullOrEmpty(files.Md5))
                        {
                            //#region stream to byte[]

                            //byte[] bytes = new byte[stream.Length];
                            //stream.Read(bytes, 0, bytes.Length);
                            //stream.Seek(0, SeekOrigin.Begin);


                            //#endregion


                            //#region byte to stream
                            ////  Stream stream = new MemoryStream(bytes);
                            //#endregion


                            Files f = new Files
                            {
                                Md5 = md5,
                                FileName = file.FileName,
                                FileType = Path.GetExtension(file.FileName),
                                ContentType = file.ContentType,
                                Discriminator = "Files"
                                //Bytes = bytes.ToArray(),
                            };
                            database.Insert(f, isOpenTrans);
                            fileses.Add(f);
                            //上传文件

                            if (new Remote(Config.GetValue("RemoteConn")).Connect())
                            {
                                file.SaveAs($"{Config.GetValue("FormUDPath")}{md5}");
                            }
                        }
                        else
                        {
                            fileses.Add(files);
                        }

                        //返回前台的文件名
                        fileList += file.FileName + ",";
                    }
                }

                database.Insert(new FileGroup
                {
                    GroupGuid = groupId,
                    CreateTime = DateTime.Now,
                    CreateEmpNo = "1109001"
                }, isOpenTrans);

                List<FilesFileGroup> ffGroup = fileses.Select(c => new FilesFileGroup
                {
                    Md5 = c.Md5,
                    GroupGuid = groupId
                }).ToList();


                database.Insert(ffGroup, isOpenTrans);
                database.Commit();
            }
            catch (NullReferenceException)
            {
                database.Rollback();
                Response.Write("<script>alert('请登陆后继续操作！！！');window.parent.location.href='/Account/Login'</script>");
                Response.Flush();
                return;
            }
            catch (Exception exception)
            {
                database.Rollback();
                msg = "上传失败，请联系管理员" + exception.Message;
            }



            Response.Write("<script>window.parent.Finish('" + msg + "|" + groupId + "|" + fileList + "');</script>");
        }

        #endregion
    }


}
