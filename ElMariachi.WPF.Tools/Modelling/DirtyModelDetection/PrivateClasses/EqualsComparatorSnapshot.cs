namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{
    internal class EqualsComparatorSnapshot : SnapshotElement
    {
        public EqualsComparatorSnapshot(DirtyModelDetector modelDetector, object snapshotValue)
            : base(modelDetector, snapshotValue)
        {
        }

        public override void ReleaseListeners()
        {
        }

        protected override SnapshotState CheckValueChanged(object newValue)
        {
            return Equals(this.SnapshotValue, newValue) ? SnapshotState.UNCHANGED : SnapshotState.CHANGED;
        }
    }
}