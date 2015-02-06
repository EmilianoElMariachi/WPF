using System;

namespace ElMariachi.WPF.Tools.Logging.Tools
{
    public class DefaultExceptionFormatter : IExceptionFormatter
    {
        public string Format(Exception exception)
        {
            if (exception == null)
            {
                exception = new Exception("NULL EXCEPTION");
            }   

            var formattedMessage = exception.Message + Environment.NewLine + "STACK TRACE :" + Environment.NewLine + exception.StackTrace + "INNER EXCEPTION MESSAGES :" + BuildInnerExceptionMessages(exception);

            return formattedMessage;
        }

        private static string BuildInnerExceptionMessages(Exception ex)
        {
            var innerExceptionMessages = "";

            if (ex != null)
            { ex = ex.InnerException; }

            while (ex != null)
            {
                innerExceptionMessages += Environment.NewLine + ex.Message;
                ex = ex.InnerException;
            }

            return innerExceptionMessages;
        }

    }
}