using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("FormNo")]
    public class Form
    {

        public Form()
        {

        }


        [Key]
        [Required]
        [DisplayName("表单号")]
        public string FormNo { get; set; }

        [Required]
        [ForeignKey("FormType")]
        [DisplayName("表单ID")]
        public int FormId { get; set; }

        [Required]
        [ForeignKey("BaseEmployee")]
        [DisplayName("表单创建者")]
        public string CreateEmpNo { get; set; }

        [Required]
        [DisplayName("申请时间")]
        public DateTime CreateTime { get; set; }

        [DisplayName("签核顺序")]
        public string SignPath { get; set; }


        public string SourceEmpNo { get; set; }

        [DisplayName("加签理由")]
        public string Reason { get; set; }

        [Required]
        [DisplayName("表单状态")]
        public int FormStatus { get; set; }

        [Required]
        [DisplayName("加急")]
        public bool IsEmergents { get; set; }

        [DisplayName("关闭时间")]
        public DateTime? CloseTime { get; set; }

        //[DisplayName("说明")]
        //public string Explain { get; set; }

        public bool IsDel { get; set; }



        public virtual FormType FormType { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        public virtual ICollection<FormSign> FormSigns { get; set; }


        public enum StatusEnum
        {
            未送出 = 0,//刚建立的
            签核中 = 1,//签核中表单
            已否决 = 2,//否决的表单
            签核完成 = 3,//已完成表单
            已撤消 = 4,//已撤消表单
        }
    }
}
