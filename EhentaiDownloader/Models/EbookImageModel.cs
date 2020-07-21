using EhentaiDownloader.Delegates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class EbookImageModel
    {
        public string ImageName { get; set; }
        public string ImageUrl { get; set; }
        public string ImageFileExtention
        {
            get
            {
                string[] splitResult = ImageUrl.Split('.');
                if (splitResult.Length > 0)
                {
                    return splitResult[splitResult.Length - 1];
                }
                else
                {
                    return "UnFoundType";
                }
            }
        }
        public string ImageSavePath
        {
            get { return Path.Combine(DelegateCommands.GetFolderPathCommand?.Invoke(), ImageName + "." + ImageFileExtention); }
        }

        public EbookImageModel(string name,string url)
        {
            this.ImageName = name;
            this.ImageUrl = url;
        }
    }
}
