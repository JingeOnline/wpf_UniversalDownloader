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
        public async Task<List<string>> FindImagePageUrl(string url)
        {

            List<string> urls = new List<string>();
            string html = await HttpDownloader.DownloadHtmlPage(url);
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));

            IElement pageNumElement = document.QuerySelector("div.paging");
            string pageNumString = pageNumElement.QuerySelector("a").InnerHtml;
            Match matchNum = Regex.Match(pageNumString, "[0-9]+");
            int maxPageNum = Int32.Parse(matchNum.Value);
            string generalUrl = url.Remove(url.Length - 6);
            for (int i = 0; i < maxPageNum; i++)
            {
                urls.Add(generalUrl + i + ".html");
            }
            return urls;
        }

        public async Task<ImageModel> FindImageUrl(ImageModel imageModel)
        {

            string html = await HttpDownloader.DownloadHtmlPage(imageModel.ImagePageUrl);
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));

            IElement titleElement = document.QuerySelector("title");
            string albumTitle = titleElement.InnerHtml;

            IElement urlElement = document.QuerySelector("div.picture").QuerySelector("img");
            imageModel.ImageUrl = urlElement.GetAttribute("src");

            IElement pageNumElement = document.QuerySelector("div.paging");
            string pageNum = pageNumElement.QuerySelector("b").InnerHtml;

            imageModel.ImageName = albumTitle + "_" + pageNum;

            string[] splitResult = imageModel.ImageUrl.Split('.');
            imageModel.ImageFileExtention = splitResult[splitResult.Length - 1];

            return imageModel;
        }
    }
}
