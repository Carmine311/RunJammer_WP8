using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.ViewModel
{
    public class SplitsViewModel : ViewModelBase
    {

        private RunSession _runSession;

        private ObservableCollection<RunSessionSplit> _distanceUnitSplits;
        public ObservableCollection<RunSessionSplit> DistanceUnitSplits
        {
            [DebuggerStepThrough]
            get { return _distanceUnitSplits; }
            set
            {
                if (value != _distanceUnitSplits)
                {
                    _distanceUnitSplits = value;
                    OnPropertyChanged("DistanceUnitSplits");
                }
            }
        }

        private ObservableCollection<RunSessionSplit> _fiveKSplits;
        public ObservableCollection<RunSessionSplit> FiveKSplits
        {
            [DebuggerStepThrough]
            get { return _fiveKSplits; }
            set
            {
                if (value != _fiveKSplits)
                {
                    _fiveKSplits = value;
                    OnPropertyChanged("FiveKSplits");
                }
            }
        }

        private ObservableCollection<RunSessionSplit> _tenkSplits;
        public ObservableCollection<RunSessionSplit> TenKSplits
        {
            [DebuggerStepThrough]
            get { return _tenkSplits; }
            set
            {
                if (value != _tenkSplits)
                {
                    _tenkSplits = value;
                    OnPropertyChanged("TenKSplits");
                }
            }
        }



        public void UpdateSplits()
        {
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                DistanceUnitSplits = new ObservableCollection<RunSessionSplit>(
                    _runSession.Splits.Where(
                        s => s.DistanceUnit == _runSession.DistanceUnit.ToString() && s.Measurement == 1));

                FiveKSplits = new ObservableCollection<RunSessionSplit>(_runSession.Splits.Where(s => s.DistanceUnit == DistanceUnit.Kilometre.ToString() && s.Measurement == 5));
                TenKSplits = new ObservableCollection<RunSessionSplit>(_runSession.Splits.Where(s => s.DistanceUnit == DistanceUnit.Kilometre.ToString() && s.Measurement == 10));
            });
        }


        public SplitsViewModel(RunSession runSession)
        {
            _runSession = runSession;
        }
    }
}
