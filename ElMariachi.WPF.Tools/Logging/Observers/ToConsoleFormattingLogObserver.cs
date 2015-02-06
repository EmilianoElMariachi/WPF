using System;
using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;

namespace ElMariachi.WPF.Tools.Logging.Observers
{
    /// <summary>
    /// Log and format all log events raised by <see cref="IObservableLog"/> to the <see cref="Console"/>
    /// </summary>
    public class ToConsoleFormattingLogObserver : FormattingLogObserver
    {

        #region Constructors

        public ToConsoleFormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        #endregion

        #region Methods

        protected override void OnLoggedItem(string formattedLoggedItem, ILoggedItem loggedItem)
        {
            Console.Write(formattedLoggedItem);
        }

        #endregion

    }
}
