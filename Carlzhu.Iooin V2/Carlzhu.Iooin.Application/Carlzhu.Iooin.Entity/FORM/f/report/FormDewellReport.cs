using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f.report
{

    [PrimaryKey("RowId")]
    public  class FormDewellReport : F
    {
        [ForeignKey("BaseEmployee")]
        [FormHeader]
        public string EmpNo { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }


        [DisplayName("班组")]
        [FormHeader]
        public int Team { get; set; }


        [DisplayName("班次")]
        [FormHeader]
        public int Shift { get; set; }



        [DisplayName("产品料号")]
        public string ProductNo { get; set; }

        [DisplayName("订单编号")]
        public string OrderNo { get; set; }

        [DisplayName("工单编号")]
        public string ManNo { get; set; }

        [DisplayName("标准工时(单件)")]
        public double StandardWorkingHours { get; set; }

        [DisplayName("不良数量")]
        public  double BadQuantity { get; set; }

        [DisplayName("合格数量")]
        public double GoodQuality { get; set; }

        [DisplayName("单价")]
        public double UnitPrice { get; set; }

        [DisplayName("不良原因")]
        public string BadReason { get; set; }

        [DisplayName("有效工时")]
        public double BaEffectiveWorkingHours { get; set; }


        [DisplayName("出勤工时")]
        public double AttendanceHours { get; set; }
    }
}
