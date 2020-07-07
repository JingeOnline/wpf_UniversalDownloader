using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    class ParseResult
    {
        public List<string> ImagePageList { get; set; }
        public string NextPage { get; set; }
    }
}
