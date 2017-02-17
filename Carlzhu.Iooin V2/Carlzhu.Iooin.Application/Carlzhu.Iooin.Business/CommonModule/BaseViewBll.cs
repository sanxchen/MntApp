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
    /// ��ͼ���ñ�
    /// </summary>
    public class BaseViewBll : RepositoryFactory<BaseView>
    {
        /// <summary>
        /// ����ģ��Id��ȡ��ͼ�б�
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
        /// ��ͼ�ύ���������༭��ɾ����
        /// </summary>
        /// <param name="KeyValue">�ж��������޸�</param>
        /// <param name="ModuleId">ģ��Id</param>
        /// <param name="ViewJson">��ͼJson</param>
        /// <param name="ViewWhereJson">��ͼ����Json</param>
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