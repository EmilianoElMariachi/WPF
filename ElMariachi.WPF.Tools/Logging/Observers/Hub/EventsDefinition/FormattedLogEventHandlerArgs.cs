namespace ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition
{
    public class FormattedLogEventHandlerArgs
    {
        private readonly string _formattedMessage;

        public string FormattedMessage
        {
            get { return _formattedMessage; }
        }

        private readonly ILoggedItem _originalLoggedItem;

        public ILoggedItem OriginalLoggedItem
        {
            get { return _originalLoggedItem; }
        }

        public FormattedLogEventHandlerArgs(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            _formattedMessage = formattedMessage;
            _originalLoggedItem = originalLoggedItem;
        }
    }
}