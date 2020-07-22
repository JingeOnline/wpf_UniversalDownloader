using EhentaiDownloader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Delegates
{
    //public delegate void Delegate_ImageDownloadCount(int number);
    //public delegate void Delegate_ImageDownloadFailCount(int number);
    
    public class DelegateCommands
    {
        public static Action ClearAllCommand { get; set; }
        public static Func<string> GetFolderPathCommand { get; set; }
        public static Action<int> SetImageDownloadCountCommand { get; set; }
        public static Action<int> SetImageDownloadFailCountCommand { get; set; }
        public static Action<long> AddImageDownloadSizeCommand { get; set; }
        public static Action<int> SetUnavailableImagePageCountCommand { get; set; }
        public static Action<EbookPageModel> AddToEbookCollection { get; set; }
        //public static Action<List<EbookPageModel>> AddRangeToEbookCollection { get; set; }
    }
}
