using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Carlzhu.Iooin.Business.BaseModule;

using Carlzhu.Iooin.Entity;
using Carlzhu.Iooin.InteractiveAdapter;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;
using Files = Carlzhu.Iooin.Entity.FORM.Files;

//using FilesFileGroup = Base.FilesFileGroup;

namespace Carlzhu.Iooin.WebApp.Controllers
{
    public class DownloadController : Controller
    {
        //
        // GET: /Download/


        private readonly DownDetailsBll _details = new DownDetailsBll();


        public void Chrome()
        {
            var down = new DownLoad();
            down.DownloadFunc += _details.Chrome;
            down.Down(new DownloadEventArgs());
        }

        public void PublishedTemplate()
        {
            var down = new DownLoad();
            down.DownloadFunc += _details.PublishedTemplate;
            down.Down(new DownloadEventArgs());
        }


        /// <summary>
        /// 加密文件下载
        /// </summary>
        /// <param name="md5"></param>
        public void Form(string md5)
        {
            var down = new DownLoad();
            down.DownloadFunc += _details.Form;
            down.Down(new DownloadEventArgs { Md5 = md5.Decrypt() });
        }


        public JsonResult GetGroupFiles(string groupGuid)
        {

            List<Files> fs = new List<Files>();

            new Carlzhu.Iooin.Business.BaseModule.FilesFileGroupBll().GetFileListByGroupGuid(Guid.Parse(groupGuid)).ForEach(c => fs.Add(new Files { FileName = c.Files.FileName, Md5 = c.Files.Md5 }));


            var serializer = new JavaScriptSerializer();

            return Json(serializer.Serialize(fs));

        }




        public void Online(string md5, string method)
        {

            string d5 = md5.Decrypt();

            var file = new FilesBll().SignFiles(d5);

            string uppath = BaseHelper.UpPath;





            if (".pdf".Contains(file.FileType))
            {
                //打印与在线查看
                BaseHelper.ViewPdf(method == "print"
                    ? PdfHelper.PdfToStream(uppath + d5, true, null, "mjdrawadm", true, "表单文件",
                        "minicutdraw.png")
                    : PdfHelper.PdfToStream(uppath + d5, true, null, "mjdrawadm", false, "表单文件",
                        "minicutdraw.png"));

            }
            else
            {
                Form(md5);
                //   Link.ErrorBy(new Exception(string.Format("暂不支持{0}文件在线查看，请等待开发...", file.FileType)));
            }


        }


    }
}
