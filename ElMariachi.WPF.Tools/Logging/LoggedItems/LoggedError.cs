namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    public class LoggedError : ILoggedItem
    {
        public const string TYPE_TEXT = "Error";

        public LoggedError(string errorMessage)
        {
            Text = errorMessage;
        }

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }
    }
}