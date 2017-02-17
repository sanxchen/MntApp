using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.QUALITY;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormReplaceDrawings : F
    {
        [DisplayName("原表单号")]
        [Required]
        [RegularExpression("\\d{13}", ErrorMessage = "原表单号好像是13位数字")]
        public string OldFormNo { get; set; }


        [DisplayName("文件发行码")]
        [Required]
        [RegularExpression(@"^\w{8}-(\w{4}-){3}\w{12}$",ErrorMessage = "文件发行码不符合规范")]
        [ForeignKey("Published")]
        public Guid PublishedCode { get; set; }
        public virtual Published Published { get; set; }


        [Required]
        [DisplayName("原文件码")]
        [RegularExpression("\\w{32}", ErrorMessage = "这个应该是32位字符")]
        public string OldMd5 { get; set; }



        [Required]
        [DisplayName("新文件组")]
        public Guid FileGroup { get; set; }

    }
}
