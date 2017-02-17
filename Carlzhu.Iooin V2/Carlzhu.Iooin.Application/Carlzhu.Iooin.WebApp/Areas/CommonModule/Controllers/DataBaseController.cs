
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;
using Iooin.Framework.Code;
using OperationType = Carlzhu.Iooin.Business.CommonModule.OperationType;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 数据库管理控制器
    /// </summary>
    public class DataBaseController : Controller
    {
        readonly BaseDataBaseBll _baseDatabasebll = new BaseDataBaseBll();

        #region 列表
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 【数据库管理】返回列表JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult GridListJson(string tableName)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = _baseDatabasebll.GetList(tableName);
                var jsonData = new
                {
                    records = listData.Rows.Count,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = listData,
                };
                string str = Util.Json.ToJson(jsonData);
                return Content(Util.Json.ToJson(jsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 【数据库管理】返回列表JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult GridColumnListJson(string tableName)
        {
            try
            {
                DataTable listData = _baseDatabasebll.GetColumnList(tableName);
                var jsonData = new
                {
                    rows = listData,
                };
                return Content(Util.Json.ToJson(jsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult DeleteTable(string keyValue)
        {
            try
            {
                _baseDatabasebll.DeleteTable(keyValue);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 表单
        /// <summary>
        /// 表单
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表说明</param>
        /// <param name="tableFieldJson">表字段</param>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SubmitForm(string tableName, string tableDescription, string tableFieldJson, string keyValue)
        {
            try
            {
                string message = keyValue == "" ? "新建成功。" : "编辑成功。";
                string strSql = JsonToSql(tableName, tableDescription, tableFieldJson);
                _baseDatabasebll.CreateTable(new StringBuilder(strSql), keyValue);
                return Content(new JsonMessage { Success = true, Code = "1", Message = message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 查看Sql语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表说明</param>
        /// <param name="tableFieldJson">表字段</param>
        public ActionResult LookSql(string tableName, string tableDescription, string tableFieldJson)
        {
            return Content(JsonToSql(tableName, tableDescription, tableFieldJson));
        }
        /// <summary>
        /// Json转换SQL语句
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableDescription">表说明</param>
        /// <param name="tableFieldJson">表字段</param>
        /// <returns></returns>
        public string JsonToSql(string tableName, string tableDescription, string tableFieldJson)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("/*==============================================================*/\r\n");
            strSql.Append("--Table: " + tableName + "                                             \r\n");
            strSql.Append("--Description: " + tableDescription + "                                \r\n");
            strSql.Append("/*==============================================================*/\r\n");
            strSql.Append("CREATE TABLE " + tableName + "(\r\n");
            DataTable TableField = tableFieldJson.JsonToDataTable();
            if (!DataHelper.IsExistRows(TableField))
            {
                foreach (DataRow item in TableField.Rows)
                {
                    string Column = item["Column"].ToString();                      //列名
                    if (!string.IsNullOrEmpty(Column))
                    {
                        string dataBaseType = item["DataBaseType"].ToString();      //数据类型
                        string allowNull = item["AllowNull"].ToString();            //允许空
                        string identify = item["Identify"].ToString();              //标识
                        string primaryKey = item["PrimaryKey"].ToString();          //主键
                        string defaultValue = item["DefaultValue"].ToString();      //默认值
                        string description = item["Description"].ToString();        //说明
                        strSql.Append("\t" + Column + "\t\t\t\t");
                        strSql.Append(dataBaseType + " ");
                        strSql.Append(allowNull == "1" ? " NULL" : " NOT NULL " + " ");
                        strSql.Append(primaryKey == "0" ? "" : " PRIMARY KEY" + " ");
                        strSql.Append(identify == "0" ? "" : " identity(1,1)" + " ");
                        if (!string.IsNullOrEmpty(defaultValue))
                        {
                            strSql.Append(" default(" + defaultValue + ")" + " ");
                        }
                        strSql.Append(",\r\n");
                    }
                }
            }
            strSql.Append(")\r\n");
            //strSql.Append("go\r\n\r\n");
            //为表添加描述信息
            strSql.Append("EXECUTE sp_addextendedproperty 'MS_Description', \r\n\t'" + tableDescription + "',\r\n\t'user', 'dbo', 'table', '" + tableName + "'\r\n");
            //strSql.Append("go\r\n\r\n");
            if (!DataHelper.IsExistRows(TableField))
            {
                foreach (DataRow item in TableField.Rows)
                {
                    string column = item["Column"].ToString();                      //列名
                    if (!string.IsNullOrEmpty(column))
                    {
                        string description = item["Description"].ToString();        //说明
                        //为字段添加描述信息
                        strSql.Append("EXECUTE sp_addextendedproperty 'MS_Description', \r\n\t'" + description + "',\r\n\t'user', 'dbo', 'table', '" + tableName + "', 'column', '" + column + "'\r\n");
                        //strSql.Append("go\r\n\r\n");
                    }
                }
            }
            return strSql.ToString();
        }
        #endregion

        #region 明细
        /// <summary>
        /// 明细
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult Detail()
        {
            return View();
        }
        /// <summary>
        /// 查询数据库表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="parameterJson">查询条件</param>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridDataTableListJson(string tableName, string parameterJson, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = _baseDatabasebll.GetDataTableList(tableName, parameterJson, ref jqgridparam);
                var jsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = listData,
                };
                string str = Util.Json.ToJson(jsonData);
                return Content(Util.Json.ToJson(jsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 编辑表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <param name="entityJons">实体参数</param>
        /// <returns></returns>
        public ActionResult EditDataTableRow(string tableName, string pk, string entityJons)
        {
            try
            {
                Hashtable ht = new Hashtable();
                DataTable dt = DataHelper.DataFilter(_baseDatabasebll.GetColumnList(tableName), "datatype = 'datetime'");
                foreach (DataRow item in dt.Rows)
                {
                    ht[item["column"].ToString().ToLower()] = item["column"].ToString().ToLower();
                }
                Hashtable tableField = HashtableHelper.JsonToHashtable(entityJons);
                foreach (string item in ht.Keys)
                {
                    tableField.Remove(item);
                }
                int IsOk = DataFactory.Database().Update(tableName, tableField, pk);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        /// <summary>
        /// 删除表格行数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="pk">主键字段</param>
        /// <param name="entityJons"></param>
        /// <returns></returns>
        public ActionResult DeleteDataTableRow(string tableName, string pk, string entityJons)
        {
            try
            {
                Hashtable tableField = HashtableHelper.JsonToHashtable(entityJons);
                int isOk = DataFactory.Database().Delete(tableName, pk.ToLower(), tableField[pk.ToLower()].ToString());
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = "删除成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region SQL命令
        /// <summary>
        /// SQL命令视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult SqlCommand()
        {
            return View();
        }
        #endregion

        #region 数据库备份计划
        /// <summary>
        /// 数据库备份视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult DbBackup()
        {
            return View();
        }
        /// <summary>
        ///【备份计划】返回列表JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult DbBackupList()
        {
            try
            {
                List<BaseBackupJob> listData = DataFactory.Database().FindList<BaseBackupJob>("ORDER BY CreateDate DESC");
                var jsonData = new
                {
                    rows = listData,
                };
                return Content(Util.Json.ToJson(jsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 删除备份计划
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="jobName">计划名称</param>
        /// <returns></returns>
        public ActionResult DeleteDbBackup(string keyValue, string jobName)
        {
            try
            {
                _baseDatabasebll.DeleteDbBackup(keyValue, jobName);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "删除成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 新增备份计划
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="_Mode"></param>
        /// <param name="_StartTime"></param>
        /// <returns></returns>
        public ActionResult AddDbBackup(BaseBackupJob entity, string _Mode, string _StartTime)
        {
            try
            {
                entity.FilePath = entity.FilePath;
                entity.Create();
                _baseDatabasebll.CreateDbBackup(entity, _Mode, _StartTime);
                return Content(new JsonMessage { Success = true, Code = "1", Message = "操作成功" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion
    }
}
