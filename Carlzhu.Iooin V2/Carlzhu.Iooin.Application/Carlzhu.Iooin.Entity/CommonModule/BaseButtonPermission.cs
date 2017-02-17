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
    /// ������ťȨ�ޱ�
    /// </summary>
    [Description("������ťȨ�ޱ�")]
    [PrimaryKey("ButtonPermissionId")]
    public class BaseButtonPermission : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ������ťȨ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������ťȨ������")]
        [Key]
        public string ButtonPermissionId { get; set; }
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
        //[ForeignKey("BaseModule")]
        public string ModuleId { get; set; }
        //public virtual BaseModule BaseModule { get; set; }
        /// <summary>
        /// ģ�鰴ť����
        /// </summary>
        /// <returns></returns>
        [DisplayName("ģ�鰴ť����")]
        [ForeignKey("BaseButton")]
        public string ModuleButtonId { get; set; }
        public  virtual  BaseButton BaseButton { get; set; }

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
            this.ButtonPermissionId = CommonHelper.GetGuid;
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
            this.ButtonPermissionId = keyValue;
        }
        #endregion
    }
}