using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.CommonModule;

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("FormClassId")]
    public class FormClass
    {
       

        [Key]
        [DisplayName("部门主键")]
        public string FormClassId { get; set; }



        /// <summary>
        /// 公司主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("公司主键")]
        [ForeignKey("BaseCompany")]
        public string CompanyId { get; set; }
        public virtual BaseCompany BaseCompany { get; set; }

        /// <summary>
        /// 父级主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("父级主键")]
        public string ParentId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        /// <returns></returns>
        [DisplayName("编码")]
        public string Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("名称")]
        public string FullName { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        /// <returns></returns>
        [DisplayName("简称")]
        public string ShortName { get; set; }
        /// <summary>
        /// 性质
        /// </summary>
        /// <returns></returns>
        [DisplayName("性质")]
        public string Nature { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        /// <returns></returns>
        [DisplayName("负责人")]
        public string Manager { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("联系电话")]
        public string Phone { get; set; }
        /// <summary>
        /// 传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("传真")]
        public string Fax { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        /// <returns></returns>
        [DisplayName("电子邮件")]
        public string Email { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效")]
        public int? Enabled { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [DisplayName("排序码")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        [DisplayName("删除标记")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户主键")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改时间")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户主键")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string ModifyUserName { get; set; }


    }
}
