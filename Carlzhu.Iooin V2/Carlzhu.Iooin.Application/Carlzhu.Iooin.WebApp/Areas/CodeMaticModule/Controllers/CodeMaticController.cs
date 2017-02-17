
using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web.Mvc;
using System.Xml;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;

namespace Carlzhu.Iooin.WebApp.Areas.CodeMaticModule.Controllers
{
    /// <summary>
    /// 代码生成器
    /// </summary>
    public class CodeMaticController : Controller
    {
        readonly CodeMaticBll _codematicbll = new CodeMaticBll();
        readonly BaseDataBaseBll _basedatabasebll = new BaseDataBaseBll();
        /// <summary>
        /// 代码生成 初始化页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CodeMaticIndex()
        {
            return View();
        }
        /// <summary>
        /// 代码生成设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult CodeMaticConten()
        {
            return View();
        }
        /// <summary>
        /// 加载数据库表 返回JSon
        /// </summary>
        /// <returns></returns>
        public ActionResult GetTableNameTreeJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            if (Config.GetValue("CodeMaticMode") == "PowerDesigner")
            {
                XmlNodeList myXmlNodeList = _codematicbll.GetTableName();
                foreach (XmlNode myXmlNode in myXmlNodeList)
                {
                    sb.Append("{");
                    sb.Append("\"id\":\"" + myXmlNode.ChildNodes[2].InnerText + "\",");
                    sb.Append("\"text\":\"" + myXmlNode.ChildNodes[1].InnerText + "\",");
                    sb.Append("\"value\":\"" + myXmlNode.ChildNodes[2].InnerText + "\",");
                    sb.Append("\"title\":\"" + myXmlNode.ChildNodes[2].InnerText + "\",");
                    sb.Append("\"img\":\"/Content/Images/Icon16/dataBase_table.png\",");
                    sb.Append("\"isexpand\":true,");
                    sb.Append("\"hasChildren\":false,");
                    sb.Append("\"ChildNodes\":[]");
                    sb.Append("},");
                }
            }
            else if (Config.GetValue("CodeMaticMode") == "DataBase")
            {
                DataTable dt = _basedatabasebll.GetList();
                if (DataHelper.IsExistRows(dt))
                {
                    foreach (DataRow itemRow in dt.Rows)
                    {
                        sb.Append("{");
                        sb.Append("\"id\":\"" + itemRow["name"] + "\",");
                        sb.Append("\"text\":\"" + itemRow["tdescription"] + "\",");
                        sb.Append("\"value\":\"" + itemRow["name"] + "\",");
                        sb.Append("\"title\":\"" + itemRow["name"] + "\",");
                        sb.Append("\"img\":\"/Content/Images/Icon16/dataBase_table.png\",");
                        sb.Append("\"isexpand\":true,");
                        sb.Append("\"hasChildren\":false,");
                        sb.Append("\"ChildNodes\":[]");
                        sb.Append("},");
                    }
                }
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return Content(sb.ToString());
        }
        /// <summary>
        /// 加载数据库表字段 返回JSon
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFieldTreeJson(string tableCode)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = new DataTable();
            if (Config.GetValue("CodeMaticMode") == "PowerDesigner")
            {
                dt = _codematicbll.GetColumns(tableCode);
            }
            else if (Config.GetValue("CodeMaticMode") == "DataBase")
            {
                dt = _basedatabasebll.GetColumnList(tableCode);
                dt.Columns["column"].ColumnName = "column_Name";
                dt.Columns["datatype"].ColumnName = "data_type";
                dt.Columns["length"].ColumnName = "char_col_decl_length";
                dt.Columns["remark"].ColumnName = "comments";
            }
            return Content(Util.Json.ToJson(dt));
        }

        #region 查看生成代码
        /// <summary>
        /// 生成实体类
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="className">类名备注</param>
        /// <param name="entityName">实体类名称</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderEntity(string projectName, string className, string entityName, string table)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = className;
            _codematicbll.ProjectName = projectName;
            _codematicbll.EntityName = entityName;

