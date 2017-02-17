using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;


using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Entity.TPA;
using Carlzhu.Iooin.Util.MvcHtml;


namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormNcr : F
    {

        public FormNcr()
        {
            this.FileNo = "MJ42036-003";
            this.Ver = "B1";
            this.CatchTime = DateTime.Now;
            this.AbnormalPointFeed = "S1000";
        }



        //[DisplayName("图纸连接")]
        //[RegularExpression(@"^(\{{0,1}([0-9a-fA-F]){8}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){4}-([0-9a-fA-F]){12}\}{0,1})$", ErrorMessage = "请填写正确的发形码")]
        //public string PublishedLink { get; set; }

        [ForeignKey("BaseEmployee")]
        [Required]
        [DisplayName("检验员")]
        public string Inspector { get; set; }
        public virtual BaseEmployee BaseEmployee { get; set; }



        [DisplayName("文件编号")]
        [Required]
        public string FileNo { get; set; }

        [DisplayName("Ver")]
        [Required]
        public string Ver { get; set; }

        [DisplayName("报告编号/NO")]
        [Required]
        public string NoticeNo { get; set; }

        [DisplayName("品名/料号")]
        [Required]
        public string PartNo { get; set; }

        [ForeignKey("Customer")]
        [DisplayName("客户编号")]
        [Required]
        public string CustomerNo { get; set; }
        public virtual TpaCustomer Customer { get; set; }

        [Required]
        [DisplayName("异常发现时间")]
        public DateTime CatchTime { get; set; }

        [DisplayName("批号")]
        [Required]
        public string LotNo { get; set; }

        [DisplayName("批量数")]
        [Range(1, 999999)]
        public double BatchNo { get; set; }

        [DisplayName("抽样数")]
        [Range(1, 999999)]
        public double SamplingNo { get; set; }

        [DisplayName("不良数")]
        [Range(1, 999999)]
        public double DefectsNo { get; set; }



        [DisplayName("抽样标准")]
        public bool SamplingStandardSip { get; set; }

        [DisplayName("AQL")]
        public double SamplingStandardAql { get; set; }


        //[DisplayName("不良率")]
        //public double DefectsRate { get; set; }



        [DisplayName("允收标准")]
        public bool AcceptanceCriteriaC { get; set; }

        [DisplayName("AC")]
        public double AcceptanceCriteriaAc { get; set; }

        [DisplayName("RE")]
        public double AcceptanceCriteriaRe { get; set; }





        [DisplayName("缺陷等级")]
        public bool DefectsGradeCr { get; set; }
        public bool DefectsGradeMa { get; set; }
        public bool DefectsGradeMi { get; set; }


        [DisplayName("异常发现地点")]
        public int AbnormalPoint { get; set; }


        [DisplayName("供方")]
        [ForeignKey("Supplier")]
        [Required]
        public string AbnormalPointFeed { get; set; }
        public virtual TpaSupplier Supplier { get; set; }

        [DisplayName("车间")]
        public int AbnormalPointWorkshop { get; set; }

        [DisplayName("工序")]
        public string AbnormalPointProcess { get; set; }


        [Required]
        [DisplayName("文件组")]
        [ForeignKey("GroupGuid")]
        public Guid FileGroup { get; set; }
        public virtual FileGroup GroupGuid { get; set; }




        [DisplayName("品质问题现像描述")]
        [Required]
        public string QualityDescription { get; set; }

        [DisplayName("责任人")]
        public string Responsible { get; set; }

        public string AuditEmp { get; set; }
        public DateTime? AuditTime { get; set; }


        [DisplayName("异常现像")]
        public string AbnormalImage { get; set; }
        [DisplayName("异常规属")]
        public string AbnormalAttribution { get; set; }

        public virtual ICollection<FormNcrPart> FormNcrParts { get; set; }


        public enum AbnormalPointEnum
        {
            [EnumShowName("进料检验")]
            ReceivingInspection = 0,

            [EnumShowName("半成品检验")]
            SemiFinishedProductsInspection = 1,

            [EnumShowName("FQC检验")]
            FqCtest = 2,

            [EnumShowName("制程异常")]
            ProcessAbnormality = 3,

            [EnumShowName("客户")]
            Customer = 4,
        }



        public enum AbnormalPointWorkshopEnum
        {
            [EnumShowName("非车间")]
            Product = 0,

            [EnumShowName("制一")]
            ProductOne = 1,

            [EnumShowName("制二")]
            ProductTwo = 2,

            [EnumShowName("制三")]
            ProductThree = 3,


            [EnumShowName("制四")]
            ProductFour = 4,


            [EnumShowName("制五")]
            ProductFive = 5,


        }


        public enum AbnormalImageEnum
        {
            [EnumShowName("外观")]
            外观 = 0,
            [EnumShowName("尺寸")]
            尺寸 = 1,
            [EnumShowName("结构")]
            结构 = 2
        }

        public enum AbnormalAttributionEnum
        {
            [EnumShowName("装夹")]
            装夹 = 0,
            [EnumShowName("调机")]
            调机 = 1,
            [EnumShowName("工装")]
            工装 = 2,
            [EnumShowName("刀具")]
            刀具 = 3,
            [EnumShowName("材料")]
            材料 = 4,
            [EnumShowName("制程")]
            制程 = 5,
           

        }
    }
}
