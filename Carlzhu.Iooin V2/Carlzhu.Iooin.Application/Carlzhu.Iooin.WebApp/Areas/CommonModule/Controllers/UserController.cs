using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.CommonModule;


using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;
using OperationType = Carlzhu.Iooin.Business.CommonModule.OperationType;

namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{


    /// <summary>
    /// �û����������
    /// </summary>
    public class UserController : PublicController<BaseUser>
    {
        readonly BaseUserBll _baseuserbll = new BaseUserBll();
        BaseCompanyBll _basecompanybll = new BaseCompanyBll();
        readonly BaseObjectUserRelationBll _baseobjectuserrelationbll = new BaseObjectUserRelationBll();






        #region �û�����
        /// <summary>
        /// ��ѯǰ��50���û���Ϣ������JSON��
        /// </summary>
        /// <param name="keywords">��ѯ�ؼ���</param>
        /// <returns></returns>
        public ActionResult Autocomplete(string keywords)
        {
            DataTable listData = _baseuserbll.OptionUserList(keywords);
            return Content(Util.Json.ToJson(listData));
        }

        /// <summary>
        /// ���û����������û��б�JSON
        /// </summary>
        /// <param name="keywords">��ѯ�ؼ���</param>
        /// <param name="departmentId">����ID</param>
        /// <param name="jqgridparam">������</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string keywords, string departmentId, string empStatus, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = _baseuserbll.GetPageList(keywords, departmentId, empStatus, ref jqgridparam);
                var jsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = listData,
                };
                return Content(Util.Json.ToJson(jsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "�쳣����" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// ���û������ύ��
        /// </summary>
        /// <param name="KeyValue">����ֵ</param>
        /// <param name="Baseuser">�û���Ϣ</param>
        /// <param name="Baseemployee">Ա����Ϣ</param>
        /// <param name="BuildFormJson">�Զ����</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SubmitUserForm(string KeyValue, BaseUser Baseuser, BaseEmployee Baseemployee, string BuildFormJson)
        {
            string ModuleId = DESEncrypt.Decrypt(WebHelper.GetCookie("ModuleId"));
            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            try
            {
                var old = database.FindEntity<BaseEmployee>(Baseuser.Code);


                string message = KeyValue == "" ? "�����ɹ���" : "�༭�ɹ���";
                if (string.IsNullOrEmpty(Baseemployee.DefaultShift))
                {
                    Baseemployee.DefaultShift = "A01";
                }
                //if (Baseemployee.IsDimission == 1) { }
                //if (Baseemployee.IsDimission == 0) Baseemployee.DimissionDate = DateTime.Now;

                Baseemployee.IsShift = Request.Params["IsShift"] == "1";
                if (!string.IsNullOrEmpty(KeyValue))
                {
                    Baseuser.Modify(KeyValue);
                    Baseemployee.Modify(KeyValue);
                    Baseemployee.EmpNo = Baseuser.UserId;
                    database.Update(Baseuser, isOpenTrans);
                    database.Update(Baseemployee, isOpenTrans);
                }
                else
                {
                    Baseemployee.IsShift = Request.Params["IsShift"] == "1";
                    Baseuser.Create();
                    Baseuser.CompanyId = new RepositoryFactory<BaseDepartment>().Repository().FindEntity(Baseuser.DepartmentId).CompanyId;
                    Baseuser.SortCode = CommonHelper.GetInt(BaseFactory.BaseHelper().GetSortCode<BaseUser>("SortCode"));
                    Baseemployee.Create();
                    Baseemployee.EmpNo = Baseuser.UserId;
                    database.Insert(Baseuser, isOpenTrans);
                    database.Insert(Baseemployee, isOpenTrans);
                    BaseDataScopePermissionBll.Instance.AddScopeDefault(ModuleId, ManageProvider.Provider.Current().UserId, Baseuser.UserId, isOpenTrans);
                }
                BaseFormAttributeBll.Instance.SaveBuildForm(BuildFormJson, Baseuser.UserId, ModuleId, isOpenTrans);

                if (old.CardNo != Baseemployee.CardNo)
                {
                    //������Ƭ,���ڷ���������
                    for (int i = 0; i < 6; i++)
                    {
                        StringBuilder sql = new StringBuilder($"INSERT INTO EASTRIVER.DBO.WhiteCardTask ( " +
                                     "  card_id,                    emp_id,                 emp_fname,              flag,   clock_id,   cardtype, cardtypecode,     areacode,   opdate,     operator,   executedate,    realcardno )            VALUES " +
                                     $"('{Baseemployee.CardNo}', '{Baseemployee.EmpNo}', '{Baseemployee.RealName}', '1',    '{i}',          1,      '8669',         '0000',     getdate(),  'Admin',        getdate(),  '{Baseemployee.CardNo}')");

                        database.ExecuteBySql(sql, isOpenTrans);
                    }
                }

                database.Commit();
                return Content(new JsonMessage { Success = true, Code = "1", Message = message }.ToString());
            }
            catch (Exception ex)
            {
                database.Rollback();
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// ��ȡ�û�ְԱ��Ϣ���󷵻�JSON
        /// </summary>
        /// <param name="KeyValue">����ֵ</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SetUserForm(string KeyValue)
        {
            BaseUser Baseuser = DataFactory.Database().FindEntity<BaseUser>(KeyValue);
            if (Baseuser == null)
            {
                return Content("");
            }
            BaseEmployee Baseemployee = DataFactory.Database().FindEntity<BaseEmployee>(KeyValue);
            BaseCompany Basecompany = DataFactory.Database().FindEntity<BaseCompany>(Baseuser.CompanyId);
            string strJson = Util.Json.ToJson(Baseuser);
            //��˾
            strJson = strJson.Insert(1, "\"CompanyName\":\"" + Basecompany.FullName + "\",");
            //Ա����Ϣ
            strJson = strJson.Insert(1, Util.Json.ToJson(Baseemployee).Replace("{", "").Replace("}", "") + ",");
            //�Զ���
            strJson = strJson.Insert(1, BaseFormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(strJson);
        }
        #endregion

        #region �޸ĵ�¼����
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            ViewBag.Account = Request["Account"];
            return View();
        }
        /// <summary>
        /// �޸�����
        /// </summary>
        /// <param name="KeyValue">����</param>
        /// <param name="Password">������</param>
        /// <returns></returns>
        public ActionResult ResetPasswordSubmit(string KeyValue, string Password)
        {
            try
            {
                int isOk = 0;
                BaseUser baseuser = new BaseUser
                {
                    UserId = KeyValue,
                    ModifyDate = DateTime.Now,
                    ModifyUserId = ManageProvider.Provider.Current().UserId,
                    ModifyUserName = ManageProvider.Provider.Current().UserName,
                    Secretkey = Md5Helper.MD5(CommonHelper.CreateNo(), 16).ToLower()
                };
                baseuser.Password = Md5Helper.MD5(DESEncrypt.Encrypt(Password, baseuser.Secretkey).ToLower(), 32).ToLower();
                isOk = Repositoryfactory.Repository().Update(baseuser);
                BaseSysLogBll.Instance.WriteLog(KeyValue, OperationType.Update, isOk.ToString(), "�޸�����");
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = "�����޸ĳɹ���" }.ToString());
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog(KeyValue, OperationType.Update, "-1", "�����޸�ʧ�ܣ�" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "�����޸�ʧ�ܣ�" + ex.Message }.ToString());
            }
        }
        #endregion

        #region �û���ɫ
        /// <summary>
        /// �û���ɫ
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult UserRole()
        {
            return View();
        }
        /// <summary>
        /// �����û���ɫ
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <param name="UserId">�û�Id</param>
        /// <returns></returns>
        public ActionResult UserRoleList(string UserId)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = _baseuserbll.UserRoleList(UserId);
            foreach (DataRow dr in dt.Rows)
            {
                string strchecked = "";
                if (!string.IsNullOrEmpty(dr["objectid"].ToString()))//�ж��Ƿ�ѡ��
                {
                    strchecked = "selected";
                }
                sb.Append("<li title=\"" + dr["fullname"] + "(" + dr["code"] + ")" + "\" class=\"" + strchecked + "\">");
                sb.Append("<a id=\"" + dr["RoleId"] + "\"><img src=\"/Content/Images/Icon16/role.png \">" + dr["fullname"] + "</a><i></i>");
                sb.Append("</li>");
            }
            return Content(sb.ToString());
        }
        /// <summary>
        /// �û���ɫ - �ύ����
        /// </summary>
        /// <param name="UserId">�û�ID</param>
        /// <param name="ObjectId">��ɫid:1,2,3,4,5,6</param>
        /// <returns></returns>
        public ActionResult UserRoleSubmit(string UserId, string ObjectId)
        {
            try
            {
                string[] array = ObjectId.Split(',');
                int IsOk = _baseobjectuserrelationbll.BatchAddObject(UserId, array, "2");
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "�����ɹ���" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "����ʧ�ܣ�����" + ex.Message }.ToString());
            }
        }
        #endregion

        #region ��������
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonCenter()
        {
            ViewBag.imgGender = ManageProvider.Provider.Current().Gender == "��" ? "man.png" : "woman.png";
            ViewBag.strUserInfo = ManageProvider.Provider.Current().UserName + "��" + ManageProvider.Provider.Current().Account + "��";
            return View();
        }
        /// <summary>
        /// ��֤������
        /// </summary>
        /// <param name="OldPassword"></param>
        /// <returns></returns>
        public ActionResult ValidationOldPassword(string OldPassword)
        {
            if (ManageProvider.Provider.Current().Account == "System" || ManageProvider.Provider.Current().Account == "guest")
            {
                return Content(new JsonMessage { Success = true, Code = "0", Message = "��ǰ�˻������޸�����" }.ToString());
            }
            OldPassword = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(OldPassword, 32).ToLower(), ManageProvider.Provider.Current().Secretkey).ToLower(), 32).ToLower();
            if (OldPassword != ManageProvider.Provider.Current().Password)
            {
                return Content(new JsonMessage { Success = true, Code = "0", Message = "ԭ�����������������" }.ToString());
            }
            else
            {
                return Content(new JsonMessage { Success = true, Code = "1", Message = "ͨ����Ϣ��֤" }.ToString());
            }
        }
        #endregion
    }
}