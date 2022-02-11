using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunJammer.WP.Model
{
    public class UserSongRating
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string SongName { get; set; }
        public string ArtistName { get; set; }
        public string AlbumName { get; set; }
        public int Rating { get; set; }
        public bool AlwaysSkip { get; set; }
        public DateTime TimeCreated { get; set; }
    }
}
