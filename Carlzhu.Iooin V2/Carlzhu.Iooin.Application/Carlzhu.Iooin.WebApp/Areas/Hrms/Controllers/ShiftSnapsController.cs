using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Script.Serialization;

using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using WebGrease.Css.Extensions;

namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class ShiftSnapsController : PublicHrmsController<HrmsShiftSnaps>
    {

        /// <summary>
        /// 排班
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult SubmitFormSnaps(string resource, string data)
        {
            List<string> res = new List<string>();
            var patten = new Regex("\\d{7}");
            string inRes = string.Empty;
            resource.Split(',').ForEach(c =>
            {
                if (!patten.IsMatch(c)) return;
                res.Add(c);
                inRes += $"'{c}',";
            });

            JavaScriptSerializer js = new JavaScriptSerializer();
            var events = js.Deserialize<List<dynamic>>(data);
            List<BehandValue> de = (from item in events let value = item["value"].ToString() let date = DateTime.Parse(item["date"].ToString()) select new BehandValue { Date = (DateTime)date, StringValue = value }).ToList();


            //查出所有班别

            var shifts = DataFactory.Database().FindList<HrmsShift>();


            List<HrmsShiftSnaps> hrmsShiftSnapses = new List<HrmsShiftSnaps>();

            res.ForEach(r => //所有人
            {
                de.ForEach(c => //本月所有天
                {
                    var shift = shifts.Single(m => m.Code == c.StringValue);
                    hrmsShiftSnapses.Add(new HrmsShiftSnaps()
                    {
                        EvenTime = c.Date,
                        EventEmpNo = r,
                        ShiftId = c.StringValue,
                        WorkStart = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.WorkStart.ToShortTimeString()),
                        WorkStop = Convert.ToDateTime((shift.ShiftId.StartsWith("A") ? c.Date : c.Date.AddDays(1)).ToShortDateString() + " " + shift.WorkStop.ToShortTimeString()),
                        Rist1Start = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist1Start.ToShortTimeString()),
                        Rist1Stop = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist1Stop.ToShortTimeString()),
                        Rist2Start = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist2Start.ToShortTimeString()),
                        Rist2Stop = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist2Stop.ToShortTimeString()),
                        Rist3Start = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist3Start.ToShortTimeString()),
                        Rist3Stop = Convert.ToDateTime(c.Date.ToShortDateString() + " " + shift.Rist3Stop.ToShortTimeString()),
                        SettlementMin = shift.SettlementMin,
                        SettlementMax = shift.SettlementMax
                    });

                });

            });


            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();
            var old = database.FindList<HrmsShiftSnaps>($"AND EventEmpNo in ({inRes.Substring(0, inRes.Length - 1)}) AND DATEPART(year, EvenTime)='{DateTime.Now.Year}' AND DATEPART(month, EvenTime)='{DateTime.Now.Month}'");
            if (old.Any()) database.Delete<HrmsShiftSnaps>(old);

            database.Insert(hrmsShiftSnapses, isOpenTrans);


            database.Commit();
            return Content("1");



        }

    }
}