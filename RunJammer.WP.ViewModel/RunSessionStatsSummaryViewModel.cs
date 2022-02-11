using RunJammer.WP.DataAccess;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.ViewModel
{
    public class RunSessionStatsSummaryViewModel : ViewModelBase
    {
        private int _runSessionCount;
        public int RunSessionCount
        {
            get { return _runSessionCount; }
            set
            {
                if (value != _runSessionCount)
                {
                    _runSessionCount = value;
                    OnPropertyChanged("RunSessionCount");
                }
            }
        }

        private TimeSpan _fastestMile;
        public TimeSpan FastestMile
        {
            get { return _fastestMile; }
            set
            {
                if (value != _fastestMile)
                {
                    _fastestMile = value;
                    OnPropertyChanged("FastestMile");
                }
            }
        }

        private double _totalDistance;
        public double TotalDistance
        {
            get { return _totalDistance; }
            set
            {
                if (value != _totalDistance)
                {
                    _totalDistance = value;
                    OnPropertyChanged("TotalDistance");
                }
            }
        }

        private TimeSpan _totalRunTime;
        public TimeSpan TotalRunTime
        {
            get { return _totalRunTime; }
            set
            {
                if (value != _totalRunTime)
                {
                    _totalRunTime = value;
                    OnPropertyChanged("TotalRunTime");
                }
            }
        }

        private double _averageSpeed;
        public double AverageSpeed
        {
            get { return _averageSpeed; }
            set
            {
                if (value != _averageSpeed)
                {
                    _averageSpeed = value;
                    OnPropertyChanged("AverageSpeed");
                }
            }
        }

        private TimeSpan _averagePace;
        public TimeSpan AveragePace
        {
            get { return _averagePace; }
            set
            {
                if (value != _averagePace)
                {
                    _averagePace = value;
                    OnPropertyChanged("AveragePace");
                }
            }
        }



        private LocalDbDataProvider _dataProvider;

        public RunSessionStatsSummaryViewModel(LocalDbDataProvider dataProvider)
        {
            _dataProvider = dataProvider;

            UpdateStatsSummary();
        }

        private void UpdateStatsSummary()
        {
            UpdateSessionStats();
            UpdateSplitsStats();
        }

        private void UpdateSplitsStats()
        {
            var splits = _dataProvider.GetData<RunSessionSplit>();
            if (splits.Any())
            {
                var mileSplits = splits.Where(s => s.DistanceUnit == "Mile" && s.Measurement == 1).ToList();
                if (mileSplits.Any())
                {
                    try
                    {
                        var times = mileSplits.Select(s => TimeSpan.Parse(s.Duration)).OrderBy(s => s.TotalMinutes).ToList();
                        FastestMile = times.FirstOrDefault(t => t.TotalSeconds > 0);
                    }
                    catch (Exception ex)
                    {
                        _dataProvider.Create(ex);
                    }
                }
            }
        }

        public void UpdateSessionStats()
        {
            var runSessions = _dataProvider.GetData<RunSession>().ToList();
            if (runSessions.Any())
            {
                try
                {
                    RunSessionCount = runSessions.Count();
                    TotalDistance = runSessions.Sum(s => s.TotalDistance);
                    TotalRunTime = TimeSpan.FromMinutes(runSessions.Sum(s => s.StartTime != null && s.EndTime != null ? (s.EndTime.Value - s.StartTime.Value).TotalMinutes : 0));
                    AveragePace = TimeSpan.FromSeconds(runSessions.ToList().Average(s => s.Pace != null ? TimeSpan.Parse(s.Pace).TotalSeconds : 0d));
                    AverageSpeed = runSessions.Sum(s => s.AverageSpeed) / runSessions.Count;
                }
                catch (Exception ex)
                {
                    _dataProvider.Create(ex);
                }
            }
        }
    }
}
