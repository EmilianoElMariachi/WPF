using System;
using System.IO;

namespace ElMariachi.WPF.Tools.Logging.Observers.ToFile
{
    public class ToFileSyncLogObserver : FormattingLogObserver
    {

        #region Fields & Properties

        private string _logFilePath = null;

        /// <summary>
        /// Chemin du fichier de log
        /// </summary>
        public string LogFilePath
        {
            get { return _logFilePath; }
            set { _logFilePath = value; }
        }

        #endregion

        #region Constructor / Destructor

        public ToFileSyncLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {
        }

        #endregion

        #region Methods

        protected override void OnFormattedLogMessage(string formattedMessage, ILoggedItem originalLoggedItem)
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
            catch (Exception ex)
            {
                LogUnexpectedExceptionToConsole(ex);
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
            catch (Exception ex)
            {
                LogUnexpectedExceptionToConsole(ex);
            }
            return logFileDirectoryState;
        }

        #endregion

    }
}
