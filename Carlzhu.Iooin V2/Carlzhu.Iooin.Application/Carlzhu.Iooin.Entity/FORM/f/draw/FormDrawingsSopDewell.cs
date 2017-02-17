using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{

    [PrimaryKey("RowId")]
    public class FormDrawingsSopDewell : DrawingsBase
    {
        public FormDrawingsSopDewell()
        {
         
            PageSize = 1;
            DrawVer = "v1";
        }




        [Required]
        [DisplayName("图纸料号")]
        public string DrawPartNo { get; set; }




        [TableShow(1)]
        [Required]
        [DisplayName("工位号")]
        public string Tag { get; set; }

        [Required]
        [DisplayName("编制人")]
        public string Author { get; set; }


        //添加删除标记

        public bool IsDel { get; set; }

    }


}
