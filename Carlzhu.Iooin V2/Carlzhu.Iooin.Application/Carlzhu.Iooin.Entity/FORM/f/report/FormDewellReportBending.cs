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
    /// 折弯
    /// </summary>
    [PrimaryKey("RowId")]
    public class FormDewellReportBending : FormDewellReport
    {

        [DisplayName("停机原因")]
        public string ShutdownReason { get; set; }


        [DisplayName("停机时间")]
        public int DownTime { get; set; }


        [DisplayName("折弯刀数")]
        public int BendingKnifeNumber { get; set; }

        [DisplayName("换模次数")]
        public int DieChangingTimes { get; set; }

        [DisplayName("产品规格")]
        public string ProductSpecifications { get; set; }
    }
}
