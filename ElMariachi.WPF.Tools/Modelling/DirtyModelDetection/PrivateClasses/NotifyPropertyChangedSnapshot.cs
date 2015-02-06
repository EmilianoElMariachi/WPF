using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.Attributes;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{
    internal class NotifyPropertyChangedSnapshot : SnapshotElement
    {

        #region Fiels & Properties

        private readonly Dictionary<string, KeyValuePair<PropertyInfo, SnapshotElement>> _propertiesSnapshots = new Dictionary<string, KeyValuePair<PropertyInfo, SnapshotElement>>();

        private INotifyPropertyChanged _listenedNotifyPropertyChanged;

        #endregion

        #region Constructors

        public NotifyPropertyChangedSnapshot(DirtyModelDetector modelDetector, INotifyPropertyChanged snapshotValue)
            : base(modelDetector, snapshotValue)
        {
            if (snapshotValue == null)
            {
                throw new ArgumentNullException("snapshotValue");
            }

            foreach (var propertyInfo in snapshotValue.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            {
                var getMethod = propertyInfo.GetGetMethod();
                if (getMethod != null && getMethod.GetParameters().Any())
                {
                    continue;
                }
                var attributes = propertyInfo.GetCustomAttributes(typeof(IsSerialized), true);
                if (attributes.Length <= 0)
                {
                    continue;
                }

                var snapshot = SnaphotFactory.Create(modelDetector, propertyInfo.GetValue(snapshotValue, null));
                var keyValuePair = new KeyValuePair<PropertyInfo, SnapshotElement>(propertyInfo, snapshot);
                _propertiesSnapshots.Add(propertyInfo.Name, keyValuePair);

            }

            _listenedNotifyPropertyChanged = snapshotValue;
            _listenedNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;
        }

        #endregion

        #region Methods

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var changedPropertyName = propertyChangedEventArgs.PropertyName;

            if (_propertiesSnapshots.ContainsKey(changedPropertyName))
            {
                var keyValuePair = _propertiesSnapshots[changedPropertyName];
                keyValuePair.Value.OnValueChanged(keyValuePair.Key.GetValue(_listenedNotifyPropertyChanged, null));
            }
        }

        public override void ReleaseListeners()
        {
            if (_listenedNotifyPropertyChanged != null)
            {
                _listenedNotifyPropertyChanged.PropertyChanged -= OnPropertyChanged;
                _listenedNotifyPropertyChanged = null;
            }

            foreach (var propertiesSnapshot in _propertiesSnapshots)
            {
                propertiesSnapshot.Value.Value.ReleaseListeners();
            }
        }

        protected override SnapshotState CheckValueChanged(object newValue)
        {
            if (_listenedNotifyPropertyChanged != null && ReferenceEquals(_listenedNotifyPropertyChanged, newValue))
            {
                return this.State;
            }

            ReleaseListeners();

            var newValueAsINotifyPropertyChanged = newValue as INotifyPropertyChanged;
            if (newValueAsINotifyPropertyChanged == null ||
                newValueAsINotifyPropertyChanged.GetType() != this.SnapshotValue.GetType())
            {
                return SnapshotState.CHANGED;
            }

            Resynchronize(newValueAsINotifyPropertyChanged);

            return SnapshotState.UNCHANGED;
        }

        private void Resynchronize(INotifyPropertyChanged newValueAsINotifyPropertyChanged)
        {
            _listenedNotifyPropertyChanged = newValueAsINotifyPropertyChanged;
            _listenedNotifyPropertyChanged.PropertyChanged += OnPropertyChanged;

            foreach (var propertiesSnapshot in _propertiesSnapshots.Values)
            {
                var newSnapshotPropertyValue = propertiesSnapshot.Key.GetValue(_listenedNotifyPropertyChanged, null);
                propertiesSnapshot.Value.OnValueChanged(newSnapshotPropertyValue);
            }
        }

        #endregion

    }
}