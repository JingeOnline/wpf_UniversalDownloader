using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class WebPageModel
    {
        public string Url { get; set; }
        public string StatusMessage { get; set; } = "Unknown";
        public string ParentUrl { get; set; }

        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Year { get; set; }
        public string Pages { get; set; }
        public string Language { get; set; }
        public string FileSize { get; set; }
        public string FileFormat { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }

        public WebPageModel(string url)
        {
            this.Url = url;
        }
    }
}
