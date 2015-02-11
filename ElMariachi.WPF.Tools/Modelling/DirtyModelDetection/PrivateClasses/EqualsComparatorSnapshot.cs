namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{

    /// <summary>
    /// Snapshot comparator based on Equals method
    /// </summary>
    internal class EqualsComparatorSnapshot : SnapshotElement
    {

        #region Constructors

        public EqualsComparatorSnapshot(DirtyModelDetector modelDetector, object snapshotValue)
            : base(modelDetector, snapshotValue)
        {
        }

        #endregion

        #region Methods

        public override void ReleaseListeners()
        {
        }

        protected override SnapshotState CheckValueChanged(object newValue)
        {
            return Equals(this.SnapshotValue, newValue) ? SnapshotState.UNCHANGED : SnapshotState.CHANGED;
        }

        #endregion

    }
}