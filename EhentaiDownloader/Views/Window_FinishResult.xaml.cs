using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EhentaiDownloader.Views
{
    /// <summary>
    /// Window_FinishResult.xaml 的交互逻辑
    /// </summary>
    public partial class Window_FinishResult : Window,INotifyPropertyChanged
    {
        public List<ImageModel> _imageList;
        public List<ImageModel> ImageList 
        {
            get { return _imageList; }
            set { _imageList = value;OnPropertyChanged(); } 
        }
        public List<ImagePageModel> _imagePageList;
        public List<ImagePageModel> ImagePageList
        {
            get { return _imagePageList; }
            set { _imagePageList = value;OnPropertyChanged(); }
        }

        public Window_FinishResult()
        {
            InitializeComponent();
            ImageList = DownloadService.DownloadFailImages;
            _imagePageList = DownloadService.UnAvailablePages;
            this.DataContext = this;
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

        private void Button_ReTry_Click(object sender, RoutedEventArgs e)
        {
            new Window_SetTimeOut().ShowDialog();
            this.Close();
            DownloadService.ReTryAsync();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            DelegateCommands.ClearAllCommand?.Invoke();
        }
    }
}
