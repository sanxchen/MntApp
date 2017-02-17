using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 部门管理控制器
    /// </summary>
    public class DepartmentController : PublicController<BaseDepartment>
    {
        readonly BaseDepartmentBll _baseDepartmentbll = new BaseDepartmentBll();
        /// <summary>
        /// 【部门管理】返回 公司、部门 树JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson()
        {
            DataTable dt = _baseDepartmentbll.GetTree();
            List<TreeEntity> treeList = new List<TreeEntity>();
            if (!DataHelper.IsExistRows(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    string departmentId = row["departmentid"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(dt, "parentid='" + departmentId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity
                    {
                        id = departmentId,
                        text = row["fullname"].ToString(),
                        value = row["code"].ToString(),
                        parentId = row["parentid"].ToString(),
                        Attribute = "Type",
                        AttributeValue = row["sort"].ToString(),
                        AttributeA = "CompanyId",
                        AttributeValueA = row["companyid"].ToString(),
                        isexpand = true,
                        complete = true,
                        hasChildren = hasChildren
                    };
                    if (row["parentid"].ToString() == "0")
                    {
                        tree.img = "/Content/Images/Icon16/molecule.png";
                    }
                    else if (row["sort"].ToString() == "Company")
                    {
                        tree.img = "/Content/Images/Icon16/hostname.png";
                    }
                    else if (row["sort"].ToString() == "Department")
                    {
                        tree.img = "/Content/Images/Icon16/chart_organisation.png";
                    }
                    treeList.Add(tree);
                }
            }
            return Content(treeList.TreeToJson());
        }


        /// <summary>
        /// 【部门管理】返回 公司、部门 树JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeJson2()
        {
            List<BaseDepartment> list = _baseDepartmentbll.GetList();
            List<TreeEntity> treeList = new List<TreeEntity>();
            foreach (BaseDepartment item in list)
            {
                bool hasChildren = false;
                IList childnode = list.FindAll(t => t.ParentId == item.ParentId);
                if (childnode.Count > 0)
                {
                    hasChildren = true;
                }
                TreeEntity tree = new TreeEntity
                {
                    id = item.DepartmentId,
                    text = item.FullName.ToString(),
                    value = item.Code.ToString(),
                    Attribute = "DepartmentId",
                    AttributeValue = item.DepartmentId.ToString(),
                    //AttributeA = "DepartmentId",
                    //AttributeValueA = item.DepartmentId.ToString(),
                    isexpand = true,
                    complete = true,
                    hasChildren = hasChildren,
                    parentId = item.ParentId,

                };
                if (item.ParentId.ToString() == "0")
                {
                    tree.img = "/Content/Images/Icon16/molecule.png";
                }
                else if (item.SortCode.ToString() == "Company")
                {
                    tree.img = "/Content/Images/Icon16/hostname.png";
                }
                else if (item.SortCode.ToString() == "Department")
                {
                    tree.img = "/Content/Images/Icon16/chart_organisation.png";
                }
                treeList.Add(tree);
            }

            return Content(treeList.TreeToJson());
        }



        /// <summary>
        /// 【部门管理】返回表格JONS
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <returns></returns>
        public ActionResult GridListJson(string companyId)
        {
            DataTable listData = _baseDepartmentbll.GetList(companyId);
            var jsonData = new
            {
                rows = listData,
            };
            return Content(Util.Json.ToJson(jsonData));
        }
        /// <summary>
        /// 【部门管理】根据公司id获取部门列表返回树JONS
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <returns></returns>
        public ActionResult DepartmentTreeJson(string companyId)
        {
            DataTable listData = _baseDepartmentbll.GetList(companyId);
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (DataRow item in listData.Rows)
            {
                sb.Append("{");
                sb.Append("\"id\":\"" + item["departmentid"] + "\",");
                sb.Append("\"text\":\"" + item["fullname"] + "\",");
                sb.Append("\"value\":\"" + item["departmentid"] + "\",");
                sb.Append("\"img\":\"/Content/Images/Icon16/chart_organisation.png\",");
                sb.Append("\"isexpand\":true,");
                sb.Append("\"hasChildren\":false");
                sb.Append("},");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return Content(sb.ToString());
        }
        /// <summary>
        /// 【部门管理】返回列表JONS
        /// </summary>
        /// <param name="companyId">公司ID</param>
        /// <returns></returns>
        public ActionResult ListJson(string companyId)
        {
            DataTable listData = _baseDepartmentbll.GetList(companyId);
            return Content(Util.Json.ToJson(listData));
        }
        /// <summary>
        /// 【部门管理】删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult DeleteDepartment(string keyValue)
        {
            try
            {
                var message = "删除失败。";
                int isOk = 0;
                int userCount = DataFactory.Database().FindCount<BaseUser>("DepartmentId", keyValue);
                if (userCount == 0)
                {
                    isOk = Repositoryfactory.Repository().Delete(keyValue);
                    if (isOk > 0)
                    {
                        message = "删除成功。";
                    }
                }
                else
                {
                    message = "部门内有用户，不能删除。";
                }
                WriteLog(isOk, keyValue, message);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                WriteLog(-1, keyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        // update------tree


        /// <summary>
        /// 【公司管理】返回公司列表JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult TreeGridListJson()
        {
            List<BaseDepartment> ListData = _baseDepartmentbll.GetList();
            StringBuilder sb = new StringBuilder();
            sb.Append("{ \"rows\": [");
            sb.Append(TreeGridJson(ListData, -1));
            sb.Append("]}");
            return Content(sb.ToString());
        }
        int lft = 1, rgt = 1000000;
        public string TreeGridJson(List<BaseDepartment> ListData, int index, string ParentId = "0")
        {
            StringBuilder sb = new StringBuilder();
            List<BaseDepartment> ChildNodeList = ListData.FindAll(t => t.ParentId == ParentId);
            if (ChildNodeList.Count > 0) { index++; }
            foreach (BaseDepartment entity in ChildNodeList)
            {
                string strJson = Util.Json.ToJson(entity);
                strJson = strJson.Insert(1, "\"level\":" + index + ",");
                strJson = strJson.Insert(1, "\"isLeaf\":" + (ListData.Count<BaseDepartment>(t => t.ParentId == entity.DepartmentId) == 0 ? true : false).ToString().ToLower() + ",");
                strJson = strJson.Insert(1, "\"expanded\":true,");
                strJson = strJson.Insert(1, "\"lft\":" + lft++ + ",");
                strJson = strJson.Insert(1, "\"rgt\":" + rgt-- + ",");
                sb.Append(strJson);
                sb.Append(TreeGridJson(ListData, index, entity.DepartmentId));
            }
            return sb.ToString().Replace("}{", "},{");
        }


        public ActionResult InitCompanyDepartment(string departmentId)
        {
            return Content(Util.Json.ToJson(_baseDepartmentbll.InitCompanyDepartment(departmentId)));
        }


    }
}