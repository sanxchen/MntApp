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
    /// 系统日志明细
    /// </summary>
    [Description("系统日志明细")]
    [PrimaryKey("SysLogDetailId")]
    public class BaseSysLogDetail : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 系统日志明细主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("系统日志明细主键")]
        [Key]
        public string SysLogDetailId { get; set; }
        /// <summary>
        /// 日志主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("日志主键")]
        public string SysLogId { get; set; }
        /// <summary>
        /// 属性名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("属性名称")]
        public string PropertyName { get; set; }
        /// <summary>
        /// 属性字段
        /// </summary>
        /// <returns></returns>
        [DisplayName("属性字段")]
        public string PropertyField { get; set; }
        /// <summary>
        /// 属性新值
        /// </summary>
        /// <returns></returns>
        [DisplayName("属性新值")]
        public string NewValue { get; set; }
        /// <summary>
        /// 属性旧值
        /// </summary>
        /// <returns></returns>
        [DisplayName("属性旧值")]
        public string OldValue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? CreateDate { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.SysLogDetailId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
                                }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.SysLogDetailId = keyValue;
                                            }
        #endregion
    }
}