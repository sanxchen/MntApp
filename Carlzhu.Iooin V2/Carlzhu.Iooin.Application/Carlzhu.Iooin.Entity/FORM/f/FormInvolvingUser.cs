using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormInvolvingUser
    {
        [Key]
        public int RowId { get; set; }



        //[ForeignKey("Form")]
        //public string FormNo { get; set; }
        //public virtual Form Form { get; set; }

        public Guid Guid { get; set; }


        [ForeignKey("BaseEmployee")]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }



        public string UpdateEmp { get; set; }

    }
}
