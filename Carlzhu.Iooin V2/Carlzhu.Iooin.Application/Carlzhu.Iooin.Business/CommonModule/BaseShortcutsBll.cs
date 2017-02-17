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
    /// ��ҳ��ݷ�ʽ
    /// </summary>
    public class BaseShortcutsBll : RepositoryFactory<BaseShortcuts>
    {
        /// <summary>
        /// ��ȡ��ҳ��ݷ�ʽ�б�
        /// </summary>
        /// <param name="UserId">�û�Id</param>
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
        /// ��ݷ�ʽ���������༭��ɾ����
        /// </summary>
        /// <param name="ModuleId">ģ��Id</param>
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