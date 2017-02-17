
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormBll : F
    {

        [DisplayName("供应商名称")]
        public string SupplierCode { get; set; }
        public virtual TpaSupplier Supplier { get; set; }

        [Required]
        [DisplayName("发票号")]
        public string InvoiceNo { get; set; }



        [Required]
        [DisplayName("金额")]
        public decimal AmountOfMoney { get; set; }

        
    }
}
