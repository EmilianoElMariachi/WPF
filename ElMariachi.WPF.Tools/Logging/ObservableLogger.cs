using System;
using ElMariachi.WPF.Tools.Logging.EventsDefinition;
using ElMariachi.WPF.Tools.Logging.LoggedItems;

namespace ElMariachi.WPF.Tools.Logging
{
    public class ObservableLogger : IObservableLogger
    {

        #region Events

        public event LogEventHandler LogEvent;

        protected virtual void NotifyLogEvent(LogEventHandlerArgs args)
        {
            var handler = LogEvent;
            if (handler != null) handler(this, args);
        }

        #endregion

        #region Methods

        public void Log(ILoggedItem loggedItem)
        {
            NotifyLogEvent(new LogEventHandlerArgs(loggedItem));
        }

        public void LogWarning(string warningMessage)
        {
            this.Log(new LoggedWarning(warningMessage));
        }

        public void LogInfo(string infoMessage)
        {
            this.Log(new LoggedInfo(infoMessage));
        }

        public void LogError(string errorMessage)
        {
            this.Log(new LoggedError(errorMessage));
        }

        public void LogException(Exception exception)
        {
            this.Log(new LoggedException(exception));
        }

        #endregion

    }
}