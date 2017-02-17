using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.TPA
{
    [PrimaryKey("CustomerNo")]
    public class TpaCustomer:BaseEntity
    {
        [Key]
        [DisplayName("客户编号")]
        public string CustomerNo { get; set; }
        [DisplayName("客户名称")]
        public string CustomerName { get; set; }
        [DisplayName("客户地址")]
        public string CustomerAddr { get; set; }

        [DisplayName("业务负责人")]
        [MinLength(HrmsConfig.NoLength), MaxLength(HrmsConfig.NoLength)]
        public string EmpSales { get; set; }

        [DisplayName("品质负责人")]
        [MinLength(HrmsConfig.NoLength), MaxLength(HrmsConfig.NoLength)]
        public string EmpQuality { get; set; }


        [DisplayName("工程负责人")]
        [MinLength(HrmsConfig.NoLength), MaxLength(HrmsConfig.NoLength)]
        public string EmpManager { get; set; }



    }
}
