using System.Windows;
using System.Windows.Media.Imaging;
using RunJammer.WP.Messaging;
using RunJammer.WP.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RunJammer.WP.ViewModel
{
    public class RunJammerPlaylistViewModel : RunJammerMusicItemViewModel
    {
        private RunJammerPlaylist _playlist;


        private ObservableCollection<RunJammerSongViewModel> _runJammerSongs;
        public ObservableCollection<RunJammerSongViewModel> RunJammerSongs
        {
            get { return _runJammerSongs; }
            set
            {
                if (value != _runJammerSongs)
                {
                    _runJammerSongs = value;
                    OnPropertyChanged("RunJammerSongs");
                }
            }
        }

        private int _runRating;
        public int RunRating
        {
            get { return _runRating; }
            set
            {
                if (value != _runRating)
                {
                    OnRunRatingChanged(new RunJammerMusicItemRunRatingChangedEventArgs(_runRating, value) { RunJammerMusicItem = _runJammerMusicItem });
                    _runRating = value;
                    _playlist.RunRating = _runRating;
                    OnPropertyChanged("RunRating");
                   // _dataProvider.SubmitChanges();
                }
            }
        }

        public event EventHandler<PlayPlaylistEventArgs> PlayPlaylist;

        public RunJammerPlaylistViewModel()
        {
            RunJammerSongs = new ObservableCollection<RunJammerSongViewModel>();
        }

        public RunJammerPlaylistViewModel(RunJammerPlaylist playlist, IEnumerable<RunJammerSong> playlistSongs)
            : this()
        {
            RunJammerSongs = new ObservableCollection<RunJammerSongViewModel>();
            //_playlist = playlist;
            //foreach (var runJammerSong in playlistSongs)
            //{
            //    RunJammerSongs.Add(runJammerSong);
            //}
        }

        public RunJammerPlaylistViewModel(RunJammerPlaylist playlist)
            : this()
        {
            _playlist = playlist;
            RunRating = playlist.RunRating;
            RunJammerSongs = new ObservableCollection<RunJammerSongViewModel>( playlist.RunJammerSongs.Select(rjs => new RunJammerSongViewModel(rjs)));
            //_playlist.RunJammerSongs.CollectionChanged += HandleRunJammerSongsCollectionChanged;
            Name = _playlist.Name;
        }

        //private void HandleRunJammerSongsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    Deployment.Current.Dispatcher.BeginInvoke(() =>
        //    {
        //        if (e.OldItems != null)
        //        {
        //            foreach (var obj in e.OldItems)
        //            {
        //                var rjs = obj as RunJammerSong;
        //                if (rjs != null)
        //                {
        //                    RunJammerSongs.Remove(RunJammerSongs.FirstOrDefault(vm => vm.GetRunJammerSong() == rjs));
        //                }
        //            }
        //        }

        //        if (e.NewItems != null)
        //        {
        //            foreach (var newItem in e.NewItems)
        //            {
        //                var rjs = newItem as RunJammerSong;
        //                if (rjs != null)
        //                {
        //                    RunJammerSongs.Add(new RunJammerSongViewModel(rjs, new RunJammerAlbumViewModel(rjs.Album)));
        //                }
        //            }
        //        }
        //    });
        //}

        public RunJammerPlaylist GetPlaylist()
        {
            return _playlist;
        }

        protected override BitmapImage GetDisplayImage()
        {
            return null;
        }

        protected override void Play()
        {
            throw new NotImplementedException();
        }

        protected override bool CanPlay()
        {
            throw new NotImplementedException();
        }
    }
}
