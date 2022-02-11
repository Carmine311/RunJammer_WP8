using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Windows.Devices.Geolocation;
using Common.DataAccess.Interface;
using Common.Model.Implementation;
using Common.Model.Logging;
using Microsoft.Phone.Maps.Controls;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;
using RunJammer.WP.ViewModel.Helpers;
using System.Data.Linq;
using RunJammer.WP.Messaging;

namespace RunJammer.WP.ViewModel
{
    public class RunSessionViewModel : LoggerViewModelBase
    {

        #region Fields

        private readonly RunJammerApplicationDataProvider _dataProvider;
        private RunSession _runSession;

        private Geolocator _geolocator;

        private List<Geocoordinate> _discardedPositionUpdates;
        private List<Geocoordinate> _goodPositionUpdates;
        private Geocoordinate _lastPositionUpdate;

        private RunSessionSplitsRecorder _splitsRecorder;

        private double _currentMetersPerSecond;
        private bool _isSavingToDb;

        private bool _isUserLoggedIn;


        private DispatcherTimer _sessionDurationTimer;
        private DispatcherTimer _dbUpdateTimer;

        #endregion

        #region Interface

        private SplitsViewModel _splitsViewModel;
        public SplitsViewModel SplitsViewModel
        {
            [DebuggerStepThrough]
            get { return _splitsViewModel; }
            set
            {
                if (value != _splitsViewModel)
                {
                    _splitsViewModel = value;
                    OnPropertyChanged("SplitsViewModel");
                }
            }
        }


        private ObservableCollection<RunSessionWaypoint> _waypoints;
        public ObservableCollection<RunSessionWaypoint> Waypoints
        {
            get { return _waypoints; }
            set
            {
                if (value != _waypoints)
                {
                    _waypoints = value;
                    OnPropertyChanged("Waypoints");
                    OnPropertyChanged("Songs");
                }
            }
        }


        private bool _isLocationDisabled;
        public bool IsLocationDisabled
        {
            get { return _isLocationDisabled; }
            set
            {
                if (value != _isLocationDisabled)
                {
                    _isLocationDisabled = value;
                    OnPropertyChanged("IsLocationDisabled");
                }
            }
        }

        private bool _isLocationDataReady;
        public bool IsLocationDataReady
        {
            get { return _isLocationDataReady; }
            set
            {
                if (value != _isLocationDataReady)
                {
                    _isLocationDataReady = value;
                    OnPropertyChanged("IsLocationDataReady");
                    OnPropertyChanged("CanStartRunSession");
                }
            }
        }

        private bool _isRunningInBackground;
        public bool IsRunningInBackground
        {
            get { return _isRunningInBackground; }
            set
            {
                if (value != _isRunningInBackground)
                {
                    _isRunningInBackground = value;
                    OnPropertyChanged("IsRunningInBackground");
                }
            }
        }


        private bool _canStartRunSession;
        public bool CanStartRunSession
        {
            get { return IsLocationDataReady && !IsSessionActive; }
            set
            {
                if (value != _canStartRunSession)
                {
                    _canStartRunSession = value;
                    OnPropertyChanged("CanStartRunSession");
                }
            }
        }

        private ObservableCollection<RunJammerSongViewModel> _songs;
        public ObservableCollection<RunJammerSongViewModel> Songs
        {

            get
            {
                if (_songs == null)
                {
                    _songs = new ObservableCollection<RunJammerSongViewModel>(GetRunSessionSongs());
                }
                return _songs;
            }
            set
            {
                if (_songs != value)
                {
                    _songs = value;
                    OnPropertyChanged("Songs");
                }
            }
        }

        private IEnumerable<RunJammerSongViewModel> GetRunSessionSongs()
        {
            var songs =
                Waypoints.Where(w => w.CurrentSong != null && !w.CurrentSong.ExcludeFromRunSessions)
                    .Select(w => w.CurrentSong)
                    .Distinct()
                    .ToList();
            return songs.Select(s => new RunJammerSongViewModel(s));

        }


        private double _totalDistance = 0.00;
        public double TotalDistance
        {
            [DebuggerStepThrough]
            get
            {
                return _totalDistance;
            }
            set
            {
                if (value != _totalDistance)
                {
                    _totalDistance = value;
                    _runSession.TotalDistance = value;
                    OnPropertyChanged("TotalDistance");
                }
            }
        }

