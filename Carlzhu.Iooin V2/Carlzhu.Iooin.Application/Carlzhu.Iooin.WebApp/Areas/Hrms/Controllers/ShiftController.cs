using System;
using System.Web.Mvc;
using Carlzhu.Iooin.Entity.HRMS;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class ShiftController : PublicController<HrmsShift>
    {
        // GET: Hrms/Shift

        [HttpPost]
        public ActionResult GetEmpShift(string empNo)
        {
            var models = DataFactory.Database().FindList<HrmsShiftSnaps>($"AND EventEmpNo='{empNo}' AND  DATEPART(year, EvenTime)='{DateTime.Now.Year}' and DATEPART(month, EvenTime)='{DateTime.Now.Month}'");
            return Content(models.ToJson());
        }


        public void import()
        {

            //历史班别数据
            //var datatable = DataFactory.Database().FindTableBySql("select * from eastriver.dbo.Scheme where EMP_ID IN  (SELECT EMP_NO FROM RMS.DBO.EMPLOYEE WHERE EMP_STATUS='Y' and emp_position_type in ('M','P','S') ) AND YM LIKE '2016-__' order by emp_id ");

            //foreach (var item in datatable.Rows)
            //{




            //}









            var dt = DataFactory.Database().FindListBySql<BaseEmployee>("AND IsDimission!=0");

            Console.Write(dt);


        }



        public ActionResult ShiftData()
        {
            return View();
        }

        //public ActionResult GridPageJson()
        //{
        //    var models = DataFactory.Database().FindList<HrmsShift>();
        //    return Content(models.ToJson());
        //}
    }
}