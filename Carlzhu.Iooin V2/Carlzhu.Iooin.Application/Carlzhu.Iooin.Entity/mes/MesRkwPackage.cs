using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.mes
{
    [PrimaryKey("RowId")]
    public class MesRkwPackage
    {
        [Key]
        public int RowId { get; set; }

        [ForeignKey("MesRkwBox")]
        public string BoxId { get; set; }


        [ForeignKey("MesRkwSn")]
        public string Sn { get; set; }
        
        public DateTime CreateTime { get; set; }

        public virtual MesRkwBox MesRkwBox { get; set; }
        public virtual MesRkwSn MesRkwSn { get; set; }
    }
}
