using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AngleSharp;
using EhentaiDownloader.DataBaseService;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;
using EhentaiDownloader.Tools;


namespace EhentaiDownloader.Services
{
    class DataScrapingService
    {
        private static IEbookParser parser;

        public static async Task StartDownload(string url1, string url2)
        {
            parser = new AllitebooksParser();
            List<string> pageList = buildAllUrls(url1, url2);

            List<EbookPageModel> webPages = new List<EbookPageModel>();
            foreach (string pageUrl in pageList)
            {
                webPages.AddRange(await parser.FindEbookPage(pageUrl));
                Debug.WriteLine(webPages.Count());
                foreach(EbookPageModel page in webPages)
                {
                    DelegateCommands.AddToEbookCollection?.Invoke(page);
                }
            }

            foreach (EbookPageModel webPage in webPages)
            {
                Debug.WriteLine($"------开始尝试解析下载{webPage.Url}-----");
                await ParseAndDownload(webPage);
            }

            //for (int i = 0; i < taskItems.Count; i++)
            //{
            //    await downloadATask(taskItems[i]);
            //}
            //SoundPlayer.PlayCompleteness();
            Debug.WriteLine("Finish");
        }

        static int RecordCount = 0;
        static int DownloadFileCount = 0;

        private static async Task ParseAndDownload(EbookPageModel webpage)
        {
            EbookPageModel page = await parser.FindEbookUrl(webpage);
            //下载图书文件
            int success = 0;
            foreach (EbookFileModel ebook in page.EBooks)
            {
                try
                {
                    byte[] file = await HttpDownloader.DownloadBytes(ebook.DownloadUrl);
                    FileWriter.WriteToFile(file, ebook.FileSavePath);
                    success++;
                    ebook.IsDownloaded = true;
                    DownloadFileCount++;
                    Debug.WriteLine("下载文件数量=" + DownloadFileCount);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("下载图书文件超时");
                    continue;
                }
                catch(HttpRequestException e)
                {
                    Debug.WriteLine("访问图书下载连接失败：" + e.Message);
                    continue;
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                    continue;
                }
            }
            if (success == 0)
            {
                return;
            }
            //下载图书图片
            try
            {
                byte[] imageBytes = await HttpDownloader.DownloadBytes(page.Image.ImageUrl);
                FileWriter.WriteToFile(imageBytes, page.Image.ImageSavePath);
                //如果图片下载成功，则填入地址，失败则为null
                page.ImagePath = page.Image.ImageSavePath;
            }
            catch (TaskCanceledException)
            {
                Debug.WriteLine("下载图片超时");
            }
            catch (HttpRequestException e)
            {
                Debug.WriteLine("访问图片连接失败：" + e.Message);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            //添加对象属性
            page.FilePaths = string.Join(", ", page.EBooks.Where(x=>x.IsDownloaded).Select(x => x.FileSavePath));
            page.FileFormat = string.Join(", ", page.EBooks.Where(x => x.IsDownloaded).Select(x => x.FileExtention));
            page.FileCount = page.EBooks.Where(x => x.IsDownloaded).Count();
            //写入数据库
            //using (var db=new MyAppDbContext())
            //{
            //    db.Blogs.Add(page);
            //    db.SaveChanges();
            //    RecordCount++;
            //    Debug.WriteLine("写入记录条数=" + RecordCount);
            //}

        }


        /// <summary>
        /// 根据起始和终止URL自动构建中间的URL
        /// </summary>
        /// <param name="url1"></param>
        /// <param name="url2"></param>
        /// <returns></returns>
        private static List<string> buildAllUrls(string url1, string url2)
        {
            if (string.IsNullOrEmpty(url2))
            {
                return new List<string> { url1 };
            }

            //http://www.allitebooks.org/programming/c/page/2/
            string[] url2Parts = url2.Split('/');
            string[] url1Parts = url1.Split('/');
            string url2LastPart = url2Parts[url2Parts.Length - 2];
            string url1LastPart = url1Parts[url1Parts.Length - 2];
            string generalUrl = string.Join("/", url2Parts.Take(url2Parts.Length - 2)) + "/";

            List<string> urls = new List<string>();
            if (url2Parts.Length == url1Parts.Length)
            {
                try
                {
                    int url1Num = int.Parse(url1LastPart);
                    int url2Num = int.Parse(url2LastPart);
                    for (int i = url1Num; i <= url2Num; i++)
                    {
                        urls.Add(generalUrl + i + "/");
                    }
                }
                catch
                {
                    MessageBox.Show("Url解析失败");
                    throw;
                }
            }
            else
            {
                try
                {
                    int url2Num = int.Parse(url2LastPart);
                    for (int i = 1; i <= url2Num; i++)
                    {
                        urls.Add(generalUrl + i + "/");
                    }
                }
                catch
                {
                    MessageBox.Show("Url解析失败");
                    throw;
                }
            }
            return urls;
        }

    }
}
