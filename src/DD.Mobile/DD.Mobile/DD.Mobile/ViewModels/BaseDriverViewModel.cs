using System.ComponentModel; 
using System.Runtime.CompilerServices;
 
namespace DD.Mobile.ViewModels
{
    public abstract class BaseDriverViewModel : INotifyPropertyChanged
    {
        protected BaseDriverViewModel()
        { }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }
    }
}
