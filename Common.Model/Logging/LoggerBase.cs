using System;
using System.Collections.Generic;

namespace Common.Model.Logging
{
    public abstract class LoggerBase : ILogger
    {
        public List<Exception> Exceptions { get; set; }

        public virtual void Log(Exception ex)
        {
            Exceptions.Add(ex);
        }

        protected LoggerBase()
        {
            Exceptions = new List<Exception>();
        }
    }
}
