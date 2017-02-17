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
    public class FormSign
    {
        [Key]
        public int RowId { get; set; }

        [ForeignKey("Form")]
        [DisplayName("表单号")]
        public string FormNo { get; set; }

        [Required]
        [ForeignKey("BaseEmployee")]
        [DisplayName("签核人")]
        public string SignEmpNo { get; set; }

        [DisplayName("实际签核人")]
        public string ActualSignEmpNo { get; set; }

        [Required]
        [DisplayName("签核来源")]
        public int SourceType { get; set; }


        [DisplayName("来源工号")]
        public string SourceEmpNo { get; set; }

        [DisplayName("来源理由")]
        public string SourceReason { get; set; }

        [Required]
        [DisplayName("签核结果")]
        public int SignResult { get; set; }


        [DisplayName("签核意见")]
        public string SignMark { get; set; }

        [Required]
        [DisplayName("到达本站时间")]
        public DateTime CreateTime { get; set; }

        [DisplayName("签核时间")]
        public DateTime? SignTime { get; set; }

        [Required]
        [DisplayName("等级")]
        public int Grade { get; set; }

        [Required]
        [DisplayName("标记")]
        public int Tag { get; set; }

        [Required]
        [DisplayName("软删除")]
        public bool IsDel { get; set; }


        [DisplayName("文件组")]
        [ForeignKey("FileGroup")]
        public Guid AuditingFileGroup { get; set; }
        public virtual FileGroup FileGroup { get; set; }

        public virtual BaseEmployee BaseEmployee { get; set; }
        public virtual Form Form { get; set; }




        public enum SourceTypeEnum
        {
            Auto = 0,
            Redirect = 1,
            AddBefore = 2,
            AddParallel = 3,
            AddAfter = 4,
            Replace = 5,
        }

        public enum DirectEnum
        {
            Befor = -1,
            Parallel = 0,
            After = 1
        }

        public enum SignResultEnum
        {
            Watting = 0,
            Agree = 1,
            Reject = 2,
            Cancel = 3,
            Add = 4,
            Rdirect = 5
        }
    }
}
