using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;

namespace Carlzhu.Iooin.Entity.HRMS
{
    [PrimaryKey("RowId")]
    public class DoorInout
    {
        [Key]
        public int RowId { get; set; }


        [MaxLength(10)]
        public string CardNo { get; set; }

        public int Forward { get; set; }

        public DateTime EventTime { get; set; }


        [ForeignKey("BaseEmployee")]
        public string Record { get; set; }

        public virtual BaseEmployee BaseEmployee { get; set; }


        public enum ForwardEnum
        {
            In = 0,
            Out = 1
        }
    }

    public class DoorInoutReport
    {
        [DisplayName("姓名")]
        public string Name { get; set; }

        [DisplayName("部门")]
        public string Department { get; set; }


        [DisplayName("员工卡号")]
        public string CardNo { get; set; }



        [DisplayName("外出时间")]
        public string OutTime { get; set; }
        [DisplayName("进来时间")]
        public string InTime { get; set; }

        [DisplayName("外出停留时间")]

        public string SumTime { get; set; }
    }
}
