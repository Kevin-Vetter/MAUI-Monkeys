using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AppProg1.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isLoading;
        private bool _isBusy;
        private string _tilte;
            
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetField(ref _isLoading, value);}
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetField(ref _isBusy,value);}
        }


        public string Title
        {
            get { return _tilte; }
            set 
            { SetField(ref _tilte,value);}
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
        
        public bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
