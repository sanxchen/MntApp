using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormJumpTheQueue:F
    {
        public string JumpForm { get; set; }
    }
}
