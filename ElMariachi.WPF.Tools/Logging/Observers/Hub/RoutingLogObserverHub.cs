using ElMariachi.WPF.Tools.Logging.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// This hub allows the redirection of all <see cref="IObservableLog"/> log events to a single event <see cref="HubLogEvent"/>
    /// </summary>
    public class RoutingLogObserverHub : LogObserverHub
    {

        #region Events

        public event LogEventHandler HubLogEvent;

        private void NotifyLogEvent(LogEventHandlerArgs args)
        {
            var handler = HubLogEvent;
            if (handler != null) handler(this, args);
        }

        #endregion

        #region Methods

        protected override void OnLogEvent(object sender, LogEventHandlerArgs args)
        {
            NotifyLogEvent(args);
        }

        #endregion

    }
}