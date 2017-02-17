using System;
using System.Collections.Generic;
using System.Linq;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Business.HrmsModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Business.BaseModule
{
    public class BpmBll : TemplateBll
    {
        public BpmBll(string formNo, string empNo)
            : base(formNo, empNo)
        {
        }

        /// <summary>
        /// 表单送出邮件
        /// </summary>
        /// <returns></returns>
        public void Send()
        {
            if(base.Form.BaseEmployee.Email==null) return;
            base.ListArgs.Add(new EmailFormEventArgs
            {
                To = new Dictionary<string, string> { { base.Form.BaseEmployee.Email, base.Form.BaseEmployee.Account } },
                Subject = "表单送签成功[" + base.Form.FormType.FormName + "]",
                NickName = base.Form.BaseEmployee.Account,
                Title = "表单已送出[" + base.Form.FormType.FormName + "]",
                From = "MINICUT",
                Content = "<p>您申请的表单已成功送出；</p><p>表单号为：" + base.Form.FormNo + "</p>",
                Date = DateTime.Now,
                Link = $"进入系统处理查看<a href='{base.HostUrl}{"/Form/Tracking/Index"}' >进入系统</a>"
            });
        }



        /// <summary>
        /// 通知审核人
        /// </summary>
        /// <param name="signers"></param>
        /// <returns></returns>
        public void NoticeSigner(List<string> signers)
        {
            if (base.Form.BaseEmployee.Email == null) return;
            signers.ForEach(emp =>
            {
                BaseEmployee signerEmployee = new BaseEmployeeBll().Single(emp);

                base.ListArgs.Add(new EmailFormEventArgs()
                {
                    To = new Dictionary<string, string> { { signerEmployee.Email, signerEmployee.Account } },
                    Subject = "表单签核通知[" + base.Form.FormType.FormName + "]",
                    NickName = signerEmployee.Account,
                    Title = "表单签核通知[" + base.Form.FormType.FormName + "]",
                    From = "MINICUT",
                    Content = "<p>系统中有一张由 " + base.Form.BaseEmployee.Account + " 申请的表单需要您要的处理;</p><p>表单号为:" + base.Form.FormNo + ";</p>",
                    Date = DateTime.Now,
                    Link = $"进入系统处理表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
                });
            });


        }

        /// <summary>
        /// 通知代理人
        /// </summary>
        /// <param name="sourceEmps"></param>
        /// <returns></returns>
        public void NoticeReplace(List<string> sourceEmps)
        {
            if (base.Form.BaseEmployee.Email == null) return;
            sourceEmps.ForEach(emp =>
            {
                BaseEmployee sourceEmployee = new BaseEmployeeBll().Single(emp);

                List<FormProxy> replaceEmployees =
                 new   BaseServices<FormProxy>().LoadEntities(c => c.SourceEmpNo == emp && c.FormId == base.Form.FormType.FormId && c.StarTime < DateTime.Now && c.EndTime > DateTime.Now).ToList();
                replaceEmployees.ForEach(e => base.ListArgs.Add(new EmailFormEventArgs()
                {
                    To = new Dictionary<string, string> { { e.BaseEmployee.Email, e.BaseEmployee.Account } },
                    Subject = "表单签核通知【代签】[" + base.Form.FormType.FormName + "]",
                    NickName = e.BaseEmployee.Account,
                    Title = "表单签核通知【代签】[" + base.Form.FormType.FormName + "]",
                    From = "MINICUT",
                    Content = "<p>系统中有一张由" + base.Form.BaseEmployee.Account + "申请的表单需要您或 <b>" + sourceEmployee.Account + "</b> 的处理;</p><p>表单号为:" + base.Form.FormNo + ";</p>",
                    Date = DateTime.Now,
                    Link = $"进入系统处理表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
                }));

            });
        }


        /// <summary>
        /// 否决表单
        /// </summary>
        public void Reject()
        {
            if (base.Form.BaseEmployee.Email == null) return;
            base.ListArgs.Add(new EmailFormEventArgs()
            {
                To = new Dictionary<string, string> { { base.Form.BaseEmployee.Email, base.Form.BaseEmployee.Account } },
                Subject = "表单否决通知[" + base.Form.FormType.FormName + "]",
                NickName = base.Form.BaseEmployee.Account,
                Title = "表单否决通知[" + base.Form.FormType.FormName + "]",
                From = "MINICUT",
                Content = "<p>您申请的一张表单已被审核人 <b>" + base.Emp.Account + "</b> 否决;</p><p>表单号为;" + base.Form.FormNo + ";</p>",
                Date = DateTime.Now,
                Link = $"进入系统查看表单<a href='{HostUrl}{"/eForm/Tracking/Index"}' >进入系统</a>"
            });
        }


        /// <summary>
        /// 添加加签,加签在后不发
        /// </summary>
        /// <param name="addSignerEmpNo"></param>
        /// <param name="direct"></param>
        /// <returns></returns>
        public void AddSigner(string addSignerEmpNo, int direct)
        {
            if (base.Form.BaseEmployee.Email == null) return;
            if (direct != (int)FormSign.DirectEnum.Befor && direct != (int)FormSign.DirectEnum.Parallel) return;

            var addSignerEmployee = new EmployeeBll().Single(addSignerEmpNo);
            base.ListArgs.Add(new EmailFormEventArgs()
            {
                To = new Dictionary<string, string> { { addSignerEmployee.Email, addSignerEmployee.Account } },
                Subject = "表单签核通知【加签】[" + base.Form.FormType.FormName + "]",
                NickName = addSignerEmployee.Account,
                Title = "表单签核通知【加签】[" + base.Form.FormType.FormName + "]",
                From = "MINICUT",
                Content = "<p>系统中有一张由审核人<b> " + base.Emp.Account + "</b> 加签给您的表单;</p><p>表单号为:" + base.Form.FormNo + ";</p>",
                Date = DateTime.Now,
                Link = $"进入系统处理表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
            });
        }


        /// <summary>
        /// 发送签核在后的表单
        /// </summary>
        /// <param name="addSignerEmpNo"></param>
        /// <param name="direct"></param>
        public void AddSignerAfter(string addSignerEmpNo, int direct)
        {
            //手工将direct 1改为  非1 ，强制发送
            this.AddSigner(addSignerEmpNo, 0);
        }

        /// <summary>
        /// 转签
        /// </summary>
        /// <returns></returns>
        public void RedirectSigner(string redirectSignerEmpNo)
        {
            if (base.Form.BaseEmployee.Email == null) return;
            var redirectSignerEmployee = new EmployeeBll().Single(redirectSignerEmpNo);

            base.ListArgs.Add(new EmailFormEventArgs()
            {
                To = new Dictionary<string, string> { { redirectSignerEmployee.Email, redirectSignerEmployee.Account } },
                Subject = "表单签核通知【转签】[" + base.Form.FormType.FormName + "]",
                NickName = redirectSignerEmployee.Account,
                Title = "表单签核通知【转签】[" + base.Form.FormType.FormName + "]",
                From = "MINICUT",
                Content = "<p>系统中有一张由 " + base.Form.BaseEmployee.Account + " 申请，审核人<b> " + base.Emp.Account + "</b> 转签给您的表单需要您的处理;</p><p>表单号为:" + base.Form.FormNo + ";</p>",
                Date = DateTime.Now,
                Link = $"进入系统处理表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
            });
        }

        /// <summary>
        /// 签核完成
        /// </summary>
        public void Finlish()
        {
            if (base.Form.BaseEmployee.Email == null) return;
            base.ListArgs.Add(new EmailFormEventArgs()
            {
                To = new Dictionary<string, string> { { base.Form.BaseEmployee.Email, base.Form.BaseEmployee.Account } },
                Subject = "表单签核完成通知[" + base.Form.FormType.FormName + "]",
                NickName = base.Form.BaseEmployee.Account,
                Title = "表单签核完成通知[" + base.Form.FormType.FormName + "]",
                From = "MINICUT",
                Content = "<p>系统中有一张您申请的表单已处理完毕;</p><p>表单号为:" + base.Form.FormNo + ";</p>",
                Date = DateTime.Now,
                Link = $"进入系统查看表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
            });
        }





        /// <summary>
        /// 催签
        /// </summary>
        public void Urge(List<string> signsEmps)
        {
            if (base.Form.BaseEmployee.Email == null) return;
            signsEmps.ForEach(e =>
            {
                var signEmployee = new EmployeeBll().Single(e);
                base.ListArgs.Add(new EmailFormEventArgs()
                {
                    To = new Dictionary<string, string> { { signEmployee.Email, signEmployee.Account } },
                    Subject = "表单催签通知[" + base.Form.FormType.FormName + "]",
                    NickName = signEmployee.Account,
                    Title = "表单催签通知[" + base.Form.FormType.FormName + "]",
                    From = "MINICUT",
                    Content = "<p>系统中有一张表单已被申请人 <b>" + base.Form.BaseEmployee.Account + "</b> 催签：" + base.Form.FormNo + "</p>",
                    Date = DateTime.Now,
                    Link = $"进入系统处理表单<a href='{HostUrl}{"/eForm/Signing/Index"}' >进入系统</a>"
                });
            });
        }


    }
}
