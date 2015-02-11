using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ElMariachi.WPF.Tools.Modelling.ModelRecording.Attributes;
using ElMariachi.WPF.Tools.UndoRedo;
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

        internal NotifyPropertyChangedRecordedElement(IUndoRedoService undoRedoService, INotifyPropertyChanged objAsNotifyPropertyChanged)
            : base(objAsNotifyPropertyChanged)
        {
            if (undoRedoService == null) { throw new ArgumentNullException("undoRedoService"); }
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

                var property = PropertyFactory.Create(undoRedoService, objAsNotifyPropertyChanged, (IsRecordableAttribute)attributes[0], propertyInfo);

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

        private static class PropertyFactory
        {
            internal static Property Create(IUndoRedoService undoRedoService, INotifyPropertyChanged objAsNotifyPropertyChanged, IsRecordableAttribute isRecordableAttribute, PropertyInfo propertyInfo)
            {
                var recordableWithFilterAttribute = isRecordableAttribute as IsRecordableWithFilterAttribute;

                Property property;
                if (recordableWithFilterAttribute != null)
                {
                    property = new FilteredChangeRecordProperty(undoRedoService, objAsNotifyPropertyChanged, propertyInfo, RecordedElementFactory.Create(undoRedoService, propertyInfo.GetValue(objAsNotifyPropertyChanged, null)), recordableWithFilterAttribute);
                }
                else
                {
                    property = new EveryChangeRecordProperty(undoRedoService, objAsNotifyPropertyChanged, propertyInfo, RecordedElementFactory.Create(undoRedoService, propertyInfo.GetValue(objAsNotifyPropertyChanged, null)), isRecordableAttribute);
                }
                return property;
            }
        }

        private class EveryChangeRecordProperty : Property
        {

            #region Constructors

            internal EveryChangeRecordProperty(IUndoRedoService undoRedoService, INotifyPropertyChanged propertyOwnerObj, PropertyInfo propertyInfo, RecordedElement initialPropertyValue, IsRecordableAttribute isRecordableAttribute)
                : base(undoRedoService, propertyOwnerObj, propertyInfo, initialPropertyValue, isRecordableAttribute)
            {
            }

            #endregion

            #region Methods

            protected override void RecordValueChange(object newPropertyValue, object oldPropertyValue)
            {
                _undoRedoService.AddExecutedCommand(new ChangePropertyRevertibleCommand(_propertyOwnerObj, this.Name, newPropertyValue, oldPropertyValue, _isRecordableAttribute.Description));
            }

            #endregion

        }

        /// <summary>
        /// Represents a property for which value changes are added to the <see cref="IUndoRedoService"/> only when property value is stable
        /// </summary>
        private class FilteredChangeRecordProperty : Property
        {

            #region Fields & Properties

            private readonly UndoRedoDelayedExecutedRevertibleCommandAppender<object> _undoRedoDelayedExecutedRevertibleCommandAppender;

            #endregion

            #region Constructors

            internal FilteredChangeRecordProperty(IUndoRedoService undoRedoService, INotifyPropertyChanged propertyOwnerObj, PropertyInfo propertyInfo, RecordedElement initialPropertyValue, IsRecordableWithFilterAttribute isRecordableWithFilterAttribute)
                : base(undoRedoService, propertyOwnerObj, propertyInfo, initialPropertyValue, isRecordableWithFilterAttribute)
            {
                _undoRedoDelayedExecutedRevertibleCommandAppender = new UndoRedoDelayedExecutedRevertibleCommandAppender<object>(undoRedoService, _propertyOwnerObj, propertyInfo)
                {
                    DelayMs = isRecordableWithFilterAttribute.FilterTimeMs,
                    CommandDescription = _isRecordableAttribute.Description,
                };
            }

            #endregion

            #region Methods

            protected override void RecordValueChange(object newPropertyValue, object oldPropertyValue)
            {
                if (!_undoRedoDelayedExecutedRevertibleCommandAppender.IsFlushPending)
                {
                    _undoRedoDelayedExecutedRevertibleCommandAppender.OldValueToFlush = oldPropertyValue;
                }
                _undoRedoDelayedExecutedRevertibleCommandAppender.UpdatePropertyValue(newPropertyValue);
            }

            internal override void Release()
            {
                base.Release();
                _undoRedoDelayedExecutedRevertibleCommandAppender.Dispose();
            }

            #endregion

        }

        /// <summary>
        /// Represents an observed property of a <see cref="INotifyPropertyChanged"/>
        /// </summary>
        private abstract class Property
        {

            #region Fields & Properties

            protected readonly IUndoRedoService _undoRedoService;
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

            #endregion

            #region Constructors

            internal Property(IUndoRedoService undoRedoService, INotifyPropertyChanged propertyOwnerObj, PropertyInfo propertyInfo, RecordedElement initialPropertyValue, IsRecordableAttribute isRecordableAttribute)
            {
                if (undoRedoService == null) { throw new ArgumentNullException("undoRedoService"); }
                if (propertyOwnerObj == null) { throw new ArgumentNullException("propertyOwnerObj"); }
                if (propertyInfo == null) { throw new ArgumentNullException("propertyInfo"); }
                if (initialPropertyValue == null) { throw new ArgumentNullException("initialPropertyValue"); }
                if (isRecordableAttribute == null) { throw new ArgumentNullException("isRecordableAttribute"); }

                _undoRedoService = undoRedoService;
                _propertyOwnerObj = propertyOwnerObj;
                _propertyInfo = propertyInfo;
                _value = initialPropertyValue;
                _isRecordableAttribute = isRecordableAttribute;
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

                if (!_undoRedoService.IsUndoing && !_undoRedoService.IsRedoing)
                {
                    RecordValueChange(newPropertyValue, oldValue);
                }

                this.Value = RecordedElementFactory.Create(_undoRedoService, _propertyInfo.GetValue(_propertyOwnerObj, null));
            }

            /// <summary>
            /// Method called when the value of this property is changed, and when this change is not due to a looping effect of the <see cref="IUndoRedoService.Undo"/> or <see cref="IUndoRedoService.Redo"/> method call.
            /// The implementer is responsible adding the value change to the <see cref="IUndoRedoService"/>
            /// </summary>
            /// <param name="newPropertyValue"></param>
            /// <param name="oldPropertyValue"></param>
            protected abstract void RecordValueChange(object newPropertyValue, object oldPropertyValue);

            #endregion

        }

        #endregion

    }

}