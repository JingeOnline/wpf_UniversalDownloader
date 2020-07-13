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

        public async Task<List<ImagePageModel>> FindImagePageUrl(TaskItem taskItem)
        {
            string url = taskItem.Url;
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

            List<ImagePageModel> imagePageModels = new List<ImagePageModel>();
            foreach (var res in result)
            {
                string pageUrl = "https://asmhentai.com"+res.QuerySelector("a").GetAttribute("href");
                imagePageModels.Add(new ImagePageModel(pageUrl, taskItem));
            }
            return imagePageModels;
            //return imagePageUrls.Select(link => "https://asmhentai.com" + link).ToList();
        }

        public async Task<List<ImageModel>> FindImageUrls(ImagePageModel imagePageModel)
        {
            string html = await HttpDownloader.DownloadHtmlPage(imagePageModel.ImagePageUrl);
            
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

            string imageName = document.All.Where(m => m.LocalName == "title").First().Text();
            ImageModel imageModel = new ImageModel(imagePageModel,imageName,imageUrl);
            return new List<ImageModel>() {imageModel};
        }
    }
}
