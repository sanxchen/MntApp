using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Entity.TPA;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.TpaModule
{
    public class TpaCustomerBll : RepositoryFactory<TpaCustomer>
    {

        public List<TpaCustomer> GetList()
        {
            return base.Repository().FindList();
        }


        public List<SelectListItem> GetCustomerDropList(string selectIndex)
        {


            return this.GetList().Select(c => new SelectListItem
            {
                Text = c.CustomerNo + "-" + c.CustomerName,
                Value = c.CustomerNo,
                Selected = c.CustomerNo == selectIndex,
            }).ToList();
        }

        public string GetCustomerNameByNo(string customerNo)
        {
            return base.Repository().FindEntity(customerNo).CustomerName;
        }
    }
}
