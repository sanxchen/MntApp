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
    /// ��ѯ������¼
    /// </summary>
    public class BaseQueryRecordBll : RepositoryFactory<BaseQueryRecord>
    {
        /// <summary>
        /// ����������ȡ�����б�
        /// </summary>
        /// <param name="ModuleId">ģ��ID</param>
        /// <param name="CreateUserId">�û�ID</param>
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
        /// ���ó�ʼ��Ĭ�Ϸ���
        /// </summary>
        /// <param name="ModuleId">ģ��ID</param>
        /// <param name="QueryRecordId">����</param>
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