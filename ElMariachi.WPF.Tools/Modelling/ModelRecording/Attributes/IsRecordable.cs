using System;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes
{

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IsRecordableAttribute : Attribute
    {
        private readonly string _description;

        public string Description
        {
            get { return _description; }
        }

        public IsRecordableAttribute(string description = "")
        {
            _description = description;
        }

    }
}