namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    /// <summary>
    /// A logged Warning
    /// </summary>
    public class LoggedWarning : ILoggedItem
    {

        #region Fields & Properties

        public const string TYPE_TEXT = "Warning";

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }

        #endregion

        #region Constructors

        public LoggedWarning(string warningMessage)
        {
            Text = warningMessage;
        }

        #endregion

    }
}