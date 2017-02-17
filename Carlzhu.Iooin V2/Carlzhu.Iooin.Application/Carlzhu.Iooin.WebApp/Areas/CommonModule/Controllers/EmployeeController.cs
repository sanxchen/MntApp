using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Entity.CommonModule;
using Carlzhu.Iooin.Util;
using System.Threading.Tasks;

namespace Carlzhu.Iooin.WebApp.Areas.CommonModule.Controllers
{
    public class EmployeeController : PublicController<BaseEmployee>
    {
        // GET: CommonModule/Employee
        readonly BaseEmployeeBll _baseEmployeeBll = new BaseEmployeeBll();

        public ActionResult SetForm114(string keyValue)
        {
            var entities = _baseEmployeeBll.GetListByKeyValue(keyValue);

            return Content(entities.ToJson());

        }

        public override ActionResult SetFormFiled(string propertyName, string propertyValue)
        {
            var entity = Repositoryfactory.Repository().FindList(propertyName, propertyValue).FirstOrDefault(c => c.IsDimission != 0);
            return Content(entity.ToJson());

        }

        public async Task<ContentResult> GetNewEmpNo()
        {
            int count = await Task.Run(() => Repositoryfactory.Repository().FindCount($"AND EMPNO LIKE '{DateTimeHelper.YearMouth}%'"));
            return Content(DateTimeHelper.YearMouth + (count + 1).ToString().PadLeft(3, '0'));

        }
    }
}