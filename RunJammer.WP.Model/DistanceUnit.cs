using System;
using System.Runtime.Serialization;

namespace RunJammer.WP.Model
{
    [DataContract]
	public enum DistanceUnit
	{
        [EnumMember(Value = "Miles")]
		Mile = 0,
        [EnumMember(Value = "Kilometres")]
		Kilometre = 1
	}
}
