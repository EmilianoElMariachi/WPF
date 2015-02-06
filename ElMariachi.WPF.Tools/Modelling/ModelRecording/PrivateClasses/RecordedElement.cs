namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{
    internal abstract class RecordedElement
    {

        #region Fields & Properties

        private readonly object _oldValue;

        internal object OldValue
        {
            get { return _oldValue; }
        }

        #endregion

        #region Constructors

        internal RecordedElement(object oldValue)
        {
            _oldValue = oldValue;
        }

        #endregion

        #region Methods

        internal abstract void Release();

        #endregion

    }
}