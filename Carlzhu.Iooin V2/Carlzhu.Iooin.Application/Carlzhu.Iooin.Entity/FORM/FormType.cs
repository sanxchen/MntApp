using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.HRMS;

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("FormId")]
    public class FormType : BaseEntity
    {

        public FormType()
        {
            this.CreateTime = DateTime.Now;
            this.IsHot = false;
            this.IsRedirect = true;
            this.IsReplace = true;
            this.IsColsed = false;
            this.IsModel = true;
            this.FileLimit = 0;
        }

        [Key]
        [DisplayName("表单ID")]
        public int FormId { get; set; }

        [Required]
        [DisplayName("表单名称")]
        public string FormName { get; set; }

        [Required]
        [DisplayName("表单方法")]
        public string Method { get; set; }


        [Required]
        [DisplayName("表单页面")]
        public string FormPage { get; set; }

        [Required]
        [DisplayName("表单类别")]
        [ForeignKey("FormClass")]
        public string Class { get; set; }



        [Required]
        public string RouteOne { get; set; }
        public string RouteTwo { get; set; }
        public string RouteThree { get; set; }
        public string RouteFour { get; set; }




        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        [DisplayName("热门")]
        public bool IsHot { get; set; }

        [DisplayName("转签")]
        public bool IsRedirect { get; set; }

        [DisplayName("加签")]
        public bool IsAdd { get; set; }


        [DisplayName("代签")]
        public bool IsReplace { get; set; }

        [DisplayName("系统")]
        public string Os { get; set; }


        [DisplayName("可上传文件个数")]
        public int FileLimit { get; set; }


        [DisplayName("上传文件类型")]
        public string AllowFileType { get; set; }

        [DisplayName("必传类型")]
        public string ReqFileExp { get; set; }

        [DisplayName("表单前面执行")]
        public bool IsStart { get; set; }

        [DisplayName("表单是否需要结案")]
        public bool IsColsed { get; set; }

        [DisplayName("是否为单一实体Model")]
        public bool IsModel { get; set; }

        ///// <summary>
        ///// 是否生成备注
        ///// </summary>
        //[DisplayName("是否显示备注栏")]
        //public bool IsMark { get; set; }


        [DisplayName("是否前端显示")]
        public bool IsDisplay { get; set; }

        [DisplayName("自申自审")]
        public bool IsMyself { get; set; }



        [DisplayName("处理人")]
        public string Handler { get; set; }


        public FormClass FormClass { get; set; }


        /// <summary>
        /// 用户端辑html
        /// </summary>
        public string EditHtml { get; set; }

        //详情Html
        public string DetailsHtml { get; set; }




        public virtual ICollection<Form> Forms { get; set; }

    }
}
