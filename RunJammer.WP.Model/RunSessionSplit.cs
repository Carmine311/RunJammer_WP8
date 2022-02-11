using System;
using System.Data.Linq.Mapping;
using Common.Model.Implementation;
using Common.Model.Interface;

namespace RunJammer.WP.Model
{
    [Table]
    public class RunSessionSplit : LocalEntityBase
	{
		[Column]
		public override int ID { get; set; }

		[Column(IsDbGenerated = true, IsPrimaryKey = true)]
		public override int LocalID { get; set; }

		[Column]
		public int SessionID { get; set; }

		[Column]
		public string DistanceUnit { get; set; }

		[Column]
		public string Duration { get; set; }

		[Column]
		public double Measurement { get; set; }

		[Column]
		public int Instance { get; set; }

		[Column]
		public DateTime StartTime { get; set; }

		[Column]
		public DateTime? EndTime { get; set; }

		public RunSessionSplit()
		{
			StartTime = DateTime.Now;
		}

	}
}
