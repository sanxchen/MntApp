
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
    /// FileGroup
    
    ///		<name>Carlzhu</name>
    ///		<date>2016.08.22 15:00</date>
    
    /// </summary>
    [Description("FileGroup")]
    [PrimaryKey("CreateEmpNo")]
    public class FileGroup : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// GroupGuid
        /// </summary>
        /// <returns></returns>
        [DisplayName("GroupGuid")]
        public string GroupGuid { get; set; }
        /// <summary>
        /// CreateTime
        /// </summary>
        /// <returns></returns>
        [DisplayName("CreateTime")]
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// CreateEmpNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("CreateEmpNo")]
        public string CreateEmpNo { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CreateEmpNo = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.CreateEmpNo = KeyValue;
                                            }
        #endregion
    }
}