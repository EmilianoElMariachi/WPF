using System;
using System.Collections.Generic;
using ElMariachi.WPF.Tools.Logging.LoggedItems;
using ElMariachi.WPF.Tools.Logging.Observers.Hub.EventsDefinition;

namespace ElMariachi.WPF.Tools.Logging.Observers.Hub
{
    /// <summary>
    /// TODO ELM : à traduire
    /// 
    /// Classe abstraite capable d'écouter une liste de Logger Observables.
    /// Il est à la charge des classes dérivées de disposer de la concentration des logs observés
    /// 
    /// TODO ELM : supprimer le formattage pour ne faire qu'un HUB simple
    /// </summary>
    public abstract class AbstractLogObserverHub
    {
        #region Fields

        private readonly List<RoutingFormattingLogObserver> _routingLogObservers = new List<RoutingFormattingLogObserver>();

        #endregion

        #region Public Methods

        /// <summary>
        /// Ajoute un observeur de log au concentrateur.
        /// Jette une exception si l'argument est null.
        /// </summary>
        /// <param name="routingFormattingLogObserver"></param>
        public void AddLogObserver(RoutingFormattingLogObserver routingFormattingLogObserver)
        {
            if (routingFormattingLogObserver == null)
            {
                throw new ArgumentNullException("routingFormattingLogObserver", "Can't add null \"" + typeof(RoutingFormattingLogObserver) + "\" to " + this.GetType().Name + "\".");
            }

            if (!_routingLogObservers.Contains(routingFormattingLogObserver))
            {
                _routingLogObservers.Add(routingFormattingLogObserver);
            }

            SubscribeToEvents(routingFormattingLogObserver);
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
        /// <param name="routingFormattingLogObserver"></param>
        /// <returns>true si l'élément a été enlevé</returns>
        public bool RemoveLogObserver(RoutingFormattingLogObserver routingFormattingLogObserver)
        {
            var removed = false;

            if (routingFormattingLogObserver != null)
            {
                removed = _routingLogObservers.Remove(routingFormattingLogObserver);

                if (removed)
                {
                    UnsubscribeToEvents(routingFormattingLogObserver);
                }
            }

            return removed;
        }

        /// <summary>
        /// Supprime l'observeur de log en charge de surveillé le log observable spécifié, et retourne ce dernier
        /// </summary>
        /// <param name="observedLog"></param>
        /// <returns>Null si aucun n'observeur de log surveillant le log observable spécifié n'a été trouvé</returns>
        public RoutingFormattingLogObserver RemoveLogObserverByObservedLog(IObservableLog observedLog)
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

        private void SubscribeToEvents(RoutingFormattingLogObserver observableFormattingLog)
        {
            if (observableFormattingLog != null)
            {
                observableFormattingLog.FormattedLogEvent += OnFormattedLogEvent;
            }
        }

        private void OnFormattedLogEvent(object sender, FormattedLogEventHandlerArgs args)
        {
            OnFormattedLogEvent(args.FormattedMessage, args.OriginalLoggedItem);
        }

        private void UnsubscribeToEvents(RoutingFormattingLogObserver observableFormattingLog)
        {
            if (observableFormattingLog != null)
            {
                observableFormattingLog.FormattedLogEvent -= OnFormattedLogEvent;
            }
        }

        protected abstract void OnFormattedLogEvent(string formattedmessage, ILoggedItem originalLoggedItem);

        #endregion

    }

}