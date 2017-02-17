using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    [PrimaryKey("RowId")]
    public  class FormDrawingsFmea:DrawingsBase
    {

      [Required]
      [DisplayName("编制人")]
      public string Author { get; set; }
    }
}
