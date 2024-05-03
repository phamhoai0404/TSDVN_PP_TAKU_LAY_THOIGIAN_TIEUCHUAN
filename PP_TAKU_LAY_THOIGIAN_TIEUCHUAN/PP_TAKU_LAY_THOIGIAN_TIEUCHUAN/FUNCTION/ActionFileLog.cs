using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PP1_HIEUSUAT.FUNCTION
{
    internal class ActionFileLog
    {
        public static void WriteResultOKNG(string pathFile, string text)
        {
            using (StreamWriter writer = File.AppendText(pathFile))
            {
                writer.WriteLine(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + ";" + text);
            }
        }
    }
}
