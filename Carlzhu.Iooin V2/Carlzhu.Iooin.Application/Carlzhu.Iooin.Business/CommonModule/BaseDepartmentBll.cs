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
    /// 部门管理
    /// </summary>
    public class BaseDepartmentBll : RepositoryFactory<BaseDepartment>
    {
        /// <summary>
        /// 获取 公司、部门 列表
        /// </summary>
        /// <returns></returns>
        public DataTable GetTree()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    CompanyId,				--公司ID
												CompanyId AS DepartmentId ,--部门ID
                                                Code ,					--编码
                                                FullName ,				--名称
                                                ParentId ,				--节点ID
                                                SortCode,				--排序编码
                                                'Company' AS Sort		--分类
                                      FROM      BaseCompany			--公司表
                                      UNION
                                      SELECT    CompanyId,				--公司ID
												DepartmentId,			--部门ID
                                                Code ,					--编码
                                                FullName ,				--名称
                                                CompanyId AS ParentId ,	--节点ID
                                                SortCode,				--排序编码
                                                'Department' AS Sort	--分类
                                      FROM      BaseDepartment			--部门表ParentId=0
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
        /// 根据公司id获取部门 列表
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <returns></returns>
        public DataTable GetList(string CompanyId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    d.DepartmentId ,			--主键
                                                c.FullName AS CompanyName ,	--所属公司
                                                d.CompanyId ,				--所属公司Id
                                                d.Code ,					--编码
                                                d.FullName ,				--部门名称
                                                d.ShortName ,				--部门简称
                                                d.Nature ,					--部门性质
                                                d.Manager ,					--负责人
                                                d.Phone ,					--电话
                                                d.Fax ,						--传真
                                                d.Enabled ,					--有效
                                                d.SortCode,                 --排序吗
                                                d.Remark					--说明
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