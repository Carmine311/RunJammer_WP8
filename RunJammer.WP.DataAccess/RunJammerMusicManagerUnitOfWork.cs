using Microsoft.Xna.Framework.Media;
using RunJammer.WP.Messaging;
using RunJammer.WP.DataAccess;
using RunJammer.WP.DataAccess;
using RunJammer.WP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RunJammer.WP.DataAccess
{
	public class RunJammerMusicManagerUnitOfWork : RunJammerUnitOfWorkBase
	{

		private MediaLibrary _mediaLibrary;

		public IEnumerable<RunJammerSong> GetRunJammerSongs()
		{
			List<RunJammerSong> ret = new List<RunJammerSong>();
			if (_localDataContext.RunJammerSongs.Any())
			{
				var dbSongs = _localDataContext.RunJammerSongs.ToList();
				foreach (var runJammerSong in dbSongs)
				{
					var mediaLibrarySong = _mediaLibrary.Songs.FirstOrDefault(s => s.Name == runJammerSong.Title && s.Artist.Name == runJammerSong.ArtistName && s.Album.Name == runJammerSong.AlbumName);
					if (mediaLibrarySong != null)
					{
						runJammerSong.SetSong(mediaLibrarySong);
						ret.Add(runJammerSong);
					}
				}
			}
			return ret;
		}

		public IEnumerable<RunJammerPlaylist> GetPlaylists()
		{
			List<RunJammerPlaylist> ret = new List<RunJammerPlaylist>();

			var playlists = _localDataContext.RunJammerPlaylists.ToList();

			if (!playlists.Any())
			{
				//GenerateDefaultPlaylists();
				//foreach (var playlist in RunJammerPlaylistViewModels)
				//{
				//	playlist.PlayPlaylist += HandlePlayPlaylist;
				//}
			}
			else
			{
				var playlsitSongs = _localDataContext.RunJammerPlaylistSongs.ToList();
				foreach (var playlist in playlists)
				{
					var dbSongs = _localDataContext.GetTable<RunJammerSong>().ToList();
					var playlistSongsIDs = playlsitSongs.Where(pls => pls.PlaylistID == playlist.LocalID).Select(pls => pls.SongID).ToList();
					if (playlistSongsIDs.Any())
					{
						List<RunJammerSong> playlistSongs = new List<RunJammerSong>();

						foreach (var songID in playlistSongsIDs)
						{
							var song = dbSongs.FirstOrDefault(rjs => rjs.LocalID == songID);
							if (song != null)
							{
								playlistSongs.Add(song);
							}
						}

						//RunJammerPlaylistViewModel playlistViewModel = null;
						if (playlistSongs.Any())
						{
							foreach (var playlistSong in playlistSongs)
							{
								if (playlistSong != null)
								{
									var song = _mediaLibrary.Songs.FirstOrDefault(s => s.Name == playlistSong.Title && s.Album.Name == playlistSong.AlbumName && s.Artist.Name == playlistSong.ArtistName); if (song != null)
									{
										playlistSong.SetSong(song);
									}
								}
							}
							//playlistViewModel = new RunJammerPlaylistViewModel(playlist, playlistSongs);
						}
						else
						{
							//playlistViewModel = new RunJammerPlaylistViewModel(playlist);
						}

						//playlistViewModel.PlayPlaylist += HandlePlayPlaylist;
						//RunJammerPlaylistViewModels.Add(playlistViewModel);
					}

					ret.Add(playlist);
				}
			}

			return ret;
		}

		public event EventHandler<RunJammerSongCreatedEventArgs> RunJammerSongsCreated;

		public RunJammerMusicManagerUnitOfWork(RunJammerDataContext dataContext, RunJammerMobileServiceClient mobileServiceClient)
			: base(dataContext, mobileServiceClient)
		{
			_localDataContext = dataContext;
			_mobileServiceClient = mobileServiceClient;
			_mediaLibrary = new MediaLibrary();
			CheckForNewSongs();
		}

		//private void GenerateDefaultPlaylists()
		//{
		//	List<RunJammerPlaylist> playlists = new List<RunJammerPlaylist>{ 
		//						new RunJammerPlaylist { Name = "5 Speed Songs" },                            
		//						new RunJammerPlaylist{ Name = "4 Speed Songs"},
		//						new RunJammerPlaylist{ Name = "3 Speed Songs"},
		//						new RunJammerPlaylist{ Name = "2 Speed Songs"},
		//						new RunJammerPlaylist{ Name = "1 Speed Songs"},
		//					};
		//	//RunJammerPlaylistViewModels = new System.Collections.ObjectModel.ObservableCollection<RunJammerPlaylistViewModel>(playlists.Select(pl => new RunJammerPlaylistViewModel(pl)));

		//	_localDataContext.RunJammerPlaylists.InsertAllOnSubmit<RunJammerPlaylist>(playlists);
		//	_localDataContext.SubmitChanges();
		//}

		private async void CheckForNewSongs()
		{
			var mediaLibrarySongs = _mediaLibrary.Songs.OrderByDescending(s => s.PlayCount).ToList();
			List<RunJammerSong> dbSongs = null;
			List<RunJammerSong> runJammerSongsToAdd = new List<RunJammerSong>();
			dbSongs = _localDataContext.RunJammerSongs.ToList();
			await Task.Run(() =>
			{
				foreach (var song in mediaLibrarySongs)
				{
					var runJammerSong = dbSongs.FirstOrDefault(rjs => rjs.Title == song.Name && rjs.AlbumName == song.Album.Name && rjs.ArtistName == song.Artist.Name);
					if (runJammerSong == null)
					{
						runJammerSong = new RunJammerSong(song);
						runJammerSongsToAdd.Add(runJammerSong);
					}
				}
			});

			if (RunJammerSongsCreated != null)
			{
				RunJammerSongsCreated(this, new RunJammerSongCreatedEventArgs(runJammerSongsToAdd));
			}
			_localDataContext.RunJammerSongs.InsertAllOnSubmit(runJammerSongsToAdd);
			_localDataContext.SubmitChanges();
			_mobileServiceClient.AddRunJammerSongs(runJammerSongsToAdd);

		}
	}
}
