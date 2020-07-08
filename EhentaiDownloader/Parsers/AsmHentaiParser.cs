using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AngleSharp;
using AngleSharp.Dom;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;

namespace EhentaiDownloader.Tools
{
    class AsmHentaiParser: IWebpageParser
    {
        //private static string albumTitle;
        private static string saveFolderPath;
        private static void setSaveFolderPath()
        {
            saveFolderPath = DelegateCommands.GetFolderPath?.Invoke();
        }

        public async Task<List<string>> FindImagePageUrl(string url)
        {
            
            if (url[url.Length - 1] != '#')
            {
                url += "#";
            }
            string html = await HttpDownloader.DownloadHtmlPage(url);

            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(response => response.Content(html));
            var result = document.All.Where(m => m.LocalName == "div" && m.ClassName == "preview_thumb");
            Debug.WriteLine("成功：" + "找到图片网页" + result.Count() + "个");

            List<string> imagePageUrls = new List<string>();
            foreach (var res in result)
            {
                imagePageUrls.Add(res.QuerySelector("a").GetAttribute("href"));
            }

            return imagePageUrls.Select(link => "https://asmhentai.com" + link).ToList();
        }

        public async Task<ImageModel> FindImageUrl(string url)
        {
            setSaveFolderPath();
            string html = await HttpDownloader.DownloadHtmlPage(url);
            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(response => response.Content(html));
            IEnumerable<IElement> urlResult = document.All.Where(m => m.LocalName == "img" && m.ClassName == "lazy no_image");
            string imageUrl;
            if (urlResult.Count() == 0)
            {
                urlResult = document.All.Where(m => m.LocalName == "div" && m.ClassName == "image_1");

                if (urlResult.Count() == 0)
                {
                    Debug.WriteLine("失败："+"解析失败，没有找到图片链接。");
                    throw new Exception("失败：" + "解析失败，没有找到图片链接。");
                }
                else
                {
                    imageUrl = "https://" + urlResult.First().QuerySelector("img").GetAttribute("src").Remove(0, 2);
                }
            }
            else
            {
                imageUrl = "https://" + urlResult.First().GetAttribute("src").Remove(0, 2);
            }

            var nameResult = document.All.Where(m => m.LocalName == "title");
            string imageName = nameResult.First().Text();

            imageName = FileWriter.FileNameCheck(imageName);
            ImageModel image = new ImageModel
            {
                ImageName = imageName,
                ImageUrl = imageUrl,
                ImagePageUrl = url,
                ImageSavePath = Path.Combine(saveFolderPath, imageName + ".jpg")
            };
            return image;
        }
    }
}
