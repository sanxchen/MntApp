using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Microsoft.SqlServer.Server;

namespace Carlzhu.Iooin.Entity
{

    [PrimaryKey("RowId")]
    public class EmailLog
    {
        [Key]
        public int RowId { get; set; }
        public string ModelName { get; set; }
        public bool SendResult { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }

        public string Body { get; set; }

        public string Message { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
