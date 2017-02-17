using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormPdAbnor : F
    {
        public FormPdAbnor()
        {

            AbnormalTime = DateTime.Now;
            PlanFinishTime = Convert.ToDateTime("3000/1/1");
            FinishTime = Convert.ToDateTime("3000/1/1");


        }




        [DisplayName("异常类型")]
        [Required]
        public int AbnormalType { get; set; }


        [DisplayName("发生地点")]
        [Required]
        //[ForeignKey("Workshop")]
        public string WorkshopCode { get; set; }
        //public virtual Workshop Workshop { get; set; }


        [DisplayName("工位")]
        [Required]
        public string WorkTag { get; set; }



        [DisplayName("工序")]
        [Required]
        public string WorkProcess { get; set; }

        [DisplayName("料号")]
        [Required]
        public string PartNo { get; set; }




        [DisplayName("发生时间")]
        [Required]
        public DateTime AbnormalTime { get; set; }


        [DisplayName("异常描述")]
        [Required]
        public string AbnormalDetails { get; set; }

        [DisplayName("责任人")]
        [ForeignKey("BaseEmployee")]
        public string ResponsibilityPeople { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        [DisplayName("责任单位")]
        [ForeignKey("BaseDepartment")]
        public string ResponsibilityDept { get; set; }

        public virtual BaseDepartment BaseDepartment { get; set; }






        [DisplayName("处理方式")]
        public string DealingMethod { get; set; }


        [DisplayName("计划完成时间")]
        public DateTime PlanFinishTime { get; set; }


        [DisplayName("完成时间")]
        public DateTime FinishTime { get; set; }


        /// <summary>
        /// 是否结案
        /// </summary>
        public bool IsClose { get; set; }


        /// <summary>
        /// 异常类型
        /// </summary>
        public enum EnumAbnormalType
        {
            设备异常 = 0,
            程序异常 = 1,
            工装异常 = 2,
            刀具异常 = 3,
            模具异常 = 4,
            缺料异常 = 5,
            发料异常 = 6,
            来料品质异常 = 7,
            工艺规范异常 = 8,
            产品检验异常 = 9,
            制程品质异常 = 10

        }
    }
}
