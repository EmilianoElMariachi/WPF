namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes
{
    public class IsRecordableWithFilterAttribute : IsRecordableAttribute
    {
        private readonly uint _filterTimeMs;

        public IsRecordableWithFilterAttribute(uint filterTimeMs)
            : this("", filterTimeMs)
        {
        }

        public IsRecordableWithFilterAttribute(string description = "", uint filterTimeMs = 1000)
            : base(description)
        {
            _filterTimeMs = filterTimeMs;
        }

        public uint FilterTimeMs
        {
            get { return _filterTimeMs; }
        }
    }
}