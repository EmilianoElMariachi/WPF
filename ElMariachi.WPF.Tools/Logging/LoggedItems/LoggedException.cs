using System;

namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    /// <summary>
    /// A logged Exception
    /// </summary>
    public class LoggedException : ILoggedItem
    {

        #region Fields & Properties

        public const string TYPE_TEXT = "Exception";

        public string Text
        {
            get { return _exception != null ? _exception.Message : null; }
        }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }

        private readonly Exception _exception;

        public Exception Exception
        {
            get { return _exception; }
        }

        #endregion

        #region Constructors

        public LoggedException(Exception exception)
        {
            _exception = exception;
        }

        #endregion

    }
}