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
    /// 接口管理
    /// </summary>
    public class BaseInterfaceManageBll : RepositoryFactory<BaseInterfaceManage>
    {
        /// <summary>
        /// 调试日志
        /// </summary>
        //public static LogHelper log = LogFactory.GetLogger("InterfaceManageBll");
        /// <summary>
        /// 获取接口列表
        /// </summary>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public List<BaseInterfaceManage> GetPageList(ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT * FROM BaseInterfaceManage WHERE 1=1 ");
            return Repository().FindListPageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// 获取接口参数列表
        /// </summary>
        /// <param name="InterfaceId">接口主键</param>
        /// <returns></returns>
        public List<BaseInterfaceManageParameter> GetInterfaceParameterList(string InterfaceId)
        {
            return DataFactory.Database().FindList<BaseInterfaceManageParameter>("InterfaceId", InterfaceId);
        }
        /// <summary>
        /// 提交接口表单（新增、编辑）
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="entity">接口对象</param>
        /// <param name="ParameterJson">接口参数</param>
        /// <returns></returns>
        public int SubmitInterfaceForm(string KeyValue, BaseInterfaceManage entity, string ParameterJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string Message = KeyValue == "" ? "新增成功。" : "编辑成功。";
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
                //添加参数
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

        #region 执行动态接口配置
        /// <summary>
        /// 业务接口调用
        /// </summary>
        /// <param name="Xml">XML格式</param>
        /// <param name="Token">记号</param>
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
                        case "Insert":                  //新增
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "Add"));
                            break;
                        case "Update":                  //修改
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "Edit"));
                            break;
                        case "Delete":                  //删除
                            IsOk = DataFactory.Database().ExecuteBySql(strSql, parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "DEL"));
                            break;
                        case "Select":                  //查询
                            DataTable dt = DataFactory.Database().FindTableBySql(strSql.ToString(), parameter);
                            Output.Append(!DataHelper.IsExistRows(dt)
                                ? DataHelper.DataTableToXML(dt)
                                : ResultMsg(false, "没有找到您要的相关数据"));
                            break;
                        case "Procedure":               //存储过程
                            IsOk = DataFactory.Database().ExecuteByProc(strSql.ToString(), parameter) >= 0 ? true : false;
                            Output.Append(ResultMsg(IsOk, "PROC"));
                            break;
                        case "DataTableProc":           //存储过程-FindTableByProc
                            DataTable dtProc = DataFactory.Database().FindTableByProc(strSql.ToString(), parameter);
                            Output.Append(!DataHelper.IsExistRows(dtProc)
                                ? DataHelper.DataTableToXML(dtProc)
                                : ResultMsg(false, "没有找到您要的相关数据"));
                            break;
                        case "DataSetProc":             //存储过程-DataSetByProc
                            DataSet dsProc = DataFactory.Database().FindDataSetByProc(strSql.ToString(), parameter);
                            Output.Append(dsProc != null
                                ? DataHelper.DataSetToXML(dsProc)
                                : ResultMsg(false, "没有找到您要的相关数据"));
                            break;
                        case "-1"://异常信息
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
        /// 解析XML字符串格式
        /// </summary>
        /// <param name="Xml"></param>
        /// <param name="arrSql">返回SQL</param>
        /// <param name="arrParam">返回参数化</param>
        /// <param name="Action">操作动作</param>
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
                                //根据接口代码获取对象
                                BaseInterfaceManage interfacemanage = Repository().FindEntity("Code", subnode.Attributes["code"].Value);
                                if (interfacemanage.InterfaceId != null)
                                {
                                    _Action = interfacemanage.Action;                                                 //动作类型
                                    string Constraint = interfacemanage.Constraints;                                  //约束达式
                                    foreach (XmlNode node_data in subnode.ChildNodes)
                                    {
                                        if (node_data.Name.Trim() == "no")
                                        {
                                            string pkName;
                                            string pkVal;
                                            switch (_Action)
                                            {
                                                case "Insert"://新增
                                                    Hashtable ht_add = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(DatabaseCommon.InsertSql(Constraint.Trim(), ht_add));
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_add));
                                                    break;
                                                case "Update"://修改
                                                    Hashtable ht_edit = this.XmlNodeToHashtable(node_data, out pkName, out pkVal);
                                                    _arrSql.Add(new StringBuilder(Constraint.Trim()));
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_edit));
                                                    break;
                                                case "Delete"://删除
                                                    Hashtable ht_Delete = this.XmlNodeToHashtable(node_data, out pkName, out pkVal);
                                                    _arrSql.Add(DatabaseCommon.DeleteSql(Constraint.Trim(), pkName));
                                                    _arrParam.Add(DbFactory.CreateDbParameter("@" + pkName, pkVal));
                                                    break;
                                                case "Select"://查询
                                                    Hashtable ht_Param = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DatabaseCommon.GetParameter(ht_Param));
                                                    break;
                                                case "Procedure"://存储过程
                                                    Hashtable ExecuteByProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(ExecuteByProc);
                                                    break;
                                                case "DataTableProc"://存储过程-DataTableProc
                                                    Hashtable DataTableProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DataTableProc);
                                                    break;
                                                case "DataSetProc"://存储过程-DataSetProc
                                                    Hashtable DataSetProc = this.XmlNodeToHashtable(node_data);
                                                    _arrSql.Add(Constraint.Trim());
                                                    _arrParam.Add(DataSetProc);
                                                    break;
                                                case "-1"://异常信息
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //log.Error(Xml.ToString() + "\r\n接口不存在\r\n");
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
        /// XML节点转换Hashtable
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <param name="pkName">返回主键</param>
        /// <param name="pkVal">返回主键值</param>
        /// <returns>返回XML节点</returns>
        public Hashtable XmlNodeToHashtable(XmlNode node, out string pkName, out string pkVal)
        {
            string _pkName = "";
            string _pkVal = "";
            bool isFirstValue = true;
            Hashtable ht = new Hashtable();
            foreach (XmlNode node_info in node.ChildNodes)
            {
                if (isFirstValue)//<no>里面标签固定第一行主键
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
        /// XML节点转换Hashtable
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <returns>返回XML节点</returns>
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
        /// 返回结果
        /// </summary>
        /// <param name="result">true:成功，false:失败</param>
        /// <param name="reason">原因</param>
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