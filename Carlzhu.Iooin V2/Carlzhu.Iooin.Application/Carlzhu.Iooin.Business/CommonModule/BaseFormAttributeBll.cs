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
    /// ����������
    /// </summary>
    public class BaseFormAttributeBll : RepositoryFactory<BaseFormAttribute>
    {
        private static BaseFormAttributeBll _item;
        /// <summary>
        /// ��̬��
        /// </summary>
        public static BaseFormAttributeBll Instance => _item ?? (_item = new BaseFormAttributeBll());

        /// <summary>
        /// ��ȡ�б�
        /// </summary>
        /// <param name="moduleId">ģ��Id</param>
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
        /// ���涯̬������
        /// </summary>
        /// <param name="BuildFormJson">��JSON����</param>
        /// <param name="ObjectId">����Id</param>
        /// <param name="ModuleId">ģ��Id</param>
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
                throw new Exception("�Զ������" + ex);
            }
        }
        /// <summary>
        /// ��ȡ��̬������(����JSON)
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

        #region ƴ�ӱ�������html��
        /// <summary>
        /// ���ɱ�table
        /// </summary>
        /// <param name="ColumnCount">�Ű�ģʽ��1����2�б�2����4�У�3���� 6��</param>
        /// <param name="ModuleId">ģ��Id</param>
        /// <returns></returns>
        public string CreateBuildFormTable(int ColumnCount, string ModuleId)
        {
            List<BaseFormAttribute> ListData = this.GetList(ModuleId); ;
            return CreateBuildFormTable(ColumnCount, ListData); ;
        }
        /// <summary>
        /// ���ɱ�table
        /// </summary>
        /// <param name="ColumnCount">�Ű�ģʽ��1����2�б�2����4�У�3���� 6��</param>
        /// <param name="ListData">����Դ</param>
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
                string PropertyName = entity.PropertyName;                                            //��������
                int ControlColspan = CommonHelper.GetInt(entity.ControlColspan);                      //�ϲ���
                FormTable.Append("            <th class='formTitle'>" + PropertyName + "��</th>\r\n");
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
        /// ���ɿؼ�
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string CreateControl(BaseFormAttribute entity)
        {
            StringBuilder sbControl = new StringBuilder();
            string propertyName = entity.PropertyName;                          //��������
            string controlId = entity.ControlId;                                //�ؼ�Id
            string controlType = entity.ControlType;                            //�ؼ�����
            string controlStyle = entity.ControlStyle;                          //�ؼ���ʽ
            string controlValidator = "";
            if (entity.ControlValidator != null)
            {
                controlValidator = entity.ControlValidator.Replace("&nbsp;", "");                  //�ؼ���֤
            }
            string importLength = entity.ImportLength.ToString().Replace("&nbsp;", "");        //���볤��
            string attributesProperty = "";
            if (entity.AttributesProperty != null)
            {
                attributesProperty = entity.AttributesProperty.Replace("&nbsp;", "");              //�Զ�������
            }
            int dataSourceType = CommonHelper.GetInt(entity.DataSourceType);    //�ؼ�����Դ����
            string dataSource = entity.DataSource;                              //�ؼ�����Դ
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
                case "1"://�ı���
                    sbControl.Append("<input id=\"Build_" + controlId + "\" " + maxlength + " type=\"text\" class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + " />");
                    break;
                case "2"://������
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
                case "3"://���ڿ�
                    sbControl.Append("<input id=\"Build_" + controlId + "\" " + maxlength + " type=\"text\" class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + "/>");
                    break;
                case "4"://��  ǩ
                    sbControl.Append("<label id=\"Build_" + controlId + "\" " + attributesProperty + "/>");
                    break;
                case "5"://�����ı���
                    sbControl.Append("<textarea id=\"Build_" + controlId + "\" " + maxlength + " class=\"" + controlStyle + "\" " + controlValidator + " " + attributesProperty + "></textarea>");
                    break;
                default:
                    return "�ڲ����������д���";
            }
            return sbControl.ToString();
        }
        /// <summary>
        /// �������ֵ䣨������
        /// </summary>
        /// <param name="dataSource">�����ֵ�����Դ</param>
        /// <returns></returns>
        public string CreateBindDrop(string dataSource)
        {
            StringBuilder sb = new StringBuilder("<option value=''>==��ѡ��==</option>");
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