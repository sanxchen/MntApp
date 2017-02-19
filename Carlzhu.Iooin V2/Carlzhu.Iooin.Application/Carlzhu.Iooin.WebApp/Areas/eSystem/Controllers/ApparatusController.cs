using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Business.QualityModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;

namespace Carlzhu.Iooin.WebApp.Areas.eSystem.Controllers
{
    public class ApparatusController : PublicController<Apparatus>
    {
        // GET: eSystem/Apparatus

        public ActionResult SendMail()
        {

            List<Apparatus> apparatuses = base.Repositoryfactory.Repository().FindList($"AND CALTYPE==1 AND  NEXTCALDATE<'{DateTime.Now.AddDays(-10)}' ");
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<tr><th>仪器名称</th><th>仪器编号</th><th>使用人</th><th>位置</th><th>下次效验时间</th><th>使用状态</th></tr>");
            foreach (var apparatuse in apparatuses)
            {
                sb.AppendLine($"<tr><td>{apparatuse.Name}</td><td>{apparatuse.MntNo}</td><td>{apparatuse.UseEmp}</td><td>{apparatuse.UseAdd}</td><td>{apparatuse.NextCalDate}</td><td>{apparatuse.Status}</td></tr>");
            }

            var template = BaseHelper.GetTemplatePage("/Resource/Template/Cal.html");

            var email = new Email
            {
                To = new Dictionary<string, string> { { "dcc10@minicut.com.cn", "Betty.lu" } },
                Cc = new Dictionary<string, string> { { "carl.zhu@minicut.com.cn", "Carl.zhu" } },
                Bcc = null,
                Body = template.Replace("@tr", sb.ToString()).Replace("@count", apparatuses.Count.ToString()).Replace("@date", DateTime.Now.ToString("yyyy-MM-dd")),
                Subject = $"[仪器到期校验通知！]-{DateTime.Now.Date}"
            };

            return Json(TemplateBll.Send("Bpm", email.To, email.Cc, email.Bcc, email.Subject, email.Body, null, null) ? new { Code = "1", Message = "邮件发送成功" } : new { Code = "-1", Message = "邮件发送失败" });
        }
    }
}