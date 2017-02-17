using System.Collections.Generic;
using System.Data.Common;
using System.Text;



using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ʡ����
    /// </summary>
    public class BaseProvinceCityBll : RepositoryFactory<BaseProvinceCity>
    {
        /// <summary>
        /// ��ȡʡ���С��� �б�
        /// </summary>
        /// <param name="ParentId"></param>
        /// <returns></returns>
        public List<BaseProvinceCity> GetList(string ParentId)
        {
            StringBuilder WhereSql = new StringBuilder();
            WhereSql.Append(" AND ParentId = @ParentId Order By SortCode ASC");
            List<DbParameter> parameter = new List<DbParameter>();
            parameter.Add(DbFactory.CreateDbParameter("@ParentId", ParentId));
            return this.Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
    }
}