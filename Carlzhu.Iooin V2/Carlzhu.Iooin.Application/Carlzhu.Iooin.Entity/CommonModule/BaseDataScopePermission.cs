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
    /// ���ݷ�ΧȨ�ޱ�
    /// </summary>
    [Description("���ݷ�ΧȨ�ޱ�")]
    [PrimaryKey("DataScopePermissionId")]
    public class BaseDataScopePermission : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ���ݷ�ΧȨ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ݷ�ΧȨ������")]
        [Key]
        public string DataScopePermissionId { get; set; }
        /// <summary>
        /// �������:1-����2-��ɫ3-��λ4-Ⱥ��5-�û�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�������:1-����2-��ɫ3-��λ4-Ⱥ��5-�û�")]
        public string Category { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public string ObjectId { get; set; }
        /// <summary>
        /// ģ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("ģ������")]
        [ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        public  virtual  BaseModule BaseModule { get; set; }
        /// <summary>
        /// ��ʲô��Դ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ʲô��Դ")]
        public string ResourceId { get; set; }
        /// <summary>
        /// �Զ�������
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Զ�������")]
        public string Condition { get; set; }
        /// <summary>
        /// �Զ���������Json
        /// </summary>
        /// <returns></returns>
        [DisplayName("�Զ���������Json")]
        public string ConditionJson { get; set; }
        /// <summary>
        /// ��Χ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Χ����")]
        public string ScopeType { get; set; }
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
        #endregion

        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            this.DataScopePermissionId = CommonHelper.GetGuid;
            this.CreateDate = DateTime.Now;
            this.CreateUserId = ManageProvider.Provider.Current().UserId;
            this.CreateUserName = ManageProvider.Provider.Current().UserName;
            this.ScopeType = "1";
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            this.DataScopePermissionId = keyValue;
        }
        #endregion
    }
}