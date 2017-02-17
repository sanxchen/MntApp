using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Util;

using EmailLog = Carlzhu.Iooin.Business.SysModule.EmailLog;

namespace Carlzhu.Iooin.Business.BaseModule
{
    public class SendMailBll
    {
        //public bool Bpm(object sender, EmailFormEventArgs e)
        //{
        //    var template = BaseHelper.GetTemplatePage("~/Template/Form.html");
        //    var email = new Email
        //    {
        //        To = e.To,
        //        Cc = e.Cc,
        //        Bcc = e.Bcc,
        //        Body = template.Replace("@title", e.Title).Replace("@from", e.From).Replace("@date", e.Date.ToString("D")).Replace("@nickname", e.NickName).Replace("@content", e.Content).Replace("@link", e.Link),
        //        Subject = e.Subject
        //    };

        //    return Send("Bpm", email.To, email.Cc, email.Bcc, email.Subject, email.Body, null, null);

        //}

        //public bool Pdm(object sender, EmailFormEventArgs e)
        //{
        //    var template = BaseHelper.GetTemplatePage("/Resource/Template/publish.html");
        //    var email = new Email
        //    {
        //        To = e.To,
        //        Cc = e.Cc,
        //        Bcc = e.Bcc,
        //        Body = template.Replace("@title", e.Title)
        //        .Replace("@from", e.From)
        //        .Replace("@date", e.Date.ToString("D"))
        //        .Replace("@nickname", e.NickName)
        //        .Replace("@content", e.Content)
        //        .Replace("@link", e.Link),
        //        Subject = e.Subject
        //    };
        //    return Send("PDM", email.To, email.Cc, email.Bcc, email.Subject, email.Body, null, null);

        //}


        //public static bool Send(string modelName, Dictionary<string, string> toDictionary, Dictionary<string, string> ccDictionary, Dictionary<string, string> bccDictionary, string subject, string body, string[] file, MemoryStream msFile)
        //{

        //    if (!BaseHelper.IsEmail) return true;

        //    string sendMsg;
        //    bool result =  Send2(toDictionary, ccDictionary, bccDictionary, subject, body, file, msFile, out sendMsg);
        

        //    ccDictionary?.ToList().ForEach(c => toDictionary.Add(c.Key, c.Value));

        //    new EmailLog().AddEntity(new Carlzhu.Iooin.Entity.EmailLog
        //    {
        //        ModelName = modelName,
        //        To = string.Join(",", (toDictionary.ToList()).Select(c => c.Key + ";")),
        //        Subject = subject,
        //        Body = body,
        //        CreateTime = DateTime.Now,
        //        SendResult = result,
        //        Message = sendMsg,
        //    });

        //    return result;
        //}



        public static bool Send2(Dictionary<string, string> toDictionary, Dictionary<string, string> ccDictionary, Dictionary<string, string> bccDictionary, string subject, string body, string[] file, MemoryStream msFile, out string sendMsg)
        {

            sendMsg = "正常发送";
            //if (config["switch"].Equals("off")) return false;

            bool result = false;


            try
            {
                //邮件发送时请确认服务的杀毒软件因素

                var client = new SmtpClient("smtp.minicut.com.cn")
                {
                    Timeout = 60000,
                    Credentials = new NetworkCredential("System@minicut.com.cn", "Mnt@1234"),
                    DeliveryMethod = SmtpDeliveryMethod.Network
                };

                //client.UseDefaultCredentials = false;
                var message = new MailMessage
                {
                    SubjectEncoding = Encoding.UTF8,
                    BodyEncoding = Encoding.UTF8,
                    From = new MailAddress("System@minicut.com.cn", "System", Encoding.UTF8)
                };

                if (file != null)
                    foreach (var s in file)
                    {
                        message.Attachments.Add(new Attachment(s));
                    }
                if (msFile != null)
                    message.Attachments.Add(new Attachment(msFile, "Test.pdf"));

                //添加收件人
                toDictionary.ToList().ForEach(c => message.To.Add(new MailAddress(c.Key, c.Value, Encoding.UTF8)));

                //添加抄送人
                ccDictionary?.ToList().ForEach(c => message.To.Add(new MailAddress(c.Key, c.Value, Encoding.UTF8)));

                //添加密送 
                bccDictionary?.ToList().ForEach(c => message.To.Add(new MailAddress(c.Key, c.Value, Encoding.UTF8)));
                message.Bcc.Add(new MailAddress("hello@iooin.com", "Admin"));



                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;


                client.Send(message);
                result = true;

            }
            catch (Exception exception)
            {
                sendMsg = exception.Message;
                result = false;
            }

            return result;
        }
    }
}
