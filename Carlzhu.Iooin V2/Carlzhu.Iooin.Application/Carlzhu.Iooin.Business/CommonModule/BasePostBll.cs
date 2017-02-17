using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ��λ����
    /// </summary>
    public class BasePostBll : RepositoryFactory<BasePost>
    {
        /// <summary>
        /// ��ȡ��λ�б�
        /// </summary>
        /// <param name="CompanyId">����ID</param>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public DataTable GetPageList(string CompanyId, ref Pagination jqgridparam)
        {
            //ѭ����ѯ��������
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    post.PostId ,                   --��λID
                                                post.Code ,                     --��λ����
                                                post.FullName ,                 --��λ����
                                                post.CompanyId ,             --���ڲ���Id
                                                com.FullName AS CompanyName ,--���ڲ���
                                                post.Enabled ,                  --�Ƿ���Ч
                                                post.Remark ,                   --��λ����
                                                post.SortCode                   --������
                                      FROM      BasePost post
                                                LEFT JOIN BaseCompany com ON com.CompanyId = post.CompanyId
                                    ) T
                            WHERE   1 = 1 ");
            if (!string.IsNullOrEmpty(CompanyId))
            {
                CompanyId = base.Repository().FindChildAllKeys("BaseCompany", "CompanyId", CompanyId);
                strSql.Append($" AND CompanyId in ({CompanyId})");
                //strSql.Append(" AND DepartmentId in (@DepartmentId)");
                //parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", departmentId));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( PostId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}