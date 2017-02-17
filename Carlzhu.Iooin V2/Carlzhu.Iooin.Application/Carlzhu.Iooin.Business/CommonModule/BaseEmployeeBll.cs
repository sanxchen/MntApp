using System.Collections.Generic;
using System.Data;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.CommonModule
{
    public class BaseEmployeeBll : RepositoryFactory<BaseEmployee>
    {
        public List<BaseEmployee> GetList()
        {
            return base.Repository().FindList();
        }


        public List<BaseEmployee> GetListIsDimission()
        {
            return base.Repository().FindList("AND isDimission!=0");
        }
        public List<BaseEmployee> GetListByWheresql(string wheresql)
        {
            return base.Repository().FindList(wheresql);
        }

        public BaseEmployee Single(string empNo)
        {
            return base.Repository().FindEntity(empNo);
        }


        public DataTable GetListByKeyValue(string keyValue)
        {
            return base.Repository().FindTableBySql($"SELECT EmpNo AS 工号,RealName AS 姓名,Account AS 英文名,Gender AS 性别,CardNo AS 卡号,Email AS  邮箱,[Identity] AS  人员性质,DepartmentId AS  部门,Mobile AS  手机,officeCornet AS  办公电话,ManagerId AS 主管ID,Manager AS 主管姓名 FROM BASEEMPLOYEE  WHERE  EmpNo  = '{keyValue}' or RealName = '{keyValue}' or Account = '{keyValue}' or Email = '{keyValue}' or Mobile = '{keyValue}' or OfficeCornet = '{keyValue}'");}

    }
}
