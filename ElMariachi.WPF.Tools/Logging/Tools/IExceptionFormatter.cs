using System;

namespace ElMariachi.WPF.Tools.Logging.Tools
{
    public interface IExceptionFormatter
    {
        string Format(Exception exception);
    }
}