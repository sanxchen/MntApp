using System;
using System.Collections.Generic;
using System.Linq;
using Carlzhu.Iooin.Business.BaseModule;
using Carlzhu.Iooin.Business.BaseUtility;

using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Util.Extension;


namespace Carlzhu.Iooin.Business.FormModule
{
    public class Tracking
    {
        /// <summary>
        /// 取得已经申请过的表单
        /// </summary>
        /// <param name="startTime">申请日期始</param>
        /// <param name="endTime">申请日期止</param>
        /// <param name="empNo">提交人工号</param>
        /// <returns></returns>
        public List<Form> GetApplyedFormList(DateTime startTime, DateTime endTime, string empNo)
        {
            return new BaseServices<Form>().LoadEntities(c => !c.IsDel && c.CreateTime > startTime && c.CreateTime < endTime && c.CreateEmpNo == empNo).ToList();
        }

        /// <summary>
        /// 取得已经签核过的表单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public IEnumerable<FormSign> GetSignedFormList(DateTime startTime, DateTime endTime, string empNo)
        {
            return new BaseServices<FormSign>().LoadEntities(
                   c => c.ActualSignEmpNo == empNo && c.SignTime > startTime && c.SignTime < endTime)
                   .OrderByDescending(m => m.SignTime)
                   .Distinct(new PropertyComparer<FormSign>("FormNo"))
                   .ToList();
        }




        #region ApplyOperator

        /// <summary>
        /// 申请人表单操作之崔签
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Urge(string formNo, string empNo)
        {
            //检查表单所有人
            var forms = new BaseServices<Form>().LoadEntities(c => c.CreateEmpNo == empNo && c.FormNo == formNo).FirstOrDefault();

            if (forms == null) return false;

            var signs = new BaseServices<FormSign>().LoadEntities(c => c.FormNo == formNo && c.SignResult == (int)FormSign.SignResultEnum.Watting);

            //崔签邮件
            if (signs.Any())
            {
                var signsEmp = new List<string>();
                signs.ToList().ForEach(c => signsEmp.Add(c.BaseEmployee.EmpNo));

                var emailForm = new Carlzhu.Iooin.Business.BaseModule.BpmBll(formNo, empNo);
                //var emailForm = new EmailForm(formNo, empNo);
                emailForm.Urge(signsEmp);
                emailForm.NoticeReplace(signsEmp);


                //var email = new Email();
                //email.SdEmail += new Mailtemp.SendMail().Bpm;
                //emailForm.GetArgses().ForEach(a => email.Sending(a));
                TemplateBll.SendBpm(emailForm);
            }



            return true;
        }

        /// <summary>
        /// 申请认操作之撤销表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Cancel(string formNo, string empNo)
        {
            return new Forms().Cancel(formNo, empNo);
        }

        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Delete(string formNo, string empNo)
        {
            return new Forms().Delete(formNo, empNo);
        }

        /// <summary>
        /// 申请人操作之送签表单
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public bool Send(string formNo, string empNo)
        {
            return new Applying().Send(formNo, empNo);
        }

        #endregion
    }
}
