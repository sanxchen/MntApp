using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Util.MvcHtml;



namespace Carlzhu.Iooin.Entity.HRMS
{
    [PrimaryKey("RowId")]
    public class HrmsCalendarEvents : BaseEntity
    {
        [Key]
        public int RowId { get; set; }

        /// <summary>
        /// 公司主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("公司主键")]
        [ForeignKey("BaseCompany")]
        public string CompanyId { get; set; }
        public virtual BaseCompany BaseCompany { get; set; }

        public int CalendarItem { get; set; }

        public string CalendarEventsId { get; set; }

        public int D0 { get; set; }

        public string Details { get; set; }

        public int D1 { get; set; }
        public int D2 { get; set; }
        public int D3 { get; set; }
        public int D4 { get; set; }
        public int D5 { get; set; }
        public int D6 { get; set; }
        public int D7 { get; set; }
        public int D8 { get; set; }
        public int D9 { get; set; }
        public int D10 { get; set; }

        public int D11 { get; set; }
        public int D12 { get; set; }
        public int D13 { get; set; }
        public int D14 { get; set; }
        public int D15 { get; set; }
        public int D16 { get; set; }
        public int D17 { get; set; }
        public int D18 { get; set; }
        public int D19 { get; set; }
        public int D20 { get; set; }


        public int D21 { get; set; }
        public int D22 { get; set; }
        public int D23 { get; set; }
        public int D24 { get; set; }
        public int D25 { get; set; }
        public int D26 { get; set; }
        public int D27 { get; set; }
        public int D28 { get; set; }
        public int D29 { get; set; }
        public int D30 { get; set; }

        public int D31 { get; set; }

        public enum CalendarEnum
        {
            [Description("正常班加班1.5")]
            [EnumShowName("正常")]
            Null = -1,

            [Description("正常班加班1.5")]
            [EnumShowName("正常")]
            Normal = 0,

            [Description("周末加班2")]
            [EnumShowName("周末")]
            WeekDay = 1,

            [Description("国家指定节日3")]
            [EnumShowName("正常")]
            Holiday = 2,


        }

    }
}
