using EhentaiDownloader.Delegates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Tools
{
    class HttpDownloader
    {
        //这里不知道怎么回事，想在这里初始化HttpClient。第一次调用DonwloadHtmlPage正常，第二次调用就报错。而且找不到为什么，异常也无法捕获。
        //private static HttpClient httpClient = new HttpClient();
        private static HttpClient httpClient;

        private static TimeSpan timeOut = new TimeSpan(0, 0, 60);
        public static void SetTimeOut(int second)
        {
            timeOut = new TimeSpan(0,0,second);
            Debug.WriteLine("成功:" + "设定TimeOut=" + timeOut.TotalSeconds+"秒");
        }
        public static int GetTimeOut()
        {
            return (int)timeOut.TotalSeconds;
        }


        public async static Task<string> DownloadHtmlPage(string uri)
        {
            //Debug.WriteLine("----------------");
            httpClient = new HttpClient();
            httpClient.Timeout = timeOut;
            try
            {
                string htmlText = await httpClient.GetStringAsync(uri);
                //Debug.WriteLine("成功:" + uri + " 页面html获取成功");
                return htmlText;
            }
            catch(Exception e)
            {
                Debug.WriteLine(uri+ " 页面html获取失败" + e.Message);
                throw;
            }
        }

        public async static Task<string> DownloadHtmlPage(string uri,TimeSpan timeSpan)
        {
            httpClient = new HttpClient();
            httpClient.Timeout = timeSpan;
            try
            {
                string htmlText = await httpClient.GetStringAsync(uri);
                //Debug.WriteLine("成功:" + uri + " 页面html获取成功");
                return htmlText;
            }
            catch (Exception e)
            {
                Debug.WriteLine(uri + " 页面html获取失败" + e.Message);
                throw;
            }
        }

        public async static Task<Byte[]> DownloadBytes(string uri)
        {
            httpClient = new HttpClient();
            httpClient.Timeout = timeOut;
            try
            {
                byte[] file = await httpClient.GetByteArrayAsync(uri);
                //Debug.WriteLine("成功:"+uri + "二进制文件获取成功");
                DelegateCommands.AddImageDownloadSizeCommand?.Invoke((long)file.Length);
                return file;
            }
            catch(Exception e)
            {
                Debug.WriteLine(uri + "二进制文件获取失败" + e.Message);
                throw;
            }
        }

        public async static Task<Byte[]> DownloadBytes(string uri,TimeSpan timeSpan)
        {
            httpClient = new HttpClient();
            httpClient.Timeout = timeSpan;
            try
            {
                byte[] file = await httpClient.GetByteArrayAsync(uri);
                //Debug.WriteLine("成功:"+uri + "二进制文件获取成功");
                return file;
            }
            catch (Exception e)
            {
                Debug.WriteLine(uri + "二进制文件获取失败" + e.Message);
                throw;
            }
        }


    }
}
