using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace Common.Model.Logging
{
    public class UILogger : LoggerBase, INotifyPropertyChanged
    {
        private ObservableCollection<Exception> _exceptionsCollection;
        public ObservableCollection<Exception> ExceptionsCollection
        {
            get { return _exceptionsCollection; }
            set
            {
                if (value != _exceptionsCollection)
                {
                    _exceptionsCollection = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Exceptions"));
                    }
                }
            }
        }

        public UILogger()
        {
            ExceptionsCollection = new ObservableCollection<Exception>();
        }

        public override void Log(Exception ex)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                base.Log(ex);
                ExceptionsCollection.Add(ex);
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
