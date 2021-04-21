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
using EhentaiDownloader.Exceptions;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;
using System.Text.RegularExpressions;

namespace EhentaiDownloader.Tools
{
    class AsmHentaiParser: IImageParser
    {
        //private static string albumTitle;

        public async Task<List<ImagePageModel>> FindImagePageUrl(TaskItem taskItem)
        {
            //string url = taskItem.Url;
            //if (url[url.Length - 1] != '#')
            //{
            //    url += "#";
            //}
            string html = await HttpDownloader.DownloadHtmlPage(taskItem.Url);

            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(response => response.Content(html));
            //var result = document.All.Where(m => m.LocalName == "div" && m.ClassName == "preview_thumb");
            //去页面中寻找Page元素，该元素包含当前漫画的总页数。
            var result = document.All.Where(m => m.LocalName == "div" && m.ClassName == "pages").FirstOrDefault();
            string pages = result.QuerySelector("h3").TextContent;
            Match pageNum = Regex.Match(pages, "[0-9]+");
            int pageCount = Convert.ToInt32(pageNum.ToString());

            //人工根据漫画页数构造每一页的URL
            List<ImagePageModel> imagePageModels = new List<ImagePageModel>();
            string imagePageUrlBase = taskItem.Url.Replace("g", "gallery");
            for(int i = 1; i <= pageCount; i++)
            {
                string imagePageUrl = Path.Combine(imagePageUrlBase, i.ToString());
                imagePageModels.Add(new ImagePageModel(imagePageUrl, taskItem));
            }
            return imagePageModels;


            //if (result.Count() == 0)
            //{
            //    throw new TargetNotFindException("未找到div.preview_thumb元素");
            //}
            //Debug.WriteLine("成功：" + "找到图片网页" + result.Count() + "个");

            //List<ImagePageModel> imagePageModels = new List<ImagePageModel>();
            //foreach (var res in result)
            //{
            //    string pageUrl = "https://asmhentai.com"+res.QuerySelector("a").GetAttribute("href");
            //    imagePageModels.Add(new ImagePageModel(pageUrl, taskItem));
            //}
            //return imagePageModels;
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
