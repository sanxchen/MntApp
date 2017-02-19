using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.QUALITY
{
    [PrimaryKey("MntNo")]
    public class Apparatus:BaseEntity
    {
        [Key]
        [Required]
        [DisplayName("编号")]
        public string MntNo { get; set; }

        [DisplayName("名称")]
        public string Name { get; set; }

 

        [DisplayName("型号")]
        public string Model { get; set; }


        [DisplayName("校验日期")]
        public DateTime CalDate { get; set; }


        [DisplayName("校验结果")]
        public string CalResult { get; set; }


        public string CertificateResults { get; set; }


        [DisplayName("校验周期")]
        public string CalCircle { get; set; }


        public DateTime? NextCalDate { get; set; }

        [DisplayName("校验类型")]
        public string CalType { get; set; }

        /// <summary>
        /// 使用状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 使用人
        /// </summary>
        public string UseEmp { get; set; }
        /// <summary>
        /// 使用人工号
        /// </summary>
        public string UseEmpId { get; set; }

        public string UseAdd { get; set; }


      


        [DisplayName("备注")]
        public string Mark { get; set; }









        public enum CalCircleEnum
        {
            免校 = 0,
            半年 = 6,
            一年 = 12,
        }

        public enum CalResultEnum
        {
            免校 = 0,
            合格 = 1,
            不合格 = 2,

        }
        public enum StatusEnum
        {
            免校 = 0,
            使用中 = 1,
            未使用 = 2,
            维修中 = 3,
            丢失 = 4,
            报废 = 5
        }
        public enum CalTypeEnum
        {
            免校 = 0,
            内校 = 1,
            外校 = 2,
            其他 = 3
        }

    }
}
