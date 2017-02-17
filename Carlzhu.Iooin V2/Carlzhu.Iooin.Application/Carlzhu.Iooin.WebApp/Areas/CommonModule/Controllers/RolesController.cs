
using System;
using System.Data;
using System.Diagnostics;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    /// <summary>
    /// 角色管理控制器
    /// </summary>
    public class RolesController : PublicController<BaseRoles>
    {
        BaseRolesBll Baserolesbll = new BaseRolesBll();

        /// <summary>
        /// 【角色管理】返回列表JONS
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="jqgridparam">JqGrid表格参数</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string CompanyId, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable ListData = Baserolesbll.GetPageList(CompanyId, ref jqgridparam);
                var JsonData = new
                {
                    total = jqgridparam.total,
                    page = jqgridparam.page,
                    records = jqgridparam.records,
                    costtime = CommonHelper.TimerEnd(watch),
                    rows = ListData,
                };
                return Content(Util.Json.ToJson(JsonData));
            }
            catch (Exception ex)
            {
                BaseSysLogBll.Instance.WriteLog("", OperationType.Query, "-1", "异常错误：" + ex.Message);
                return null;
            }
        }
    }
}