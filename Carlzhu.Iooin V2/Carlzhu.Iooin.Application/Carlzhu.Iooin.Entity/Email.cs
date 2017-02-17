using System;
using System.Collections.Generic;

namespace Carlzhu.Iooin.Entity
{
    public class EmailFormEventArgs : EventArgs
    {

        public Dictionary<string, string> To { get; set; }
        public Dictionary<string, string> Cc { get; set; }
        public Dictionary<string, string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
        public string From { get; set; }
        public DateTime Date { get; set; }
        public string NickName { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }

    }

    public delegate bool SendFormEmailEventArgsHander(object sender, EmailFormEventArgs e);




    public class Email
    {
        public event SendFormEmailEventArgsHander SdEmail;

        protected virtual bool OnSdEmail(EmailFormEventArgs e)
        {
            SendFormEmailEventArgsHander handler = SdEmail;
            return handler != null && handler(this, e);
        }


        public Dictionary<string, string> To { get; set; }
        public Dictionary<string, string> Cc { get; set; }
        public Dictionary<string, string> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


        public bool Sending(EmailFormEventArgs e)
        {
            return this.OnSdEmail(e);
        }



    }
}
