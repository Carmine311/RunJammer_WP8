using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using System.Windows.Media.Imaging;
using Common.Model.Implementation;
using Common.Model.Interface;
using Microsoft.Xna.Framework.Media;

namespace RunJammer.WP.Model
{
    [Table]
    [DataContract]
    public class RunJammerSong : RunJammerMusicItem
    {
        [Column]
        [DataMember]
        public override int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        [IgnoreDataMember]
        public override int LocalID { get; set; }

        [Column]
        [DataMember]
        public override string Name { get; set; }

        [Column]
        [IgnoreDataMember]
        public override int RunRating { get; set; }

        [Column]
        [IgnoreDataMember]
        public int ArtistID { get; set; }

        [Column]
        [IgnoreDataMember]
        public int AlbumID { get; set; }

        [Column]
        [DataMember]
        public string ImagePath { get; set; }

        [Column]
        [IgnoreDataMember]
        public bool ExcludeFromRunSessions { get; set; }

        [Column]
        [DataMember]
        public string ArtistName { get; set; }

        [Column]
        [DataMember]
        public string AlbumName { get; set; }

        [IgnoreDataMember]
        public BitmapImage AlbumArt { get; set; }

        /// <summary>
        /// Song-Playlist Mappings
        /// </summary>
        private EntitySet<RunJammerPlaylistRunJammerSong> _songPlaylists;
        [Association(Storage = "_songPlaylists", OtherKey = "SongID")]
        public EntitySet<RunJammerPlaylistRunJammerSong> SongPlaylists
        {
            get { return _songPlaylists; }
            set
            {
                _songPlaylists = value;
            }
        }

        public Song GetSong()
        {
            return _song;
        }

        public void SetSong(Song song)
        {
            _song = song;
        }

        public void InitializeAlbumArt()
        {
            if (_song.Album != null && _song.Album.HasArt && AlbumArt == null)
            {
                AlbumArt = new BitmapImage();
                AlbumArt.SetSource(_song.Album.GetThumbnail());
            }
        }

        public RunJammerSong()
        {
            SongPlaylists = new EntitySet<RunJammerPlaylistRunJammerSong>();
        }

        public RunJammerSong(Song song)
            : this()
        {
            _song = song;
            Name = song.Name;
            AlbumName = song.Album.Name;
            ArtistName = song.Artist.Name;
            
        }


        //public override bool Equals(object obj)
        //{
        //    var rjs = obj as RunJammerSong;
        //    if (rjs != null)
        //    {
        //        return string.Equals(Name, rjs.Name, StringComparison.CurrentCultureIgnoreCase) &&
        //             string.Equals(ArtistName, rjs.ArtistName, StringComparison.CurrentCultureIgnoreCase) &&
        //             string.Equals(AlbumName, rjs.AlbumName, StringComparison.CurrentCultureIgnoreCase) &&
        //             _song != null &&
        //             rjs.GetSong() != null &&
        //             _song == rjs.GetSong();

        //    }

        //    var song = obj as Song;
        //    if (song != null)
        //    {
        //        return GetSong().Equals(song);
        //    }
        //    return false;
        //}

        private Song _song;
    }
}
