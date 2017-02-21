using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Business.HrmsModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

using Carlzhu.Iooin.Framework.Data.DataAccess.DataBase;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.WebApp.Areas.Hrms.Controllers
{
    public class AttendnceController : PublicHrmsController<HrmsAttendance>
    {
        readonly HrmsAttendnceBll _hrmsAttendnceBll = new HrmsAttendnceBll();

        public ActionResult TempCardTo(string empNo, string kv)
        {
            return base.Content("1");
        }

        public ActionResult FillCard()
        {
            return base.View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="fillTime"></param>
        /// <returns></returns>
        public ActionResult SubmitFillCard(string empNo, string fillTime)
        {
            DateTime signTime = DateTime.Parse(fillTime);

  

            StringBuilder sql = new StringBuilder($"insert into eastriver.dbo.timerecords  (clock_id, emp_id,card_id,sign_time,dcollecttime,pos_sequ )  select '2' as clock_id, empno as emp_id,cardno as card_id,'{signTime}' as sign_time,getdate() as dcollecttime,'0' as pos_sequ from dbo.baseemployee where empno = '{empNo}' ");
            int e = DataFactory.Database().ExecuteBySql(sql);

            if (e > 0)
            {
             return   Json(new { Code = "1", Message = "补卡成功" });
            }
            else
            {
                return Json(new { Code = "-1", Message = "补卡失败" });
            }


        }

        #region 考勤历史数据
        /// <summary>
        /// 取得考勤原始数据
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="departmentId"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJson(string keywords,string yearMonth, string departmentId, Pagination jqgridparam)
        {
            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = _hrmsAttendnceBll.GetPageList(keywords, yearMonth, "", ref jqgridparam);// _baseuserbll.GetPageList(keywords, departmentId, ref jqgridparam);
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

        #endregion

        #region 考勤分析及明细查询

        public ActionResult Analysis()
        {
            return View();

        }


        /// <summary>
        /// 分析考勤数据
        /// </summary>
        /// <param name="yearmonth">分析月份</param>
        /// <param name="resourceId"></param>
        /// <returns></returns>
        public ActionResult AttDataAnalysis(string yearmonth, string resourceId)
        {
            string whereIn = DatabaseCommon.GetWhereIn(resourceId, ',', new Regex("\\d{7}"));
            List<BaseEmployee> baseEmployees = DataFactory.Database().FindList<BaseEmployee>($"AND IsDimission=1 AND  EmpNo IN ({whereIn})");

            var company = DataFactory.Database().FindEntity<BaseCompany>(ManageProvider.Provider.Current().CompanyId);

            _hrmsAttendnceBll.DataAnalysis(baseEmployees, company.SettlementDay, yearmonth);

            return Content("1");

        }


        /// <summary>
        /// 取得分析处理后的考勤数据
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="end"></param>
        /// <param name="jqgridparam"></param>
        /// <param name="start"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult GridPageAnalysisListJson(string keywords, Pagination jqgridparam, DateTime? start, DateTime? end, string type = "")
        {

            DateTime starTime = start == null ? DateTime.Now.AddDays(-7) : DateTime.Parse(start.ToString());
            DateTime endtime = end == null ? DateTime.Now : DateTime.Parse(end.ToString());

            try
            {
                Stopwatch watch = CommonHelper.TimerStart();
                DataTable listData = _hrmsAttendnceBll.GridPageAnalysisListJson(keywords, type, starTime, endtime, ref jqgridparam);// _baseuserbll.GetPageList(keywords, departmentId, ref jqgridparam);
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


        #endregion


        #region 个人考勤汇总

        public ActionResult AttendnceTotal()
        {
            return View();
        }

        /// <summary>
        /// 个人汇总查询
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="departmentId"></param>
        /// <param name="yearmonth">cc</param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public ActionResult GridPageListJsonAttendnceTotal(string keywords, string departmentId, string yearmonth, Pagination jqgridparam)
        {
            yearmonth = yearmonth ?? DateTime.Now.ToString("yyyyMM");
            return Content(Util.Json.ToJson(_hrmsAttendnceBll.AttendnceTotal(keywords, departmentId, yearmonth)));
        }

        #endregion

        /// <summary>
        /// 指定时间内加班汇总，用于请假申请时查看
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        public ActionResult OvertimeByArea(string empNo, string startTime)
        {
            var start = DateTime.Parse(startTime);
            var model = _hrmsAttendnceBll.OvertimeByArea(empNo, start.AddMonths(-3), start);
            return Content(Util.Json.ToJson(model));
        }


        public ActionResult GetOverTimeColModel()
        {

            StringBuilder sb = new StringBuilder();

            //[

            sb.AppendLine("[");
            sb.AppendLine("{ label: '主键', name: 'rowid', index: 'rowid', width: 25, align: 'center', hidden: true },");
            sb.AppendLine("{ label: '月份', name: 'Calendar', index: 'Calendar', width: 60, align: 'center' },");
            sb.AppendLine("{ label: '姓名', name: 'Name', index: 'Name', width: 60, align: 'center' },");
            sb.AppendLine("{ label: '工号', name: 'EmpNo', index: 'EmpNo', width: 60, align: 'center' },");
            sb.AppendLine("{ label: 'D26', name: 'D26', index: 'D26', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D27', name: 'D27', index: 'D27', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D28', name: 'D28', index: 'D28', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D29', name: 'D29', index: 'D29', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D30', name: 'D30', index: 'D30', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D31', name: 'D31', index: 'D31', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D1', name: 'D1', index: 'D1', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D2', name: 'D2', index: 'D2', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D3', name: 'D3', index: 'D3', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D4', name: 'D4', index: 'D4', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D5', name: 'D5', index: 'D5', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D6', name: 'D6', index: 'D6', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D7', name: 'D7', index: 'D7', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D8', name: 'D8', index: 'D8', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D9', name: 'D9', index: 'D9', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D10', name: 'D10', index: 'D10', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D11', name: 'D11', index: 'D11', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D12', name: 'D12', index: 'D12', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D13', name: 'D13', index: 'D13', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D14', name: 'D14', index: 'D14', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D15', name: 'D15', index: 'D15', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D16', name: 'D16', index: 'D16', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D17', name: 'D17', index: 'D17', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D18', name: 'D18', index: 'D18', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D19', name: 'D19', index: 'D19', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D20', name: 'D20', index: 'D20', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D21', name: 'D21', index: 'D21', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D22', name: 'D22', index: 'D22', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D23', name: 'D23', index: 'D23', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D24', name: 'D24', index: 'D24', width: 25, align: 'center' },");
            sb.AppendLine("{ label: 'D25', name: 'D25', index: 'D25', width: 25, align: 'center' },");
            sb.AppendLine("{ label: '平时', name: 'TotalOvertime', index: 'TotalOvertime', width: 33, align: 'center' },");
            sb.AppendLine("{ label: '周末', name: 'TotalWeekendOvertime', index: 'TotalWeekendOvertime', width: 33, align: 'center' },");
            sb.AppendLine("{ label: '节日', name: 'TotalHolidayOvertime', index: 'TotalHolidayOvertime', width: 33, align: 'center' },");
            sb.AppendLine("{ label: '总和', name: 'TotalAllOvertime', index: 'TotalAllOvertime', width: 33, align: 'center' }");
            sb.AppendLine("]");
            return Json(sb.ToString(), JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 预估计假时数
        /// </summary>
        /// <returns></returns>
        public ActionResult EstimateRestTime(string empNo, string startTime, string endTime)
        {
            //var start = DateTime.Parse(startTime);
            //var end = DateTime.Parse(endTime);

            //AttDataAnalysis att = new AttDataAnalysis(start, end);

            //List<HrmsShift> hrmsShifts = DataFactory.Database().FindList<HrmsShift>();
            //var shiftSnaps = DataFactory.Database().FindList<HrmsShiftSnaps>($"AND EventEmpNo='{empNo}' and  EvenTime Between '{startTime}' and '{endTime}'");
            //var baseEmployee = DataFactory.Database().FindEntity<BaseEmployee>(empNo);

            //List<FormWorkLeave> formWorkLeaves = new List<FormWorkLeave>
            //{
            //    new FormWorkLeave()
            //    {
            //        EmpNo = empNo,
            //        StartTime = start,
            //        EndTime = end
            //    }
            //};

            //List<HrmsAttAnalysis> hrmsAttAnalyses = new List<HrmsAttAnalysis>();
            //int day = end.Day - start.Day + 1;



            //if (baseEmployee.IsShift)
            //{
            //    if (!shiftSnaps.Any()) return Content("请先排班");
            //}

            //for (int i = 0; i < day; i++)
            //{
            //    HrmsAttAnalysis hrmsAttAnalysis = new HrmsAttAnalysis();


            //    //当天班别
            //    HrmsShiftSnaps shiftSnap;
            //    //不需要排班
            //    if (!baseEmployee.IsShift)
            //    {
            //        var shift = hrmsShifts.Single(l => l.ShiftId == baseEmployee.DefaultShift);
            //        shiftSnap = new HrmsShiftSnaps
            //        {
            //            ShiftId = shift.ShiftId,
            //            WorkStart = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.WorkStart.ToShortTimeString()),
            //            WorkStop = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.WorkStop.ToShortTimeString()),
            //            Rist1Start = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist1Start.ToShortTimeString()),
            //            Rist1Stop = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist1Stop.ToShortTimeString()),
            //            Rist2Start = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist2Start.ToShortTimeString()),
            //            Rist2Stop = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist2Stop.ToShortTimeString()),
            //            Rist3Start = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist3Start.ToShortTimeString()),
            //            Rist3Stop = Convert.ToDateTime(start.AddDays(i).ToShortDateString() + " " + shift.Rist3Stop.ToShortTimeString()),
            //            SettlementMin = shift.SettlementMin,
            //            SettlementMax = shift.SettlementMax,
            //        };
            //    }
            //    else
            //    {
            //        shiftSnap = shiftSnaps.Single(c => c.EvenTime.Date == start.Date);
            //    }


            //    int Event = att._hrmsCalendarEventses.Single(m => m.CalendarItem == start.Day).D0;
            //    hrmsAttAnalyses.Add(att.WorkLeaveAnalysisAnd(hrmsAttAnalysis, shiftSnap, formWorkLeaves, Event));
            //}

            //double fi = hrmsAttAnalyses.Aggregate<HrmsAttAnalysis, double>(0, (current, k) => current + k.WorkLeave);


            return Content("0");
        }



    }
}