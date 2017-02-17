using System.ComponentModel;
using Carlzhu.Iooin.Framework.Data.DataAccess.Attributes;

namespace Carlzhu.Iooin.Entity.FORM.f
{
    [PrimaryKey("RowId")]
    public class FormItEquipment : F
    {
        //computer
        [DisplayName("台式机")]
        public bool Computer { get; set; }

        [DisplayName("笔记本")]
        public bool NoteBook { get; set; }
        [DisplayName("主机")]
        public bool CHost { get; set; }
        public bool CPower { get; set; }
        [DisplayName("主板")]
        public bool CMainBoard { get; set; }
        [DisplayName("CPU")]
        public bool CCpu { get; set; }
        [DisplayName("硬盘")]
        public bool CHd { get; set; }

        [DisplayName("内存")]
        public bool CMemory { get; set; }

        [DisplayName("显示器")]
        public bool CDisplay { get; set; }

        [DisplayName("键盘")]
        public bool CKeyBoard { get; set; }

        [DisplayName("鼠标")]
        public bool CMouse { get; set; }




        public bool COther { get; set; }

        //Print
        [DisplayName("扫描仪")]
        public bool Scanner { get; set; }

        [DisplayName("投影仪")]
        public bool Projector { get; set; }

        [DisplayName("彩色打印机")]
        public bool PrintColor { get; set; }

        [DisplayName("针式打印机")]
        public bool PrintNeedle { get; set; }

        [DisplayName("斑马打印机")]
        public bool PrintZebra { get; set; }

        [DisplayName("激光打印机")]
        public bool PrintLaser { get; set; }

        [DisplayName("打印一体机")]
        public bool PrintScan { get; set; }

        [DisplayName("硒鼓")]
        public bool PCartridge { get; set; }
        [DisplayName("墨盒")]
        public bool PInkBox { get; set; }

        [DisplayName("碳粉")]
        public bool PToner { get; set; }

        [DisplayName("碳带")]
        public bool PRibbon { get; set; }
        public bool PrintOther { get; set; }

        //


        //网络器材
        [DisplayName("交换机")]
        public bool NHub { get; set; }

        [DisplayName("路由器")]
        public bool NRouter { get; set; }

        [DisplayName("网线")]
        public bool NCable { get; set; }

        public bool NOther { get; set; }


        [DisplayName("电话")]
        public bool TelePhone { get; set; }
        [DisplayName("电话线")]
        public bool TeCable { get; set; }


        //Other
        [DisplayName("U盘类")]
        public bool OUsb { get; set; }

        [DisplayName("读卡器类")]
        public bool OReadeCard { get; set; }


        [DisplayName("打印纸")]
        public bool PrintPaper { get; set; }

        [DisplayName("其他")]
        public bool OOther { get; set; }

    }
}
