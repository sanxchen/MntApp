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
    /// �û������
    /// </summary>
    public class BaseGroupUserBll : RepositoryFactory<BaseGroupUser>
    {
        /// <summary>
        /// ��ȡ�û����б�
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <param name="departmentId">����ID</param>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public DataTable GetPageList(string departmentId, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    gu.GroupUserId ,              --�û���ID
                                                gu.Code ,                     --�û������
                                                gu.FullName ,                 --�û�������
                                                gu.DepartmentId ,             --���ڲ���Id
                                                dep.FullName AS DepartmentName ,--���ڲ���
                                                gu.Enabled ,                  --�Ƿ���Ч
                                                gu.Remark ,                   --��λ����
                                                gu.SortCode                   --������
                                      FROM      BaseGroupUser gu
                                                LEFT JOIN BaseDepartment dep ON dep.DepartmentId = gu.DepartmentId
                                    ) T WHERE   1 = 1 ");
    
            if (!string.IsNullOrEmpty(departmentId))
            {
                departmentId = base.Repository().FindChildAllKeys("BaseDepartment", "DepartmentId", departmentId);
                strSql.Append($" AND DepartmentId in ({departmentId})");

                //strSql.Append(" AND DepartmentId = @DepartmentId");
                //parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", DepartmentId));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( GroupUserId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
    }
}