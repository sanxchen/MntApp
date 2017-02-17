using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;

namespace Carlzhu.Iooin.Entity.HRMS
{
    [PrimaryKey("No")]
    public class HrmsCar
    {
        [Key]
        [DisplayName("车号")]
        [MinLength(10), MaxLength(10)]
        public string No { get; set; }


        [Required]
        [DisplayName("车牌")]
        public string Nameplate { get; set; }


        [Required]
        [DisplayName("车名")]
        public string Name { get; set; }


        [DisplayName("保养周期/KM")]
        public int MaintenanceCycle { get; set; }



        [DisplayName("上次保养/KM")]
        public int LastMaintenance { get; set; }

        [Required]
        [DisplayName("所有者")]
        [ForeignKey("BaseEmployee")]
        public string Owner { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }


        [DisplayName("车辆状态")]
        public int Status { get; set; }

        [DisplayName("车辆位置")]
        public int Location { get; set; }

        [DisplayName("坐标")]
        public string Xy { get; set; }

        [DisplayName("当前公里数")]
        public int CurrentKilometers { get; set; }


        [DisplayName("当前处理单号")]
        [ForeignKey("Form")]
        public string FormNo { get; set; }

        public virtual Form Form { get; set; }
    }
}
