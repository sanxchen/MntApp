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
    /// ������ťȨ�ޱ�
    /// </summary>
    public class BaseButtonPermissionBll : RepositoryFactory<BaseButtonPermission>
    {
        /// <summary>
        /// ��ťȨ���б�
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
                strSql.Append(@"SELECT  b.ButtonId ,				--��ťID
                                        b.ModuleId ,				--ģ��ID
                                        b.Code ,					--����
                                        b.FullName ,				--����
                                        b.Category ,				--����
                                        b.Icon ,					--ͼ��
                                        b.SortCode ,				--������
                                        cp.ModuleButtonId AS ObjectId					--�Ƿ����
                                FROM    BaseButton b INNER JOIN ( SELECT DISTINCT ModuleButtonId  FROM   BaseButtonPermission");
                strSql.Append(" WHERE  ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')) bp ON B.ButtonId = bp.ModuleButtonId");
                strSql.Append(" LEFT JOIN ( SELECT DISTINCT ModuleButtonId  FROM  BaseButtonPermission");
                strSql.Append(" WHERE  ObjectId = @ObjectId ) cp ON cp.ModuleButtonId = b.ButtonId");
            }
            else
            {
                strSql.Append(@"SELECT  b.ButtonId ,				--��ťID
                                    b.ModuleId ,				--ģ��ID
                                    b.Code ,					--����
                                    b.FullName ,				--����
                                    b.Category ,				--����
                                    b.Icon ,					--ͼ��
                                    b.SortCode ,				--������
                                    bp.ObjectId					--�Ƿ����
                            FROM    BaseButton b
                                    LEFT JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId
                                                                          AND bp.ObjectId = @ObjectId");
            }
            strSql.Append(" order by b.SortCode ASC");
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ���ذ�ťȨ��
        /// </summary>
        /// <param name="ObjectId">��������</param>
        /// <param name="ModuleId"ģ������</param>
        /// <returns></returns>
        public List<BaseButton> GetButtonList(string ObjectId, string ModuleId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT DISTINCT B.* FROM BaseButton B");
                strSql.Append(" INNER JOIN BaseButtonPermission BP ON B.ButtonId = BP.ModuleButtonId WHERE ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            }
            else
            {
                strSql.Append(@"SELECT * FROM BaseButton B WHERE 1=1 ");
            }
            strSql.Append(" AND B.ModuleId = @ModuleId");
            strSql.Append(" ORDER BY B.SortCode ASC ");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return DataFactory.Database().FindListBySql<BaseButton>(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ���ݶ���Id��ȡģ�鰴ťȨ���б�
        /// </summary>
        /// <param name="ObjectId">����ID</param>
        /// <returns></returns>
        public DataTable GetButtonePermission(string ObjectId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    m.ModuleId AS ID ,
                                                m.ParentId ,
                                                m.FullName ,
                                                m.Icon ,
                                                m.SortCode,
                                                m.Category,
                                                'ģ��' AS Sort
                                      FROM      BaseModule m
                                                LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId");
            strSql.Append(@" WHERE     mp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "')");
            strSql.Append(@" UNION     SELECT    b.ButtonId AS ID ,
                                                b.ModuleId AS ParentId ,
                                                b.FullName ,
                                                b.Icon ,
                                                b.SortCode,
                                                b.Category,
                                                '��ť' AS Sort
                                      FROM      BaseButton b
                                                LEFT JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId");
            strSql.Append(" WHERE     bp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "')) A");
            strSql.Append(" ORDER BY SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString());
        }
    }
}