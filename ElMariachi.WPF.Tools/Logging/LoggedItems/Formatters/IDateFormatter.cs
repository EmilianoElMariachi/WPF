using System;

namespace ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters
{
    public interface IDateFormatter
    {
        string Format(DateTime dateTime);
    }
}