﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("EmpNo")]
    public class SystemPower
    {


        public SystemPower()
        {
            this.CreateTime = DateTime.Now;
        }

        [Key]
        public int RowId { get; set; }

        [ForeignKey("BaseEmployee")]
        [Required]
        public string EmpNo { get; set; }


        [ForeignKey("SystemAction")]
        [Required]
        public int ActionId { get; set; }


        public DateTime CreateTime { get; set; }


        [Required]
        public string CreateEmpNo { get; set; }


        public string CreateReason { get; set; }


        public virtual SystemAction SystemAction { get; set; }

        public virtual BaseEmployee BaseEmployee { get; set; }
    }
}