        private TimeSpan? _elapsedTime;
        public TimeSpan? ElapsedTime
        {
            [DebuggerStepThrough]
            get
            {
                return _elapsedTime.GetValueOrDefault();
            }
            set
            {
                if (value != _elapsedTime)
                {
                    _elapsedTime = value;
                    OnPropertyChanged("ElapsedTime");
                }
            }
        }

        private double _currentSpeed;
        public double CurrentSpeed
        {
            get { return _currentSpeed; }
            set
            {
                if (value != _currentSpeed)
                {
                    _currentSpeed = value;
                    _runSession.CurrentSpeed = value;
                    OnPropertyChanged("CurrentSpeed");
                }
            }
        }

        private double _topSpeed;
        public double TopSpeed
        {
            get { return _topSpeed; }
            set
            {
                if (value != _topSpeed)
                {
                    _topSpeed = value;
                    _runSession.TopSpeed = _topSpeed;
                    OnPropertyChanged("TopSpeed");
                }
            }
        }


        private TimeSpan _pace;
        public TimeSpan Pace
        {
            [DebuggerStepThrough]
            get { return _pace; }
            set
            {
                if (value != _pace)
                {
                    _pace = value;
                    _runSession.Pace = _pace.ToString();
                    OnPropertyChanged("Pace");
                }
            }
        }

        private string _bestMile;
        public string BestMile
        {
            [DebuggerStepThrough]
            get { return _bestMile; }
            set
            {
                if (value != _bestMile)
                {
                    _bestMile = value;
                    OnPropertyChanged("BestMile");
                }
            }
        }



        private double _averageSpeed;
        public double AverageSpeed
        {
            [DebuggerStepThrough]
            get
            {
                return _averageSpeed;
            }
            set
            {
                if (value != _averageSpeed)
                {
                    _averageSpeed = value;
                    OnPropertyChanged("AverageSpeed");
                }
            }
        }

        private GeoCoordinate _currentLocation = new GeoCoordinate();
        public GeoCoordinate CurrentLocation
        {
            [DebuggerStepThrough]
            get
            {
                return _currentLocation;
            }
            set
            {

                _currentLocation = value;
                OnPropertyChanged("CurrentLocation");
            }
        }

