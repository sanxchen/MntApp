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
    /// ����������
    /// </summary>
    [Description("����������")]
    [PrimaryKey("FormAttributeId")]
    public class BaseFormAttribute : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ��������������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������������")]
        [Key]
        public string FormAttributeId { get; set; }
        /// <summary>
        /// ģ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("ģ������")]
        //[ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        //public  virtual  BaseModule BaseModule { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string PropertyName { get; set; }
        /// <summary>
        /// �ؼ�Id
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�Id")]
        public string ControlId { get; set; }
        /// <summary>
        /// �ؼ�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�����")]
        public string ControlType { get; set; }
        /// <summary>
        /// �ؼ���ʽ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ���ʽ")]
        public string ControlStyle { get; set; }
        /// <summary>
        /// �ؼ���֤
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ���֤")]
        public string ControlValidator { get; set; }
        /// <summary>
        /// ���볤��
        /// </summary>
        /// <returns></returns>
        [DisplayName("���볤��")]
        public int? ImportLength { get; set; }
        /// <summary>
        /// Ĭ��ֵ
        /// </summary>
        /// <returns></returns>
        [DisplayName("Ĭ��ֵ")]
        public string DefaultVlaue { get; set; }
        /// <summary>
        /// �Զ�������
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Զ�������")]
        public string AttributesProperty { get; set; }
        /// <summary>
        /// �ؼ�����Դ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�����Դ����")]
        public int? DataSourceType { get; set; }
        /// <summary>
        /// �ؼ�����Դ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ؼ�����Դ")]
        public string DataSource { get; set; }
        /// <summary>
        /// �ϲ���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ϲ���")]
        public string ControlColspan { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ע")]
        public string Remark { get; set; }
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
        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ʱ��")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// �����û�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����û�����")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// �����û�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�����û�")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸�ʱ��")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// �޸��û�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸��û�����")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// �޸��û�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�޸��û�")]
        public string ModifyUserName { get; set; }
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.FormAttributeId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.FormAttributeId = keyValue;
            this.ModifyDate = DateTime.Now;
            this.ModifyUserId = ManageProvider.Provider.Current().UserId;
            this.ModifyUserName = ManageProvider.Provider.Current().UserName;
        }
        #endregion
    }
}