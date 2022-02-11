using System;

namespace Common.Model.Logging
{
    public interface ILogger
    {
        void Log(Exception ex);
    }
}
