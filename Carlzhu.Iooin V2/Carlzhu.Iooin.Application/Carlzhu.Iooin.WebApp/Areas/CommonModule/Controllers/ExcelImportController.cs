
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.CommonModule;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Offices;
using Iooin.Framework.Code;
using OperationType = Carlzhu.Iooin.Business.CommonModule.OperationType;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// Excel导入控制器
    /// </summary>
    public class ExcelImportController : PublicController<BaseExcelImport>
    {
        #region 列表
        /// <summary>
        /// 【Excel模板设置】返回列表JONS
        /// </summary>
        /// <returns></returns>
        public ActionResult ListJson()
        {
            List<BaseExcelImport> list = Baseexceliportbll.GetList();
            return Content(Util.Json.ToJson(list));
        }
        /// <summary>
        /// 【Excel模板设置】返回主表JSON
        /// </summary>
        /// <param name="ViewId">主表 主键值</param>
        /// <returns></returns>
        public JsonResult GetEntityJson(string ImportId)
        {
            BaseExcelImport entity = Repositoryfactory.Repository().FindEntity(ImportId);
            return Json(entity, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 【Excel模板设置】返回列表JSON
        /// </summary>
        /// <param name="ViewId">主表 主键值</param>
        /// <returns></returns>
        public JsonResult GetDetailsEntityJson(string ImportId)
        {
            List<BaseExcelImportDetail> list = DataFactory.Database().FindList<BaseExcelImportDetail>("ImportId", ImportId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 表单
        CodeMaticBll codematicbll = new CodeMaticBll();
        BaseDataBaseBll Basedatabasebll = new BaseDataBaseBll();
        BaseExcelImportBll Baseexceliportbll = new BaseExcelImportBll();
        /// <summary>
        /// Excel导入主页面
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ExcelImportIndex()
        {
            return View();
        }
        /// <summary>
        /// Excel导入设置页面
        /// </summary>
        /// <returns></returns>
        public ActionResult ExcelImportConten()
        {
            string keyValue = Request["KeyValue"];//主键
            return View();
        }
        /// <summary>
        /// 加载数据库表字段 返回JSon
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFieldTreeJson()
        {
            string tableCode = Request["tableCode"];//表名
            StringBuilder sb = new StringBuilder();
            string ImportId = DataFactory.Database().FindEntity<BaseExcelImport>("ImportTable", tableCode).ImportId;
            if (string.IsNullOrEmpty(ImportId))
            {
                return Content(sb.ToString());
            }
            List<BaseExcelImportDetail> list = DataFactory.Database().FindList<BaseExcelImportDetail>("ImportId", ImportId);
            DataTable dt = codematicbll.GetColumns(tableCode);
            //list.Find()
            sb.Append("[");
            sb.Append("{");
            sb.Append("\"id\":\"0\",");
            sb.Append("\"text\":\"" + tableCode + "\",");
            sb.Append("\"value\":\"0\",");
            sb.Append("\"isexpand\":true,");
            sb.Append("\"hasChildren\":true,");
            sb.Append("\"ChildNodes\":[");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("{");
                sb.Append("\"id\":\"" + dr["column_Name"].ToString() + "\",");
                sb.Append("\"text\":\"" + dr["comments"].ToString() + "\",");
                sb.Append("\"value\":\"" + dr["column_Name"].ToString() + "\",");
                sb.Append("\"title\":\"" + dr["column_Name"].ToString() + "\",");
                sb.Append("\"length\":\"" + dr["char_col_decl_length"].ToString() + "\",");
                sb.Append("\"showcheck\":true,");
                sb.Append(string.Format("\"checkstate\":{0},", list.Exists(x => x.FieldName == dr["column_Name"].ToString()) ? "1" : "0"));
                sb.Append("\"isexpand\":true,");
                sb.Append("\"hasChildren\":false");
                sb.Append("},");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            sb.Append("}");
            sb.Append("]");
            return Content(sb.ToString());
        }
        /// <summary>
        /// 加载数据库表字段 返回JSon
        /// </summary>
        /// <returns></returns>
        public ActionResult GetFieldJson()
        {
            string tableCode = Request["tableCode"];//表名
            StringBuilder sb = new StringBuilder();
            DataTable dt = Basedatabasebll.GetColumnList(tableCode);
            sb.Append("[");
            foreach (DataRow dr in dt.Rows)
            {
                sb.Append("{");
                sb.Append("\"id\":\"" + dr["column"].ToString() + "\",");
                sb.Append("\"text\":\"" + dr["remark"].ToString() + "\",");
                sb.Append("\"value\":\"" + dr["column"].ToString() + "\",");
                sb.Append("\"title\":\"" + dr["column"].ToString() + "\",");
                sb.Append("\"length\":\"" + dr["length"].ToString() + "\",");
                sb.Append("\"showcheck\":true,");
                sb.Append("\"checkstate\":0,");
                sb.Append("\"isexpand\":true,");
                sb.Append("\"hasChildren\":false");
                sb.Append("},");
            }
            sb = sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return Content(sb.ToString());
        }
        /// <summary>
        /// 【Excel模板设置】表单提交事件
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <param name="Baseexcelimport">导入模板实体</param>
        /// <param name="ExcelImportDetailJson">导入模板明细Json</param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult SubmitForm_ExcelImport(string KeyValue, BaseExcelImport Baseexcelimport, string ExcelImportDetailJson)
        {
            try
            {
                int IsOk = 0;
                IsOk = Baseexceliportbll.SubmitForm(KeyValue, Baseexcelimport, ExcelImportDetailJson);
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 点检计划Excel弹出框页面
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult ExcelImportDialog()
        {
            string moduleId = DESEncrypt.Decrypt(WebHelper.GetCookie("ModuleId"));
            //模板主表
            BaseExcelImport Baseexcellimport = DataFactory.Database().FindEntity<BaseExcelImport>("ModuleId", moduleId);
            if (Baseexcellimport.ModuleId != null)
            {
                ViewBag.ModuleId = moduleId;
                ViewBag.ImportFileName = Baseexcellimport.ImportFileName;
                ViewBag.ImportName = Baseexcellimport.ImportName;
                ViewBag.ImportId = Baseexcellimport.ImportId;
            }
            else
            {
                ViewBag.ModuleId = "0";
            }
            return View();
        }
        /// <summary>
        /// Excel模板编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult EditExcelImportConten()
        {

            string KeyValue = Request["KeyValue"];//主键
            BaseExcelImport BaseexcelImport = DataFactory.Database().FindEntity<BaseExcelImport>(KeyValue);
            ViewBag.tableCode = BaseexcelImport.ImportTable;            //表名
            ViewBag.tableName = BaseexcelImport.ImportTableName;
            ViewBag.ModuleName = DataFactory.Database().FindEntity<BaseModule>(BaseexcelImport.ModuleId).FullName;
            return View();
        }
        #endregion

        #region 导入
        /// <summary>
        /// 导入Excell数据
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportExel()
        {
            int IsOk = 1;//导入状态
            DataTable Result = new DataTable();//导入错误记录表
            try
            {
                string moduleId = Request["moduleId"];//表名
                StringBuilder sb_table = new StringBuilder();
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = files["filePath"];//获取上传的文件
                string fullname = file.FileName;
                string IsXls = System.IO.Path.GetExtension(fullname).ToString().ToLower();//System.IO.Path.GetExtension获得文件的扩展名
                if (IsXls != ".xls" && IsXls != ".xlsx")
                {
                    IsOk = 0;
                }
                else
                {

                    string filename = Guid.NewGuid().ToString() + ".xls";
                    if (fullname.IndexOf(".xlsx") > 0)
                    {
                        filename = Guid.NewGuid().ToString() + ".xlsx";
                    }
                    if (file != null && file.FileName != "")
                    {
                        string msg = UploadHelper.FileUpload(file, Server.MapPath("~/Resource/UploadFile/ImportExcel/"), filename);
                    }
                    DataTable dt = ImportExcel.ExcelToDataTable("Sheet1", Server.MapPath("~/Resource/UploadFile/ImportExcel/") + filename);
                    IsOk = Baseexceliportbll.ImportExcel(moduleId, dt, out Result);
                }

            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Add, "-1", "异常错误：" + ex.Message);
                IsOk = 0;
            }
            if (Result.Rows.Count > 0)
            {
                IsOk = 0;
            }
            var JsonData = new
            {
                Status = IsOk > 0 ? "true" : "false",
                ResultData = Result
            };
            return Content(Util.Json.ToJson(JsonData));
        }
        #endregion

        #region 导出模板
        /// <summary>
        /// 导出Excell模板
        /// </summary>
        /// <returns></returns>
        public void GetExcellTemperature(string ImportId)
        {
            if (!string.IsNullOrEmpty(ImportId))
            {
                DataTable data = new DataTable(); string DataColumn = ""; string fileName;
                Baseexceliportbll.GetExcellTemperature(ImportId, out data, out DataColumn, out fileName);
                DeriveExcel.DataTableToExcel(data, DataColumn.Split('|'), fileName);
            }
        }
        #endregion

        #region 导出Excel
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Ignore)]
        public ActionResult DeriveDialog()
        {
            return View();
        }
        #endregion
    }
}