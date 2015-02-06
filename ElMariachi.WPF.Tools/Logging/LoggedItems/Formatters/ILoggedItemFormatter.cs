namespace ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters
{

    /// <summary>
    /// Format a <see cref="ILoggedItem"/> to a string
    /// </summary>
    public interface ILoggedItemFormatter
    {
        string Format(ILoggedItem loggedItem);
    }
}