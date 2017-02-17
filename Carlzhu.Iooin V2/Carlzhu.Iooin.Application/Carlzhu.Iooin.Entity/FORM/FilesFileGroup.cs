using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM
{

    [PrimaryKey("RowId")]
    public class FilesFileGroup
    {
        [Key]
        public int RowId { get; set; }

        [ForeignKey("Files")]
        public string Md5 { get; set; }

        [ForeignKey("FileGroup")]
        public Guid GroupGuid { get; set; }




        public virtual Files Files { get; set; }

        public virtual FileGroup FileGroup { get; set; }
    }
}
