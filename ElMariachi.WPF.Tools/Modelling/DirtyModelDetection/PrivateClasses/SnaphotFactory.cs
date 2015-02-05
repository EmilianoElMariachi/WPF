using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ElMariachi.WPF.Tools.Modelling.DirtyModelDetection.PrivateClasses
{
    internal static class SnaphotFactory
    {

        public static SnapshotElement Create(DirtyModelDetector dirtyModelDetector, object value)
        {

            var valueAsINotifyCollectionChanged = value as INotifyCollectionChanged;
            var valueAsIEnumerable = value as IEnumerable;
            if (valueAsINotifyCollectionChanged != null && valueAsIEnumerable != null)
            {
                return new ObservableCollectionSnapshot(dirtyModelDetector, valueAsINotifyCollectionChanged, valueAsIEnumerable);
            }
            else
            {
                var valueAsINotifyPropertyChanged = value as INotifyPropertyChanged;
                if (valueAsINotifyPropertyChanged != null)
                {
                    return new NotifyPropertyChangedSnapshot(dirtyModelDetector, valueAsINotifyPropertyChanged);

                }
                else
                {
                    return new EqualsComparatorSnapshot(dirtyModelDetector, value);
                }

            }

        }

    }
}