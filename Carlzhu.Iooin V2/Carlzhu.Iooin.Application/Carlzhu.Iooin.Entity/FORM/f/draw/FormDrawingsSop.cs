using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsSop : DrawingsBase
    {
        public FormDrawingsSop()
        {

        }



        [Required]
        [DisplayName("图纸料号")]
        public string DrawPartNo { get; set; }





        [Required]
        [DisplayName("工位号")]
        public string Tag { get; set; }

        [Required]
        [DisplayName("编制人")]
        public string Author { get; set; }
    }
}
