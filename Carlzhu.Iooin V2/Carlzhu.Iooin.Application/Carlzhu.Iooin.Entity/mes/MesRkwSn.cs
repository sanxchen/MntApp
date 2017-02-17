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
    [PrimaryKey("Sn")]
    public class MesRkwSn
    {
        [Key]
        public string Sn { get; set; }

        [ForeignKey("MesRkwPo")]
        public string ManNo { get; set; }
        public virtual MesRkwPo MesRkwPo { get; set; }


        public int Reprint { get; set; }


        public virtual List<MesRkwPackage> mesRkwPackages { get; set; }

        public virtual ICollection<MesRkwBox> MesRkwBoxs { get; set; }
    }
}
