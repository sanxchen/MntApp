
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 表单附加属性 控制器
    /// </summary>
    public class FormLayoutController : PublicController<BaseFormAttribute>
    {



        BaseFormAttributeBll Baseformattributebll = new BaseFormAttributeBll();


        public ActionResult FormTest()
        {
            return View();
        }

        public ActionResult FormSubmit()
        {
            BaseDataBaseBll Basedatabasebll = new BaseDataBaseBll();


            StringBuilder sb = new StringBuilder();
            StringBuilder values = new StringBuilder("(");
            DataTable dt = Basedatabasebll.GetColumnList("TestTable");
            List<DbParameter> parameter = new List<DbParameter>();

            sb.Append("INSERT INTO TestTable (");
            foreach (DataRow dr in dt.Rows)
            {
                string filed = dr["column"].ToString();
                parameter.Add(DbFactory.CreateDbParameter($"@{filed}", Request.Form[$"Build_{filed}"]));
                sb.Append($"[{filed}],");
                values.Append($"@{filed},");
            }
            sb.Append(") values");
            values.Append(")");

            base.Repositoryfactory.Repository().ExecuteBySql(sb.Append(values).Replace(",)", ")"), parameter.ToArray());


            Console.WriteLine(parameter);


            return Content("xxxxxxxxxxxxxxxxxxx");
        }



        /// <summary>
        /// 【系统表单】模块目录
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            List<BaseModule> list = new BaseModuleBll().GetList();
            List<TreeEntity> TreeList = new List<TreeEntity>();
            foreach (BaseModule item in list)
            {
                string ModuleId = item.ModuleId;
                bool hasChildren = false;
                List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> childnode = list.FindAll(t => t.ParentId == ModuleId);
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
                    if (item.AllowForm != 1)
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
        /// 表单设计器
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Layout()
        {
            string ModuleId = Request["ModuleId"];
            string strhtml = Baseformattributebll.CreateBuildFormTable(2, ModuleId);
            ViewBag.BuildFormTable = strhtml;
            return View();
        }
    }
}
