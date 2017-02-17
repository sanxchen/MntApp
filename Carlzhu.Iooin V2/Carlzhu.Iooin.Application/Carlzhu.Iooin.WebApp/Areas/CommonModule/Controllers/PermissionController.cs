﻿


using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;



using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 权限操作控制器
    /// </summary>
    public class PermissionController : Controller
    {
        BaseModulePermissionBll Basemodulepermissionbll = new BaseModulePermissionBll();
        BaseButtonPermissionBll Basebuttonpermissionbll = new BaseButtonPermissionBll();
        BaseViewPermissionBll Baseviewpermissionbll = new BaseViewPermissionBll();
        BaseObjectUserRelationBll Baseobjectuserrelationbll = new BaseObjectUserRelationBll();
        BaseModuleBll Basemodulebll = new BaseModuleBll();
        BaseDataScopePermissionBll Basedatascopepermissionbll = new BaseDataScopePermissionBll();

        #region 分配权限
        /// <summary>
        /// 分配权限
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AllotPermission()
        {
            return View();
        }
        /// <summary>
        /// 加载模块 返回树JSON
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// <returns></returns>
        public ActionResult ModuleTree(string ObjectId, string Category)
        {
            DataTable dt = Basemodulepermissionbll.GetList(ObjectId, Category);
            List<TreeEntity> treeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    string moduleId = item["moduleid"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(dt, "ParentId = '" + moduleId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity
                    {
                        id = item["moduleid"].ToString(),
                        text = item["fullname"].ToString(),
                        value = item["moduleid"].ToString(),
                        checkstate = item["objectid"].ToString() != "" ? 1 : 0,
                        showcheck = true,
                        isexpand = true,
                        complete = true,
                        hasChildren = hasChildren,
                        parentId = item["parentid"].ToString(),
                        img = "/Content/Images/Icon16/" + item["icon"]
                    };
                    treeList.Add(tree);
                }
            }
            return Content(treeList.TreeToJson());
        }
        /// <summary>
        /// 加载按钮
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// <returns></returns>
        public ActionResult ButtoneList(string ObjectId, string Category)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = Basebuttonpermissionbll.GetList(ObjectId, Category);
            foreach (DataRow dr in dt.Rows)
            {
                string strchecked = "";
                if (!string.IsNullOrEmpty(dr["objectid"].ToString()))//判断是否选中
                {
                    strchecked = "selected";
                }
                sb.Append("<li title=\"" + dr["fullname"] + "\" moduleid=\"" + dr["moduleid"] + "\" style='display:none;' class=\"" + strchecked + "\">");
                sb.Append("<a class=\"disabled Category_" + dr["category"] + "\" id=\"" + dr["buttonid"] + "|" + dr["moduleid"] + "\"><img src=\"/Content/Images/Icon16/" + dr["icon"] + "\">" + dr["fullname"] + "</a><i></i>");
                sb.Append("</li>");
            }
            return Content(sb.ToString());
        }
        /// <summary>
        /// 加载视图
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// <returns></returns>
        public ActionResult ViewList(string ObjectId, string Category)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = Baseviewpermissionbll.GetList(ObjectId, Category);
            foreach (DataRow dr in dt.Rows)
            {
                string strchecked = "";
                if (!string.IsNullOrEmpty(dr["objectid"].ToString()))//判断是否选中
                {
                    strchecked = "selected";
                }
                sb.Append("<li title=\"" + dr["showname"] + "\" moduleid=\"" + dr["moduleid"] + "\" style='display:none;' class=\"" + strchecked + "\">");
                sb.Append("<a class=\"disabled\" id=\"" + dr["viewid"] + "|" + dr["moduleid"] + "\"><img src=\"/Content/Images/Icon16/tag_blue.png\">" + dr["showname"] + "</a><i></i>");
                sb.Append("</li>");
            }
            return Content(sb.ToString());
        }
        /// <summary>
        /// 权限授权提交事件
        /// </summary>
        /// <param name="ModuleId">访问权限值</param>
        /// <param name="ModuleButtonId">操作权限值</param>
        /// <param name="ViewDetailId">视图权限值</param>
        /// <param name="ObjectId">对象ID</param>
        /// <param name="Category">分类</param>
        /// <returns></returns>
        public ActionResult AuthorizedSubmit(string ModuleId, string ModuleButtonId, string ViewDetailId, string ObjectId, string Category)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<DbParameter> parameter = new List<DbParameter>
                {
                    DbFactory.CreateDbParameter("@ObjectId", ObjectId),
                    DbFactory.CreateDbParameter("@Category", Category)
                };

                #region 访问
                string[] arrayModuleId = ModuleId.Split(',');
                StringBuilder sbDelete_Module = new StringBuilder("delete from BaseModulePermission where ObjectId = @ObjectId AND Category=@Category");
                database.ExecuteBySql(sbDelete_Module, parameter.ToArray(), isOpenTrans);
                int index = 1;
                foreach (var item in arrayModuleId)
                {
                    if (item.Length > 0)
                    {
                        BaseModulePermission entity = new BaseModulePermission();
                        entity.ModulePermissionId = CommonHelper.GetGuid;
                        entity.ObjectId = ObjectId;
                        entity.Category = Category;
                        entity.ModuleId = item;
                        entity.SortCode = index;
                        entity.Create();
                        database.Insert(entity, isOpenTrans);
                        index++;
                    }
                }
                #endregion

                #region 操作
                string[] arrayModuleButtonId = ModuleButtonId.Split(',');
                StringBuilder sbDelete_Button = new StringBuilder("delete from BaseButtonPermission where ObjectId = @ObjectId AND Category=@Category");
                database.ExecuteBySql(sbDelete_Button, parameter.ToArray(), isOpenTrans);
                index = 1;
                foreach (var item in arrayModuleButtonId)
                {
                    if (item.Length > 0)
                    {
                        string[] stritem = item.Split('|');
                        BaseButtonPermission entity = new BaseButtonPermission();
                        entity.ButtonPermissionId = CommonHelper.GetGuid;
                        entity.ObjectId = ObjectId;
                        entity.Category = Category;
                        entity.ModuleButtonId = stritem[0];
                        entity.ModuleId = stritem[0];
                        entity.SortCode = index;
                        entity.Create();
                        database.Insert(entity, isOpenTrans);
                        index++;
                    }
                }
                #endregion

                #region 视图
                string[] arrayViewDetailId = ViewDetailId.Split(',');
                StringBuilder sbDelete_View = new StringBuilder("delete from BaseViewPermission where ObjectId = @ObjectId AND Category=@Category");
                database.ExecuteBySql(sbDelete_View, parameter.ToArray(), isOpenTrans);
                index = 1;
                foreach (var item in arrayViewDetailId)
                {
                    if (item.Length > 0)
                    {
                        string[] stritem = item.Split('|');
                        BaseViewPermission entity = new BaseViewPermission();
                        entity.ViewPermissionId = CommonHelper.GetGuid;
                        entity.ObjectId = ObjectId;
                        entity.Category = Category;
                        entity.ViewId = stritem[0];
                        entity.ModuleId = stritem[1];
                        entity.SortCode = index;
                        entity.Create();
                        database.Insert(entity, isOpenTrans);
                        index++;
                    }
                }
                #endregion
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 分配权限 批量
        #endregion

        #region 分配用户
        /// <summary>
        /// 根据部门Id加载用户视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AllotUser()
        {
            return View();
        }
        /// <summary>
        /// 根据公司Id/部门Id加载用户视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AllotMember()
        {
            return View();
        }
        /// <summary>
        /// 根据公司Id加载用户列表
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="DepartmentId">部门ID</param>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// <returns></returns>
        public ActionResult MemberList(string CompanyId, string DepartmentId, string ObjectId, string Category)
        {
            StringBuilder sb = new StringBuilder();
            DataTable ListData = Baseobjectuserrelationbll.GetList(CompanyId, DepartmentId, ObjectId, Category);
            if (ListData != null && ListData.Rows.Count != 0)
            {
                foreach (DataRow item in ListData.Rows)
                {
                    string Genderimg = "user_female.png";
                    if (item["Gender"].ToString() == "男")
                    {
                        Genderimg = "user_green.png";
                    }
                    string strchecked = "";
                    if (!string.IsNullOrEmpty(item["objectid"].ToString()))//判断是否选中
                    {
                        strchecked = "selected";
                    }
                    sb.Append("<li class=\"" + item["departmentid"] + " " + strchecked + "\">");
                    sb.Append("<a class=\"a_" + strchecked + "\" id=\"" + item["userid"] + "\" title='工号：" + item["code"] + "\r\n账户：" + item["account"] + "'><img src=\"/Content/Images/Icon16/" + Genderimg + "\">" + item["realname"] + "</a><i></i>");
                    sb.Append("</li>");
                }
            }
            return Content(sb.ToString());
        }
        /// <summary>
        /// 添加用户 - 提交事件
        /// </summary>
        /// <param name="UserId">选中用户ID: 1,2,3,4,5,6</param>
        /// <param name="ObjectId"></param>
        /// <param name="Category"></param>
        /// <returns></returns>
        public ActionResult AuthorizedMember(string UserId, string ObjectId, string Category)
        {
            try
            {
                string[] array = UserId.Split(',');
                int IsOk = Baseobjectuserrelationbll.BatchAddMember(array, ObjectId, Category);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 分配用户 批量
        /// <summary>
        /// 分配用户 批量
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult AllotMemberBatch()
        {
            return View();
        }
        #endregion

        #region 数据范围
        /// <summary>
        /// 数据范围
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ScopePermission()
        {
            return View();
        }
        /// <summary>
        /// 加载授权项目
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeAuthorizedProject()
        {
            StringBuilder sbHtml = new StringBuilder();
            List<BaseModule> list = Basemodulebll.GetList().FindAll(t => t.DataScope == 1);
            int index = 0;
            string leftselected = "class=\"leftselected\"";
            foreach (BaseModule entity in list)
            {
                if (index > 0)
                    leftselected = "";
                sbHtml.Append("<li>");
                sbHtml.Append("    <div ModuleId=\"" + entity.ModuleId + "\"  " + leftselected + ">");
                sbHtml.Append("        <img src=\"/Content/Images/Icon16/" + entity.Icon + "\"><span>" + entity.FullName + "</span>");
                sbHtml.Append("    </div>");
                sbHtml.Append("</li>");
                index++;
            }
            return Content(sbHtml.ToString());
            //StringBuilder sbJson = new StringBuilder();
            //List<BaseModule> list = Basemodulebll.GetList().FindAll(t => t.DataScope == 1);
            //if (list.Count > 0 )
            //{
            //    foreach (BaseModule entity in list)
            //    {
            //        sbJson.Append("{");
            //        sbJson.Append("\"id\":\"" + entity.ModuleId + "\",");
            //        sbJson.Append("\"text\":\"" + entity.FullName + "\",");
            //        sbJson.Append("\"value\":\"" + entity.Code + "\",");
            //        sbJson.Append("\"isexpand\":true,");
            //        sbJson.Append("\"img\":\"/Content/Images/Icon16/" + entity.Icon + "\",");
            //        sbJson.Append("\"hasChildren\":false");
            //        sbJson.Append("},");
            //    }
            //    sbJson = sbJson.Remove(sbJson.Length - 1, 1);
            //}
            //StringBuilder strJson = new StringBuilder();
            //strJson.Append("[{");
            //strJson.Append("\"id\":\"0\",");
            //strJson.Append("\"text\":\"授权项目\",");
            //strJson.Append("\"value\":\"0\",");
            //strJson.Append("\"isexpand\":true,");
            //strJson.Append("\"img\":\"/Content/Images/Icon16/change_password.png\",");
            //strJson.Append("\"hasChildren\":true,");
            //strJson.Append("\"ChildNodes\":[" + sbJson + "]");
            //strJson.Append("}]");
            //return Content(strJson.ToString());
        }
        /// <summary>
        /// 授权提交事件
        /// </summary>
        /// <param name="ScopeType">范围类型:1-显示设置；2-条件设置</param>
        /// <param name="ModuleId">模块Id</param>
        /// <param name="ResourceId">对什么资源Id</param>
        /// <param name="ObjectId">对象ID</param>
        /// <param name="Category">分类</param>
        /// <returns></returns>
        public ActionResult ScopeAuthorizedSubmit(string ScopeType, string ModuleId, string ResourceId, string ObjectId, string Category)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                Hashtable htDelete = new Hashtable
                {
                    ["ModuleId"] = ModuleId,
                    ["ObjectId"] = ObjectId,
                    ["Category"] = Category
                };
                database.Delete("BaseDataScopePermission", htDelete, isOpenTrans);
                string[] arrayResourceId = ResourceId.Split(',');
                int index = 1;
                foreach (var item in arrayResourceId)
                {
                    if (item.Length > 0)
                    {
                        BaseDataScopePermission entity = new BaseDataScopePermission
                        {
                            DataScopePermissionId = CommonHelper.GetGuid,
                            ObjectId = ObjectId,
                            Category = Category,
                            ModuleId = ModuleId,
                            ResourceId = item,
                            ScopeType = ScopeType,
                            SortCode = index
                        };
                        entity.Create();
                        database.Insert(entity, isOpenTrans);
                        index++;
                    }
                }
                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }

        #region 公司管理
        /// <summary>
        /// 加载公司
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeCompanyList(string ObjectId, string Category)
        {
            string ModuleId = "b29cabd8-ffb6-4d34-9d08-ee1dba2b5b6b";
            DataTable DataList = Basedatascopepermissionbll.GetScopeCompanyList(ModuleId, ObjectId, Category);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow item in DataList.Rows)
                {
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid = '" + item["companyid"].ToString() + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity();
                    tree.id = item["companyid"].ToString();
                    tree.text = item["fullname"].ToString();
                    tree.value = item["code"].ToString();
                    tree.checkstate = item["objectid"].ToString() != "" ? 1 : 0;
                    tree.showcheck = true;
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item["parentid"].ToString();
                    if (item["parentid"].ToString() == "0")
                    {
                        tree.img = "/Content/Images/Icon16/molecule.png";
                    }
                    else
                    {
                        tree.img = "/Content/Images/Icon16/hostname.png";
                    }
                    TreeList.Add(tree);
                }
            }
            else
            {
                TreeEntity tree = new TreeEntity();
                tree.id = "";
                tree.text = "<span style='color:red'>没有找到您要的相关数据...</span>";
                tree.value = "";
                tree.isexpand = true;
                tree.complete = true;
                tree.hasChildren = false;
                tree.parentId = "0";
                tree.img = "/Content/Images/Icon32/dataBasered.png";
                TreeList.Add(tree);
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #region 部门管理
        /// <summary>
        /// 加载部门
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeDepartmentList(string ObjectId, string Category)
        {
            string ModuleId = "e84c0fca-d912-4f5c-a25e-d5765e33b0d2";
            DataTable DataList = Basedatascopepermissionbll.GetScopeDepartmentList(ModuleId, ObjectId, Category);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow row in DataList.Rows)
                {
                    TreeEntity tree = new TreeEntity();
                    string DepartmentId = row["departmentid"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid='" + DepartmentId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    else
                    {
                        if (row["sort"].ToString() == "Company")
                        {
                            continue;
                        }
                    }
                    if (row["parentid"].ToString() != "0")
                    {
                        tree.checkstate = row["objectid"].ToString() != "" ? 1 : 0;
                        tree.showcheck = true;
                    }
                    tree.id = DepartmentId;
                    tree.text = row["fullname"].ToString();
                    tree.value = row["code"].ToString();
                    tree.parentId = row["parentid"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
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
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #region 角色管理
        /// <summary>
        /// 加载角色
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeRoleList(string ObjectId, string Category)
        {
            string ModuleId = "cef74b80-24a5-4d77-9ede-bbbc75cdb431";
            DataTable DataList = Basedatascopepermissionbll.GetScopeRoleList(ModuleId, ObjectId, Category);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow row in DataList.Rows)
                {
                    TreeEntity tree = new TreeEntity();
                    string RoleId = row["roleid"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid='" + RoleId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    else
                    {
                        if (row["sort"].ToString() == "Company")
                        {
                            continue;
                        }
                    }
                    if (row["parentid"].ToString() != "0")
                    {
                        tree.checkstate = row["objectid"].ToString() != "" ? 1 : 0;
                        tree.showcheck = true;
                    }
                    tree.id = RoleId;
                    tree.text = row["fullname"].ToString();
                    tree.value = row["code"].ToString();
                    tree.parentId = row["parentid"].ToString();
                    tree.Attribute = "Type";
                    tree.AttributeValue = row["sort"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    if (row["parentid"].ToString() == "0")
                    {
                        tree.img = "/Content/Images/Icon16/molecule.png";
                    }
                    else if (row["sort"].ToString() == "Company")
                    {
                        tree.img = "/Content/Images/Icon16/hostname.png";
                    }
                    else if (row["sort"].ToString() == "Roles")
                    {
                        tree.img = "/Content/Images/Icon16/role.png";
                    }
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #region 岗位管理
        /// <summary>
        /// 加载岗位
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopePostList(string ObjectId, string Category)
        {
            string ModuleId = "eb0c4d65-4757-4892-b2e9-35882704e592";
            DataTable DataList = Basedatascopepermissionbll.GetScopePostList(ModuleId, ObjectId, Category);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow row in DataList.Rows)
                {
                    TreeEntity tree = new TreeEntity();
                    string PostId = row["id"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid='" + PostId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    else
                    {
                        if (row["sort"].ToString() == "Company" || row["sort"].ToString() == "Department")
                        {
                            continue;
                        }
                    }
                    if (row["parentid"].ToString() != "0")
                    {
                        tree.checkstate = row["objectid"].ToString() != "" ? 1 : 0;
                        tree.showcheck = true;
                    }
                    tree.id = PostId;
                    tree.text = row["fullname"].ToString();
                    tree.value = row["code"].ToString();
                    tree.parentId = row["parentid"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
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
                    else if (row["sort"].ToString() == "Post")
                    {
                        tree.img = "/Content/Images/Icon16/outlook_new_meeting.png";
                    }
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #region 用户组管理
        /// <summary>
        /// 加载岗位
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeUserGroupList(string ObjectId, string Category)
        {
            string ModuleId = "b863d076-37bb-45aa-8318-37942026921e";
            DataTable DataList = Basedatascopepermissionbll.GetScopeUserGroupList(ModuleId, ObjectId, Category);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow row in DataList.Rows)
                {
                    TreeEntity tree = new TreeEntity();
                    string PostId = row["id"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid='" + PostId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    else
                    {
                        if (row["sort"].ToString() == "Company" || row["sort"].ToString() == "Department")
                        {
                            continue;
                        }
                    }
                    if (row["parentid"].ToString() != "0")
                    {
                        tree.checkstate = row["objectid"].ToString() != "" ? 1 : 0;
                        tree.showcheck = true;
                    }
                    tree.id = PostId;
                    tree.text = row["fullname"].ToString();
                    tree.value = row["code"].ToString();
                    tree.parentId = row["parentid"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
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
                    else if (row["sort"].ToString() == "UserGroup")
                    {
                        tree.img = "/Content/Images/Icon16/group_gear.png";
                    }
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #region 用户管理
        /// <summary>
        /// 加载岗位
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// </summary>
        /// <returns></returns>
        public ActionResult ScopeUserList(string ObjectId, string Category, string nocheck)
        {
            string ModuleId = "58e86c4c-8022-4d30-95d5-b3d0eedcc878";

            DataTable DataList = Basedatascopepermissionbll.GetScopeUserList(ModuleId, ObjectId, Category);
            //补充部门下人员：
            //parentid='MJCompany' 取 id as department
            //SELECT * FROM BASEEMPLOYEE  WHERE DEPARTMENTID IN ('PDA20')


            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (DataHelper.IsExistRows(DataList))
            {
                foreach (DataRow row in DataList.Rows)
                {
                    TreeEntity tree = new TreeEntity();
                    string PostId = row["id"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(DataList, "parentid='" + PostId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    else
                    {
                        if (row["sort"].ToString() == "Company" || row["sort"].ToString() == "Department")
                        {
                            continue;
                        }
                    }
                    if (row["parentid"].ToString() != "0")
                    {
                        tree.checkstate = row["objectid"].ToString() != "" ? 1 : 0;
                        tree.showcheck = string.IsNullOrEmpty(nocheck) ? true : false;
                    }
                    tree.id = PostId;
                    tree.text = row["fullname"].ToString();
                    tree.value = row["code"].ToString();
                    tree.parentId = row["parentid"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
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
                    else if (row["sort"].ToString() == "User")
                    {
                        if (row["gender"].ToString() == "男")
                        {
                            tree.img = "/Content/Images/Icon16/user_green.png";
                        }
                        else if (row["gender"].ToString() == "女")
                        {
                            tree.img = "/Content/Images/Icon16/user_female.png";
                        }
                    }
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion

        #endregion

        #region 数据范围 批量
        #endregion

        #region 查看拥有权限
        /// <summary>
        /// 查看拥有成员 返回树JSON
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public ActionResult LookObjectUserList(string ObjectId)
        {
            StringBuilder sb = new StringBuilder();
            List<BaseUser> ListData = Baseobjectuserrelationbll.GetUserList(ObjectId);
            if (ListData.Count > 0)
            {
                foreach (BaseUser item in ListData)
                {
                    string Genderimg = "user_female.png";
                    if (item.Gender.ToString() == "男")
                    {
                        Genderimg = "user_green.png";
                    }
                    sb.Append("<li class=\"selected\">");
                    sb.Append("<a id=\"" + item.UserId + "\" title='工号：" + item.UserId + "\r\n账户：" + item.Account + "'><img src=\"/Content/Images/Icon16/" + Genderimg + "\">" + item.RealName + "</a><i></i>");
                    sb.Append("</li>");
                }
            }
            return Content(sb.ToString());
        }
        /// <summary>
        /// 查看拥有模块权限 返回树JSON
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <returns></returns>
        public ActionResult LookModulePermission(string ObjectId)
        {
            //如果ObjectId为空。自动获取当前登录用户拥有权限
            if (string.IsNullOrEmpty(ObjectId))
            {
                ObjectId = ManageProvider.Provider.Current().ObjectId;
            }
            DataTable dt = Basemodulepermissionbll.GetModulePermission(ObjectId);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (!DataHelper.IsExistRows(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    string ModuleId = item["moduleid"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(dt, "ParentId = '" + ModuleId + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity();
                    tree.id = item["moduleid"].ToString();
                    tree.text = item["fullname"].ToString();
                    tree.value = item["moduleid"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item["parentid"].ToString();
                    tree.img = item["icon"].ToString() != null ? "/Content/Images/Icon16/" + item["icon"].ToString() : item["icon"].ToString();
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// 查看拥有按钮权限 返回树JSON
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <returns></returns>
        public ActionResult LookButtonePermission(string ObjectId)
        {
            //如果ObjectId为空。自动获取当前登录用户拥有权限
            if (string.IsNullOrEmpty(ObjectId))
            {
                ObjectId = ManageProvider.Provider.Current().ObjectId;
            }
            DataTable dt = Basebuttonpermissionbll.GetButtonePermission(ObjectId);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (!DataHelper.IsExistRows(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    string text = "";
                    if (item["Sort"].ToString() == "按钮")
                    {
                        if (item["Category"].ToString() == "1")
                        {
                            text = item["fullname"].ToString() + "（工具栏）";
                        }
                        else if (item["Category"].ToString() == "2")
                        {
                            text = item["fullname"].ToString() + "（右击栏）";
                        }
                    }
                    else
                    {
                        text = item["fullname"].ToString();
                    }
                    string id = item["id"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(dt, "ParentId = '" + id + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity();
                    tree.id = item["id"].ToString();
                    tree.text = text;
                    tree.value = item["id"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item["parentid"].ToString();
                    tree.img = item["icon"].ToString() != null ? "/Content/Images/Icon16/" + item["icon"].ToString() : item["icon"].ToString();
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        /// <summary>
        /// 查看拥有视图权限 返回树JSON
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <returns></returns>
        public ActionResult LookViewPermission(string ObjectId)
        {
            //如果ObjectId为空。自动获取当前登录用户拥有权限
            if (string.IsNullOrEmpty(ObjectId))
            {
                ObjectId = ManageProvider.Provider.Current().ObjectId;
            }
            DataTable dt = Baseviewpermissionbll.GetViewPermission(ObjectId);
            List<TreeEntity> TreeList = new List<TreeEntity>();
            if (!DataHelper.IsExistRows(dt))
            {
                foreach (DataRow item in dt.Rows)
                {
                    string id = item["id"].ToString();
                    bool hasChildren = false;
                    DataTable childnode = DataHelper.DataFilter(dt, "ParentId = '" + id + "'");
                    if (childnode.Rows.Count > 0)
                    {
                        hasChildren = true;
                    }
                    TreeEntity tree = new TreeEntity();
                    tree.id = item["id"].ToString();
                    tree.text = item["fullname"].ToString();
                    tree.value = item["id"].ToString();
                    tree.isexpand = true;
                    tree.complete = true;
                    tree.hasChildren = hasChildren;
                    tree.parentId = item["parentid"].ToString();
                    tree.img = item["icon"].ToString() != "" ? "/Content/Images/Icon16/" + item["icon"].ToString() : "/Content/Images/Icon16/tag_blue.png";
                    TreeList.Add(tree);
                }
            }
            return Content(TreeList.TreeToJson());
        }
        #endregion
    }
}
