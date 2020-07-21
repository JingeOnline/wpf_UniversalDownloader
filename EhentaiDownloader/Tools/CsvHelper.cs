using EhentaiDownloader.Delegates;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Tools
{
    /// <summary>
    /// CSV文件转换类
    /// </summary>
    public static class CsvHelper
    {
        public static string FileName =FileWriter.FileNameCheck(DateTime.Now.ToString("s"));
        public static string FilePath;
        public static string SplitChar="@#$";

        public static async Task CreateCsvAsync(List<string> titles)
        {
            FilePath = Path.Combine(DelegateCommands.GetFolderPathCommand?.Invoke(), FileName + ".csv");
            while (File.Exists(FilePath))
            {
                await Task.Delay(1000);
                FilePath= Path.Combine(DelegateCommands.GetFolderPathCommand?.Invoke(), FileName + ".csv");
            }
            //File.Create(filePath);
            string line = string.Join(SplitChar, titles);
            File.WriteAllLines(FilePath, new List<string> { line });
        }
        public static void AppendToCsv(List<string> fields)
        {
            string line = string.Join(SplitChar, fields);
            File.AppendAllLines(FilePath, new List<string> {line});
        }

        public static void DataTableToCSV(DataTable dt, string fileName)
        {
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Create, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                string head = "";
                //拼接列头
                for (int cNum = 0; cNum < dt.Columns.Count; cNum++)
                {
                    head += dt.Columns[cNum].ColumnName + ",";
                }
                //csv文件写入列头
                sw.WriteLine(head);
                string data = "";
                //csv写入数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string data2 = string.Empty;
                    //拼接行数据
                    for (int cNum1 = 0; cNum1 < dt.Columns.Count; cNum1++)
                    {
                        data2 = data2 + "\"" + dt.Rows[i][dt.Columns[cNum1].ColumnName].ToString() + "\",";
                    }
                    bool flag = data != data2;
                    if (flag)
                    {
                        sw.WriteLine(data2);
                    }
                    data = data2;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("导出csv失败！" + ex.Message);
                return;
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
                if (fs != null)
                {
                    fs.Close();
                }
                sw = null;
                fs = null;
            }
        }

    }
}
