namespace Carlzhu.Iooin.Desk.RkwPackage
{


    public class Parameters
    {
        /// <summary>
        /// 用于信息传送
        /// </summary>
        public static System.Windows.Forms.Form Main { get; set; }


        public static string StartPath = $"{System.Windows.Forms.Application.StartupPath}\\";
        public static string LabelPath => "D:\\templab\\";// System.Windows.Forms.Application.StartupPath + "\\label\\";
        public static string SnLabel => $"{LabelPath}sn.lab";
        public static string PackageLabel => $"{LabelPath}Package.lab";
        public static string ShippingLabel => $"{LabelPath}Shipping.lab";
        public static string TraceableTagsLabel => $"{LabelPath}TraceableTags.lab";

        public static string DbSnLabel => $"{LabelPath}dbsn.lab";

        public static string CheckWarehouse => $"{LabelPath}checkwarehouse.lab";
    }


}
