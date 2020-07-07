using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class ImageModel
    {
        public string ImageName { get; set; }
        public string ImageUrl{ get; set; }
        public string ImageSavePath { get; set; }
        public string ImagePageUrl { get; set; }
        public string ImageFileExtention { get; set; }
    }
}
