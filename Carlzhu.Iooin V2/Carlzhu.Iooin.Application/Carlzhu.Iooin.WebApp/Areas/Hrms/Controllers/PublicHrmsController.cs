using System;
using System.Web.Mvc;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class PublicHrmsController<TEntity> : PublicController<TEntity> where TEntity : BaseEntity, new()
    {
        // GET: Hrms/PublicHrms
        public class BehandValue
        {


            public int Dn { get; set; }
            public DateTime Date { get; set; }

            public int Value { get; set; }
            public string StringValue { get; set; }
        }

        public ActionResult EnumToJson()
        {
            return Content(Carlzhu.Iooin.Util.MvcHtml.SelectListExtendExpress.EnumToList(typeof(HrmsConfig.AttendanceStatusEnum)).ToJson());
        }

    }
}