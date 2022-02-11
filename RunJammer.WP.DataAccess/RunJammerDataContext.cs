using System.Data.Linq;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.DataAccess
{
    public class RunJammerDataContext : DataContext
    {
        private const string ConnectionString = @"isostore:/RunJammerDatabase.sdf";

        public Table<RunSession> RunSession
        {
            get
            {
                return GetTable<RunSession>();
            }
        }

        public Table<RunJammerUser> Users
        {
            get { return GetTable<RunJammerUser>(); }
        }


        public Table<RunJammerPlaylist> RunJammerPlaylist
        {
            get
            {
                return GetTable<RunJammerPlaylist>();
            }
        }

        public Table<RunJammerSong> RunJammerSong
        {
            get
            {
                return GetTable<RunJammerSong>();
            }
        }

        public Table<RunJammerPlaylistRunJammerSong> RunJammerPlaylistRunJammerSong
        {
            get
            {
                return GetTable<RunJammerPlaylistRunJammerSong>();
            }
        }

        public Table<RunSessionWaypoint> RunSessionWaypoint
        {
            get
            {
                return GetTable<RunSessionWaypoint>();
            }
        }

        public Table<RunSessionSplit> RunSessionSplit
        {
            get
            {
                return GetTable<RunSessionSplit>();
            }
        }

        public RunJammerDataContext()
            : base(ConnectionString)
        {

        }
    }

}
