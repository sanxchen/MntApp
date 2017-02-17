using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{

    [PrimaryKey("RowId")]
    public class FormNcrPart : F
    {



        public string ParentFormNo { get; set; }

        [ForeignKey("BaseEmployee")]
        public string Reviewer { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        [ForeignKey("FormType")]
        public int ReplyType { get; set; }
        public virtual FormType FormType { get; set; }


        public DateTime ReviewTime { get; set; }

        public string AuditEmp { get; set; }
        public DateTime? AuditTime { get; set; }

        [Required]
        public new string Mark { get; set; }
        


    }
}
