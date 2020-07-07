using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EhentaiDownloader.Models
{
    public class TaskItem : INotifyPropertyChanged
    {
        public string Url { get; set; }
        public string SaveFolderName { get; set; }

        private string _status="Waiting";
        public string Status
        {
            get { return _status; }
            set { _status = value; OnPropertyChanged(); }
        }

        private int _numberOfFiles;
        public int NumberOfFiles
        {
            get { return _numberOfFiles; }
            set { _numberOfFiles = value; OnPropertyChanged();OnPropertyChanged("Progress"); OnPropertyChanged("Files"); }
        }

        private int _numberOfFinish;
        public int NumberOfFinish
        {
            get { return _numberOfFinish; }
            set { _numberOfFinish = value; OnPropertyChanged();OnPropertyChanged("Progress"); OnPropertyChanged("Files"); OnPropertyChanged("ShowProgressBar"); }
        }

        public double Progress 
        {
            get 
            {
                if (NumberOfFiles == 0)
                {
                    return 0;
                }
                else
                {
                return (double)NumberOfFinish / NumberOfFiles;
                }
            }
        }

        public string Files
        {
            get
            {
                if (NumberOfFiles == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return NumberOfFinish + "/" + NumberOfFiles;
                }
            }
        }

        public bool ShowProgressBar
        {
            get
            {
                if (NumberOfFiles == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.PropertyChanged != null)
            {
                var handler = PropertyChanged;
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public TaskItem(string url)
        {
            this.Url = url;
        }
    }
}
