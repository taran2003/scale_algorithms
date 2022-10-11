using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1
{
    internal class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public String FielPath { get; } = new String("D:\\c#\\компьютерная график\\Lab1\\Lab1\\Lab1\\MegaMan.jpg");
        public ObservableCollection<string> filePath {get;} = new ObservableCollection<string>() { "D:\\c#\\компьютерная график\\Lab1\\Lab1\\Lab1\\MegaMan.jpg" };

        RelayCommand? addToList;

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public RelayCommand Neighbor => addToList ?? (addToList = new RelayCommand(obj => { neighbor(); }));

        public void neighbor()
        {

            
        }
    }
}
