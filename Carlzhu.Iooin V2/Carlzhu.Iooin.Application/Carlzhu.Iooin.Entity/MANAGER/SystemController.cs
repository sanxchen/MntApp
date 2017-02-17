using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("ControllerId")]
    public class SystemController
    {
        public SystemController()
        {
            this.CreateTime = DateTime.Now;
            IsDisplay = true;
        }
        [Key]
        public int ControllerId { get; set; }


        [ForeignKey("SystemModel")]
        public int ModelId { get; set; }

        [MaxLength(50)]
        [RegularExpression(@"[a-zA-Z]*")]
        public string ControllerName { get; set; }

        public string ControllerDetails { get; set; }

        public DateTime CreateTime { get; set; }


        public bool IsDisplay { get; set; }


        public virtual SystemModel SystemModel { get; set; }

        public virtual ICollection<SystemAction> SystemActions { get; set; }
    }
}
