using System;
using System.Collections.Generic;
using RunJammer.WP.Model;

namespace RunJammer.WP.Messaging
{
	public class PlayPlaylistEventArgs : EventArgs
	{
		public RunJammerPlaylist Playlist { get; set; }
		public List<RunJammerSong> Songs { get; set; }
	}
}
