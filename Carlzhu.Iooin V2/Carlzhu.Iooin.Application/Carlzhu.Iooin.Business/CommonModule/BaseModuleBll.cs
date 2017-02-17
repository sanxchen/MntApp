using System.Collections.Generic;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ģ������
    /// </summary>
    public class BaseModuleBll : RepositoryFactory<Entity.CommonModule.BaseModule>
    {
        public List<Entity.CommonModule.BaseModule> GetList()
        {
            return this.Repository().FindList("ORDER BY ParentId ASC,SortCode ASC");
        }
    }
}