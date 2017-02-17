using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public class FormDrawingsExternal : DrawingsBase
    {
        public FormDrawingsExternal()
        {
  
        }




        [Required]
        [DisplayName("文件名称")]
        public string FileName { get; set; }

        [Required]
        [DisplayName("文件编号")]
        public string FileCode { get; set; }


        [Required]
        [DisplayName("明捷编号")]
        public string MinicutCode { get; set; }


        [Required]
        [DisplayName("接收人")]
        public string ReciveEmpNo { get; set; }

        
    }
}
