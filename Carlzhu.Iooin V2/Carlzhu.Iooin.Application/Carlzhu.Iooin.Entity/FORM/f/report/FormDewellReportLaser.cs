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
    /// 激光
    /// </summary>
    [PrimaryKey("RowId")]
    public class FormDewellReportLaser : FormDewellReport
    {
        [DisplayName("停机原因")]
        public string ShutdownReason { get; set; }


        [DisplayName("停机时间")]
        public int DownTime { get; set; }


        [DisplayName("运转速度")]
        public double RunningSpeed { get; set; }


        [DisplayName("套牌号（排版图）")]
        public string DeckNo { get; set; }
    }
}
