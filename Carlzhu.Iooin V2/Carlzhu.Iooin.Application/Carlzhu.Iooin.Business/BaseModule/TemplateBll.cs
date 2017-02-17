using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Business.FormModule;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Util;
using EmailLog = Carlzhu.Iooin.Business.SysModule.EmailLog;

namespace Carlzhu.Iooin.Business.BaseModule
{
    public abstract class TemplateBll
    {
        public readonly Form Form;
        public readonly BaseEmployee Emp;

        public readonly List<EmailFormEventArgs> ListArgs = new List<EmailFormEventArgs>();
        public readonly string HostUrl = BaseHelper.GetLocalhost;


        /// <summary>
        /// 表单号，当前处理人
        /// </summary>
        /// <param name="formNo"></param>
        /// <param name="empNo"></param>
        protected TemplateBll(string formNo, string empNo)
        {
            Form = new Applying().GetFormByFormNo(formNo);
            Emp = new BaseEmployeeBll().Single(empNo);
        }

        public List<EmailFormEventArgs> GetArgses()
        {
            return ListArgs;
        }

        public static bool SendBpm(TemplateBll emailForm)
        {
            try
            {
                Task.Run(() =>
                {
                    var email = new Email();
                    email.SdEmail += email_SdEmailBpm;
                    emailForm.GetArgses().ForEach(c => email.Sending(c));
                });

            }
            catch (Exception exception)
            {
                Console.Write(exception.Message);
            }
            finally
            {
                
            }

            return true;
        }

        static bool email_SdEmailBpm(object sender, EmailFormEventArgs e)
        {
            var template = BaseHelper.GetTemplatePage("/Resource/Template/Form.html");
            var email = new Email
            {
                To = e.To,
                Cc = e.Cc,
                Bcc = e.Bcc,
                Body = template.Replace("@title", e.Title).Replace("@from", e.From).Replace("@date", e.Date.ToString("D")).Replace("@nickname", e.NickName).Replace("@content", e.Content).Replace("@link", e.Link),
                Subject = e.Subject
            };

            return Send("Bpm", email.To, email.Cc, email.Bcc, email.Subject, email.Body, null, null);
        }

        


        public static bool Send(string modelName, Dictionary<string, string> toDictionary, Dictionary<string, string> ccDictionary, Dictionary<string, string> bccDictionary, string subject, string body, string[] file, MemoryStream msFile)
        {

            if (!BaseHelper.IsEmail) return true;

            string sendMsg;
            bool result = SendMailBll.Send2(toDictionary, ccDictionary, bccDictionary, subject, body, file, msFile, out sendMsg);

            ccDictionary?.ToList().ForEach(c => toDictionary.Add(c.Key, c.Value));

            new EmailLog().AddEntity(new Entity.EmailLog
            {
                ModelName = modelName,
                To = string.Join(",", (toDictionary.ToList()).Select(c => c.Key + ";")),
                Subject = subject,
                Body = body,
                CreateTime = DateTime.Now,
                SendResult = result,
                Message = sendMsg
            });

            return result;
        }
    }
}
