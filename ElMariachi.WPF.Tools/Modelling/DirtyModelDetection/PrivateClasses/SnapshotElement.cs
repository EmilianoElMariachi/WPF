using System;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{

    /// <summary>
    /// Represent the base class for a snapshot element.
    /// A snapshot element is used for maintaining the copy value of a property at a snapshot event
    /// </summary>
    internal abstract class SnapshotElement
    {

        #region Fields & Properties

        private readonly DirtyModelDetector _modelDetector;

        private readonly object _snapshotValue;

        public object SnapshotValue
        {
            get { return _snapshotValue; }
        }

        private SnapshotState _state = SnapshotState.UNCHANGED;

        internal SnapshotState State
        {
            get { return _state; }
            set
            {
                if (value != _state)
                {
                    _state = value;
                    if (_state == SnapshotState.CHANGED)
                    {
                        this._modelDetector.MarkPropertyAsChanged(this);
                    }
                    else
                    {
                        this._modelDetector.MarkPropertyAsUnchanged(this);
                    }
                }
            }
        }

        #endregion

        #region Methods

        internal SnapshotElement(DirtyModelDetector modelDetector, object snapshotValue)
        {
            if (modelDetector == null)
            {
                throw new ArgumentNullException("modelDetector");
            }
            _modelDetector = modelDetector;
            _snapshotValue = snapshotValue;
        }

        internal void OnValueChanged(object newValue)
        {
            this.State = CheckValueChanged(newValue);
        }

        public abstract void ReleaseListeners();

        protected abstract SnapshotState CheckValueChanged(object newValue);

        #endregion

    }
}
