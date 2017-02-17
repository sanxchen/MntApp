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
    /// 用户管理控制器
    /// </summary>
    public class UserController : PublicController<BaseUser>
    {
        readonly BaseUserBll _baseuserbll = new BaseUserBll();
        BaseCompanyBll _basecompanybll = new BaseCompanyBll();
        readonly BaseObjectUserRelationBll _baseobjectuserrelationbll = new BaseObjectUserRelationBll();






        #region 用户管理
        /// <summary>
        /// 查询前面50条用户信息（返回JSON）
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <returns></returns>
        public ActionResult Autocomplete(string keywords)
        {
            DataTable listData = _baseuserbll.OptionUserList(keywords);
            return Content(Util.Json.ToJson(listData));
        }

        /// <summary>
        /// 【用户管理】返回用户列表JSON
        /// </summary>
        /// <param name="keywords">查询关键字</param>
        /// <param name="departmentId">部门ID</param>
        /// <param name="jqgridparam">表格参数</param>
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
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
        /// <summary>
        /// 【用户管理】提交表单
        /// </summary>
        /// <param name="KeyValue">主键值</param>
        /// <param name="Baseuser">用户信息</param>
        /// <param name="Baseemployee">员工信息</param>
        /// <param name="BuildFormJson">自定义表单</param>
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


                string message = KeyValue == "" ? "新增成功。" : "编辑成功。";
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
                    //新增卡片,用于发卡机发卡
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
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        /// <summary>
        /// 获取用户职员信息对象返回JSON
        /// </summary>
        /// <param name="KeyValue">主键值</param>
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
            //公司
            strJson = strJson.Insert(1, "\"CompanyName\":\"" + Basecompany.FullName + "\",");
            //员工信息
            strJson = strJson.Insert(1, Util.Json.ToJson(Baseemployee).Replace("{", "").Replace("}", "") + ",");
            //自定义
            strJson = strJson.Insert(1, BaseFormAttributeBll.Instance.GetBuildForm(KeyValue));
            return Content(strJson);
        }
        #endregion

        #region 修改登录密码
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            ViewBag.Account = Request["Account"];
            return View();
        }
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="KeyValue">主键</param>
        /// <param name="Password">新密码</param>
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
                BaseSysLogBll.Instance.WriteLog(KeyValue, OperationType.Update, isOk.ToString(), "修改密码");
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = "密码修改成功。" }.ToString());
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog(KeyValue, OperationType.Update, "-1", "密码修改失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "密码修改失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 用户角色
        /// <summary>
        /// 用户角色
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public ActionResult UserRole()
        {
            return View();
        }
        /// <summary>
        /// 加载用户角色
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="UserId">用户Id</param>
        /// <returns></returns>
        public ActionResult UserRoleList(string UserId)
        {
            StringBuilder sb = new StringBuilder();
            DataTable dt = _baseuserbll.UserRoleList(UserId);
            foreach (DataRow dr in dt.Rows)
            {
                string strchecked = "";
                if (!string.IsNullOrEmpty(dr["objectid"].ToString()))//判断是否选中
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
        /// 用户角色 - 提交保存
        /// </summary>
        /// <param name="UserId">用户ID</param>
        /// <param name="ObjectId">角色id:1,2,3,4,5,6</param>
        /// <returns></returns>
        public ActionResult UserRoleSubmit(string UserId, string ObjectId)
        {
            try
            {
                string[] array = ObjectId.Split(',');
                int IsOk = _baseobjectuserrelationbll.BatchAddObject(UserId, array, "2");
                return Content(new JsonMessage { Success = true, Code = IsOk.ToString(), Message = "操作成功。" }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败，错误：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 个人中心
        /// <summary>
        /// 个人中心
        /// </summary>
        /// <returns></returns>
        public ActionResult PersonCenter()
        {
            ViewBag.imgGender = ManageProvider.Provider.Current().Gender == "男" ? "man.png" : "woman.png";
            ViewBag.strUserInfo = ManageProvider.Provider.Current().UserName + "（" + ManageProvider.Provider.Current().Account + "）";
            return View();
        }
        /// <summary>
        /// 验证旧密码
        /// </summary>
        /// <param name="OldPassword"></param>
        /// <returns></returns>
        public ActionResult ValidationOldPassword(string OldPassword)
        {
            if (ManageProvider.Provider.Current().Account == "System" || ManageProvider.Provider.Current().Account == "guest")
            {
                return Content(new JsonMessage { Success = true, Code = "0", Message = "当前账户不能修改密码" }.ToString());
            }
            OldPassword = Md5Helper.MD5(DESEncrypt.Encrypt(Md5Helper.MD5(OldPassword, 32).ToLower(), ManageProvider.Provider.Current().Secretkey).ToLower(), 32).ToLower();
            if (OldPassword != ManageProvider.Provider.Current().Password)
            {
                return Content(new JsonMessage { Success = true, Code = "0", Message = "原密码错误，请重新输入" }.ToString());
            }
            else
            {
                return Content(new JsonMessage { Success = true, Code = "1", Message = "通过信息验证" }.ToString());
            }
        }
        #endregion
    }
}