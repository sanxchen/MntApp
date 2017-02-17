using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormWorkLeave : F
    {
        public FormWorkLeave()
        {
            this.StartTime = DateTime.Now.AddDays(-1);
            this.EndTime = DateTime.Now;
        }


        [ForeignKey("BaseEmployee")]
        [DisplayName("请假人")]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }


        [DisplayName("班别")]
        public int Sheet { get; set; }

        [DisplayName("开始时间")]
        public DateTime StartTime { get; set; }

        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }

        [DisplayName("总计时间")]
        public double SumTime { get; set; }

        [DisplayName("请假类型")]
        public int LeaveType { get; set; }



        public enum LeaveTypEnum
        {
            事假 = 1,
            婚假 = 2,
            产假 = 3,
            病假 = 4,
            丧假 = 5,
            补休 = 6,
            调休 = 7,
            工伤 = 8,
            带薪假 = 9,

        }
        public enum SheetEnum
        {
            白班 = 0,
            晚班 = 1
        }


    }



}
