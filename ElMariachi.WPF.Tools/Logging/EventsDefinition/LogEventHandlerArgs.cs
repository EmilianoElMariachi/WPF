using ElMariachi.WPF.Tools.Logging.LoggedItems;

namespace ElMariachi.WPF.Tools.Logging.EventsDefinition
{
    public class LogEventHandlerArgs
    {
        private readonly ILoggedItem _loggedItem;

        public LogEventHandlerArgs(ILoggedItem loggedItem)
        {
            _loggedItem = loggedItem;
        }

        public ILoggedItem LoggedItem
        {
            get { return _loggedItem; }
        }
    }
}