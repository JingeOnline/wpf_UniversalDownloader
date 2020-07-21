using EhentaiDownloader.Delegates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class EbookFileModel
    {

        public string DownloadUrl { get; set; }
        //public EbookPageModel WebPageModel{get;set; }
        public string ErrorMessage { get; set; }
        public bool IsDownloaded { get; set; } = false;
        public string FileName { get; set; }
        public string FileSavePath 
        {
            get { return Path.Combine(DelegateCommands.GetFolderPathCommand?.Invoke(), FileName + "." + FileExtention); }
        }
        public string FileExtention
        {
            get
            {
                string[] splitResult = DownloadUrl.Split('.');
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

        public EbookFileModel(EbookPageModel webPageModel,string downloadUrl, string fileName)
        {
            //this.WebPageModel = webPageModel;
            this.DownloadUrl = downloadUrl;
            this.FileName = fileName;
        }
    }
}
