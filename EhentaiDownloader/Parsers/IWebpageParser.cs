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
        Task<List<ImagePageModel>> FindImagePageUrl(TaskItem taskItem);
        Task<List<ImageModel>> FindImageUrls(ImagePageModel imagePage);
    }
}
