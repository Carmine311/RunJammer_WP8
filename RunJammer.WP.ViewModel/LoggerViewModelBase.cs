using Common.Model.Logging;

namespace RunJammer.WP.ViewModel
{
    public abstract class LoggerViewModelBase : ViewModelBase
    {
        public ILogger Logger { get; set; }
        public LoggerViewModelBase(ILogger logger)
        {
            Logger = logger;
        }

        public LoggerViewModelBase()
        {
            
        }
    }
}
