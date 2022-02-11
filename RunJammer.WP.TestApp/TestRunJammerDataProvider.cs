using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DataAccess.Implementation;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RunJammer.WP.Model;

namespace RunJammer.WP.TestApp
{
    [TestClass]
    public class TestRunJammerDataProvider : TestDataProvider
    {

        //public TestRunJammerDataProvider()
        //{
        //    var runJammerSongs = new List<RunJammerSong>();
        //    runJammerSongs.Add(new RunJammerSong { LocalID = 1 });
        //    runJammerSongs.Add(new RunJammerSong { LocalID = 2 });
        //    runJammerSongs.Add(new RunJammerSong { LocalID = 3 });
        //    runJammerSongs.Add(new RunJammerSong { LocalID = 4 });

        //    var runJammerRunRatingPlaylists = new List<RunJammerRunRatingPlaylist>();
        //    runJammerRunRatingPlaylists.Add(new RunJammerRunRatingPlaylist { RunRating = 1 });
        //    runJammerRunRatingPlaylists.Add(new RunJammerRunRatingPlaylist { RunRating = 2 });
        //    runJammerRunRatingPlaylists.Add(new RunJammerRunRatingPlaylist { RunRating = 3 });
        //    runJammerRunRatingPlaylists.Add(new RunJammerRunRatingPlaylist { RunRating = 4 });
        //    runJammerRunRatingPlaylists.Add(new RunJammerRunRatingPlaylist { RunRating = 5 });

        //    var runJammerPlaylistSongs = new List<RunJammerPlaylistRunJammerSong>();



        //    Lists.Add(runJammerSongs);
        //    Lists.Add(runJammerRunRatingPlaylists);
        //    Lists.Add(runJammerPlaylistSongs);
        //}

        [TestMethod]
        public void GetRunJammerSongs()
        {
            var songs = GetData<RunJammerSong>();
            Assert.IsInstanceOfType(songs, typeof(IEnumerable<RunJammerSong>));
        }

        [TestMethod]
        public void GetRunJammerPlaylists()
        {
            var playlists = GetData<RunJammerPlaylist>();
            Assert.IsInstanceOfType(playlists, typeof(IEnumerable<RunJammerPlaylist>));
        }

        public override List<IEnumerable<object>> Lists { get; set; }
    }
}
