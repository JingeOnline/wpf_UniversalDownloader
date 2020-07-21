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

namespace Navigation
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        //public Page PageFill { get; set; }

        Page p1 = new Page1();
        Page p2 = new Page2();

        public MainWindow()
        {
            InitializeComponent();
            //PageFill = new Page1();
            MainFrame.Navigate(p1);
        }

        private void Button_Page1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(p1);
        }

        private void Button_Page2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(p2);
        }
    }
}
