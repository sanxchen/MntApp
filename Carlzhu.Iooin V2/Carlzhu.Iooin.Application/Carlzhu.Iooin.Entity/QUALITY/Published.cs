using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Entity.HRMS;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.QUALITY
{
    [PrimaryKey("PubishedGuid")]
    public class Published
    {
        public Published()
        {
            this.IsDel = false;
            this.IsPass = false;
        }
        [Key]
        public Guid PubishedGuid { get; set; }

        [DisplayName("表单号")]
        [ForeignKey("Form")]
        public string FormNo { get; set; }


        //----------------

        [DisplayName("发行类型")]
        [ForeignKey("FormType")]
        public int PublishType { get; set; }

        [DisplayName("发行时间")]
        public DateTime PublishTime { get; set; }

        [DisplayName("发行人")]
        [ForeignKey("BaseEmployee")]
        public string EmpNo { get; set; }

        [DisplayName("发行版本")]
        public string PublishVer { get; set; }

        [DisplayName("是否删除")]
        public bool IsDel { get; set; }

        [DisplayName("是否加密")]
        public bool IsPass { get; set; }

        [DisplayName("发行标识")]
        public Guid Identity { get; set; }




        [DisplayName("客户名称")]
        [ForeignKey("Customer")]
        public string CustomerNo { get; set; }
        public virtual TpaCustomer Customer { get; set; }


        [Required]
        [DisplayName("文件组")]
        [ForeignKey("FileGroupGuid")]
        public Guid FileGroup { get; set; }
        public virtual FileGroup FileGroupGuid { get; set; }



        [DisplayName("产品料号")]
        public string ProductNo { get; set; }

        public int Visit { get; set; }


        public virtual BaseEmployee BaseEmployee { get; set; }


        public virtual FormType FormType { get; set; }

        public virtual Form Form { get; set; }


    }
}
