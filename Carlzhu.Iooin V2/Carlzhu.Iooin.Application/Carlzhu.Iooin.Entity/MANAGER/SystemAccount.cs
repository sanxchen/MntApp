using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.MANAGER
{
    [PrimaryKey("EmpNo")]
    public class SystemAccount
    {
        [Key]
        [Required]
        public string EmpNo { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DisplayName("旧密码")]
        [MinLength(6,ErrorMessage = "长度不能小于6位"),MaxLength(12,ErrorMessage = "长度要小于12位")]
        public string OldPassword { get; set; }

        [Required]
        [DisplayName("新密码")]
        public string NewPassword { get; set; }
    }
}
