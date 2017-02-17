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
    /// 钳工
    /// </summary>
    /// 
    [PrimaryKey("RowId")]
    public class FormDewellReportFitter : FormDewellReport
    {

        [DisplayName("工单用量")]
        public double ManConsumption { get; set; }

        [DisplayName("实际用量")]
        public double ManFactConsumption { get; set; }

        [DisplayName("停机原因")]
        public string ShutdownReason { get; set; }


        [DisplayName("停机时间")]
        public int DownTime { get; set; }


        [DisplayName("差异常说明")]
        public string DifferenceDescription { get; set; }

        [DisplayName("产品规格")]
        public string ProductSpecifications { get; set; }

        [DisplayName("品名型号")]
        public  string ProductNameType { get; set; }
    }
}
