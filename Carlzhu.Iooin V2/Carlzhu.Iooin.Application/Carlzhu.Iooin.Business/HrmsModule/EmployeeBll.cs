using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.Business.HrmsModule
{
    public class EmployeeBll : RepositoryFactory<BaseEmployee>// BaseServices<BaseEmployee>
    {




        /// <summary>
        /// 取得员工信息根据工号
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public BaseEmployee Single(string empNo)
        {
            return ContextFactory.ContextHelper.BaseEmployees.Find(empNo);
            //return base.Repository().FindEntity(empNo);
            //return base.LoadEntities(c => c.EmpNo.Equals(empNo) || c.RealName.Equals(empNo)).First();
        }

        


        /// <summary>
        /// 根据部门取得部门员工信息
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <returns></returns>
        public List<BaseEmployee> GetEmployeesByDepartmentCode(string departmentCode)
        {

            return ContextFactory.ContextHelper.BaseEmployees.Where(c => string.Equals(c.DepartmentId, departmentCode, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }


        /// <summary>
        /// 根据员工信息查找部门成员
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<BaseEmployee> GetEmployeesByEmpNoDepartmentCode(string empNo)
        {
            return this.GetEmployeesByDepartmentCode(this.Single(empNo).DepartmentId);
        }

        /// <summary>
        /// 根据部门查找部门人员列表
        /// </summary>
        /// <param name="departmentCode"></param>
        /// <param name="empNo"></param>
        /// <param name="isInnerMe"></param>
        /// <param name="office"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetDepartmentEmployeesByDpCode(string departmentCode, string empNo, bool isInnerMe, bool office)
        {
            //
            return this.EmployeeToDropdownList(ContextFactory.ContextHelper.BaseEmployees.Where(c =>
                c.DepartmentId.Contains(departmentCode)
                && c.IsDimission != 0 && (office && c.Email != "system@minicut.com.cn")
                && (c.EmpNo.Substring(4, 1) == empNo.Substring(4, 1))
                && (isInnerMe || (c.EmpNo != empNo))
                ).ToList());
        }




        /// <summary>
        /// 将员工转换为dropdown
        /// </summary>
        /// <param name="employees"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> EmployeeToDropdownList(List<BaseEmployee> employees)
        {
            return employees.Select(d => new SelectListItem()
            {
                Text=$"{d.RealName}_{d.EmpNo}",
                //Text = d.Account + " (" + d.RealName + ")",
                Value = d.EmpNo
            }).ToList();
        }




        /// <summary>
        /// 根据工号查找部门员工
        /// </summary>
        /// <param name="empNo"></param>
        /// <param name="isInnerMe"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetDepartmentEmployeesByEmpNo(string empNo, bool isInnerMe)
        {
            //有可能为七位工号
            var patten = new Regex("\\d{7}");

            if (!patten.IsMatch(empNo)) return null;

            var departCode = this.Single(empNo).DepartmentId;
            if (departCode.Contains("000"))
            {
                departCode = departCode.Substring(0, 2);
            }
            else if (departCode.Contains("00"))
            {
                departCode = departCode.Substring(0, 3);
            }

            return new EmployeeBll().EmployeeToDropdownList(ContextFactory.ContextHelper.BaseEmployees.Where(c => c.DepartmentId.Contains(departCode) && c.IsDimission != 0 && (isInnerMe || (c.EmpNo != empNo))).ToList());
        }

        public IEnumerable<SelectListItem> GetScopUser()
        {
            //ManageProvider.Provider.Current().UserId

            List<SelectListItem> sli = new List<SelectListItem>();

            DataTable dt = new BaseDataScopePermissionBll().GetScopeUserList("58e86c4c-8022-4d30-95d5-b3d0eedcc878", "", "");
            var patten = new Regex("\\d{7}");



            foreach (DataRow row in dt.Rows)
            {

                string ss = row["Id"].ToString();
                if (patten.IsMatch(ss))
                {
                    sli.Add(new SelectListItem()
                    {
                        Value = ss,
                        Text = $"{row["FullName"]}_{ss}",
                    });

                }
            }

           
            sli.AddRange(GetDepartmentEmployeesByEmpNo(ManageProvider.Provider.Current().UserId, true));
            
            return sli;
        }

    }
}
