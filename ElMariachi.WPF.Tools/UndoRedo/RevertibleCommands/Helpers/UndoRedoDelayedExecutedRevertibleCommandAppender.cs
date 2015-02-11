using System;
using System.Reflection;
using System.Timers;
using System.Windows;
using ElMariachi.WPF.Tools.UndoRedo.EventsDefinition;

namespace ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers
{
    /// <summary>
    /// This class acts as a wrapper of object property value updater. It interacts with a <see cref="IUndoRedoService"/> when the property value is updated
    /// under the conditions defined below.
    /// 
    /// The property of the wrapped object is updated by calling the <see cref="UpdatePropertyValue"/> method.
    /// 
    /// If after being updated, the property value stay unupdated for at least the time defined by <see cref="DelayMs"/>, then a new <see cref="IRevertibleCommand"/>
    /// with the property change is added to the <see cref="IUndoRedoService"/>
    /// </summary>
    public class UndoRedoDelayedExecutedRevertibleCommandAppender<T> : IDisposable
    {

        #region Fields & Properties

        /// <summary>
        /// The default delay value in milliseconds
        /// </summary>
        public const int DEFAULT_DELAY_MS = 1000;

        private static readonly bool _canInvokeOnUiThread = Application.Current != null && Application.Current.Dispatcher != null;

        private readonly Timer _timer = new Timer(DEFAULT_DELAY_MS);

        private readonly IUndoRedoService _undoRedoService;
        private readonly object _instance;
        private readonly PropertyInfo _propertyInfo;
        private bool _invokeOnUiThreadIfPossible = true;

        /// <summary>
        /// Gets or sets a boolean indicating whether the <see cref="IRevertibleCommand"/> should be added in the <see cref="IUndoRedoService"/>
        /// in the UI Thread. If true, the invocations will be performed only if an UI thread is available.
        /// </summary>
        public bool InvokeOnUiThreadIfPossible
        {
            get { return _invokeOnUiThreadIfPossible; }
            set { _invokeOnUiThreadIfPossible = value; }
        }

        /// <summary>
        /// Gets or sets the waiting delay of the stable property value
        /// </summary>
        public double DelayMs
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        /// <summary>
        /// Gets a boolean indicating that a value to be flushed in the <see cref="IUndoRedoService"/> is pending
        /// </summary>
        public bool IsFlushPending { get; private set; }

        /// <summary>
        /// The reverted property value which will provided to the <see cref="IUndoRedoService"/> at flush time
        /// </summary>
        public T OldValueToFlush { get; set; }

        /// <summary>
        /// Gest the last updated value (see <see cref="UpdatePropertyValue"/>)
        /// </summary>
        public T LastUpdatedValue { get; private set; }

        /// <summary>
        /// Gets or sets a boolean indicating how the old property value is taken at flush time.
        /// If true, when the property value is updated whereas no flush is pending, then the object property is read and 
        /// assigned to <see cref="OldValueToFlush"/>.
        /// If false the user is responsible for setting the <see cref="OldValueToFlush"/> which will be used at flush time.
        /// </summary>
        public bool AutoReadOldValueToFlush { get; set; }

        /// <summary>
        /// Gets or sets the description of the <see cref="IRevertibleCommand"/> sent to the <see cref="IUndoRedoService"/>
        /// </summary>
        public string CommandDescription { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// </summary>
        /// <param name="undoRedoService">The Undo/Redo service in which property changes will be added</param>
        /// <param name="instance">The object for which the property will be updated</param>
        /// <param name="propertyInfo">The property owned by the provided object</param>
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

            _undoRedoService.BeforeUndoRedoCommandExecuted += OnBeforeUndoRedoCommandExecuted;
        }

        /// <summary>
        /// </summary>
        /// <param name="undoRedoService">The Undo/Redo service in which property changes will be added</param>
        /// <param name="instance">The object for which the property will be updated</param>
        /// <param name="propertyName">The name of an existing property in the provided object</param>
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
            _undoRedoService.BeforeUndoRedoCommandExecuted -= OnBeforeUndoRedoCommandExecuted;
        }

        public T ReadCurrentPropertyValue()
        {
            return (T)_propertyInfo.GetValue(_instance, null);
        }

        #endregion

        #region Private Methods

        private void OnBeforeUndoRedoCommandExecuted(object sender, BeforeUndoRedoCommandExecutedEventHandlerArgs args)
        {
            this.Flush();
        }

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