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

        public async Task<List<EbookPageModel>> FindEbookPage(string url)
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
            List<EbookPageModel> webPageModels = new List<EbookPageModel>();
            try
            {
                foreach (IElement element in urlElements)
                {
                    string bookPageUrl = element.QuerySelector("a").GetAttribute("href");
                    webPageModels.Add(new EbookPageModel(bookPageUrl));
                    Debug.WriteLine("成功:找到pageurl=" + bookPageUrl);
                }
            }
            catch
            {
                throw new TargetNotFindException("无法在HTML中找到a");
            }

            return webPageModels;
        }

        public async Task<EbookPageModel> FindEbookUrl(EbookPageModel webPageModel)
        {
            

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
            webPageModel.Year = Int32.Parse( terms.ElementAt(2).InnerHtml);
            webPageModel.Pages = Int32.Parse(terms.ElementAt(3).InnerHtml);
            webPageModel.Language = terms.ElementAt(4).InnerHtml;
            webPageModel.FileSize = terms.ElementAt(5).InnerHtml;
            //webPageModel.FileFormat = terms.ElementAt(6).InnerHtml;
            IEnumerable<IElement> tagElements = terms.ElementAt(7).QuerySelectorAll("a");
            webPageModel.Category = string.Join(";", tagElements.Select(x => x.InnerHtml));
            webPageModel.Description = document.QuerySelector("div.entry-content").InnerHtml;
            webPageModel.Title = document.QuerySelector("h1.single-title").InnerHtml;
            IElement subTitleElement = document.QuerySelector("h4");
            if (subTitleElement != null)
            {
            webPageModel.SubTitle = subTitleElement.InnerHtml;
            }

            IElement imageElement = document.QuerySelector("header.entry-header");
            IElement imageElement1=imageElement.QuerySelector("img");
            string imageUrl = imageElement1.GetAttribute("src");
            string fileUniqueName = TimeHelper.GetTimeStamp();
            webPageModel.Image = new EbookImageModel(fileUniqueName, imageUrl);

            List<EbookFileModel> ebooks = new List<EbookFileModel>();
            int i = 1;
            foreach (Element downloadLinkElement in downloadLinkElements)
            {
                string url = downloadLinkElement.QuerySelector("a").GetAttribute("href");
                string fileName = fileUniqueName +"_"+ i;
                ebooks.Add(new EbookFileModel(webPageModel, url, fileName));
                i++;
                //Debug.WriteLine("fileName=" + fileName);
            }

            webPageModel.EBooks = ebooks;

            return webPageModel;
        }
    }
}
