using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.mes
{
    [PrimaryKey("ManNo")]
    public class MesRkwPackageView : MesRkwPo
    {
        public string Sn { get; set; }

    }
}
