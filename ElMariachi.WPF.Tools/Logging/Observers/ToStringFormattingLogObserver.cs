using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;

namespace ElMariachi.WPF.Tools.Logging.Observers
{

    /// <summary>
    /// Log and format all log events raised by <see cref="IObservableLog"/> to a string
    /// </summary>
    public class ToStringFormattingLogObserver : FormattingLogObserver
    {

        #region Fields & Properties

        private string _log = "";

        public string LogValue
        {
            get { return _log; }
            private set { _log = value; }
        }

        #endregion

        #region Constructors

        public ToStringFormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        #endregion

        #region Methods

        private void AppengFormattedLoggedMessage(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            LogValue += formattedMessage;
        }

        protected override void OnLoggedItem(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            AppengFormattedLoggedMessage(formattedMessage, originalLoggedItem);
        }

        #endregion

    }
}
