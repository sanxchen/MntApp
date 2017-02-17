
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 接口管理控制器
    /// </summary>
    public class InterfaceManageController : PublicController<BaseInterfaceManage>
    {
        readonly BaseInterfaceManageBll _baseinterfacemanagebll = new BaseInterfaceManageBll();
        /// <summary>
        /// 【接口管理】返回接口列表JSON
        /// </summary>
        /// <param name="jqgridparam">分页参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(Pagination jqgridparam)
        {
            Stopwatch watch = CommonHelper.TimerStart();
            List<BaseInterfaceManage> listData = _baseinterfacemanagebll.GetPageList(ref jqgridparam);
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
        /// <summary>
        /// 【接口管理】返回接口参数列表JSON
        /// </summary>
        /// <param name="InterfaceId"></param>
        /// <returns></returns>
        public ActionResult GridInterfaceParameterListJson(string InterfaceId)
        {
            List<BaseInterfaceManageParameter> listData = _baseinterfacemanagebll.GetInterfaceParameterList(InterfaceId);
            var jsonData = new
            {
                rows = listData,
            };
            return Content(Util.Json.ToJson(jsonData));
        }
        /// <summary>
        /// 提交接口表单（新增、编辑）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">接口对象</param>
        /// <param name="parameterJson">接口参数</param>
        /// <returns></returns>
        public ActionResult SubmitInterfaceForm(string keyValue, BaseInterfaceManage entity, string parameterJson)
        {
            try
            {
                string message = keyValue == "" ? "新增成功。" : "编辑成功。";
                int isOk = _baseinterfacemanagebll.SubmitInterfaceForm(keyValue, entity, parameterJson);
                return Content(new JsonMessage { Success = true, Code = isOk.ToString(), Message = message }.ToString());
            }
            catch (Exception ex)
            {
                return Content(new JsonMessage { Success = false, Code = "-1", Message = "操作失败：" + ex.Message }.ToString());
            }
        }
    }
}