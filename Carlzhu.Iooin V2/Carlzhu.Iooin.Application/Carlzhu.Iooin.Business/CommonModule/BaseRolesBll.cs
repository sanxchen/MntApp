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
    /// 角色管理
    /// </summary>
    public class BaseRolesBll : RepositoryFactory<BaseRoles>
    {
        /// <summary>
        /// 根据公司id获取角色 列表
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public DataTable GetPageList(string CompanyId, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    r.RoleId ,					--主键
                                                r.CompanyId ,				--所属公司Id
                                                c.FullName AS CompanyName ,	--所属公司
                                                r.Code ,					--编码
                                                r.FullName ,				--名称
                                                isnull(U.Qty,0) AS MemberCount,--成员人数
                                                r.Category ,			    --分类
                                                r.Enabled ,					--有效
                                                r.SortCode ,				--排序码
                                                r.Remark					--说明
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