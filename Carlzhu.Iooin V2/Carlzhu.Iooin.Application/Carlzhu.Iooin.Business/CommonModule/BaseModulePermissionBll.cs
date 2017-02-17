using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;




using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 模块权限表
    /// </summary>
    public class BaseModulePermissionBll : RepositoryFactory<BaseModulePermission>
    {
        private static BaseModulePermissionBll _item;
        public static BaseModulePermissionBll Instance => _item ?? (_item = new BaseModulePermissionBll());

        /// <summary>
        /// 模块权限列表
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
                strSql.Append(@"SELECT  m.ModuleId ,				--模块ID
                                        m.ParentId ,				--模块节点
                                        m.Code ,					--编码
                                        m.FullName ,				--名称
                                        m.Icon ,					--图标
                                        m.SortCode ,				--排序码
                                        cp.ModuleId AS ObjectId     --是否存在
                                FROM    BaseModule M INNER JOIN ( SELECT DISTINCT ModuleId  FROM   BaseModulePermission");
                strSql.Append(" WHERE  ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')) MP ON M.ModuleId = mp.ModuleId");
                strSql.Append(" LEFT JOIN ( SELECT DISTINCT ModuleId  FROM  BaseModulePermission");
                strSql.Append(" WHERE  ObjectId = @ObjectId ) CP ON cp.ModuleId = M.ModuleId");
            }
            else
            {
                strSql.Append(@"SELECT  m.ModuleId ,				--模块ID
                                        m.ParentId ,				--模块节点
                                        m.Code ,					--编码
                                        m.FullName ,				--名称
                                        m.Icon ,					--图标
                                        m.SortCode ,				--排序吗
                                        mp.ObjectId					--是否存在
                                FROM    BaseModule m
                                        LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId
                                                                          AND mp.ObjectId = @ObjectId");
            }
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            strSql.Append(" ORDER BY  m.SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 加载权限模块
        /// </summary>
        /// <param name="objectId">对象主键</param>
        /// <returns></returns>
        public List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> GetModuleList(string objectId)
        {
            StringBuilder strSql = new StringBuilder();
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(@"SELECT DISTINCT  M.* FROM BaseModule M");
                strSql.Append(" INNER JOIN BaseModulePermission MP ON M.ModuleId = MP.ModuleId WHERE   ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            }
            else
            {
                strSql.Append(@"SELECT * FROM BaseModule M");
            }
            strSql.Append(" ORDER BY  M.SortCode ASC ");
            return DataFactory.Database().FindListBySql<Carlzhu.Iooin.Entity.CommonModule.BaseModule>(strSql.ToString());
        }
        /// <summary>
        /// 根据对象Id获取模块权限列表
        /// </summary>
        /// <param name="objectId">对象ID</param>
        /// <returns></returns>
        public DataTable GetModulePermission(string objectId)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  m.ModuleId ,
                                    m.ParentId ,
                                    m.FullName ,
                                    m.Icon
                            FROM    BaseModule m
                                    LEFT JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId");
            strSql.Append(" WHERE   mp.ObjectId IN ('" + objectId.Replace(",", "','") + "')");
            strSql.Append(" ORDER BY  m.SortCode ASC ");
            return Repository().FindTableBySql(strSql.ToString());
        }

        /// <summary>
        /// Action执行权限认证
        /// </summary>
        /// <param name="Action">视图Action</param>
        /// <param name="ObjectId">对象主键</param>
        /// <param name="ModuleId">模块Id</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        public bool ActionAuthorize(string Action, string ObjectId, string ModuleId, string UserId)
        {
            List<Carlzhu.Iooin.Entity.CommonModule.BaseModule> listData;
            StringBuilder strSql = new StringBuilder();
            strSql.Append(@"SELECT  b.ModuleId,b.FullName ,b.ActionEvent AS Location FROM    BaseButton b INNER JOIN BaseButtonPermission bp ON bp.ModuleButtonId = b.ButtonId AND bp.ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            strSql.Append(" UNION ");
            strSql.Append(@"SELECT  m.ModuleId,m.FullName , m.Location FROM    BaseModule m INNER JOIN BaseModulePermission mp ON mp.ModuleId = m.ModuleId  AND mp.ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "')");
            listData = DataFactory.Database().FindListBySql<Carlzhu.Iooin.Entity.CommonModule.BaseModule>(strSql.ToString());
            listData = (from entity in listData
                        where (entity.Location.ToLower() == Action && entity.ModuleId == ModuleId)
                        select entity).ToList();
            int count = listData.Count;
            return count > 0;
        }
    }
}