using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;







using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.DataAccess.DataBase;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// �ӿڹ���
    /// </summary>
    public class BaseInterfaceManageBll : RepositoryFactory<BaseInterfaceManage>
    {
        /// <summary>
        /// ������־
        /// </summary>
        //public static LogHelper log = LogFactory.GetLogger("InterfaceManageBll");
        /// <summary>
        /// ��ȡ�ӿ��б�
        /// </summary>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public List<BaseInterfaceManage> GetPageList(ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT * FROM BaseInterfaceManage WHERE 1=1 ");
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// ��ȡ�ӿڲ����б�
        /// </summary>
        /// <param name="InterfaceId">�ӿ�����</param>
        /// <returns></returns>
        public List<BaseInterfaceManageParameter> GetInterfaceParameterList(string InterfaceId)
        {
            return DataFactory.Database().FindList<BaseInterfaceManageParameter>("InterfaceId", InterfaceId);
        }
        /// <summary>
        /// �ύ�ӿڱ������������༭��
        /// </summary>
        /// <param name="KeyValue">����</param>
        /// <param name="entity">�ӿڶ���</param>
        /// <param name="ParameterJson">�ӿڲ���</param>
        /// <returns></returns>
        public int SubmitInterfaceForm(string KeyValue, BaseInterfaceManage entity, string ParameterJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = KeyValue == "" ? "�����ɹ���" : "�༭�ɹ���";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    database.Delete<BaseInterfaceManageParameter>("InterfaceId", KeyValue, isOpenTrans);
                    entity.Modify(KeyValue); database.Update(entity, isOpenTrans);
                }
                else
                {
                    entity.Create();
                    database.Insert(entity, isOpenTrans);
                }
                //���Ӳ���
                List<BaseInterfaceManageParameter> InterfaceManageParameterList = ParameterJson.JonsToList<BaseInterfaceManageParameter>();
                foreach (BaseInterfaceManageParameter interfacemanageparameter in InterfaceManageParameterList)
                {
                    if (!string.IsNullOrEmpty(interfacemanageparameter.Field))
                    {
                        interfacemanageparameter.Create();
                        interfacemanageparameter.InterfaceId = entity.InterfaceId;
                        database.Insert(interfacemanageparameter, isOpenTrans);
                    }
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

        #region ִ�ж�̬�ӿ�����
        /// <summary>
        /// ҵ��ӿڵ���
        /// </summary>
        /// <param name="Xml">XML��ʽ</param>
        /// <param name="Token">�Ǻ�</param>
        /// <returns></returns>
        public string Invoke(string Xml, string Token)
        {
            //IDatabase database = DataFactory.Database();
            //DbTransaction isOpenTrans = database.BeginTrans();
            StringBuilder Output = new StringBuilder();
            ArrayList arraySql = new ArrayList();
            ArrayList arrayParam = new ArrayList();
            string Action = "";
            bool IsOk = false;
            try
            {
                AnalysisXml(Xml, out arraySql, out arrayParam, out Action);
                for (int i = 0; i < arraySql.Count; i++)
                {
                    StringBuilder strSql = new StringBuilder(arraySql[i].ToString());
                    DbParameter[] parameter = (DbParameter[])arrayParam[i];
                    switch (Action)
                    {
                        case "Insert":                  //����
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "Add"));
                            break;
                        case "Update":                  //�޸�
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "Edit"));
                            break;
                        case "Delete":                  //ɾ��
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "DEL"));
                            break;
                        case "Select":                  //��ѯ
                            DataTable dt = DataFactory.Database().FindTableBySql(strSql.ToString(), parameter);
                            Output.Append(!DataHelper.IsExistRows(dt)
                                ? DataHelper.DataTableToXML(dt)
                                : ResultMsg(false, "û���ҵ���Ҫ���������"));
                            break;
                        case "Procedure":               //�洢����
                            IsOk = DataFactory.Database().ExecuteByProc(strSql.ToString(), parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "PROC"));
                            break;
                        case "DataTableProc":           //�洢����-FindTableByProc
                            DataTable dtProc = DataFactory.Database().FindTableByProc(strSql.ToString(), parameter);
                            Output.Append(!DataHelper.IsExistRows(dtProc)
                                ? DataHelper.DataTableToXML(dtProc)
                                : ResultMsg(false, "û���ҵ���Ҫ���������"));
                            break;
                        case "DataSetProc":             //�洢����-DataSetByProc
                            DataSet dsProc = DataFactory.Database().FindDataSetByProc(strSql.ToString(), parameter);
                            Output.Append(dsProc != null
                                ? DataHelper.DataSetToXML(dsProc)
                                : ResultMsg(false, "û���ҵ���Ҫ���������"));
                            break;
                        case "-1"://�쳣��Ϣ
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Output.Append(ResultMsg(false, ex.Message));
            }
            return GZipHelper.Compress(Output.ToString());
        }
        /// <summary>
        /// ����XML�ַ�����ʽ
        /// </summary>
        /// <param name="Xml"></param>
        /// <param name="arrSql">����SQL</param>
        /// <param name="arrParam">���ز�����</param>
        /// <param name="Action">��������</param>
        private void AnalysisXml(string Xml, out ArrayList arrSql, out ArrayList arrParam, out string Action)
        {
            Xml = GZipHelper.Uncompress(Xml);
            ArrayList _arrSql = new ArrayList();
            ArrayList _arrParam = new ArrayList();
            string _Action = "-1";
            try
            {
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(Xml);
                foreach (XmlNode node in xd.ChildNodes)
                {
                    if (node.Name.Trim() == "Request")
                    {
                        foreach (XmlNode subnode in node.ChildNodes)
                        {
                            if (subnode.Name.Trim() == "data")
                            {
                                //���ݽӿڴ����ȡ����
                                BaseInterfaceManage interfacemanage = Repository().FindEntity("Code", subnode.Attributes["code"].Value);
                                if (interfacemanage.InterfaceId != null)
                                {
                                    _Action = interfacemanage.Action;                                                 //��������
                                    string Constraint = interfacemanage.Constraints;                                  //Լ����ʽ
                                    foreach (XmlNode node_data in subnode.ChildNodes)
                                    {
                                        if (node_data.Name.Trim() == "no")
                                        {
                                            string pkName;
                                            string pkVal;
                                            switch (_Action)
                                            {
                                                case "Insert"://����
                                                    Hashtable ht_add = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(DatabaseCommon.InsertSql(Constraint.Trim(), ht_add));
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_add));
                                                    break;
                                                case "Update"://�޸�
                                                    Hashtable ht_edit = this.XmlNodeToHashtable(node_data, out pkName, out pkVal);
                                                    _arrSql.Add(new StringBuilder(Constraint.Trim()));
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_edit));
                                                    break;
                                                case "Delete"://ɾ��
                                                    Hashtable ht_Delete = this.XmlNodeToHashtable(node_data, out pkName, out pkVal);
                                                    _arrSql.Add(DatabaseCommon.DeleteSql(Constraint.Trim(), pkName));
                                                    _arrParam.Add(DbFactory.CreateDbParameter("@" + pkName, pkVal));
                                                    break;
                                                case "Select"://��ѯ
                                                    Hashtable ht_Param = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_Param));
                                                    break;
                                                case "Procedure"://�洢����
                                                    Hashtable ExecuteByProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(ExecuteByProc);
                                                    break;
                                                case "DataTableProc"://�洢����-DataTableProc
                                                    Hashtable DataTableProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DataTableProc);
                                                    break;
                                                case "DataSetProc"://�洢����-DataSetProc
                                                    Hashtable DataSetProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DataSetProc);
                                                    break;
                                                case "-1"://�쳣��Ϣ
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //log.Error(Xml.ToString() + "\r\n�ӿڲ�����\r\n");
                                }
                            }
                        }
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                //log.Error(Xml.ToString() + "\r\n" + e.Message + "\r\n");
            }
            finally
            {
                arrSql = _arrSql;
                arrParam = _arrParam;
                Action = _Action;
            }
        }

        /// <summary>
        /// XML�ڵ�ת��Hashtable
        /// </summary>
        /// <param name="node">XML�ڵ�</param>
        /// <param name="pkName">��������</param>
        /// <param name="pkVal">��������ֵ</param>
        /// <returns>����XML�ڵ�</returns>
        public Hashtable XmlNodeToHashtable(XmlNode node, out string pkName, out string pkVal)
        {
            string _pkName = "";
            string _pkVal = "";
            bool isFirstValue = true;
            Hashtable ht = new Hashtable();
            foreach (XmlNode node_info in node.ChildNodes)
            {
                if (isFirstValue)//<no>�����ǩ�̶���һ������
                {
                    isFirstValue = false;
                    _pkName = node_info.Name.Trim();
                    _pkVal = node_info.InnerText.Trim();
                }
                ht[node_info.Name.Trim()] = node_info.InnerText.Trim();
            }
            pkName = _pkName;
            pkVal = _pkVal;
            return ht;
        }
        /// <summary>
        /// XML�ڵ�ת��Hashtable
        /// </summary>
        /// <param name="node">XML�ڵ�</param>
        /// <returns>����XML�ڵ�</returns>
        public Hashtable XmlNodeToHashtable(XmlNode node)
        {
            Hashtable ht = new Hashtable();
            foreach (XmlNode node_info in node.ChildNodes)
            {
                ht[node_info.Name.Trim()] = node_info.InnerText.Trim();
            }
            return ht;
        }
        /// <summary>
        /// ���ؽ��
        /// </summary>
        /// <param name="result">true:�ɹ���false:ʧ��</param>
        /// <param name="reason">ԭ��</param>
        /// <returns></returns>
        private string ResultMsg(bool result, string reason)
        {
            StringBuilder strResponse = new StringBuilder();
            strResponse.Append("<Response>");
            strResponse.Append("<result>" + result + "</result>");
            strResponse.Append("<reason>" + reason + "</reason>");
            strResponse.Append("</Response>");
            return strResponse.ToString();
        }
        #endregion
    }
}