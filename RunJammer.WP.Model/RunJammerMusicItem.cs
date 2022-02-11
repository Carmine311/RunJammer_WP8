
using Common.Model.Implementation;

namespace RunJammer.WP.Model
{
    public abstract class RunJammerMusicItem : LocalEntityBase
    {
        public abstract string Name { get; set; }
        public abstract int RunRating { get; set; }

    }
}
