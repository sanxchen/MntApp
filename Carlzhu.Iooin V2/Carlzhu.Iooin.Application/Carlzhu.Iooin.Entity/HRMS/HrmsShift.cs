using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;
using Carlzhu.Iooin.Entity.BaseUtility;

namespace Carlzhu.Iooin.Entity.HRMS
{

    /// <summary>
    /// 班别设定
    /// </summary>
    [PrimaryKey("ShiftId")]
    public class HrmsShift : BaseEntity
    {
        [Key]
        public string ShiftId { get; set; }
        public string Code { get; set; }

        public DateTime WorkStart { get; set; }
        public DateTime WorkStop { get; set; }




        public DateTime Rist1Start { get; set; }
        public DateTime Rist1Stop { get; set; }


        public DateTime Rist2Start { get; set; }
        public DateTime Rist2Stop { get; set; }

        public DateTime Rist3Start { get; set; }
        public DateTime Rist3Stop { get; set; }


        public int SettlementMin { get; set; }
        public int SettlementMax { get; set; }



        /// <summary>
        /// 岗位名称
        /// </summary>
        /// <returns></returns>
        [DisplayName("岗位名称")]
        public string FullName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        [DisplayName("备注")]
        public string Remark { get; set; }
        /// <summary>
        /// 有效
        /// </summary>
        /// <returns></returns>
        [DisplayName("有效")]
        public int? Enabled { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        [DisplayName("排序码")]
        public int? SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        [DisplayName("删除标记")]
        public int? DeleteMark { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建时间")]
        public DateTime? CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户主键")]
        public string CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("创建用户")]
        public string CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改时间")]
        public DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户主键")]
        public string ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        [DisplayName("修改用户")]
        public string ModifyUserName { get; set; }




    }
}
