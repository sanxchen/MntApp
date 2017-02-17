using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Util;



namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// �ӿڲ���
    /// </summary>
    [Description("�ӿڲ���")]
    [PrimaryKey("InterfaceParameterId")]
    public class BaseInterfaceManageParameter : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// �ӿڲ�������
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ӿڲ�������")]
        [Key]
        public string InterfaceParameterId { get; set; }
        /// <summary>
        /// �ӿ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ӿ�����")]
        [ForeignKey("BaseInterfaceManage")]
        public string InterfaceId { get; set; }
        public  virtual BaseInterfaceManage BaseInterfaceManage { get; set; }
        /// <summary>
        /// �����ֶ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����ֶ�")]
        public string Field { get; set; }
        /// <summary>
        /// ����˵��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����˵��")]
        public string FieldMemo { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string FieldType { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string FieldMaxLength { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����")]
        public int? AllowNull { get; set; }
        /// <summary>
        /// ��Ч
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Ч")]
        public int? Enabled { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public int? SortCode { get; set; }
        /// <summary>
        /// ɾ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("ɾ�����")]
        public int? DeleteMark { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.InterfaceParameterId = CommonHelper.GetGuid;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.InterfaceParameterId = keyValue;
        }
        #endregion
    }
}