using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// ְԱ��Ϣ
    /// </summary>
    [Description("ְԱ��Ϣ")]
    [PrimaryKey("EmpNo")]
    public class BaseEmployee : BaseEntity
    {
        //public BaseEmployee this[string empNo] => CarlzhuContext.CzContext.BaseEmployees.Find(empNo);

        //public const int NoLength = 7;


        #region ��ȡ/���� �ֶ�ֵ
        /// <summary>
        /// ְԱ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("ְԱ����")]
        [Key]
        //[ForeignKey("BaseUser")]
        public string EmpNo { get; set; }

        [Required]
        [DisplayName("�ſ���")]
        public string CardNo { get; set; }

        [Required]
        [DisplayName("����")]
        public string RealName { get; set; }

        [DisplayName("Ӣ����")]
        public string Account { get; set; }

        [DisplayName("����")]
        public string Email { get; set; }

        [DisplayName("�Ա�")]
        public string Gender { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��������")]
        public DateTime? Birthday { get; set; }

        [DisplayName("Ա�����")]
        [Required]
        public string Identity { get; set; }


        [ForeignKey("BaseDepartment")]
        public string DepartmentId { get; set; }
        public virtual BaseDepartment BaseDepartment { get; set; }




        [ForeignKey("BasePost")]
        public string Position { get; set; }
        public virtual BasePost BasePost { get; set; }



        [Required]
        [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "��������ȷ���ֻ�����")]
        [DisplayName("�ֻ�")]
        public string Mobile { get; set; }




        /// <summary>
        /// �绰
        /// </summary>
        /// <returns></returns>
        [DisplayName("�绰")]
        public string Telephone { get; set; }


        /// <summary>
        /// �칫�̺�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�칫�̺�")]
        public string OfficeCornet { get; set; }

        /// <summary>
        /// ��Ƭ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��Ƭ")]
        public string Photograph { get; set; }
        /// <summary>
        /// ���֤����
        /// </summary>
        /// <returns></returns>
        [DisplayName("���֤����")]
        public string IDCard { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public int? Age { get; set; }
        /// <summary>
        /// ���ʿ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ʿ�")]
        public string BankCode { get; set; }

        /// <summary>
        /// �칫�绰
        /// </summary>
        /// <returns></returns>
        [DisplayName("�칫�绰")]
        public string OfficePhone { get; set; }
        /// <summary>
        /// �칫�ʱ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ס��ַ")]
        public string LiveAddress { get; set; }
        /// <summary>
        /// �칫��ַ
        /// </summary>
        /// <returns></returns>
        [DisplayName("�칫��ַ")]
        public string OfficeAddress { get; set; }
        /// <summary>
        /// �칫����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�칫����")]
        public string OfficeFax { get; set; }
        /// <summary>
        /// ���ѧ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ѧ��")]
        public string Education { get; set; }
        /// <summary>
        /// ��ҵԺУ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ҵԺУ")]
        public string School { get; set; }
        /// <summary>
        /// ��ҵʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ҵʱ��")]
        public DateTime? GraduationDate { get; set; }
        /// <summary>
        /// ��ѧרҵ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ѧרҵ")]
        public string Major { get; set; }
        /// <summary>
        /// ���ѧλ
        /// </summary>
        /// <returns></returns>
        [DisplayName("���ѧλ")]
        public string Degree { get; set; }
        /// <summary>
        /// ��ְʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ְʱ��")]
        public DateTime? WorkingDate { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public int ProbationPeriod { get; set; }



        /// <summary>
        /// ��ͬǩ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͬǩ������")]
        public DateTime? ContractDate { get; set; }

        /// <summary>
        /// ��ͬ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͬ����")]
        public int ContractPeriod { get; set; }




        /// <summary>
        /// ��ͥסַ�ʱ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͥסַ�ʱ�")]
        public string HomeZipCode { get; set; }
        /// <summary>
        /// ��ͥסַ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͥסַ")]
        public string HomeAddress { get; set; }
        /// <summary>
        /// סլ�绰
        /// </summary>
        /// <returns></returns>
        [DisplayName("סլ�绰")]
        public string HomePhone { get; set; }
        /// <summary>
        /// ��ͥ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ͥ����")]
        public string HomeFax { get; set; }
        /// <summary>
        /// ����ʡ
        /// </summary>
        /// <returns></returns>
        [DisplayName("����ʡ")]
        public string Province { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string City { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("������")]
        public string Area { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public string NativePlace { get; set; }
        /// <summary>
        /// ������ò
        /// </summary>
        /// <returns></returns>
        [DisplayName("������ò")]
        public string Party { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public string Nation { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("����")]
        public string Nationality { get; set; }
        /// <summary>
        /// ְ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("ְ��")]
        public string Duty { get; set; }
        /// <summary>
        /// �ù�����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�ù�����")]
        public string WorkingProperty { get; set; }
        /// <summary>
        /// ְҵ�ʸ�
        /// </summary>
        /// <returns></returns>
        [DisplayName("ְҵ�ʸ�")]
        public string Competency { get; set; }
        /// <summary>
        /// ������ϵ
        /// </summary>
        /// <returns></returns>
        [DisplayName("������ϵ")]
        public string EmergencyContact { get; set; }


        /// <summary>
        /// ��ְ
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ְ")]
        public int? IsDimission { get; set; }
        /// <summary>
        /// ��ְ����
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ְ����")]
        public DateTime? DimissionDate { get; set; }
        /// <summary>
        /// ��ְԭ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ְԭ��")]
        public string DimissionCause { get; set; }
        /// <summary>
        /// ��ְȥ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("��ְȥ��")]
        public string DimissionWhither { get; set; }


        /// <summary>
        /// �Ƿ���Ҫ�Ű�
        /// </summary>
        public bool IsShift { get; set; }

        /// <summary>
        /// Ĭ�ϰ��
        /// </summary>
        public string DefaultShift { get; set; }

        /// <summary>
        /// ����״��
        /// </summary>
        public int MaritalStatus { get; set; }

        /// <summary>
        /// ���ܹ���
        /// </summary>
        public string ManagerId { get; set; }
        /// <summary>
        /// ��������
        /// </summary>
        public string Manager { get; set ; }

        #endregion

        #region �α���Ϣ

        /// <summary>
        /// �籣
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣")]
        public int? IsSocialSecurity { get; set; }

        /// <summary>
        /// �籣���
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣���")]
        public string SocialSecurityNo { get; set; }


        /// <summary>
        /// �α�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�α�ʱ��")]
        public DateTime? SocialSecuritySDate { get; set; }

        /// <summary>
        /// �˱�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�˱�ʱ��")]
        public DateTime? SocialSecurityEDate { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣")]
        public int? Isfund { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣���")]
        public string FundNo { get; set; }


        /// <summary>
        /// �α�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�α�ʱ��")]
        public DateTime? FundSDate { get; set; }

        /// <summary>
        /// �˱�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�˱�ʱ��")]
        public DateTime? FundEDate { get; set; }


        /// <summary>
        /// �̱�
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣")]
        public int? IsCommercialInsurance { get; set; }

        /// <summary>
        /// �̱����
        /// </summary>
        /// <returns></returns>
        [DisplayName("�籣���")]
        public string CommercialInsuranceNo { get; set; }


        /// <summary>
        /// �α�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�α�ʱ��")]
        public DateTime? CommercialInsuranceSDate { get; set; }

        /// <summary>
        /// �˱�ʱ��
        /// </summary>
        /// <returns></returns>
        [DisplayName("�˱�ʱ��")]
        public DateTime? CommercialInsuranceEDate { get; set; }
        #endregion





        #region ��չ����
        /// <summary>
        /// ��������
        /// </summary>
        public override void Create()
        {
            //  this.EmployeeId = CommonHelper.GetGuid;
        }
        /// <summary>
        /// �༭����
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            // this.EmployeeId = keyValue;
        }
        #endregion
    }
}