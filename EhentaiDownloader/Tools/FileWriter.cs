using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Tools
{
    class FileWriter
    {
        public static void WriteToFile(Byte[] bytes, string filePath)
        {
            try
            {
                File.WriteAllBytes(filePath, bytes);
                //Debug.WriteLine("成功:" + "文件" + filePath + "写入成功");
            }
            catch (Exception e)
            {
                Debug.Write("失败:文件" + filePath + "写入失败。"+ "原因:" + e.Message);
                throw e;
            }
        }

        public static string FileNameCheck(string name)
        {
            Char[] unSafeChars = { '*', ':', '\\', '/', '|', '\"', '|', '?', '<', '>' };
            foreach (char c in unSafeChars)
            {
                name = name.Replace(c, '_');
            }
            return name;
        }
    }
}
