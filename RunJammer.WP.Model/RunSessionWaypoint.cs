using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Common.Model.Implementation;
using Common.Model.Interface;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.Model
{
    [Table]
    public class RunSessionWaypoint : LocalEntityBase
    {
        [Column]
        public override int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        public override int LocalID { get; set; }

        [Column]
        public int SessionID { get; set; }

        [Column(CanBeNull = true)]
        public int? CurrentSongID { get; set; }

        [Column]
        public double Lat { get; set; }

        [Column]
        public double Lon { get; set; }

        [Column]
        public double Altitude { get; set; }

        [Column]
        public string Pace { get; set; }

        [Column]
        public double Speed { get; set; }

        public override bool Equals(object obj)
        {
            var waypoint = (RunSessionWaypoint)obj;
            return Lat == waypoint.Lat && Lon == waypoint.Lon && Altitude == waypoint.Altitude && Pace == waypoint.Pace && Speed == waypoint.Speed;
        }

        private EntityRef<RunJammerSong> _currentSong;

        [Association(Storage = "_currentSong", ThisKey = "CurrentSongID", OtherKey = "LocalID", IsForeignKey = true)]
        public RunJammerSong CurrentSong
        {
            get
            {
                return _currentSong.Entity;
            }
            set
            {
                _currentSong.Entity = value;
            }
        }

        private EntityRef<RunSession> _runSession;

        [Association(Storage = "_runSession", ThisKey = "SessionID", OtherKey = "LocalID", IsForeignKey = true)]
        public RunSession RunSession
        {
            get { return _runSession.Entity; }
            set { _runSession.Entity = value; }
        }

    }
}
