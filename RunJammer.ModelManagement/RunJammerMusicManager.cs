using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Common.Model.Logging;
using Microsoft.Xna.Framework.Media;
using RunJammer.WP.DataAccess.Implementation;
using RunJammer.WP.Model;

namespace RunJammer.ModelManagement
{
    public class RunJammerMusicManager
    {
        #region Interface

        public bool HasSongs()
        {
            return _mediaLibrary.Songs.Any();
        }

        public IEnumerable<RunJammerPlaylist> GetPlaylists()
        {

            return _runJammerPlaylistCache;
        }

        public async Task<IEnumerable<RunJammerSong>> GetSongsAsync(
            Action<RunJammerSong> runJammerSongAddedCallback = null, int callbackThreshold = 1)
        {
            int itemsAdded = 0;
            //var songsToSave = new List<RunJammerSong>();
            var ret = new List<RunJammerSong>();
            try
            {
                await Task.Run(() =>
                    {
                        if (_isCachingRunJammerSongs)
                        {
                            _canCacheRunJammerSongs = false;

                            foreach (var runJammerSong in _runJammerSongCache)
                            {
                                ret.Add(runJammerSong);
                                itemsAdded++;
                                if (runJammerSongAddedCallback != null && itemsAdded == callbackThreshold)
                                {
                                    Deployment.Current.Dispatcher.BeginInvoke(() => runJammerSongAddedCallback(runJammerSong));

                                    itemsAdded = 0;

                                }
                            }

                            for (int i = _runJammerSongCachingIndex + 1; i < _songs.Count; i++)
                            {
                                RunJammerSong runJammerSong = null;
                                Deployment.Current.Dispatcher.BeginInvoke(() => runJammerSong = GetRunJammerSongFromMediaLibrarySong(_songs[i]));
                                ret.Add(runJammerSong);
                                itemsAdded++;
                                if (runJammerSongAddedCallback != null && itemsAdded == callbackThreshold)
                                {
                                    Deployment.Current.Dispatcher.BeginInvoke(() => runJammerSongAddedCallback(runJammerSong));

                                    itemsAdded = 0;

                                }
                            }
                        }
                        else
                        {

                            foreach (var runJammerSong in _runJammerSongCache)
                            {
                                ret.Add(runJammerSong);
                                itemsAdded++;
                                if (runJammerSongAddedCallback != null && itemsAdded == callbackThreshold)
                                {
                                    Deployment.Current.Dispatcher.BeginInvoke(() => runJammerSongAddedCallback(runJammerSong));

                                    itemsAdded = 0;

                                }
                            }
                        }
                    });
            }
            catch (Exception ex)
            {

                throw new Exception("Error getting songs " + DateTime.Now, ex);
            }

            return ret;
        }

        public IEnumerable<Album> GetAlbums()
        {
            return _albums;
        }

        public IEnumerable<Artist> GetArtists()
        {
            return _artists;
        }

        public ObservableCollection<RunJammerSong> RunJammerSongs { get; set; }
        public ObservableCollection<RunJammerPlaylist> RunJammerPlaylists { get; set; }


        private Dictionary<Album, BitmapImage> _albumArtCache;

        private List<RunJammerSong> _runJammerSongCache;
        private bool _canCacheRunJammerSongs = true;
        private bool _isCachingRunJammerSongs;
        private bool _completedChachingRunJammerSongs;
        private int _runJammerSongCachingIndex;

        private List<RunJammerPlaylist> _runJammerPlaylistCache;

        public IEnumerable<BitmapImage> GetAlbumArt()
        {
            var sortedAlbums = _albumArtCache.OrderByDescending(a => a.Key.Songs.Sum(s => s.PlayCount)).ToList();

            return sortedAlbums.Select(s => s.Value);
        }

