using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("ModelId")]
    public class SystemModel
    {
        public SystemModel()
        {
            this.Status = true;
        }

        [Key]
        public int ModelId { get; set; }

        public string ModelName { get; set; }

        public string ModelUrl { get; set; }

        public bool Status { get; set; }

        public virtual ICollection<SystemController> SystemControllers { get; set; }
    }
}
