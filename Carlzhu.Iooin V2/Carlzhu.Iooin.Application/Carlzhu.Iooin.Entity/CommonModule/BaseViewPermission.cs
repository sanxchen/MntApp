using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Util;




namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// 视图设置权限表
    /// </summary>
    [Description("视图设置权限表")]
    [PrimaryKey("ViewPermissionId")]
    public class BaseViewPermission : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 视图设置权限主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("视图设置权限主键")]
        [Key]
        public string ViewPermissionId { get; set; }
        /// <summary>
        /// 对象分类:1-部门2-角色3-岗位4-群组5-用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("对象分类:1-部门2-角色3-岗位4-群组5-用户")]
        public string Category { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("对象主键")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 模块主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("模块主键")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public  virtual BaseModule BaseModule { get; set; }


        /// <summary>
        /// 视图主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("视图主键")]
        [ForeignKey("BaseView")]
        public string ViewId { get; set; }
        public  virtual BaseView BaseView { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [DisplayName("排序码")]
        public int? SortCode { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ViewPermissionId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ViewPermissionId = keyValue;
                                            }
        #endregion
    }
}