using System;
using System.Globalization;

namespace ElMariachi.WPF.Tools.Logging.Tools
{
    public class UtcDateFormatter : IDateFormatter
    {
        public string Format(DateTime dateTime)
        {
            return dateTime.ToString("R", new CultureInfo("en-US"));
        }
    }
}