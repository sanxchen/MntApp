
// All Rights Reserved , Copyright @ Iooin 2016
// Software Developers @ Iooin 2016


using Carlzhu.Iooin.DataAccess.Attributes;
using Carlzhu.Iooin.Utilities;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Carlzhu.Iooin.Entity
{
    /// <summary>
    /// DoorInout
    
    ///		<name>Carlzhu</name>
    ///		<date>2016.08.22 14:58</date>
    
    /// </summary>
    [Description("DoorInout")]
    [PrimaryKey("Record")]
    public class DoorInout : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// RowId
        /// </summary>
        /// <returns></returns>
        [DisplayName("RowId")]
        public int? RowId { get; set; }
        /// <summary>
        /// CardNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("CardNo")]
        public string CardNo { get; set; }
        /// <summary>
        /// Forward
        /// </summary>
        /// <returns></returns>
        [DisplayName("Forward")]
        public int? Forward { get; set; }
        /// <summary>
        /// EventTime
        /// </summary>
        /// <returns></returns>
        [DisplayName("EventTime")]
        public DateTime? EventTime { get; set; }
        /// <summary>
        /// Record
        /// </summary>
        /// <returns></returns>
        [DisplayName("Record")]
        public string Record { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.Record = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.Record = KeyValue;
                                            }
        #endregion
    }
}