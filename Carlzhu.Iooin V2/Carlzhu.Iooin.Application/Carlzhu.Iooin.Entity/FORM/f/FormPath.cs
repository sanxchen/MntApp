using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormPath : F
    {
        public FormPath()
        {
            this.FormId = 1;
        }


        [DisplayName("操作单号")]
        [Required]
        public string ParentFormNo { get; set; }

        [DisplayName("对应表单")]
        [ForeignKey("FormType")]
        public int FormId { get; set; }
        public virtual FormType FormType { get; set; }

    }
}
