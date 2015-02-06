using System;

namespace ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters
{
    public class TimestampedLogFormatter : ILoggedItemFormatter
    {

        #region Fields & Properties

        private readonly IDateFormatter _dateFormatter;

        #endregion

        #region Constructors

        public TimestampedLogFormatter()
        {
            _dateFormatter = new UtcDateFormatter();
        }

        public TimestampedLogFormatter(IDateFormatter dateFormatter)
        {
            if (dateFormatter == null)
            {
                throw new ArgumentNullException("dateFormatter");
            }

            _dateFormatter = dateFormatter;
        }

        #endregion

        #region Methods

        public string Format(ILoggedItem loggedItem)
        {
            return loggedItem.TypeText + " @ " + _dateFormatter.Format(DateTime.Now) + " : " + loggedItem.Text;
        }

        #endregion

    }
}