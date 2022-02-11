using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using RunJammer.WP.Messaging;
using RunJammer.WP.Model;

namespace RunJammer.WP.ViewModel
{
    public abstract class RunJammerMusicItemViewModel : ViewModelBase
    {

        public DelegateCommand PlayCommand { get; set; }
        protected abstract BitmapImage GetDisplayImage();

        private string _displayText;
        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                if (value != _displayText)
                {
                    _displayText = value;
                    OnPropertyChanged("DisplayText");
                }
            }
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        private BitmapImage _displayImage;
        public BitmapImage DisplayImage
        {
            get { return _displayImage; }
            set
            {
                if (_displayImage != value)
                {
                    _displayImage = value;
                    OnPropertyChanged("DisplayImage");
                }
            }
        }


        public event EventHandler<RunJammerMusicItemRunRatingChangedEventArgs> RunRatingChanged;

        protected virtual void OnRunRatingChanged(RunJammerMusicItemRunRatingChangedEventArgs e)
        {
            EventHandler<RunJammerMusicItemRunRatingChangedEventArgs> handler = RunRatingChanged;
            if (handler != null) handler(this, e);
        }

        protected RunJammerMusicItem _runJammerMusicItem;

        protected RunJammerMusicItemViewModel()
        {
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            PlayCommand = new DelegateCommand(Play, CanPlay);
        }

        protected RunJammerMusicItemViewModel(RunJammerMusicItem runJammerMusicItem)
            : this()
        {
            _runJammerMusicItem = runJammerMusicItem;
            Name = runJammerMusicItem.Name;
        }

        protected abstract void Play();
        protected abstract bool CanPlay();
    }
}
