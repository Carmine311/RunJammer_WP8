using System;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Runtime.Serialization;
using RunJammer.WP.Model.Interface;

namespace RunJammer.WP.Model.Implementation
{
    [Table]
    [DataContract]
    public class RunSession : IRunSession<DateTime?>
    {
        [Column]
        [DataMember]
        public int ID { get; set; }

        [Column(IsDbGenerated = true, IsPrimaryKey = true)]
        [IgnoreDataMember]
        public int LocalID { get; set; }

        [Column(CanBeNull = true)]
        [DataMember]
        public DateTime? StartTime { get; set; }

        [Column(CanBeNull = true)]
        [DataMember]
        public DateTime? LastUpdateTime { get; set; }

        [Column(CanBeNull = true)]
        [DataMember]
        public DateTime? EndTime { get; set; }

        [Column]
        [DataMember]
        public double TotalDistance { get; set; }

        [Column(DbType = "bit")]
        [DataMember]
        public bool IsSessionActive { get; set; }

        [Column(DbType = "bit")]
        [DataMember]
        public bool UserEndedSession { get; set; }

        [Column]
        [DataMember]
        public string Pace { get; set; }

        [Column]
        [DataMember]
        public double AverageSpeed { get; set; }

        private double _currentSpeed { get; set; }

        [Column]
        [DataMember]
        public double CurrentSpeed
        {
            get { return _currentSpeed; }
            set
            {
                if (double.IsNaN(value))
                {
                    _currentSpeed = 0d;
                }
                else
                {
                    _currentSpeed = value;
                }
            }
        }

        [Column]
        [DataMember]
        public double TopSpeed { get; set; }

        [Column]
        [IgnoreDataMember]
        public DistanceUnit DistanceUnit { get; set; }

        [DataMember]
        public string UserId { get; set; }


        private EntitySet<RunSessionWaypoint> _waypoints;

        [Association(Storage = "_waypoints", OtherKey = "SessionID", DeleteRule = "CASCADE"), IgnoreDataMember]
        public EntitySet<RunSessionWaypoint> Waypoints { get { return _waypoints; } set { _waypoints = value; } }


        private EntitySet<RunSessionSplit> _splits;
        [Association(Storage = "_splits", OtherKey = "SessionID"), IgnoreDataMember]
        public EntitySet<RunSessionSplit> Splits { get { return _splits; } set { _splits = value; } }

        public RunSession()
        {
            Splits = new EntitySet<RunSessionSplit>();
            StartTime = null;
            EndTime = null;
            Waypoints = new EntitySet<RunSessionWaypoint>();
            Pace = TimeSpan.FromMinutes(0d).ToString();
        }

    }
}
