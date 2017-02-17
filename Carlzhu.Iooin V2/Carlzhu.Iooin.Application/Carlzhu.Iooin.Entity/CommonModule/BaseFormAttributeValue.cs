using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// 表单附加属性实例
    /// </summary>
    [Description("表单附加属性实例")]
    [PrimaryKey("AttributeValueId")]
    public class BaseFormAttributeValue : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// 附加属性实例主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("附加属性实例主键")]
        [Key]
        public string AttributeValueId { get; set; }
        /// <summary>
        /// 模块主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("模块主键")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public  virtual  BaseModule BaseModule { get; set; }

        /// <summary>
        /// 对象主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("对象主键")]
        public string ObjectId { get; set; }
        /// <summary>
        /// 参数Json
        /// </summary>
        /// <returns></returns>
        [DisplayName("参数Json")]
        public string ObjectParameterJson { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.AttributeValueId = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.AttributeValueId = keyValue;
                                            }
        #endregion
    }
}