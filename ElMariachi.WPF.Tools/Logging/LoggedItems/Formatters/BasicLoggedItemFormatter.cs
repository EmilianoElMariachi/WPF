namespace ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters
{
    public class BasicLoggedItemFormatter : ILoggedItemFormatter
    {
        public string Format(ILoggedItem loggedItem)
        {
            if (loggedItem != null)
            {
                return loggedItem.TypeText + " : " + loggedItem.Text;
            }
            return "";
        }
    }
}
