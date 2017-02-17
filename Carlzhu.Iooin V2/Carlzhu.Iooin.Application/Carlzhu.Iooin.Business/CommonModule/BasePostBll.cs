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
    /// 岗位管理
    /// </summary>
    public class BasePostBll : RepositoryFactory<BasePost>
    {
        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <param name="CompanyId">部门ID</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public DataTable GetPageList(string CompanyId, ref Pagination jqgridparam)
        {
            //循环查询父级主建
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    post.PostId ,                   --岗位ID
                                                post.Code ,                     --岗位编码
                                                post.FullName ,                 --岗位名称
                                                post.CompanyId ,             --所在部门Id
                                                com.FullName AS CompanyName ,--所在部门
                                                post.Enabled ,                  --是否有效
                                                post.Remark ,                   --岗位描述
                                                post.SortCode                   --排序码
                                      FROM      BasePost post
                                                LEFT JOIN BaseCompany com ON com.CompanyId = post.CompanyId
                                    ) T
                            WHERE   1 = 1 ");
            if (!string.IsNullOrEmpty(CompanyId))
            {
                CompanyId = base.Repository().FindChildAllKeys("BaseCompany", "CompanyId", CompanyId);
                strSql.Append($" AND CompanyId in ({CompanyId})");
                //strSql.Append(" AND DepartmentId in (@DepartmentId)");
                //parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", departmentId));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( PostId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}