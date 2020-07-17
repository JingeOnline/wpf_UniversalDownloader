using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            MultiTaskTest().Wait();
            //testShzxParserAsync().Wait();
        }

        public static async Task testShzxParserAsync()
        {
            ShzxParser parser = new ShzxParser();
            string html = File.ReadAllText("./HtmlPages/HTMLPage1.html");
            //List<ImageModel> imageModels= await parser.test(html);
        }

        public async static Task MultiTaskTest()
        {
            List<int> list = new List<int>();
            list.AddRange(Enumerable.Range(0, 1000000));

            List<int> newList = new List<int>();
            Object lockobj = new Object();

            List<Task> tasks = new List<Task>();
            var limitation = new SemaphoreSlim(10);
            foreach (int i in list)
            {
                await limitation.WaitAsync();
                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        lock (lockobj)
                        {
                        newList.Add(i);
                        }
                    }

                    finally
                    {
                        limitation.Release();
                    }
                }));
            }


            Console.WriteLine(list.Count);
            Console.WriteLine(newList.Count);
            Console.ReadKey();
        }

        //public static void Task_0(List<int> list)
        //{
        //    for(int i=0;i<=10000 ; i++)
        //    {
        //        list.Add(i);
        //    }
        //}
        //public static void Task_1(List<int> list)
        //{
        //    for (int i = 0; i <= 10000; i++)
        //    {
        //        list.Add(i);
        //    }
        //}
        //public static void Task_2(List<int> list)
        //{
        //    for (int i = 0; i <= 10000; i++)
        //    {
        //        list.Add(i);
        //    }
        //}
    }
}
