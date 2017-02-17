using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carlzhu.Iooin.Entity.HRMS
{
    public class HrmsPersonAttView
    {
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string Calendar { get; set; }
        public double D1 { get; set; }
        public double D2 { get; set; }
        public double D3 { get; set; }
        public double D4 { get; set; }
        public double D5 { get; set; }
        public double D6 { get; set; }
        public double D7 { get; set; }
        public double D8 { get; set; }
        public double D9 { get; set; }
        public double D10 { get; set; }
        public double D11 { get; set; }
        public double D12 { get; set; }
        public double D13 { get; set; }
        public double D14 { get; set; }
        public double D15 { get; set; }
        public double D16 { get; set; }
        public double D17 { get; set; }
        public double D18 { get; set; }
        public double D19 { get; set; }
        public double D20 { get; set; }
        public double D21 { get; set; }
        public double D22 { get; set; }
        public double D23 { get; set; }
        public double D24 { get; set; }
        public double D25 { get; set; }
        public double D26 { get; set; }
        public double D27 { get; set; }
        public double D28 { get; set; }
        public double D29 { get; set; }
        public double D30 { get; set; }
        public double D31 { get; set; }




        /// <summary>
        /// 平时加班总数
        /// </summary>
        public double TotalOvertime { get; set; }

        /// <summary>
        /// 周末加班总数
        /// </summary>
        public double TotalWeekendOvertime { get; set; }

        /// <summary>
        /// 节日加班总数
        /// </summary>
        public double TotalHolidayOvertime { get; set; }

        /// <summary>
        /// 加班总时数
        /// </summary>
        public double TotalAllOvertime { get; set; }

        /// <summary>
        /// 请假时数
        /// </summary>
        public double WorkLeaverHour { get; set; }
    }
}
