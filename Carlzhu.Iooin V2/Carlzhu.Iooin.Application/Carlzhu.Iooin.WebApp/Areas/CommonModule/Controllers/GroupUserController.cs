
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
    /// �û�����������
    /// </summary>
    public class GroupUserController : PublicController<BaseGroupUser>
    {
        BaseGroupUserBll Basegroupuserbll = new BaseGroupUserBll();
        /// <summary>
        /// ���û�����������б�JONS
        /// </summary>
        /// <param name="CompanyId">��˾ID</param>
        /// <param name="DepartmentId">����ID</param>
        /// <param name="jqgridparam">��ҳ����</param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string DepartmentId, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = Basegroupuserbll.GetPageList(DepartmentId, ref jqgridparam);
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
    }
}