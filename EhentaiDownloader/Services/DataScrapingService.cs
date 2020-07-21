using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AngleSharp;
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
            List<string> titles = new List<string>()
            {
                "FileName","FileSavePath","DownloadUrl",
                "Title","SubTitle","Author","ISBN","Year","Pages",
                "Language","FileSize","FileFormat","Category","Description"
            };
            await CsvHelper.CreateCsvAsync(titles);
            parser = new AllitebooksParser();
            List<string> pageList = buildAllUrls(url1, url2);

            List<WebPageModel> webPages = new List<WebPageModel>();
            foreach (string pageUrl in pageList)
            {
                webPages.AddRange(await parser.FindEbookPage(pageUrl));
                Debug.WriteLine(webPages.Count());
            }

            foreach (WebPageModel webPage in webPages)
            {
                Debug.WriteLine("-----------");
                await ParseAndDownload(webPage);
            }

            //for (int i = 0; i < taskItems.Count; i++)
            //{
            //    await downloadATask(taskItems[i]);
            //}
            //SoundPlayer.PlayCompleteness();
            Debug.WriteLine("Finish");
        }

        private static async Task ParseAndDownload(WebPageModel webpage)
        {
            List<EbookModel> ebooks = await parser.FindEbookUrl(webpage);
            foreach (EbookModel ebook in ebooks)
            {
                try
                {
                    byte[] file = await HttpDownloader.DownloadBytes(ebook.DownloadUrl);
                    FileWriter.WriteToFile(file, ebook.FileSavePath);
                    List<string> fields = new List<string>()
                        {
                        ebook.FileName,ebook.FileSavePath,ebook.DownloadUrl,
                        ebook.WebPageModel.Title,ebook.WebPageModel.SubTitle,ebook.WebPageModel.Author,
                        ebook.WebPageModel.ISBN,ebook.WebPageModel.Year,ebook.WebPageModel.Pages,
                        ebook.WebPageModel.Language,ebook.WebPageModel.FileSize,
                        ebook.WebPageModel.FileFormat,ebook.WebPageModel.Category,ebook.WebPageModel.Description
                        };
                    CsvHelper.AppendToCsv(fields);
                }
                catch (TaskCanceledException)
                {
                    Debug.WriteLine("下载文件超时");
                    continue;
                }
                catch(Exception e)
                {
                    Debug.WriteLine(e.Message);
                }

            }
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
