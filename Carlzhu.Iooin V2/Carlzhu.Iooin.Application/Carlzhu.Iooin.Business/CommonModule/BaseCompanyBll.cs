using System.Collections.Generic;
using System.Text;


using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 公司管理
    /// </summary>
    public class BaseCompanyBll : RepositoryFactory<BaseCompany>
    {
        /// <summary>
        /// 获取公司列表
        /// </summary>
        /// <returns></returns>
        public List<BaseCompany> GetList()
        {
            StringBuilder WhereSql = new StringBuilder();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                WhereSql.Append(" AND ( CompanyId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                WhereSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                WhereSql.Append(" ) )");
            }
            WhereSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString());
        }
    }
}