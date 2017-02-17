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
    /// 操作按钮权限表
    /// </summary>
    public class BaseButtonPermissionBll : RepositoryFactory<BaseButtonPermission>
    {
        /// <summary>
        /// 按钮权限列表
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="Category">对象分类:1-部门2-角色3-岗位4-群组</param>
        /// <returns></returns>
        public DataTable GetList(string ObjectId, string Category)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT  b.ButtonId ,				--按钮ID
                                        b.ModuleId ,				--模块ID
                                        b.Code ,					--编码
                                        b.FullName ,				--名称
                                        b.Category ,				--分类
                                        b.Icon ,					--图标
                                        b.SortCode ,				--排序吗
                                        cp.ModuleButtonId AS ObjectId					--是否存在
                                FROM    BaseButton b INNER JOIN ( SELECT DISTINCT ModuleButtonId  FROM   BaseButtonPermission");
                strSql.Append(" WHERE  ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')) bp ON B.ButtonId = bp.ModuleButtonId");
                strSql.Append(" LEFT JOIN ( SELECT DISTINCT ModuleButtonId  FROM  BaseButtonPermission");
                strSql.Append(" WHERE  ObjectId = @ObjectId ) cp ON cp.ModuleButtonId = b.ButtonId");
            }
            else
            {
                strSql.Append(@"SELECT  b.ButtonId ,				--按钮ID
                                    b.ModuleId ,				--模块ID
                                    b.Code ,					--编码
                                    b.FullName ,				--名称
                                    b.Category ,				--分类
                                    b.Icon ,					--图标
                                    b.SortCode ,				--排序吗
                                    bp.ObjectId					--是否存在
                            FROM    BaseButton b
                                    LEFT JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId
                                                                          AND bp.ObjectId = @ObjectId");
            }
            strSql.Append(" order by b.SortCode ASC");
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 加载按钮权限
        /// </summary>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="ModuleId"模块主键</param>
        /// <returns></returns>
        public List<BaseButton> GetButtonList(string ObjectId, string ModuleId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT DISTINCT B.* FROM BaseButton B");
                strSql.Append(" INNER JOIN BaseButtonPermission BP ON B.ButtonId = BP.ModuleButtonId WHERE ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            }
            else
            {
                strSql.Append(@"SELECT * FROM BaseButton B WHERE 1=1 ");
            }
            strSql.Append(" AND B.ModuleId = @ModuleId");
            strSql.Append(" ORDER BY B.SortCode ASC ");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return DataFactory.Database().FindListBySql<BaseButton>(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 根据对象Id获取模块按钮权限列表
        /// </summary>
        /// <param name="ObjectId">对象ID</param>
        /// <returns></returns>
        public DataTable GetButtonePermission(string ObjectId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    m.ModuleId AS ID ,
                                                m.ParentId ,
                                                m.FullName ,
                                                m.Icon ,
                                                m.SortCode,
                                                m.Category,
                                                '模块' AS Sort
                                      FROM      BaseModule m
                                                LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId");
            strSql.Append(@" WHERE     mp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "')");
            strSql.Append(@" UNION     SELECT    b.ButtonId AS ID ,
                                                b.ModuleId AS ParentId ,
                                                b.FullName ,
                                                b.Icon ,
                                                b.SortCode,
                                                b.Category,
                                                '按钮' AS Sort
                                      FROM      BaseButton b
                                                LEFT JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId");
            strSql.Append(" WHERE     bp.ObjectId IN ('" + ObjectId.Replace(",", "','") + "')) A");
            strSql.Append(" ORDER BY SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString());
        }
    }
}