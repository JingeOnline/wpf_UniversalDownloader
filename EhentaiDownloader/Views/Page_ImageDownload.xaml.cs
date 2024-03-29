﻿using EhentaiDownloader.Delegates;
using EhentaiDownloader.Models;
using EhentaiDownloader.Parsers;
using EhentaiDownloader.Services;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Configuration;

namespace EhentaiDownloader.Views
{
    /// <summary>
    /// ImageDownloadPage.xaml 的交互逻辑
    /// </summary>
    public partial class Page_ImageDownload : Page, INotifyPropertyChanged
    {
        private string _urlInput;
        public string UrlInput
        {
            get { return _urlInput; }
            set { _urlInput = value; OnPropertyChanged(); }
        }
        //一定要绑定属性，而不能是字段，public字段也不行。
        public ObservableCollection<TaskItem> DownloadTaskCollection { get; set; } = new ObservableCollection<TaskItem>();
        private string _saveFolder = @"C:\Users\jinge\Pictures";
        public string SaveFolder
        {
            get { return _saveFolder; }
            set
            {
                if (value != _saveFolder)
                {
                    Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    cfa.AppSettings.Settings["OutputFolder"].Value = value;
                    cfa.Save();
                }
                _saveFolder = value;
                OnPropertyChanged();
            }
        }
        private int _imageDownloadCount;
        public int ImageDownloadCount
        {
            get { return _imageDownloadCount; }
            set { _imageDownloadCount = value; OnPropertyChanged(); }
        }
        private int _imageDownloadFailCount;
        public int ImageDownloadFailCount
        {
            get { return _imageDownloadFailCount; }
            set { _imageDownloadFailCount = value; OnPropertyChanged(); }
        }
        private long _imageDownloadSize = 0;
        public long ImageDownloadSize
        {
            get { return _imageDownloadSize; }
            set { _imageDownloadSize = value; OnPropertyChanged(); OnPropertyChanged("ImageDownloadSizeUi"); }
        }
        private int _imagePageUnavailableCount;
        public int ImagePageUnavailableCount
        {
            get { return _imagePageUnavailableCount; }
            set { _imagePageUnavailableCount = value; OnPropertyChanged(); }
        }

        public string ImageDownloadSizeUi
        {
            get
            {
                if (ImageDownloadSize < 1024 * 1024) { return (ImageDownloadSize / 1024).ToString() + " KB"; }
                if (ImageDownloadSize < 1024 * 1024 * 1024) { return (ImageDownloadSize / 1024 / 1024).ToString() + " MB"; }
                else
                {
                    return (ImageDownloadSize / (float)1024 / 1024 / 1024).ToString("0.00") + " GB";
                }
            }
        }

        private Action<string> addUserInputToTaskList;

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propName = "")
        {
            if (this.PropertyChanged != null)
            {
                var handler = PropertyChanged;
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }

        public Page_ImageDownload()
        {
            InitializeComponent();
            this.DataContext = this;
            InitialOutputPath();
        }

        public void InitialOutputPath()
        {
            string outpath = ConfigurationManager.AppSettings["OutputFolder"];
            if (!string.IsNullOrEmpty(outpath))
            {
                SaveFolder = outpath;
            }
        }

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(UrlInput))
            {
                TextBox_UrlInput.Focus();
                return;
            }
            if (DownloadTaskCollection.Any(x => x.Url == UrlInput))
            {
                TextBox_UrlInput.Focus();
                return;
            }
            addUserInputToTaskList(UrlInput);
            UrlInput = string.Empty;
            TextBox_UrlInput.Focus();
        }

        private async void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            registerCommands();
            await ImageDownloadService.StartDownload(DownloadTaskCollection);
            TextBox_UrlInput.Focus();
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
            ImageDownloadSize = 0;
        }

        private void registerCommands()
        {
            DelegateCommands.ClearAllCommand = ClearAll;
            DelegateCommands.GetFolderPathCommand = () => SaveFolder;
            DelegateCommands.SetImageDownloadCountCommand = (x) => ImageDownloadCount = x;
            DelegateCommands.SetImageDownloadFailCountCommand = (x) => ImageDownloadFailCount = x;
            DelegateCommands.AddImageDownloadSizeCommand = (x) => ImageDownloadSize += x;
            DelegateCommands.SetUnavailableImagePageCountCommand = (x) => ImagePageUnavailableCount = x;
        }

        private void Button_RemoveFromList_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            TaskItem taskItem = button.DataContext as TaskItem;
            DownloadTaskCollection.Remove(taskItem);
            DataGrid_Tasks.Items.Refresh();
        }

        private void ComboBox_InputMethod_DropDownClosed(object sender, EventArgs e)
        {

        }

        private void ComboBox_InputMethod_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            int index = comboBox.SelectedIndex;
            if (index == 0)
            {
                addUserInputToTaskList = (s) =>
                {
                    DownloadTaskCollection.Add(new TaskItem(s.Trim()));
                };
            }
            else if (index == 1)
            {
                addUserInputToTaskList = (s) =>
                {
                    string pattern = @"(https?|ftp|file)://[-A-Za-z0-9+&@#/%?=~_|!:,.;]+[-A-Za-z0-9+&@#/%=~_|]";
                    MatchCollection matches = Regex.Matches(s, pattern);
                    foreach (Match match in matches)
                    {
                        DownloadTaskCollection.Add(new TaskItem(match.Value));
                    }
                };

            }
            else if (index == 2)
            {
                addUserInputToTaskList = async (s) =>
                {
                    List<string> urls = await new ShzxParser_ParentParser().Parse(s);
                    IEnumerable<string> removeDuplicateUrls = urls.Distinct();
                    foreach (string url in removeDuplicateUrls)
                    {
                        DownloadTaskCollection.Add(new TaskItem(url));
                    }
                };
            }
            else
            {
                return;
            }
        }

        private void DataGrid_Tasks_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
