using System;
using ElMariachi.WPF.Tools.Logging.EventsDefinition;
using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;

namespace ElMariachi.WPF.Tools.Logging.Observers
{

    /// <summary>
    /// This class has a basic implementation used for listening a <see cref="IObservableLog"/> and formatting received <see cref="ILoggedItem"/>
    /// Inherit this class and implement method <see cref="OnLoggedItem"/> to handle formatted logged items from the <see cref="IObservableLog"/> given at constructor
    /// </summary>
    public abstract class FormattingLogObserver : IDisposable
    {

        #region Fields & Properties

        private readonly IObservableLog _observableLog;
        private readonly ILoggedItemFormatter _loggedItemFormatter;

        public IObservableLog ObservedLog
        {
            get { return _observableLog; }
        }

        #endregion

        #region Constructor

        public FormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
        {
            if (observableLog == null) { throw new ArgumentNullException("observableLog"); }
            if (loggedItemFormatter == null) { throw new ArgumentNullException("loggedItemFormatter"); }

            _observableLog = observableLog;
            _loggedItemFormatter = loggedItemFormatter;
            _observableLog.LogEvent += OnLogEvent;
        }

        #endregion

        #region Methods

        public virtual void Dispose()
        {
            _observableLog.LogEvent -= OnLogEvent;
        }

        protected abstract void OnLoggedItem(string formattedLoggedItem, ILoggedItem loggedItem);

        private void OnLogEvent(object sender, LogEventHandlerArgs args)
        {
            this.OnLoggedItem(_loggedItemFormatter.Format(args.LoggedItem), args.LoggedItem);
        }

        #endregion

    }
}