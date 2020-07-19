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
using EhentaiDownloader.Exceptions;

namespace EhentaiDownloader.Parsers
{
    public class ShzxParser : IWebpageParser
    {
        //static string albumTitle;
        public async Task<List<ImagePageModel>> FindImagePageUrl(TaskItem taskItem)
        {

            //List<string> urls = new List<string>();
            string html;
            try
            {
                html = await HttpDownloader.DownloadHtmlPage(taskItem.Url);
            }
            catch
            {
                throw;
            }
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));


            IElement pageNumElement = document.QuerySelector("div.paging");
            if (pageNumElement == null)
            {
                throw new TargetNotFindException("无法在HTML中找到div.paging");
            }
            string pageNumString;
            try
            {
                pageNumString = pageNumElement.QuerySelector("a").InnerHtml;
            }
            catch
            {
                throw new TargetNotFindException("无法在HTML中找到a标签");
            }
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
            string albumTitle = titleElement?.InnerHtml;

            IElement divElement = document.QuerySelector("div.picture");
            var imageElements = divElement.QuerySelectorAll("img");
            if (imageElements == null || imageElements.Count() == 0)
            {
                Debug.WriteLine("在图片页面hmtl中没有匹配到图片url元素: " + imagePageModel.ImagePageUrl);
                throw new Exception("在图片页面hmtl中没有匹配到图片url元素");
            }

            string pageNum = "";
            IElement pageNumElement = document.QuerySelector("div.paging");
            pageNum = pageNumElement?.QuerySelector("b")?.InnerHtml;



            int index = 0;
            foreach (IElement imgElement in imageElements)
            {
                string imageName;
                string url = imgElement.GetAttribute("src");
                if (index > 0)
                {
                    imageName = albumTitle + "_" + pageNum + "_" + index;
                }
                else
                {
                    imageName = albumTitle + "_" + pageNum;
                }
                ImageModel imageModel = new ImageModel(imagePageModel, imageName, url);
                index++;

                Debug.WriteLine("成功:找到图片" + imageModel.ImageUrl + imageModel.ImageName + imageModel.ImagePage.ImagePageUrl);
                imageModels.Add(imageModel);
            }
            return imageModels;
        }

    }
}
