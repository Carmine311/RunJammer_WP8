using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Windows.Devices.Geolocation;
using Common.Model.Logging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.WindowsAzure.MobileServices;
using RunJammer.ModelManagement;
using RunJammer.WP.DataAccess;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Model;
using System.Device.Location;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.ViewModel
{
    public class RunJammerApplicationViewModel : ViewModelBase
    {

        private LocalDbDataProvider _localDbDataProvider;
        private RunJammerMobileServiceClient _mobileServiceClient;

        private RunJammerUser _loggedInUser;


        #region Properties

        public DelegateCommand StartRunSessionCommand { get; protected set; }
        public DelegateCommand<string> LoginCommand { get; set; }


        public UILogger Logger { get; set; }

        private DateTime _lastApplicationRunTime;
        public DateTime LastApplicationRunTime
        {
            get { return _lastApplicationRunTime; }
            set
            {
                if (value != _lastApplicationRunTime)
                {
                    _lastApplicationRunTime = value;
                    OnPropertyChanged("LastApplicationRunTime");
                }
            }
        }

        private SongIndexingProcessViewModel _songIndexingProcessViewModel;
        public SongIndexingProcessViewModel SongIndexingProcessViewModel
        {
            get { return _songIndexingProcessViewModel; }
            set
            {
                if (value != _songIndexingProcessViewModel)
                {
                    _songIndexingProcessViewModel = value;
                    OnPropertyChanged("SongIndexingProcessViewModel");
                }
            }
        }

        private GreetingViewModel _greetingViewModel;
        public GreetingViewModel GreetingViewModel
        {
            get { return _greetingViewModel; }
            set
            {
                if (value != _greetingViewModel)
                {
                    _greetingViewModel = value;
                    OnPropertyChanged("GreetingViewModel");
                }
            }
        }

        private RunSessionViewModel _runSessionViewModel;
        public RunSessionViewModel RunSessionViewModel
        {
            get { return _runSessionViewModel; }
            set
            {
                if (value != _runSessionViewModel)
                {
                    _runSessionViewModel = value;
                    OnPropertyChanged("RunSessionViewModel");
                }
            }
        }

        private RunJammerJukeBoxViewModel _runJammerJukeBoxViewModel;
        public RunJammerJukeBoxViewModel RunJammerJukeBoxViewModel
        {
            get { return _runJammerJukeBoxViewModel; }
            set
            {
                if (value != _runJammerJukeBoxViewModel)
                {
                    _runJammerJukeBoxViewModel = value;
                    OnPropertyChanged("RunJammerJukeBoxViewModel");
                }
            }
        }

        private RunSessionStatsSummaryViewModel _runSessionStatsSummaryViewModel;
        public RunSessionStatsSummaryViewModel RunSessionStatsSummaryViewModel
        {
            get { return _runSessionStatsSummaryViewModel; }
            set
            {
                if (value != _runSessionStatsSummaryViewModel)
                {
                    _runSessionStatsSummaryViewModel = value;
                    OnPropertyChanged("RunSessionStatsSummaryViewModel");
                }
            }
        }

        private ObservableCollection<RunSessionViewModel> _runSessions;
        public ObservableCollection<RunSessionViewModel> RunSessions
        {
            get { return _runSessions; }
            set
            {
                if (value != _runSessions)
                {
                    _runSessions = value;
                    OnPropertyChanged("RunSessions");
                }
            }
        }

        private RunSessionViewModel _selectedRunSession;
        public RunSessionViewModel SelectedRunSession
        {
            get { return _selectedRunSession; }
            set
            {
                if (value != _selectedRunSession)
                {
                    _selectedRunSession = value;
                    if (_selectedRunSession != null)
                    {
                        foreach (var runJammerSongViewModel in _selectedRunSession.Songs)
                        {
                            runJammerSongViewModel.GetHiResDisplayImage();
                            runJammerSongViewModel.CalculateAverageSpeed(_selectedRunSession.Waypoints);
                        }
                    }
                    OnPropertyChanged("SelectedRunSession");
                }
            }
        }

        private bool _runTrackingModeSelected;
        public bool RunTrackingModeSelected
        {
            get { return _runTrackingModeSelected; }
            set
            {
                if (value != _runTrackingModeSelected)
                {
                    _runTrackingModeSelected = value;
                    OnPropertyChanged("RunTrackingModeSelected");
                }
            }
        }

        private bool _runSessionIsActive;
        public bool RunSessionIsActive
        {
            get { return _runSessionIsActive; }
            set
            {
                if (value != _runSessionIsActive)
                {
                    _runSessionIsActive = value;
                    OnPropertyChanged("RunSessionIsActive");
                }
            }
        }

        private bool _displayStartScreen;
        public bool DisplayStartScreen
        {
            get { return _displayStartScreen; }
            set
            {
                if (value != _displayStartScreen)
                {
                    _displayStartScreen = value;
                    OnPropertyChanged("DisplayStartScreen");
                }
            }
        }

        private bool _isFirstRun = true;
        public bool IsFirstRun
        {
            get { return _isFirstRun; }
            set
            {
                if (value != _isFirstRun)
                {
                    _isFirstRun = value;
                    OnPropertyChanged("IsFirstRun");
                }
            }
        }

        private bool _isUserLoggedIn;
        public bool IsUserLoggedIn
        {
            get { return _isUserLoggedIn; }
            set
            {
                if (value != _isUserLoggedIn)
                {
                    _isUserLoggedIn = value;
                    OnPropertyChanged("IsUserLoggedIn");
                }
            }
        }

        #endregion

        #region Public Methods

        public void HandleRunningInBackground()
        {
            if (RunSessionViewModel != null && RunSessionViewModel.IsSessionActive)
            {
                RunSessionViewModel.IsRunningInBackground = true;
            }
        }

        public void HandleAppReactivated()
        {
            if (RunSessionViewModel != null && RunSessionViewModel.IsSessionActive)
            {
                RunSessionViewModel.Reactivate();
            }
        }

        public bool ExitOnBackButtonPress()
        {
            return DisplayStartScreen;
        }


        #endregion

        public RunJammerApplicationViewModel()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                Initialize();
            }
        }

        #region Implementation

        #region Fields

        private RunJammerApplicationDataProvider _dataProvider;
        private RunJammerMusicManager _musicManager;
        private const string LastApplicationRunTimeKey = "LastApplicationRunTime";
        private GeoCoordinate _currentLocation;

        #endregion

        #endregion

        private void Initialize()
        {
            Logger = new UILogger();

            InitializeCommands();

            SetLastApplicationRunTime();

            InitializeDataAccessComponents();
            InitializeMusicManager();

            InitializeRunSessionData();
            DisplayStartScreen = true;

            

            InitializeRunSession();

            if (_localDbDataProvider.GetData<RunJammerUser>().Any())
            {
                _loggedInUser = _localDbDataProvider.GetData<RunJammerUser>().FirstOrDefault();
                if (_loggedInUser != null)
                {
                    IsUserLoggedIn = true;
                    RunSessionViewModel.IsUserLoggedIn = true;
                    RunSessionViewModel.UpdateUserId(_loggedInUser.UserID);
                    RunJammerJukeBoxViewModel.UserId = _loggedInUser.UserID;
                }
            }
        }

        private void InitializeRunSessionData()
        {
            var runSessions = _localDbDataProvider.GetData<RunSession>().ToList();
            RunSessions = new ObservableCollection<RunSessionViewModel>(runSessions.Select(s => new RunSessionViewModel(s)));
            RunSessionStatsSummaryViewModel = new RunSessionStatsSummaryViewModel(_localDbDataProvider);
        }

        private void InitializeCommands()
        {
            StartRunSessionCommand = new DelegateCommand(ExecuteRunSessionStarted);
            LoginCommand = new DelegateCommand<string>(ExecuteLoginCommand);
        }

        private async void ExecuteLoginCommand(string loginProvider)
        {
            MobileServiceUser user = null;

            if (loginProvider.ToLower() == "facebook")
            {
                user = await _mobileServiceClient.LoginAsync(MobileServiceAuthenticationProvider.Facebook);
            }
            if (loginProvider.ToLower() == "microsoft")
            {
                user = await _mobileServiceClient.LoginAsync(MobileServiceAuthenticationProvider.MicrosoftAccount);
            }

            if (user != null)
            {
                //Remove existing id and update with new one.
                if (IsolatedStorageSettings.ApplicationSettings.Contains("UserID"))
                {
                    IsolatedStorageSettings.ApplicationSettings.Remove("UserID");
                }

                IsolatedStorageSettings.ApplicationSettings.Add("UserID", user.UserId);
                await _dataProvider.CreateRunJammerUserAsync(new RunJammerUser { UserID = user.UserId });
                IsUserLoggedIn = true;
                RunSessionViewModel.IsUserLoggedIn = true;
                RunSessionViewModel.UpdateUserId(user.UserId);
                RunJammerJukeBoxViewModel.UserId = user.UserId;
            }
        }

        private RunSession _unfinishedSession;
        private void CheckForUnfinishedSession()
        {
            try
            {
                var ufs = _dataProvider.GetRunSessionHistory().FirstOrDefault(s => !s.UserEndedSession);
                if (ufs != null)
                {
                    _unfinishedSession = ufs;
                    var sessionResolutionDialog = new CustomMessageBox();
                    sessionResolutionDialog.Title = "Unfinished Run Session";
                    sessionResolutionDialog.Content = ufs;
                    sessionResolutionDialog.Message =
                        "It looks like the app closed before you finished your session. We're sorry about that. Press the back button to pretend like it never happened, or choose an option below.";
                    var resources = Application.Current.Resources;
                    sessionResolutionDialog.ContentTemplate =
                        Application.Current.Resources["RunSessionResolutionDataTemplate"] as DataTemplate;
                    sessionResolutionDialog.LeftButtonContent = "End Session";
                    sessionResolutionDialog.RightButtonContent = "Continue Session";
                    sessionResolutionDialog.Dismissed += HandleSessionResolutionDismissed;
                    sessionResolutionDialog.Show();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error checking for unfinished sesssion.", ex));
            }
        }

        private async void HandleSessionResolutionDismissed(object sender, DismissedEventArgs e)
        {
            if (e.Result == CustomMessageBoxResult.RightButton)
            {
                RunSessionViewModel = new RunSessionViewModel(_dataProvider, _unfinishedSession, Logger, IsUserLoggedIn);
            }
            if (e.Result == CustomMessageBoxResult.LeftButton)
            {
                _unfinishedSession.UserEndedSession = true;
                if (IsUserLoggedIn)
                {
                    _unfinishedSession.UserId = _loggedInUser.UserID;
                }
                await _dataProvider.Update(_unfinishedSession, IsUserLoggedIn);
            }
            if (e.Result == CustomMessageBoxResult.None)
            {
                _dataProvider.Delete(_unfinishedSession);
            }
        }

        private void SetLastApplicationRunTime()
        {
            DateTime now = DateTime.Now;
            DateTime lastRunTime;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(LastApplicationRunTimeKey, out lastRunTime))
            {
                IsFirstRun = false;
                IsolatedStorageSettings.ApplicationSettings[LastApplicationRunTimeKey] = now;
            }
            else
            {

                IsFirstRun = true;
                IsolatedStorageSettings.ApplicationSettings.Add(LastApplicationRunTimeKey, LastApplicationRunTime);
            }
            LastApplicationRunTime = now;

        }

        private void InitializeMusicManager()
        {
            try
            {
                _musicManager = new RunJammerMusicManager(_dataProvider, Logger);
                _musicManager.Initialize();
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error initializing music manager.", ex));
            }
        }

        private void InitializeRunSession()
        {
            var runSession = new RunSession();

            if (IsUserLoggedIn)
            {
                runSession.UserId = _loggedInUser.UserID;
            }

            runSession.Waypoints.CollectionChanged += HandleWaypointCollectionChanged;

            RunJammerJukeBoxViewModel = new RunJammerJukeBoxViewModel(_musicManager, runSession, _dataProvider, Logger, !string.IsNullOrEmpty(runSession.UserId) ? runSession.UserId : string.Empty);
            RunSessionViewModel = new RunSessionViewModel(_dataProvider, runSession, Logger, IsUserLoggedIn);
            RunSessionViewModel.RunSessionComplete += HandleRunSessionComplete;

            DisplayStartScreen = false;
        }

        private void ExecuteRunSessionStarted()
        {
            CheckForUnfinishedSession();
            RunJammerJukeBoxViewModel.Initialize();
        }

        private void HandleRunSessionComplete(object sender, Messaging.RunSessionCompleteEventArgs e)
        {
            RunSessionIsActive = false;

            SelectedRunSession = RunSessionViewModel;
            RunSessionStatsSummaryViewModel.UpdateSessionStats();

            //Re-initialize for another run.
            var runSession = new RunSession();
            runSession.Waypoints.CollectionChanged += HandleWaypointCollectionChanged;
            RunSessionViewModel = new RunSessionViewModel(_dataProvider, runSession, Logger, IsUserLoggedIn);
            RunSessionViewModel.RunSessionComplete += HandleRunSessionComplete;

        }

        private void HandleWaypointCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                var waypoint = e.NewItems[0] as RunSessionWaypoint;
                if (waypoint != null && RunJammerJukeBoxViewModel.CurrentRunJammerSongViewModel != null)
                {
                    waypoint.CurrentSong = RunJammerJukeBoxViewModel.CurrentRunJammerSongViewModel.GetRunJammerSong();
                }
            }
            catch (Exception ex)
            {

                Logger.Log(new Exception(ex.Message, new Exception("Error updating Waypoints. Source: " + this)));
            }

        }

        private void InitializeDataAccessComponents()
        {
            _localDbDataProvider = new LocalDbDataProvider(new RunJammerDataContext());
            _mobileServiceClient = new RunJammerMobileServiceClient(Logger);
            _dataProvider = new RunJammerApplicationDataProvider(_localDbDataProvider, _mobileServiceClient);
        }

    }
}
