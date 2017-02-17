using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.TPA
{
    [PrimaryKey("Code")]
    public class TpaSupplier
    {
        [Key]
        [DisplayName("供应商编号")]
        public string Code { get; set; }
        [DisplayName("供应商名称")]
        public string Name { get; set; }
        [DisplayName("供应商地址")]
        public string Addr { get; set; }

        public virtual ICollection<TpaGoods> TpaGoodses { get; set; }
    }
}
