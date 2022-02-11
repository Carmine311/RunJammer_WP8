using System;
using RunJammer.WP.Model;

namespace RunJammer.WP.Messaging
{
    public class RunJammerMusicItemRunRatingChangedEventArgs : EventArgs
    {
        public RunJammerMusicItem RunJammerMusicItem { get; set; }
        public int OldRating { get; set; }
        public int NewRating { get; set; }

        public RunJammerMusicItemRunRatingChangedEventArgs()
        {
            
        }

        public RunJammerMusicItemRunRatingChangedEventArgs(int oldRating, int newRating)
        {
            OldRating = oldRating;
            NewRating = newRating;
        }
    }
}
