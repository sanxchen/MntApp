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
    /// 首页快捷方式
    /// </summary>
    public class BaseShortcutsBll : RepositoryFactory<BaseShortcuts>
    {
        /// <summary>
        /// 获取首页快捷方式列表
        /// </summary>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        public List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> GetShortcutList(string UserId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    BaseModule M
                                    RIGHT JOIN BaseShortcuts S ON s.ModuleId = M.ModuleId
                            WHERE   S.CreateUserId = @CreateUserId
                            ORDER BY M.SortCode");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@CreateUserId", UserId));
            return DataFactory.Database().FindListBySql<Carlzhu.Iooin.Entity.CommonModule.BaseModule>(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 快捷方式（新增、编辑、删除）
        /// </summary>
        /// <param name="ModuleId">模块Id</param>
        /// <returns></returns>
        public int SubmitForm(string ModuleId, string UserId)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                string[] array = ModuleId.Split(',');
                database.Delete<BaseShortcuts>("CreateUserId", UserId, isOpenTrans);
                foreach (string item in array)
                {
                    if (item.Length>0)
                    {
                        BaseShortcuts entity = new BaseShortcuts();
                        entity.Create();
                        entity.ModuleId = item;
                        entity.CreateUserId = UserId;
                        database.Insert(entity);
                    }
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