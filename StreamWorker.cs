using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace AffdexMe
{
    class StreamWorker
    {
        static string path = @"C:\Users\root\Desktop\Emotions.txt";

        public void write(string text)
        {
            using (StreamWriter sw = new StreamWriter(path, true, System.Text.Encoding.Default))
            {
                sw.WriteLine(text + Environment.NewLine);
            }

        }

        public void writeTrunc(string text)
        {
            using (StreamWriter sw = new StreamWriter(path, false, System.Text.Encoding.Default))
            {
                sw.WriteLine(text);
            }

        }
    }
}
