using System;
using System.Collections.Generic;
using System.Linq;
using Carlzhu.Iooin.Business.BaseUtility;
using Carlzhu.Iooin.Entity.BaseUtility;
using Carlzhu.Iooin.Entity.FORM;
using Carlzhu.Iooin.Framework.Data.Repository;


namespace Carlzhu.Iooin.Business.BaseModule
{
    public class FilesFileGroupBll : RepositoryFactory<FilesFileGroup>
        // : BaseServices<Carlzhu.Iooin.Entity.FORM.FilesFileGroup>
    {

        public List<Carlzhu.Iooin.Entity.FORM.FilesFileGroup> GetFileListByGroupGuid(Guid groupGuid)
        {


            using (CarlzhuContext context = new CarlzhuContext())
            {
                var s = context.FilesFileGroups.Where(c => c.GroupGuid == groupGuid);
                var sss = s.ToList();
                return sss;
            }







        }



    }
}
