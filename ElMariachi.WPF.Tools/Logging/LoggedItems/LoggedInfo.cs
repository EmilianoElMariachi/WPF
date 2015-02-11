namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    /// <summary>
    /// A logged Information
    /// </summary>
    public class LoggedInfo : ILoggedItem
    {

        #region Fields & Properties

        public const string TYPE_TEXT = "Info";

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }

        #endregion

        #region Constructors

        public LoggedInfo(string infoMessage)
        {
            Text = infoMessage;
        }

        #endregion

    }
}