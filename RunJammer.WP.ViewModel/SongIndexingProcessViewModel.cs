using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunJammer.ModelManagement;

namespace RunJammer.WP.ViewModel
{
    public class SongIndexingProcessViewModel : ViewModelBase
    {
        private int _totalSongsToIndex;
        public int TotalSongsToIndex
        {
            get { return _totalSongsToIndex; }
            set
            {
                if (value != _totalSongsToIndex)
                {
                    _totalSongsToIndex = value;
                    OnPropertyChanged("TotalSongsToIndex");
                }
            }
        }

        private int _songsIndexed;
        public int SongsIndexed
        {
            get { return _songsIndexed; }
            set
            {
                if (value != _songsIndexed)
                {
                    _songsIndexed = value;
                    OnPropertyChanged("SongsIndexed");
                }
            }
        }


        private bool _isIndexingSongs;
        public bool IsIndexingSongs
        {
            get { return _isIndexingSongs; }
            set
            {
                if (value != _isIndexingSongs)
                {
                    _isIndexingSongs = value;
                    OnPropertyChanged("IsIndexingSongs");
                }
            }
        }



        public SongIndexingProcessViewModel()
        {
            
        }

        public SongIndexingProcessViewModel(RunJammerMusicManager musicManager)
        {
            _musicManager = musicManager;
            TotalSongsToIndex = _musicManager.GetSongCollectionCount();
             IndexCollection();
        }

        private async void IndexCollection()
        {
            IsIndexingSongs = true;
            // _musicManager.IndexMediaLibrary();
            IsIndexingSongs = false;
        }

        private RunJammerMusicManager _musicManager;

        private void UpdateProgress(int songsIndexed)
        {
            SongsIndexed = songsIndexed;
        }
    }
}
