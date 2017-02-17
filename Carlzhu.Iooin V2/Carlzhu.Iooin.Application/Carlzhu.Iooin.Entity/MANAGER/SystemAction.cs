using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("ActionId")]
    public class SystemAction
    {
        public SystemAction()
        {

            IsCheck = false;
        }

        [Key]
        public int ActionId { get; set; }

        [Required]
        public string ActionName { get; set; }

        public string ActionDetails { get; set; }

        [ForeignKey("SystemController")]
        public int ControllerId { get; set; }

        public bool IsCheck { get; set; }

        public virtual SystemController SystemController { get; set; }

        public virtual ICollection<SystemPower> Permissions { get; set; }
    }
}
