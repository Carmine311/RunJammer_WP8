using Microsoft.Xna.Framework.Media;

namespace RunJammer.WP.ViewModel
{
	public abstract class SongCollectionViewModelBase : ViewModelBase
	{
		public DelegateCommand<SongCollection> PlaySongCollectionCommand { get; protected set; }
		protected abstract void Play();
		protected MediaLibrary _mediaLibrary = new MediaLibrary();

		public SongCollectionViewModelBase()
		{
			PlaySongCollectionCommand = new DelegateCommand<SongCollection>(c => Play());
		}
	}
}
