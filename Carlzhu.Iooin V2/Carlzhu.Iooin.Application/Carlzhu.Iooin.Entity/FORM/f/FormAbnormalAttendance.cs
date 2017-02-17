using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;


using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Util.MvcHtml;



namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormAbnormalAttendance : F
    {
        public FormAbnormalAttendance()
        {
            this.TimeStart = HrmsConfig.DefaultDateTime;
            this.TimeEnd = HrmsConfig.DefaultDateTime;
        }

        [Required]
        [DisplayName("异常人工号")]
        [ForeignKey("BaseEmployee")]
        [RegularExpression("\\d{7}", ErrorMessage = "只能为7位工号")]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        [Required]
        [DisplayName("异常开始时间")]
        public DateTime TimeStart { get; set; }

        [Required]
        [DisplayName("异常结束时间")]
        public DateTime TimeEnd { get; set; }

        [Required]
        [DisplayName("异常类型")]
        public int AbnormalType { get; set; }


        [DisplayName("临时卡号")]
        [RegularExpression("\\d{10}", ErrorMessage = "只能为10位数字")]
        public string TempCard { get; set; }


        public enum TypeEnum
        {
            [EnumShowName("忘打卡")]
            ForgetToClockOut = 0,

            [EnumShowName("忘带卡")]
            ForgetTheCard = 1,

            [EnumShowName("卡丢失")]
            TheCardisLost = 2,

            [EnumShowName("因公外出")]
            OnaBusinessTrip = 3,

            [EnumShowName("超时加班")]
            OvertimeHours = 4,

            [EnumShowName("其他异常")]
            OtherAbnormalities = 5,
        }






    }
}