        private void RaisePositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(this, EventArgs.Empty);
            }
        }


        private GeoCoordinateCollection _routeLocations;
        public GeoCoordinateCollection RouteLocations
        {
            [DebuggerStepThrough]
            get
            {
                return _routeLocations;
            }
            set
            {
                if (value != _routeLocations)
                {
                    _routeLocations = value;
                    OnPropertyChanged("RouteLocations");
                }
            }
        }

        private double _heading;
        public double Heading
        {
            [DebuggerStepThrough]
            get { return _heading; }
            set
            {
                if (value != _heading)
                {
                    _heading = value;
                    OnPropertyChanged("Heading");
                }
            }
        }


        private PositionStatus _gpsStatus;
        public PositionStatus GpsStatus
        {
            [DebuggerStepThrough]
            get
            {
                return _gpsStatus;
            }
            set
            {
                if (value != _gpsStatus)
                {
                    _gpsStatus = value;
                    OnPropertyChanged("GpsStatus");
                }
            }
        }

        private DistanceUnit _distanceUnit;
        public DistanceUnit DistanceUnit
        {
            [DebuggerStepThrough]
            get
            {
                return _distanceUnit;
            }
            set
            {
                if (value != _distanceUnit)
                {
                    _distanceUnit = value;
                    OnPropertyChanged("DistanceUnit");
                }
            }
        }

        private bool _isSessionActive;
        public bool IsSessionActive
        {
            get { return _isSessionActive; }
            set
            {
                if (value != _isSessionActive)
                {
                    _isSessionActive = value;
                    OnPropertyChanged("IsSessionActive");
                    OnPropertyChanged("CanStartRunSession");
                }
            }
        }



        public bool IsSessionInactive
        {
            get { return !IsSessionActive; }
        }

        public event EventHandler PositionChanged;
        public event EventHandler<RunSessionCompleteEventArgs> RunSessionComplete;


        public DelegateCommand StartRunSessionCommand
        {
            get;
            protected set;
        }

        public DelegateCommand EndRunSessionCommand
        {
            get;
            protected set;
        }

        public bool IsUserLoggedIn
        {
            get { return _isUserLoggedIn; }
            set { _isUserLoggedIn = value; }
        }

        public void UpdateUserId(string userId)
        {
            _runSession.UserId = userId;
        }

        #endregion

        public void GetWaypoints()
        {
            Waypoints = new ObservableCollection<RunSessionWaypoint>(_runSession.Waypoints);
        }

        #region Construction


        //Create new session
        public RunSessionViewModel(RunJammerApplicationDataProvider dataProvider, RunSession newRunSession, UILogger logger, bool isUserLoggedIn)
            : base(logger)
        {
            IsUserLoggedIn = isUserLoggedIn;
            _runSession = newRunSession;
            TotalDistance = _runSession.TotalDistance;

            if (IsUserLoggedIn)
            {
                // _azureRunSession = new AzureRunSession();
            }

            if (newRunSession.StartTime != null)

                if (newRunSession.LastUpdateTime != null)
                    ElapsedTime = newRunSession.LastUpdateTime.Value - newRunSession.StartTime.Value;
            InitializeGeolocator();
            _dataProvider = dataProvider;
            Initialize();
        }

        private void InitializeGeolocator()
        {
            _geolocator = new Geolocator();
            _geolocator.ReportInterval = 600;
            _geolocator.DesiredAccuracyInMeters = 100;
            if (_geolocator.LocationStatus == PositionStatus.Disabled)
            {
                IsLocationDisabled = true;
            }


            //var currentPosition = await _geolocator.GetGeopositionAsync();
            //CurrentLocation = currentPosition.Coordinate.ToGeoCoordinate();
            _geolocator.StatusChanged += HandleGeolocatorStatusChanged;
        }

        private async void HandleGeolocatorPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var geocoordinate = args.Position.Coordinate;

            if (_lastPositionUpdate != null)
            {
                if (_lastPositionUpdate.Latitude == geocoordinate.Latitude &&
                    _lastPositionUpdate.Longitude == geocoordinate.Longitude) return;
            }
            if (geocoordinate != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                     {
                         if (!_isSessionActive)
                         {
                             UpdateCurrentLocation(geocoordinate);
                             return;
                         }
                         if (LocationDataIsAccurate(geocoordinate, 10))
                         {

                             _goodPositionUpdates.Add(geocoordinate);
                             _currentMetersPerSecond = geocoordinate.Speed.HasValue ? geocoordinate.Speed.Value : 0;
                             UpdateRunSessionWaypoints((geocoordinate));
                             if (IsSessionActive)
                             {
                                 if (!IsRunningInBackground && !_isUpdatingRunStats)
                                 {
                                     _isUpdatingRunStats = true;
                                     UpdateTotalDistance(geocoordinate);
                                     UpdateCurrentSpeed();
                                     _isUpdatingRunStats = false;
                                 }
                                 else
                                 {

                                     CalculateTotalDistance(geocoordinate.ToGeoCoordinate());
                                     CalculateCurrentSpeed();
                                     //CalculateCurrentPace();
                                 }
                                 RouteLocations.Add(CurrentLocation);
                             }

                             _lastPositionUpdate = geocoordinate;
                             UpdateCurrentLocation(geocoordinate);

                             if (_splitsRecorder != null)
                             {
                                 _splitsRecorder.Update();
                             }
                         }
                         else
                         {
                             _discardedPositionUpdates.Add(geocoordinate);
                         }
                     });
            }
        }

        private void UpdateCurrentLocation(Geocoordinate geocoordinate)
        {
            CurrentLocation = geocoordinate.ToGeoCoordinate();
            if (!IsLocationDataReady)
            {
                IsLocationDataReady = true;
            }
        }

        private void HandleGeolocatorStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            Deployment.Current.Dispatcher.BeginInvoke(async () =>
            {
                GpsStatus = args.Status;
                if (args.Status == PositionStatus.Ready)
                {

                    //var currentPosition = await _geolocator.GetGeopositionAsync();
                    _geolocator.PositionChanged += HandleGeolocatorPositionChanged;

                    //CurrentLocation = currentPosition.Coordinate.ToGeoCoordinate();
                }

                IsLocationDisabled = (args.Status == PositionStatus.Disabled);
            });
        }

        //View existing session
        public RunSessionViewModel(RunSession existingSession)
        {
            _runSession = existingSession;
            AverageSpeed = _runSession.AverageSpeed;
            DistanceUnit = _runSession.DistanceUnit;
            ElapsedTime = _runSession.EndTime - _runSession.StartTime;
            TimeSpan pace;
            if (TimeSpan.TryParse(_runSession.Pace, out pace))
            {
                Pace = pace;
            }
            TotalDistance = _runSession.TotalDistance;
            Waypoints = new ObservableCollection<RunSessionWaypoint>(_runSession.Waypoints.ToList());
        }

        #endregion

        #region Implementation



        #region Command Implementation

        #region StartRunSessionCommand

        private void ExecuteStartRunSession()
        {
            InitializeSessionDurationTimer();

            if (_runSession.LocalID == 0)
            {
                InitializeRunSession();
            }

            InitializeDbUpdateTimer();
            //await InitializeSplitsRecorder();
        }



        private async void InitializeRunSession()
        {
            SetDistanceUnit();
            _runSession.StartTime = DateTime.Now;


            InitializeSplitsRecorder();
            SplitsViewModel = new SplitsViewModel(_runSession);
            IsSessionActive = true;
            if (SessionStarted != null)
                SessionStarted(this, EventArgs.Empty);
            _dataProvider.CreateRunSession(_runSession, IsUserLoggedIn);

            await UpdateRunSessionAsync();
        }

        private void InitializeSplitsRecorder()
        {
            _splitsRecorder = new RunSessionSplitsRecorder(_runSession);
            _splitsRecorder.RunSessionSplitCompleted += HandleRunSessionSplitCompleted;
            _splitsRecorder.Start();
        }

        private bool CanExecuteStartRunSession()
        {
            return CanStartRunSession;
        }

        #endregion

        #endregion

        #region Helper Methods

        private void Initialize()
        {
            //InitializeUpdateTimer();
            Waypoints = new ObservableCollection<RunSessionWaypoint>();
            _goodPositionUpdates = new List<Geocoordinate>();
            _discardedPositionUpdates = new List<Geocoordinate>();
            InitializeCommands();

            RouteLocations = new GeoCoordinateCollection();
        }

        private void HandleRunSessionSplitCompleted(object sender, RunSessionSplitCompletedEventArgs e)
        {
            if (_runSession.Splits == null)
            {
                _runSession.Splits = new EntitySet<RunSessionSplit>();
            }
            _runSession.Splits.Add(e.Split);
            SplitsViewModel.UpdateSplits();
        }

        private void SetDistanceUnit()
        {
            DistanceUnit = Thread.CurrentThread.CurrentCulture.Name == "en-US" ? DistanceUnit.Mile : DistanceUnit.Kilometre;
            _runSession.DistanceUnit = DistanceUnit;
        }


        private async void InitializeCommands()
        {
            StartRunSessionCommand = new DelegateCommand(ExecuteStartRunSession);
            EndRunSessionCommand = new DelegateCommand(ExecuteEndRunSession);
        }

        private void InitializeSessionDurationTimer()
        {
            _sessionDurationTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _sessionDurationTimer.Tick += HandleSessionDurationTimerTick;
            _sessionDurationTimer.Start();
        }

        private void InitializeDbUpdateTimer()
        {
            _dbUpdateTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(4) };
            _dbUpdateTimer.Tick += async (sender, args) => await UpdateRunSessionAsync();
            _dbUpdateTimer.Start();
        }


        /// <summary>
        /// Calculates the elapsed time and updates the UI. Used when app is running in foreground and can update UI.
        /// </summary>
        private void UpdateElapsedTime()
        {
            //Calculate
            CalculateElapsedTime();

            //Update the UI.
            Deployment.Current.Dispatcher.BeginInvoke(() => OnPropertyChanged("ElapsedTime"));
        }

        /// <summary>
        /// Calculates the elapsed time but doesn't update the UI. Used when app is running in background and can't update UI.
        /// </summary>
        private void CalculateElapsedTime()
        {
            var runSessionStartTime = new DateTimeOffset(_runSession.StartTime.Value).ToLocalTime();

            var newTime = DateTime.Now.TimeOfDay - runSessionStartTime.TimeOfDay;

            _elapsedTime = newTime;
        }

        private void UpdateTotalDistance(Geocoordinate position)
        {
            CalculateTotalDistance(position.ToGeoCoordinate());
            Deployment.Current.Dispatcher.BeginInvoke(() => OnPropertyChanged("TotalDistance"));
        }

        private void CalculateTotalDistance(GeoCoordinate position)
        {
            if (_lastPositionUpdate != null)
            {
                var geoCoordinate = _lastPositionUpdate.ToGeoCoordinate();

                GeoCoordinate targetGeoCoordinate;

                if (_lastPositionUpdate.Latitude == geoCoordinate.Latitude &&
                    _lastPositionUpdate.Longitude == geoCoordinate.Longitude && _goodPositionUpdates.Count > 1)
                {
                    targetGeoCoordinate =
                        _goodPositionUpdates[_goodPositionUpdates.IndexOf(_goodPositionUpdates.LastOrDefault()) - 1]
                            .ToGeoCoordinate();
                }
                else
                {
                    targetGeoCoordinate = _lastPositionUpdate.ToGeoCoordinate();
                }

                double distanceChange = DistanceUnit == DistanceUnit.Mile
                    ? position.GetDistanceTo(targetGeoCoordinate) * 0.000621371192
                    : position.GetDistanceTo(targetGeoCoordinate) / 1000;

                _totalDistance += distanceChange;
                if (string.Format("{0:0.00}", _totalDistance) != string.Format("{0:0.00}", _runSession.TotalDistance))
                {
                    _runSession.TotalDistance = _totalDistance;
                }
            }
        }


        private TimeSpan CalculateAveragePace()
        {
            TimeSpan ret;

            if (_totalDistance == 0d) return ret;

            if (_totalDistance < 1)
            {
                var multiplier = 1 / _totalDistance;
                ret = TimeSpan.FromMinutes(ElapsedTime.GetValueOrDefault().TotalMinutes * multiplier);
            }
            else
            {
                ret = TimeSpan.FromMinutes(_elapsedTime.GetValueOrDefault().TotalMinutes / _totalDistance);
            }

            _pace = ret;
            _runSession.Pace = _pace.ToString();

            return ret;
        }

        private double CalculateAverageSpeed()
        {

            if (ElapsedTime.GetValueOrDefault().TotalHours < 1)
            {
                var multiplier = 60 / ElapsedTime.GetValueOrDefault().TotalMinutes;
                return TotalDistance * multiplier;
            }
            else
            {
                return TotalDistance / ElapsedTime.GetValueOrDefault().TotalHours;
            }
        }

        private void CalculateCurrentSpeed()
        {
            try
            {
                _currentSpeed = _distanceUnit == DistanceUnit.Mile ? CalculateCurrentMilesPerHour() : CalculateCurrentKilometersPerHour();
                if (double.IsNaN(_currentSpeed))
                    _currentSpeed = 0;
                _runSession.CurrentSpeed = _currentSpeed;
                if (_currentSpeed > _topSpeed)
                {
                    _topSpeed = _currentSpeed;
                    _runSession.TopSpeed = _topSpeed;
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error calculating current speed.", ex));
                Logger.Log(ex);
            }
        }

        private void CalculateCurrentPace()
        {
            try
            {
                //if (_totalDistance == 0d) return;
                //TimeSpan value;

                //if (_totalDistance < 1)
                //{
                //    var multiplier = 1 / _totalDistance;
                //    value = TimeSpan.FromMinutes(ElapsedTime.GetValueOrDefault().TotalMinutes * multiplier);
                //}
                //else
                //{
                //    value = TimeSpan.FromMinutes(_elapsedTime.GetValueOrDefault().TotalMinutes / _totalDistance);
                //}

                //_pace = value;
                //_runSession.Pace = _pace.ToString();

                if (CurrentSpeed > 0d)
                {
                    _pace = TimeSpan.FromMinutes(60 / CurrentSpeed);
                }
                _runSession.Pace = _pace.ToString();
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error calculating current pace", ex));
                Logger.Log(ex);
            }
        }

        private double CalculateCurrentMilesPerHour()
        {
            try
            {
                return CalculateCurrentKilometersPerHour() * 0.621371192;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return 0;
            }
        }

        private double CalculateCurrentKilometersPerHour()
        {
            try
            {
                var metersPerHour = _currentMetersPerSecond * 3600;
                return metersPerHour / 1000;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
                return 0;
            }
        }

        private async void ExecuteEndRunSession()
        {
            IsSessionActive = false;
            _sessionDurationTimer.Stop();
            _dbUpdateTimer.Stop();
            _geolocator.PositionChanged -= HandleGeolocatorPositionChanged;
            _geolocator.StatusChanged -= HandleGeolocatorStatusChanged;

            if (_runSession.Waypoints != null && _runSession.Waypoints.Any())
            {
                try
                {
                    _runSession.TopSpeed = TopSpeed;
                    UpdateAverageSpeed();
                    _runSession.Pace = CalculateAveragePace().ToString();
                    _runSession.EndTime = DateTime.Parse(DateTime.Now.ToLongTimeString());
                    _runSession.UserEndedSession = true;

                    var mileSplits = _runSession.Splits.Where(sp => sp.DistanceUnit == "Mile" && sp.Measurement == 1).ToList();


                    if (mileSplits.Any())
                    {
                        var orderedSplits = mileSplits.OrderBy(sp => TimeSpan.Parse(sp.Duration).TotalMinutes).ToList();

                        var runSessionSplit = orderedSplits.FirstOrDefault();
                        if (runSessionSplit != null)
                            BestMile = string.Format("{0:mm\\:ss\\:fff}", TimeSpan.Parse(runSessionSplit.Duration));
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }

                await UpdateRunSessionAsync();
            }

            if (RunSessionComplete != null)
            {
                RunSessionComplete(this, new RunSessionCompleteEventArgs { RunSession = _runSession });
            }

            _runSession = new RunSession();
        }

        #endregion

        #region Event Handlers

        public event EventHandler SessionStarted;
        private bool _isUpdatingRunStats;


        private async void HandleSessionDurationTimerTick(object sender, EventArgs e)
        {
            try
            {
                _runSession.LastUpdateTime = DateTime.Now;
                if (!IsRunningInBackground)
                {
                    UpdateElapsedTime();
                    UpdateAveragePace();
                }
                else
                {
                    CalculateElapsedTime();
                    CalculateAveragePace();
                }
                _splitsRecorder.Update();
            }
            catch (Exception ex)
            {

                Logger.Log(ex);
                Logger.Log(ex);
            }
        }

        public void Reactivate()
        {
            IsRunningInBackground = false;
            OnPropertyChanged("ElapsedTime");
            OnPropertyChanged("TotalDistance");
            OnPropertyChanged("CurrentSpeed");
            OnPropertyChanged("TopSpeed");
            OnPropertyChanged("CurrentPace");
        }

        private void UpdateAverageSpeed()
        {
            AverageSpeed = CalculateAverageSpeed();
            _runSession.AverageSpeed = AverageSpeed;
        }

        private void UpdateCurrentSpeed()
        {
            CalculateCurrentSpeed();


            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                OnPropertyChanged("CurrentSpeed");
                OnPropertyChanged("TopSpeed");
            });
        }

        private void UpdateAveragePace()
        {
            try
            {
                CalculateAveragePace();
                Deployment.Current.Dispatcher.BeginInvoke(() => OnPropertyChanged("Pace"));
            }
            catch (Exception ex)
            {

                Logger.Log(new Exception("Error updating Pace.", ex));
                Logger.Log(ex);
            }
        }

        private void UpdateRunSessionWaypoints(Geocoordinate geocoordinate)
        {
            try
            {
                var waypoint = GetRunSessionWaypointFromGeocoordinate(geocoordinate);
                if (!_isSavingToDb)
                {
                    _runSession.Waypoints.Add(waypoint);
                }
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    try
                    {

                        Waypoints.Add(waypoint);
                    }
                    catch (Exception ex)
                    {


                        Logger.Log(new Exception("Error updating waypoints", ex));
                        Logger.Log(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error updating waypoints", ex));
                Logger.Log(ex);
            }
        }

        private RunSessionWaypoint GetRunSessionWaypointFromGeocoordinate(Geocoordinate geocoordinate)
        {
            CalculateCurrentSpeed();
            var waypoint = new RunSessionWaypoint
            {
                SessionID = _runSession.ID,
                Lat = geocoordinate.Latitude,
                Lon = geocoordinate.Longitude,
                Altitude = geocoordinate.Altitude.GetValueOrDefault(),
                Speed = CurrentSpeed,
                Pace = CalculateAveragePace().ToString()
            };
            waypoint.RunSession = _runSession;
            return waypoint;
        }

        private bool LocationDataIsAccurate(Geocoordinate location, int accuracy)
        {
            return location.Accuracy <= accuracy;
        }

        private async Task UpdateRunSessionAsync()
        {
            Debug.WriteLine("Start update run session" + DateTime.Now);
            try
            {
                if (!_isSavingToDb)
                {
                    _isSavingToDb = true;
                    try
                    {
                        Debug.WriteLine("Start Submit Changes " + DateTime.Now);
                        await _dataProvider.Update(_runSession, IsUserLoggedIn);
                        Debug.WriteLine("End Submit Changes " + DateTime.Now);
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(new Exception("Error submitting changes " + ElapsedTime + ex));
                        Logger.Log(ex);
                    }
                    finally
                    {
                        _isSavingToDb = false;
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
            Debug.WriteLine("End update run session" + DateTime.Now);
        }

        #endregion

        #endregion
    }
}
