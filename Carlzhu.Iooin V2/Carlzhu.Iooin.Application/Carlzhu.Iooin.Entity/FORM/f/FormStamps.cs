using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormStamps : F
    {
        public FormStamps()
        {
            this.GongZhang = 0;
            this.FapiaoZhang = 0;
        }


        [Required]
        [DisplayName("文件概述")]
        public string FileSummary { get; set; }


        [Range(0, 10)]
        [DisplayName("公章")]
        public int GongZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("发票章")]
        public int FapiaoZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("合同章")]
        public int HetongZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("财务章")]
        public int CaiwuZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("法人章")]
        public int FarenZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("人事章")]
        public int RenshiZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("报关章")]
        public int BaoguanZhang { get; set; }

        [Range(0, 10)]
        [DisplayName("其他章")]
        public int QitaZhang { get; set; }


        [DisplayName("文件")]
        [ForeignKey("GroupGuid")]
        public Guid FileGroup { get; set; }

        public virtual FileGroup GroupGuid { get; set; }
    }
}
