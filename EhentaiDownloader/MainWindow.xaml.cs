using EhentaiDownloader.Models;
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
using Microsoft.WindowsAPICodePack.Dialogs;
using EhentaiDownloader.Services;
using EhentaiDownloader.Views;
using EhentaiDownloader.Delegates;

namespace EhentaiDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private string _urlInput;
        public string UrlInput
        {
            get { return _urlInput; }
            set { _urlInput = value; OnPropertyChanged(); }
        }
        //一定要绑定属性，而不能是字段，public字段也不行。
        public ObservableCollection<TaskItem> DownloadTaskCollection { get; set; } = new ObservableCollection<TaskItem>();
        private string _saveFolder= @"C:\Users\jinge\Pictures\hentai";
        public string SaveFolder
        {
            get { return _saveFolder; }
            set { _saveFolder = value; OnPropertyChanged(); }
        }
        public int _imageDownloadCount;
        public int ImageDownloadCount
        {
            get { return _imageDownloadCount; }
            set { _imageDownloadCount = value; OnPropertyChanged(); }
        }
        public int _imageDownloadFailCount;
        public int ImageDownloadFailCount
        {
            get { return _imageDownloadFailCount; }
            set { _imageDownloadFailCount = value; OnPropertyChanged(); }
        }

        public MainWindow()
        {
            InitializeComponent();
            registerCommands();
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

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            DownloadTaskCollection.Add(new TaskItem(UrlInput));
            UrlInput = string.Empty;
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            DownloadService.StartDownload(DownloadTaskCollection);
            DownloadService.SetImageDownloadCount_Delegate = (x) => ImageDownloadCount = x;
            DownloadService.SetImageDownloadFailCount_Delegate = (x) => ImageDownloadFailCount = x;
        }

        private void Button_SaveTo_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            //设置为选择文件夹模式
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
               SaveFolder= dialog.FileName;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            new Window_SetTimeOut().Show();
        }

        public void ClearAll()
        {
            UrlInput = "";
            DownloadTaskCollection.Clear();
            ImageDownloadCount = 0;
            ImageDownloadFailCount = 0;
        }

        private void registerCommands()
        {
            DelegateCommands.ClearAllCommand = ClearAll;
            DelegateCommands.GetFolderPath = () => SaveFolder;
        }

        private void Button_RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TaskItem taskItem = button.DataContext as TaskItem;
            DownloadTaskCollection.Remove(taskItem);
        }
    }
}
