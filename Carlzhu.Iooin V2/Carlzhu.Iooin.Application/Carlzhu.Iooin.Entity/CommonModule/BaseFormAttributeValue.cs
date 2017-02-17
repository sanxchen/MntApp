using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// ����������ʵ��
    /// </summary>
    [Description("����������ʵ��")]
    [PrimaryKey("AttributeValueId")]
    public class BaseFormAttributeValue : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ��������ʵ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������ʵ������")]
        [Key]
        public string AttributeValueId { get; set; }
        /// <summary>
        /// ģ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("ģ������")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public  virtual  BaseModule BaseModule { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string ObjectId { get; set; }
        /// <summary>
        /// ����Json
        /// </summary>
        /// <returns></returns>
        [DisplayName("����Json")]
        public string ObjectParameterJson { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.AttributeValueId = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.AttributeValueId = keyValue;
                                            }
        #endregion
    }
}