        public void UpdateSongRunRating(RunJammerSong runJammerSong, int newRating)
        {
            if (runJammerSong.RunRating > 0)
            {
                RemoveFromOldRatingPlaylist(runJammerSong, runJammerSong.RunRating);
            }
            AddToNewRatingPlaylist(runJammerSong, newRating);
            runJammerSong.RunRating = newRating;
        }

        public int GetSongCollectionCount()
        {
            return _mediaLibrary.Songs.Count;
        }

        public IEnumerable<RunJammerPlaylist> GenerateDefaultPlaylists()
        {
            yield return new RunJammerPlaylist { Name = "5 Speed Songs", RunRating = 5 };
            yield return new RunJammerPlaylist { Name = "4 Speed Songs", RunRating = 4 };
            yield return new RunJammerPlaylist { Name = "3 Speed Songs", RunRating = 3 };
            yield return new RunJammerPlaylist { Name = "2 Speed Songs", RunRating = 2 };
            yield return new RunJammerPlaylist { Name = "1 Speed Songs", RunRating = 1 };
        }

        #endregion

        #region Construction

        public RunJammerMusicManager()
        {
            _songs = _mediaLibrary.Songs.ToList();
            _albums = _mediaLibrary.Albums.ToList();
            _artists = _mediaLibrary.Artists.ToList();

            RunJammerPlaylists = new ObservableCollection<RunJammerPlaylist>();
            RunJammerSongs = new ObservableCollection<RunJammerSong>();
        }

        public void Initialize()
        {
            _albumArtCache = new Dictionary<Album, BitmapImage>();
            _runJammerSongCache = new List<RunJammerSong>();
            _runJammerPlaylistCache = new List<RunJammerPlaylist>();

            InitializeAlbumArtCache();
            InitializeDefaultPlaylists();

            Debug.WriteLine("Started Indexing Library: " + DateTime.Now);
            IndexMediaLibrary();
            Debug.WriteLine("Finished Indexing Library: " + DateTime.Now);
        }

        private void InitializeAlbumArtCache()
        {
            Debug.WriteLine("Started AlbumArt Cache: " + DateTime.Now);
            foreach (var album in _albums)
            {
                if (album.HasArt)
                {
                    var albumArt = new BitmapImage();
                    albumArt.SetSource(album.GetThumbnail());
                    _albumArtCache.Add(album, albumArt);
                }
            }
            Debug.WriteLine("Finished AlbumArt Cache: " + DateTime.Now);
        }

        public void IndexMediaLibrary()
        {
            _songs = _mediaLibrary.Songs.ToList();
            _albums = OrderMediaLibraryAlbumsByTotalSongPlayCount();
            _artists = OrderMediaLibraryArtistsByTotalSongPlayCount();

            CacheRunJammerSongs();
        }

        private void CacheRunJammerSongs()
        {
            while (_canCacheRunJammerSongs && !_completedChachingRunJammerSongs)
            {
                _isCachingRunJammerSongs = true;
                for (int i = 0; i < _songs.Count(); i++)
                {
                    _runJammerSongCachingIndex = i;
                    var runJammerSong = GetRunJammerSongFromMediaLibrarySong(_songs[i]);
                    if (runJammerSong == null) continue;
                    _runJammerSongCache.Add(runJammerSong);
                }
                _completedChachingRunJammerSongs = true;
                _isCachingRunJammerSongs = false;
            }
        }

        private List<Artist> OrderMediaLibraryArtistsByTotalSongPlayCount()
        {
            List<Artist> orderedArtists = null;
            Debug.WriteLine("Started Ordering Artists: " + DateTime.Now);
            orderedArtists = _mediaLibrary.Artists.OrderByDescending(a => a.Songs.Sum(s => s.PlayCount)).ToList();
            Debug.WriteLine("Finished Ordering Artists: " + DateTime.Now);
            return orderedArtists;
        }

