using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.mes
{
    [PrimaryKey("BoxId")]
    public class MesRkwBox
    {

        [Key]
        public string BoxId { get; set; }

        public DateTime CreateTime { get; set; }

        public bool BoxStatus { get; set; }


        public virtual ICollection<MesRkwSn> MesRkwSns { get; set; }
    }
}
