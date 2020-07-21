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
        Task<List<EbookPageModel>> FindEbookPage(string url);
        Task<EbookPageModel> FindEbookUrl(EbookPageModel webPageModel);
    }
}
