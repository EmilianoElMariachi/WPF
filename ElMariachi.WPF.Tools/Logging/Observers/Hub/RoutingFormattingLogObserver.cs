using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;
using ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// Représente un observateur de log capable de réémettre (router) les logs observés
    /// </summary>
    public class RoutingFormattingLogObserver : FormattingLogObserver
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

        public RoutingFormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        protected override void OnLoggedItem(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            NotifyFormattedLogEvent(new FormattedLogEventHandlerArgs(formattedMessage, originalLoggedItem));
        }

    }
}