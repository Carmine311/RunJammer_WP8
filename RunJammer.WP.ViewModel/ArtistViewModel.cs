using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace RunJammer.WP.ViewModel
{
    public class ArtistViewModel : ViewModelBase
    {
        private Artist _artist;
        public Artist Artist
        {
            get { return _artist; }
            set
            {
                if (value != _artist)
                {
                    _artist = value;
                    OnPropertyChanged("Artist");
                }
            }
        }

        private ObservableCollection<BitmapImage> _albumCovers;
        public ObservableCollection<BitmapImage> AlbumCovers
        {
            get { return _albumCovers; }
            set
            {
                if (value != _albumCovers)
                {
                    _albumCovers = value;
                    OnPropertyChanged("AlbumCovers");
                }
            }
        }

        public event EventHandler PlayArtistRequested;

        protected virtual void OnPlayArtistRequested()
        {
            EventHandler handler = PlayArtistRequested;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public DelegateCommand PlayCommand { get; set; }

        public ArtistViewModel()
        {
            AlbumCovers = new ObservableCollection<BitmapImage>();
            PlayCommand = new DelegateCommand(() => MediaPlayer.Play(Artist.Songs));
        }

        public ArtistViewModel(Artist artist)
            : this()
        {
            Artist = artist;
            if (artist.Albums != null && artist.Albums.Any())
            {
                var sortedAlbums = artist.Albums.OrderByDescending(al => al.Songs.Sum(s => s.PlayCount));
                foreach (var album in sortedAlbums)
                {
                    if (album.HasArt)
                    {
                        var albumCover = new BitmapImage();
                        albumCover.SetSource(album.GetThumbnail());
                        AlbumCovers.Add(albumCover);
                    }
                }
            }
        }


    }
}
