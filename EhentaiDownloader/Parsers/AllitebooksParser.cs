using AngleSharp;
using AngleSharp.Dom;
using EhentaiDownloader.Exceptions;
using EhentaiDownloader.Models;
using EhentaiDownloader.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Parsers
{
    class AllitebooksParser : IEbookParser
    {

        public async Task<List<WebPageModel>> FindEbookPage(string url)
        {
            string html;
            try
            {
                html = await HttpDownloader.DownloadHtmlPage(url);
            }
            catch
            {
                throw;
            }
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));
            IEnumerable<IElement> urlElements = document.QuerySelectorAll("h2.entry-title");
            if (urlElements.Count() == 0)
            {
                throw new TargetNotFindException("无法在HTML中找到h2.entry-title");
            }
            List<WebPageModel> webPageModels = new List<WebPageModel>();
            try
            {
                foreach (IElement element in urlElements)
                {
                    string bookPageUrl = element.QuerySelector("a").GetAttribute("href");
                    webPageModels.Add(new WebPageModel(bookPageUrl));
                    Debug.WriteLine("成功:找到pageurl=" + bookPageUrl);
                }
            }
            catch
            {
                throw new TargetNotFindException("无法在HTML中找到a");
            }

            return webPageModels;
        }

        public async Task<List<EbookModel>> FindEbookUrl(WebPageModel webPageModel)
        {
            List<EbookModel> ebooks = new List<EbookModel>();

            string html;
            try
            {
                html = await HttpDownloader.DownloadHtmlPage(webPageModel.Url);
            }
            catch
            {
                throw;
            }
            IBrowsingContext context = BrowsingContext.New();
            IDocument document = await context.OpenAsync(response => response.Content(html));

            IEnumerable<IElement> downloadLinkElements = document.QuerySelectorAll("span.download-links");
            if (downloadLinkElements.Count() == 0)
            {
                throw new TargetNotFindException("无法在HTML中找到下载链接");
            }

            IElement detail = document.QuerySelector("div.book-detail");
            IEnumerable<IElement> terms = detail.QuerySelectorAll("dd");
            IEnumerable<IElement> authorsElements= terms.ElementAt(0).QuerySelectorAll("a");
            webPageModel.Author = string.Join(";", authorsElements.Select(x => x.InnerHtml));
            webPageModel.ISBN = terms.ElementAt(1).InnerHtml;
            webPageModel.Year = terms.ElementAt(2).InnerHtml;
            webPageModel.Pages = terms.ElementAt(3).InnerHtml;
            webPageModel.Language = terms.ElementAt(4).InnerHtml;
            webPageModel.FileSize = terms.ElementAt(5).InnerHtml;
            webPageModel.FileFormat = terms.ElementAt(6).InnerHtml;
            IEnumerable<IElement> tagElements = terms.ElementAt(7).QuerySelectorAll("a");
            webPageModel.Category = string.Join(";", tagElements.Select(x => x.InnerHtml));
            webPageModel.Description = document.QuerySelector("div.entry-content").TextContent;

            int i = 0;
            foreach (Element downloadLinkElement in downloadLinkElements)
            {
                string url = downloadLinkElement.QuerySelector("a").GetAttribute("href");
                string fileName = TimeHelper.GetTimeStamp() + i;
                ebooks.Add(new EbookModel(webPageModel, url, fileName));
                i++;
                Debug.WriteLine("fileName=" + fileName);
            }

            return ebooks;
        }
    }
}
