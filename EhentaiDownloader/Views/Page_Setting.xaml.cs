using EhentaiDownloader.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// SettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class Page_Setting : Page
    {
        public int TimeOut
        {
            get { return HttpDownloader.GetTimeOut(); }
            set { HttpDownloader.SetTimeOut(value); }
        }

        public bool IsSoundOn
        {
            get { return SoundPlayer.IsOn; }
            set { SoundPlayer.IsOn = value; }
        }

        public Page_Setting()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
