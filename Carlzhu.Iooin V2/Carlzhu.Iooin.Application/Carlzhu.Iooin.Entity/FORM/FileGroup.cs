using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("GroupGuid")]
    public class FileGroup
    {
        [Key]
        public Guid GroupGuid { get; set; }
        public DateTime CreateTime { get; set; }

        [ForeignKey("Employee")]
        public string CreateEmpNo { get; set; }

        public virtual BaseEmployee Employee { get; set; }

        public virtual ICollection<Files> Fileses { get; set; }

    }
}
