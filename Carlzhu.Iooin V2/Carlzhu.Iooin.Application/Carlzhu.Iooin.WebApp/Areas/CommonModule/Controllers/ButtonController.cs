using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 按钮管理控制器
    /// </summary>
    public class ButtonController : PublicController<BaseButton>
    {
        public BaseModuleBll BaseModulebll = new BaseModuleBll();
        readonly BaseButtonBll _baseButtonbll = new BaseButtonBll();
        /// <summary>
        /// 加载模块目录
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseModule> list = BaseModulebll.GetList();
            List<TreeEntity> treeList = new List<TreeEntity>();
            foreach (BaseModule item in list)
            {
                string moduleId = item.ModuleId;
                bool hasChildren = false;
                List<BaseModule> childnode = list.FindAll(t => t.ParentId == moduleId);
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
                    if (item.AllowButton != 1)
                        continue;
                TreeEntity tree = new TreeEntity
                {
                    id = moduleId,
                    text = item.FullName,
                    value = moduleId,
                    isexpand = true,
                    complete = true,
                    hasChildren = hasChildren,
                    parentId = item.ParentId,
                    img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon
                };
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 加载按钮 （返回树Json）
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="category">分类：1-工具栏，2：右击栏</param>
        /// <returns></returns>
        public ActionResult ButtonTreeJson(string moduleId, string category)
        {
            List<BaseButton> list = _baseButtonbll.GetList(moduleId, category);
            List<TreeEntity> treeList = new List<TreeEntity>();
            foreach (BaseButton item in list)
            {
                string buttonId = item.ButtonId;
                bool hasChildren = false;
                List<BaseButton> childnode = list.FindAll(t => t.ParentId == buttonId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                TreeEntity tree = new TreeEntity
                {
                    id = buttonId,
                    text = item.FullName,
                    value = item.Code,
                    isexpand = true,
                    complete = true,
                    hasChildren = hasChildren,
                    parentId = item.ParentId,
                    img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon
                };
                treeList.Add(tree);
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 【模块按钮管理】返回公司列表JONS
        /// </summary>
        /// <param name="moduleId">模块ID</param>
        /// <param name="category">分类：1-工具栏，2：右击栏</param>
        /// <returns></returns>
        public ActionResult TreeGridListJson(string moduleId, string category)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(moduleId))
            {
                List<BaseButton> listData = _baseButtonbll.GetList(moduleId, category);
                sb.Append("{ \"rows\": [");
                sb.Append(TreeGridJson(listData, -1));
                sb.Append("]}");
            }
            return Content(sb.ToString());
        }
        int lft = 1, rgt = 1000000;
        public string TreeGridJson(List<BaseButton> listData, int index, string parentId = "0")
        {
            StringBuilder sb = new StringBuilder();
            List<BaseButton> childNodeList = listData.FindAll(t => t.ParentId == parentId);
            if (childNodeList.Count > 0) { index++; }
            foreach (BaseButton entity in childNodeList)
            {
                string strJson = entity.ToJson();
                strJson = strJson.Insert(1, "\"ButtonName\":\"" + entity.FullName + "\",");
                strJson = strJson.Insert(1, "\"level\":" + index + ",");
                strJson = strJson.Insert(1, "\"isLeaf\":" + (listData.Count<BaseButton>(t => t.ParentId == entity.ButtonId) == 0 ? true : false).ToString().ToLower() + ",");
                strJson = strJson.Insert(1, "\"expanded\":true,");
                strJson = strJson.Insert(1, "\"lft\":" + lft++ + ",");
                strJson = strJson.Insert(1, "\"rgt\":" + rgt-- + ",");
                sb.Append(strJson);
                sb.Append(TreeGridJson(listData, index, entity.ButtonId));
            }
            return sb.ToString().Replace("}{", "},{");
        }
        /// <summary>
        /// 【模块按钮管理】返回对象JSON
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SetFormControl(string keyValue)
        {
            BaseButton entity = Repositoryfactory.Repository().FindEntity(keyValue);
            string jsonData = entity.ToJson();
            jsonData = jsonData.Insert(1, "\"ParentName\":\"" + Repositoryfactory.Repository().FindEntity(entity.ParentId).FullName + "\",");
            return Content(jsonData);
        }
    }
}
