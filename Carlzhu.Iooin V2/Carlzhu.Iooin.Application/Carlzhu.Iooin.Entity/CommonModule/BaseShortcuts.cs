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
    /// 首页快捷方式
    /// </summary>
    [Description("首页快捷方式")]
    [PrimaryKey("ShortcutsId")]
    public class BaseShortcuts : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 快捷方式主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("快捷方式主键")]
        [Key]
        public string ShortcutsId { get; set; }
        /// <summary>
        /// 模块主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("模块主键")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public  virtual BaseModule BaseModule { get; set; }
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
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.ShortcutsId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
                    }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ShortcutsId = keyValue;
                                            }
        #endregion
    }
}