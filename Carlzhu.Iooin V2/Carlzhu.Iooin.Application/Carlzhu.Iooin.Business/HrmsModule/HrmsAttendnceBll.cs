using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Framework.Data.DataAccess.DataBase;
using Carlzhu.Iooin.Framework.Data.DataAccess.DbProvider;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Framework.Data.WebControl;
using Carlzhu.Iooin.Util;
using Carlzhu.Iooin.Util.Extension;


namespace Carlzhu.Iooin.Business.HrmsModule
{
    public class HrmsAttendnceBll : RepositoryFactory<HrmsAttendance>
    {
        /// <summary>
        /// 所有考勤原档数据
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="yearMonth"></param>
        /// <param name="departmentId"></param>
        /// <param name="jqgridparam"></param>
        /// <returns></returns>
        public DataTable GetPageList(string keyword,string yearMonth, string departmentId, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            //strSql.Append($"select * from (select ha.*,be.realname from dbo.HrmsAttendance ha left join baseemployee be on ha.empno=be.empno) T WHERE 1=1 ");

            strSql.Append($"select * from ( select ha.ID AS ROWID,ha.CARD_ID AS CardNo,ha.clock_id as ClockId,ha.emp_id as EmpNo,ha.Sign_time as signtime,ha.mark as mark,ha.flag as flag,ha.bill_id as billid,ha.dcollecttime as collecttime,ha.eventname as eventname,be.realname from EASTRIVER.DBO.TIMERECORDS ha left join baseemployee be on ha.emp_id = be.empno ) T WHERE 1 = 1 ");

            if (!string.IsNullOrEmpty(yearMonth))
            {
                strSql.Append($"AND  CONVERT(varchar(100), signtime, 112) LIKE '{yearMonth}%'");
            }


            if (!ManageProvider.Provider.Current().IsSystem)
            {
                //strSql.Append(" AND ( EmpNo IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
                //new BaseDataScopePermissionBll().GetScopeUserIdList("58e86c4c-8022-4d30-95d5-b3d0eedcc878", "", "");
                strSql.Append($" AND empNo in ({new BaseDataScopePermissionBll().GetScopeUserIdList("58e86c4c-8022-4d30-95d5-b3d0eedcc878", "", "")})");
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                string whereIn = DatabaseCommon.GetWhereIn(keyword, ',', new Regex("\\d{7}"));
                strSql.Append($" AND empNo in ({whereIn})");
            }


            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }




        // <summary>
        /// 所有考勤原档数据
        /// 
        /// <param name="keyword"></param>
        /// <param name="end"></param>
        /// <param name="jqgridparam"></param>
        /// <param name="abnorType"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public DataTable GridPageAnalysisListJson(string keyword, string abnorType, DateTime start, DateTime end, ref Pagination jqgridparam)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append($"SELECT * FROM ( SELECT ht.*,be.RealName FROM [dbo].[HrmsAttAnalysis] ht LEFT JOIN [dbo].[BaseEmployee] be ON ht.EmpNo=be.EmpNo ) T WHERE 1=1 AND  attDate BETWEEN '{start}' AND '{end}'");

