
using System.Collections.Generic;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 模块管理控制器
    /// </summary>
    public class ModuleController : PublicController<BaseModule>
    {
        public BaseModuleBll Basemodulebll = new BaseModuleBll();

        /// <summary>
        /// 【模块管理】返回树JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseModule> list = Basemodulebll.GetList();
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
                tree.id = item.ModuleId;
                tree.text = item.FullName;
                tree.value = item.ModuleId;
                tree.isexpand = item.Isexpand == 1 ? true : false;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// 【模块管理】返回对象JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetFormControl(string KeyValue)
        {
            BaseModule entity = Repositoryfactory.Repository().FindEntity(KeyValue);
            string JsonData = entity.ToJson();
            JsonData = JsonData.Insert(1, "\"ParentName\":\"" + Basemodulebll.Repository().FindEntity(entity.ParentId).FullName + "\",");
            return Content(JsonData);
        }
    }
}