            DataTable dt = new DataTable();
            string primaryKeyColumns = "";
            switch (Config.GetValue("CodeMaticMode"))
            {
                case "PowerDesigner":
                    dt = _codematicbll.GetColumns(table);
                    primaryKeyColumns = _codematicbll.GetPrimaryKey(table);
                    break;
                case "DataBase":
                    dt = _basedatabasebll.GetColumnList(table);
                    dt.Columns["column"].ColumnName = "column_Name";
                    dt.Columns["datatype"].ColumnName = "data_type";
                    dt.Columns["length"].ColumnName = "char_col_decl_length";
                    dt.Columns["remark"].ColumnName = "comments";
                    primaryKeyColumns = _basedatabasebll.GetPrimaryKey(table);
                    break;
            }
            return Content(_codematicbll.CodeBuilder_Entity(table, primaryKeyColumns, dt));
        }
        /// <summary>
        /// 生成业务层
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="className">类名备注</param>
        /// <param name="entityName">实体类名称</param>
        /// <param name="serviceName">实体类名称</param>
        /// <param name="businessName">业务类名称</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderBusiness(string projectName, string className, string serviceName, string entityName, string businessName, string table)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = className;
            _codematicbll.ProjectName = projectName;
            _codematicbll.EntityName = entityName;
            _codematicbll.BusinessName = businessName;
            _codematicbll.ServiceName = serviceName;

            return Content(_codematicbll.GetCodeBuilderBusiness(table).ToString());
        }

        /// <summary>
        /// 生成表单页面
        /// </summary>
        /// <param name="areas">业务区域</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="className">类名备注</param>
        /// <param name="pageFormName">表单文件名称</param>
        /// <param name="taboe">表名</param>
        /// <param name="table"></param>
        /// <param name="fromJson">表单参数</param>
        /// <param name="formCss">表单Css</param>
        /// <param name="columnCount">显示列数</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderFrom(string areas, string controllerName, string className, string pageFormName, string table, string fromJson, string formCss, int columnCount)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = className;
            _codematicbll.PageFormName = pageFormName;
            _codematicbll.ControllerName = controllerName;
            _codematicbll.AreasName = areas;

            return Content(_codematicbll.GetCodeBuilderFrom(table, fromJson, columnCount, formCss).ToString());
        }
        /// <summary>
        /// 生成明细页面
        /// </summary>
        /// <param name="Areas">业务区域</param>
        /// <param name="ControllerName">控制器名称</param>
        /// <param name="ClassName">类名备注</param>
        /// <param name="PageFormDetailName">明细文件名称</param>
        /// <param name="taboe">表名</param>
        /// <param name="FromJson">表单参数</param>
        /// <param name="FormCss">表单Css</param>
        /// <param name="ColumnCount">显示列数</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderFromDetail(string Areas, string ControllerName, string ClassName, string PageFormDetailName, string table, string FromDetailJson, string FormDetailCss, int ColumnCount)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = ClassName;
            _codematicbll.PageFormDetailName = PageFormDetailName;
            _codematicbll.ControllerName = ControllerName;
            _codematicbll.AreasName = Areas;

            return Content(_codematicbll.GetCodeBuilderFromDetail(table, FromDetailJson, ColumnCount, FormDetailCss).ToString());
        }
        /// <summary>
        /// 生成列表页面
        /// </summary>
        /// <param name="areas">业务区域</param>
        /// <param name="controllerName">控制器名称</param>
        /// <param name="className">类名备注</param>
        /// <param name="pageListName">列表文件名称</param>
        /// <param name="table">表名</param>
        /// <param name="showFieldJson">显示字段</param>
        /// <param name="allowOrder">是否排序</param>
        /// <param name="orderType">排序类型</param>
        /// <param name="orderField">排序字段</param>
        /// <param name="allowPageing">是否分页</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageLayout">页面布局</param>
        /// <param name="method">操作</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderList(string areas, string controllerName, string className, string pageListName, string table,
            string showFieldJson, string allowOrder, string orderType, string orderField, string allowPageing, string pageSize, string pageLayout, string method)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = className;
            _codematicbll.PageListName = pageListName;
            _codematicbll.ControllerName = controllerName;
            _codematicbll.AreasName = areas;

            Hashtable htmethod = HashtableHelper.ParameterToHashtable(method);
            return Content(_codematicbll.GetCodeBuilderList(table, showFieldJson, allowOrder, orderType, orderField, allowPageing, pageSize, pageLayout, htmethod).ToString());
        }
        /// <summary>
        /// 生成控制器
        /// </summary>
        /// <param name="Areas">业务区域</param>
        /// <param name="ClassName">类名备注</param>
        /// <param name="ControllerName">控制器名称</param>
        /// <param name="BusinessName">业务层名称</param>
        /// <param name="EntityName">实体层名称</param>
        /// <param name="table">名表</param>
        /// <returns></returns>
        public ActionResult GetCodeBuilderController(string Areas, string ClassName, string ControllerName, string BusinessName, string EntityName, string table)
        {
            _codematicbll.Company = "Iooin";
            _codematicbll.Author = "Carlzhu";
            _codematicbll.CreateYear = DateTime.Now.ToString("yyyy");
            _codematicbll.CreateDate = DateTime.Now.ToString("yyyy.MM.dd HH:mm");
            _codematicbll.ClassName = ClassName;
            _codematicbll.ControllerName = ControllerName;
            _codematicbll.BusinessName = BusinessName;
            _codematicbll.EntityName = EntityName;
            _codematicbll.AreasName = Areas;
            return Content(_codematicbll.GetCodeBuilderController(table));
        }
        #endregion

        #region 下载生成代码
        /// <summary>
        /// 下载生成代码
        /// </summary>
        /// <param name="table">表名</param>
        /// <returns></returns>
        public ActionResult DownloadCodeBuilder(string table)
        {
            var path = Server.MapPath("~/Areas/CodeMaticModule/DataModel/CodeMatic/" + table + ".zip");
            return File(path, "application/zip-x-compressed", table + ".zip");
        }
        #endregion
    }
}
