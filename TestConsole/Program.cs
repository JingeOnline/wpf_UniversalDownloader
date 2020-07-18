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
            //MultiTaskTest().Wait();
            //testShzxParserAsync().Wait();
            multiThreadTest();
            Console.ReadKey();
        }

        public static async Task testShzxParserAsync()
        {
            ShzxParser parser = new ShzxParser();
            string html = File.ReadAllText("./HtmlPages/HTMLPage1.html");
            //List<ImageModel> imageModels= await parser.test(html);
        }

        public static void multiThreadTest()
        {
            //add();

            //object locker = new object();

            Thread t1 = new Thread(new ThreadStart(add));
            Thread t2 = new Thread(new ThreadStart(add));
            Thread t3 = new Thread(new ThreadStart(add));
            Thread t4 = new Thread(new ThreadStart(add));
            Thread t5 = new Thread(new ThreadStart(add));
            t1.Start();
            t2.Start();
            t3.Start();
            t4.Start();
            t5.Start();
            t1.Join();
            t2.Join();
            t3.Join();
            t4.Join();
            t5.Join();
            Console.WriteLine(money);
        }

        static int money=0;

        public static void add()
        {
            for (int i = 0; i < 100000; i++)
            {
                money += 1;
                //Console.WriteLine(money);
            }
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
    }
}
