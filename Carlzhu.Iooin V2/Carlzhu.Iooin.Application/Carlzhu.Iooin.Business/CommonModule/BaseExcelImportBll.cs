using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;



using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.DataAccess.DataBase;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// Excel导入
    /// </summary>
    public class BaseExcelImportBll : RepositoryFactory<BaseExcelImport>
    {
        #region 表单提交
        /// <summary>
        /// 【Excel模板设置】表单提交事件
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <param name="Entity">导入模板实体</param>
        /// <param name="ExcelImportDetailJson">导入模板明细Json</param>
        /// <returns></returns>
        public int SubmitForm(string KeyValue, BaseExcelImport BaseexcelImport, string ExcelImportDetailJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BaseExcelImportDetail> ExcelImportDetailList = ExcelImportDetailJson.JonsToList<BaseExcelImportDetail>();
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    BaseexcelImport.Modify(KeyValue);
                    database.Update(BaseexcelImport, isOpenTrans);
                    database.Delete("BaseExcelImportDetail", "ImportId", BaseexcelImport.ImportId);//将原有明细删除掉，后面新增进来，确保不会有重复明细值
                }
                else
                {
                    BaseexcelImport.Create(); database.Insert(BaseexcelImport, isOpenTrans);
                }
                foreach (BaseExcelImportDetail BaseexcelImportDetail in ExcelImportDetailList)
                {
                    BaseexcelImportDetail.ImportDetailId = CommonHelper.GetGuid;
                    BaseexcelImportDetail.ImportId = BaseexcelImport.ImportId;
                    database.Insert(BaseexcelImportDetail, isOpenTrans);
                }
                database.Commit();
                return 1;
            }
            catch
            {
                database.Rollback();
                return -1;
            }
        }
        #endregion

        #region 获取导入模板列表
        /// <summary>
        /// 获取导入模板列表
        /// </summary>
        /// <returns></returns>
        public List<BaseExcelImport> GetList()
        {
            return Repository().FindList();
        }
        #endregion

        #region 导入数据
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="dt">Excel数据</param>
        /// <returns></returns>
        public int ImportExcel(string moduleId, DataTable dt, out DataTable Result)
        {
            //构造导入返回结果表
            DataTable Newdt = new DataTable("Result");
            Newdt.Columns.Add("rowid", typeof(String));                 //行号
            Newdt.Columns.Add("locate", typeof(String));                 //位置
            Newdt.Columns.Add("reason", typeof(String));                 //原因
            int IsOk = 0;
            //获得导入模板
            //模板主表
            BaseExcelImport Baseexcellimport = DataFactory.Database().FindEntity<BaseExcelImport>("ModuleId", moduleId);
            if (Baseexcellimport.ImportId == null)
            {
                IsOk = 0;
            }
            else
            {
                string pkName = DatabaseCommon.GetKeyField("Carlzhu.Iooin.Entity." + Baseexcellimport.ImportTable).ToString();
                //模板明细表
                List<BaseExcelImportDetail> listBaseExcelImportDetail = DataFactory.Database().FindList<BaseExcelImportDetail>("ImportId", Baseexcellimport.ImportId);

                //取出要插入的表名
                string tableName = Baseexcellimport.ImportTable;
                if (dt != null && dt.Rows.Count > 0)
                {
                    bool isExit = false;
                    IDatabase database = DataFactory.Database();
                    DbTransaction isOpenTrans = database.BeginTrans();
                    try
                    {
                        #region 遍历Excel数据行
                        int rowNum = 1;
                        int errorNum = 1;
                        foreach (DataRow item in dt.Rows)
                        {
                            Hashtable entity = new Hashtable();//最终要插入数据库的hashtable
                            StringBuilder sb = new StringBuilder();
                            entity[pkName] = Guid.NewGuid().ToString();//首先给主键赋值
                            #region 遍历模板，为每一行中每个字段找到模板列并赋值
                            foreach (BaseExcelImportDetail excelImportDetail in listBaseExcelImportDetail)
                            {
                                string value = "";
                                value = item[excelImportDetail.ColumnName].ToString();
                                DateTime dateTime = DateTime.Now;
                                decimal num = 0;
                                #region 单个字段赋值
                                switch (excelImportDetail.DataType)
                                {
                                    //字符串
                                    case "0":
                                        entity[excelImportDetail.FieldName] = value;
                                        break;
                                    //数字
                                    case "1":
                                        if (decimal.TryParse(value, out num))
                                        {
                                            entity[excelImportDetail.FieldName] = value;
                                        }
                                        else
                                        {
                                            if (Baseexcellimport.ErrorHanding == "0")
                                            {
                                                isExit = true;
                                            }
                                            DataRow dr = Newdt.NewRow();
                                            dr = Newdt.NewRow();
                                            dr[0] = errorNum;
                                            dr[1] = "第[" + rowNum.ToString() + "]行[" + excelImportDetail.ColumnName + "]";
                                            dr[2] = "数字格式不正确";
                                            Newdt.Rows.Add(dr);
                                            errorNum++;
                                            continue;
                                        }
                                        break;
                                    //日期
                                    case "2":
                                        if (DateTime.TryParse(value, out dateTime))
                                        {
                                            entity[excelImportDetail.FieldName] = value;
                                        }
                                        else
                                        {
                                            if (Baseexcellimport.ErrorHanding == "0")
                                            {
                                                isExit = true;
                                            }
                                            DataRow dr = Newdt.NewRow();
                                            dr = Newdt.NewRow();
                                            dr[0] = errorNum;
                                            dr[1] = "第[" + rowNum.ToString() + "]行[" + excelImportDetail.ColumnName + "]";
                                            dr[2] = "日期格式不正确";
                                            Newdt.Rows.Add(dr);
                                            errorNum++;
                                            continue;
                                        }
                                        break;
                                    //外键
                                    case "3":
                                        sb.Clear();
                                        sb.Append(" and ");
                                        sb.Append(excelImportDetail.CompareField);
                                        sb.Append("='");
                                        sb.Append(value);
                                        sb.Append("' ");
                                        Hashtable htf = database.FindHashtable(excelImportDetail.ForeignTable, sb);
                                        if (htf.Count > 0)
                                        {
                                            entity[excelImportDetail.FieldName] = htf[excelImportDetail.BackField.ToLower()];
                                        }
                                        else
                                        {
                                            if (Baseexcellimport.ErrorHanding == "0")
                                            {
                                                isExit = true;
                                            }
                                            DataRow dr = Newdt.NewRow();
                                            dr = Newdt.NewRow();
                                            dr[0] = errorNum;
                                            dr[1] = "第[" + rowNum.ToString() + "]行[" + excelImportDetail.ColumnName + "]";
                                            dr[2] = excelImportDetail.ColumnName + "在系统中不存在";
                                            Newdt.Rows.Add(dr);
                                            errorNum++;
                                            continue;
                                        }
                                        break;
                                    //唯一识别
                                    case "4":
                                        //判断该值是否在表中已存在
                                        sb.Clear();
                                        sb.Append(" and ");
                                        sb.Append("='");
                                        sb.Append(value);
                                        sb.Append("' ");
                                        Hashtable htm = database.FindHashtable(Baseexcellimport.ImportTable, sb);
                                        if (htm.Count > 0)
                                        {
                                            if (Baseexcellimport.ErrorHanding == "0")
                                            {
                                                isExit = true;
                                            }
                                            DataRow dr = Newdt.NewRow();
                                            dr = Newdt.NewRow();
                                            dr[0] = errorNum;
                                            dr[1] = "第[" + rowNum.ToString() + "]行[" + excelImportDetail.ColumnName + "]";
                                            dr[2] = excelImportDetail.ColumnName + "在系统中已存在不能重复插入";
                                            Newdt.Rows.Add(dr);
                                            errorNum++;
                                            continue;
                                        }
                                        entity[excelImportDetail.FieldName] = value;
                                        break;
                                    default:
                                        break;
                                }
                                #endregion 单字段赋值结束
                            }
                            #endregion 遍历模板结束
                            if (isExit)
                            {
                                break;
                            }

                            database.Insert(Baseexcellimport.ImportTable, entity, isOpenTrans);
                            rowNum++;
                        }
                        #endregion 遍历Excel数据行结束
                        database.Commit();
                        IsOk = 1;
                    }
                    catch (Exception ex)
                    {
                        database.Rollback();
                        BaseSysLogBll.Instance.WriteLog("", OperationType.Add, "-1", "异常错误：" + ex.Message);
                        IsOk = -1;
                    }
                }
            }
            Result = Newdt;
            return IsOk;
        }
        #endregion

        #region 获得导出模板
        public void GetExcellTemperature(string ImportId, out DataTable data, out string DataColumn, out string fileName)
        {
            DataColumn = "";
            data = new DataTable();
            BaseExcelImport Baseexcelimport = DataFactory.Database().FindEntity<BaseExcelImport>(ImportId);
            fileName = Baseexcelimport.ImportFileName;
            List<BaseExcelImportDetail> listBaseExcelImportDetail = DataFactory.Database().FindList<BaseExcelImportDetail>("ImportId", ImportId);
            object[] rows = new object[listBaseExcelImportDetail.Count];
            int i = 0;
            foreach (BaseExcelImportDetail excelImportDetail in listBaseExcelImportDetail)
            {
                if (DataColumn == "")
                {
                    DataColumn = DataColumn + excelImportDetail.ColumnName;
                }
                else
                {
                    DataColumn = DataColumn + "|" + excelImportDetail.ColumnName;
                }
                switch (excelImportDetail.DataType)
                {
                    //字符串
                    case "0":
                        data.Columns.Add(excelImportDetail.ColumnName, typeof(string));
                        rows[i] = "";
                        break;
                    //数字
                    case "1":
                        data.Columns.Add(excelImportDetail.ColumnName, typeof(decimal));
                        rows[i] = 0;
                        break;
                    //日期
                    case "2":
                        data.Columns.Add(excelImportDetail.ColumnName, typeof(DateTime));
                        rows[i] = DateTime.Now;
                        break;
                    //外键
                    case "3":
                        data.Columns.Add(excelImportDetail.ColumnName, typeof(string));
                        rows[i] = "";
                        break;
                    //唯一识别
                    case "4":
                        data.Columns.Add(excelImportDetail.ColumnName, typeof(string));
                        rows[i] = "";
                        break;
                    default:
                        break;
                }
                i++;
            }
            data.Rows.Add(rows);

        }
        #endregion
    }
}
