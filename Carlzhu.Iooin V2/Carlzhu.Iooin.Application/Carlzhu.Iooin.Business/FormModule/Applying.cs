using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f.draw;

namespace Carlzhu.Iooin.Business.FormModule
{
    public class Applying
    {
        private readonly Forms _iApplying = new Forms();


        /// <summary>
        /// 根据条件加载可显示的表单类型
        /// </summary>
        /// <param name="whereLamda"></param>
        /// <returns></returns>
        public List<FormType> GetFormTypesByDisplay(Func<FormType, bool> whereLamda)
        {
            return new BaseServices<FormType>().LoadEntities(whereLamda).Where(c => c.IsDisplay).ToList();
        }


        public bool UpdateInvolvingUser(List<string> newemp, Guid guid, string empNo)
        {
            return _iApplying.UpdateInvolvingUser(newemp, guid, empNo);
        }


        /// <summary>
        /// 根据表单号查出表单实体
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public dynamic GetFormEntityByFormNo(string formNo)
        {
            var formType = this.GetFormByFormNo(formNo).FormType;

            return this.GetFormEntityByFormNo(formNo, formType);
        }




        public dynamic GetFormEntityByFormNo(string formNo, FormType formType)
        {
            //随便初始化一个F对像
            F<FormDrawingsBom> f = new F<FormDrawingsBom>();

            var type = f.ReflectionByFormType(formType);

            //等到这个泛型类型 设定泛型类型的泛型参数
            Type tType = typeof(FormDatabase<>).MakeGenericType(type);

            //得到这个类型的无参构造函数 然后调用 得到这个类型的实例
            var constructorInfo = tType.GetConstructor(Type.EmptyTypes);
            return constructorInfo == null ? Link.ErrorBy(new Exception("表单需加载类型还没在创建，请联系管理员！！！"), this.GetType()) : tType.GetMethod("LoadFormEntity").Invoke(constructorInfo.Invoke(null), new object[] { formNo, formType.IsModel });

            //得到这个类型的LoadEntity方法
        }
















        /// <summary>
        /// 生成表单号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public string CreateFormNo(object o)
        {
            return _iApplying.CreateFormNo(o);
        }


        /// <summary>
        /// 根据表单Id与前台提交的表单信息自动生成签核流程
        /// </summary>
        /// <param name="model">表单信息</param>
        /// <param name="formId">表单ID</param>
        /// <param name="empNo">提交人</param>
        /// <returns></returns>
        public List<BaseEmployee> GetSignListByFormId(Dictionary<string, object> model, int formId, string empNo)
        {
            return FormExtend.SetFormStart(formId, model, empNo);
        }


        /// <summary>
        /// 取得表单信息，根据表单号
        /// </summary>
        /// <param name="formNo">表单号</param>
        /// <returns></returns>
        public Form GetFormByFormNo(string formNo)
        {
            var model = new BaseServices<Form>().LoadEntities(c => c.FormNo == formNo).First();
            return model;
        }

        /// <summary>
        /// 将表单送签
        /// </summary>
        /// <param name="formNo">表单号</param>
        /// <param name="empNo">送签人</param>
        /// <returns></returns>
        public bool Send(string formNo, string empNo)
        {
            string firstEmployee;

            //送签结果
            bool result = _iApplying.Send(formNo, empNo, out firstEmployee);
            //
            //邮件通知
            //
            if (!result) return false;
            var signs = new List<string> { firstEmployee };

            var emailForm = new Carlzhu.Iooin.Business.BaseModule.BpmBll(formNo, empNo);
            //var emailForm = new EmailForm(formNo, empNo);
            emailForm.Send();
            emailForm.NoticeSigner(signs);
            emailForm.NoticeReplace(signs);

            TemplateBll.SendBpm(emailForm);

            return true;
        }


        /// <summary>
        /// 将表单类型转化为Select
        /// </summary>
        /// <param name="isReplace"></param>
        /// <param name="isHot"></param>
        /// <param name="isRedirect"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetFormTypeDropdownList(bool isReplace, bool isHot, bool isRedirect)
        {
            return this.GetFormTypesByDisplay(c => (!isReplace || c.IsReplace) && (!isHot || c.IsHot) && (!isRedirect || c.IsRedirect)).Select(c => new SelectListItem()
            {
                Text = $"[{c.FormId.ToString().PadLeft(3, '0')}]-{c.FormName}",
                Value = c.FormId.ToString(CultureInfo.InvariantCulture)
            });
        }

    }
}
