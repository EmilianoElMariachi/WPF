using System;
using System.Reflection;
using System.Timers;
using System.Windows;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    /// <summary>
    /// Cette classe fournit un mécanisme d'attente d'état stable de modification d'une propriété d'objet avant d'envoyer la valeur
    /// modifiée dans le service Undo/Redo
    /// 
    /// </summary>
    public class UndoRedoDelayedExecutedRevertibleCommandAppender<T> : IDisposable
    {
        #region Fields & Accessors

        private static readonly bool _canInvokeOnUiThread = Application.Current != null && Application.Current.Dispatcher != null;

        private readonly Timer _timer = new Timer(1000);

        private readonly IUndoRedoService _undoRedoService;
        private readonly object _instance;
        private readonly PropertyInfo _propertyInfo;
        private bool _invokeOnUiThreadIfPossible = true;

        /// <summary>
        /// Obtient ou défini un booléen indiquant que l'ajout de la commande au service Undo/Redo doit être effectué sur la Thread UI
        /// (Seulement si un Thread UI est présent)
        /// </summary>
        public bool InvokeOnUiThreadIfPossible
        {
            get { return _invokeOnUiThreadIfPossible; }
            set { _invokeOnUiThreadIfPossible = value; }
        }

        /// <summary>
        /// Obtient ou défini la durée d'attente de l'état stable de la propriété
        /// </summary>
        public double DelayMs
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Obtient un booléen qui indique si un flush est en attente.
        /// </summary>
        public bool IsFlushPending { get; private set; }

        /// <summary>
        /// La valeur précédente inscrite dans <see cref="IUndoRedoService"/> au moment du flush
        /// </summary>
        public T OldValueToFlush { get; set; }

        /// <summary>
        /// Obtient la dernière valeur mise à jour
        /// </summary>
        public T LastUpdatedValue { get; private set; }

        /// <summary>
        /// Obtient ou défini un booléen qui indique si la valeur ancienne qui sera flushée est lue automatiquement
        /// à l'appel de la méthode <see cref="UpdatePropertyValue"/>alors qu'aucun Flush n'est en attente
        /// </summary>
        public bool AutoReadOldValueToFlush { get; set; }

        #endregion

        #region Constructors

        public UndoRedoDelayedExecutedRevertibleCommandAppender(IUndoRedoService undoRedoService, object instance, PropertyInfo propertyInfo)
        {
            if (undoRedoService == null)
            {
                throw new ArgumentNullException("undoRedoService");
            }
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            _undoRedoService = undoRedoService;
            _instance = instance;
            _propertyInfo = propertyInfo;

            IsFlushPending = false;

            _timer.Stop();
            _timer.AutoReset = false;
            _timer.Enabled = true;
            _timer.Elapsed += OnTimerElapsed;

            _undoRedoService.BeforeUndoRedoCommandExecuted += (sender, args) => Flush();
        }

        public UndoRedoDelayedExecutedRevertibleCommandAppender(IUndoRedoService undoRedoService, object instance, string propertyName)
            : this(undoRedoService, instance, instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
        }

        #endregion

        #region Public Methods

        public void UpdatePropertyValue(T newValue)
        {
            if (!this.IsFlushPending)
            {
                if (AutoReadOldValueToFlush)
                {
                    OldValueToFlush = ReadCurrentPropertyValue();
                }
                this.IsFlushPending = true;
            }

            LastUpdatedValue = newValue;

            RestartTimer();
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public T ReadCurrentPropertyValue()
        {
            return (T)_propertyInfo.GetValue(_instance, null);
        }

        #endregion

        #region Private Methods

        private void OnTimerElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (this.InvokeOnUiThreadIfPossible && _canInvokeOnUiThread && !Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(new Action(Flush));
            }
            else
            {
                Flush();
            }
        }

        private void Flush()
        {
            if (this.IsFlushPending)
            {
                this.IsFlushPending = false;
                StopTimer();

                var changePropertyRevertibleCommand = new ChangePropertyRevertibleCommand(_instance, _propertyInfo, LastUpdatedValue, OldValueToFlush, CommandDescription);
                _undoRedoService.AddExecutedCommand(changePropertyRevertibleCommand);
            }
        }

        public string CommandDescription { get; set; }

        private void RestartTimer()
        {
            _timer.Stop();
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
        }

        #endregion

    }
}