using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.QUALITY;
using Carlzhu.Iooin.Framework.Data.DataAccess.DataBase;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Controllers
{
    public class HomeController : Controller
    {
        public BaseModuleBll BaseModulebll = new BaseModuleBll();
        readonly BaseModulePermissionBll _baseModulepermissionbll = new BaseModulePermissionBll();
        /// <summary>
        /// ��ʼ��ҳ��
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return RedirectToAction("Index", "Login");
        }
        /// <summary>
        /// ����ģ�飬д��ϵͳ�˵�Id
        /// </summary>
        /// <param name="ModuleId">ģ��id</param>
        /// <param name="moduleName">ģ������</param>
        /// <returns></returns>
        public ActionResult SetModuleId(string ModuleId, string moduleName)
        {
            string moduleId = DESEncrypt.Encrypt(ModuleId);
            WebHelper.WriteCookie("ModuleId", moduleId);
            if (!string.IsNullOrEmpty(moduleName))
            {
                BaseSysLogBll.Instance.WriteLog(ModuleId, OperationType.Visit, "1", moduleName);
            }
            return Content(moduleId);
        }
        /// <summary>
        /// �뿪tab�¼�
        /// </summary>
        /// <param name="moduleId">ģ��id</param>
        /// <param name="moduleName">ģ������</param>
        /// <returns></returns>
        public ActionResult SetLeave(string moduleId, string moduleName)
        {
            BaseSysLogBll.Instance.WriteLog(moduleId, OperationType.Leave, "1", moduleName);
            return Content(moduleId);
        }

        #region ��̨��ҳ-��ʼ�˵�
        /// <summary>
        /// ��ʼ�˵�UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult StartIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "��" + ManageProvider.Provider.Current().UserName + "��";
            return View();
        }
        /// <summary>
        /// ��ʼ-��ӭ��ҳ
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult StartPanel()
        {
            return View();
        }
        /// <summary>
        /// ���ؿ�ʼ�˵�
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadStartMenu()
        {
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<BaseModule> list = _baseModulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            return Content(Util.Json.ToJson(list).Replace("&nbsp;", ""));
        }
        #endregion

        #region ��̨��ҳ-�ַ��ٲ˵�
        /// <summary>
        /// �ַ���UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult AccordionIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "��" + ManageProvider.Provider.Current().UserName + "��";
            return View();
        }
        /// <summary>
        /// �ַ���-��ӭ��ҳ
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult AccordionPage()
        {
            return View();
        }
        /// <summary>
        /// �����ַ��ٲ˵�
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadAccordionMenu()
        {
            string objectId = ManageProvider.Provider.Current().ObjectId;
            List<BaseModule> list = _baseModulepermissionbll.GetModuleList(objectId).FindAll(t => t.Enabled == 1);
            return Content(Util.Json.ToJson(list).Replace("&nbsp;", ""));
        }
        #endregion

        #region ��̨��ҳ-�������˵�
        /// <summary>
        /// ������UI
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult TreeIndex()
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "��" + ManageProvider.Provider.Current().UserName + "��";
            return View();
        }
        /// <summary>
        /// ������-��ӭ��ҳ
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult TreePage()
        {
            return View();
        }
        /// <summary>
        /// �����������˵�
        /// </summary>
        /// <returns></returns>
        public ActionResult LoadTreeMenu(string ModuleId)
        {
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<BaseModule> list = _baseModulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            foreach (BaseModule item in list)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                List<BaseModule> childnode = list.FindAll(t => t.ParentId == item.ModuleId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                if (item.Category == "ҳ��")
                {
                    tree.Attribute = "Location";
                    tree.AttributeValue = item.Location;
                }
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.isexpand = false;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson(ModuleId));
        }
        #endregion

        #region ��ݷ�ʽ����
        /// <summary>
        /// ��ݷ�ʽ����
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult Shortcuts()
        {
            return View();
        }
        /// <summary>
        /// ��ݷ�ʽ ���ز˵�ģ����JSON
        /// </summary>
        /// <returns></returns>
        public ActionResult ShortcutsModuleTreeJson()
        {
            BaseShortcutsBll Baseshortcutsbll = new BaseShortcutsBll();
            string UserId = ManageProvider.Provider.Current().UserId;
            List<BaseModule> ShortcutList = Baseshortcutsbll.GetShortcutList(UserId);
            string ObjectId = ManageProvider.Provider.Current().ObjectId;
            List<BaseModule> list = _baseModulepermissionbll.GetModuleList(ObjectId).FindAll(t => t.Enabled == 1);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            foreach (BaseModule item in list)
            {
                TreeEntity tree = new TreeEntity();
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                if (item.Category == "ҳ��")
                {
                    tree.checkstate = ShortcutList.FindAll(t => t.ModuleId == item.ModuleId).Count == 0 ? 0 : 1;
                    //tree.checkstate = item["objectid"].ToString() != "" ? 1 : 0;
                    tree.showcheck = true;
                }
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = list.FindAll(t => t.ParentId == item.ModuleId).Count > 0 ? true : false;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// ��ݷ�ʽ�б���JSON
        /// </summary>
        /// <returns></returns>
        public ActionResult ShortcutsListJson()
        {
            BaseShortcutsBll Baseshortcutsbll = new BaseShortcutsBll();
            string UserId = ManageProvider.Provider.Current().UserId;
            List<BaseModule> ShortcutList = Baseshortcutsbll.GetShortcutList(UserId);
            return Content(Util.Json.ToJson(ShortcutList));
        }
        /// <summary>
        /// ��ݷ�ʽ���� �ύ����
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        public ActionResult SubmitShortcuts(string moduleId)
        {
            try
            {
                BaseShortcutsBll Baseshortcutsbll = new BaseShortcutsBll();
                string UserId = ManageProvider.Provider.Current().UserId;
                int IsOk = Baseshortcutsbll.SubmitForm(moduleId, UserId);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "���óɹ���" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
        #endregion

        #region ����֧��
        /// <summary>
        /// ����֧��
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult SupportPage()
        {
            return View();
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult AboutPage()
        {
            return View();
        }
        #endregion

        #region ���Ի�Ƥ������
        /// <summary>
        /// ���Ի�Ƥ������
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public ActionResult SkinIndex()
        {
            return View();
        }
        /// <summary>
        /// �л�����
        /// </summary>
        /// <param name="UItheme"></param>
        /// <returns></returns>
        public ActionResult SwitchTheme(string UItheme)
        {
            WebHelper.WriteCookie("UItheme", UItheme, 43200);
            return Content("1");
        }

        public ActionResult SwitchFooter(string footer)
        {
            ViewBag.Account = ManageProvider.Provider.Current().Account + "��" + ManageProvider.Provider.Current().UserName + "��";
            return PartialView(footer);
        }

        #endregion



        public void Test()
        {
            var datatable = Util.Offices.ExcepNpoi.ExcelToDataTable(@"D:\abc.xls", "TT", 0);

            DateTime minTime = DateTime.Parse("1999/01/01");

            List<Apparatus> apparatuses = new List<Apparatus>();


            foreach (DataRow dataRow in datatable.Rows)
            {
                if(dataRow[0].ToString()== "���CODE") continue;

                apparatuses.Add(new Apparatus()
                {
                    //MntNo = dataRow[0].ToString(),
                    //Name = dataRow[1].ToString(),
                    //Model = dataRow[2].ToString(),
                    //CalDate = dataRow[3].ToString(),
                    //CalResult = dataRow[4].ToString(),
                    //CertificateResults = dataRow[5].ToString(),
                    //CalCircle = dataRow[6].ToString(),
                    //NextCalDate = dataRow[7].ToString(),
                    //CalType = dataRow[8].ToString(),
                    //UseEmp = dataRow[9].ToString(),
                    //UseAdd = dataRow[10].ToString(),
                    
                });
            }

            DataFactory.Database().Insert(apparatuses);

            Console.Write(datatable);

        }
    }
}
