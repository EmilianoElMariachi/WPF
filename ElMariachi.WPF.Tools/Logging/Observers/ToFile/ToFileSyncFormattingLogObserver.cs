using System.IO;
using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;

namespace ElMariachi.WPF.Tools.Logging.Observers.ToFile
{
    public class ToFileSyncFormattingLogObserver : FormattingLogObserver
    {

        #region Fields & Properties

        private string _logFilePath = null;

        /// <summary>
        /// The log file path
        /// </summary>
        public string LogFilePath
        {
            get { return _logFilePath; }
            set { _logFilePath = value; }
        }

        #endregion

        #region Constructor / Destructor

        public ToFileSyncFormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        #endregion

        #region Methods

        protected override void OnLoggedItem(string formattedMessage, ILoggedItem originalLoggedItem)
        {
            WriteToFileSecure(formattedMessage);
        }

        protected virtual void WriteToFileSecure(string message)
        {
            try
            {
                var logFilePath = this.LogFilePath;
                if (!string.IsNullOrEmpty(logFilePath) && TryCreateIntermediateLogFileDirectoriesSecure() == LogFileDirectoryState.EXIST)
                {
                    File.AppendAllText(logFilePath, message);
                }
            }
            catch
            {
            }
        }

        private enum LogFileDirectoryState
        {
            EXIST,
            DO_NOT_EXIST,
        }

        private LogFileDirectoryState TryCreateIntermediateLogFileDirectoriesSecure()
        {
            var logFileDirectoryState = LogFileDirectoryState.DO_NOT_EXIST;
            try
            {
                if (LogFilePath != null)
                {
                    var logDirectory = Path.GetDirectoryName(LogFilePath);
                    if (!Directory.Exists(logDirectory))
                    {
                        Directory.CreateDirectory(logDirectory);
                    }
                    logFileDirectoryState = LogFileDirectoryState.EXIST;
                }
            }
            catch
            {

            }
            return logFileDirectoryState;
        }

        #endregion

    }
}
