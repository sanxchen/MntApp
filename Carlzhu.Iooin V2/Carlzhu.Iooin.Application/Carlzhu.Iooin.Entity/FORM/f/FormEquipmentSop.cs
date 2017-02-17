using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormEquipmentSop : F
    {
      
        [DisplayName("设备名")]
        public string EquipmentName { get; set; }

        [DisplayName("总页数")]
        public int PageCount { get; set; }

        [DisplayName("版本")]
        public string Ver { get; set; }

        [Required]
        [DisplayName("文件组")]
        [ForeignKey("GroupGuid")]
        public Guid FileGroup { get; set; }
        public virtual FileGroup GroupGuid { get; set; }

        [DisplayName("作者")]
        [ForeignKey("BaseEmployee")]
        [RegularExpression(@"\w{7}", ErrorMessage = "必须是7位有效工号！")]
        public string Author { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

    }
}
