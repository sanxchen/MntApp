using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    /// <summary>
    /// 图纸基类
    /// </summary>
    public abstract class DrawingsBase : F
    {
        protected DrawingsBase()
        {
            this.PageSize = 0;
            this.IsPublished = false;
            //this.Customer = new Customer();
        }



        [Required]
        [DisplayName("客户名称")]
        [ForeignKey("Customer")]
        public string CustomerNo { get; set; }
        public virtual TpaCustomer Customer { get; set; }




        [DisplayName("产品编号(出货料号)")]
        public string ProductNo { get; set; }






        [Required]
        [DisplayName("版本名称")]
        public string DrawVer { get; set; }


        [Required]
        [DisplayName("总页数")]
        [Range(1, 50)]
        public int PageSize { get; set; }

        [Required]
        [DisplayName("文件组")]
        [ForeignKey("GroupGuid")]
        public Guid FileGroup { get; set; }

        [Required]
        [DisplayName("发行原因")]
        public string Reason { get; set; }

        public bool IsPublished { get; set; }

        public Guid Identity { get; set; }

        public virtual FileGroup GroupGuid { get; set; }
    }
}
