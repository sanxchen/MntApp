using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.FORM.f
{

    public class FormCreateEventArgs : EventArgs
    {
        public Form Form { get; set; }
    }

    public delegate bool FormCreateEventHander(object sender, FormCreateEventArgs e);




    /// <summary>
    /// 所有表单基类
    /// </summary>
    public abstract class F : BaseEntity, IForm
    {
        public event FormCreateEventHander FormCreate;

        protected virtual bool OnFormCreate(FormCreateEventArgs e)
        {
            FormCreateEventHander handler = FormCreate;
            return handler != null && handler(this, e);
        }

        [Key]
        public int RowId { get; set; }

        [ForeignKey("Form")]
        public string FormNo { get; set; }
        public virtual Form Form { get; set; }


        [DisplayName("备注")]
        public string Mark { get; set; }

        //    public virtual ICollection<FormSign> FormSigns { get; set; }



        public bool CreateForm(FormCreateEventArgs e)
        {
            return this.OnFormCreate(e);
        }

    }


}
