using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsProgram : DrawingsBase
    {
        [Required]
        [DisplayName("文件类型")]
        public string FileType { get; set; }

        [Required]
        [DisplayName("文件名称")]
        public string FileName { get; set; }

        [Required]
        [DisplayName("文件编号")]
        public string FileCode { get; set; }


        [Required]
        [DisplayName("编制人")]
        public string Author { get; set; }
        
    }
}
