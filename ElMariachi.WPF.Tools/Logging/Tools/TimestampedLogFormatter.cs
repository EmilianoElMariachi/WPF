using System;
using ElMariachi.WPF.Tools.Logging.Observers;

namespace ElMariachi.WPF.Tools.Logging.Tools
{
    public class TimestampedLogFormatter : ILoggedItemFormatter
    {
        private readonly IDateFormatter _dateFormatter;
        private readonly IExceptionFormatter _exceptionFormatter;

        public TimestampedLogFormatter()
        {
            _dateFormatter = new UtcDateFormatter();
            _exceptionFormatter = new DefaultExceptionFormatter();
        }

        public TimestampedLogFormatter(IDateFormatter dateFormatter, IExceptionFormatter exceptionFormatter)
        {
            if (dateFormatter == null)
            {
                throw new ArgumentNullException("dateFormatter");
            }

            if (exceptionFormatter == null)
            {
                throw new ArgumentNullException("exceptionFormatter");
            }

            _dateFormatter = dateFormatter;
            _exceptionFormatter = exceptionFormatter;
        }

        public string Format(ILoggedItem loggedItem)
        {
            return loggedItem.TypeText + " @ " + _dateFormatter.Format(DateTime.Now) + " : " + loggedItem.Text;
        }

        public string Format(ILoggedException loggedException)
        {
            return loggedException.TypeText + " @ " + _dateFormatter.Format(DateTime.Now) + _exceptionFormatter.Format(loggedException.Exception);
        }
    }
}