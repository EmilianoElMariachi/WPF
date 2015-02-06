namespace ElMariachi.WPF.Tools.Logging
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