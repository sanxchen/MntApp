using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;




using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ģ��Ȩ�ޱ�
    /// </summary>
    public class BaseModulePermissionBll : RepositoryFactory<BaseModulePermission>
    {
        private static BaseModulePermissionBll _item;
        public static BaseModulePermissionBll Instance => _item ?? (_item = new BaseModulePermissionBll());

        /// <summary>
        /// ģ��Ȩ���б�
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
                strSql.Append(@"SELECT  m.ModuleId ,				--ģ��ID
                                        m.ParentId ,				--ģ��ڵ�
                                        m.Code ,					--����
                                        m.FullName ,				--����
                                        m.Icon ,					--ͼ��
                                        m.SortCode ,				--������
                                        cp.ModuleId AS ObjectId     --�Ƿ����
                                FROM    BaseModule M INNER JOIN ( SELECT DISTINCT ModuleId  FROM   BaseModulePermission");
                strSql.Append(" WHERE  ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')) MP ON M.ModuleId = mp.ModuleId");
                strSql.Append(" LEFT JOIN ( SELECT DISTINCT ModuleId  FROM  BaseModulePermission");
                strSql.Append(" WHERE  ObjectId = @ObjectId ) CP ON cp.ModuleId = M.ModuleId");
            }
            else
            {
                strSql.Append(@"SELECT  m.ModuleId ,				--ģ��ID
                                        m.ParentId ,				--ģ��ڵ�
                                        m.Code ,					--����
                                        m.FullName ,				--����
                                        m.Icon ,					--ͼ��
                                        m.SortCode ,				--������
                                        mp.ObjectId					--�Ƿ����
                                FROM    BaseModule m
                                        LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId
                                                                          AND mp.ObjectId = @ObjectId");
            }
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            strSql.Append(" ORDER BY  m.SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ����Ȩ��ģ��
        /// </summary>
        /// <param name="objectId">��������</param>
        /// <returns></returns>
        public List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> GetModuleList(string objectId)
        {
            StringBuilder strSql = new StringBuilder();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT DISTINCT  M.* FROM BaseModule M");
                strSql.Append(" INNER JOIN BaseModulePermission MP ON M.ModuleId = MP.ModuleId WHERE   ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            }
            else
            {
                strSql.Append(@"SELECT * FROM BaseModule M");
            }
            strSql.Append(" ORDER BY  M.SortCode ASC ");
            return DataFactory.Database().FindListBySql<Carlzhu.Iooin.Entity.CommonModule.BaseModule>(strSql.ToString());
        }
        /// <summary>
        /// ���ݶ���Id��ȡģ��Ȩ���б�
        /// </summary>
        /// <param name="objectId">����ID</param>
        /// <returns></returns>
        public DataTable GetModulePermission(string objectId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  m.ModuleId ,
                                    m.ParentId ,
                                    m.FullName ,
                                    m.Icon
                            FROM    BaseModule m
                                    LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId");
            strSql.Append(" WHERE   mp.ObjectId IN ('" + objectId.Replace(",", "','") + "')");
            strSql.Append(" ORDER BY  m.SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString());
        }

        /// <summary>
        /// Actionִ��Ȩ����֤
        /// </summary>
        /// <param name="Action">��ͼAction</param>
        /// <param name="ObjectId">��������</param>
        /// <param name="ModuleId">ģ��Id</param>
        /// <param name="UserId">�û�Id</param>
        /// <returns></returns>
        public bool ActionAuthorize(string Action, string ObjectId, string ModuleId, string UserId)
        {
            List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> listData;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  b.ModuleId,b.FullName ,b.ActionEvent AS Location FROM    BaseButton b INNER JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId AND bp.ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            strSql.Append(" UNION ");
            strSql.Append(@"SELECT  m.ModuleId,m.FullName , m.Location FROM    BaseModule m INNER JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId  AND mp.ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            listData = DataFactory.Database().FindListBySql<Carlzhu.Iooin.Entity.CommonModule.BaseModule>(strSql.ToString());
            listData = (from entity in listData
                        where (entity.Location.ToLower() == Action && entity.ModuleId == ModuleId)
                        select entity).ToList();
            int count = listData.Count;
            return count > 0;
        }
    }
}