        private List<Album> OrderMediaLibraryAlbumsByTotalSongPlayCount()
        {
            List<Album> orderedAlbums = null;
            Debug.WriteLine("Started Ordering Albums: " + DateTime.Now);
            orderedAlbums = _mediaLibrary.Albums.OrderByDescending(a => a.Songs.OrderByDescending(s => s.PlayCount).Sum(s => s.PlayCount)).ToList();
            Debug.WriteLine("Finished Ordering Albums: " + DateTime.Now);
            return orderedAlbums;
        }

        private List<Song> OrderMediaLibrarySongsByPlayCount()
        {
            return _mediaLibrary.Songs.OrderByDescending(s => s.PlayCount).ToList();
        }

        public RunJammerSong GetRunJammerSongFromMediaLibrarySong(Song song, bool commitToDatabase = false)
        {
            try
            {
                RunJammerSong existingRunJammerSong = null;

                existingRunJammerSong =
                   Enumerable.FirstOrDefault<RunJammerSong>(DataProvider.GetRunJammerSongs(), rjs =>
                               rjs.Name == song.Name && rjs.ArtistName == song.Artist.Name &&
                               rjs.AlbumName == song.Album.Name);

                RunJammerSong runJammerSong;

                if (existingRunJammerSong != null)
                {
                    if (existingRunJammerSong.ExcludeFromRunSessions)
                    {
                        return null;
                    }
                    existingRunJammerSong.SetSong(song);
                    runJammerSong = existingRunJammerSong;
                }
                else
                {
                    runJammerSong = new RunJammerSong(song);
                    DataProvider.CreateRunJammerSong(runJammerSong);
                }

                SetRunJammerSongAlbumArt(runJammerSong);

                return runJammerSong;
            }
            catch (Exception ex)
            {
                _logger.Log(new Exception("Error getting RunJammerSong for " + song.Name, ex));
                return null;
            }
        }

        private void SetRunJammerSongAlbumArt(RunJammerSong runJammerSong)
        {
            var album = runJammerSong.GetSong().Album;
            if (album != null)
            {
                if (_albumArtCache.ContainsKey(album))
                {
                    runJammerSong.AlbumArt = _albumArtCache[album];
                }
            }
        }

        //private async Task<RunJammerPlaylist> CreateRunJammerPlaylistFromMediaLibraryPlaylist(Playlist playlist)
        //{
        //    var runJammerPlaylist = new RunJammerPlaylist(playlist);

        //    var runJammerSongs = await InitializeRunJammerSongsForPlaylist(playlist);

        //    foreach (var runJammerSong in runJammerSongs)
        //    {
        //        if (runJammerSong == null) continue;
        //        var playlistSong = new RunJammerPlaylistRunJammerSong { Playlist = runJammerPlaylist, Song = runJammerSong };
        //        runJammerPlaylist.PlaylistSongs.Add(playlistSong);
        //        runJammerSong.SongPlaylists.Add(playlistSong);
        //    }
        //    return runJammerPlaylist;

        //}


        //private async Task<List<RunJammerSong>> InitializeRunJammerSongsForPlaylist(Playlist playlist)
        //{
        //    List<RunJammerSong> ret = new List<RunJammerSong>();
        //    await Task.Run(async () =>
        //    {
        //        foreach (var song in playlist.Songs)
        //        {
        //            var existingRunJammerSong = await GetExistingRunJammerSongFromMediaLibrarySong(song);
        //            if (existingRunJammerSong == null) ret.Add(CreateNewRunJammerSongFromMediaLibrarySong(song));
        //            ret.Add(existingRunJammerSong);
        //        }
        //    });
        //    return ret;
        //}



        private IEnumerable<RunJammerPlaylist> CreateDefaultPlaylists()
        {
            var defaultPlaylists = GenerateDefaultPlaylists().ToList();
            foreach (var playlist in defaultPlaylists)
            {
                DataProvider.CreateRunJammerPlaylist(playlist);
                yield return playlist;
            }
        }

