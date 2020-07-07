using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public class ServerClass
    {
        public static ObservableCollection<int> IntList { set; get; } = new ObservableCollection<int>();

        public static void AddOneInList(MyDelegate myDelegate)
        {
            Random random = new Random();
            int number = random.Next(1, 100);
            IntList.Add(number);
            Debug.WriteLine("List中添加了" + number);
            myDelegate.Invoke();
        }
    }
}
