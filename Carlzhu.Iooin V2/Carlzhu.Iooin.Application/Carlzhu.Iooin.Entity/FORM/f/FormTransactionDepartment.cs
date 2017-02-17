using System;
using System.ComponentModel;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    public class FormTransactionDepartment : F
    {

        [DisplayName("异动种类")]
        public int TransactionType { get; set; }

        [DisplayName("异动人员")]
        public string TransactionEmp { get; set; }

        [DisplayName("当前部门")]
        public string CurrentDepatment { get; set; }

        [DisplayName("启用时间")]
        public DateTime EnableDateTime { get; set; }

        [DisplayName("之前部门")]
        public string BeforedDepart { get; set; }

        [DisplayName("之前职等")]
        public string BeforePosition { get; set; }

        [DisplayName("转至部门")]
        public string AfterDepart { get; set; }

        [DisplayName("转至职等")]
        public string AfterPosition { get; set; }


        [DisplayName("以前部门意见")]
        public string BeforeDepartmentMark { get; set; }
        [DisplayName("接收部门意见")]
        public string AfterDepartmentMark { get; set; }
        [DisplayName("行政意见")]
        public string HrDepartmentMark { get; set; }
        [DisplayName("总经理意见")]
        public string ManagerDepartmentMark { get; set; }



    }
}
