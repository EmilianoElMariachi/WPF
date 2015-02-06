using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using ElMariachi.WPF.Tools.Logging.LoggedItems.Formatters;

namespace ElMariachi.WPF.Tools.Logging.Observers.ToFile
{
    public class ToFileAsyncFormattingLogObserver : ToFileSyncFormattingLogObserver
    {

        #region Fields & Properties

        private readonly object _fileWrittingLock = new object();
        private readonly object _pendingLogWorkersStackLock = new object();

        private readonly List<PendingLogWorker> _pendingLogWorkersStack = new List<PendingLogWorker>();
        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(false);

        #endregion

        #region Constructor / Destructor

        public ToFileAsyncFormattingLogObserver(IObservableLog observableLog, ILoggedItemFormatter loggedItemFormatter)
            : base(observableLog, loggedItemFormatter)
        {

        }

        #endregion

        #region Public Methods

        public TimeoutResult WaitForPendingLogs(int timeoutMs)
        {
            var timeoutResult = (_pendingLogWorkersStack.Count <= 0 || _autoResetEvent.WaitOne(timeoutMs)) ? TimeoutResult.NO_TIMEOUT : TimeoutResult.TIMEOUT_OCCURRED;
            return timeoutResult;
        }

        #endregion

        #region Private Methods

        protected override void WriteToFileSecure(string formattedMessage)
        {
            StartAsyncLogFileWriterThread(formattedMessage);
        }

        private void StartAsyncLogFileWriterThread(string formattedMessage)
        {
            lock (_pendingLogWorkersStackLock)
            {
                PendingLogWorker pendingLogBackgroundWorker = null;

                try
                {
                    pendingLogBackgroundWorker = new PendingLogWorker(formattedMessage);
                    pendingLogBackgroundWorker.BackgroundWorker.DoWork += AsyncLog;
                    pendingLogBackgroundWorker.BackgroundWorker.RunWorkerCompleted += AsyncLogCompleted;
                    _pendingLogWorkersStack.Add(pendingLogBackgroundWorker);

                    if (_pendingLogWorkersStack.Count == 1) //
                    {
                        _autoResetEvent.Reset();
                        pendingLogBackgroundWorker.Run();
                    }
                }
                catch (Exception ex)
                {
                    if (pendingLogBackgroundWorker != null)
                    {
                        _pendingLogWorkersStack.Remove(pendingLogBackgroundWorker);
                    }
                }
            }
        }

        private void AsyncLog(object sender, DoWorkEventArgs eventArgs)
        {
            lock (_fileWrittingLock)
            {
                var args = (object[])eventArgs.Argument;
                var formattedMessage = (string)args[0];
                base.WriteToFileSecure(formattedMessage);
            }
        }

        private void AsyncLogCompleted(object sender, RunWorkerCompletedEventArgs runWorkerCompletedEventArgs)
        {
            lock (_pendingLogWorkersStackLock)
            {
                RemoveEndedLogWorkerFromStack(sender);

                RunNextPengingLogWorker();
            }
        }

        private void RemoveEndedLogWorkerFromStack(object sender)
        {
            var endingLogWorker = _pendingLogWorkersStack.Find(pendingLogWorker => pendingLogWorker.BackgroundWorker == sender);
            if (endingLogWorker != null)
            {
                _pendingLogWorkersStack.Remove(endingLogWorker);
                endingLogWorker.BackgroundWorker.DoWork -= AsyncLog;
                endingLogWorker.BackgroundWorker.RunWorkerCompleted -= AsyncLogCompleted;
                endingLogWorker.BackgroundWorker.Dispose();
            }
        }

        private void RunNextPengingLogWorker()
        {
            if (_pendingLogWorkersStack.Count > 0)
            {
                var pendingLogBackgroundWorker = _pendingLogWorkersStack[0];
                pendingLogBackgroundWorker.Run();
            }

            if (_pendingLogWorkersStack.Count <= 0 && !_autoResetEvent.SafeWaitHandle.IsClosed)
            {
                _autoResetEvent.Set();
            }
        }

        #endregion

        #region Private Classes

        private class PendingLogWorker
        {
            private readonly BackgroundWorker _backgroundWorker;
            readonly private object[] _arguments;

            public BackgroundWorker BackgroundWorker
            {
                get { return _backgroundWorker; }
            }

            public PendingLogWorker(string message)
            {
                _arguments = new object[] { message };
                _backgroundWorker = new BackgroundWorker();
            }

            public void Run()
            {
                this.BackgroundWorker.RunWorkerAsync(_arguments);
            }
        }

        #endregion

    }
}