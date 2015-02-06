namespace ElMariachi.WPF.Tools.Logging
{
    public class ObservableLogger : IObservableLogger
    {
        public event LogEventHandler LogEvent;

        protected virtual void NotifyLogEvent(LogEventHandlerArgs args)
        {
            var handler = LogEvent;
            if (handler != null) handler(this, args);
        }

        public void Log(ILoggedItem loggedItem)
        {
            NotifyLogEvent(new LogEventHandlerArgs(loggedItem));
        }
    }
}