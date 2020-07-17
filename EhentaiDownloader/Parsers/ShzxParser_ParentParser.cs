using AngleSharp;
using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Parsers
{
    public class ShzxParser_ParentParser
    {
        public async Task<List<string>> Parse(string url)
        {
            IConfiguration config = Configuration.Default.WithDefaultLoader();
            IBrowsingContext context = BrowsingContext.New(config);
            IDocument document = await context.OpenAsync(url);
            IElement element1 = document.QuerySelector("div.b_img");
            var elements = element1.QuerySelectorAll("li>a");

            List<string> urls = new List<string>();
            foreach(var element in elements)
            {
                urls.Add("https://www.shzx.org"+element.GetAttribute("href"));
            }
            return urls;
        }
    }
}
