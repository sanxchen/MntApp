using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.TPA
{
    /// <summary>
    /// 物品表
    /// </summary>
    /// 
    [PrimaryKey("Code")]
    public class TpaGoods
    {

        //public TpaGoods()
        //{
        //    this.CreateTime = DateTime.Now;
        //    this.Unit = "5";
        //    this.Warning = 5;

        //}

        [Key]
        [Required]
        [DisplayName("商品代码")]
        public string Code { get; set; }

        //[Required]
        //[DisplayName("名称")]
        //public string Name { get; set; }

        //[Required]
        //[DisplayName("规格")]
        //public string Format { get; set; }


        //[DisplayName("描述")]
        //public string Describe { get; set; }


        //[Required]
        //[DisplayName("类别")]
        //[ForeignKey("TpaGoodType")]
        //public string Type { get; set; }
        //public virtual TpaGoodType TpaGoodType { get; set; }



        //[Required]
        //[DisplayName("单位")]
        //public string Unit { get; set; }


        //[DisplayName("单价")]
        //public decimal Price { get; set; }


        //[DisplayName("供应商")]
        //[ForeignKey("Supplier")]
        //[Required]
        //public string SupplierCode { get; set; }
        //public virtual TpaSupplier Supplier { get; set; }

        //[DisplayName("预警数量")]
        //[Required]
        //[Range(0, 100, ErrorMessage = "必须界于0-100")]
        //public int Warning { get; set; }


        //[DisplayName("创建人")]
        //[ForeignKey("BaseEmployee")]
        //[Required]
        //public string Emp { get; set; }
        //public virtual BaseEmployee BaseEmployee { get; set; }


        //[DisplayName("创建时间")]
        //public DateTime CreateTime { get; set; }

        //public virtual ICollection<TpaGoodWarehouse> TpaGoodWarehouses { get; set; }
        //public virtual ICollection<TpaUse> TpaUses { get; set; }
    }
}
