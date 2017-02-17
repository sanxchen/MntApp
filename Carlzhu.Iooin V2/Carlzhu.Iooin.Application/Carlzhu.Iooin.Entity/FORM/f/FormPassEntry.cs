using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormPassEntry : F
    {
        public FormPassEntry()
        {

        }

        [DisplayName("采购负责人")]
        [FormHeader]
        public string PurchasePrincipal { get; set; }


        [DisplayName("采购主管")]
        [FormHeader]
        public string PurchaseMaster { get; set; }


        [DisplayName("详细内容")]
        public string DetailedContent { get; set; }


        [DisplayName("客户名称")]
        [ForeignKey("TpaCustomer")]
        public string CustomerName { get; set; }
        public virtual TpaCustomer TpaCustomer { get; set; }



        [DisplayName("使用部门")]
        [ForeignKey("BaseDepartment")]
        public string UseDepartment { get; set; }
        public virtual BaseDepartment BaseDepartment { get; set; }


        [DisplayName("供应商名称")]
        [ForeignKey("TpaSupplier")]
        public string SupplierCode { get; set; }
        public virtual TpaSupplier TpaSupplier { get; set; }



        [Required]
        [DisplayName("金额")]
        public decimal AmountOfMoney { get; set; }

        [DisplayName("发票号")]
        public string InvoiceNo { get; set; }


        [DisplayName("付款期")]
        public DateTime TimeOfPayment { get; set; }


    }

}
