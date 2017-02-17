using System.Collections.Generic;
using System.Data.Common;
using System.Text;





using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 查询条件记录
    /// </summary>
    public class BaseQueryRecordBll : RepositoryFactory<BaseQueryRecord>
    {
        /// <summary>
        /// 根据条件获取方案列表
        /// </summary>
        /// <param name="ModuleId">模块ID</param>
        /// <param name="CreateUserId">用户ID</param>
        /// <returns></returns>
        public List<BaseQueryRecord> GetList(string ModuleId, string CreateUserId)
        {
            StringBuilder WhereSql = new StringBuilder();
            WhereSql.Append(" AND CreateUserId = @CreateUserId ");
            WhereSql.Append(" AND ModuleId = @ModuleId Order By CreateDate Desc");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateUserId", CreateUserId));
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return DataFactory.Database().FindList<BaseQueryRecord>(WhereSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 设置初始化默认方案
        /// </summary>
        /// <param name="ModuleId">模块ID</param>
        /// <param name="QueryRecordId">主键</param>
        /// <returns></returns>
        public int DefaultProject(string ModuleId, string QueryRecordId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                StringBuilder strSql = new StringBuilder();
                strSql.Append(string.Format("UPDATE BaseQueryRecord SET NextDefault = 0 WHERE ModuleId = '{0}'", ModuleId));
                database.ExecuteBySql(strSql, isOpenTrans);
                BaseQueryRecord entity = new BaseQueryRecord();
                if (!string.IsNullOrEmpty(QueryRecordId))
                {
                    entity.QueryRecordId = QueryRecordId;
                    entity.NextDefault = 1;
                    database.Update(entity, isOpenTrans);
                }
                database.Commit();
                return 1;
            }
            catch
            {
                database.Rollback();
                return -1;
            }
        }
    }
}