            switch (abnorType)
            {
                case "999":
                    strSql.Append($" AND  treatmentresult in " +
                                  $"(" +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.旷工}'," +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.上班未打卡}'," +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.下班未打卡}'," +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.迟到}'," +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.早退}'," +
                                  $"'{(int)HrmsConfig.AttendanceStatusEnum.迟到早退}'" +
                                  $")");
                    break;
                case "1000":
                    //综合不作处理
                    break;
                default:
                    strSql.Append($" AND  treatmentresult='{abnorType}'");
                    break;
            }


            //if (!string.IsNullOrEmpty(abnorType) && abnorType != "1000")
            //{
            //    strSql.Append($" AND  treatmentresult='{abnorType}'");
            //}
            //排除临时卡
          //  strSql.Append("AND realname not like '临时卡%'");


            if (!string.IsNullOrEmpty(keyword))
            {
                string whereIn = DatabaseCommon.GetWhereIn(keyword, ',', new Regex("\\d{7}"));

                strSql.Append($" AND empNo in ({whereIn})");
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append($" AND EmpNo in ({new BaseDataScopePermissionBll().GetScopeUserIdList("58e86c4c-8022-4d30-95d5-b3d0eedcc878", "", "")})");
                //strSql.Append(" AND ( EmpNo IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                //strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                //strSql.Append(" ) )");
            }

            return Repository().FindTablePageBySql(strSql.ToString(), parameter.ToArray(), ref jqgridparam);
        }


        /// <summary>
        /// 获取指定用户指定时间的打卡数据进行分析
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public List<HrmsAttendance> GetAttData(string empNo, DateTime startTime, DateTime endTime)
        {
            StringBuilder strSql = new StringBuilder();
            List<DbParameter> parameter = new List<DbParameter>();
            strSql.Append($"select * from ( select ha.ID AS ROWID,ha.CARD_ID AS CardNo,ha.clock_id as ClockId,ha.emp_id as EmpNo,ha.Sign_time as signtime,ha.mark as mark,ha.flag as flag,ha.bill_id as billno,ha.dcollecttime as collecttime,ha.eventname as eventname,be.realname from EASTRIVER.DBO.TIMERECORDS ha left join baseemployee be on ha.emp_id = be.empno ) T WHERE 1 = 1 AND  signtime BETWEEN '{startTime}' AND '{endTime}' ");

            if (!string.IsNullOrEmpty(empNo))
            {
                strSql.Append(@" AND empNo = @keyword");
                parameter.Add(DbFactory.CreateDbParameter("@keyword", empNo));
            }
            if (!ManageProvider.Provider.Current().IsSystem)
            {
                strSql.Append(" AND ( EmpNo IN ( SELECT ResourceId FROM BaseDataScopePermission WHERE");
                strSql.Append(" ObjectId IN ('" + ManageProvider.Provider.Current().ObjectId.Replace(",", "','") + "') ");
                strSql.Append(" ) )");
            }

            return Repository().FindListBySql(strSql.Append(" ORDER BY SIGNTIME").ToString(), parameter.ToArray());
        }


        /// <summary>
        /// 考勤数据分析
        /// </summary>
        /// <param name="baseEmployees"></param>
        /// <param name="settlementDay"></param>
        /// <param name="yearmonth"></param>
        /// <returns></returns>
        public void DataAnalysis(List<BaseEmployee> baseEmployees, int settlementDay, string yearmonth)
        {
            //var now = DateTime.Parse($"{yearmonth.Substring(0, 4)}-{yearmonth.Substring(5, 2)}-{settlementDay}");


            ////考勤开始结束时间
            //DateTime startTime = now.AddDays(settlementDay - now.Day + 1).AddMonths(-1).Date;
            //DateTime endTime = now.Date;


            DateTime startTime = (DateTime.Parse($"{yearmonth.Substring(0, 4)}-{yearmonth.Substring(4, 2)}-{settlementDay}")).AddMonths(-1).AddDays(1); //26
            DateTime endTime = DateTime.Parse($"{yearmonth.Substring(0, 4)}-{yearmonth.Substring(4, 2)}-{settlementDay}"); //25

            AttenEvent eAttenEvent = new AttenEvent();
            AttDataAnalysis att = new AttDataAnalysis(startTime, endTime);

            //分析正常考勤
            eAttenEvent.AttAnalysisEventHandler += (baseEmployee, startTime1, endTime1) => att.PunchCardDataAnalysis(baseEmployee, startTime1, endTime1);

            //临时卡数据分析
            eAttenEvent.AttAnalysisEventHandler += (baseEmployee, startTime1, endTime1) => att.TemporaryCardAnalysis(baseEmployee, startTime1, endTime1);

            //表单号分析
            eAttenEvent.AttAnalysisEventHandler += (baseEmployee, startTime1, endTime1) => att.WorkFlowAnalysis(baseEmployee, startTime1, endTime1);

            //手工处理分析
            eAttenEvent.AttAnalysisEventHandler += (baseEmployee, startTime1, endTime1) => att.ManualProcessingAnalysis(baseEmployee, startTime1, endTime1);


            foreach (var baseEmployee in baseEmployees)
            {
                eAttenEvent.OnAttAnalysisEventHandler(baseEmployee, startTime.Date, endTime.Date);
            }

            att.SubmitAnalyses();
        }


        /// <summary>
        /// 月结日考勤汇总
        /// </summary>
        /// <param name="keywords"></param>
        /// <param name="departmentId"></param>
        /// <param name="yearMonth"></param>
        public DataTable AttendnceTotal(string keywords, string departmentId, string yearMonth)
        {
            //DateTime end = DateTime.MinValue;
            //var start = this.YearMonth(yearMonth, "25", out end);
            DateTime start = (DateTime.Parse($"{yearMonth.Substring(0, 4)}-{yearMonth.Substring(4, 2)}-{25}")).AddMonths(-1).AddDays(1); //26
            DateTime end = DateTime.Parse($"{yearMonth.Substring(0, 4)}-{yearMonth.Substring(4, 2)}-{25}"); //25



            BaseEmployeeBll baseEmployeeBll = new BaseEmployeeBll();

            //所有在职员工
            var employees = //baseEmployeeBll.GetListIsDimission();

            baseEmployeeBll.GetListByWheresql($" AND empNo in ({new BaseDataScopePermissionBll().GetScopeUserIdList("58e86c4c-8022-4d30-95d5-b3d0eedcc878", "", "")})");


            if (!string.IsNullOrEmpty(keywords))
            {
                var newemployee = new List<BaseEmployee>();
                keywords.Split(',').ToList().ForEach(m =>
                {
                    newemployee.Add(employees.FirstOrDefault(c => c.EmpNo == m));
                });
                newemployee.RemoveAll(c => c == null);
                employees = newemployee;
            }



            List<HrmsAttAnalysis> attAnalysis = DataFactory.Database().FindList<HrmsAttAnalysis>($"AND attdate>='{start}'  AND attdate <= '{end}'");





            List<HrmsPersonAttView> hrmsPersonAttViews = new List<HrmsPersonAttView>();

            employees.ForEach(m =>
            {

                var currentAtt = attAnalysis.Where(k => k.EmpNo == m.EmpNo).ToList();

                HrmsPersonAttView hrmsPersonAttView = new HrmsPersonAttView { Calendar = yearMonth };

                var day1 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 1);
                var day2 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 2);
                var day3 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 3);
                var day4 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 4);
                var day5 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 5);
                var day6 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 6);
                var day7 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 7);
                var day8 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 8);
                var day9 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 9);
                var day10 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 10);
                var day11 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 11);
                var day12 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 12);
                var day13 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 13);
                var day14 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 14);
                var day15 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 15);
                var day16 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 16);
                var day17 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 17);
                var day18 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 18);
                var day19 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 19);
                var day20 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 20);
                var day21 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 21);
                var day22 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 22);
                var day23 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 23);
                var day24 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 24);
                var day25 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 25);
                var day26 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 26);
                var day27 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 27);
                var day28 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 28);
                var day29 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 29);
                var day30 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 30);
                var day31 = currentAtt.FirstOrDefault(k => k.AttDate.Day == 31);

                hrmsPersonAttView.D1 = day1 != null ? day1.Overtime + day1.WeekendOvertime + day1.HolidayOvertime : 0;
                hrmsPersonAttView.D2 = day2 != null ? day2.Overtime + day2.WeekendOvertime + day2.HolidayOvertime : 0;
                hrmsPersonAttView.D3 = day3 != null ? day3.Overtime + day3.WeekendOvertime + day3.HolidayOvertime : 0;
                hrmsPersonAttView.D4 = day4 != null ? day4.Overtime + day4.WeekendOvertime + day4.HolidayOvertime : 0;
                hrmsPersonAttView.D5 = day5 != null ? day5.Overtime + day5.WeekendOvertime + day5.HolidayOvertime : 0;
                hrmsPersonAttView.D6 = day6 != null ? day6.Overtime + day6.WeekendOvertime + day6.HolidayOvertime : 0;
                hrmsPersonAttView.D7 = day7 != null ? day7.Overtime + day7.WeekendOvertime + day7.HolidayOvertime : 0;
                hrmsPersonAttView.D8 = day8 != null ? day8.Overtime + day8.WeekendOvertime + day8.HolidayOvertime : 0;
                hrmsPersonAttView.D9 = day9 != null ? day9.Overtime + day9.WeekendOvertime + day9.HolidayOvertime : 0;
                hrmsPersonAttView.D10 = day10 != null
                    ? day10.Overtime + day10.WeekendOvertime + day10.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D11 = day11 != null
                    ? day11.Overtime + day11.WeekendOvertime + day11.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D12 = day12 != null
                    ? day12.Overtime + day12.WeekendOvertime + day12.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D13 = day13 != null
                    ? day13.Overtime + day13.WeekendOvertime + day13.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D14 = day14 != null
                    ? day14.Overtime + day14.WeekendOvertime + day14.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D15 = day15 != null
                    ? day15.Overtime + day15.WeekendOvertime + day15.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D16 = day16 != null
                    ? day16.Overtime + day16.WeekendOvertime + day16.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D17 = day17 != null
                    ? day17.Overtime + day17.WeekendOvertime + day17.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D18 = day18 != null
                    ? day18.Overtime + day18.WeekendOvertime + day18.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D19 = day19 != null
                    ? day19.Overtime + day19.WeekendOvertime + day19.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D20 = day20 != null
                    ? day20.Overtime + day20.WeekendOvertime + day20.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D21 = day21 != null
                    ? day21.Overtime + day21.WeekendOvertime + day21.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D22 = day22 != null
                    ? day22.Overtime + day22.WeekendOvertime + day22.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D23 = day23 != null
                    ? day23.Overtime + day23.WeekendOvertime + day23.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D24 = day24 != null
                    ? day24.Overtime + day24.WeekendOvertime + day24.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D25 = day25 != null
                    ? day25.Overtime + day25.WeekendOvertime + day25.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D26 = day26 != null
                    ? day26.Overtime + day26.WeekendOvertime + day26.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D27 = day27 != null
                    ? day27.Overtime + day27.WeekendOvertime + day27.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D28 = day28 != null
                    ? day28.Overtime + day28.WeekendOvertime + day28.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D29 = day29 != null
                    ? day29.Overtime + day29.WeekendOvertime + day29.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D30 = day30 != null
                    ? day30.Overtime + day30.WeekendOvertime + day30.HolidayOvertime
                    : 0;
                hrmsPersonAttView.D31 = day31 != null
                    ? day31.Overtime + day31.WeekendOvertime + day31.HolidayOvertime
                    : 0;


                //总和
                foreach (var hrmsAttAnalysis in currentAtt)
                {
                    hrmsPersonAttView.TotalOvertime += hrmsAttAnalysis.Overtime;
                    hrmsPersonAttView.TotalWeekendOvertime += hrmsAttAnalysis.WeekendOvertime;
                    hrmsPersonAttView.TotalHolidayOvertime += hrmsAttAnalysis.HolidayOvertime;
                    hrmsPersonAttView.TotalAllOvertime = hrmsPersonAttView.TotalOvertime +
                                                         hrmsPersonAttView.TotalWeekendOvertime +
                                                         hrmsPersonAttView.TotalHolidayOvertime;
                }


                hrmsPersonAttView.EmpNo = m.EmpNo;
                hrmsPersonAttView.Name = m.RealName;

                hrmsPersonAttViews.Add(hrmsPersonAttView);
            });



            return hrmsPersonAttViews.ToDataTable();
        }

        public DataTable OvertimeByArea(string empNo, DateTime startTime, DateTime endTime)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("SELECT ");
            sb.AppendLine("SUM(OVERTIME) OVERTIME,						--平时加班 ");
            sb.AppendLine("SUM(WEEKENDOVERTIME) WEEKENDOVERTIME,		--周末加班 ");
            sb.AppendLine("SUM(HOLIDAYOVERTIME) HOLIDAYOVERTIME ,		--节日加班 ");
            sb.AppendLine("(SUM(OVERTIME)+SUM(WEEKENDOVERTIME)+SUM(HOLIDAYOVERTIME)) SUMTIME, --总加班 ");
            sb.AppendLine($"(SELECT ISNULL(SUM(WORKLEAVE),0) FROM dbo.HrmsAttAnalysis WHERE empno ='{empNo}' AND treatmentResult='{(int)HrmsConfig.AttendanceStatusEnum.事假}' AND  attdate between '{startTime}' and '{endTime}') as CompassionateLeave, --请假 ");
            sb.AppendLine($"(SELECT ISNULL(SUM(WORKLEAVE),0) FROM dbo.HrmsAttAnalysis WHERE empno ='{empNo}' AND treatmentResult='{(int)HrmsConfig.AttendanceStatusEnum.调休}' AND attdate between '{startTime}' and '{endTime}')		as [off]--调休 ");
            sb.AppendLine($"FROM dbo.HrmsAttAnalysis WHERE empno ='{empNo}' and attdate between '{startTime}' and '{endTime}' ");

            return DataFactory.Database().FindTableBySql(sb.ToString());

        }

    }
}
