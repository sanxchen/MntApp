using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// ���Ź���
    /// </summary>
    public class BaseDepartmentBll : RepositoryFactory<BaseDepartment>
    {
        /// <summary>
        /// ��ȡ ��˾������ �б�
        /// </summary>
        /// <returns></returns>
        public DataTable GetTree()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    CompanyId,				--��˾ID
												CompanyId AS DepartmentId ,--����ID
                                                Code ,					--����
                                                FullName ,				--����
                                                ParentId ,				--�ڵ�ID
                                                SortCode,				--�������
                                                'Company' AS Sort		--����
                                      FROM      BaseCompany			--��˾��
                                      UNION
                                      SELECT    CompanyId,				--��˾ID
												DepartmentId,			--����ID
                                                Code ,					--����
                                                FullName ,				--����
                                                CompanyId AS ParentId ,	--�ڵ�ID
                                                SortCode,				--�������
                                                'Department' AS Sort	--����
                                      FROM      BaseDepartment			--���ű�ParentId=0
                                    ) T WHERE 1=1 ");
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //strSql.Append(" AND ( DepartmentId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
            }
            strSql.Append(" ORDER BY SortCode ASC");
            return Repository().FindTableBySql(strSql.ToString());
        }
        /// <summary>
        /// ���ݹ�˾id��ȡ���� �б�
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <returns></returns>
        public DataTable GetList(string CompanyId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    d.DepartmentId ,			--����
                                                c.FullName AS CompanyName ,	--������˾
                                                d.CompanyId ,				--������˾Id
                                                d.Code ,					--����
                                                d.FullName ,				--��������
                                                d.ShortName ,				--���ż��
                                                d.Nature ,					--��������
                                                d.Manager ,					--������
                                                d.Phone ,					--�绰
                                                d.Fax ,						--����
                                                d.Enabled ,					--��Ч
                                                d.SortCode,                 --������
                                                d.Remark					--˵��
                                      FROM      BaseDepartment d
                                                LEFT JOIN BaseCompany c ON c.CompanyId = d.CompanyId
                                    ) T WHERE 1=1 ");
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(CompanyId))
            {
                strSql.Append(" AND CompanyId = @CompanyId");
                parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( DepartmentId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            strSql.Append(" ORDER BY CompanyId ASC,SortCode ASC");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }

        public List<BaseDepartment> GetList()
        {
            StringBuilder WhereSql = new StringBuilder();
            WhereSql.Append("AND CompanyId!='MDCompany'  ORDER BY SortCode ASC");
            return Repository().FindList(WhereSql.ToString());

        }

        public DataTable InitCompanyDepartment(string departmentId)
        {
            return DataFactory.Database().FindTableBySql($"select dpt.DepartmentId as dptid,dpt.FullName as dptname,com.CompanyId as comid,com.FullName as comname,dpt.parentid as dptpid,(select fullname from basedepartment where departmentid=dpt.parentid) as dptpname from basedepartment dpt LEFT OUTER JOIN baseCompany com on dpt.companyid=com.companyid where dpt.departmentid='{departmentId}'");
        }


    }
}