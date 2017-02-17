using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Carlzhu.Iooin.Entity.TPA;
using Carlzhu.Iooin.Framework.Data.Repository;


namespace Carlzhu.Iooin.Business.TpaModule
{
    public class TpaSupplierBll : RepositoryFactory<TpaSupplier>
    {

        public List<TpaSupplier> GetList()
        {
            return base.Repository().FindList();
        }

        public List<SelectListItem> GetSupplierDropList()
        {
            return this.GetList().Select(c => new SelectListItem
            {
                Text = c.Code + "-" + c.Name,
                Value = c.Code,
            }).ToList();
        }

        public List<SelectListItem> GetDataToSelectListItems(Func<TpaSupplier, bool> lambda)
        {
            return this.GetList().Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Code
            }).ToList();

        }
    }
}
