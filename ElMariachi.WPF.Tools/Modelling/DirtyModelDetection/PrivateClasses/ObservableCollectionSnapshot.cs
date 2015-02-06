using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{
    internal class ObservableCollectionSnapshot : SnapshotElement
    {

        #region Fields & Properties

        private readonly ReadOnlyCollection<SnapshotElement> _itemsSnapshot;
        private INotifyCollectionChanged _listenedCollectionAsINotifyCollectionChanged;
        private IEnumerable _listenedCollectionAsIEnumerable;

        #endregion

        #region Constructors

        public ObservableCollectionSnapshot(DirtyModelDetector modelDetector,
            INotifyCollectionChanged iCollectionAsINotifyCollectionChanged, IEnumerable listenedCollectionAsIEnumerable)
            : base(modelDetector, iCollectionAsINotifyCollectionChanged)
        {
            if (iCollectionAsINotifyCollectionChanged == null)
                throw new ArgumentNullException("iCollectionAsINotifyCollectionChanged");
            if (listenedCollectionAsIEnumerable == null)
                throw new ArgumentNullException("listenedCollectionAsIEnumerable");
            if (!ReferenceEquals(iCollectionAsINotifyCollectionChanged, listenedCollectionAsIEnumerable))
                throw new Exception("Same instance expected");

            _listenedCollectionAsINotifyCollectionChanged = iCollectionAsINotifyCollectionChanged;
            _listenedCollectionAsIEnumerable = listenedCollectionAsIEnumerable;

            var itemsSnapshot = new List<SnapshotElement>();
            foreach (var item in _listenedCollectionAsIEnumerable)
            {
                itemsSnapshot.Add(SnaphotFactory.Create(modelDetector, item));
            }
            _itemsSnapshot = new ReadOnlyCollection<SnapshotElement>(itemsSnapshot);
            _listenedCollectionAsINotifyCollectionChanged.CollectionChanged += OnCollectionChanged;
        }

        #endregion

        #region Methods

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            //TODO ELM: (Optimisation) utiliser l'argument notifyCollectionChangedEventArgs pour gérer plus finement les changements
            this.State = CheckCollectionChanged();
        }

        public override void ReleaseListeners()
        {
            if (_listenedCollectionAsINotifyCollectionChanged != null)
            {
                _listenedCollectionAsINotifyCollectionChanged.CollectionChanged -= OnCollectionChanged;
                _listenedCollectionAsINotifyCollectionChanged = null;
            }
            _listenedCollectionAsIEnumerable = null;

            foreach (var snapshotElement in _itemsSnapshot)
            {
                snapshotElement.ReleaseListeners();
            }
        }

        private SnapshotState CheckCollectionChanged()
        {
            var collectionItemIndex = 0;
            foreach (var collectionItem in _listenedCollectionAsIEnumerable)
            {

                if (collectionItemIndex < _itemsSnapshot.Count)
                {
                    _itemsSnapshot[collectionItemIndex].OnValueChanged(collectionItem);
                }
                collectionItemIndex++;
            }
            if (collectionItemIndex != _itemsSnapshot.Count)
            {
                return SnapshotState.CHANGED;
            }
            else
            {
                return SnapshotState.UNCHANGED;
            }
        }

        protected override SnapshotState CheckValueChanged(object newValue)
        {
            if (_listenedCollectionAsINotifyCollectionChanged != null &&
                ReferenceEquals(_listenedCollectionAsINotifyCollectionChanged, newValue))
            {
                return this.State;
            }

            ReleaseListeners();

            var newValueAsINotifyCollectionChanged = newValue as INotifyCollectionChanged;
            var newValueAsIEnumerable = newValue as IEnumerable;

            if (newValueAsINotifyCollectionChanged == null || newValueAsIEnumerable == null ||
                newValue.GetType() != this.SnapshotValue.GetType())
            {
                return SnapshotState.CHANGED;
            }

            _listenedCollectionAsINotifyCollectionChanged = newValueAsINotifyCollectionChanged;
            _listenedCollectionAsIEnumerable = newValueAsIEnumerable;
            _listenedCollectionAsINotifyCollectionChanged.CollectionChanged += OnCollectionChanged;

            return CheckCollectionChanged();
        }

        #endregion

    }
}