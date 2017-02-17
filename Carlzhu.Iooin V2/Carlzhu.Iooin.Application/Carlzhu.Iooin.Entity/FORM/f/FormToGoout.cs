using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormToGoout : Involving
    {
        public FormToGoout()
        {
            InvolvingUser = Guid.NewGuid();
        }


        //[Required]
        //[DisplayName("外出人员")]
        //public string OutEmployees { get; set; }

        [Required]
        [DisplayName("外出员因")]
        public string Reason { get; set; }


        [Required]
        [DisplayName("目的地")]
        public string Destination { get; set; }



        [DisplayName("外出时间")]
        public DateTime OutTime { get; set; }

        [DisplayName("预计回来时间")]
        public DateTime InTime { get; set; }



        //public virtual ICollection<FormInvolvingUser> FormInvolvingUsers { get; set; }
    }
}
