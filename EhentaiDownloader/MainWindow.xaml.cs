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
using System.Text.RegularExpressions;
using EhentaiDownloader.Parsers;
using EhentaiDownloader.Tools;

namespace EhentaiDownloader
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private Page imageDownloaderPage = new Page_ImageDownload();
        private Page settingPage = new Page_Setting();
        private Page dataScrapingPage = new Page_DataScraping();

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(imageDownloaderPage);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void StackPanel_Setting_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(settingPage);
        }

        private void StackPanel_DataScraping_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(dataScrapingPage);
        }

        private void StackPanel_PictureDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainFrame.Navigate(imageDownloaderPage);
        }
    }
}
