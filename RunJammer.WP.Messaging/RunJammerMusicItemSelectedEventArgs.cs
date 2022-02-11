using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RunJammer.WP.Model;

namespace RunJammer.WP.Messaging
{
    public class RunJammerMusicItemSelectedEventArgs : EventArgs
    {
        public RunJammerMusicItem RunJammerMusicItem { get; set; }

        public RunJammerMusicItemSelectedEventArgs()
        {
            
        }

        public RunJammerMusicItemSelectedEventArgs(RunJammerMusicItem runJammerMusicItem)
        {
            RunJammerMusicItem = runJammerMusicItem;
        }
    }
}
