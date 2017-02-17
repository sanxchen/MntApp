using System.Collections.Generic;
using System.Web.Mvc;

using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.eForm.Controllers
{
    public class FormTypeController : PublicController<FormType>
    {
        // GET: eForm/FormType


        /// <summary>
        /// 【模块管理】返回树JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJsonFormClass()
        {
            List<FormClass> list = DataFactory.Database().FindList<FormClass>();
            List<TreeEntity> TreeList = new List<TreeEntity>();
            foreach (FormClass item in list)
            {
                TreeEntity tree = new TreeEntity();
                bool hasChildren = false;
                List<FormClass> childnode = list.FindAll(t => t.ParentId == item.FormClassId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                tree.id = item.FormClassId;
                tree.text = item.FullName;
                tree.value = item.FormClassId;
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = hasChildren;
                tree.parentId = item.ParentId;
                //tree.img = item.Icon != null ? "/Content/Images/Icon16/" + item.Icon : item.Icon;
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }

    }
}