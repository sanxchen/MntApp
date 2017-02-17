using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Carlzhu.Iooin.Entity.HRMS;

using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class CalendarEventsController : PublicHrmsController<HrmsCalendarEvents>
    {
        // GET: Hrms/CalendarEvents
        public override ActionResult SubmitForm(HrmsCalendarEvents entity, string keyValue)
        {



            return base.SubmitForm(entity, keyValue);
        }



        [HttpPost]
        public ActionResult UpdateEvents(string data, DateTime date)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var events = js.Deserialize<List<dynamic>>(data);
            //List<BehandValue> de = (from item in events let dn = int.Parse(item["dn"].ToString()) let value = int.Parse(item["value"].ToString()) let date = DateTime.Parse(item["date"].ToString()) select new BehandValue() { Date = (DateTime)date, Dn = (int)dn, Value = (int)value }).ToList();

            List<BehandValue> de = new List<BehandValue>();
            foreach (var item in events)
            {
                //dynamic dn = int.Parse(item["dn"].ToString());
                dynamic value = int.Parse(item["value"].ToString());
                //dynamic date = DateTime.Parse(item["date"].ToString());
                de.Add(new BehandValue() { Value = (int)value });
            }

            #region New
            List<HrmsCalendarEvents> hrmsCalendar = new List<HrmsCalendarEvents>();

            var currentMonth = date.ToString("yyyyMM");
            var companyId = ManageProvider.Provider.Current().CompanyId;
            int days = DateTimeHelper.GetDaysOfMonth(date);
            Console.Write(days);
            for (int i = 0; i < days; i++)
            {
                hrmsCalendar.Add(new HrmsCalendarEvents()
                {
                    CompanyId = companyId,
                    CalendarEventsId = currentMonth,
                    CalendarItem = i + 1,
                    D0 = de[i].Value,
                });
            }






            #endregion







            //HrmsCalendarEvents hcEvents = new HrmsCalendarEvents
            //{
            //    CompanyId = ManageProvider.Provider.Current().CompanyId,
            //    CalendarEventsId = DateTime.Now.ToString("yyyyMM"),
            //    CalendarItem = 0,
            //    D1 = de[0].value,
            //    D2 = de[1].value,
            //    D3 = de[2].value,
            //    D4 = de[3].value,
            //    D5 = de[4].value,
            //    D6 = de[5].value,
            //    D7 = de[6].value,
            //    D8 = de[7].value,
            //    D9 = de[8].value,
            //    D10 = de[9].value,
            //    D11 = de[10].value,
            //    D12 = de[11].value,
            //    D13 = de[12].value,
            //    D14 = de[13].value,
            //    D15 = de[14].value,
            //    D16 = de[15].value,
            //    D17 = de[16].value,
            //    D18 = de[17].value,
            //    D19 = de[18].value,
            //    D20 = de[19].value,
            //    D21 = de[20].value,
            //    D22 = de[21].value,
            //    D23 = de[22].value,
            //    D24 = de[23].value,
            //    D25 = de[24].value,
            //    D26 = de[25].value,
            //    D27 = de[26].value,
            //    D28 = ((de.Count >= 28) ? de[27].value : -1),
            //    D29 = ((de.Count >= 29) ? de[28].value : -1),
            //    D30 = ((de.Count >= 30) ? de[29].value : -1),
            //    D31 = ((de.Count >= 31) ? de[30].value : -1)
            //};

            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();

            var old = database.FindList<HrmsCalendarEvents>($"AND CompanyId='{companyId}' AND CalendarEventsId='{currentMonth}'");
            if (old.Any()) database.Delete<HrmsCalendarEvents>(old, isOpenTrans);

            database.Insert(hrmsCalendar, isOpenTrans);

            database.Commit();
            return Content("1");
        }

        public ActionResult GetEvents(string calendarEventsId)
        {
            //HrmsCalendarEvents hs =
            //    DataFactory.Database()
            //        .FindList<HrmsCalendarEvents>("CalendarEventsId", calendarEventsId)
            //        .FirstOrDefault(c => c.CompanyId == ManageProvider.Provider.Current().CompanyId);


            List<HrmsCalendarEvents> hrmsCalendar =
                base.Repositoryfactory.Repository()
                    .FindList(
                        $"AND CompanyId='{ManageProvider.Provider.Current().CompanyId}' AND CalendarEventsId='{calendarEventsId}'");


            return Content(!hrmsCalendar.Any() ? "1" : Util.Json.ToJson(hrmsCalendar));
        }
    }
}