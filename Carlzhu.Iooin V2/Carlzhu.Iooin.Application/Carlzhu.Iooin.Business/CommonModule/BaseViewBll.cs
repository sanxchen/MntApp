using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// 视图设置表
    /// </summary>
    public class BaseViewBll : RepositoryFactory<BaseView>
    {
        /// <summary>
        /// 根据模块Id获取视图列表
        /// </summary>
        /// <param name="ModuleId"></param>
        /// <returns></returns>
        public List<BaseView> GetViewList(string ModuleId)
        {
            StringBuilder WhereSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            WhereSql.Append(" AND ModuleId = @ModuleId order by sortcode asc");
            parameter.Add(DbFactory.CreateDbParameter("@ModuleId", ModuleId));
            return Repository().FindList(WhereSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// 视图提交（新增、编辑、删除）
        /// </summary>
        /// <param name="KeyValue">判断新增、修改</param>
        /// <param name="ModuleId">模块Id</param>
        /// <param name="ViewJson">视图Json</param>
        /// <param name="ViewWhereJson">视图条件Json</param>
        /// <returns></returns>
        public int SubmitForm(string KeyValue, string ModuleId, string ViewJson, string ViewWhereJson)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                List<BaseView> ViewList = ViewJson.JonsToList<BaseView>();
                List<BaseViewWhere> ViewWhereList = ViewWhereJson.JonsToList<BaseViewWhere>();
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    database.Delete<BaseView>("ModuleId", ModuleId, isOpenTrans);
                    database.Delete<BaseViewWhere>("ModuleId", ModuleId, isOpenTrans);
                }
                foreach (BaseView Baseview in ViewList)
                {
                    if (string.IsNullOrEmpty(Baseview.ViewId))
                        Baseview.ViewId = CommonHelper.GetGuid;
                    Baseview.ModuleId = ModuleId;
                    Baseview.ParentId = "0";
                    database.Insert(Baseview, isOpenTrans);
                }
                foreach (BaseViewWhere Baseviewwhere in ViewWhereList)
                {
                    if (string.IsNullOrEmpty(Baseviewwhere.ViewWhereId))
                        Baseviewwhere.ViewWhereId = CommonHelper.GetGuid;
                    Baseviewwhere.ModuleId = ModuleId;
                    database.Insert(Baseviewwhere, isOpenTrans);
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