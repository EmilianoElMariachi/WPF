using System;
using System.Collections.Generic;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.EventsDefinition;
using ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection
{

    /// <summary>
    /// Detects if a model is dirty.
    /// A model is considered dirty if its serialized state is changed.
    /// 
    /// The serialized state reference is created at call of <see cref="Start"/> method.
    /// </summary>
    public class DirtyModelDetector : IDirtyModelDetector
    {

        #region Fields & Properties

        private readonly HashSet<SnapshotElement> _modifiedProperties = new HashSet<SnapshotElement>();
        private SnapshotElement _snapshotElement;

        private bool _isDirty;

        /// <summary>
        /// True if the observed model is dirty.
        /// False if no observed model or if model is not dirty
        /// </summary>
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

        /// <summary>
        /// Gets a boolean indicating whether an object is observed
        /// </summary>
        public bool IsStarted
        {
            get
            {
                return _snapshotElement != null;
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event fired when the <see cref="IsDirty"/> property is changed
        /// </summary>
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

        /// <summary>
        /// Makes a snapshot of the given object and starts dirty state detection
        /// </summary>
        /// <param name="obj"></param>
        public void Start(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            CleanUp();

            _snapshotElement = SnaphotFactory.Create(this, obj);
        }

        /// <summary>
        /// Restart the dirty state detection of the last observed object (see <see cref="Start"/> method) based on its current state
        /// </summary>
        public void Restart()
        {
            if (!IsStarted)
            {
                throw new InvalidOperationException("No object currently observed");
            }

            Start(_snapshotElement.SnapshotValue);
        }

        /// <summary>
        /// Stops observing changes of current object.
        /// (do not throws if no object is observed)
        /// </summary>
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
