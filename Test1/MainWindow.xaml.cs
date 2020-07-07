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

namespace Test1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        private int _number=0;

        public int Number
        {
            get { return _number; }
            set { _number = value; OnPropertyChanged(); }
        }


        public MainWindow()
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

        private void Button_Add_Click(object sender, RoutedEventArgs e)
        {
            Number++;
        }

        private void Button_AddInOtherClass_Click(object sender, RoutedEventArgs e)
        {
            ServerClass.AddOneInList(Update);
        }

        private void Button_ShowListCount_Click(object sender, RoutedEventArgs e)
        {
            
            
        }

        public void Update()
        {
            ObservableCollection<int> myList = ServerClass.IntList;
            Number = myList.Count();
        }
    }
}
