namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    public class LoggedWarning : ILoggedItem
    {
        public const string TYPE_TEXT = "Warning";

        public LoggedWarning(string warningMessage)
        {
            Text = warningMessage;
        }

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }
    }
}