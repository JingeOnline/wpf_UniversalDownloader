﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class ImagePageModel
    {
        public string ImagePageUrl { get; set; }
        public TaskItem TaskItem { get; set; }
        public string FailMessage { get; set; }

        public ImagePageModel(string imagePageUrl, TaskItem taskItem)
        {
            this.ImagePageUrl = imagePageUrl;
            this.TaskItem = taskItem;
        }

        public override string ToString()
        {
            //return base.ToString();
            return ImagePageUrl;
        }
    }
}
