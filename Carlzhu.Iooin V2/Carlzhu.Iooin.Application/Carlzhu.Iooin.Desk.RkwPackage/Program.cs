using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Carlzhu.Iooin.Desk.RkwPackage
{
    static class Program
    {



        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
            //自定义注册错误

        }
    }

    public class CheckWhare
    {
        public string Location { get; set; }
        public string PartNo { get; set; }
        public string PartName { get; set; }
        public string Qty { get; set; }

    }


}
