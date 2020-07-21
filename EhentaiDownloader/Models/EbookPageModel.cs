using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class EbookPageModel
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int Year { get; set; }
        public int Pages { get; set; }
        public string Language { get; set; }
        public string FileSize { get; set; }
        public string FileFormat { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string FilePaths { get; set; }
        public string ImagePath { get; set; }
        public int FileCount { get; set; }

        [NotMapped]
        public string StatusMessage { get; set; } = "Unknown";
        [NotMapped]
        public List<EbookFileModel> EBooks { get; set; }
        [NotMapped]
        public EbookImageModel Image { get; set; }

        public EbookPageModel(string url)
        {
            this.Url = url;
        }
    }
}