        private void InitializeDefaultPlaylists()
        {
            var playlists = DataProvider.GetRunJammerPlaylists().ToList();

            if (!playlists.Any())
            {
                _runJammerPlaylistCache = CreateDefaultPlaylists().ToList();
                return;
            }

            foreach (var runJammerPlaylist in playlists)
            {
                _runJammerPlaylistCache.Add(runJammerPlaylist);
                InitializePlaylistSongs(runJammerPlaylist);
            }
        }
        private void InitializePlaylistSongs(RunJammerPlaylist playlist)
        {
            foreach (var playlistSong in playlist.PlaylistSongs)
            {
                InitializeSong(playlistSong);
            }
        }

        private void InitializeSong(RunJammerPlaylistRunJammerSong playlistSong)
        {
            RunJammerSongs.Add(playlistSong.Song);
        }


        public RunJammerMusicManager(RunJammerApplicationDataProvider dataProvider, ILogger logger)
            : this()
        {
            _logger = logger;
            DataProvider = dataProvider;
        }

        #endregion

        #region Implementation

        protected readonly RunJammerApplicationDataProvider DataProvider;
        private MediaLibrary _mediaLibrary = new MediaLibrary();
        private List<Song> _songs;
        private List<Artist> _artists;
        private List<Album> _albums;
        private ILogger _logger;

        private void RemoveFromOldRatingPlaylist(RunJammerSong runJammerSong, int oldRating)
        {
            var oldPlaylist = GetRatingPlaylist(oldRating);
            var playlistSongEntry = GetPlaylistSongEntry(oldPlaylist, runJammerSong);
            if (playlistSongEntry != null)
            {
                oldPlaylist.PlaylistSongs.Remove(playlistSongEntry);
            }
        }

        private void AddToNewRatingPlaylist(RunJammerSong runJammerSong, int newRating)
        {
            var newPlaylist = GetRatingPlaylist(newRating);
            if (newPlaylist != null)
            {
                var songPlaylistMapping = new RunJammerPlaylistRunJammerSong
                    {
                        Playlist = newPlaylist,
                        Song = runJammerSong
                    };
                newPlaylist.PlaylistSongs.Add(songPlaylistMapping);
                newPlaylist.RunJammerSongs.Add(runJammerSong);

                runJammerSong.SongPlaylists.Add(songPlaylistMapping);
                DataProvider.CreatePlaylistSongMapping(songPlaylistMapping);
            }
        }

        protected RunJammerPlaylistRunJammerSong GetPlaylistSongEntry(RunJammerPlaylist playlist, RunJammerSong target)
        {
            return playlist.PlaylistSongs.FirstOrDefault(pls => Equals(pls.Song, target));
        }

        protected RunJammerPlaylist GetRatingPlaylist(int rating)
        {
            return _runJammerPlaylistCache.FirstOrDefault(pl => pl.RunRating == rating);
        }

        //private void OrderLibraryAlbumsByTotalPlays()
        //{
        //    var sortedAlbums = _mediaLibrary.RunJammerAlbums.OrderByDescending(a => a.Songs.Sum(s => s.PlayCount)).ToList();

        //    for (int i = 0; i < sortedAlbums.Count; i++)
        //    {
        //        var targetSortedItem = sortedAlbums[i];
        //        var targetSortedItemUnsortedIndex = RunJammerAlbums.IndexOf(targetSortedItem);

        //        if (targetSortedItemUnsortedIndex > -1 && i != targetSortedItemUnsortedIndex)
        //        {
        //            var targetUnsortedItem = RunJammerAlbums[i];

        //            RunJammerAlbums.RemoveAt(targetSortedItemUnsortedIndex);
        //            RunJammerAlbums.RemoveAt(i);

        //            RunJammerAlbums.Insert(i, targetSortedItem);
        //            RunJammerAlbums.Insert(targetSortedItemUnsortedIndex, targetUnsortedItem);
        //        }
        //    }
        //}

        #endregion
    }
}
