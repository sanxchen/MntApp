using System.Web;
using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.BaseModule
{
    public class DownDetailsBll
    {

        private bool _isConnect = false;

        public DownLoad Chrome(object sender, DownloadEventArgs e)
        {
            return new DownLoad()
            {
                FileName = "Chrome_chs.exe",
                FilePath = @"\\192.168.0.4\Minicut\软件\办公软件\Chrome_chs.exe"
            };
        }

        public DownLoad PublishedTemplate(object sender, DownloadEventArgs e)
        {
            return new DownLoad
            {
                FileName = "导入模版.xls",
                FilePath = HttpContext.Current.Server.MapPath("/Resource/Template/ImportPublish.xls")
            };
        }

        public DownLoad Form(object sender, DownloadEventArgs e)
        {
            var file = new FilesBll().SignFiles(e.Md5);

            return new DownLoad()
            {
                FileName = file.FileName,

                FilePath = BaseHelper.UpPath + e.Md5,
                ContentType = file.ContentType,
            };
        }
    }
}
