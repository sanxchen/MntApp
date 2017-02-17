using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f.report
{
    /// <summary>
    /// 装配
    /// </summary>
    /// 
    [PrimaryKey("RowId")]
    public  class FormDewellReportAssembling : FormDewellReport
    {

        [DisplayName("工单用量")]
        public double ManConsumption { get; set; }

        [DisplayName("实际用量")]
        public double ManFactConsumption { get; set; }


        [DisplayName("差异常说明")]
        public string DifferenceDescription { get; set; }
    }
}
