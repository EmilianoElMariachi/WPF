namespace ElMariachi.WPF.Tools.Logging.LoggedItems
{
    public class LoggedInfo : ILoggedItem
    {
        public const string TYPE_TEXT = "Info";

        public LoggedInfo(string infoMessage)
        {
            Text = infoMessage;
        }

        public string Text { get; private set; }

        public string TypeText
        {
            get { return TYPE_TEXT; }
        }
    }
}