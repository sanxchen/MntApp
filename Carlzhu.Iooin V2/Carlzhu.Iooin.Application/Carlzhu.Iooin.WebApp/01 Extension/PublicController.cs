using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Framework.Data;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Iooin.Framework.Code;
using OperationType = Carlzhu.Iooin.Business.CommonModule.OperationType;


namespace Carlzhu.Iooin.WebApp
{
    public class PublicController<TEntity> : Controller where TEntity : BaseEntity, new()
    {




        public readonly RepositoryFactory<TEntity> Repositoryfactory = new RepositoryFactory<TEntity>();

        #region 列表
        /// <summary>
        /// 列表视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 绑定表格
        /// </summary>
        /// <param name="parameterJson">查询条件</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        [LoginAuthorize]
        public virtual JsonResult GridPageJson(string parameterJson, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                List<TEntity> listData;
                if (!string.IsNullOrEmpty(parameterJson))
                {
                    List<DbParameter> parameter;
                    IList conditions = parameterJson.JonsToList<Condition>();
                    string whereSql = ConditionBuilder.GetWhereSql(conditions, out parameter);
                    listData = Repositoryfactory.Repository().FindListPage(whereSql, parameter.ToArray(), ref jqgridparam);
                }
                else
                {
                    listData = Repositoryfactory.Repository().FindListPage(ref jqgridparam);
                }
                var jsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = listData,
                };
                return Json(jsonData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + parameterJson);
                return null;
            }
        }

        /// <summary>
        /// 绑定表格
        /// </summary>
        /// <param name="parameterJson">查询条件</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        [LoginAuthorize]
        public virtual JsonResult GridJson(string parameterJson, Pagination jqgridparam)
        {
            try
            {
                List<TEntity> listData;
                if (!string.IsNullOrEmpty(parameterJson))
                {
                    List<DbParameter> parameter;
                    IList conditions = parameterJson.JonsToList<Condition>();
                    string whereSql = ConditionBuilder.GetWhereSql(conditions, out parameter, jqgridparam.sidx, jqgridparam.sord);
                    listData = Repositoryfactory.Repository().FindList(whereSql, parameter.ToArray());
                }
                else
                {
                    listData = Repositoryfactory.Repository().FindList();
                }
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message + "\r\n条件：" + parameterJson);
                return null;
            }
        }
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="parentId">判断是否有子节点</param>
        /// <returns></returns>
        [HttpPost]
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Delete(string keyValue, string parentId)
        {
            string[] array = keyValue.Split(',');
            try
            {
                var message = "删除失败。";
                int isOk = 0;
                if (!string.IsNullOrEmpty(parentId))
                {
                    if (Repositoryfactory.Repository().FindCount("ParentId", parentId) > 0)
                    {
                        throw new Exception("当前所选有子节点数据，不能删除。");
                    }
                }
                isOk = Repositoryfactory.Repository().Delete(array);
                if (isOk > 0)
                {
                    message = "删除成功。";
                }
                WriteLog(isOk, array, message);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                WriteLog(-1, array, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
        #endregion

        #region 表单
        /// <summary>
        /// 明细视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Detail()
        {
            return View();
        }
        /// <summary>
        /// 表单视图
        /// </summary>
        /// <returns></returns>
        [ManagerPermission(PermissionMode.Enforce)]
        public virtual ActionResult Form()
        {
            return View();
        }
        /// <summary>
        /// 返回显示顺序号
        /// </summary>
        /// <returns></returns>
        [LoginAuthorize]
        public virtual ActionResult SortCode()
        {
            string strCode = BaseFactory.BaseHelper().GetSortCode<TEntity>("SortCode").ToString();
            return Content(strCode);
        }
        /// <summary>
        /// 表单赋值
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        //[LoginAuthorize]
        public virtual ActionResult SetForm(string keyValue)
        {
            TEntity entity = Repositoryfactory.Repository().FindEntity(keyValue);
            return Content(entity.ToJson());
        }

        public virtual ActionResult SetFormFiled(string propertyName, string propertyValue)
        {
            TEntity entity = Repositoryfactory.Repository().FindEntity(propertyName, propertyValue);
            return Content(entity.ToJson());
        }
        /// <summary>
        /// 提交表单
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        [LoginAuthorize]
        public virtual ActionResult SubmitForm(TEntity entity, string keyValue)
        {
            //处理布尔值
            foreach (PropertyInfo prop in entity.GetType().GetProperties().Where(prop => prop.PropertyType.FullName == "System.Boolean"))
            {
                prop.SetValue(entity, Request.Params[prop.Name]=="1");
            }


            try
            {
                int isOk = 0;
                string message = keyValue == "" ? "新增成功。" : "编辑成功。";
                if (!string.IsNullOrEmpty(keyValue))
                {
                    TEntity oldentity = Repositoryfactory.Repository().FindEntity(keyValue);//获取没更新之前实体对象
                    entity.Modify(keyValue);
                    isOk = Repositoryfactory.Repository().Update(entity);
                    this.WriteLog(isOk, entity, oldentity, keyValue, message);
                }
                else
                {
                    entity.Create();
                    isOk = Repositoryfactory.Repository().Insert(entity);
                    this.WriteLog(isOk, entity, null, keyValue, message);
                }
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                this.WriteLog(-1, entity, null, keyValue, "操作失败：" + ex.Message);
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }

        #endregion

        #region 写入作业日志

        /// <summary>
        /// 写入作业日志（新增、修改）
        /// </summary>
        /// <param name="isOk">操作状态</param>
        /// <param name="entity">实体对象</param>
        /// <param name="oldentity">之前实体对象</param>
        /// <param name="keyValue"></param>
        /// <param name="message">备注信息</param>
        public void WriteLog(int isOk, TEntity entity, TEntity oldentity, string keyValue, string message = "")
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                BaseSysLogBll.Instance.WriteLog(entity, OperationType.Add, isOk.ToString(), message);
            }
            else
            {
                BaseSysLogBll.Instance.WriteLog(oldentity, entity, OperationType.Update, isOk.ToString(), message);
            }
        }

        /// <summary>
        /// 写入作业日志（删除操作）
        /// </summary>
        /// <param name="isOk">操作状态</param>
        /// <param name="keyValue">主键值</param>
        /// <param name="message">备注信息</param>
        public void WriteLog(int isOk, string[] keyValue, string message = "")
        {
            BaseSysLogBll.Instance.WriteLog<TEntity>(keyValue, isOk.ToString(), message);
        }
        /// <summary>
        /// 写入作业日志（删除操作）
        /// </summary>
        /// <param name="isOk">操作状态</param>
        /// <param name="keyValue">主键值</param>
        /// <param name="message">备注信息</param>
        public void WriteLog(int isOk, string keyValue, string message = "")
        {
            string[] array = keyValue.Split(',');
            BaseSysLogBll.Instance.WriteLog<TEntity>(array, isOk.ToString(), message);
        }
        #endregion

    }
}