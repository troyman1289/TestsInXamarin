using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Backend.Utils
{
    /// <summary>
    /// Sends notification when the property of an object changed
    /// </summary>
    public class NotifyingObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        protected void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null) {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
