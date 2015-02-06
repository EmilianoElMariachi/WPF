namespace ElMariachi.WPF.Tools.Logging.Observers
{

    public class ToStringLogObserver : FormattingLogObserver
    {
        private string _log = "";

        public string LogValue
        {
            get { return _log; }
            private set
            {
                _log = value;
            }
        }

        public ToStringLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        private void AppengFormattedLoggedMessage(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            LogValue += formattedMessage;
        }

        protected override void OnFormattedLogMessage(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            AppengFormattedLoggedMessage(formattedMessage, originalLoggedItem);
        }

    }
}
