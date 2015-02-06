using ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// Représente une classe capable d'écouter un multitude de log observables, et de
    /// concentrer l'ensemble des logs au sein des évènements FormattedLogEvent et FormattedLogLineEvent
    /// </summary>
    public class RoutingLogObserverHub : AbstractLogObserverHub
    {
        public event FormattedLogEventHandler FormattedLogEvent;

        protected virtual void NotifyFormattedLogEvent(FormattedLogEventHandlerArgs args)
        {
            var handler = FormattedLogEvent;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected override void OnFormattedLogEvent(string formattedmessage, ILoggedItem originalLoggedItem)
        {
            NotifyFormattedLogEvent(new FormattedLogEventHandlerArgs(formattedmessage, originalLoggedItem));
        }

    }
}