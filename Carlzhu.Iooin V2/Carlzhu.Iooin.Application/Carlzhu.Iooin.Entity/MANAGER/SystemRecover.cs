using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("RowId")]
    public class SystemRecover
    {
        public SystemRecover()
        {
            this.LinkEnabled = DateTime.Now;
            this.LinkDisabled = DateTime.Now.AddDays(1);
            this.IsDel = false;
        }

        [Key]
        public int RowId { get; set; }

        [DisplayName("邮箱")]
        [RegularExpression(@"^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$", ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        public DateTime LinkEnabled { get; set; }
        public DateTime LinkDisabled { get; set; }

        public Guid RecoverGuid { get; set; }

        public bool IsDel { get; set; }
    }
}
