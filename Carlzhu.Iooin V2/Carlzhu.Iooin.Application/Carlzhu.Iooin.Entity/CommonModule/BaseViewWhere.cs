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
    /// ��ͼ��ѯ������
    /// </summary>
    [Description("��ͼ��ѯ������")]
    [PrimaryKey("ViewWhereId")]
    public class BaseViewWhere : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ��ͼ��ѯ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͼ��ѯ��������")]
        [Key]
        public string ViewWhereId { get; set; }
        /// <summary>
        /// ģ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("ģ������")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public virtual  BaseModule BaseModule { get; set; }
        /// <summary>
        /// �ؼ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�����")]
        public string ControlType { get; set; }
        /// <summary>
        /// �ؼ�Ĭ��ֵ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�Ĭ��ֵ")]
        public string ControlDefault { get; set; }
        /// <summary>
        /// ������Դ
        /// </summary>
        /// <returns></returns>
        [DisplayName("������Դ")]
        public string ControlSource { get; set; }
        /// <summary>
        /// �ֶ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ֶ�����")]
        public string FieldName { get; set; }
        /// <summary>
        /// �ڲ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ڲ�����")]
        public string FullName { get; set; }
        /// <summary>
        /// ��ʾ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ʾ����")]
        public string ShowName { get; set; }
        /// <summary>
        /// �Ƿ���ʾ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Ƿ���ʾ")]
        public int? AllowShow { get; set; }
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
        /// ����ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ʱ��")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// �Զ���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Զ���")]
        public string ControlCustom { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.ViewWhereId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.ViewWhereId = keyValue;
        }
        #endregion
    }
}