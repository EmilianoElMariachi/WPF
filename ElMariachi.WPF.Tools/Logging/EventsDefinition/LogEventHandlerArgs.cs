using ElMariachi.WPF.Tools.Logging.LoggedItems;

namespace ElMariachi.WPF.Tools.Logging.EventsDefinition
{
    public class LogEventHandlerArgs
    {

        #region Fields & Properties

        private readonly ILoggedItem _loggedItem;
        /// <summary>
        /// The logged item
        /// </summary>
        public ILoggedItem LoggedItem
        {
            get { return _loggedItem; }
        }

        #endregion

        #region Constructors

        public LogEventHandlerArgs(ILoggedItem loggedItem)
        {
            _loggedItem = loggedItem;
        }

        #endregion

    }
}