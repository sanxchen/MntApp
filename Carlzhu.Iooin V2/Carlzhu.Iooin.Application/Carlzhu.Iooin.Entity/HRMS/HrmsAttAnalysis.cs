using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;

namespace Carlzhu.Iooin.Entity.HRMS
{
    public class HrmsAttAnalysis
    {
        [Key]
        public int RowId { get; set; }



        [ForeignKey("BaseEmployee")]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }
        public DateTime AttDate { get; set; }

        [DisplayName("数据刷新时间")]
        public DateTime AnalysisTime { get; set; }


        [ForeignKey("HrmsShift")]
        public string Shift { get; set; }
        public virtual HrmsShift HrmsShift { get; set; }
        [DisplayName("上班")]
        public DateTime OnDutty { get; set; }

        [DisplayName("下班")]
        public DateTime OffDutty { get; set; }


        [DisplayName("工时")]
        public double WorkingHours { get; set; }


        [DisplayName("事由")]
        public int Reason { get; set; }


        [ForeignKey("Form")]
        [DisplayName("单号")]
        public string FormNo { get; set; }
        public virtual Form Form { get; set; }

        public double Late { get; set; }

        public double LeaveEarly { get; set; }


        [DisplayName("平时加班")]
        public double Overtime { get; set; }

        [DisplayName("周末加班")]
        public double WeekendOvertime { get; set; }

        [DisplayName("节日加班")]
        public double HolidayOvertime { get; set; }

        [DisplayName("请假时数")]
        public double WorkLeave { get; set; }

        [DisplayName("处理结果")]
        public int TreatmentResult { get; set; }
    }
}
