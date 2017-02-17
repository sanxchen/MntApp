using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ��ɫ����
    /// </summary>
    public class BaseRolesBll : RepositoryFactory<BaseRoles>
    {
        /// <summary>
        /// ���ݹ�˾id��ȡ��ɫ �б�
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public DataTable GetPageList(string CompanyId, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    r.RoleId ,					--����
                                                r.CompanyId ,				--������˾Id
                                                c.FullName AS CompanyName ,	--������˾
                                                r.Code ,					--����
                                                r.FullName ,				--����
                                                isnull(U.Qty,0) AS MemberCount,--��Ա����
                                                r.Category ,			    --����
                                                r.Enabled ,					--��Ч
                                                r.SortCode ,				--������
                                                r.Remark					--˵��
                                      FROM      BaseRoles r
                                                LEFT JOIN BaseCompany c ON c.CompanyId = r.CompanyId
                                                LEFT JOIN ( SELECT  COUNT(1) AS Qty ,
                                                                    ObjectId
                                                            FROM    BaseObjectUserRelation
                                                            WHERE   Category = '2'
                                                            GROUP BY ObjectId
                                                          ) U ON U.ObjectId = R.RoleId
                                    ) T WHERE 1=1 ");
            if (!string.IsNullOrEmpty(CompanyId))
            {

                CompanyId = base.Repository().FindChildAllKeys("BaseCompany", "CompanyId", CompanyId);
                strSql.Append($" AND DepartmentId in ({CompanyId})");

                //strSql.Append(" AND CompanyId = @CompanyId");
                //parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            }
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{
            //    strSql.Append(" AND ( RoleId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
            //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
            //    strSql.Append(" ) )");
            //}
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}