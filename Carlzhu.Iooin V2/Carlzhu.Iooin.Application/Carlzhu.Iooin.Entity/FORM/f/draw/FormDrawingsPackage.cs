using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsPackage : DrawingsBase
    {
        public FormDrawingsPackage()
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
        [DisplayName("编制人")]
        public string Author { get; set; }


        
    }
}
