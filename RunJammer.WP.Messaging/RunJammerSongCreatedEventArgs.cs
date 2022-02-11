
using System;
using System.Collections.Generic;
using RunJammer.WP.Model;

namespace RunJammer.WP.Messaging
{
	public class RunJammerSongCreatedEventArgs : EventArgs
	{
		public IEnumerable<RunJammerSong> RunJammerSongs { get; set; }

		public RunJammerSongCreatedEventArgs(IEnumerable<RunJammerSong> runJammerSongs)
		{
			RunJammerSongs = runJammerSongs;
		}
	}
}
