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
                Debug.WriteLine("成功:" + "文件" + filePath + "写入成功");
            }
            catch (Exception e)
            {
                Debug.WriteLine("文件" + filePath + "写入失败");
                Debug.WriteLine("写入失败原因" + e.Message);
                throw e;
            }

        }
    }
}
