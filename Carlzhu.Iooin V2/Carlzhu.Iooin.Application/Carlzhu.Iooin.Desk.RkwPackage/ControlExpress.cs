using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Carlzhu.Iooin.Entity.mes;
using Carlzhu.Iooin.Util;
using LabelManager2;
using DateTime = System.DateTime;

namespace Carlzhu.Iooin.Desk.RkwPackage
{


    public class ControlExpress
    {

        /// <summary>
        /// 打印程序执行信息
        /// </summary>
        /// <param name="text"></param>
        void ConsoleWriteToLabelMsg(string text)
        {
            ShowMessage sm = new ShowMessage(Parameters.Main, "lblMsg");
            sm.ConsoleWriteToLabelMsg(text);
        }


        /// <summary>
        /// 调用codesoft打印标签
        /// </summary>
        /// <param name="print"></param>
        /// <param name="lab"></param>
        /// <param name="dictionary"></param>
        /// <param name="qty"></param>
        void Print(string print, string lab, Dictionary<string, string> dictionary, int qty)
        {

            ApplicationClass lbl = new ApplicationClass();

            // 调用设计好的label文件
            lbl.Documents.Open(lab, false);

            Document doc = lbl.ActiveDocument;

            //给参数传值
            dictionary.ToList().ForEach(c => doc.Variables.Item(c.Key).Value = c.Value.ToString());

            //设置打印机
            doc.Printer.Name = print;

            //打开打印机选择对应框
            if (doc.Printer.Name != print) lbl.Dialogs.Item(enumDialogType.lppxPrinterSelectDialog).Show();

            //打印数量
            doc.PrintDocument(qty);

            //退出
            lbl.Quit();
        }



        /// <summary>
        /// 打印SN条码
        /// </summary>
        /// <param name="sns"></param>
        public void PrintSn(List<MesRkwSn> sns)
        {
            ConsoleWriteToLabelMsg("打印开始....");
            foreach (var item in sns)
            {
                Dictionary<string, string> snLabel = new Dictionary<string, string>()
                            {
                                {"PartNo",item.MesRkwPo.PartNo},
                                {"Partname",item.MesRkwPo.PartName},
                                {"sn",item.Sn},
                                {"mono",item.MesRkwPo.MoNo},
                                {"PartNo1",""},
                                {"Partname1",""},
                                {"sn1",""},
                                {"mono1",""},
                            };
                ConsoleWriteToLabelMsg($"打印SN：{item.Sn}");
                Print(LocalPrint.DefaultPrinter(), Parameters.DbSnLabel, snLabel, 1);
            }
            ConsoleWriteToLabelMsg($"打印完成！");
        }


        /// <summary>
        /// 打印SN条码
        /// </summary>
        /// <param name="sns"></param>
        public void PrintDbSn(List<MesRkwSn> sns)
        {
            ConsoleWriteToLabelMsg("打印开始....");
            for (int i = 0; i < sns.Count; i = i + 2)
            {
                int j = i + 1;
                if (j >= sns.Count) j = i;

                Dictionary<string, string> snLabel = new Dictionary<string, string>()
                            {
                                {"PartNo",sns[i].MesRkwPo.PartNo},
                                {"Partname",sns[i].MesRkwPo.PartName},
                                {"sn",sns[i].Sn},
                                {"mono",sns[i].MesRkwPo.MoNo},

                                {"PartNo1",i==j?"":sns[j].MesRkwPo.PartNo},
                                {"Partname1",i==j?"":sns[j].MesRkwPo.PartName},
                                {"sn1",i==j?"":sns[j].Sn},
                                {"mono1",i==j?"":sns[j].MesRkwPo.MoNo},
                            };

                ConsoleWriteToLabelMsg($"打印SN：{sns[i].Sn}/{sns[j].Sn}");
                Print(LocalPrint.DefaultPrinter(), Parameters.DbSnLabel, snLabel, 1);
            }
            ConsoleWriteToLabelMsg($"打印完成！");
        }

        public void PrintDTryPrint(Dictionary<string, string> snLabel)
        {
            ConsoleWriteToLabelMsg($"打印SN：OK");
            Print(LocalPrint.DefaultPrinter(), Parameters.DbSnLabel, snLabel, 1);
        }




