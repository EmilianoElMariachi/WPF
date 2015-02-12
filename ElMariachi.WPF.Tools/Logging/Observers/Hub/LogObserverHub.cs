using System;
using System.Collections.Generic;
using System.Linq;
using ElMariachi.WPF.Tools.Logging.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// This class acts like a hub of <see cref="IObservableLog"/>.
    /// Use this class if you want to concentrate the log events of several <see cref="IObservableLog"/>
    /// in a single method <see cref="OnLogEvent"/>
    /// </summary>
    public abstract class LogObserverHub
    {

        #region Fields & Properties

        private readonly HashSet<IObservableLog> _observedLogs = new HashSet<IObservableLog>();

        #endregion

        #region Methods

        /// <summary>
        /// Adds an observed log to the hub
        /// </summary>
        /// <param name="observableLog"></param>
        public bool AddObservedLog(IObservableLog observableLog)
        {
            if (observableLog == null) { throw new ArgumentNullException("observableLog"); }

            if (!_observedLogs.Contains(observableLog))
            {
                _observedLogs.Add(observableLog);
                SubscribeToEvents(observableLog);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Removes an observed log from the hub
        /// </summary>
        public void RemoveAllObservedLogs()
        {
            var routingLogObserversCopyTmp = _observedLogs.ToArray();
            foreach (var routingLogObserver in routingLogObserversCopyTmp)
            {
                RemoveLogObserver(routingLogObserver);
            }
        }

        /// <summary>
        /// Removes all observed logs from the hub
        /// </summary>
        /// <param name="observableLog"></param>
        /// <returns>true si l'élément a été enlevé</returns>
        public bool RemoveLogObserver(IObservableLog observableLog)
        {
            var removed = false;

            if (observableLog != null)
            {
                removed = _observedLogs.Remove(observableLog);

                if (removed)
                {
                    UnsubscribeToEvents(observableLog);
                }
            }

            return removed;
        }

        private void SubscribeToEvents(IObservableLog observableLog)
        {
            if (observableLog != null)
            {
                observableLog.LogEvent += OnLogEvent;
            }
        }

        private void UnsubscribeToEvents(IObservableLog observableLog)
        {
            if (observableLog != null)
            {
                observableLog.LogEvent -= OnLogEvent;
            }
        }

        protected abstract void OnLogEvent(object sender, LogEventHandlerArgs args);

        #endregion

    }

}