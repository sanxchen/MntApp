using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.CommonModule
{
    /// <summary>
    /// 职员信息
    /// </summary>
    [Description("职员信息")]
    [PrimaryKey("EmpNo")]
    public class BaseEmployee : BaseEntity
    {
        //public BaseEmployee this[string empNo] => CarlzhuContext.CzContext.BaseEmployees.Find(empNo);

        //public const int NoLength = 7;


        #region 获取/设置 字段值
        /// <summary>
        /// 职员主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("职员主键")]
        [Key]
        //[ForeignKey("BaseUser")]
        public string EmpNo { get; set; }

        [Required]
        [DisplayName("磁卡号")]
        public string CardNo { get; set; }

        [Required]
        [DisplayName("姓名")]
        public string RealName { get; set; }

        [DisplayName("英文名")]
        public string Account { get; set; }

        [DisplayName("邮箱")]
        public string Email { get; set; }

        [DisplayName("性别")]
        public string Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("出生日期")]
        public DateTime? Birthday { get; set; }

        [DisplayName("员工类别")]
        [Required]
        public string Identity { get; set; }


        [ForeignKey("BaseDepartment")]
        public string DepartmentId { get; set; }
        public virtual BaseDepartment BaseDepartment { get; set; }




        [ForeignKey("BasePost")]
        public string Position { get; set; }
        public virtual BasePost BasePost { get; set; }



        [Required]
        [RegularExpression(@"^1[0-9]{10}$", ErrorMessage = "请输入正确的手机号码")]
        [DisplayName("手机")]
        public string Mobile { get; set; }




        /// <summary>
        /// 电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("电话")]
        public string Telephone { get; set; }


        /// <summary>
        /// 办公短号
        /// </summary>
        /// <returns></returns>
        [DisplayName("办公短号")]
        public string OfficeCornet { get; set; }

        /// <summary>
        /// 照片
        /// </summary>
        /// <returns></returns>
        [DisplayName("照片")]
        public string Photograph { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        /// <returns></returns>
        [DisplayName("身份证号码")]
        public string IDCard { get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        /// <returns></returns>
        [DisplayName("年龄")]
        public int? Age { get; set; }
        /// <summary>
        /// 工资卡
        /// </summary>
        /// <returns></returns>
        [DisplayName("工资卡")]
        public string BankCode { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("办公电话")]
        public string OfficePhone { get; set; }
        /// <summary>
        /// 办公邮编
        /// </summary>
        /// <returns></returns>
        [DisplayName("现住地址")]
        public string LiveAddress { get; set; }
        /// <summary>
        /// 办公地址
        /// </summary>
        /// <returns></returns>
        [DisplayName("办公地址")]
        public string OfficeAddress { get; set; }
        /// <summary>
        /// 办公传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("办公传真")]
        public string OfficeFax { get; set; }
        /// <summary>
        /// 最高学历
        /// </summary>
        /// <returns></returns>
        [DisplayName("最高学历")]
        public string Education { get; set; }
        /// <summary>
        /// 毕业院校
        /// </summary>
        /// <returns></returns>
        [DisplayName("毕业院校")]
        public string School { get; set; }
        /// <summary>
        /// 毕业时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("毕业时间")]
        public DateTime? GraduationDate { get; set; }
        /// <summary>
        /// 所学专业
        /// </summary>
        /// <returns></returns>
        [DisplayName("所学专业")]
        public string Major { get; set; }
        /// <summary>
        /// 最高学位
        /// </summary>
        /// <returns></returns>
        [DisplayName("最高学位")]
        public string Degree { get; set; }
        /// <summary>
        /// 入职时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("入职时间")]
        public DateTime? WorkingDate { get; set; }

        /// <summary>
        /// 试用期
        /// </summary>
        /// <returns></returns>
        [DisplayName("试用期")]
        public int ProbationPeriod { get; set; }



        /// <summary>
        /// 合同签订日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("合同签订日期")]
        public DateTime? ContractDate { get; set; }

        /// <summary>
        /// 合同周期
        /// </summary>
        /// <returns></returns>
        [DisplayName("合同周期")]
        public int ContractPeriod { get; set; }




        /// <summary>
        /// 家庭住址邮编
        /// </summary>
        /// <returns></returns>
        [DisplayName("家庭住址邮编")]
        public string HomeZipCode { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        /// <returns></returns>
        [DisplayName("家庭住址")]
        public string HomeAddress { get; set; }
        /// <summary>
        /// 住宅电话
        /// </summary>
        /// <returns></returns>
        [DisplayName("住宅电话")]
        public string HomePhone { get; set; }
        /// <summary>
        /// 家庭传真
        /// </summary>
        /// <returns></returns>
        [DisplayName("家庭传真")]
        public string HomeFax { get; set; }
        /// <summary>
        /// 籍贯省
        /// </summary>
        /// <returns></returns>
        [DisplayName("籍贯省")]
        public string Province { get; set; }
        /// <summary>
        /// 籍贯市
        /// </summary>
        /// <returns></returns>
        [DisplayName("籍贯市")]
        public string City { get; set; }
        /// <summary>
        /// 籍贯区
        /// </summary>
        /// <returns></returns>
        [DisplayName("籍贯区")]
        public string Area { get; set; }
        /// <summary>
        /// 籍贯
        /// </summary>
        /// <returns></returns>
        [DisplayName("籍贯")]
        public string NativePlace { get; set; }
        /// <summary>
        /// 政治面貌
        /// </summary>
        /// <returns></returns>
        [DisplayName("政治面貌")]
        public string Party { get; set; }
        /// <summary>
        /// 国籍
        /// </summary>
        /// <returns></returns>
        [DisplayName("国籍")]
        public string Nation { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        /// <returns></returns>
        [DisplayName("民族")]
        public string Nationality { get; set; }
        /// <summary>
        /// 职务
        /// </summary>
        /// <returns></returns>
        [DisplayName("职务")]
        public string Duty { get; set; }
        /// <summary>
        /// 用工性质
        /// </summary>
        /// <returns></returns>
        [DisplayName("用工性质")]
        public string WorkingProperty { get; set; }
        /// <summary>
        /// 职业资格
        /// </summary>
        /// <returns></returns>
        [DisplayName("职业资格")]
        public string Competency { get; set; }
        /// <summary>
        /// 紧急联系
        /// </summary>
        /// <returns></returns>
        [DisplayName("紧急联系")]
        public string EmergencyContact { get; set; }


        /// <summary>
        /// 离职
        /// </summary>
        /// <returns></returns>
        [DisplayName("离职")]
        public int? IsDimission { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        /// <returns></returns>
        [DisplayName("离职日期")]
        public DateTime? DimissionDate { get; set; }
        /// <summary>
        /// 离职原因
        /// </summary>
        /// <returns></returns>
        [DisplayName("离职原因")]
        public string DimissionCause { get; set; }
        /// <summary>
        /// 离职去向
        /// </summary>
        /// <returns></returns>
        [DisplayName("离职去向")]
        public string DimissionWhither { get; set; }


        /// <summary>
        /// 是否需要排班
        /// </summary>
        public bool IsShift { get; set; }

        /// <summary>
        /// 默认班别
        /// </summary>
        public string DefaultShift { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public int MaritalStatus { get; set; }

        /// <summary>
        /// 主管工号
        /// </summary>
        public string ManagerId { get; set; }
        /// <summary>
        /// 主管姓名
        /// </summary>
        public string Manager { get; set ; }

        #endregion

        #region 参保信息

        /// <summary>
        /// 社保
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保")]
        public int? IsSocialSecurity { get; set; }

        /// <summary>
        /// 社保编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保编号")]
        public string SocialSecurityNo { get; set; }


        /// <summary>
        /// 参保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("参保时间")]
        public DateTime? SocialSecuritySDate { get; set; }

        /// <summary>
        /// 退保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("退保时间")]
        public DateTime? SocialSecurityEDate { get; set; }

        /// <summary>
        /// 公积金
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保")]
        public int? Isfund { get; set; }

        /// <summary>
        /// 公积金编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保编号")]
        public string FundNo { get; set; }


        /// <summary>
        /// 参保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("参保时间")]
        public DateTime? FundSDate { get; set; }

        /// <summary>
        /// 退保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("退保时间")]
        public DateTime? FundEDate { get; set; }


        /// <summary>
        /// 商保
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保")]
        public int? IsCommercialInsurance { get; set; }

        /// <summary>
        /// 商保编号
        /// </summary>
        /// <returns></returns>
        [DisplayName("社保编号")]
        public string CommercialInsuranceNo { get; set; }


        /// <summary>
        /// 参保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("参保时间")]
        public DateTime? CommercialInsuranceSDate { get; set; }

        /// <summary>
        /// 退保时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("退保时间")]
        public DateTime? CommercialInsuranceEDate { get; set; }
        #endregion





        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            //  this.EmployeeId = CommonHelper.GetGuid;
        }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="keyValue"></param>
        public override void Modify(string keyValue)
        {
            // this.EmployeeId = keyValue;
        }
        #endregion
    }
}