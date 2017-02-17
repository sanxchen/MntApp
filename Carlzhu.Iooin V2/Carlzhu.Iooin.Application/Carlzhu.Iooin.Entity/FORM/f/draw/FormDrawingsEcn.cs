using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsEcn : DrawingsBase
    {
        public FormDrawingsEcn()
        {
            //this.ProductNo = base.ProductNo;
        }

        //[Required]
        //[DisplayName("产品编号(出货料号)")]
        //public string ProductNo { get; set; }



        [Required]
        [DisplayName("产品名称")]
        public string ProductName { get; set; }


        [Required]
        [DisplayName("ECN编号")]
        public string EcnNo { get; set; }



        [Required]
        [DisplayName("发行人")]
        public string PublishEmp { get; set; }
    }
}
