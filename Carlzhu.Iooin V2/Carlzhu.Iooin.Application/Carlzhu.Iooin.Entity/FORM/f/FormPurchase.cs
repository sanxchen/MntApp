using System.ComponentModel;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormPurchase : F
    {
        [DisplayName("ERP品名")]
        public string PartNo { get; set; }

        [DisplayName("名称")]
        public string Name { get; set; }

        [DisplayName("规格")]
        public string Details { get; set; }

        [DisplayName("单位")]
        public string Unit { get; set; }

        [DisplayName("数量")]
        public int Qty { get; set; }

        [DisplayName("交期")]
        public string Delivery { get; set; }

        [DisplayName("用途")]
        public string Usage { get; set; }

        [DisplayName("备注")]
        public string Reason { get; set; }

    }
}
