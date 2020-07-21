using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EhentaiDownloader.Views
{
    /// <summary>
    /// Page_DataScraping.xaml 的交互逻辑
    /// </summary>
    public partial class Page_DataScraping : Page,INotifyPropertyChanged
    {
        private string _urlInput1;
        public string UrlInput1
        {
            get { return _urlInput1; }
            set { _urlInput1 = value; OnPropertyChanged(); }
        }

        private string _urlInput2;
        public string UrlInput2
        {
            get { return _urlInput2; }
            set { _urlInput2 = value; OnPropertyChanged(); }
        }

        private string _saveFolder = @"E:\Ebooks";
        public string SaveFolder
        {
            get { return _saveFolder; }
            set { _saveFolder = value; OnPropertyChanged(); }
        }

        public ObservableCollection<EbookModel> BookCollection { get; set; } = new ObservableCollection<EbookModel>();

        public Page_DataScraping()
        {
            InitializeComponent();
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

        private async void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            registerCommands();
            await DataScrapingService.StartDownload(UrlInput1, UrlInput2);
        }

        private void Button_SaveTo_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //设置为选择文件夹模式
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SaveFolder = dialog.FileName;
            }
        }

        private void registerCommands()
        {
            //DelegateCommands.ClearAllCommand = ClearAll;
            DelegateCommands.GetFolderPathCommand = () => SaveFolder;
            //DelegateCommands.SetImageDownloadCountCommand = (x) => ImageDownloadCount = x;
            //DelegateCommands.SetImageDownloadFailCountCommand = (x) => ImageDownloadFailCount = x;
            //DelegateCommands.AddImageDownloadSizeCommand = (x) => ImageDownloadSize += x;
            //DelegateCommands.SetUnavailableImagePageCountCommand = (x) => ImagePageUnavailableCount = x;
        }
    }
}
