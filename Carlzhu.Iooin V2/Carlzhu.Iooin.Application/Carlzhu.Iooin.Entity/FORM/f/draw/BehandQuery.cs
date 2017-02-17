using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.QUALITY;

namespace Carlzhu.Iooin.Entity.FORM.f.draw
{
    public class BehandQuery<T> where T : DrawingsBase
    {
        public Published Published { get; set; }

        public T t { get; set; }


    }
}
