using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Messaging;
using RunJammer.WP.Model;
using System;

namespace RunJammer.WP.ViewModel
{
    public class RunJammerSongViewModel : RunJammerMusicItemViewModel
    {
        private RunJammerSong _runJammerSong;
        private RunJammerApplicationDataProvider _dataProvider;

        public Song Song
        {
            get { return _runJammerSong.GetSong(); }
        }

        public DelegateCommand PlayCommand { get; set; }

        private bool _excludeFromRunSession;
        public bool ExcludeFromRunSessions
        {
            get { return _excludeFromRunSession; }
            set
            {
                if (value != _excludeFromRunSession)
                {
                    _excludeFromRunSession = value;
                        
                    OnPropertyChanged("ExcludeFromRunSessions");
                    _runJammerSong.ExcludeFromRunSessions = _excludeFromRunSession;

                    if (_excludeFromRunSession)
                    {
                        _runJammerSong.RunRating = 0;
                        RunRating = 0;
                    }

                    _dataProvider.Update(_runJammerSong);
                    OnExcluded();
                }
            }
        }

        private BitmapImage _hiResDisplayImage;
        public BitmapImage HiResDisplayImage
        {
            get { return _hiResDisplayImage; }
            set
            {
                if (value != _hiResDisplayImage)
                {
                    _hiResDisplayImage = value;
                    OnPropertyChanged("HiResDisplayImage");
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
                    _runJammerSong.RunRating = _runRating;
                    if (ExcludeFromRunSessions)
                    {
                        ExcludeFromRunSessions = false;
                    }
                    OnPropertyChanged("RunRating");
                }
            }
        }

        private double _averageRunSpeedForSession;
        public double AverageRunSpeedForSession
        {
            get { return _averageRunSpeedForSession; }
            set
            {
                if (value != _averageRunSpeedForSession)
                {
                    _averageRunSpeedForSession = value;
                    OnPropertyChanged("AverageRunSpeedForSession");
                }
            }
        }


        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set
            {
                if (value != _artistName)
                {
                    _artistName = value;
                    OnPropertyChanged("ArtistName");
                }
            }
        }

        private string _albumName;
        public string AlbumName
        {
            get { return _albumName; }
            set
            {
                if (value != _albumName)
                {
                    _albumName = value;
                    OnPropertyChanged("AlbumName");
                }
            }
        }

        public event EventHandler Excluded;

        protected virtual void OnExcluded()
        {
            EventHandler handler = Excluded;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public RunJammerSongViewModel()
        {

        }

        public RunJammerSongViewModel(RunJammerSong runJammerSong)
            : base(runJammerSong)
        {
            if (runJammerSong != null)
            {
                InitializeRunJammerSong(runJammerSong);
            }
        }

        private void InitializeRunJammerSong(RunJammerSong runJammerSong)
        {
            _runJammerSong = runJammerSong;
            Name = _runJammerSong.Name;
            AlbumName = runJammerSong.AlbumName;
            ArtistName = runJammerSong.ArtistName;
            RunRating = _runJammerSong.RunRating;
            DisplayImage = runJammerSong.AlbumArt;
        }

        public RunJammerSongViewModel(RunJammerSong runJammerSong, RunJammerApplicationDataProvider dataProvider)
            : base(runJammerSong)
        {
            _dataProvider = dataProvider;
            InitializeRunJammerSong(runJammerSong);
            PlayCommand = new DelegateCommand(() => MediaPlayer.Play(GetRunJammerSong().GetSong()));

        }


        public RunJammerSong GetRunJammerSong()
        {
            return _runJammerSong;
        }

        public void CalculateAverageSpeed(IEnumerable<RunSessionWaypoint> sessionWaypoints)
        {
            var songWaypoints = sessionWaypoints.Where(wp => wp.CurrentSongID == GetRunJammerSong().LocalID).ToList();
            if (songWaypoints.Any())
            {
                AverageRunSpeedForSession = songWaypoints.Average(w => w.Speed);
            }
        }

        public void GetHiResDisplayImage()
        {
            var song = GetRunJammerSong().GetSong();
            if (song != null && song.Album != null & song.Album.HasArt)
            {
                var bm = new BitmapImage();
                bm.SetSource(song.Album.GetAlbumArt());
                HiResDisplayImage = bm;
            }
        }

        protected override BitmapImage GetDisplayImage()
        {
            //if (RunJammerAlbumViewModel != null)
            //{
            //    return RunJammerAlbumViewModel.DisplayImage;
            //}
            return _runJammerSong.AlbumArt;
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
