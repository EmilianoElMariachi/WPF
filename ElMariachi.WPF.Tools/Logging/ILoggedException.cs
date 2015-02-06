using System;

namespace ElMariachi.WPF.Tools.Logging
{
    public interface ILoggedException : ILoggedItem
    {
        Exception Exception { get; }
    }
}