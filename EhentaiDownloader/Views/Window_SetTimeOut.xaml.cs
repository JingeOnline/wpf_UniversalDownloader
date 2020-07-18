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
using System.Windows.Shapes;

namespace EhentaiDownloader.Views
{
    /// <summary>
    /// Window_SetTimeOut.xaml 的交互逻辑
    /// </summary>
    public partial class Window_SetTimeOut : Window
    {
        private int _timeOut;
        public int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }

        public Window_SetTimeOut()
        {
            InitializeComponent();
            TimeOut = Convert.ToInt32(HttpDownloader.GetTimeOut());
            this.DataContext = this;
        }

        private void Button_Apply_Click(object sender, RoutedEventArgs e)
        {
            HttpDownloader.SetTimeOut(TimeOut);
            this.Close();
        }

        
    }
}
