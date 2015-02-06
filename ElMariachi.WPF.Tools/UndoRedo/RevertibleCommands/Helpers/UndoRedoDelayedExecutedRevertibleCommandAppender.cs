using System;
using System.Reflection;
using System.Timers;
using System.Windows;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    /// <summary>
    /// Cette classe fournit un m�canisme d'attente d'�tat stable de modification d'une propri�t� d'objet avant d'envoyer la valeur
    /// modifi�e dans le service Undo/Redo
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
        /// Obtient ou d�fini un bool�en indiquant que l'ajout de la commande au service Undo/Redo doit �tre effectu� sur la Thread UI
        /// (Seulement si un Thread UI est pr�sent)
        /// </summary>
        public bool InvokeOnUiThreadIfPossible
        {
            get { return _invokeOnUiThreadIfPossible; }
            set { _invokeOnUiThreadIfPossible = value; }
        }

        /// <summary>
        /// Obtient ou d�fini la dur�e d'attente de l'�tat stable de la propri�t�
        /// </summary>
        public double DelayMs
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Obtient un bool�en qui indique si un flush est en attente.
        /// </summary>
        public bool IsFlushPending { get; private set; }

        /// <summary>
        /// La valeur pr�c�dente inscrite dans <see cref="IUndoRedoService"/> au moment du flush
        /// </summary>
        public T OldValueToFlush { get; set; }

        /// <summary>
        /// Obtient la derni�re valeur mise � jour
        /// </summary>
        public T LastUpdatedValue { get; private set; }

        /// <summary>
        /// Obtient ou d�fini un bool�en qui indique si la valeur ancienne qui sera flush�e est lue automatiquement
        /// � l'appel de la m�thode <see cref="UpdatePropertyValue"/>alors qu'aucun Flush n'est en attente
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