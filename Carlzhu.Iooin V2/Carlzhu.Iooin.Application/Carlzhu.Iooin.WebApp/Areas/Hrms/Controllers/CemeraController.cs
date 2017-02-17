using System.Web;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class CemeraController : Controller
    {
        //
        // GET: /Hrms/Cemera/




        [LoginAuthorize]
        public ActionResult Pat(string uid)
        {
            //string uid = Account.GetLoginEmpNo(); ;
            string localhost = BaseHelper.GetLocalhost;
            var encodeLocalhost = HttpUtility.UrlEncode(localhost);
            var avatarFlashParam =
                $"{localhost}/plug/camera.swf?nt=1&inajax=1&appid=1&input={uid}&ucapi={encodeLocalhost}/hrms/Cemera/PatPost";
            ViewData["AvatarFlashParam"] = avatarFlashParam;
            return View();
        }


        public void PatPost()
        {
            const string path = "/upfile/employee/";
            var cemera = new CemeraLoad(path);

            var context = HttpContext;
            string uid = context.Request.QueryString["input"];
            if (!string.IsNullOrEmpty(context.Request["Filename"]) && !string.IsNullOrEmpty(context.Request["Upload"]))
            {
                cemera.ResponseText(cemera.UploadTempAvatar(uid));
            }
            if (string.IsNullOrEmpty(context.Request["avatar1"]) || string.IsNullOrEmpty(context.Request["avatar2"]) ||
                string.IsNullOrEmpty(context.Request["avatar3"]))
                return;
            cemera.CreateDir(uid);
            if (!(cemera.SaveAvatar("avatar1", uid) && cemera.SaveAvatar("avatar2", uid) && cemera.SaveAvatar("avatar3", uid)))
            {
                System.IO.File.Delete(CemeraLoad.GetMapPath(path + uid + ".jpg"));
                cemera.ResponseText("<?xml version=\"1.0\" ?><root><face success=\"0\"/></root>");
                return;
            }

            System.IO.File.Delete(CemeraLoad.GetMapPath(path + uid + ".jpg"));

            BaseEmployee employee = new BaseEmployeeBll().Single(uid);
            cemera.GenerationCard(Server.MapPath($"{path}{uid}/large.jpg"), employee.EmpNo, employee.BaseDepartment.FullName, employee.RealName);
            
            cemera.ResponseText("<?xml version=\"1.0\" ?><root><face success=\"1\"/></root>");
        }


      

    }
}
