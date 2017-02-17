
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// ��ͼ���ÿ�����
    /// </summary>
    public class ViewController : PublicController<BaseView>
    {
        private BaseModuleBll Basemodulebll = new BaseModuleBll();
        private BaseViewBll Baseviewbll = new BaseViewBll();
        private BaseViewWhereBll Baseviewwherebll = new BaseViewWhereBll();
        /// <summary>
        /// ����ͼ���á�ģ��Ŀ¼
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseModule> list = Basemodulebll.GetList();
            List<TreeEntity> TreeList = new List<TreeEntity>();
            foreach (BaseModule item in list)
            {
                string ModuleId = item.ModuleId;
                bool hasChildren = false;
                List<BaseModule> childnode = list.FindAll(t => t.ParentId == ModuleId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                else
                {
                    if (item.Category == "Ŀ¼")
                    {
                        continue;
                    }
                }
                if (item.Category == "ҳ��")
                    if (item.AllowView != 1)
                        continue;
                TreeEntity tree = new TreeEntity();
                tree.id = ModuleId;
                tree.text = item.FullName;
                tree.value = ModuleId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// ����ͼ���á����ر��JONS
        /// </summary>
        /// <param name="CompanyId">ģ������</param>
        /// <returns></returns>
        public ActionResult GridListJson(string ModuleId)
        {
            if (!string.IsNullOrEmpty(ModuleId))
            {
                List<BaseView> ListData = Baseviewbll.GetViewList(ModuleId);
                var JsonData = new
                {
                    rows = ListData,
                };
                return Content(JsonData.ToJson());
            }
            return null;
        }
        /// <summary>
        /// ����ͼ���á���ʾ�����ֶΡ������б�JSON
        /// </summary>
        /// <param name="ModuleId">ģ������</param>
        /// <returns></returns>
        public JsonResult GetViewJson(string ModuleId)
        {
            List<BaseView> list = Baseviewbll.GetViewList(ModuleId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ����ͼ���á���ѯ�����ֶΡ������б�JSON
        /// </summary>
        /// <param name="ModuleId">ģ������</param>
        /// <returns></returns>
        public JsonResult GetViewWhereJson(string ModuleId)
        {
            List<BaseViewWhere> list = Baseviewwherebll.GetViewWhereList(ModuleId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ����ͼ���á����ύ�¼�
        /// </summary>
        /// <param name="KeyValue">����ֵ</param>
        /// <param name="ModuleId">ģ��Id</param>
        /// <param name="ViewJson">��ͼJson</param>
        /// <param name="ViewWhereJson">��ͼ����Json</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ViewSubmitForm(string KeyValue, string ModuleId, string ViewJson, string ViewWhereJson)
        {
            try
            {
                int IsOk = 0;
                IsOk = Baseviewbll.SubmitForm(KeyValue, ModuleId, ViewJson, ViewWhereJson);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "�����ɹ���" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�����" + ex.Message }.ToString());
            }
        }
    }
}