using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.mes
{
    [PrimaryKey("ManNo")]
    public class MesRkwPo
    {
        /// <summary>
        /// 工单号
        /// </summary>
        [Key]
        [DisplayName("工单号")]
        [Required]
        public string ManNo { get; set; }

        /// <summary>
        /// 供应商代码
        /// </summary>
        [DisplayName("供应商代码")]
        [Required]
        public string SupplierNo { get; set; }

        /// <summary>
        /// 工单总数
        /// </summary>
        [DisplayName("工单总数")]
        [Required]
        public int Qty { get; set; }


        /// <summary>
        /// 客户名称
        /// </summary>
        [DisplayName("客户名称")]
        [Required]
        public string CusName { get; set; }

        /// <summary>
        /// 明捷PO
        /// </summary>
        [DisplayName("明捷PO")]
        [Required]
        public string MinicutPoNo { get; set; }

        /// <summary>
        /// 料号
        /// </summary>
        [DisplayName("料号")]
        [Required]
        public string PartNo { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        [DisplayName("品名")]
        [Required]
        public string PartName { get; set; }

        /// <summary>
        /// Mo
        /// </summary>
        [DisplayName("Mo")]
        [Required]
        public string MoNo { get; set; }

        /// <summary>
        /// 炉号
        /// </summary>
        [DisplayName("炉号")]
        [Required]
        public string HeatNo { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        [DisplayName("供应商")]
        [Required]
        public string Supplier { get; set; }

        /// <summary>
        /// RecaroPo
        /// </summary>
        [DisplayName("RecaroPo")]
        [Required]
        public string RecaroPoNo { get; set; }


        public int BoxSize { get; set; }


        public int BoxLabelQty { get; set; }


        /// <summary>
        /// 是否打印
        /// </summary>
        public bool IsPrint { get; set; }

    }
}
