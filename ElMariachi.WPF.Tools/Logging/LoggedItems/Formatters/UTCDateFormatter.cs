using System;
using System.Globalization;

namespace ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters
{
    public class UtcDateFormatter : IDateFormatter
    {

        #region Methods

        public string Format(DateTime dateTime)
        {
            return dateTime.ToString("R", new CultureInfo("en-US"));
        }

        #endregion

    }
}