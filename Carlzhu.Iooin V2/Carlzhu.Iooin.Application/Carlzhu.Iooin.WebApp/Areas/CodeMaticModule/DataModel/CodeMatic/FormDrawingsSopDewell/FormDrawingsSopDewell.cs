
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
    /// FormDrawingsSopDewell
    
    ///		<name>Carlzhu</name>
    ///		<date>2016.08.22 15:08</date>
    
    /// </summary>
    [Description("FormDrawingsSopDewell")]
    [PrimaryKey("CustomerNo")]
    public class FormDrawingsSopDewell : BaseEntity
    {
        #region 获取/设置 字段值
        /// <summary>
        /// RowId
        /// </summary>
        /// <returns></returns>
        [DisplayName("RowId")]
        public int? RowId { get; set; }
        /// <summary>
        /// DrawPartNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("DrawPartNo")]
        public string DrawPartNo { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        /// <returns></returns>
        [DisplayName("Tag")]
        public string Tag { get; set; }
        /// <summary>
        /// Author
        /// </summary>
        /// <returns></returns>
        [DisplayName("Author")]
        public string Author { get; set; }
        /// <summary>
        /// CustomerNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("CustomerNo")]
        public string CustomerNo { get; set; }
        /// <summary>
        /// ProductNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("ProductNo")]
        public string ProductNo { get; set; }
        /// <summary>
        /// DrawVer
        /// </summary>
        /// <returns></returns>
        [DisplayName("DrawVer")]
        public string DrawVer { get; set; }
        /// <summary>
        /// PageSize
        /// </summary>
        /// <returns></returns>
        [DisplayName("PageSize")]
        public int? PageSize { get; set; }
        /// <summary>
        /// FileGroup
        /// </summary>
        /// <returns></returns>
        [DisplayName("FileGroup")]
        public string FileGroup { get; set; }
        /// <summary>
        /// Reason
        /// </summary>
        /// <returns></returns>
        [DisplayName("Reason")]
        public string Reason { get; set; }
        /// <summary>
        /// IsPublished
        /// </summary>
        /// <returns></returns>
        [DisplayName("IsPublished")]
        public bool? IsPublished { get; set; }
        /// <summary>
        /// Identity
        /// </summary>
        /// <returns></returns>
        [DisplayName("Identity")]
        public string Identity { get; set; }
        /// <summary>
        /// FormNo
        /// </summary>
        /// <returns></returns>
        [DisplayName("FormNo")]
        public string FormNo { get; set; }
        /// <summary>
        /// Mark
        /// </summary>
        /// <returns></returns>
        [DisplayName("Mark")]
        public string Mark { get; set; }
        /// <summary>
        /// IsDel
        /// </summary>
        /// <returns></returns>
        [DisplayName("IsDel")]
        public bool? IsDel { get; set; }
        #endregion

        #region 扩展操作
        /// <summary>
        /// 新增调用
        /// </summary>
        public override void Create()
        {
            this.CustomerNo = CommonHelper.GetGuid;
                                            }
        /// <summary>
        /// 编辑调用
        /// </summary>
        /// <param name="KeyValue"></param>
        public override void Modify(string KeyValue)
        {
            this.CustomerNo = KeyValue;
                                            }
        #endregion
    }
}