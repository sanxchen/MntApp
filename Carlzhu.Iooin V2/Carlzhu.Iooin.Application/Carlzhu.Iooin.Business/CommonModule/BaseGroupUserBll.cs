using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 用户组管理
    /// </summary>
    public class BaseGroupUserBll : RepositoryFactory<BaseGroupUser>
    {
        /// <summary>
        /// 获取用户组列表
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="departmentId">部门ID</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public DataTable GetPageList(string departmentId, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    gu.GroupUserId ,              --用户组ID
                                                gu.Code ,                     --用户组编码
                                                gu.FullName ,                 --用户组名称
                                                gu.DepartmentId ,             --所在部门Id
                                                dep.FullName AS DepartmentName ,--所在部门
                                                gu.Enabled ,                  --是否有效
                                                gu.Remark ,                   --岗位描述
                                                gu.SortCode                   --排序码
                                      FROM      BaseGroupUser gu
                                                LEFT JOIN BaseDepartment dep ON dep.DepartmentId = gu.DepartmentId
                                    ) T WHERE   1 = 1 ");
    
            if (!string.IsNullOrEmpty(departmentId))
            {
                departmentId = base.Repository().FindChildAllKeys("BaseDepartment", "DepartmentId", departmentId);
                strSql.Append($" AND DepartmentId in ({departmentId})");

                //strSql.Append(" AND DepartmentId = @DepartmentId");
                //parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", DepartmentId));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( GroupUserId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}