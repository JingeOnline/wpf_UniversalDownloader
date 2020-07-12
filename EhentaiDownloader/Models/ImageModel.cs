using EhentaiDownloader.Delegates;
using EhentaiDownloader.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class ImageModel
    {
        private string _imageName;
        public string ImageName 
        { 
            get { return _imageName; }
            set { _imageName = FileWriter.FileNameCheck(value); }
        }
        public string ImageUrl{ get; set; }
        public string ImageSavePath 
        {
            get {  return Path.Combine(DelegateCommands.GetFolderPathCommand?.Invoke(), ImageName + "." + ImageFileExtention); }
            //set; 
        }
        public string ImagePageUrl { get; set; }
        public string ImageFileExtention { get; set; }

        public TaskItem taskItem { get; set; }
    }
}
