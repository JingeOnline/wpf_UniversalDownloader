using EhentaiDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Parsers
{
    interface IEbookParser
    {
        Task<List<WebPageModel>> FindEbookPage(string url);
        Task<List<EbookModel>> FindEbookUrl(WebPageModel webPageModel);
    }
}
