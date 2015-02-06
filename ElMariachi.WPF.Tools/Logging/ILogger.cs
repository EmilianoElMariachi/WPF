using ElMariachi.WPF.Tools.Logging.LoggedItems;

namespace ElMariachi.WPF.Tools.Logging
{
    /// <summary>
    /// Primary logger interface
    /// </summary>
    public interface ILogger
    {
        void Log(ILoggedItem loggedItem);
    }
}