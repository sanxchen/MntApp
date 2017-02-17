using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;




using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ��ͼ����Ȩ�ޱ�
    /// </summary>
    public class BaseViewPermissionBll : RepositoryFactory<BaseViewPermission>
    {
        /// <summary>
        /// ��ͼȨ���б�
        /// </summary>
        /// <param name="ObjectId">��������</param>
        /// <param name="Category">�������:1-����2-��ɫ3-��λ4-Ⱥ��</param>
        /// <returns></returns>
        public DataTable GetList(string ObjectId, string Category)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT  v.*,vp.ModuleId AS ObjectId FROM  BaseView v
                                INNER JOIN BaseViewPermission P ON v.ModuleId = P.ModuleId AND v.ViewId = P.ViewId AND p.ObjectId IN ( '" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "' )");
                strSql.Append(" LEFT JOIN ( SELECT DISTINCT *  FROM    BaseViewPermission");
                strSql.Append(" WHERE ObjectId = @ObjectId) vp ON v.ModuleId = vP.ModuleId  AND v.ViewId = vP.ViewId");
            }
            else
            {
                strSql.Append(@"SELECT  v.ViewId ,					--ID
                                        v.ModuleId ,				--ģ��ID
                                        v.ShowName ,				--����
                                        v.SortCode ,				--������
                                        vp.ObjectId					--�Ƿ����
                                FROM    BaseView v
                                        LEFT JOIN BaseViewPermission vp ON vp.ViewId = v.ViewId
                                                                            AND vp.ObjectId = @ObjectId");
            }
            strSql.Append(" order by v.SortCode ASC");
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ���ر����ͼȨ��
        /// </summary>
        /// <param name="ObjectId">��������</param>
        /// <param name="ModuleId">ģ������</param>
        /// <returns></returns>
        public List<BaseView> GetViewList(string ObjectId, string ModuleId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!ManageProvider.Provider.Current().IsSystem)
            {

                strSql.Append(@"SELECT  FieldName,Enabled
                                FROM    BaseView v
                                WHERE   v.ModuleId = @ModuleId AND FieldName NOT IN (
                                SELECT  FieldName FROM    BaseView v INNER JOIN BaseViewPermission P ON v.ModuleId = P.ModuleId AND v.ViewId = P.ViewId");
                strSql.Append(" WHERE   p.ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') AND v.ModuleId = @ModuleId)");
            }
            else
            {
                strSql.Append(@"SELECT FieldName,Enabled FROM BaseView v WHERE 1=0 ");
                strSql.Append(" AND v.ModuleId = @ModuleId");
            }
            strSql.Append(" ORDER BY v.SortCode ASC ");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return DataFactory.Database().FindListBySql<BaseView>(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ���ݶ���Id��ȡģ����ͼȨ���б�
        /// </summary>
        /// <param name="ObjectId">����ID</param>
        /// <returns></returns>
        public DataTable GetViewPermission(string ObjectId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    m.ModuleId AS ID ,
                                                m.ParentId ,
                                                m.FullName ,
                                                m.Icon ,
                                                m.SortCode
                                      FROM      BaseModule m
                                                LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId");
            strSql.Append(@" WHERE     mp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "')");
            strSql.Append(@" UNION
                                      SELECT    v.ViewId AS ID ,
                                                v.ModuleId AS ParentId ,
                                                v.FullName ,
                                                '' AS Icon ,
                                                v.SortCode
                                      FROM      BaseView v
                                                LEFT JOIN BaseViewPermission vp ON vp.ViewId = v.ViewId
                                      WHERE     vp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "') ) A");
            strSql.Append(" ORDER BY SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString());
        }
    }
}