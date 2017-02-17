
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
    /// 视图设置控制器
    /// </summary>
    public class ViewController : PublicController<BaseView>
    {
        private BaseModuleBll Basemodulebll = new BaseModuleBll();
        private BaseViewBll Baseviewbll = new BaseViewBll();
        private BaseViewWhereBll Baseviewwherebll = new BaseViewWhereBll();
        /// <summary>
        /// 【视图设置】模块目录
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
                    if (item.Category == "目录")
                    {
                        continue;
                    }
                }
                if (item.Category == "页面")
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
        /// 【视图设置】返回表格JONS
        /// </summary>
        /// <param name="CompanyId">模块主键</param>
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
        /// 【视图设置》显示标题字段】返回列表JSON
        /// </summary>
        /// <param name="ModuleId">模块主键</param>
        /// <returns></returns>
        public JsonResult GetViewJson(string ModuleId)
        {
            List<BaseView> list = Baseviewbll.GetViewList(ModuleId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 【视图设置》查询条件字段】返回列表JSON
        /// </summary>
        /// <param name="ModuleId">模块主键</param>
        /// <returns></returns>
        public JsonResult GetViewWhereJson(string ModuleId)
        {
            List<BaseViewWhere> list = Baseviewwherebll.GetViewWhereList(ModuleId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 【视图设置】表单提交事件
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <param name="ModuleId">模块Id</param>
        /// <param name="ViewJson">视图Json</param>
        /// <param name="ViewWhereJson">视图条件Json</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult ViewSubmitForm(string KeyValue, string ModuleId, string ViewJson, string ViewWhereJson)
        {
            try
            {
                int IsOk = 0;
                IsOk = Baseviewbll.SubmitForm(KeyValue, ModuleId, ViewJson, ViewWhereJson);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
    }
}