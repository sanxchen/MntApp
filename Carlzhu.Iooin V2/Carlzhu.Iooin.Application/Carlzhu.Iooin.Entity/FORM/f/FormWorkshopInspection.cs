using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormWorkshopInspection : F
    {

        public FormWorkshopInspection()
        {
            Picture = Guid.Empty;
        }

        [DisplayName("排队号")]
        public double Order { get; set; }

        [DisplayName("料号")]
        public string ProductNo { get; set; }

        [DisplayName("工艺")]
        public string Engineering { get; set; }

        [DisplayName("机台")]
        public string Machine { get; set; }


        [DisplayName("调机人")]
        public string TransferMachine { get; set; }

        [DisplayName("送检人")]
        public string SendUser { get; set; }



        [DisplayName("开始检测时间")]
        public DateTime? StartDateTime { get; set; }

        [DisplayName("结束检测时间")]
        public DateTime? EndDateTime { get; set; }


        [DisplayName("检测结果")]
        public int DetectionResult { get; set; }


        [ForeignKey("BaseEmployee")]
        [DisplayName("检测人")]
        public string Checker { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }

        [DisplayName("说明")]
        public string Explan { get; set; }

        [DisplayName("不良图片")]
        [ForeignKey("FileGroup")]
        public Guid Picture { get; set; }


        [DisplayName("拿取")]
        public bool CanTakeit { get; set; }


        [DisplayName("工单编号")]
        public string ManNo { get; set; }


        [DisplayName("产品类型")]
        public string ProductType { get; set; }


        public virtual FileGroup FileGroup { get; set; }



        public enum Detection
        {
            合格 = 0,
            不合格 = 1,
            其他 = 2

        }
    }
}
