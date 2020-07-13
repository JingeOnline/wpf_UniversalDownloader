using AngleSharp;
using AngleSharp.Dom;
using EhentaiDownloader.Models;
using EhentaiDownloader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Diagnostics;
using EhentaiDownloader.Delegates;
using System.IO;

namespace EhentaiDownloader.Parsers
{
    class ShzxParser : IWebpageParser
    {
        //static string albumTitle;
        public async Task<List<ImagePageModel>> FindImagePageUrl(TaskItem taskItem)
        {

            //List<string> urls = new List<string>();
            string html = await HttpDownloader.DownloadHtmlPage(taskItem.Url);
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));

            IElement pageNumElement = document.QuerySelector("div.paging");
            string pageNumString = pageNumElement.QuerySelector("a").InnerHtml;
            Match matchNum = Regex.Match(pageNumString, "[0-9]+");
            int maxPageNum = Int32.Parse(matchNum.Value);
            string generalUrl = taskItem.Url.Remove(taskItem.Url.Length - 6);
            List<ImagePageModel> imagePageModels = new List<ImagePageModel>();
            for (int i = 0; i < maxPageNum; i++)
            {
                string url = generalUrl + i + ".html";
                imagePageModels.Add(new ImagePageModel(url, taskItem));
            }
            return imagePageModels;
        }

        public async Task<List<ImageModel>> FindImageUrls(ImagePageModel imagePageModel)
        {

            string html = await HttpDownloader.DownloadHtmlPage(imagePageModel.ImagePageUrl);
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));

            List<ImageModel> imageModels = new List<ImageModel>();
            IElement titleElement = document.QuerySelector("title");
            string albumTitle = titleElement.InnerHtml;

            IElement divElement = document.QuerySelector("div.picture");
            var imageElements = divElement.QuerySelectorAll("img");
            if (imageElements.Count() == 0)
            {
                Debug.WriteLine("在图片页面hmtl中没有匹配到图片url元素: "+imagePageModel.ImagePageUrl);
                throw new Exception("在图片页面hmtl中没有匹配到图片url元素");
            }
            

            IElement pageNumElement = document.QuerySelector("div.paging");
            string pageNum = pageNumElement.QuerySelector("b").InnerHtml;

            int index = 1;
            foreach (IElement imgElement in imageElements)
            {
                string imageName;
                string url = imgElement.GetAttribute("src");
                if (index > 1)
                {
                    imageName = albumTitle + "_" + pageNum + "_" + index;
                }
                else
                {
                    imageName = albumTitle + "_" + pageNum;
                }
                ImageModel imageModel = new ImageModel(imagePageModel, imageName, url);
                index++;

                imageModels.Add(imageModel);
            }
            return imageModels;
        }
    }
}
