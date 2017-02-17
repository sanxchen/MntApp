using System.Collections.Generic;
using System.Data.Common;
using System.Text;


using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// �����ֵ��
    /// </summary>
    public class BaseDataDictionaryBll : RepositoryFactory<BaseDataDictionary>
    {
        /// <summary>
        /// ��ȡ�����ֵ���ϸ�б�
        /// </summary>
        /// <param name="dataDictionaryId">���� ����ֵ</param>
        /// <returns></returns>
        public List<BaseDataDictionaryDetail> GetDataDictionaryDetailList(string dataDictionaryId)
        {
            if (!string.IsNullOrEmpty(dataDictionaryId))
            {
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" AND DataDictionaryId = @DataDictionaryId Order By SortCode ASC");
                List<DbParameter> parameter = new List<DbParameter>
                {
                    DbFactory.CreateDbParameter("@DataDictionaryId", dataDictionaryId)
                };
                return DataFactory.Database().FindList<BaseDataDictionaryDetail>(whereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// ��ȡ�����ֵ���ϸ�б�
        /// </summary>
        /// <param name="code">�������</param>
        /// <returns></returns>
        public List<BaseDataDictionaryDetail> GetDataDictionaryDetailListByCode(string code)
        {
            if (!string.IsNullOrEmpty(code))
            {
                StringBuilder whereSql = new StringBuilder();
                whereSql.Append(" AND DataDictionaryId IN(SELECT DataDictionaryId FROM BaseDataDictionary WHERE Code=@Code)");
                whereSql.Append(" ORDER BY SortCode ASC");
                List<DbParameter> parameter = new List<DbParameter> {DbFactory.CreateDbParameter("@Code", code)};
                return DataFactory.Database().FindList<BaseDataDictionaryDetail>(whereSql.ToString(), parameter.ToArray());
            }
            else
            {
                return null;
            }
        }
    }
}