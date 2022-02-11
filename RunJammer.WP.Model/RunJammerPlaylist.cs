using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Common.Model.Implementation;
using Common.Model.Interface;
using Microsoft.Xna.Framework.Media;

namespace RunJammer.WP.Model
{
    [Table]
    public class RunJammerPlaylist : RunJammerMusicItem
    {
        [Column]
        public override int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public override int LocalID { get; set; }

        [Column]
        public override string Name { get; set; }

        [Column]
        public override int RunRating { get; set; }

        /// <summary>
        /// Playlist -Song mappings
        /// </summary>
        private EntitySet<RunJammerPlaylistRunJammerSong> _playlistSongs;
        [Association(OtherKey = "PlaylistID", Storage = "_playlistSongs")]
        public EntitySet<RunJammerPlaylistRunJammerSong> PlaylistSongs
        {
            get { return _playlistSongs; }
            set { _playlistSongs = value; }
        }

        [IgnoreDataMember]
        public ObservableCollection<RunJammerSong> RunJammerSongs { get; set; } 

        public RunJammerPlaylist()
        {
            PlaylistSongs = new EntitySet<RunJammerPlaylistRunJammerSong>();
            RunJammerSongs = new ObservableCollection<RunJammerSong>();
        }

        public RunJammerPlaylist(Playlist playlist)
        {
            Name = playlist.Name;

        }
    }
}
