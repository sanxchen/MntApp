using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{

    [PrimaryKey("RowId")]
    public class FormDrawingsSupplierSampleAck : DrawingsBase
    {
        [DisplayName("产品名称")]
        [Required]
        public string ProductName { get; set; }

        [DisplayName("规格")]
        [Required]
        public string Format { get; set; }

        [DisplayName("材质")]
        [Required]
        public string Material { get; set; }

        [DisplayName("日期")]
        [Required]
        public DateTime DateTime { get; set; }


        [DisplayName("供应商")]
        [ForeignKey("Supplier")]
        [Required]
        public string SupplierNo { get; set; }

        public virtual TpaSupplier Supplier { get; set; }
    }
}
