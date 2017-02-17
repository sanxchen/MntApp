using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Business.BaseUtility
{
    public class ContextFactory
    {

        /// <summary>
        /// 对象用于锁
        /// </summary>
        private static readonly Object Locker = new Object();
        private static readonly CarlzhuContext Context = null;



        /// <summary>
        /// 通用操作
        /// </summary>
        /// <returns></returns>
        public static CarlzhuContext ContextHelper
        {
            get
            {
                if (Context == null)
                {
                    return new CarlzhuContext();
                }
                lock (Locker)
                {
                    return Context;
                }
            }

        }
    }
}
