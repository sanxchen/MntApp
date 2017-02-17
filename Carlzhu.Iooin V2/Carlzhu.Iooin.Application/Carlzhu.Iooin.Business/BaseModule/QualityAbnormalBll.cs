using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;


using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;
using Carlzhu.Iooin.Util.MvcHtml;

namespace Carlzhu.Iooin.Business.BaseModule
{
    public class QualityAbnormalBll : TemplateBll
    {
        readonly FormNcr _ncr;
        private readonly string _content;
        private readonly string _path;

        private readonly Dictionary<string, string> _toAll = new Dictionary<string, string> { { "ywb@minicut.com.cn", "业务部" }, { "cgk@minicut.com.cn", "采购部" }, { "gcb@minicut.com.cn", "工程部" }, { "product@minicut.com.cn", "生产部" }, { "pzb@minicut.com.cn", "品质部" }, { "equipment@minicut.com.cn","生产技术部" } };
        private readonly Dictionary<string, string> _cc = new Dictionary<string, string> { { "King.he@minicut.com.cn", "King.he (何华军)" }, { "eric.leu@minicut.com.cn", "Eric.leu (吕理桢)" }, };

        private readonly string _formMsg;

        public QualityAbnormalBll(string formNo, string empNo)
            : base(formNo, empNo)
        {

            //_toAll = new Dictionary<string, string>() { { "8707008@qq.com", "Lu.zhu" } };
            //_cc = new Dictionary<string, string>() { { "Admin@mtapu.com", "Admin" } };
            _ncr = new BaseServices<FormNcr>().LoadEntities(c => c.FormNo == formNo).First();
            _path =
                $"<a href='{base.HostUrl}{$"/eSystem/Abnormal/AForm?p={_ncr.FormNo.Encrypt()}"}' >查看异常</a>";
            _formMsg =
                $"({EnumUtil.GetEnumShowName(typeof (FormNcr.AbnormalPointEnum), _ncr.AbnormalPoint)}/{_ncr.CatchTime.ToString("yyyyMMdd")}/{_ncr.PartNo})";



            _content = BaseHelper.GetTemplatePage("/Resource/Template/ncr.html")
                .Replace("@partno", _ncr.PartNo)
                .Replace("@customer", _ncr.Customer.CustomerName)
                .Replace("@catchtime", _ncr.CatchTime.ToShortDateString())
                .Replace("@batchno", _ncr.BatchNo.ToString(CultureInfo.InvariantCulture))
                .Replace("@samplingno", _ncr.SamplingNo.ToString(CultureInfo.InvariantCulture))
                .Replace("@defectno", _ncr.DefectsNo.ToString(CultureInfo.InvariantCulture))
                .Replace("@defect", $"{((_ncr.DefectsNo/_ncr.SamplingNo)*100).ToString("F1")}%")
                .Replace("@point", EnumUtil.GetEnumShowName(typeof(FormNcr.AbnormalPointEnum), _ncr.AbnormalPoint))
                .Replace("@user", _ncr.BaseEmployee.RealName)
                .Replace("@mark", _ncr.QualityDescription);
        }






        public void NoticeA()
        {
            ListArgs.Add(new EmailFormEventArgs
            {
                To = _toAll,
                Cc = _cc,
                // Subject = string.Format("品质异常通知-请尽快回复临时措施-{0}", _formMsg),

                Subject = $"[{_ncr.PartNo}] 发生品质异常({DateTime.Now.ToString("MM/dd")})",
                NickName = "All",
                Title = $"料号:{_ncr.PartNo}",
                From = "BPM",
                Content = $"<p>以下为新发生的品质异常，请相关人员及时回复临时措施!!!{_path}</p><div>{_content}</div>",
                Date = DateTime.Now,
                Link = $"点击进入系统{_path}"
            });
        }


        public void NoticeB(string emp, string msg)
        {
            ListArgs.Add(new EmailFormEventArgs
            {
                To = _toAll,
                Cc = _cc,
                Subject = $"[{_ncr.PartNo}] 品质异常[临时措施]",
                //Subject = string.Format("品质异常通知-请尽快给出不良品原因分析-{0}", _formMsg),
                NickName = "All",
                Title = $"料号:{_ncr.PartNo}",
                From = "BPM",
                Content = string.Format(
                "<p><b style='color:red'>{0}</b>已回复临时措施:<br>{1}</p>" +
                "<p>请相关人员尽快给出不良原因分析。{3}</p>" +
                "<div>{2}</div>"
                , emp, msg, _content, _path),
                Date = DateTime.Now,
                Link = $"点击进入系统{_path}"
            });
        }

        public void NoticeC(string emp, string msg)
        {
            ListArgs.Add(new EmailFormEventArgs
            {
                To = _toAll,
                Cc = _cc,
                Subject = $"[{_ncr.PartNo}] 品质异常[原因分析]",
               // Subject = string.Format("品质异常通知-请尽快给出改善对策及预防措施-{0}", _formMsg),
                NickName = "All",
                Title = $"料号:{_ncr.PartNo}",
                From = "BPM",
                Content = string.Format(
                "<p><b style='color:red'>{0}</b>已回复不良品原因分析:<br>{1}</p>" +
                "<p>请相关人员尽快给出改善对策及预防措施。{3}</p>" +
                "<div>{2}</div>"
                , emp, msg, _content, _path),
                Date = DateTime.Now,
                Link = $"点击进入系统{_path}"
            });
        }

        public void NoticeD(string emp, string msg)
        {
            ListArgs.Add(new EmailFormEventArgs
            {
                To = new Dictionary<string, string> { { base.Form.BaseEmployee.Email, base.Form.BaseEmployee.Account } },
                Cc = _cc,
                Subject = $"[{_ncr.PartNo}] 品质异常[改善对策及预防措施]",
                //Subject = string.Format("品质异常通知-请尽快进行结果确认-{0}", _formMsg),
                NickName = base.Form.BaseEmployee.Account,
                Title = $"料号:{_ncr.PartNo}",
                From = "BPM",
                Content = string.Format(
                "<p><b style='color:red'>{0}</b>已回复改善对策及预防措施:<br>{1}</p>" +
                "<p>请尽快进行结果确认。{3}</p>" +
                "<div>{2}</div>"
                , emp, msg, _content, _path),
                Date = DateTime.Now,
                Link = $"点击进入系统{_path}"
            });
        }

        public void NoticeE(string emp, string msg)
        {
            ListArgs.Add(new EmailFormEventArgs
            {
                To = _toAll,
                Cc = _cc,
                Subject = $"[{_ncr.PartNo}] 品质异常[结案]",
                //Subject = string.Format("品质异常通知-结案-{0}", _formMsg),
                NickName = "All",
                Title = $"料号:{_ncr.PartNo}",
                From = "BPM",
                Content = string.Format(
                "<p>此异常已被<b style='color:red'>{0}</b>结案，以下为结案信息，请知悉。</p>" +
                "<p>{1}</p>" +
                "<p>点击查看完整异常信息。{3}</p>" +
                "<div>{2}</div>",
                emp, msg, _content, _path),
                Date = DateTime.Now,
                Link = $"点击进入系统{_path}"
            });
        }
    }
}
