using System;

namespace ElMariachi.WPF.Tools.Logging.Observers
{
    public class ToConsoleLogObserver : FormattingLogObserver
    {
        public ToConsoleLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        protected override void OnFormattedLogMessage(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            Console.Write(formattedMessage);
        }
    }
}
