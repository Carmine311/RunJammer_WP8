
using RunJammer.WP.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.ViewModel.Helpers
{
    public class RunSessionSplitsRecorder
    {
        private List<RunSessionSplit> _activeSplits = new List<RunSessionSplit>();
        private List<RunSessionSplit> _completedSplits = new List<RunSessionSplit>();
        private RunSession _runSession;

        public void Start()
        {
            foreach (var split in _activeSplits)
            {
                split.StartTime = DateTime.Now;
            }
        }

        public void Update()
        {

            foreach (var split in _activeSplits.ToArray())
            {
                if (_runSession.DistanceUnit == DistanceUnit.Mile)
                {
                    double targetDistance;

                    targetDistance = split.DistanceUnit == DistanceUnit.Mile.ToString() ?
                        _runSession.TotalDistance : ConvertMilesToKilometers(_runSession.TotalDistance);

                    if (targetDistance >= (split.Measurement * split.Instance))
                    {
                        split.EndTime = DateTime.Now;
                        split.Duration = (split.EndTime - split.StartTime).ToString();

                        _runSession.Splits.Add(split);
                        _completedSplits.Add(split);
                        _activeSplits.Remove(split);
                        _runSession.Splits.Add(split);
                        OnRunSessionSplitCompleted(new RunSessionSplitCompletedEventArgs { Split = split });


                        _activeSplits.Add(new RunSessionSplit
                        {
                            DistanceUnit = split.DistanceUnit,
                            StartTime = split.EndTime.Value,
                            SessionID = split.SessionID,
                            Measurement = split.Measurement,
                            Instance = split.Instance + 1
                        });
                    }
                }
            }
        }

        private double ConvertMilesToKilometers(double miles)
        {
            return miles * 1.609344;
        }

        public async Task Stop()
        {
            await Task.Run(() =>
            {
                foreach (var split in _activeSplits)
                {
                    split.EndTime = DateTime.Now;
                }
            });
        }

        public RunSessionSplitsRecorder()
        {
            _activeSplits = new List<RunSessionSplit>();

        }

        public RunSessionSplitsRecorder(RunSession runSession)
            : this()
        {
            _runSession = runSession;
            InitializeDefaultSplits();
            InitializeDistanceUnitSplit();
        }

        private void InitializeDistanceUnitSplit()
        {
            _activeSplits.Add(new RunSessionSplit
            {
                DistanceUnit = _runSession.DistanceUnit.ToString(),
                Measurement = 1,
                Instance = 1
            });
        }

        private void InitializeDefaultSplits()
        {

            var _5k = new RunSessionSplit
            {
                DistanceUnit = DistanceUnit.Kilometre.ToString(),
                Measurement = 5,
                Instance = 1
            };
            var _10k = new RunSessionSplit
            {
                DistanceUnit = DistanceUnit.Kilometre.ToString(),
                Measurement = 10,
                Instance = 1
            };
            var halfMarathon = new RunSessionSplit
            {
                DistanceUnit = DistanceUnit.Mile.ToString(),
                Measurement = 13.1,
                Instance = 1
            };
            var marathon = new RunSessionSplit
            {
                DistanceUnit = DistanceUnit.Mile.ToString(),
                Measurement = 26.2,
                Instance = 1
            };

            _activeSplits.Add(_5k);
            _activeSplits.Add(_10k);
            _activeSplits.Add(halfMarathon);
            _activeSplits.Add(marathon);
        }

        public event EventHandler<RunSessionSplitCompletedEventArgs> RunSessionSplitCompleted;

        protected virtual void OnRunSessionSplitCompleted(RunSessionSplitCompletedEventArgs e)
        {
            EventHandler<RunSessionSplitCompletedEventArgs> handler = RunSessionSplitCompleted;
            if (handler != null) handler(this, e);
        }
    }
}
