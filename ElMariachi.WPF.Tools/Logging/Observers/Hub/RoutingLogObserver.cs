using ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// Représente un obserateur de log capable de réémettre (router) les logs observés
    /// </summary>
    public class RoutingLogObserver : FormattingLogObserver
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

        public RoutingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        protected override void OnFormattedLogMessage(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            NotifyFormattedLogEvent(new FormattedLogEventHandlerArgs(formattedMessage, originalLoggedItem));
        }

    }
}