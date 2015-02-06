using System;
using System.Collections.Generic;
using ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// Classe abstraite capable d'écouter une liste de Logger Observables.
    /// Il est à la charge des classes dérivées de disposer de la concentration des logs observés
    /// </summary>
    public abstract class AbstractLogObserverHub
    {
        #region Fields

        private readonly List<RoutingLogObserver> _routingLogObservers = new List<RoutingLogObserver>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Ajoute un observeur de log au concentrateur.
        /// Jette une exception si l'argument est null.
        /// </summary>
        /// <param name="routingLogObserver"></param>
        public void AddLogObserver(RoutingLogObserver routingLogObserver)
        {
            if (routingLogObserver == null)
            {
                throw new ArgumentNullException("routingLogObserver", "Can't add null \"" + typeof(RoutingLogObserver) + "\" to " + this.GetType().Name + "\".");
            }

            if (!_routingLogObservers.Contains(routingLogObserver))
            {
                _routingLogObservers.Add(routingLogObserver);
            }

            SubscribeToEvents(routingLogObserver);
        }

        /// <summary>
        /// Supprime tous les observeurs de log
        /// </summary>
        public void RemoveAllLogObservers()
        {
            var routingLogObserversCopyTmp = _routingLogObservers.ToArray();
            foreach (var routingLogObserver in routingLogObserversCopyTmp)
            {
                RemoveLogObserver(routingLogObserver);
            }
        }

        /// <summary>
        /// Supprime un observeur de log du concentrateur
        /// </summary>
        /// <param name="routingLogObserver"></param>
        /// <returns>true si l'élément a été enlevé</returns>
        public bool RemoveLogObserver(RoutingLogObserver routingLogObserver)
        {
            var removed = false;

            if (routingLogObserver != null)
            {
                removed = _routingLogObservers.Remove(routingLogObserver);

                if (removed)
                {
                    UnsubscribeToEvents(routingLogObserver);
                }
            }

            return removed;
        }

        /// <summary>
        /// Supprime l'observeur de log en charge de surveillé le log observable spécifié, et retourne ce dernier
        /// </summary>
        /// <param name="observedLog"></param>
        /// <returns>Null si aucun n'observeur de log surveillant le log observable spécifié n'a été trouvé</returns>
        public RoutingLogObserver RemoveLogObserverByObservedLog(IObservableLog observedLog)
        {
            foreach (var rootingLogObserver in _routingLogObservers)
            {
                if (rootingLogObserver.ObservedLog == observedLog)
                {
                    RemoveLogObserver(rootingLogObserver);
                    return rootingLogObserver;
                }
            }
            return null;
        }

        #endregion

        #region Private / Abstract Methods

        private void SubscribeToEvents(RoutingLogObserver observableLog)
        {
            if (observableLog != null)
            {
                observableLog.FormattedLogEvent += OnFormattedLogEvent;
            }
        }

        private void OnFormattedLogEvent(object sender, FormattedLogEventHandlerArgs args)
        {
            OnFormattedLogEvent(args.FormattedMessage, args.OriginalLoggedItem);
        }

        private void UnsubscribeToEvents(RoutingLogObserver observableLog)
        {
            if (observableLog != null)
            {
                observableLog.FormattedLogEvent -= OnFormattedLogEvent;
            }
        }

        protected abstract void OnFormattedLogEvent(string formattedmessage, ILoggedItem originalLoggedItem);

        #endregion
    }
}