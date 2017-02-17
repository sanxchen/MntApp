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
    /// ��ͼ����Ȩ�ޱ�
    /// </summary>
    [Description("��ͼ����Ȩ�ޱ�")]
    [PrimaryKey("ViewPermissionId")]
    public class BaseViewPermission : BaseEntity
    {
        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ��ͼ����Ȩ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͼ����Ȩ������")]
        [Key]
        public string ViewPermissionId { get; set; }
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
        public  virtual BaseModule BaseModule { get; set; }


        /// <summary>
        /// ��ͼ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͼ����")]
        [ForeignKey("BaseView")]
        public string ViewId { get; set; }
        public  virtual BaseView BaseView { get; set; }
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
            this.ViewPermissionId = CommonHelper.GetGuid;
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
            this.ViewPermissionId = keyValue;
                                            }
        #endregion
    }
}