using System.Collections.Generic;
using System.Data.Common;
using System.Text;


using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 模块按钮
    /// </summary>
    public class BaseButtonBll : RepositoryFactory<BaseButton>
    {
        /// <summary>
        /// 获取按钮列表
        /// </summary>
        /// <param name="ModuleId">模块ID</param>
        /// <param name="Category">分类：1-工具栏，2：右击栏</param>
        /// <returns></returns>
        public List<BaseButton> GetList(string ModuleId, string Category)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM BaseButton WHERE 1=1");
            strSql.Append(" AND ModuleId = @ModuleId ");
            strSql.Append(" AND Category = @Category ");
            strSql.Append(" ORDER BY SortCode ASC");
            List<DbParameter> parameter = new List<DbParameter>
            {
                DbFactory.CreateDbParameter("@ModuleId", ModuleId),
                DbFactory.CreateDbParameter("@Category", Category)
            };
            return Repository().FindListBySql(strSql.ToString(), parameter.ToArray());
        }
    }
}
