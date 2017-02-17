using System.Collections.Generic;
using System.Data;
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
    /// �����û���ϵ��
    /// </summary>
    public class BaseObjectUserRelationBll : RepositoryFactory<BaseObjectUserRelation>
    {
        /// <summary>
        /// ��Ա�б�
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <param name="DepartmentId">����ID</param>
        /// <param name="ObjectId">��������</param>
        /// <param name="Category">�������:1-����2-��ɫ3-��λ4-Ⱥ��</param>
        /// <returns></returns>
        public DataTable GetList(string CompanyId, string DepartmentId, string ObjectId, string Category)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,				--�û�ID
                                                u.Account ,				--�˻�
                                                u.RealName ,			--����
                                                u.Code ,				--����
                                                u.Gender ,				--�Ա�
                                                u.CompanyId ,			--��˾ID
                                                u.DepartmentId ,		--����ID
                                                u.SortCode ,			--������
                                                ou.ObjectId				--�Ƿ����
                                      FROM      BaseUser u
                                                LEFT JOIN BaseObjectUserRelation ou ON ou.UserId = u.UserId
                                                                                        AND ou.ObjectId = @ObjectId
                                                                                        AND ou.Category = @Category
                                    ) T WHERE 1=1");

            //ɾ����ְ��Ա
            strSql.Append("AND USERID IN (SELECT  EMPNO FROM BaseEmployee WHERE IsDimission=1)");

            if (!string.IsNullOrEmpty(CompanyId))
            {
                CompanyId = Repository().FindChildAllKeys("BaseCompany", "CompanyId", CompanyId);
                strSql.Append($" AND CompanyId in ({CompanyId})");
                //strSql.Append(" AND CompanyId = @CompanyId");
                //parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            }
            if (!string.IsNullOrEmpty(DepartmentId))
            {
                CompanyId = Repository().FindChildAllKeys("BaseDepartment", "DepartmentId", DepartmentId);
                strSql.Append($" AND DepartmentId in ({CompanyId})");

                //strSql.Append(" AND DepartmentId = @DepartmentId");
                //parameter.Add(DbFactory.CreateDbParameter("@DepartmentId", DepartmentId));
            }
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            parameter.Add(DbFactory.CreateDbParameter("@Category", Category));
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( UserId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }
            strSql.Append(" ORDER BY ObjectId DESC,SortCode ASC");
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ������ӳ�Ա
        /// </summary>
        /// <param name="arrayUserId">ѡ���û�ID: 1,2,3,4,5,6</param>
        /// <param name="ObjectId">��������</param>
        /// <param name="Category">�������:1-����2-��ɫ3-��λ4-Ⱥ��</param>
        /// <returns></returns>
        public int BatchAddMember(string[] arrayUserId, string ObjectId, string Category)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                StringBuilder sbDelete = new StringBuilder("DELETE FROM BaseObjectUserRelation WHERE ObjectId = @ObjectId AND Category=@Category");
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
                parameter.Add(DbFactory.CreateDbParameter("@Category", Category));
                database.ExecuteBySql(sbDelete, parameter.ToArray(), isOpenTrans);
                int index = 1;
                foreach (string item in arrayUserId)
                {
                    if (item.Length > 0)
                    {
                        BaseObjectUserRelation entity = new BaseObjectUserRelation();
                        entity.ObjectUserRelationId = CommonHelper.GetGuid;
                        entity.ObjectId = ObjectId;
                        entity.UserId = item;
                        entity.Category = Category;
                        entity.SortCode = index;
                        entity.Create();
                        index++;
                        database.Insert(entity, isOpenTrans);
                    }
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
        /// <summary>
        /// ������Ӷ����û���ϵ
        /// </summary>
        /// <param name="UserId">�û�ID</param>
        /// <param name="arrayObjectId">����ID</param>
        /// <param name="Category">�������:1-����2-��ɫ3-��λ4-Ⱥ��</param>
        /// <returns></returns>
        public int BatchAddObject(string UserId, string[] arrayObjectId, string Category)
        {
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                StringBuilder sbDelete = new StringBuilder("DELETE FROM BaseObjectUserRelation WHERE UserId = @UserId AND Category=@Category");
                List<DbParameter> parameter = new List<DbParameter>();
                parameter.Add(DbFactory.CreateDbParameter("@UserId", UserId));
                parameter.Add(DbFactory.CreateDbParameter("@Category", Category));
                database.ExecuteBySql(sbDelete, parameter.ToArray(), isOpenTrans);
                int index = 1;
                foreach (string item in arrayObjectId)
                {
                    if (item.Length > 0)
                    {
                        BaseObjectUserRelation entity = new BaseObjectUserRelation();
                        entity.ObjectUserRelationId = CommonHelper.GetGuid;
                        entity.UserId = UserId;
                        entity.ObjectId = item;
                        entity.Category = Category;
                        entity.SortCode = index;
                        entity.Create();
                        index++;
                        database.Insert(entity, isOpenTrans);
                    }
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
        /// <summary>
        /// �����û�ID��ѯ�����б�
        /// </summary>
        /// <param name="UserId">�û�ID</param>
        /// <returns></returns>
        public List<BaseObjectUserRelation> GetList(string UserId)
        {
            return Repository().FindList("UserId", UserId);
        }
        /// <summary>
        /// �����û�ID��ѯ�����б�
        /// </summary>
        /// <param name="UserId">�û�ID</param>
        /// <returns></returns>
        public string GetObjectId(string UserId)
        {
            StringBuilder sb_ObjectId = new StringBuilder();
            List<BaseObjectUserRelation> list = GetList(UserId);
            if (list.Count > 0)
            {
                foreach (BaseObjectUserRelation item in list)
                {
                    sb_ObjectId.Append(item.ObjectId + ",");
                }
                sb_ObjectId.Append(UserId);
            }
            return sb_ObjectId.ToString();
        }
        /// <summary>
        /// ���ݶ���Id ��ȡ�û��б�
        /// </summary>
        /// <param name="ObjectId"></param>
        /// <returns></returns>
        public List<BaseUser> GetUserList(string ObjectId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  u.UserId ,				--�û�ID
                                    u.Account ,				--�˻�
                                    u.RealName ,			--����
                                    u.Code ,				--����
                                    u.Gender ,				--�Ա�
                                    u.CompanyId ,			--��˾ID
                                    u.DepartmentId ,		--����ID
                                    u.SortCode 			    --������
                            FROM    BaseUser u
                                    INNER JOIN BaseObjectUserRelation ou ON ou.UserId = u.UserId
                                                                            AND ou.ObjectId = @ObjectId");
            parameter.Add(DbFactory.CreateDbParameter("@ObjectId", ObjectId));
            return DataFactory.Database().FindListBySql<BaseUser>(strSql.ToString(), parameter.ToArray());
        }
    }
}