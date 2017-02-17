using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity
{
    [PrimaryKey("RowId")]
    public class FileReplaceRecords
    {
        [Key]
        public int RowId { get; set; }

        public string FormNo { get; set; }

        public Guid OldFileGroupGuid { get; set; }

        public string OldMd5 { get; set; }

        public Guid NewFileGroupGuid { get; set; }


        [ForeignKey("Files")]
        public string NewMd5 { get; set; }

        public DateTime CreateDate { get; set; }


        [ForeignKey("BaseEmployee")]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }


        public virtual Files Files { get; set; }


    }
}
