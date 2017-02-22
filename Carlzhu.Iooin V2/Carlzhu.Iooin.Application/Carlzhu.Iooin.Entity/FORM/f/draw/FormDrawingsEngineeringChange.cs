using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.TPA;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    public class FormDrawingsEngineeringChange:F
    {

        public FormDrawingsEngineeringChange()
        {
            ValiDateTime = DateTime.Now.Date;
            VerificationDate = DateTime.Now.Date;
            ReleaseDeptDate =DateTime.Now.Date;
            ReleaseEmpDate = DateTime.Now.Date;
            ReleaseAuditDate = DateTime.Now.Date;
            ReleaseApprovalDate = DateTime.Now.Date;
        }


        [DisplayName("ECN编号")]
        public string No { get; set; }


        [DisplayName("客户编号")]
        [ForeignKey(("TpaCustomer"))]
        public string CustomerNo { get; set; }

        public virtual  TpaCustomer TpaCustomer { get; set; }


        [DisplayName("产品编号")]
        public string ProductNo { get; set; }

        [DisplayName("产品名称")]
        public string ProductName { get; set; }

        [DisplayName("版本号")]
        public string Ver { get; set; }

        [DisplayName("变更方式")]
        public int ChangeMethod { get; set; }

        [DisplayName("变更原因")]
        public string ChangeReason { get; set; }
        [DisplayName("变更内容")]
        public string ChangeContent { get; set; }
        [DisplayName("是否需要验证")]
        public int HasValidate { get; set; }
        [DisplayName("不需验证原因")]
        public string NoValidateReason { get; set; }

        [DisplayName("验证日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime VerificationDate { get; set; }

        [DisplayName("投入数量")]
        public double InputQuantity { get; set; }
        [DisplayName("OK数量")]
        public double OKQuantity { get; set; }

        [DisplayName("良率")]
        public double Yield { get; set; }

        [DisplayName("验证结果及说明")]
        public string VerificationConclusion { get; set; }

        [DisplayName("验证批准")]
        public string ValidateApproval { get; set; }
        [DisplayName("验证审核")]
        public string ValidateAuditing { get; set; }
        [DisplayName("投报人")]
        public string ValidateForm { get; set; }
        [DisplayName("验证日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ValiDateTime { get; set; }

        #region 生效时间
        public int EntryTime { get; set; }

        #endregion

        #region 通报及会签部门
        [DisplayName("工程部")]
        public bool Department1 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign1 { get; set; }
       

        [DisplayName("生产部")]
        public bool Department2 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign2 { get; set; }

        [DisplayName("业务部")]
        public bool Department3 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign3 { get; set; }


        [DisplayName("品质部")]
        public bool Department4 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign4 { get; set; }

        [DisplayName("资材部")]
        public bool Department5 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign5 { get; set; }

        [DisplayName("仓储部")]
        public bool Department6 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign6 { get; set; }

        [DisplayName("经理室")]
        public bool Department7 { get; set; }
        [DisplayName("会签或通知")]
        public string NoticeOrJointlySign7 { get; set; }

        [DisplayName("客户确认")]
        public bool CustomerValidate { get; set; }
        #endregion


        #region 发行
        [DisplayName("发行部门")]
        public string ReleaseDept { get; set; }
        [DisplayName("发行时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseDeptDate { get; set; }


        [DisplayName("发行人")]
        public string ReleaseEmp { get; set; }
        [DisplayName("发行时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseEmpDate { get; set; }


        [DisplayName("确认")]
        public string ReleaseAudit { get; set; }
        [DisplayName("确认时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseAuditDate { get; set; }


        [DisplayName("批准")]
        public string ReleaseApproval { get; set; }
        [DisplayName("批准时间")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ReleaseApprovalDate { get; set; }

        #endregion

    }
}
