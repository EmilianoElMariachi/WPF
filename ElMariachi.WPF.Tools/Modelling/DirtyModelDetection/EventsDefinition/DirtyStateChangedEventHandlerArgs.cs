namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.EventsDefinition
{
    public class DirtyStateChangedEventHandlerArgs
    {

        #region Fields & Properties

        private readonly object _observedModel;

        /// <summary>
        /// The dirty object or null when detection is stopped
        /// </summary>
        public object ObservedModel
        {
            get { return _observedModel; }
        }

        private readonly bool _isDirty;

        /// <summary>
        /// True when observed object is dirty or when detection is stopped
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
        }

        #endregion

        #region Constructor(s)

        internal DirtyStateChangedEventHandlerArgs(object observedModel, bool isDirty)
        {
            _observedModel = observedModel;
            _isDirty = isDirty;
        }

        #endregion

    }
}