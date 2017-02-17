using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.HRMS
{
    /// <summary>
    /// 考勤
    /// </summary>
    /// 
    [PrimaryKey("RowId")]
    public class HrmsAttendance : BaseEntity
    {
        [Key]
        public int RowId { get; set; }

        public int ClockId { get; set; }

        public string EmpNo { get; set; }


        /// <summary>
        /// 磁卡号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 刷卡时间
        /// </summary>
        public DateTime SignTime { get; set; }


        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 标识 0：正常,1:假日
        /// </summary>
        public int Flag { get; set; }

        /// <summary>
        /// 手动补单号
        /// </summary>
        public string BillNo { get; set; }


        /// <summary>
        /// 收集时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 事件名
        /// </summary>
        public string EventName { get; set; }


        /// <summary>
        /// 时间差,用于计算考勤
        /// </summary>
        public double TimeDifference { get; set; }

    }
}
