using System;
using System.Collections.Generic;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.EventsDefinition;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection
{
    public class DirtyModelDetector : IDirtyModelDetector
    {

        #region Fields & Properties

        private readonly HashSet<SnapshotElement> _modifiedProperties = new HashSet<SnapshotElement>();
        private SnapshotElement _snapshotElement;

        private bool _isDirty;

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {

                if (_isDirty != value)
                {
                    _isDirty = value;
                    NotifyDirtyStateChanged(new DirtyStateChangedEventHandlerArgs(_snapshotElement == null ? null : _snapshotElement.SnapshotValue, _isDirty));
                }
            }
        }

        public bool IsStarted
        {
            get
            {
                return _snapshotElement != null;
            }
        }

        #endregion

        #region Events

        public event DirtyStateChangedEventHandler DirtyStateChanged;

        private void NotifyDirtyStateChanged(DirtyStateChangedEventHandlerArgs args)
        {
            var handler = DirtyStateChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Public Methods

        public void Start(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            CleanUp();

            _snapshotElement = SnaphotFactory.Create(this, obj);
        }

        public void Restart()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("No object currently observed");
            }

            Start(_snapshotElement.SnapshotValue);
        }

        public void Stop()
        {
            CleanUp();
        }

        #endregion

        #region Private Methods

        private void CleanUp()
        {
            if (_snapshotElement != null)
            {
                _snapshotElement.ReleaseListeners();
                _snapshotElement = null;
            }
            _modifiedProperties.Clear();
            IsDirty = false;
        }

        internal void MarkPropertyAsUnchanged(SnapshotElement propertySnapshot)
        {
            this._modifiedProperties.Remove(propertySnapshot);
            UpdateDirtyState();
        }

        internal void MarkPropertyAsChanged(SnapshotElement propertySnapshot)
        {
            this._modifiedProperties.Add(propertySnapshot);
            UpdateDirtyState();
        }

        private void UpdateDirtyState()
        {
            this.IsDirty = this._modifiedProperties.Count > 0;
        }

        #endregion

    }
}
