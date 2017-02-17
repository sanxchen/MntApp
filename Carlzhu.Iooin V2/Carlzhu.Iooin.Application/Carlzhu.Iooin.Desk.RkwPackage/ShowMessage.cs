using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carlzhu.Iooin.Desk.RkwPackage
{
    public delegate void WriteLabelDelegate(object entry);


    public class ShowMessage
    {
        /// <summary>
        /// 信息标签
        /// </summary>
        public static Label LblMsg;

        public ShowMessage(Form form, string label)
        {
            LblMsg = (Label)form.Controls.Find(label, true).First();
        }

        /// <summary>
        /// 写入信息
        /// </summary>
        /// <param name="obj"></param>
        public void WriteMsgToX(object obj)
        {
            var objs = (object[])obj;
            ((Label)objs[1]).Text = $"{((string)objs[0]).ToString()}";
        }

        public void ConsoleWriteToLabelMsg(string text)
        {
            object obj = new object[] { text, LblMsg };

            LblMsg.Invoke(new WriteLabelDelegate(WriteMsgToX), obj);

        }
    }


}
