using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormCar : F
    {
        [Required]
        [DisplayName("开始时间：")]
        public DateTime StartTime { get; set; }
        [Required]
        [DisplayName("结束时间：")]
        public DateTime EndTime { get; set; }





        [DisplayName("用车类型：")]
        public string Type { get; set; }

        [DisplayName("用车目的：")]
        public string Purpose { get; set; }

        [DisplayName("目的地名称：")]
        [Required]
        [ForeignKey("Customer")]
        public string CustomerNo { get; set; }

        [DisplayName("目的地")]
        public string Addr { get; set; }


        public virtual TpaCustomer Customer { get; set; }
    }
}
