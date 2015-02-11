namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    /// <summary>
    /// A logged Error
    /// </summary>
    public class LoggedError : ILoggedItem
    {

        #region Fields & Properties

        public const string TYPE_TEXT = "Error";

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }

        #endregion

        #region Constructors

        public LoggedError(string errorMessage)
        {
            Text = errorMessage;
        }

        #endregion

    }
}