namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes
{
    /// <summary>
    /// Use this attribute on properties to indicate that the property changes should be recorded with a filter.
    /// (See <see cref="IModelRecorder"/>)
    /// </summary>
    public class IsRecordableWithFilterAttribute : IsRecordableAttribute
    {

        #region Fields & Properties

        private readonly uint _filterTimeMs;

        /// <summary>
        /// The filter time in milliseconds
        /// </summary>
        public uint FilterTimeMs
        {
            get { return _filterTimeMs; }
        }

        #endregion

        #region Constructors

        public IsRecordableWithFilterAttribute(uint filterTimeMs)
            : this("", filterTimeMs)
        {
        }

        public IsRecordableWithFilterAttribute(string description = "", uint filterTimeMs = 1000)
            : base(description)
        {
            _filterTimeMs = filterTimeMs;
        }

        #endregion

    }
}