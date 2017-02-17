using System;
using System.IO;
using System.Web;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Entity
{
    public class DownloadEventArgs : EventArgs
    {
        public string Md5 { get; set; }
    }

    public delegate DownLoad DownLoadEventHander(object sender, DownloadEventArgs e);

    public class DownLoad : Files
    {
        public event DownLoadEventHander DownloadFunc;
        protected virtual DownLoad OnDownloadFunc(DownloadEventArgs e)
        {
            DownLoadEventHander handler = DownloadFunc;
            return handler?.Invoke(this, e);
        }

        public void Down(DownloadEventArgs e)
        {
            Console.WriteLine(BaseHelper.IsRemote);
            

            var file = this.OnDownloadFunc(e);
            try
            {
                HttpContext.Current.Response.ContentType = string.IsNullOrEmpty(file.ContentType) ? "application/x-excel" : file.ContentType;
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(file.FileName));
                HttpContext.Current.Response.WriteFile(Path.GetFullPath(file.FilePath));
            }
            catch (Exception)
            {
                throw new Exception("文件下载失败....");
            }
        }
    }
}
