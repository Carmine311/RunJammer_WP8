using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;

namespace RunJammer.WP.ViewModel
{
    public class AlbumViewModel : ViewModelBase
    {
        private Album _album;
        public Album Album
        {
            get { return _album; }
            set
            {
                if (value != _album)
                {
                    _album = value;
                    OnPropertyChanged("Album");
                }
            }
        }

        private BitmapImage _albumCover;
        public BitmapImage AlbumCover
        {
            get { return _albumCover; }
            set
            {
                if (value != _albumCover)
                {
                    _albumCover = value;
                    OnPropertyChanged("AlbumCover");
                }
            }
        }

        public event EventHandler PlayAlbumRequested;

        protected virtual void OnPlayAlbumRequested()
        {
            EventHandler handler = PlayAlbumRequested;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public DelegateCommand PlayCommand { get; set; }

        public AlbumViewModel()
        {
            PlayCommand = new DelegateCommand(() =>
            {
                MediaPlayer.Play(Album.Songs);
                OnPlayAlbumRequested();
            });

        }

        public AlbumViewModel(Album album)
            : this()
        {
            Album = album;
            if (Album.HasArt)
            {
                AlbumCover = new BitmapImage();
                AlbumCover.SetSource(Album.GetThumbnail());
            }
        }

    }
}
