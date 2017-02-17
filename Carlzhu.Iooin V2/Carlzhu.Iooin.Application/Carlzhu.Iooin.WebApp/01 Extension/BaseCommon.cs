using Carlzhu.Iooin.Business.CommonModule;
using Carlzhu.Iooin.Util;


namespace Carlzhu.Iooin.WebApp
{
    /// <summary>
    /// 通用方法
    /// </summary>
    public class BaseCommon
    {
        /// <summary>
        /// 拼接表单（返回html）
        /// </summary>
        /// <param name="columnCount"></param>
        /// <returns></returns>
        public static string CreateBuildForm(int columnCount)
        {
            string moduleId = DESEncrypt.Decrypt(WebHelper.GetCookie("ModuleId"));
            return CreateBuildForm(columnCount, moduleId);
        }
        public static string CreateBuildForm(int columnCount, string moduleId)
        {
            BaseFormAttributeBll baseformattributebll = new BaseFormAttributeBll();
            return baseformattributebll.CreateBuildFormTable(columnCount, moduleId);
        }
    }
}