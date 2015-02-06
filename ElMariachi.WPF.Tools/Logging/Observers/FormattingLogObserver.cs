using System;
using System.Diagnostics;
using ElMariachi.WPF.Tools.Logging.Observers.ToFile;

namespace ElMariachi.WPF.Tools.Logging.Observers
{
    public abstract class FormattingLogObserver
    {
        #region Fields & Properties

        private readonly IObservableLog _observableLog;

        public IObservableLog ObservedLog
        {
            get { return _observableLog; }
        }

        private readonly ILoggedItemFormatter _loggedItemFormatter;

        public ILoggedItemFormatter LoggedItemFormatter
        {
            get { return _loggedItemFormatter; }
        }

        #endregion

        #region Constructor

        public FormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
        {
            if (observableLog == null)
            {
                throw new ArgumentNullException("observableLog", "Can't create \"" + this.GetType().Name + "\" with null \"" + typeof(IObservableLog) + "\".");
            }

            if (loggedItemFormatter == null)
            {
                throw new ArgumentNullException("loggedItemFormatter", "Can't create \"" + this.GetType().Name + "\" with null \"" + typeof(ILoggedItemFormatter) + "\".");
            }

            _observableLog = observableLog;
            _loggedItemFormatter = loggedItemFormatter;

            ObservedLog.LogEvent += OnLogEvent;
        }

        ~FormattingLogObserver()
        {
            ObservedLog.LogEvent -= OnLogEvent;
        }

        #endregion

        #region Methods

        protected abstract void OnFormattedLogMessage(string formattedMessage, ILoggedItem originalLoggedItem);

        protected void LogUnexpectedExceptionToConsole(Exception ex)
        {
            if (ex != null)
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
#endif

                try
                {
                    Console.Error.WriteLine(typeof(ToFileAsyncLogObserver).Name + " Error : " + ex.Message);
                }
                catch
                {
                }
            }
        }

        private void OnLogEvent(object sender, LogEventHandlerArgs args)
        {
            this.OnFormattedLogMessage(LoggedItemFormatter.Format(args.LoggedItem), args.LoggedItem);
        }

        #endregion
    }
}