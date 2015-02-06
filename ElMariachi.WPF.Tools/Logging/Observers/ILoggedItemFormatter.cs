namespace ElMariachi.WPF.Tools.Logging.Observers
{
    public interface ILoggedItemFormatter
    {
        string Format(ILoggedItem loggedItem);

        string Format(ILoggedException loggedException);
    }
}