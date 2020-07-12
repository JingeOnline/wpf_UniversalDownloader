using EhentaiDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Parsers
{
    interface IWebpageParser
    {
        Task<List<string>> FindImagePageUrl(string url);
        Task<ImageModel> FindImageUrl(ImageModel imageModel);
    }
}
