using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Carlzhu.Iooin.Entity.FORM.f.FormWorkLeave;

namespace Carlzhu.Iooin.Entity.HRMS
{
    public struct HrmsConfig
    {
        /// <summary>
        /// 员工工号长度
        /// </summary>
        public const int NoLength = 7;


        /// <summary>
        /// 系统时间缺少时默认时间
        /// </summary>
        public static DateTime DefaultDateTime = DateTime.Parse("2000/01/01 00:00:00");


        /// <summary>
        /// 考勤月结日期
        /// </summary>
        public const int DateOfAttendance = 25;

        public enum AttendanceStatusEnum
        {
            综合 = 1000,
            异常 = 999,
            正常 = 0,
            事假 = 1,
            婚假 = 2,
            产假 = 3,
            病假 = 4,
            丧假 = 5,
            补休 = 6,
            调休 = 7,
            工伤 = 8,
            带薪假 = 9,
            旷工 = 10,
            迟到 = 11,
            早退 = 12,
            迟到早退 = 13,

            上班未打卡 = 14,
            下班未打卡 = 15,

            周末 = 16,
            国假 = 17,
            休息上班未打卡 = 18,
            休息下班未打卡 = 19,




        }

    }

}
