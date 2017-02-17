using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;




using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 表单附加属性
    /// </summary>
    public class BaseFormAttributeBll : RepositoryFactory<BaseFormAttribute>
    {
        private static BaseFormAttributeBll _item;
        /// <summary>
        /// 静态化
        /// </summary>
        public static BaseFormAttributeBll Instance => _item ?? (_item = new BaseFormAttributeBll());

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="moduleId">模块Id</param>
        /// <returns></returns>
        public List<BaseFormAttribute> GetList(string moduleId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append("SELECT * FROM BaseFormAttribute WHERE 1=1");
            strSql.Append(" AND ModuleId = @ModuleId AND Enabled=1");
            strSql.Append(" ORDER BY SortCode");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", moduleId));
            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 保存动态表单数据
        /// </summary>
        /// <param name="BuildFormJson">表单JSON对象</param>
        /// <param name="ObjectId">对象Id</param>
        /// <param name="ModuleId">模块Id</param>
        public void SaveBuildForm(string BuildFormJson, string ObjectId, string ModuleId, DbTransaction isOpenTrans)
        {
            try
            {
                BaseFormAttributeValue formattributevalue = new BaseFormAttributeValue();
                formattributevalue.Create();
                formattributevalue.ObjectId = ObjectId;
                formattributevalue.ModuleId = ModuleId;
                formattributevalue.ObjectParameterJson = BuildFormJson;
                DataFactory.Database().Delete<BaseFormAttributeValue>("ObjectId", ObjectId, isOpenTrans);
                DataFactory.Database().Insert<BaseFormAttributeValue>(formattributevalue, isOpenTrans);
            }
            catch (Exception ex)
            {
                throw new Exception("自定义表单，" + ex);
            }
        }
        /// <summary>
        /// 获取动态表单数据(返回JSON)
        /// </summary>
        /// <returns></returns>
        public string GetBuildForm(string objectId)
        {
            BaseFormAttributeValue formattributevalue = DataFactory.Database().FindEntity<BaseFormAttributeValue>("ObjectId", objectId);
            if (!string.IsNullOrEmpty(formattributevalue.ObjectParameterJson) && formattributevalue.ObjectParameterJson.Length > 2)
            {
                return formattributevalue.ObjectParameterJson.Replace("{", "").Replace("}", "") + ",";
            }
            else
            {
                return "";
            }
        }

        #region 拼接表单（返回html）
        /// <summary>
        /// 生成表单table
        /// </summary>
        /// <param name="ColumnCount">排版模式：1代表2列表，2代表4列，3代表 6列</param>
        /// <param name="ModuleId">模块Id</param>
        /// <returns></returns>
        public string CreateBuildFormTable(int ColumnCount, string ModuleId)
        {
            List<BaseFormAttribute> ListData = this.GetList(ModuleId); ;
            return CreateBuildFormTable(ColumnCount, ListData); ;
        }
        /// <summary>
        /// 生成表单table
        /// </summary>
        /// <param name="ColumnCount">排版模式：1代表2列表，2代表4列，3代表 6列</param>
        /// <param name="ListData">数据源</param>
        /// <returns></returns>
        public string CreateBuildFormTable(int ColumnCount, List<BaseFormAttribute> ListData)
        {
            StringBuilder FormTable = new StringBuilder();
            FormTable.Append("<table class=\"form\">\r\n        <tr>\r\n");
            int RowIndex = 1;
            foreach (BaseFormAttribute entity in ListData)
            {
                if (entity.PropertyName == null && entity.ControlId == null)
                {
                    continue;
                }
                string PropertyName = entity.PropertyName;                                            //属性名称
                int ControlColspan = CommonHelper.GetInt(entity.ControlColspan);                      //合并列
                FormTable.Append("            <th class='formTitle'>" + PropertyName + "：</th>\r\n");
                if (ControlColspan == 0)
                {
                    FormTable.Append("            <td class='formValue'>\r\n                " + CreateControl(entity) + "\r\n            </td>\r\n");
                }
                else
                {
                    FormTable.Append("            <td colspan='" + ControlColspan + "' class='formValue'>\r\n                " + CreateControl(entity) + "\r\n            </td>\r\n");
                    FormTable.Append("        </tr>\r\n        <tr>\r\n");
                    RowIndex += ControlColspan - 1;
                }
                if (RowIndex % ColumnCount == 0)
                {
                    FormTable.Append("        </tr>\r\n        <tr>\r\n");
                }
                RowIndex++;
            }
            FormTable.Append("        </tr>\r\n    </table>\r\n");
            return FormTable.ToString().Replace("<tr>\r\n</tr>", "");
        }
        /// <summary>
        /// 生成控件
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string CreateControl(BaseFormAttribute entity)
        {
            StringBuilder sbControl = new StringBuilder();
            string propertyName = entity.PropertyName;                          //属性名称
            string controlId = entity.ControlId;                                //控件Id
            string controlType = entity.ControlType;                            //控件类型
            string controlStyle = entity.ControlStyle;                          //控件样式
            string controlValidator = "";
            if (entity.ControlValidator != null)
            {
                controlValidator = entity.ControlValidator.Replace("&nbsp;", "");                  //控件验证
            }
            string importLength = entity.ImportLength.ToString().Replace("&nbsp;", "");        //输入长度
            string attributesProperty = "";
            if (entity.AttributesProperty != null)
            {
                attributesProperty = entity.AttributesProperty.Replace("&nbsp;", "");              //自定义属性
            }
            int dataSourceType = CommonHelper.GetInt(entity.DataSourceType);    //控件数据源类型
            string dataSource = entity.DataSource;                              //控件数据源
            if (!string.IsNullOrEmpty(controlValidator.Trim()))
            {
                controlValidator = "datacol=\"yes\" err=\"" + propertyName + "\" checkexpession=\"" + controlValidator + "\"";
            }
            string maxlength = "";
            if (!string.IsNullOrEmpty(importLength))
            {
                maxlength = "maxlength=" + importLength + "";
            }
            switch (controlType)
            {
                case "1"://文本框
                    sbControl.Append("<input id=\"Build_" + controlId + "\" " + maxlength + " type=\"text\" class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + " />");
                    break;
                case "2"://下拉框
                    if (!string.IsNullOrEmpty(dataSource))
                    {
                        if (dataSourceType == 0)
                        {
                            sbControl.Append("<select id=\"Build_" + controlId + "\"class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + ">");
                            sbControl.Append(dataSource);
                            sbControl.Append("</select>");
                        }
                        else
                        {
                            sbControl.Append("<select dictionaryValue=\"" + dataSource + "\" dictionary=\"yes\" id=\"Build_" + controlId + "\"class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + ">");
                            sbControl.Append(CreateBindDrop(dataSource));
                            sbControl.Append("</select>");
                        }
                    }
                    else
                    {
                        sbControl.Append("<select id=\"Build_" + controlId + "\"class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + ">");
                        sbControl.Append("</select>");
                    }
                    break;
                case "3"://日期框
                    sbControl.Append("<input id=\"Build_" + controlId + "\" " + maxlength + " type=\"text\" class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + "/>");
                    break;
                case "4"://标  签
                    sbControl.Append("<label id=\"Build_" + controlId + "\" " + attributesProperty + "/>");
                    break;
                case "5"://多行文本框
                    sbControl.Append("<textarea id=\"Build_" + controlId + "\" " + maxlength + " class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + "></textarea>");
                    break;
                default:
                    return "内部错误，配置有错误";
            }
            return sbControl.ToString();
        }
        /// <summary>
        /// 绑定数据字典（下拉框）
        /// </summary>
        /// <param name="dataSource">数据字典数据源</param>
        /// <returns></returns>
        public string CreateBindDrop(string dataSource)
        {
            StringBuilder sb = new StringBuilder("<option value=''>==请选择==</option>");
            BaseDataDictionaryBll baseDatadictionarybll = new BaseDataDictionaryBll();
            List<BaseDataDictionaryDetail> listData = baseDatadictionarybll.GetDataDictionaryDetailListByCode(dataSource);
            if (listData == null) return sb.ToString();
            foreach (BaseDataDictionaryDetail item in listData)
            {
                sb.Append("<option value=\"" + item.Code + "\">" + item.FullName + "</option>");
            }
            return sb.ToString();
        }
        #endregion
    }
}