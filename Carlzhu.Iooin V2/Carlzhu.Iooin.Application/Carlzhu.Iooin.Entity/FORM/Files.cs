using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM
{
    [PrimaryKey("Md5")]
    public class Files
    {
        public Files()
        {
            this.CreateTime = DateTime.Now;
        }

        [Key]
        public string Md5 { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }

        public string FileType { get; set; }
        public string ContentType { get; set; }
        public DateTime CreateTime { get; set; }

        public string Discriminator { get; set; }

        /// <summary>
        /// 当前文件参与的集合
        /// </summary>
        public virtual ICollection<FileGroup> FileGroups { get; set; }
    }



}
