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

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("RowId")]
    public class FormProxy
    {
        public FormProxy()
        {
            this.StarTime = DateTime.Now;
            this.EndTime = DateTime.Now.AddDays(7);
        }

        [Key]
        [DisplayName("序号")]
        public int RowId { get; set; }


        [DisplayName("代理发起人工号")]
        [Required]
        public string SourceEmpNo { get; set; }


        [ForeignKey("BaseEmployee")]
        [DisplayName("代理人")]
        [Required]
        public string EmpNo { get; set; }

        [ForeignKey("FormType")]
        [DisplayName("表单ID")]
        [Required]
        public int FormId { get; set; }

        [Required]
        [DisplayName("开始时间")]
        public DateTime StarTime { get; set; }

        [Required]
        [DisplayName("结束时间")]
        public DateTime EndTime { get; set; }


        public virtual BaseEmployee BaseEmployee { get; set; }

        public virtual FormType FormType { get; set; }
    }
}
