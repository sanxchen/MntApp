using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormTest : F
    {
        
        [Required(ErrorMessage = "必写的，这个是")]
        public string Name { get; set; }

        [Required(ErrorMessage = "必写的，这个是")]
        [Range(1, 100, ErrorMessage = "你见过你写的这个年吗？")]
        public int Age { get; set; }
    }
}
