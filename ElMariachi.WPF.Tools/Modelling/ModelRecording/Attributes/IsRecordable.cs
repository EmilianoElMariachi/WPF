using System;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes
{

    /// <summary>
    /// Use this attribute on properties to indicate that the property changes should be recorded.
    /// (See <see cref="IModelRecorder"/>)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsRecordableAttribute : Attribute
    {

        #region Fields & Properties

        private readonly string _description;

        public string Description
        {
            get { return _description; }
        }

        #endregion

        #region Constructors

        public IsRecordableAttribute(string description = "")
        {
            _description = description;
        }

        #endregion

    }
}