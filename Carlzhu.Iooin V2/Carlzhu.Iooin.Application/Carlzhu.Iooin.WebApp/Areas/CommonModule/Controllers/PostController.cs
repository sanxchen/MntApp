
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
    /// 岗位管理控制器
    /// </summary>
    public class PostController : PublicController<BasePost>
    {
        BasePostBll Basepostbll = new BasePostBll();
        /// <summary>
        /// 【岗位管理】返回列表JONS
        /// </summary>
        /// <param name="CompanyId">公司ID</param>
        /// <param name="DepartmentId">部门ID</param>
        /// <param name="jqgridparam">分页条件</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string CompanyId, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = Basepostbll.GetPageList(CompanyId, ref jqgridparam);
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


        public ActionResult BinDingDataBaseItemsJson(string code, string CompanyId)
        {
            DataTable listData = base.Repositoryfactory.Repository().FindTable();

            return Content(Util.Json.ToJson(listData));
        }

    }
}