        /// <summary>
        /// 打印Shipping
        /// </summary>
        /// <param name="mespo"></param>
        /// <param name="qty"></param>
        void PrintShippingLabel(MesRkwPo mespo, string qty)
        {
            Dictionary<string, string> shipping = new Dictionary<string, string>()
                {
                    {"cusname",mespo.CusName},
                    {"partno",mespo.PartNo},
                    {"partname",mespo.PartName},
                    {"recaropono",mespo.RecaroPoNo},
                    {"bacthno",mespo.MoNo},
                    {"supplierno",mespo.SupplierNo},
                    {"date",DateTime.Now.ToString("yyyy/MM")},
                    {"qty",qty},
                };
            ConsoleWriteToLabelMsg($"正在打印shipping标签...");
            Print(LocalPrint.DefaultPrinter(), Parameters.ShippingLabel, shipping, 1);

            ConsoleWriteToLabelMsg($"shipping标签打印完成！");
        }


        void PrintPackageLabel(MesRkwPo mespo, MesRkwBox mesbox, string qty, int PrintQty = 1)
        {

            Dictionary<string, string> packageDictionary = new Dictionary<string, string>()
                {
                    {"PartNo",mespo.PartNo},
                    {"Partname",mespo.PartName},
                    {"RecaroPONO",mespo.RecaroPoNo},
                    {"MONO",mespo.MoNo},
                    {"HeatNO",mespo.HeatNo},
                    {"SupplierCode","16253"},
                    {"Date",DateTime.Now.ToString("yyyy/MM")},
                    {"CTNNO",mesbox.BoxId},
                    {"Qty",qty},
                };
            ConsoleWriteToLabelMsg($"正在打印PackageBox标签,箱号：{mesbox.BoxId}");
            //列印大标签1
            Print(LocalPrint.DefaultPrinter(), Parameters.PackageLabel, packageDictionary, PrintQty);
            ConsoleWriteToLabelMsg($"PackageBox标签打印完成！");
        }

        void PrintTraceableTagsLabel(MesRkwPo mespo, string qty)
        {
            Dictionary<string, string> traceable = new Dictionary<string, string>()
                {
                    {"supplier",mespo.Supplier},
                    {"partno",mespo.PartNo},
                    {"partname",mespo.PartName},
                    {"minicutpono",mespo.MinicutPoNo},
                    {"recaropono",mespo.RecaroPoNo},
                    {"mono",mespo.MoNo},
                    {"heatno",mespo.HeatNo},
                    {"bacthno",mespo.MoNo},
                    { "Date",DateTime.Now.ToString("yyyy/MM")},
                    {"Qty",qty},
                };
            ConsoleWriteToLabelMsg($"正在打印traceable标签");
            //列印大标签1
            Print(LocalPrint.DefaultPrinter(), Parameters.TraceableTagsLabel, traceable, 1);
            ConsoleWriteToLabelMsg($"traceable标签打印完成");
        }

        /// <summary>
        /// 打印盘点标签
        /// </summary>
        /// <param name="checkWhares"></param>
        public void PrintCkWh(List<CheckWhare> checkWhares)
        {
            ConsoleWriteToLabelMsg("打印开始....");
            for (int i = 0; i < checkWhares.Count; i = i + 2)
            {
                int j = i + 1;
                if (j >= checkWhares.Count) j = i;

                Dictionary<string, string> snLabel = new Dictionary<string, string>()
                            {
                                {"Location",checkWhares[i].Location},
                                {"PartName",checkWhares[i].PartName},
                                {"PartNo",checkWhares[i].PartNo},
                                {"Qty",checkWhares[i].Qty},

                                {"Location1",i==j?"":checkWhares[j].Location},
                                {"PartName1",i==j?"":checkWhares[j].PartName},
                                {"PartNo1",i==j?"":checkWhares[j].PartNo},
                                {"Qty1",i==j?"":checkWhares[j].Qty},
                            };

                ConsoleWriteToLabelMsg($"打印SN：{checkWhares[i].PartName}/{checkWhares[j].PartName}");
                Print(LocalPrint.DefaultPrinter(), Parameters.CheckWarehouse, snLabel, 1);
            }

            ConsoleWriteToLabelMsg($"打印完成！");
        }


        public void PrintBoxLabel(MesRkwPo mespo, MesRkwBox mesbox, string qty)
        {
            if (mespo.BoxLabelQty == 1)
            {
                PrintPackageLabel(mespo, mesbox, qty, 1);
            }
            else
            {
                PrintTraceableTagsLabel(mespo, qty);
                PrintShippingLabel(mespo, qty);
            }

        }
    }
}
