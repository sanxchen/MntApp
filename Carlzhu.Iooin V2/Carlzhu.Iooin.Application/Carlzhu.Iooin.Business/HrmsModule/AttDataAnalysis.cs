using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.FORM.f;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Framework.Data.DataAccess;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Business.HrmsModule
{
    public class AttDataAnalysis
    {

        private readonly List<HrmsAttAnalysis> _oldDatAnalyses = new List<HrmsAttAnalysis>();
        private readonly List<HrmsAttAnalysis> _newDatAnalyses = new List<HrmsAttAnalysis>();
        private readonly List<HrmsShift> _hrmsShifts = DataFactory.Database().FindList<HrmsShift>();

        public AttDataAnalysis(DateTime startTime, DateTime endTime)
        {
            this._hrmsCalendarEventses = DataFactory.Database().FindListBySql<HrmsCalendarEvents>($"" +
                                                                 $"SELECT * FROM HrmsCalendarEvents  hc " +
                                                                 $"LEFT OUTER JOIN BaseCompany bc ON bc.CompanyId = hc.CompanyId " +
                                                                 $"WHERE( " +
                                                                 $"hc.CompanyId = '{ManageProvider.Provider.Current().CompanyId}') AND" +
                                                                 $"( " +
                                                                 $"     (hc.CalendarEventsId = '{startTime.ToString("yyyyMM")}'  AND hc.CalendarItem > bc.SettlementDay ) " +
                                                                 $" OR " +
                                                                 $"     (hc.CalendarEventsId = '{endTime.ToString("yyyyMM")}'  AND hc.CalendarItem <= bc.SettlementDay)" +
                                                                 $")");
        }


        //整理行事历
        public readonly List<HrmsCalendarEvents> _hrmsCalendarEventses;


        //当月排班数据
        private List<HrmsShiftSnaps> _shiftSnaps = new List<HrmsShiftSnaps>();

        /// <summary>
        /// 提交所有修改
        /// </summary>
        public void SubmitAnalyses()
        {

            IDatabase database = DataFactory.Database();
            DbTransaction isOpenTrans = database.BeginTrans();

            if (_oldDatAnalyses.Any()) database.Delete<HrmsAttAnalysis>(_oldDatAnalyses, isOpenTrans);
            if (_newDatAnalyses.Any()) database.Insert<HrmsAttAnalysis>(_newDatAnalyses, isOpenTrans);

            database.Commit();
        }


        #region 考勤分析种类

        /// <summary>
        /// 正常打卡数据分析
        /// </summary>
        /// <param name="baseEmployee"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool PunchCardDataAnalysis(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime)
        {
            if (endTime > DateTime.Now) endTime = DateTime.Now;

            if (!_hrmsCalendarEventses.Any())
            {
                //没有行事历
                return false;
            }

            var empNo = baseEmployee.EmpNo;
            //整理结果
            List<HrmsAttAnalysis> hrmsAttAnalyses = new List<HrmsAttAnalysis>();
            //所有考考勤数据
            List<HrmsAttendance> hrmsAttendances = new HrmsAttendnceBll().GetAttData(empNo, startTime, endTime.AddDays(2));

            //所有请假数据
            var workleaveTime =
                DataFactory.Database()
                    .FindListBySql<FormWorkLeave>(
                        $"SELECT fw.* FROM MINICUT.DBO.FormWorkLeave fw,MINICUT.DBO.Form  f WHERE fw.FormNo=f.FormNo AND f.FormStatus={(int)Form.StatusEnum.签核完成} AND  fw.Starttime>='{startTime}' AND fw.EmpNo='{baseEmployee.EmpNo}' ");

            //没有考勤数据直反回；
            //if (!hrmsAttendances.Any())
            //{
                
            //    return false;
            //}

            //要求排班，并且没有排班数据
            _shiftSnaps = DataFactory.Database().FindList<HrmsShiftSnaps>($"AND EventEmpNo='{empNo}' and  EvenTime Between '{startTime}' and '{endTime}'");
            if (baseEmployee.IsShift)
            {
                if (!_shiftSnaps.Any()) return false;
            }


            //原排班数据
            _oldDatAnalyses.AddRange(DataFactory.Database().FindList<HrmsAttAnalysis>($"AND EmpNo='{empNo}' AND attDate BETWEEN '{startTime}' AND '{endTime}' "));

            while (startTime <= endTime)
            {
                //当天班别
                HrmsShiftSnaps shiftSnap;
                //不需要排班
                if (!baseEmployee.IsShift)
                {
                    var shift = _hrmsShifts.Single(l => l.ShiftId == baseEmployee.DefaultShift);
                    shiftSnap = new HrmsShiftSnaps
                    {
                        ShiftId = shift.ShiftId,
                        WorkStart = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.WorkStart.ToShortTimeString()),
                        WorkStop = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.WorkStop.ToShortTimeString()),
                        Rist1Start = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist1Start.ToShortTimeString()),
                        Rist1Stop = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist1Stop.ToShortTimeString()),
                        Rist2Start = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist2Start.ToShortTimeString()),
                        Rist2Stop = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist2Stop.ToShortTimeString()),
                        Rist3Start = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist3Start.ToShortTimeString()),
                        Rist3Stop = Convert.ToDateTime(startTime.ToShortDateString() + " " + shift.Rist3Stop.ToShortTimeString()),
                        SettlementMin = shift.SettlementMin,
                        SettlementMax = shift.SettlementMax,
                    };
                }
                else
                {
                    //没有当天排班直接退出
                    shiftSnap = _shiftSnaps.FirstOrDefault(c => c.EvenTime.Date == startTime.Date);
                    if (shiftSnap?.ShiftId == null)
                    {
                        startTime = startTime.AddDays(1);
                        continue;
                    }
                }

                //当天刷卡记录
                var day = hrmsAttendances.Where(c => c.SignTime >= startTime.Date && c.SignTime <= startTime.AddDays(2).Date).ToList();
                //计算当天上下班时间
                string formNoOn = string.Empty;
                string formNoOff = string.Empty;
                var hr = new HrmsAttAnalysis
                {
                    EmpNo = empNo,
                    Shift = shiftSnap.ShiftId,
                    AttDate = startTime.Date,
                    AnalysisTime = DateTime.Now,
                    OnDutty = day.Any() ? this.GetOnDuttyDateTime(shiftSnap.WorkStart, day, ref formNoOn) : HrmsConfig.DefaultDateTime,
                    OffDutty = day.Any() ? this.GetOffDuttyDateTime(shiftSnap.WorkStop, day, shiftSnap.ShiftId, ref formNoOff) : HrmsConfig.DefaultDateTime, //确认当天是否有考勤
                    FormNo = (!string.IsNullOrEmpty(formNoOn)) ? formNoOn : ((!string.IsNullOrEmpty(formNoOff)) ? formNoOff : null),
                };


                //根据行事历取值
                int Event = _hrmsCalendarEventses.Single(m => m.CalendarItem == startTime.Day).D0;
                //Event = shiftSnap.CalendarEnum;
                switch (Event)
                {
                    case 0://正常班
                        hr = EventNormal(hr, shiftSnap);
                        break;
                    case 2://加班
                        hr = EventWeeken(hr, shiftSnap);
                        break;
                    case 3://节日加班
                        hr = EventHoliday(hr, shiftSnap);
                        break;
                    default:
                        break;
                }

                //分析处理状态并保存
                hr = this.ResultsOfAnalysisAndProcessing(hr, Event);

                //分析请假
                hr = this.WorkLeaveAnalysisAnd(hr, shiftSnap, workleaveTime, Event);


                hrmsAttAnalyses.Add(hr);

                startTime = startTime.AddDays(1);
            }

            if (!hrmsAttAnalyses.Any()) return true;
            _newDatAnalyses.AddRange(hrmsAttAnalyses);




            //分析当月数据





            return true;
        }


        /// <summary>
        /// 临时卡分析
        /// </summary>
        /// <param name="baseEmployee"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool TemporaryCardAnalysis(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime)
        {
            //查询当月临时卡补卡记录




            return true;
        }
        /// <summary>
        /// 表单分析
        /// </summary>
        /// <param name="baseEmployee"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool WorkFlowAnalysis(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime)
        {
            //List<FormWorkLeave> all = DataFactory.Database().FindList<FormWorkLeave>($"AND EmpNo='{baseEmployee.EmpNo}' and StartTime BETWEEN '{startTime}' AND '{endTime}'");


            return true;
        }

        /// <summary>
        /// 手工处理分析
        /// </summary>
        /// <param name="baseEmployee"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public bool ManualProcessingAnalysis(BaseEmployee baseEmployee, DateTime startTime, DateTime endTime)
        {
            return true;
        }

        #endregion

        #region 分析获取上下班时间

        /// <summary>
        /// 根据打卡时间取得上班时间
        /// </summary>
        /// <param name="workTime"></param>
        /// <param name="daysAttendances"></param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        DateTime GetOnDuttyDateTime(DateTime workTime, List<HrmsAttendance> daysAttendances, ref string formNo)
        {
            //上班只能是当天
            foreach (var daysAttendance in daysAttendances.Where(m => m.SignTime.Date == workTime.Date))
            {
                daysAttendance.TimeDifference = daysAttendance.SignTime.Subtract(workTime).TotalMinutes;
            }
            //取4小时容差
            var firstOrDefault = daysAttendances.FirstOrDefault(c => c.SignTime > workTime.AddHours(-2) && c.SignTime < workTime.AddHours(4));

            if (firstOrDefault == null) return HrmsConfig.DefaultDateTime;

            formNo = (!string.IsNullOrEmpty(firstOrDefault.BillNo)) && firstOrDefault.BillNo.Length == 13 ? firstOrDefault.BillNo : null;
            return firstOrDefault.SignTime;

            // return firstOrDefault?.SignTime ?? HrmsConfig.DefaultDateTime;

        }


        /// <summary>
        /// 获取下班时间
        /// </summary>
        /// <param name="workTime">工作结束时间</param>
        /// <param name="daysAttendances">当天考勤</param>
        /// <param name="shiftId">班别</param>
        /// <param name="formNo"></param>
        /// <returns></returns>
        DateTime GetOffDuttyDateTime(DateTime workTime, List<HrmsAttendance> daysAttendances, string shiftId, ref string formNo)
        {
            foreach (var daysAttendance in daysAttendances)
            {
                daysAttendance.TimeDifference = daysAttendance.SignTime.Subtract(workTime).TotalMinutes;
            }

            HrmsAttendance firstOrDefault;

            if (shiftId.StartsWith("A"))
            {
                //取4小时容差
                firstOrDefault = daysAttendances.FirstOrDefault(c =>
                 c.SignTime.Date == workTime.Date //当天班
                 && c.SignTime > workTime.AddHours(-5) //提前下班
                 && c.SignTime < workTime.AddHours(7) //加班
                 );
            }
            else
            {
                //后一天，下早班，加班
                firstOrDefault = daysAttendances.FirstOrDefault(c =>
                 c.SignTime.Date == workTime.Date //夜班晚一天
                 && c.SignTime > workTime.AddHours(-4) //提前4小时下班
                 && c.SignTime < workTime.AddHours(7));//超时加班2+2+3小时

            }

            if (firstOrDefault == null) return HrmsConfig.DefaultDateTime;

            formNo = (!string.IsNullOrEmpty(firstOrDefault.BillNo)) && firstOrDefault.BillNo.Length == 13 ? firstOrDefault.BillNo : null;
            return firstOrDefault.SignTime;

            //return firstOrDefault?.SignTime ?? HrmsConfig.DefaultDateTime;


        }


        #endregion


        #region 整理出勤










        /// <summary>
        /// 平时
        /// </summary>
        /// <param name="sign"></param>
        /// <param name="snaps"></param>
        /// <returns></returns>
        HrmsAttAnalysis EventNormal(HrmsAttAnalysis sign, HrmsShiftSnaps snaps)
        {
            if (sign.OnDutty == HrmsConfig.DefaultDateTime || sign.OffDutty == HrmsConfig.DefaultDateTime) return sign;

            //上班
            sign.WorkingHours =
                Math.Round(
                    DateTimeHelper.ExcludeTimeSet(sign.OnDutty, sign.OffDutty, new Hashtable()
                    {
                        {snaps.Rist1Start, snaps.Rist1Stop},
                        {snaps.Rist2Start, snaps.Rist2Stop}

                    }, Convert.ToDateTime(sign.OnDutty.ToShortDateString() + " " + snaps.WorkStart.ToShortTimeString()), snaps.SettlementMin, snaps.SettlementMax)
                        .TotalHours, 1);



            //迟到
            if (sign.OnDutty > snaps.WorkStart) sign.Late = Math.Abs(Math.Round((snaps.WorkStart - sign.OnDutty).TotalMinutes, 0));

            //早退,当天工作总时间减去已上班时间
            if (sign.OffDutty < snaps.WorkStop)
                //sign.LeaveEarly = Math.Abs(
                //    Math.Round(
                //        DateTimeHelper.ExcludeTimeSet(snaps.WorkStart, snaps.WorkStop, new Hashtable()
                //        {
                //            {snaps.Rist1Start, snaps.Rist1Stop},
                //            {snaps.Rist2Start, snaps.Rist2Stop}

                //        }, Convert.ToDateTime(sign.OnDutty.ToShortDateString() + " " + snaps.WorkStart.ToShortTimeString()), snaps.SettlementMin, snaps.SettlementMax)
                //            .TotalHours, 1) - sign.WorkingHours);


                sign.LeaveEarly = Math.Round((snaps.WorkStop - sign.OffDutty).TotalMinutes, 0);




            //平时加班
            if (sign.OffDutty > snaps.WorkStop)
                sign.Overtime =
                Math.Round(
                    DateTimeHelper.ExcludeTimeSet(snaps.WorkStop, sign.OffDutty, new Hashtable
                    {
                        {snaps.Rist2Start, snaps.Rist2Stop}

                    }, snaps.SettlementMin, snaps.SettlementMax)
                        .TotalHours, 1);




            return sign;
        }

        HrmsAttAnalysis EventWeeken(HrmsAttAnalysis sign, HrmsShiftSnaps snaps)
        {
            if (sign.OnDutty == HrmsConfig.DefaultDateTime || sign.OffDutty == HrmsConfig.DefaultDateTime) return sign;
            //上班
            sign.WorkingHours =
                Math.Round(
                    DateTimeHelper.ExcludeTimeSet(sign.OnDutty, sign.OffDutty, new Hashtable()
                    {
                        {snaps.Rist1Start, snaps.Rist1Stop},
                        {snaps.Rist2Start, snaps.Rist2Stop}

                    }, Convert.ToDateTime(sign.OnDutty.ToShortDateString() + " " + snaps.WorkStart.ToShortTimeString()), snaps.SettlementMin, snaps.SettlementMax)
                        .TotalHours, 1);

            //周末加班
            sign.WeekendOvertime = sign.WorkingHours;


            return sign;
        }

        HrmsAttAnalysis EventHoliday(HrmsAttAnalysis sign, HrmsShiftSnaps snaps)
        {
            if (sign.OnDutty == HrmsConfig.DefaultDateTime || sign.OffDutty == HrmsConfig.DefaultDateTime) return sign;

            //上班
            sign.WorkingHours =
                 Math.Round(
                     DateTimeHelper.ExcludeTimeSet(sign.OnDutty, sign.OffDutty, new Hashtable()
                     {
                        {snaps.Rist1Start, snaps.Rist1Stop},
                        {snaps.Rist2Start, snaps.Rist2Stop}

                     }, Convert.ToDateTime(sign.OnDutty.ToShortDateString() + " " + snaps.WorkStart.ToShortTimeString()), snaps.SettlementMin, snaps.SettlementMax)
                         .TotalHours, 1);

            //节日加班
            sign.HolidayOvertime = sign.WorkingHours;


            return sign;
        }


        #endregion




        public HrmsAttAnalysis ResultsOfAnalysisAndProcessing(HrmsAttAnalysis hr, int Event)
        {
            switch (Event)
            {
                case 2:
                    if (hr.OnDutty != HrmsConfig.DefaultDateTime && hr.OffDutty == HrmsConfig.DefaultDateTime)
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.休息下班未打卡;
                    }
                    else if (hr.OnDutty == HrmsConfig.DefaultDateTime && hr.OffDutty != HrmsConfig.DefaultDateTime)
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.休息上班未打卡;
                    }
                    else
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.周末;
                    }

                    break;
                case 3:

                    if (hr.OnDutty != HrmsConfig.DefaultDateTime && hr.OffDutty == HrmsConfig.DefaultDateTime)
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.休息下班未打卡;
                    }
                    else if (hr.OnDutty == HrmsConfig.DefaultDateTime && hr.OffDutty != HrmsConfig.DefaultDateTime)
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.休息上班未打卡;
                    }
                    else
                    {
                        hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.国假;
                    }
                    break;
                default:
                    if (hr.OnDutty == HrmsConfig.DefaultDateTime && hr.OffDutty == HrmsConfig.DefaultDateTime) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.旷工;

                    else if (hr.OnDutty == HrmsConfig.DefaultDateTime) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.上班未打卡;

                    else if (hr.OffDutty == HrmsConfig.DefaultDateTime) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.下班未打卡;

                    else if (hr.Late > 0 && hr.LeaveEarly > 0) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.迟到早退;

                    else if (hr.Late > 0) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.迟到;

                    else if (hr.LeaveEarly > 0) hr.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.早退;
                    break;
            }

            return hr;
        }


        /// <summary>
        /// 分析请假时间
        /// </summary>
        /// <param name="sign">用于返回存储</param>
        /// <param name="snaps">当天班别</param>
        /// <param name="workleaveTimes">多张请假单</param>
        /// <param name="Event">当天事件</param>
        public HrmsAttAnalysis WorkLeaveAnalysisAnd(HrmsAttAnalysis sign, HrmsShiftSnaps snaps, List<FormWorkLeave> workleaveTimes, int Event)
        {
            //取得当天的请假
            var workleaveTime = workleaveTimes.SingleOrDefault(c => c.EndTime.Date >= sign.AttDate && c.StartTime.Date <= sign.AttDate);
            DateTime start;//请假起止时间
            DateTime end;
            if (workleaveTime != null && Event == (int)HrmsConfig.AttendanceStatusEnum.正常)
            {
                //请假时间如果小于上班时间，就按上班时间
                start = workleaveTime.StartTime > snaps.WorkStart ? workleaveTime.StartTime : snaps.WorkStart;

                //请假时间如果超过上班时间，就按上班结束时间。
                end = workleaveTime.EndTime > snaps.WorkStop ? snaps.WorkStop : workleaveTime.EndTime;

                sign.WorkLeave =
                Math.Round(
                    DateTimeHelper.ExcludeTimeSet(start, end, new Hashtable()
                    {
                        {snaps.Rist1Start, snaps.Rist1Stop},
                        {snaps.Rist2Start, snaps.Rist2Stop}

                    }, Convert.ToDateTime(sign.OnDutty.ToShortDateString() + " " + snaps.WorkStart.ToShortTimeString()), snaps.SettlementMin, snaps.SettlementMax)
                        .TotalHours, 1);


                switch ((int)workleaveTime.LeaveType)
                {
                    case (int)FormWorkLeave.LeaveTypEnum.调休:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.调休;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.丧假:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.丧假;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.事假:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.事假;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.产假:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.产假;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.婚假:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.婚假;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.病假:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.病假;
                        break;
                    case (int)FormWorkLeave.LeaveTypEnum.补休:
                        sign.TreatmentResult = (int)HrmsConfig.AttendanceStatusEnum.补休;
                        break;
                }

                sign.FormNo = workleaveTime.FormNo;
            }
            return sign;
        }
    }
}
