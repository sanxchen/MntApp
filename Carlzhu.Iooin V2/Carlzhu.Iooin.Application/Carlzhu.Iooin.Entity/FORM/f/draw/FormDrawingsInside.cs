using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsInside : DrawingsBase
    {
        public FormDrawingsInside()
        {
          
        }





        [Required]
        [DisplayName("图纸料号")]
        public string DrawPartNo { get; set; }


        //[Required]
        //[DisplayName("产品编号(出货料号)")]
        //public string ProductNo { get; set; }



        [Required]
        [DisplayName("产品名称")]
        public string ProductName { get; set; }

        [Required]
        [DisplayName("制图人")]
        public string Drafter { get; set; }





    }
}
