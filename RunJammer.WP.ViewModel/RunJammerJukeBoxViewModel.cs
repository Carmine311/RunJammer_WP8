using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Common.Model.Logging;
using Microsoft.Xna.Framework.Media;
using RunJammer.ModelManagement;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Messaging;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.ViewModel
{
    public class RunJammerJukeBoxViewModel : LoggerViewModelBase
    {
        private string _userID;

        #region Interface

        #region Properties

        private ObservableCollection<BitmapImage> _albumCoversImages;
        public ObservableCollection<BitmapImage> AlbumCovers
        {
            get { return _albumCoversImages; }
            set
            {
                if (value != _albumCoversImages)
                {
                    _albumCoversImages = value;
                    OnPropertyChanged("AlbumCovers");
                }
            }
        }




        private ObservableCollection<RunJammerPlaylistViewModel> _runJammerPlaylists;
        public ObservableCollection<RunJammerPlaylistViewModel> RunJammerPlaylists
        {
            get
            {
                return _runJammerPlaylists;
            }
            set
            {
                if (value != _runJammerPlaylists)
                {
                    _runJammerPlaylists = value;
                    OnPropertyChanged("RunJammerPlaylists");
                }
            }
        }

        private ObservableCollection<RunJammerSongViewModel> _runJammerSongs;
        public ObservableCollection<RunJammerSongViewModel> RunJammerSongs
        {
            get
            {
                return _runJammerSongs;
            }

            set
            {
                if (value != _runJammerSongs)
                {
                    _runJammerSongs = value;
                    OnPropertyChanged("RunJammerSongs");
                }
            }
        }

        private ObservableCollection<AlbumViewModel> _albums;
        public ObservableCollection<AlbumViewModel> Albums
        {
            get { return _albums; }
            set
            {
                if (value != _albums)
                {
                    _albums = value;
                    OnPropertyChanged("Albums");
                }
            }
        }

        private ObservableCollection<ArtistViewModel> _artists;
        public ObservableCollection<ArtistViewModel> Artists
        {
            get { return _artists; }
            set
            {
                if (value != _artists)
                {
                    _artists = value;
                    OnPropertyChanged("Artists");
                }
            }
        }



        private bool _isMediaLibraryEmpty;
        public bool IsMediaLibraryEmpty
        {
            get { return !_runJammerMusicManager.HasSongs(); }
            set
            {
                if (value != _isMediaLibraryEmpty)
                {
                    _isMediaLibraryEmpty = value;
                    OnPropertyChanged("IsMediaLibraryEmpty");
                }
            }
        }


        private ObservableCollection<RunJammerSongViewModel> _nowPlayingCollection;
        public ObservableCollection<RunJammerSongViewModel> NowPlayingCollection
        {
            get { return _nowPlayingCollection; }
            set
            {
                if (value != _nowPlayingCollection)
                {
                    _nowPlayingCollection = value;
                    OnPropertyChanged("NowPlayingCollection");
                }
            }
        }

        private int _currentPlayingIndex;
        public int CurrentPlayingIndex
        {
            get { return _currentPlayingIndex; }
            set
            {
                if (value != _currentPlayingIndex)
                {
                    _currentPlayingIndex = value;
                    OnPropertyChanged("CurrentPlayingIndex");
                }
            }
        }

        private bool _shuffle;
        public bool Shuffle
        {
            get { return _shuffle; }
            set
            {
                if (value != _shuffle)
                {
                    _shuffle = value;
                    OnPropertyChanged("Shuffle");
                }
            }
        }

        private ObservableCollection<RunJammerSong> _currentPlaylistSongs;
        public ObservableCollection<RunJammerSong> CurrentPlaylistSongs
        {
            get
            {
                return _currentPlaylistSongs;
            }
            set
            {
                if (value != _currentPlaylistSongs)
                {
                    _currentPlaylistSongs = value;
                    OnPropertyChanged("CurrentPlaylistSongs");
                }
            }
        }

        private RunJammerSongViewModel _currentRunJammerSongViewModel;
        public RunJammerSongViewModel CurrentRunJammerSongViewModel
        {
            get
            {
                return _currentRunJammerSongViewModel;
            }
            set
            {
                if (value != _currentRunJammerSongViewModel)
                {
                    _currentRunJammerSongViewModel = value;
                    OnPropertyChanged("CurrentRunJammerSongViewModel");
                }
            }
        }

        private bool _isLoadingNextSong;
        public bool IsLoadingNextSong
        {
            get
            {
                return _isLoadingNextSong;
            }
            set
            {
                if (value != _isLoadingNextSong)
                {
                    _isLoadingNextSong = value;
                    OnPropertyChanged("IsLoadingNextSong");
                }
            }
        }

        private bool _nextSongLoaded;
        public bool NextSongLoaded
        {
            get
            {
                return _nextSongLoaded;
            }
            set
            {
                if (value != _nextSongLoaded)
                {
                    _nextSongLoaded = value;
                    OnPropertyChanged("NextSongLoaded");
                }
            }
        }

        private bool _isLoadingPreviousSong;
        public bool IsLoadingPreviousSong
        {
            get
            {
                return _isLoadingPreviousSong;
            }
            set
            {
                if (value != _isLoadingPreviousSong)
                {
                    _isLoadingPreviousSong = value;
                    OnPropertyChanged("IsLoadingPreviousSong");
                }
            }
        }

        private MediaState _currentMediaState;
        public MediaState CurrentMediaState
        {
            get
            {
                return _currentMediaState;
            }
            set
            {
                if (value != _currentMediaState)
                {
                    _currentMediaState = value;
                    OnPropertyChanged("CurrentMediaState");
                    OnPropertyChanged("IsMediaPlaying");
                    PauseCommand.RaiseCanExecuteChanged();
                    PlayCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private bool _isMediaPlaying;
        public bool IsMediaPlaying
        {
            get { return CurrentMediaState == MediaState.Playing; }
            set
            {
                if (value != _isMediaPlaying)
                {
                    OnPropertyChanged("IsMediaPlaying");
                }
            }
        }


        private void HandlePlayPlaylist(object sender, PlayPlaylistEventArgs e)
        {
            if (e.Songs.Any())
            {
                CurrentPlaylistSongs = new ObservableCollection<RunJammerSong>(e.Songs);
                if (MediaPlayer.IsShuffled)
                {
                    ShuffleCollection(CurrentPlaylistSongs);
                }
                MediaPlayer.Play(CurrentPlaylistSongs[_currentPlaylistPlayIndex].GetSong());
            }
        }

        private void ShuffleCollection<T>(ObservableCollection<T> collection)
        {
            try
            {
                Random rng = new Random();
                int n = collection.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    var value = collection[k];
                    collection[k] = collection[n];
                    collection[n] = value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error shuffling collection", ex);
            }
        }

        private void HandleRunJammerSongRunRatingChanged(object sender, RunJammerMusicItemRunRatingChangedEventArgs e)
        {
            var vm = sender as RunJammerSongViewModel;
            UpdateSongRunRating(vm, e.NewRating);
            _dataProvider.CreateUserSongRating(new UserSongRating { AlbumName = CurrentRunJammerSongViewModel.AlbumName, ArtistName = CurrentRunJammerSongViewModel.ArtistName, SongName = CurrentRunJammerSongViewModel.Song.Name, Rating = e.NewRating, AlwaysSkip = CurrentRunJammerSongViewModel.ExcludeFromRunSessions, UserID = UserId, TimeCreated = DateTime.UtcNow });
        }

        private void UpdateSongRunRating(RunJammerSongViewModel runJammerSongViewModel, int newRating)
        {
            try
            {
                if (runJammerSongViewModel.RunRating > 0)
                {
                    RemoveSongFromPlaylist(runJammerSongViewModel);
                }

                AddSongToRatingPlaylist(runJammerSongViewModel, newRating);

                _runJammerMusicManager.UpdateSongRunRating(runJammerSongViewModel.GetRunJammerSong(), newRating);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void AddSongToRatingPlaylist(RunJammerSongViewModel runJammerSongViewModel, int newRating)
        {
            var playlist = RunJammerPlaylists.FirstOrDefault(pl => pl.RunRating == newRating);
            if (playlist != null)
            {
                playlist.RunJammerSongs.Add(runJammerSongViewModel);
            }
        }

        private void RemoveSongFromPlaylist(RunJammerSongViewModel runJammerSongViewModel)
        {
            var playlist = RunJammerPlaylists.FirstOrDefault(pl => pl.RunRating == runJammerSongViewModel.RunRating);

            if (playlist != null)
            {
                playlist.RunJammerSongs.Remove(runJammerSongViewModel);
            }
        }

        #endregion

        #region Commands

        public DelegateCommand NextSongCommand { get; protected set; }
        public DelegateCommand RestartSongCommand { get; protected set; }
        public DelegateCommand PreviousSongCommand { get; protected set; }
        public DelegateCommand PlayCommand { get; protected set; }
        public DelegateCommand PauseCommand { get; protected set; }
        public DelegateCommand ShuffleNowPlayingCollectionCommand { get; protected set; }

        public string UserId
        {
            get { return _userID; }
            set { _userID = value; }
        }

        #endregion

        #region Construction


        public RunJammerJukeBoxViewModel(RunJammerMusicManager runJammerMusicManager, RunSession currentRunSession, RunJammerApplicationDataProvider dataProvider, UILogger logger, string userID = "")
            : base(logger)
        {
            _userID = userID;
            _dataProvider = dataProvider;
            _runJammerMusicManager = runJammerMusicManager;

            _currentRunSession = currentRunSession;
        }

        private void InitializeMediaPlayer()
        {
            NowPlayingCollection = new ObservableCollection<RunJammerSongViewModel>();
            MediaPlayer.ActiveSongChanged += HandleActiveSongChanged;
            if (MediaPlayer.Queue.ActiveSong != null)
            {
                UpdateCurrentSong(MediaPlayer.Queue.ActiveSong);
            }
            CurrentMediaState = MediaPlayer.State;
            MediaPlayer.ActiveSongChanged += HandleActiveSongChanged;
            MediaPlayer.MediaStateChanged += HandleMediaStateChanged;
        }

        private void HandleActiveSongChanged(object sender, EventArgs e)
        {
            try
            {
                var song = MediaPlayer.Queue.ActiveSong;
                if (song != null)
                {

                    UpdateCurrentSong(song);
                    //OrderAlbumsByTotalPlays();
                }
                if (MediaPlayer.Queue.Count > 1)
                {
                    PopulateNowPlayingFromMediaPlayerQueue();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(new Exception("Error handling song change.", ex));
            }
        }

        private void HandleMediaStateChanged(object sender, EventArgs e)
        {
            CurrentMediaState = MediaPlayer.State;
            if (CurrentMediaState == MediaState.Paused)
            {
                if (MediaPlayer.PlayPosition == new TimeSpan(0, 0, 0, 0))
                {
                    if (MediaPlayer.Queue.Count == 1)
                    {
                        if (NowPlayingCollection.Count > 1 && CurrentPlayingIndex < (NowPlayingCollection.Count - 1))
                        {
                            var songVm =
                                NowPlayingCollection.FirstOrDefault(
                                    rjs =>
                                        rjs.Name == MediaPlayer.Queue.ActiveSong.Name &&
                                        rjs.AlbumName == MediaPlayer.Queue.ActiveSong.Album.Name &&
                                        rjs.ArtistName == MediaPlayer.Queue.ActiveSong.Artist.Name);
                            CurrentPlayingIndex++;
                            var nextSong = NowPlayingCollection[CurrentPlayingIndex];
                            if (nextSong != null)
                            {
                                MediaPlayer.Play(nextSong.Song);
                            }
                        }
                        else
                        {
                            ExecuteNextSongCommand();
                        }
                    }
                }
            }
            UpdateCurrentSong(MediaPlayer.Queue.ActiveSong);
        }

        private void UpdateCurrentSong(Song song)
        {
            try
            {
                var songVm =
                            NowPlayingCollection.FirstOrDefault(
                                rjs =>
                                    rjs.Name == song.Name && rjs.AlbumName == song.Album.Name &&
                                    rjs.ArtistName == song.Artist.Name);
                if (songVm == null)
                {
                    songVm = RunJammerSongs.FirstOrDefault(vm => vm.GetRunJammerSong().GetSong() == song);
                }
                CurrentRunJammerSongViewModel = songVm;
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }


        #endregion

        #endregion

        #region Fields

        private RunJammerMusicManager _runJammerMusicManager;
        private RunSession _currentRunSession;
        private RunJammerApplicationDataProvider _dataProvider;

        private int _currentPlaylistPlayIndex;

        #endregion

        #region Methods

        public void Initialize()
        {
            InitializeAlbumCoversAsync();
            RunJammerSongs = new ObservableCollection<RunJammerSongViewModel>();
            RunJammerPlaylists = new ObservableCollection<RunJammerPlaylistViewModel>();
            InitializeCommands();
            InitializeSongs();
            InitializeAlbums();
            InitializeArtistsAsync();
            InitializePlaylists();
            InitializeMediaPlayer();
            InitializeCurrentMedia();
        }

        private async void InitializeAlbumCoversAsync()
        {
            try
            {
                List<BitmapImage> albumCovers = null;

                await Task.Run(() => albumCovers = _runJammerMusicManager.GetAlbumArt().ToList());

                AlbumCovers = new ObservableCollection<BitmapImage>(albumCovers);
            }
            catch (Exception ex)
            {
                throw new Exception("Error initializing album covers", ex);
            }
        }

        private async void InitializeSongs()
        {
            try
            {
                RunJammerSongs = new ObservableCollection<RunJammerSongViewModel>();
                await _runJammerMusicManager.GetSongsAsync(HandleRunJammerSongCollectionChanged);
                ShuffleCollection(RunJammerSongs);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void InitializeAlbums()
        {
            try
            {
                Albums = new ObservableCollection<AlbumViewModel>(_runJammerMusicManager.GetAlbums().Select(a =>
                    {
                        var vm = new AlbumViewModel(a);
                        vm.PlayAlbumRequested += HandlePlayRequest;
                        return vm;
                    }));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void HandlePlayRequest(object sender, EventArgs e)
        {
            if (MediaPlayer.Queue != null && MediaPlayer.Queue.Count > 1)
            {
                PopulateNowPlayingFromMediaPlayerQueue();
            }
        }

        private async void InitializeArtistsAsync()
        {
            List<Artist> artists = null;
            await Task.Run(() => artists = _runJammerMusicManager.GetArtists().ToList());
            Artists = new ObservableCollection<ArtistViewModel>(artists.Select(a =>
            {
                var vm = new ArtistViewModel(a);
                vm.PlayArtistRequested += HandlePlayRequest;
                return vm;
            }));
        }


        private void HandleRunJammerSongCollectionChanged(RunJammerSong runJammerSong)
        {
            try
            {
                if (runJammerSong == null) return;

                var vm = new RunJammerSongViewModel(runJammerSong, _dataProvider);
                vm.RunRatingChanged += HandleRunJammerSongRunRatingChanged;
                vm.Excluded += HandleExcludeSong;
                RunJammerSongs.Add(vm);
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async void HandleExcludeSong(object sender, EventArgs e)
        {
            try
            {
                var song = sender as RunJammerSongViewModel;
                if (song == null) return;

                await Task.Run(() => Thread.Sleep(TimeSpan.FromMilliseconds(200)));
                if (NowPlayingCollection.Contains(song))
                {
                    NowPlayingCollection.Remove(song);
                }
                if (CurrentRunJammerSongViewModel == song)
                {
                    CurrentPlayingIndex--;
                    ExecuteNextSongCommand();
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    _dataProvider.CreateUserSongRating(new UserSongRating { AlbumName = song.AlbumName, ArtistName = song.ArtistName, SongName = song.Name, Rating = 0, AlwaysSkip = song.ExcludeFromRunSessions, UserID = UserId, TimeCreated = DateTime.UtcNow });
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void InitializeCurrentMedia()
        {
            if (MediaPlayer.Queue != null)
            {
                if (MediaPlayer.Queue.ActiveSong != null)
                {
                    var activeSong = MediaPlayer.Queue.ActiveSong;
                    var activeRunJammerSong = _runJammerMusicManager.RunJammerSongs.FirstOrDefault(rjs => rjs != null && rjs.GetSong() == activeSong);
                    if (activeRunJammerSong != null)
                    {
                        if (RunJammerSongs != null)
                        {
                            CurrentRunJammerSongViewModel = new RunJammerSongViewModel(activeRunJammerSong, _dataProvider);
                            return;
                        }
                        CurrentRunJammerSongViewModel = new RunJammerSongViewModel(activeRunJammerSong, _dataProvider);
                    }
                    else
                    {

                        var runJammerSong = _runJammerMusicManager.GetRunJammerSongFromMediaLibrarySong(activeSong, true);
                        var vm = new RunJammerSongViewModel(runJammerSong, _dataProvider);
                        CurrentRunJammerSongViewModel = vm;
                        NowPlayingCollection.Add(vm);
                    }
                }
                if (MediaPlayer.Queue.Count > 1)
                {
                    try
                    {
                        PopulateNowPlayingFromMediaPlayerQueue();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }

                try
                {
                    //Make a RunJammerPlaylist out of the queue that was there when app started
                    var currentQueuePlaylist = new RunJammerPlaylist();
                    int existingMediaQueueCurrentIndex = MediaPlayer.Queue.ActiveSongIndex;
                    for (int i = 0; i < MediaPlayer.Queue.Count; i++)
                    {
                        var runJammerSong = new RunJammerSong(MediaPlayer.Queue[i]);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        private void PopulateNowPlayingFromMediaPlayerQueue()
        {
            try
            {
                NowPlayingCollection.Clear();
                var songQueue = MediaPlayer.Queue;
                for (int i = 0; i < songQueue.Count; i++)
                {
                    var song = songQueue[i];
                    var rjs = _runJammerMusicManager.GetRunJammerSongFromMediaLibrarySong(song);
                    NowPlayingCollection.Add(new RunJammerSongViewModel(rjs, _dataProvider));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void InitializePlaylists()
        {
            try
            {
                RunJammerPlaylists = new ObservableCollection<RunJammerPlaylistViewModel>(_runJammerMusicManager.GetPlaylists().Select(pl => new RunJammerPlaylistViewModel(pl)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }

        }

        private void HandleRunJammerPlaylistCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var item in e.NewItems)
            {
                var rjp = item as RunJammerPlaylist;
                if (rjp == null) continue;
                var rjpvm = new RunJammerPlaylistViewModel(rjp);
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    RunJammerPlaylists.Add(rjpvm);
                });
            }
        }


        private void InitializeCommands()
        {
            NextSongCommand = new DelegateCommand(ExecuteNextSongCommand);
            RestartSongCommand = new DelegateCommand(ExecuteRestartSongCommand);
            PreviousSongCommand = new DelegateCommand(ExecutePreviousSongCommand);
            PlayCommand = new DelegateCommand(ExecutePlayCommand, CanExecutePlayCommand);
            PauseCommand = new DelegateCommand(ExecutePauseCommand, CanExecutePauseCommand);
            ShuffleNowPlayingCollectionCommand = new DelegateCommand(ExecuteShuffleNowPlayingCollectionCommand);
        }

        private void ExecutePreviousSongCommand()
        {
            try
            {
                var previousPlayingIndex = CurrentPlayingIndex - 1;
                if (!(previousPlayingIndex > 0))
                {
                    return;
                }

                var previousSong = NowPlayingCollection[previousPlayingIndex];
                if (previousSong != null)
                {
                    CurrentRunJammerSongViewModel = previousSong;
                    CurrentPlayingIndex = previousPlayingIndex;
                    MediaPlayer.Play(CurrentRunJammerSongViewModel.Song);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private async void ExecuteShuffleNowPlayingCollectionCommand()
        {
            MediaPlayer.IsShuffled = !MediaPlayer.IsShuffled;
            Shuffle = MediaPlayer.IsShuffled;
            //If there's more than one item in the queue, then the media player is managing which song is played
            if (MediaPlayer.Queue.Count > 1)
            {
                return;
            }

            Random rng = new Random();
            int n = NowPlayingCollection.Count();

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                var value = NowPlayingCollection[k];
                NowPlayingCollection[k] = NowPlayingCollection[n];
                NowPlayingCollection[n] = value;
            }

            if (CurrentRunJammerSongViewModel != null)
            {
                NowPlayingCollection.Insert(0, CurrentRunJammerSongViewModel);
            }
            CurrentPlayingIndex = 0;
        }

        private bool CanExecutePlayCommand()
        {
            return CurrentMediaState != MediaState.Playing;
        }

        private bool CanExecutePauseCommand()
        {
            return CurrentMediaState == MediaState.Playing;
        }

        private void ExecuteNextSongCommand()
        {
            IsLoadingNextSong = true;
            if (MediaPlayer.Queue.Count == 1 && NowPlayingCollection.Any())
            {
                if (CurrentPlayingIndex < NowPlayingCollection.Count - 1)
                {
                    CurrentPlayingIndex++;
                }
                else
                {
                    if (_currentRunSession.IsSessionActive)
                    {
                        int currentRunSpeedRating = int.MinValue;
                        var currentSpeedRatio = _currentRunSession.CurrentSpeed / _currentRunSession.TopSpeed;
                        if (currentSpeedRatio > 0 && currentSpeedRatio <= 0.2)
                        {
                            currentRunSpeedRating = 1;
                        }
                        else if (currentSpeedRatio > 0.2 && currentSpeedRatio <= 0.4)
                        {
                            currentRunSpeedRating = 2;
                        }
                        else if (currentSpeedRatio > 0.4 && currentSpeedRatio <= 0.6)
                        {
                            currentRunSpeedRating = 3;
                        }
                        else if (currentSpeedRatio > 0.6 && currentSpeedRatio <= 0.8)
                        {
                            currentRunSpeedRating = 4;
                        }
                        else
                        {
                            currentRunSpeedRating = 5;
                        }

                        var currentSpeedSongs =
                            RunJammerSongs.Where(rjs => rjs.RunRating == currentRunSpeedRating).ToList();

                        NowPlayingCollection = new ObservableCollection<RunJammerSongViewModel>(currentSpeedSongs);
                    }
                    else
                    {
                        NowPlayingCollection = RunJammerSongs;
                    }
                    ExecuteShuffleNowPlayingCollectionCommand();
                    CurrentPlayingIndex = 1;
                }
                CurrentRunJammerSongViewModel = NowPlayingCollection[CurrentPlayingIndex];
                MediaPlayer.Play(CurrentRunJammerSongViewModel.Song);
            }
            else
            {
                MediaPlayer.MoveNext();
            }
            IsLoadingNextSong = false;
            NextSongLoaded = true;
        }

        private void ExecutePauseCommand()
        {
            MediaPlayer.Pause();
        }

        private void ExecutePlayCommand()
        {
            try
            {
                if (MediaPlayer.Queue == null || MediaPlayer.Queue.Count == 0)
                {

                    if (_currentRunSession.IsSessionActive)
                    {
                        int currentRunSpeedRating = int.MinValue;
                        var currentSpeedRatio = _currentRunSession.CurrentSpeed / _currentRunSession.TopSpeed;
                        if (currentSpeedRatio > 0 && currentSpeedRatio <= 0.2)
                        {
                            currentRunSpeedRating = 1;
                        }
                        else if (currentSpeedRatio > 0.2 && currentSpeedRatio <= 0.4)
                        {
                            currentRunSpeedRating = 2;
                        }
                        else if (currentSpeedRatio > 0.4 && currentSpeedRatio <= 0.6)
                        {
                            currentRunSpeedRating = 3;
                        }
                        else if (currentSpeedRatio > 0.6 && currentSpeedRatio <= 0.8)
                        {
                            currentRunSpeedRating = 4;
                        }
                        else
                        {
                            currentRunSpeedRating = 5;
                        }

                        var currentSpeedSongs = RunJammerSongs.Where(rjs => rjs.RunRating == currentRunSpeedRating).ToList();

                        NowPlayingCollection = new ObservableCollection<RunJammerSongViewModel>(currentSpeedSongs);
                    }

                    NowPlayingCollection = RunJammerSongs;
                    if (Shuffle)
                    {
                        ExecuteShuffleNowPlayingCollectionCommand();
                    }
                    CurrentRunJammerSongViewModel = NowPlayingCollection[CurrentPlayingIndex];
                    MediaPlayer.Play(NowPlayingCollection[CurrentPlayingIndex].Song);
                }
                else
                {
                    if (CurrentMediaState == MediaState.Paused)
                    {
                        MediaPlayer.Resume();
                        return;
                    }

                    MediaPlayer.MoveNext();
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        private void ExecuteRestartSongCommand()
        {
            try
            {
                if (MediaPlayer.Queue.Count == 1 && CurrentPlaylistSongs != null)
                {
                    if (_currentPlaylistPlayIndex > 0)
                    {
                        _currentPlaylistPlayIndex--;
                    }
                    else
                    {
                        _currentPlaylistPlayIndex = CurrentPlaylistSongs.Count - 1;
                    }
                    MediaPlayer.Play(CurrentPlaylistSongs[_currentPlaylistPlayIndex].GetSong());
                }
                else
                {
                    MediaPlayer.MovePrevious();
                }
            }
            catch (Exception ex)
            {

                Logger.Log(ex);
            }
        }


        private void OrderAlbumsByTotalPlays()
        {
            //var sortedAlbums = RunJammerAlbums.OrderByDescending<RunJammerAlbum, int>(a => a.Album.Songs.Sum(s => s.PlayCount)).ToList();

            //Sort the RunJammerAlbums by total play count of the underlying songs.
            //var sortedAlbums =
            //    RunJammerAlbums.Select(rja => rja.GetRunJammerAlbum().GetAlbum()).OrderByDescending(a => a.Songs.Sum(s => s.PlayCount)).ToList();
            //for (int i = 0; i < sortedAlbums.Count; i++)
            //{
            //    var targetSortedItem = sortedAlbums[i];
            //    var targetVm =
            //        RunJammerAlbums.FirstOrDefault(rja => rja.GetRunJammerAlbum().GetAlbum() == targetSortedItem);
            //    var targetSortedItemUnsortedIndex = RunJammerAlbums.IndexOf(targetVm);

            //    if (targetSortedItemUnsortedIndex > -1 && i != targetSortedItemUnsortedIndex)
            //    {
            //        var targetUnsortedItem = RunJammerAlbums[i];

            //        RunJammerAlbums.RemoveAt(targetSortedItemUnsortedIndex);
            //        RunJammerAlbums.RemoveAt(i);

            //        RunJammerAlbums.Insert(i, targetVm);
            //        RunJammerAlbums.Insert(targetSortedItemUnsortedIndex, targetUnsortedItem);
            //    }
            //}
        }

        private void InitializeRunJammerPlaylists()
        {
            try
            {
                var playlists = new ObservableCollection<RunJammerPlaylistViewModel>(
                            _runJammerMusicManager.RunJammerPlaylists.Select(rjpl => new RunJammerPlaylistViewModel(rjpl)));
            }
            catch (Exception ex)
            {
                Logger.Log(ex);
            }
        }

        #endregion

    }
}
