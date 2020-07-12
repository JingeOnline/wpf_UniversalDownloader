using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AngleSharp;
using AngleSharp.Browser;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;

namespace EhentaiDownloader.Tools
{
    class EHentaiParser : IWebpageParser
    {
        //private static string albumTitle;

        public async Task<List<string>> FindImagePageUrl(string url)
        {
            string html = await HttpDownloader.DownloadHtmlPage(url);
            ParseResult result = await findNextPageLinkAndCurrentImagePages(html);
            List<string> imagePageList = new List<string>();
            imagePageList.AddRange(result.ImagePageList);
            if (result.NextPage != null)
            {
                imagePageList.AddRange(await FindImagePageUrl(result.NextPage));
            }
            return imagePageList;
        }

        private async Task<ParseResult> findNextPageLinkAndCurrentImagePages(string html)
        {
            Debug.WriteLine("开始Parse下一页的链接");
            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(response => response.Content(html));
            IEnumerable<IElement> result = document.All.Where(m => m.LocalName == "a" && m.Text() == ">");
            //findAlbumTitle();
            int a = result.Count();
            Debug.WriteLine("a=" + a);
            if (result.Count() > 0)
            {
                string nextPageLink = result.First().GetAttribute("href");
                Debug.WriteLine("找到下一页链接" + nextPageLink);
                //return nextPageLink;
                return new ParseResult { NextPage = nextPageLink, ImagePageList = findImagePageLink() };
            }
            else
            {
                Debug.WriteLine("未找到NextPage链接");
                return new ParseResult { NextPage = null, ImagePageList = findImagePageLink() };
            }

            //void findAlbumTitle()
            //{
            //    IEnumerable<IElement> titleElement = document.All.Where(m => m.LocalName == "h1" && m.Id == "gn");
            //    albumTitle = titleElement.First().Text();
            //    Char[] unSafeChars= {'*','.','\\','/','|','\"','|','?','<','>'};
            //    foreach(char c in unSafeChars)
            //    {
            //        albumTitle=albumTitle.Replace(c, '_');
            //    }
            //    Debug.WriteLine("找到album title=" +albumTitle);
            //}

            List<string> findImagePageLink()
            {
                var results = document.All.Where(m => m.LocalName == "div" && m.ClassName == "gdtm");
                List<string> imagePageUrls = new List<string>();
                foreach (var res in results)
                {
                    string s = res.QuerySelector("a").GetAttribute("href");
                    //Debug.WriteLine("图片网页：" + s);
                    imagePageUrls.Add(s);
                }
                Debug.WriteLine("找到图片网页" + imagePageUrls.Count() + "个");
                return imagePageUrls;
            }
        }


        public async Task<ImageModel> FindImageUrl(string url)
        {

            string html = await HttpDownloader.DownloadHtmlPage(url);

            IConfiguration config = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(response => response.Content(html));
            var urlResult = document.All.Where(m => m.LocalName == "img" && m.Id == "img");
            string imageUrl = urlResult.First().GetAttribute("src");

            var imageNumResult = document.All.Where(m => m.LocalName == "div" && m.ClassName == "sn");
            string imageNum = imageNumResult.First().QuerySelector("div").QuerySelector("span").Text();

            var titleResult = document.All.Where(m => m.LocalName == "div" && m.Id == "i1");
            string title = titleResult.First().QuerySelector("h1").Text();
            string imageName = title + "_" + imageNum;

            ImageModel image = new ImageModel
            {
                ImageName = imageName,
                ImageUrl = imageUrl,
                ImagePageUrl = url,
                ImageFileExtention = "jpg",
            };
            return image;
        }

    }
}
