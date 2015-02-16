using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes;
using ElMariachi.WPF.Tools.UndoRedo.RevertibleCommands.Helpers;

namespace ElMariachi.WPF.Tools.Modelling.ModelRecording.PrivateClasses
{
    internal class NotifyPropertyChangedRecordedElement : RecordedElement
    {

        #region Fields & Properties

        private readonly INotifyPropertyChanged _objAsNotifyPropertyChanged;
        private readonly Dictionary<string, Property> _properties = new Dictionary<string, Property>();

        #endregion

        #region Constructors

        internal NotifyPropertyChangedRecordedElement(IRecorderInterface recorderInterface, INotifyPropertyChanged objAsNotifyPropertyChanged)
            : base(objAsNotifyPropertyChanged)
        {
            if (recorderInterface == null) { throw new ArgumentNullException("recorderInterface"); }
            if (objAsNotifyPropertyChanged == null) { throw new ArgumentNullException("objAsNotifyPropertyChanged"); }

            _objAsNotifyPropertyChanged = objAsNotifyPropertyChanged;
            _objAsNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;

            foreach (var propertyInfo in objAsNotifyPropertyChanged.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod != null && getMethod.GetParameters().Any())
                {
                    continue;
                }
                var attributes = propertyInfo.GetCustomAttributes(typeof(IsRecordableAttribute), true);
                if (attributes.Length <= 0)
                {
                    continue;
                }

                var property = new Property(recorderInterface, objAsNotifyPropertyChanged, propertyInfo, (IsRecordableAttribute)attributes[0]);

                _properties.Add(property.Name, property);
            }

        }

        #endregion

        #region Methods

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (_properties.ContainsKey(propertyChangedEventArgs.PropertyName))
            {
                var property = _properties[propertyChangedEventArgs.PropertyName];

                property.UpdateValue();
            }
        }

        internal override void Release()
        {
            _objAsNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
            foreach (var property in _properties.Values)
            {
                property.Release();
            }
            _properties.Clear();
        }

        #endregion

        #region Private Classe(s)

        /// <summary>
        /// Represents an observed property of a <see cref="INotifyPropertyChanged"/>
        /// </summary>
        private class Property : IRecordedPropertyInfo
        {

            #region Fields & Properties

            protected readonly IRecorderInterface _recorderInterface;
            protected readonly INotifyPropertyChanged _propertyOwnerObj;
            protected readonly PropertyInfo _propertyInfo;
            protected readonly IsRecordableAttribute _isRecordableAttribute;

            private RecordedElement _value;

            /// <summary>
            /// The last observed value of this property
            /// </summary>
            internal RecordedElement Value
            {
                get { return _value; }
                private set
                {
                    if (_value != null)
                    {
                        _value.Release();
                    }
                    _value = value;
                }
            }

            /// <summary>
            /// The real property name
            /// </summary>
            internal string Name
            {
                get { return _propertyInfo.Name; }
            }

            public uint DelayMs
            {
                get;
                private set;
            }

            #endregion

            #region Constructors

            internal Property(IRecorderInterface recorderInterface, INotifyPropertyChanged propertyOwnerObj, PropertyInfo propertyInfo, IsRecordableAttribute isRecordableAttribute)
            {
                if (recorderInterface == null) { throw new ArgumentNullException("recorderInterface"); }
                if (propertyOwnerObj == null) { throw new ArgumentNullException("propertyOwnerObj"); }
                if (propertyInfo == null) { throw new ArgumentNullException("propertyInfo"); }
                if (isRecordableAttribute == null) { throw new ArgumentNullException("isRecordableAttribute"); }

                _recorderInterface = recorderInterface;
                _propertyOwnerObj = propertyOwnerObj;
                _propertyInfo = propertyInfo;
                _isRecordableAttribute = isRecordableAttribute;

                _value = RecordedElementFactory.Create(_recorderInterface, this, _propertyInfo.GetValue(_propertyOwnerObj, null));

                var recordableWithFilterAttribute = isRecordableAttribute as IsRecordableWithFilterAttribute;
                if (recordableWithFilterAttribute != null)
                {
                    this.DelayMs = recordableWithFilterAttribute.FilterTimeMs;
                }
                else
                {
                    this.DelayMs = 0;
                }

            }

            #endregion

            #region Methods

            internal virtual void Release()
            {

                if (_value != null)
                {
                    _value.Release();
                    _value = null;
                }
            }

            internal void UpdateValue()
            {

                var oldValue = this.Value.OldValue;
                var newPropertyValue = _propertyInfo.GetValue(_propertyOwnerObj, null);

                if (_recorderInterface.CanRecordPropertyChange)
                {
                    _recorderInterface.RecordPropertyChange(this, new ChangePropertyRevertibleCommand(_propertyOwnerObj, this.Name, newPropertyValue, oldValue, _isRecordableAttribute.Description));
                }

                this.Value = RecordedElementFactory.Create(_recorderInterface, this, _propertyInfo.GetValue(_propertyOwnerObj, null));
            }


            #endregion

        }

        #endregion

    }

}