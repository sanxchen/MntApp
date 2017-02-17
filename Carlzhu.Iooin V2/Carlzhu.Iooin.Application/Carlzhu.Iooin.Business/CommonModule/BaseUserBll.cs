using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.CommonModule
{
    /// <summary>
    /// �û�����
    /// </summary>
    public class BaseUserBll : RepositoryFactory<BaseUser>
    {
        /// <summary>
        /// ��ȡ�û��б�
        /// </summary>
        /// <param name="keyword">ģ���ѯ</param>
        /// <param name="departmentId">����ID</param>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public DataTable GetPageList(string keyword, string departmentId, string empStatus, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  *
                            FROM    ( SELECT    u.UserId ,					--�û�ID
                                                u.Code ,					--�û�����
                                                u.Account ,					--��¼�˻�
                                                e.RealName ,				--����
                                                e.CardNo,                   --�ſ���
                                                e.ManagerId,                --���ܹ���
                                                e.Manager,                  --��������
                                                u.Spell,                    --ƴ��
                                                e.Gender ,					--�Ա�
                                                e.Mobile ,					--�ֻ�
                                                e.OfficePhone ,		        --����
                                                e.Duty ,				    --ְ��
                                                e.Email ,					--��������
                                                c.CompanyId ,			    --���ڹ�˾ID
                                                c.FullName AS CompanyName ,	--���ڹ�˾
                                                e.DepartmentId,				--���ڲ���ID
                                                d.FullName AS DepartmentName,--���ڲ���
                                                e.Position,                  --ְ��
                                                u.Enabled ,					--�Ƿ���Ч
                                                u.LogOnCount ,				--��¼����
                                                u.LastVisit ,				--����¼ʱ��
                                                u.SortCode,                 --������
                                                u.CreateUserId,				--����ʱ��
                                                e.IsDimission,				--Ա����ְ״̬
                                                u.Remark					--��ע
                                      FROM      BaseUser u
                                                LEFT JOIN BaseCompany c ON c.CompanyId = u.CompanyId
                                                LEFT JOIN BaseDepartment d ON d.DepartmentId = u.DepartmentId
                                                LEFT JOIN BaseEmployee e ON e.EmpNo = u.UserId
                                    ) T WHERE 1=1 ");
            if (!string.IsNullOrEmpty(keyword))
            {
                strSql.Append(@" AND (RealName LIKE @keyword
                                    OR Account LIKE @keyword
                                    OR Code LIKE @keyword
                                    OR Spell LIKE @keyword)");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keyword + '%'));
            }
            if (!string.IsNullOrEmpty(empStatus) && empStatus!="-1")
            {
                strSql.Append($"AND IsDimission={empStatus}");
            }
            if (!string.IsNullOrEmpty(departmentId))
            {
                departmentId = base.Repository().FindChildAllKeys("BaseDepartment", "DepartmentId", departmentId);
                strSql.Append($" AND DepartmentId in ({departmentId})");
            }
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{
            //    strSql.Append(" AND ( UserId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
            //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
            //    strSql.Append(" ) )");
            //}
            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }
        /// <summary>
        /// �ж��Ƿ����ӷ�����
        /// </summary>
        /// <returns></returns>
        public bool IsLinkServer()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT  GETDATE()");
            DataTable dt = Repository().FindTableBySql(strSql.ToString());
            return dt != null && dt.Rows.Count > 0;
        }
        /// <summary>
        /// ��½��֤��Ϣ
        /// </summary>
        /// <param name="Account">�˻�</param>
        /// <param name="Password">����</param>
        /// <param name="result">���ؽ��</param>
        /// <returns></returns>
        public BaseUser UserLogin(string Account, string Password, string pwd, out string result)
        {




            if (!this.IsLinkServer())
            {
                throw new Exception("���������Ӳ��ϣ�" + "Connect Err");
            }

            var patten = new Regex("\\d{7}");
            var entity = patten.IsMatch(Account) ?
                Repository().FindEntity(Account) :
                Repository().FindEntity("Account", Account);


            if (entity?.UserId != null)
            {
                if (entity.Enabled == 1)
                {
                    string dbPassword = Md5Helper.MD5(DESEncrypt.Encrypt(Password.ToLower(), entity.Secretkey).ToLower(), 32).ToLower();

                    //�����������֤
                    if (dbPassword == entity.Password || BaseHelper.LoginAdServer(entity.Account, pwd))
                    {
                        DateTime previousVisit = CommonHelper.GetDateTime(entity.LastVisit);
                        DateTime lastVisit = DateTime.Now;
                        int logOnCount = CommonHelper.GetInt(entity.LogOnCount) + 1;
                        entity.PreviousVisit = previousVisit;
                        entity.LastVisit = lastVisit;
                        entity.LogOnCount = logOnCount;
                        entity.Online = 1;
                        Repository().Update(entity);
                        result = "succeed";
                    }
                    else
                    {
                        result = "error";
                    }
                }
                else
                {
                    result = "lock";
                }
                return entity;
            }
            result = "-1";
            return null;
        }





        /// <summary>
        /// ��ȡ�û���ɫ�б�
        /// </summary>

        /// <param name="UserId">�û�Id</param>
        /// <returns></returns>
        public DataTable UserRoleList(string UserId)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append(@"SELECT  r.RoleId ,				--��ɫID
                                    r.Code ,				--����
                                    r.FullName ,			--����
                                    r.SortCode ,			--������
                                    ou.ObjectId				--�Ƿ����
                            FROM    BaseRoles r
                                    LEFT JOIN BaseObjectUserRelation ou ON ou.ObjectId = r.RoleId
                                                                            AND ou.UserId = @UserId
                                                                            AND ou.Category = 2
                            WHERE 1 = 1");
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{
            //    strSql.Append(" AND ( RoleId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
            //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
            //    strSql.Append(" ) )");
            //}
            //strSql.Append(" AND r.CompanyId = @CompanyId");
            parameter.Add(DbFactory.CreateDbParameter("@UserId", UserId));
            //parameter.Add(DbFactory.CreateDbParameter("@CompanyId", CompanyId));
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
        /// <summary>
        /// ѡ���û��б�
        /// </summary>
        /// <param name="keyword">ģ���ѯ</param>
        /// <returns></returns>
        public DataTable OptionUserList(string keyword)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            if (!string.IsNullOrEmpty(keyword))
            {
                strSql.Append(@"SELECT TOP 50 * FROM ( SELECT    
                                        u.UserId ,
                                        u.Account ,
                                        u.code,
                                        u.RealName ,
                                        u.DepartmentId ,
                                        d.FullName AS DepartmentName,
                                        u.Gender
                                FROM    BaseUser u
                                LEFT JOIN BaseDepartment d ON d.DepartmentId = u.DepartmentId
                                WHERE   u.RealName LIKE @keyword
                                        OR u.Account LIKE @keyword
                                        OR u.Code LIKE @keyword
                                        OR u.Spell LIKE @keyword
                                        OR u.UserId IN (
                                        SELECT  u.UserId
                                        FROM    BaseUser u
                                                INNER JOIN BaseObjectUserRelation oc ON u.UserId = oc.UserId
                                                INNER JOIN dbo.BaseCompany c ON c.CompanyId = oc.ObjectId
                                        WHERE   c.FullName LIKE @keyword
                                        UNION
                                        SELECT  u.UserId
                                        FROM    BaseUser u
                                                INNER JOIN BaseObjectUserRelation od ON u.UserId = od.UserId
                                                INNER JOIN BaseDepartment d ON d.DepartmentId = od.ObjectId
                                        WHERE   d.FullName LIKE @keyword
                                        UNION
                                        SELECT  u.UserId
                                        FROM    BaseUser u
                                                INNER JOIN BaseObjectUserRelation oro ON u.UserId = oro.UserId
                                                INNER JOIN BaseRoles r ON r.RoleId = oro.ObjectId
                                        WHERE   r.FullName LIKE @keyword
                                        UNION
                                        SELECT  u.UserId
                                        FROM    BaseUser u
                                                INNER JOIN BaseObjectUserRelation op ON u.UserId = op.UserId
                                                INNER JOIN BasePost p ON p.PostId = op.ObjectId
                                        WHERE   p.FullName LIKE @keyword
                                        UNION
                                        SELECT  u.UserId
                                        FROM    BaseUser u
                                                INNER JOIN BaseObjectUserRelation og ON u.UserId = og.UserId
                                                INNER JOIN BaseGroupUser g ON g.GroupUserId = og.ObjectId
                                        WHERE   g.FullName LIKE @keyword )
                            ) a WHERE 1 = 1");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", '%' + keyword + '%'));
            }
            else
            {
                strSql.Append(@"SELECT TOP 50
                                        u.UserId ,
                                        u.Account ,
                                        u.code ,
                                        u.RealName ,
                                        u.DepartmentId ,
                                        d.FullName AS DepartmentName ,
                                        u.Gender
                                FROM    BaseUser u
                                        LEFT JOIN BaseDepartment d ON d.DepartmentId = u.DepartmentId
                                WHERE   1 = 1");
            }
            //if (!ManageProvider.Provider.Current().IsSystem)
            //{
            //    strSql.Append(" AND ( UserId IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
            //    strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
            //    strSql.Append(" ) )");
            //}
            return Repository().FindTableBySql(strSql.ToString(), parameter.ToArray());
        }
    }
}