using System.Data.Linq;
using System.Data.Linq.Mapping;
using Common.Model.Implementation;
using Common.Model.Interface;

namespace RunJammer.WP.Model
{
    [Table]
    public class RunJammerPlaylistRunJammerSong : LocalEntityBase
    {
        [Column]
        public override int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public override int LocalID { get; set; }

        private int _playlistID;
        [Column]
        public int PlaylistID { get { return _playlistID; } set { _playlistID = value; } }

        private int _songID;
        [Column]
        public int SongID
        {
            get { return _songID; }
            set { _songID = value; }
        }

        /// <summary>
        /// Playlist Reference
        /// </summary>
        private EntityRef<RunJammerPlaylist> _playlist;
        [Association(Storage = "_playlist", OtherKey = "LocalID")]
        public RunJammerPlaylist Playlist
        {
            get { return _playlist.Entity; }
            set
            {
                _playlist.Entity = value;
                PlaylistID = _playlist.Entity.LocalID;
            }
        }

        /// <summary>
        /// SongReference
        /// </summary>
        private EntityRef<RunJammerSong> _song;
        [Association(Storage = "_song", OtherKey = "LocalID")]
        public RunJammerSong Song
        {
            get { return _song.Entity; }
            set
            {
                _song.Entity = value;
                SongID = _song.Entity.LocalID;
            }
        }

        public RunJammerPlaylistRunJammerSong()
        {

        }

        public RunJammerPlaylistRunJammerSong(RunJammerPlaylist playlist, RunJammerSong song)
        {
            Playlist = playlist;
            Song = song;
            Song.SongPlaylists.Add(this);
            Playlist.PlaylistSongs.Add(this);
        }

    }
}
