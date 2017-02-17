using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Framework.Data.Repository;

namespace Carlzhu.Iooin.Business.BaseModule
{
    public class FilesBll : RepositoryFactory<Files>
    {
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="fileMd5"></param>
        /// <returns></returns>
        public Carlzhu.Iooin.Entity.FORM.Files SignFiles(string fileMd5)
        {
            return base.Repository().FindEntity(fileMd5);
        }
    }
}
