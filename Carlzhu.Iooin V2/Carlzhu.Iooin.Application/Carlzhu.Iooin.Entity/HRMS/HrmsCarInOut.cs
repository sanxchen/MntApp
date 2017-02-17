using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;

namespace Carlzhu.Iooin.Entity.HRMS
{
    [PrimaryKey("FormNo")]
    public class HrmsCarInOut
    {
        [Key]
        [ForeignKey("Form")]
        public string FormNo { get; set; }

        public virtual Form Form { get; set; }



        [ForeignKey("HrmsCar")]
        public string CarNo { get; set; }
        public virtual HrmsCar HrmsCar { get; set; }


        [ForeignKey("BaseEmployee")]
        public string DirverNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        public DateTime OutTime { get; set; }

        public int OutKilometers { get; set; }

        public DateTime InTime { get; set; }
        public int InKilometers { get; set; }
        public double Oil { get; set; }

    }
}
