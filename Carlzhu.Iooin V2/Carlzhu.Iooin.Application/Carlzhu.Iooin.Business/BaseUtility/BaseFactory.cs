using System;

namespace Carlzhu.Iooin.Business.BaseUtility
{
    /// <summary>
    /// 定义通用的功能工厂
    /// </summary>
    public class BaseFactory
    {
        /// <summary>
        /// 对象用于锁
        /// </summary>
        private static readonly Object Locker = new Object();
        private static readonly BaseManager Basemanager = null;

   

        /// <summary>
        /// 通用操作
        /// </summary>
        /// <returns></returns>
        public static IBaseManager BaseHelper()
        {
            if (Basemanager == null)
            {
                return new BaseManager();
            }
            lock (Locker)
            {
                return Basemanager;
            }
        }

       

    }
}
