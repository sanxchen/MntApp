using System;

namespace Carlzhu.Iooin.Entity.BaseUtility
{
    public class TableShowAttribute : Attribute
    {
        private int _v;

        public TableShowAttribute(int v)
        {
            this._v = v;
        }
    }
}
