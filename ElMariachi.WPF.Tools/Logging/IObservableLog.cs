using ElMariachi.WPF.Tools.Logging.EventsDefinition;
using ElMariachi.WPF.Tools.Logging.LoggedItems;

namespace ElMariachi.WPF.Tools.Logging
{

    /// <summary>
    /// Interface of a class capable of notifying <see cref="ILoggedItem"/> events
    /// </summary>
    public interface IObservableLog
    {
        event LogEventHandler LogEvent;
    }
}
