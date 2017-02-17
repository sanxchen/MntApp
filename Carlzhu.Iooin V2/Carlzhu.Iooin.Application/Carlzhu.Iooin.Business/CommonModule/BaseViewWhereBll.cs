using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 视图查询条件表
    /// </summary>
    public class BaseViewWhereBll : RepositoryFactory<BaseViewWhere>
    {
        /// <summary>
        /// 根据模块Id获取视图查询条件列表
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public List<BaseViewWhere> GetViewWhereList(string ModuleId)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            WhereSql.Append(" AND ModuleId = @ModuleId");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
    }
}