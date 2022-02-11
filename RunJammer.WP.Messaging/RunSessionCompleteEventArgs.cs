
using System;
using RunJammer.WP.Model;
using RunJammer.WP.Model.Implementation;

namespace RunJammer.WP.Messaging
{
	public class RunSessionCompleteEventArgs : EventArgs
	{
		public RunSession RunSession { get; set; }
	}
}
