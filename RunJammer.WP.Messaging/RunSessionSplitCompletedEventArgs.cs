
using System;
using RunJammer.WP.Model;

namespace RunJammer.WP.Messaging
{
	public class RunSessionSplitCompletedEventArgs : EventArgs
	{
		public RunSessionSplit Split { get; set; }
	}
}
