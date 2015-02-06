using ElMariachi.WPF.Tools.Logging.Observers;

namespace ElMariachi.WPF.Tools.Logging.Tools
{
    public class BasicLogFormatter : ILoggedItemFormatter
    {
        public string Format(ILoggedException loggedException)
        {
            if (loggedException != null && loggedException.Exception != null)
            {
                return loggedException.TypeText + " - " + loggedException.Exception.Message;
            }
            else
            {
                return "NULL EXCEPTION";
            }
        }

        public string Format(ILoggedItem loggedItem)
        {
            return loggedItem.TypeText + " : " + loggedItem.Text;
        }
    }
}
