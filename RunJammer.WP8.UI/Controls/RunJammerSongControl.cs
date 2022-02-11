using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace RunJammer.WP.UI.Controls
{
    public class RunJammerSongControl : Control
    {
        public static readonly DependencyProperty SongNameProperty = DependencyProperty.Register(
            "SongName", typeof(string), typeof(RunJammerSongControl), new PropertyMetadata(default(string)));

        public string SongName
        {
            get { return (string)GetValue(SongNameProperty); }
            set { SetValue(SongNameProperty, value); }
        }

        public static readonly DependencyProperty ArtistNameProperty = DependencyProperty.Register(
            "ArtistName", typeof(string), typeof(RunJammerSongControl), new PropertyMetadata(default(string)));

        public string ArtistName
        {
            get { return (string)GetValue(ArtistNameProperty); }
            set { SetValue(ArtistNameProperty, value); }
        }

        public static readonly DependencyProperty AlbumNameProperty = DependencyProperty.Register(
            "AlbumName", typeof(string), typeof(RunJammerSongControl), new PropertyMetadata(default(string)));

        public string AlbumName
        {
            get { return (string)GetValue(AlbumNameProperty); }
            set { SetValue(AlbumNameProperty, value); }
        }

        public static readonly DependencyProperty RunRatingProperty = DependencyProperty.Register(
            "RunRating", typeof(int), typeof(RunJammerSongControl), new PropertyMetadata(default(int)));

        public int RunRating
        {
            get { return (int)GetValue(RunRatingProperty); }
            set { SetValue(RunRatingProperty, value); }
        }

        public static readonly DependencyProperty ExcludeFromRunSessionsProperty = DependencyProperty.Register(
            "ExcludeFromRunSessions", typeof (bool), typeof (RunJammerSongControl), new PropertyMetadata(default(bool)));

        public bool ExcludeFromRunSessions
        {
            get { return (bool) GetValue(ExcludeFromRunSessionsProperty); }
            set { SetValue(ExcludeFromRunSessionsProperty, value); }
        }

        public static readonly DependencyProperty AlbumCoverProperty = DependencyProperty.Register(
            "AlbumCover", typeof(BitmapImage), typeof(RunJammerSongControl), new PropertyMetadata(default(BitmapImage)));

        public BitmapImage AlbumCover
        {
            get { return (BitmapImage)GetValue(AlbumCoverProperty); }
            set { SetValue(AlbumCoverProperty, value); }
        }

        public RunJammerSongControl()
        {
            DefaultStyleKey = typeof(RunJammerSongControl);
        }
    }
}
