using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class Involving : F
    {
        [Required]
        [DisplayName("涉及用户")]
        public Guid InvolvingUser { get; set; }
    }
}
