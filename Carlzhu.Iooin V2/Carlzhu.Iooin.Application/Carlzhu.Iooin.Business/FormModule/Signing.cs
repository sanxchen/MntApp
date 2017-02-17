using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.BaseUtility;

using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Util.Extension;


namespace Carlzhu.Iooin.Business.FormModule
{
    public class Signing
    {
        private readonly Forms _signing = new Forms();


        /// <summary>
        /// 根据登陆人工号取出待签核的表单，包括自己的和代签的
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<FormSign> GetSignDataList(string empNo)
        {
            return _signing.GetSignDataList(empNo).Distinct(new PropertyComparer<FormSign>("FormNo")).ToList();
        }


        public static List<FormSign> GetSigns(Func<FormSign, bool> lambda)
        {
            return new BaseServices<FormSign>().LoadEntities(lambda).ToList();
        }


        /// <summary>
        /// 更新签核的表单标记
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tags"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsUpdateTags(int item, string tags, out int tag)
        {
            tag = int.Parse(tags.Substring(4));

            tag = tag >= 2 ? 0 : tag + 1;

            return _signing.IsUpdateTags(item, tag);

        }

        /// <summary>
        /// 转签表单，转签表单号，转签记录，转签至工号，转签理由，转签人
        /// </summary>
        /// <param name="formNo">转签表单号</param>
        /// <param name="item">转签记录</param>
        /// <param name="redirectempno">转签至工号</param>
        /// <param name="reason">转签理由</param>
        /// <param name="empNo">转签人</param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool IsRedirectSign(string formNo, int item, string redirectempno, string reason, string empNo, out string msg)
        {
            if (!_signing.IsRedirectSign(formNo, item, redirectempno, reason, empNo, out msg)) return false;

            //发送邮件
            var emailForm = new BpmBll(formNo, empNo);
            emailForm.RedirectSigner(redirectempno);

            TemplateBll.SendBpm(emailForm);



            return true;
        }


        /// <summary>
        /// 加签
        /// </summary>
        /// <param name="formNo">表单号</param>
        /// <param name="item">记录号</param>
        /// <param name="addempno">加签至工号</param>
        /// <param name="reason">理由</param>
        /// <param name="direction">方向，前，平，后</param>
        /// <param name="empNo">加签人</param>
        /// <returns></returns>
        public bool AddSign(string formNo, int item, string addempno, string reason, int direction, string empNo, out string msg)
        {
            msg = "加签失败";
            lock (new object())
            {
                if (!
                     new BaseServices<Form>().LoadEntities(c => c.FormNo.Equals(formNo))
                        .First()
                        .FormType.IsAdd)
                {
                    msg = "此表单不充许加签";
                    return false;
                }

                bool result;

                switch (direction)
                {
                    case -1:
                        result = _signing.IsAddSignBefore(formNo, item, addempno, reason, empNo);
                        break;
                    case 0:
                        result = _signing.IsAddSignParallel(formNo, item, addempno, reason, empNo);
                        break;
                    default:
                        result = _signing.IsAddSignAfter(formNo, item, addempno, reason, empNo);
                        break;
                }

                if (!result) return false;

                var emailForm = new BpmBll(formNo, empNo);
                emailForm.AddSigner(addempno, direction);
                TemplateBll.SendBpm(emailForm);

                msg = "加签成功";
                return true;
            }
        }


        /// <summary>
        /// 根据表单号提取签核记录，包括生成的和未生成的
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public List<FormSign> GetSignRecoredsByFormNo(string formNo)
        {
            return _signing.GetSignRecoredsByFormNo(formNo);
        }


        public List<FormSign> GetCurrentSignEmp(string formNo)
        {
            return new BaseServices<FormSign>().LoadEntities(
                         c =>
                             c.FormNo == formNo &&
                             c.SignResult == (int)FormSign.SignResultEnum.Watting).ToList();
        }


        /// <summary>
        /// 取出本站签核意见
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string GetSignMarkByItemAndFormNo(string formNo, int item)
        {
            return new BaseServices<FormSign>().LoadEntities(c => c.RowId == item && c.FormNo == formNo).First().SignMark;
        }

        /// <summary>
        /// 异常申请单各回复
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public static string GetSignMarkByItemAndFormNo(string formNo)
        {
            if (string.IsNullOrEmpty(formNo)) return "";
            var formSign = new BaseServices<FormSign>().LoadEntities(c => c.FormNo == formNo).ToList();
            StringBuilder sb = new StringBuilder();
            formSign.ForEach(c => sb.Append("<p>" + c.BaseEmployee.Account + ":" + c.SignMark + "</p>"));
            return sb.ToString();
        }





        /// <summary>
        /// 更新本站签核意见
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public bool IsUpdateSignMarkByItemAndFormNo(string formNo, int item, string mark)
        {
            return _signing.IsUpdateSignMarkByItemAndFormNo(formNo, item, mark);
        }

        /// <summary>
        /// 获取签核进度根据表单号
        /// </summary>
        /// <param name="formNo"></param>
        /// <returns></returns>
        public int SignProgress(string formNo)
        {
            List<FormSign> rs = _signing.GetSignRecoredsByFormNo(formNo);

            return (int)(((double)rs.Count(c => !string.IsNullOrEmpty(c.ActualSignEmpNo)) / rs.Count) * 100);
          
        }

        /// <summary>
        /// 同意表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Agree(string formNo, int item, string empNo)
        {

            //首先确认是否有权限签核；
            if (this.GetSignDataList(empNo).All(c => c.FormNo != formNo))
            {
                return false;
            }

            var formType = new BaseServices<Form>().LoadEntities(c => c.FormNo == formNo).First().FormType;

            //是否需要结案
            var func = formType.IsColsed ? FormExtend.SetFormFinish(formType) : new Func<string, string, int, bool>((f, e, i) => true);

            string nextEmpNo;
            string addsignEmpNo;
            bool result = _signing.Agree(formNo, item, empNo, out nextEmpNo, out addsignEmpNo, func);


            if (!result) return false;
            //有邮件发送
            var emailForm = new BpmBll(formNo, empNo);
            //var emailForm = new EmailForm(formNo, empNo);

            if (string.IsNullOrEmpty(nextEmpNo))
            {
                emailForm.Finlish();
            }
            else if (!string.IsNullOrEmpty(addsignEmpNo))
            {
                //是加签的
                emailForm.AddSignerAfter(nextEmpNo, (int)FormSign.DirectEnum.After);
            }
            else
            {
                var signers = new List<string> { nextEmpNo };

                //正常下一位签核人
                emailForm.NoticeSigner(signers);
                emailForm.NoticeReplace(signers);
            }


           return TemplateBll.SendBpm(emailForm);

            
        }


        /// <summary>
        /// 否决表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Reject(string formNo, int item, string empNo)
        {
            //首先确认是否有权限签核；
            if (this.GetSignDataList(empNo).All(c => c.FormNo != formNo))
            {
                return false;
            }

            var result = _signing.Reject(formNo, item, empNo);

            if (result)
            {
                var emailForm = new BpmBll(formNo, empNo);
                emailForm.Reject();

                TemplateBll.SendBpm(emailForm);
            }

            return result;

        }

        /// <summary>
        /// 获取表单的上位签核人
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public string UpSignEmployee(string formNo, int item)
        {
            var employee = _signing.UpSignEmployee(formNo, item);
            if (employee != null)
                return employee.DepartmentId + "/" + employee.Account + " " + employee.RealName;
            return null;
        }
    }
}
